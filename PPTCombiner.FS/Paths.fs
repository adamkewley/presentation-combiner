namespace PPTCombiner.FS

type PathType = ValidFile | InvalidFile | Folder| EmptyFolder| InvalidPath

type AddedPath = { 
    AddedPath : string
    PathType : PathType
    FileCount : int 
    ContainedPaths : AddedPath seq option }

module PathHelpers =
    open System.Collections.ObjectModel
    open System
    open System.IO
    open System.Reactive.Subjects

    let ValidExtensions = 
        [ "*.ppt"
          "*.pptx" ]

    // Returns a sequence of valid files found within a directory.
    let GetValidFilesInDirectory (directoryPath : string) : string seq =

        let getValidFiles validExtension = 
            Directory.GetFiles(directoryPath, validExtension, SearchOption.TopDirectoryOnly)

        Seq.map getValidFiles ValidExtensions
        |> Seq.concat
        |> Seq.map (fun file -> FileInfo(file).FullName)
        |> Seq.distinct

    // Returns true if the path provided is a file with a valid merge extension.
    let IsAValidFile (filePath : string) : bool =

        let fileInfo = FileInfo(filePath)

        Seq.filter (fun validExtension -> fileInfo.Extension = validExtension) ValidExtensions
        |> Seq.isEmpty |> not

    // Find valid file(s) for merging at a path. Can be a directory or a filepath.
    let rec FindValidFilesInPath (path : string) : AddedPath =
        if Directory.Exists path then
            // It's a directory (could contain no files though).

            let validFiles = GetValidFilesInDirectory path
            
            if validFiles = Seq.empty then 
                // A directory containing no presentation files.
                { AddedPath = path
                  PathType = EmptyFolder
                  FileCount = 0
                  ContainedPaths = None }
            else
                // A directory containing at least one valid file.
                { AddedPath = path
                  PathType = Folder
                  FileCount = Seq.length validFiles
                  ContainedPaths = Some(Seq.map FindValidFilesInPath validFiles) }

        else if File.Exists path && IsAValidFile path then
            if IsAValidFile path then
                // A valid file.
                { AddedPath = path
                  PathType = ValidFile
                  FileCount = 1
                  ContainedPaths = None }
            else
                // An invalid file.
                { AddedPath = path
                  PathType = InvalidFile
                  FileCount = 1
                  ContainedPaths = None }
        else
            // An invalid path was added.
            { AddedPath = path
              PathType = InvalidFile
              FileCount = 0
              ContainedPaths = None }