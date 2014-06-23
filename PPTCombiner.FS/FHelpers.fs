module PPTCombiner.FS.FHelpers

open System.Collections.ObjectModel
open System
open System.IO
open System.Reactive.Subjects

    
// Produces dynamic helper text that indicates the total number of slides found.
let PathListToButtonText (addedFilesList : ObservableCollection<AddedPath>) : BehaviorSubject<string> = 

    let countTotalValidFiles =
        Seq.filter(fun addedPath -> addedPath.PathType <> PathType.InvalidFile)
        >> Seq.map(fun addedPath -> addedPath.FileCount)
        >> Seq.fold (+) 0

    let subject = new BehaviorSubject<string>("Nothing to merge.")

    addedFilesList.CollectionChanged 
    |> Observable.map (fun _ ->
            match countTotalValidFiles addedFilesList with
            | 0 -> "Nothing to merge."
            | 1 -> "Open one file."
            | x -> "Merge " + x.ToString() + " files.")
    |> fun observable -> observable.Subscribe(subject)
    |> ignore

    subject
