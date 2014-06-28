namespace PPTCombiner.FS

open System
open System.Collections.Specialized
open System.Collections.ObjectModel

// Copied from http://fssnip.net/dv
module ObservableCollection =

    (*
    A two way observablecollection is quite difficult to implement.
    We have two ObservableCollections, A<X> and B<Y>, and two mappers: f (X -> Y) and g (Y -> X).
    We can map elements in A<X> to B<Y> with f: f(x1) -> y1
    We can map elements in B<X> to A<Y> with g: f(y1) -> x1

    Desired Behaviors:
    - Any changes in both A<X> and B<Y> will be echoed via a CollectionChanged event for UI handling.
        - There are no 'invisible' changes that the UI might miss.
    - Any changes in A<X> will be mapped onto B<Y>.
        - e.g. adding element x1 to A<X> will result in y1 being added to B<Y>.
    - Any changes in B will be mapped onto A.
        - e.g. adding element y1 to B<Y> will result in x1 being added to A<X>.
    - Mapped changes will not be circular.
        - e.g. after adding x1 to A<X>:
            Intended:
                - y1 added to B<Y> via f(x1) through the mapping.
                - End
            Unintended:
                - y1 added to B<Y> via f(x1) through the mapping.
                - THEN: x1 added (again) to A<X> via g(f(x1)) through mapping.
                - THEN: y1 added (again) to B<Y> via f(g(f(x1))) through mapping.
                - Crashes (StackOverflow, thread gets eaten up traversing callbacks).

    Implementation Barriers:
    - During a 'real' change the mapping is active.
    - For mapping based changes, mapping is inactive.

    Solutions:

    --- A subscription (the thing doing the mapping) will disable its opposite subscription during changes.
        Issues:
            - Circular references required e.g.
                - A<X> needs access to B<Y> subscription to create its subscription so it may
                  disable it while it performs its mapping.
                - B<Y> needs access to A<X> subscription to create its subscription so it may
                  disable it while it performs its mapping.
                - Cant create either subscription without the other one already existing
        Workaround:
            - Mutable reference cell that points to the subscription.
                - A<X> uses the mutable subscription ref cell to B<Y>'s (not yet existient) subscription
                - Likewise for B<Y>.
                - After both subscriptions are established, the reference cells are occupied by them.

    --- Override standard ObservableCollection and manually sort out mappings (e.g. not via a subscription).
        Issues: 
            - Multiple methods must be overrriden (Add, Remove, RemoveAt, etc.)
            - Function signature must not accept a sourceOc with standard OC implementation. Both returned OCs are the derived type.
                e.g.
                    Original Signature:     oc1.TwoWayDynamicMap(f, g) -> ObservableCollection<Y>
                    New Signature:          CreatePairedCollections(f, g) -> ObservableCollection<X> * ObservableCollection<Y>
    
    *)

    let TwoWayDynamicMap (f : 'a -> 'b) (g : 'b -> 'a) (sourceOc : ObservableCollection<'a>) : ObservableCollection<'b> * IDisposable =

        let destinationOc = ObservableCollection<_>()

        // A handler for NotifyCollectionChangedEvent that will echo changes between oc1 and oc2 via a mapper
        let mirrorTo (oc2 : ObservableCollection<_>) mapper (isLocked : bool ref) (changes : NotifyCollectionChangedEventArgs) =
            if not (isLocked.Value) then
                match changes.Action with
                    | NotifyCollectionChangedAction.Add ->
                        changes.NewItems |> Seq.cast<_> |> Seq.iteri (fun index item ->
                            oc2.Insert(changes.NewStartingIndex + index, mapper item))

                    | NotifyCollectionChangedAction.Move ->
                        oc2.Move(changes.OldStartingIndex, changes.NewStartingIndex)

                    | NotifyCollectionChangedAction.Remove ->
                        oc2.RemoveAt(changes.OldStartingIndex)

                    | NotifyCollectionChangedAction.Replace -> 
                        changes.NewItems |> Seq.cast<_> |> Seq.iteri (fun index item ->
                            oc2.[changes.NewStartingIndex + index] <- mapper item)

                    | NotifyCollectionChangedAction.Reset -> oc2.Clear()

                    | _ -> failwith "Unexpected action!"

        // Create mutable reference cells that hold the locked status. 
        // If locked (e.g. xLocked = true), "mirrorTo x f xLocked yChanges" wont echo yChanges onto x.
        let sourceLocked = ref(false)
        let destinationLocked = ref(false)

        // Subscribe to source. Lock the destination from using the handler below to circularly map.
        let sourceSub = sourceOc.CollectionChanged.Subscribe(fun change ->
            do destinationLocked := true
            do mirrorTo destinationOc f sourceLocked change
            do destinationLocked := false)

        // Subscribe to destination. Lock the source from using the handler above to circularly map.
        let destinationSub = destinationOc.CollectionChanged.Subscribe(fun change ->
            do sourceLocked := true
            do mirrorTo sourceOc g destinationLocked change
            do sourceLocked := false)

        let removeLink = 
            { new IDisposable with
                member this.Dispose() = 
                    sourceSub.Dispose()
                    destinationSub.Dispose() }

        (destinationOc, removeLink)

    /// <summary>
    /// Create a derived ObservableCollection which mirrors changes in the source collection.
    /// </summary>
    /// <param name="f"></param>
    /// <param name="oc"></param>
    let map f (oc:ObservableCollection<'T>) =
        // Create a resulting collection based on current elements
        let res = ObservableCollection<_>(Seq.map f oc)
        // Watch for changes in the source collection
        oc.CollectionChanged.Add(fun change ->
            match change.Action with
            | NotifyCollectionChangedAction.Add ->
                // Apply 'f' to all new elements and add them to the result
                change.NewItems |> Seq.cast<'T> |> Seq.iteri (fun index item ->
                res.Insert(change.NewStartingIndex + index, f item))
            | NotifyCollectionChangedAction.Move ->
                // Move element in the resulting collection
                res.Move(change.OldStartingIndex, change.NewStartingIndex)
            | NotifyCollectionChangedAction.Remove ->
                // Remove element in the result
                res.RemoveAt(change.OldStartingIndex)
            | NotifyCollectionChangedAction.Replace -> 
                // Replace element with a new one (processed using 'f')
                change.NewItems |> Seq.cast<'T> |> Seq.iteri (fun index item ->
                res.[change.NewStartingIndex + index] <- f item)
            | NotifyCollectionChangedAction.Reset ->
                // Clear everything
                res.Clear()
            | _ -> failwith "Unexpected action!" )
        res

