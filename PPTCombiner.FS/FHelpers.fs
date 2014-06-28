namespace PPTCombiner.FS

open System.Collections.Specialized
open System.Collections.ObjectModel
open System
open System.IO
open System.Reactive.Subjects

// Helper functions for UI etc.
module FHelpers = 
    /// <summary>
    /// Produces dynamic helper text that indicates the total number of slides found.
    /// </summary>
    /// <param name="addedFilesList"></param>
    let PathListToButtonText (addedFilesList : ObservableCollection<AddedPath>) = 

        let countTotalValidFiles = 
            Seq.map PathHelpers.ExtractMergeTargets
            >> Seq.concat 
            >> Seq.length

        addedFilesList.CollectionChanged 
        |> Observable.map (fun _ ->
                match countTotalValidFiles addedFilesList with
                | 0 -> "Nothing to merge."
                | 1 -> "Open one file."
                | x -> "Merge " + x.ToString() + " files.")
        |> Observable.immediateValue "Nothing to merge."

/// <summary>
/// A formatted AddedPath type for UI binding.
/// </summary>
type AddedPathView = 
    { Icon : string
      Name : string
      ValidFileCount : int
      AddedPath : AddedPath }

[<System.Runtime.CompilerServices.Extension>]
module FExtensionMethods = 
    /// <summary>
    /// Map an AddedPath type (model) to an AddedPathView (ui binding model).
    /// </summary>
    /// <param name="pth">The AddedPath to map.</param>
    let private addedPathtoAddedPathView pth = 
        let icon =  
            match pth.PathType with
            | ValidFile -> "img\\NewWindow_6277.png"
            | Folder -> "img\\folder_Open_32xLG.png"
            | InvalidFile | EmptyFolder | InvalidPath -> "img\\action_Cancel_16xLG.png"
        let name = Path.GetFileName(pth.AddedPath)

        let fileCount = match pth.PathType with
                        | InvalidFile -> 0
                        | _ -> pth.ValidFileCount

        { Icon = icon; Name = name; ValidFileCount = fileCount; AddedPath = pth }

    // C# extensions

    /// <summary>
    /// Map an AddedPath type (model) to an AddedPathView (ui binding model).
    /// </summary>
    /// <param name="pth">The AddedPath to map.</param>
    [<System.Runtime.CompilerServices.Extension>]
    let AddedPathtoAddedPathView(addedPath) =  addedPathtoAddedPathView addedPath