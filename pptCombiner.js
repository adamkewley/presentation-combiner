/**
 * Copyright:
 * Lesser GPL License. Freely distribute as you please.
 *
 * Author: Adam Kewley
 * E-Mail: AdamK117@Gmail.com
 * Date: 10/06/2013
 *
 * Summary:
 *     Name: PPT Combiner
 *     Build: 1
 *
 *     Combines ppt presentation files together into one ppt. Useful for regular meetings were contributors just combine slides.
 *
 *     REQUIREMENT: POWERPOINT MUST BE INSTALLED FOR THIS TO WORK.
 *
 *     Input arguments:
 *     If no argument is supplied all ppt's in the working directory are merged into the output. Arguments are handled as described
 *     below. Multiple arguments are handled separately in this manner and the outputs are numbered after the first (e.g. combined.ppt, combined_2.ppt, combined_3.ppt).
 *         - A path is supplied. All ppt's in the supplied path are merged into the output.
 *         - A path to a txt file is supplied as an argument.
 *             - Contains relative (to the .txt) or absolute paths to .ppt files separated on new lines.
 *         - (NEEDS) StdIn etc.
 *
 *     Output:
 *          Combined .ppt file named combined.ppt (may be changed with flags). Files named combined.ppt will be ignored when this program is ran.
 *
 *     Flags:
 *         /O:"path\filename"
 *             Redirects combined output to the location specified
 *             Example: CScript PPTCombiner.js /O:"C:\CombineHere.ppt"
 *
 *         /N:"filename"
 *             Renames the combined.ppt file, overridden by /O flag.
 *             Example: CScript PPTCombiner.js /N:"CombinedSlides"
 *
 *         /I
 *             Interactive mode. Will inform user of the number of presentations being merged, prompt for output name etc.
 */

// Globals
var FILE_SYSTEM_OBJ = new ActiveXObject("Scripting.FileSystemObject");
var WSCRIPT_SHELL = WScript.CreateObject("WScript.Shell");
var WORKING_DIRECTORY = WSCRIPT_SHELL.CurrentDirectory;

var FLAG_FILENAME = "combined.ppt";

/**
 * Application entry point (main).
 */
{
    var namedArguments = WScript.Arguments.Named;
    var unnamedArguments = WScript.Arguments.Unnamed;

    //Handle flags (named arguments, e.g. /I /O "output\path\myOutputFile.ppt") handle them before anything else.
    if(namedArguments.Count > 0){
        flagHandler(namedArguments);
    }

    //Handle arguments
    if (unnamedArguments.Count !== 0) {
        //If there are unnamed arguments handle each.
        for (var i = 0; i < unnamedArguments.Count; i++) {
            handleArgument(unnamedArguments.Item(i));
        }
    } else {
        //If there's no arguments then attempt to merge ppt files in the current working directory.
        var slidePaths = getPresentations(FILE_SYSTEM_OBJ.GetFolder(
            WSCRIPT_SHELL.CurrentDirectory
        ));

        //If there's files to combine then perform the combination.
        if (slidePaths.length > 0) {
            combine(slidePaths);
        }
        else {
            throw new Error("NYI: No PPT files found, prompt user or show error.");
        }
    }

    WScript.quit(0);
}

//Interfacial functions

/**
 * Handle a program argument. An argument may be either a path to a directory or a txt file for parsing.
 * @param argument A program argument (example1: path\to\list.txt example2: path\to\dir)
 */
function handleArgument(argument){
    //Check if the argument has an extension and if it is a .txt
    var extension = getExtension(argument);
    if (extension === "txt"){
        if(FILE_SYSTEM_OBJ.FileExists(argument)){
            var textFile = FILE_SYSTEM_OBJ.GetFile(argument);
            var files = presentationTxtListToArray(textFile);
            //Perform combination
            combine(files);
        }
        else {
            throw new Error("Text file not found.");
        }
    }
    else if (extension.length === 0) {
        //If it is a directory that exists, get the pptFiles, filter out for ppt pptFiles, and combine them.
        if (FILE_SYSTEM_OBJ.FolderExists(argument)){
            var folder = FILE_SYSTEM_OBJ.GetFolder(argument);

            var pptFiles = getPresentations(folder);

            if (pptFiles.length > 0) {
                combine(pptFiles);
            }
            else {
                throw new Error("No presentations were found in the specified folder");
            }
        }
        else {
            throw new Error("The specified folder could not be found");
        }
    }
    else {
        throw new Error("Invalid argument detected.");
    }
}