[<System.Runtime.CompilerServices.Extension>]
module OcExtensions = 
    /// <summary>
    /// Create a derived ObservableCollection which mirrors changes in the source collection.
    /// </summary>
    /// <param name="sourceObservableCollection">The source collection to mirror.</param>
    /// <param name="f">The mapping function.</param>
    [<System.Runtime.CompilerServices.Extension>]
    let DynamicMap(sourceObservableCollection, f) = ObservableCollection.map (FSharpFunc.FromConverter(f)) sourceObservableCollection

    [<System.Runtime.CompilerServices.Extension>]
    let TwoWayDynamicMap(sourceOc, f, g) = 
        let oc, dispose = ObservableCollection.TwoWayDynamicMap (FSharpFunc.FromConverter(f)) (FSharpFunc.FromConverter(g)) sourceOc
        Tuple<_,_>(oc, dispose)


// Simplified `guard` from http://stackoverflow.com/questions/3845110/observable-from-sequence-in-f
module Observable = 
    /// <summary>
    /// An observable that immediately produces a value upon subscription before subscribing
    /// to the underlying observable.
    /// </summary>
    /// <param name="x">The immediate value produced upon subscription to the observable.</param>
    let immediateValue x (e : IObservable<_>) = 
        { new IObservable<'T> with 
            member this.Subscribe(observer) = 
                observer.OnNext(x)
                e.Subscribe(observer) }
