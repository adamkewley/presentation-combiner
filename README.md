# PPTCombiner #
A console based JScript program that will combine multiple PowerPoint presentations into one. Useful for regular meetings were presenters just combine slides in a serial style. This project is inspired from the excellent <a href="http://code.google.com/p/powerpointjoin/">PPTJoin</a> (updated perl-based version <a href="https://github.com/richardsugg/PowerpointJoin">here</a>) and offers some extra functionality.

## Requirements ##
- The script may only by ran on a Windows operating system (uses CScript).
- PowerPoint must be installed

# Usage #
## Basic Usage ##
Basic usage of PPTCombiner involves copying it into a folder containing .ppt's you wish to combine and running it as a Windows clientside JScript file:

    CScript pptCombiner.js

The output will be a file named combined.ppt in the folder.

## Other usage ##
### Directory argument ###
PPTCombiner will accept a path to a directory as its argument, all presentation files in the specified directory will be combined into the combined.ppt output:

#### Absolute directory path ####

    CScript pptCombiner.js C:\MySlides
    
#### Relative directory path ####

    CScript pptCombiner.js some\relative\path
    
### Text file argument ###
A .txt file containing paths separated by newlines may also supplied. The paths may either be absolute or relative to the supplied .txt file:

    C:\An\Absolute\Path.ppt
    A_Relative_File.ppt
    \A\Relative\Path.ppt
    
The combined.ppt output will be in the current working directory unless otherwise stated.

# ToDo #
## Option flags ##
Flags shall be implemented to allow finer control over how PPTCombiner.js works. Currently scheduled for implementation are:
+ /O "some_path\some_filename.ppt" : Selects an output format
+ /I : Interactive mode. Allows user to more easily select the required combine method
+ /R : Recursive mode. Will attempt to combine all ppt found recursively through a folder structure.
+ /? : Show documentation.

## Standard input/output ##
There are plans for PPTCombiner to support StdIn and StdOut arguments:

    dir | CScript pptCombiner.js | start
    
## Enhanced text file support ##
The text file should also be able to contain directories to be merged; for example:

    C:\An\Absolute\Path\To\AFile.ppt
    A_Relative_File
    A_Relative_Directory
    C:\An\Absolute\Directory
