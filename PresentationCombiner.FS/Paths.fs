namespace PresentationCombiner.FS

/// <summary>
/// The type of path added by a user.
/// </summary>
type PathType = ValidFile | InvalidFile | Folder | EmptyFolder | InvalidPath

/// <summary>
/// A record that holds information about an added path.
/// </summary>
[<ReferenceEquality>]
type AddedPath = 
    { AddedPath : string
      PathType : PathType
      ValidFileCount : int 
      ContainedPaths : AddedPath seq }

module PathHelpers =
    open System
    open System.IO

    /// <summary>
    /// A list of valid merge targets that Microsoft PowerPoint can insert into other presentations.
    /// </summary>
    let ValidExtensions = 
        [ "*.ppt" 
          "*.pptx" 
          "*.pptm" 
          "*.ppsx" 
          "*.ppsm" 
          "*.pps" 
          "*.odp" ]

    /// <summary>
    /// Returns a sequence of valid files (presentations) found within a directory.
    /// </summary>
    /// <param name="directoryPath">The path to search.</param>
    let private GetValidFilesInDirectory (directoryPath : string) : string seq =

        let getValidFiles validExtension = 
            Directory.GetFiles(directoryPath, validExtension, SearchOption.TopDirectoryOnly)

        Seq.map getValidFiles ValidExtensions
        |> Seq.concat
        |> Seq.map (fun file -> FileInfo(file).FullName)
        |> Seq.distinct

    /// <summary>
    /// Returns true if the path provided is a file with a valid merge extension.
    /// </summary>
    /// <param name="filePath">The path of the file to check.</param>
    let IsAValidMergeTarget (filePath : string) : bool =

        let fileInfo = FileInfo(filePath)

        ValidExtensions
        |> Seq.filter (fun validExtension -> 
            ("*" + fileInfo.Extension) = validExtension) 
        |> Seq.isEmpty 
        |> not

    /// <summary>
    /// Find valid file(s) for merging at a path. Can be a directory or a filepath.
    /// </summary>
    /// <param name="path">The path to search for valid merge targets.</param>
    let rec PathToAddedPath (path : string) : AddedPath =
        let sanitizedPath = Path.GetFullPath(path)

        if Directory.Exists path then
            // It's a directory...
            let validFiles = GetValidFilesInDirectory sanitizedPath
            
            if validFiles = Seq.empty then 
                // ... but contains no valid files.
                { AddedPath = sanitizedPath
                  PathType = EmptyFolder
                  ValidFileCount = 0
                  ContainedPaths = Seq.empty }
            else
                // ... and contains at least one valid file.
                { AddedPath = sanitizedPath
                  PathType = Folder
                  ValidFileCount = Seq.length validFiles
                  ContainedPaths = Seq.map PathToAddedPath validFiles }

        else if File.Exists path then
            // It's a file...
            if IsAValidMergeTarget path then
                // ... which is valid.
                { AddedPath = sanitizedPath
                  PathType = ValidFile
                  ValidFileCount = 1
                  ContainedPaths = Seq.empty }
            else
                // ... which is invalid.
                { AddedPath = sanitizedPath
                  PathType = InvalidFile
                  ValidFileCount = 0
                  ContainedPaths = Seq.empty }
        else
            // It's neither a file or a directory. An invalid path.
            { AddedPath = sanitizedPath
              PathType = InvalidPath
              ValidFileCount = 0
              ContainedPaths = Seq.empty }

    /// <summary>
    /// Extract all merge targets from an added path.
    /// </summary>
    /// <param name="addedPath">The added path to extract merge targets from.</param>
    let rec ExtractMergeTargetPaths addedPath = seq {
        match addedPath.PathType with
        | ValidFile -> yield addedPath
        | Folder -> 
            for path in addedPath.ContainedPaths do
                yield! ExtractMergeTargetPaths path
        | _ -> ()
    }

    // C# Extensions

    /// <summary>
    /// Extract the paths of all merge targets in an added path.
    /// </summary>
    /// <param name="addedPath">The added path to extract merge targets from.</param>
    [<System.Runtime.CompilerServices.Extension>]
    let ExtractMergeTargets(addedPath) = ExtractMergeTargetPaths addedPath