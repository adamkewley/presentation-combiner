module PPTCombiner.FS.FHelpers

open System.Collections.ObjectModel
open System
open System.IO

let validExtensions = [ "*.ppt"; "*.pptx" ]

// Returns a sequence of valid files found within a directory.
let GetValidFilesInDirectory (directoryPath : string) : string seq =
    let getValidFiles validExtension = 
        Directory.GetFiles(directoryPath, validExtension, SearchOption.TopDirectoryOnly)
    Seq.map getValidFiles validExtensions
    |> Seq.concat
    |> Seq.map (fun file -> FileInfo(file).FullName)
    |> Seq.distinct

// Returns true if the path provided is a file with a valid merge extension.
let IsAValidFile (filePath : string) : bool =
    let fileInfo = FileInfo(filePath)
    Seq.filter (fun validExtension -> fileInfo.Extension = validExtension) validExtensions
    |> Seq.isEmpty
    |> not

// Find valid file(s) for merging at a path. Can be a directory or a filepath.
let FindValidFilesInPath (path : string) : string seq = seq {
    if Directory.Exists path then
        yield! GetValidFilesInDirectory path
    else if File.Exists path && IsAValidFile path then
        yield FileInfo(path).FullName
    else
        ArgumentOutOfRangeException("A string that isn't a file or a directory was passed in.")
        |> raise
}
    
// Produces dynamic helper text that indicates the total number of slides found.
let PathListToButtonText (pathList : ObservableCollection<string>) : IObservable<string> = 
    let countTotalValidFiles lst =
        Seq.map FindValidFilesInPath lst
        |> Seq.concat
        |> Seq.length
    pathList.CollectionChanged 
    |> Observable.map (fun _ ->
            match countTotalValidFiles pathList with
            | 0 -> "Nothing to merge."
            | 1 -> "Open one file."
            | x -> "Merge " + x.ToString() + " files."
    )
