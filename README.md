# PPTCombiner #

A console based JScript program that will combine multiple PowerPoint presentations into one. Useful for regular meetings were presenters just combine slides in a serial style. This project is inspired from the excellent <a href="http://code.google.com/p/powerpointjoin/">PPTJoin</a> (updated perl-based version <a href="https://github.com/richardsugg/PowerpointJoin">here</a>) and offers some extra functionality.

## Requirements ##

- The script may only by ran on a Windows operating system (uses CScript).
- Microsoft PowerPoint must be installed

# Usage #

## Basic Usage ##

Basic usage of PPTCombiner involves copying it into a folder containing .ppt's you wish to combine and running it as a Windows clientside JScript file:

    CScript pptCombiner.js

The output will be a file named `combined.ppt` in the folder.

## Other usage ##

### Directory argument ###

PPTCombiner will accept a path to a directory as its argument, all presentation files in the specified directory will be combined into the `combined.ppt` output. There are several allowed input types:

#### Absolute directory path ####


    CScript pptCombiner.js C:\some\absolute\path
    
#### Relative directory path ####


    CScript pptCombiner.js some\relative\path
    
### Text file argument ###

A .txt file containing paths separated by newlines may also supplied. The paths may either be absolute or relative to the supplied .txt file:

    C:\An\Absolute\Path.ppt
    A_Relative_File.ppt
    \A\Relative\Path.ppt
    C:\An\Absolute\Directory
    A\Relative\Directory

## Flags ##

PPTCombiner currently has the following flags implemented:

+ /O[path] : Declare the output filename\path.

    - Specify a new filename "/o:"myNewCombinedFilename.ppt"
    - Specify an absolute path "/o:"C:\CombinedOutput.ppt"
    - Specify a relative path "/o: "some\subfolder\filename.ppt"

+ /? : View the in-terminal readme.
+ /R : Enable recursive mode. Folder arguments from all sources will be handled recursively by PPTCombiner (including folders specified in text files).
