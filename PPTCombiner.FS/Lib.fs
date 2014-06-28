namespace PPTCombiner.FS

open System
open System.Collections.Specialized
open System.Collections.ObjectModel

// Copied from http://fssnip.net/dv
module ObservableCollection =
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
