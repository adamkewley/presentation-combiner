PPTCombiner
===========

Combine multiple PowerPoint presentations into one. Useful for regular meetings were presenters just combine slides in a serial style.

Requirements
------------

This project uses ActiveX objects to open PowerPoint. Because of this, PowerPoint must be installed in order to run PPTCombiner.

Usage
=====

## Basic Usage ##

Basic usage of PPTCombiner involves copying it into a folder containing .ppt's you wish to combine and running it as a Windows clientside JScript file:

CScript pptCombiner.js

The result will be a combined powerpoint file: combined.ppt

## Other arguments ##

### Directory argument ###
PPTCombiner will accept a directory as its argument, all files will be combined in that directory and the output will be the current working directory:

CScript pptCombiner.js C:\MySlides

### Text file argument ###
A text file containing the ppt's to combine may also be supplied:

-Example text file-

C:\An\Absolute\Path.ppt
A_Relative_File.ppt
\A\Relative\Path.ppt

-End text file-



