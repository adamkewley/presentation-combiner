namespace PresentationCombiner.FS

open System
open System.Collections.Generic
open System.Collections.Specialized
open System.Collections.ObjectModel

module internal ObservableCollection =
    /// <summary>
    /// A handler for an ObservableCollection.CollectionChanged event that will echo the changes 
    /// into the destination. Applying a converter (f) to each item that is mapped across.
    /// </summary>
    /// <param name="f"></param>
    /// <param name="e"></param>
    /// <param name="destination"></param>
    let MapChangesToList (f : 'a -> 'b) (changes : NotifyCollectionChangedEventArgs) (target : IList<'b>) =
        match changes.Action with
        | NotifyCollectionChangedAction.Add ->
            changes.NewItems |> Seq.cast<_> |> Seq.iteri (fun i item ->
                target.Insert(changes.NewStartingIndex + i, f item))
        | NotifyCollectionChangedAction.Move ->
            let oldIndex = changes.OldStartingIndex
            let item = target.[oldIndex]
            target.RemoveAt(oldIndex)
            target.Insert(changes.NewStartingIndex, item)
        | NotifyCollectionChangedAction.Remove ->
            target.RemoveAt(changes.OldStartingIndex)
        | NotifyCollectionChangedAction.Replace -> 
            changes.NewItems |> Seq.cast<_> |> Seq.iteri (fun i item ->
                target.[changes.NewStartingIndex + i] <- f item)
        | NotifyCollectionChangedAction.Reset -> target.Clear()
        | _ -> failwith "Unexpected action"

    // TODO: Documentation.
    let MapChangesToGenericList (f : 'a -> 'b) (changes : NotifyCollectionChangedEventArgs) (target : System.Collections.IList) =
        match changes.Action with
        | NotifyCollectionChangedAction.Add ->
            changes.NewItems |> Seq.cast<_> |> Seq.iteri (fun i item ->
                target.Insert(changes.NewStartingIndex + i, f item))
        | NotifyCollectionChangedAction.Move ->
            let oldIndex = changes.OldStartingIndex
            let item = target.[oldIndex]
            target.RemoveAt(oldIndex)
            target.Insert(changes.NewStartingIndex, item)
        | NotifyCollectionChangedAction.Remove ->
            target.RemoveAt(changes.OldStartingIndex)
        | NotifyCollectionChangedAction.Replace -> 
            changes.NewItems |> Seq.cast<_> |> Seq.iteri (fun i item ->
                target.[changes.NewStartingIndex + i] <- f item)
        | NotifyCollectionChangedAction.Reset -> target.Clear()
        | _ -> failwith "Unexpected action"

    /// <summary>
    /// Map an observable collection onto a new observable collection via a mapping function.
    /// Any changes in the source collection will be echoed into the collection.
    /// </summary>
    /// <param name="f">A mapping function between the source and the output</param>
    /// <param name="source">The source ObservableCollection to mirror.</param>
    let DynamicMap (f : 'a -> 'b) (source : ObservableCollection<'a>) : ObservableCollection<'b> * IDisposable =
        let destination = ObservableCollection<'b>()

        let subscription = source.CollectionChanged.Subscribe(fun changes ->
            MapChangesToList f changes destination)

        destination, subscription

    /// <summary>
    /// Map a source ObservableCollection onto a new ObservableCollection via a mapping function.
    /// Changes in both the source and destination ObservableCollection's will echo onto eachover.
    /// </summary>
    /// <param name="f">Source to destination mapping function.</param>
    /// <param name="g">Destination to source mapping function.</param>
    /// <param name="source">The source ObservableCollection to map.</param>
    let TwoWayDynamicMap (f : 'a -> 'b) (g : 'b -> 'a) (source : ObservableCollection<'a>) : ObservableCollection<'b> * IDisposable =

        let destination = ObservableCollection<'b>()

        (* Mutable lock that both source and destination close over in their subscriptions.
           They will check the lock before updating and will not update if the lock is true.
           The lock is active when one of the paired collections is being updated to prevent 
           the opposite collection responding to the updates in a circular fashion. *)
        let locked = ref(false)

        let attachHandler f (collectionA : ObservableCollection<_>) (collectionB : ObservableCollection<_>) =
            collectionA.CollectionChanged.Subscribe(fun e ->
                if not locked.Value then
                    locked := true                      
                    MapChangesToList f e collectionB
                    locked := false)

        let sourceSubscription = attachHandler f source destination
        let destinationSubscription = attachHandler g destination source

        let subscriptions = 
            { new IDisposable with
                member this.Dispose() = 
                    sourceSubscription.Dispose()
                    destinationSubscription.Dispose() }

        destination, subscriptions

[<System.Runtime.CompilerServices.Extension>]
module OcExtensions = 
    /// <summary>
    /// A handler for ObservableCollection CollectionChanged event.
    /// </summary>
    /// <param name="f"></param>
    /// <param name="e"></param>
    /// <param name="destination"></param>
    let MapChangesToListT(mapper, changes, targetList) =
        ObservableCollection.MapChangesToList (FSharpFunc.FromConverter(mapper)) changes targetList

    // TODO
    let MapChangesToList(mapper, changes, targetList)=
        ObservableCollection.MapChangesToGenericList (FSharpFunc.FromConverter(mapper)) changes targetList

    /// <summary>
    /// Create a derived ObservableCollection which mirrors changes in the source collection.
    /// </summary>
    /// <param name="sourceObservableCollection">The source collection to mirror.</param>
    /// <param name="f">The mapping function.</param>
    [<System.Runtime.CompilerServices.Extension>]
    let DynamicMap(sourceObservableCollection, mapper) = 
        let oc, dispose = ObservableCollection.DynamicMap (FSharpFunc.FromConverter(mapper)) sourceObservableCollection
        Tuple<_,_>(oc, dispose)

    // TODO
    [<System.Runtime.CompilerServices.Extension>]
    let TwoWayDynamicMap(sourceObservableCollection, forwardMapper, backwardMapper) = 
        let oc, dispose = ObservableCollection.TwoWayDynamicMap (FSharpFunc.FromConverter(forwardMapper)) (FSharpFunc.FromConverter(backwardMapper)) sourceObservableCollection
        Tuple<_,_>(oc, dispose)


// Simplified `guard` from http://stackoverflow.com/questions/3845110/observable-from-sequence-in-f
module Observable = 
    /// <summary>
    /// An observable that immediately produces a value upon subscription before subscribing
    /// to the underlying observable.
    /// </summary>
    /// <param name="x">The immediate value produced upon subscription to the observable.</param>
    let immediate x (e : IObservable<_>) = 
        { new IObservable<'T> with 
            member this.Subscribe(observer) = 
                observer.OnNext(x)
                e.Subscribe(observer) }
    
