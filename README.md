# PPTCombiner #

A console based, JScript, program that will combine multiple PowerPoint presentations into one. Useful for regular meetings were presenters just combine slides in a serial style. This project is inspired from <a href="http://code.google.com/p/powerpointjoin/">PPTJoin</a> (updated perl-based version <a href="https://github.com/richardsugg/PowerpointJoin">here</a>) and provides extra functionality that may not be available from these excellent projects.

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
PPTCombiner will accept a directory as its argument, all files will be combined in that directory and the output will be the current working directory:

#### Absolute directory path ####

    CScript pptCombiner.js C:\MySlides
    
#### Relative directory path ####

    CScript pptCombiner.js some\relative\path
    
### Text file argument ###
A text file containing the ppt's to combine may also be supplied:

    C:\An\Absolute\Path.ppt
    A_Relative_File.ppt
    \A\Relative\Path.ppt
    
In all cases, the combined.ppt output will always be in the current working directory.

# ToDo #
Flags shall be implemented to allow finer control over how PPTCombiner.js works. Currently scheduled for implementation are:
+ /O "some_path\some_filename.ppt" : Selects an output format
+ /I : Interactive mode. Allows user to more easily select the required combine method
+ /R : Recursive mode. Will attempt to combine all ppt found recursively through a folder structure.
+ /? : Show documentation.