/**
 * Handles program flags. Flags may have an effect on program globals.
 * @param flags {WScript.Arguments.Named} Application flags. (example: /? example2: /I)
 */
function flagHandler(flags){
    throw new Error("Flag handler not yet implemented!");
    //Flags to handle:
    //    /O: Output path
    //    /N: Filename
    //    /I: Interactive mode
    //    /?: Help dump
}

/**
 * Convert a text file containing file names or paths on each entry into an array of FileSystemObject.File's. The root folder is the folder the txt file is contained within.
 * @param textFile {FileSystemObject.File} A text file for parsing.
 * @returns {Array} An array of FileSystemObject.File's for each presentation found.
 */
function presentationTxtListToArray(textFile){
    var presentations = [];

    //Reopen the textfile as a stream, keep a reference to the directory it was contained in.
    var slideList = FILE_SYSTEM_OBJ.OpenTextFile(textFile.Path, 1);
    var directory = textFile.ParentFolder;

    //For each entry in the txt file, check if it exists (required), append the absolute path, push it onto the return value.
    while (!slideList.AtEndOfStream){
        var entry = slideList.ReadLine();

        //If it's an absolute path
        if(FILE_SYSTEM_OBJ.FileExists(entry)){
             presentations.push(FILE_SYSTEM_OBJ.GetFile(entry));
        }
        else if(FILE_SYSTEM_OBJ.FileExists(directory.Path + entry)){
            presentations.push(FILE_SYSTEM_OBJ.GetFile(directory.Path + entry));
        }
        else {
            throw new Error("Could not parse a line");
        }
    }

    return presentations;
}

//Core functions

/**
 * Combine the supplied presentation files into one presentation.
 * @param presentations {Array} An array containing FileSystemObject.File's
 */
function combine(presentations){
    //Open powerpoint, create a target document for combination, and show the window
    var powerPointApplication = new ActiveXObject("Powerpoint.Application");
    var target = powerPointApplication.Presentations.Add();
    powerPointApplication.Visible = true;

    //Perform the merge.
    for (var i = 0; i < presentations.length; i++){
        target.Slides.InsertFromFile(presentations[i].Path, target.Slides.Count);
    }

    //Save & Close
    target.SaveAs(WORKING_DIRECTORY + "\\" + FLAG_FILENAME);
    target.Close();
    powerPointApplication.Quit();
}

//General functions

/**
 * Extract the extension of a supplied path or filename.
 * @param filename {String} A filename (or path). May contain multiple dots with no problems
 * @returns {String} The extracted extension (e.g. bat, js, ppt)
 */
function getExtension(filename){
    //Extract extension regex
    var re = /(?:\.([^.]+))?$/;
    return re.exec(filename)[1];
}

/**
 * Check if a file may be merged by this program.
 * @param file {FileSystemObject.File} The file to check.
 * @returns {Boolean} If the file may be merged.
 */
function canMerge(file){
    var extension = getExtension(file.Name);
    switch(extension){
        case "ppt":
            return true;
        case "pptx":
            return true;
        default:
            return false;
    }
}

/**
 * Get all the ppt files in the specified directory.
 * @param directory {FileSystemObject.Folder} Folder object containing the presentations to be extracted
 * @returns {Array} An array of FileSystemObject.File's for presentations found.
 */
function getPresentations(directory){
    var pptFiles = [];

    var filesEnumerator = new Enumerator(directory.Files);
    for(; !filesEnumerator.atEnd(); filesEnumerator.moveNext()){
        var currentFile = filesEnumerator.item();
        if(canMerge(currentFile)){
            pptFiles.push(currentFile);
        }
    }

    return pptFiles;
}

var HELP_TEXT = "Not yet implemented!";