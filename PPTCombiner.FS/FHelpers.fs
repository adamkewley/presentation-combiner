namespace PPTCombiner.FS

open System.Collections.Specialized
open System.Collections.ObjectModel
open System
open System.IO

/// <summary>
/// A formatted AddedPath type for UI binding.
/// </summary>
type AddedPathView = 
    { Icon : string
      Name : string
      ValidFileCount : int
      AddedPath : AddedPath }

[<System.Runtime.CompilerServices.Extension>]
module FHelpers = 
    open System

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
        |> Observable.immediate "Nothing to merge."

    /// <summary>
    /// Map an AddedPath type (model) to an AddedPathView (ui binding model).
    /// </summary>
    /// <param name="pth">The AddedPath to map.</param>
    let internal addedPathtoAddedPathView pth = 

        let icon =  
            match pth.PathType with
            | ValidFile -> "img\\NewWindow_6277.png"
            | Folder    -> "img\\folder_Open_32xLG.png"
            | _         -> "img\\action_Cancel_16xLG.png"

        let name =
            match pth.PathType with
            | InvalidPath -> pth.AddedPath
            | _           -> Path.GetFileName(pth.AddedPath)

        { Icon = icon 
          Name = name 
          ValidFileCount = pth.ValidFileCount 
          AddedPath = pth }

    let DialogPaths =
        let allPaths =
            PathHelpers.ValidExtensions
            |> Seq.map (fun extension -> extension + ";")
            |> Seq.fold (+) ""
        String.Format("Presentation Files ({0})|{0}", allPaths)

    // C# extensions

    /// <summary>
    /// Map an AddedPath type (model) to an AddedPathView (ui binding model).
    /// </summary>
    /// <param name="pth">The AddedPath to map.</param>
    [<System.Runtime.CompilerServices.Extension>]
    let AddedPathtoAddedPathView(addedPath) =  addedPathtoAddedPathView addedPath
