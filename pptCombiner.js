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
 *
 *     Combines ppt presentation files together into one ppt. Useful for regular meetings were contributors just combine slides.
 *
 *     ***REQUIREMENT***: POWERPOINT MUST BE INSTALLED FOR THIS TO WORK.
 *
 *     Input arguments:
 *     If no argument is supplied all ppt's in the working directory are merged into the output. Arguments are handled as described
 *     below. Multiple arguments are handled separately in this manner and the outputs are numbered after the first (e.g. combined.ppt, combined_2.ppt, combined_3.ppt).
 *         - A path is supplied. All ppt's in the supplied directory are merged into the output.
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
 *
 *         /R
 *             Recursive mode. PPTCombine will attempt to find all ppts from the supplied directory root.
 *
 *         /?
 *             Show help documentation (this)
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
        //Handle each unnamed argument
        for (var i = 0; i < unnamedArguments.Count; i++) {
            handleArgument(unnamedArguments.Item(i));
        }
    } else {
        //Attempt to merge any ppt files in the current directory
        var slidePaths = getPresentations(FILE_SYSTEM_OBJ.GetFolder(
            WSCRIPT_SHELL.CurrentDirectory
        ));

        //If there's files to doCombine then perform the combination.
        if (slidePaths.length > 0) {
            doCombine(slidePaths);
        }
        else {
            //TODO: Handle this error properly.
            throw new Error("NYI: No PPT files found, prompt user or show error.");
        }
    }

    //TODO: StdOut, more informative message.
    WScript.Echo("Combination complete!")

    WScript.quit(0);
}

//Interfacial functions

/**
 * Handle a program argument. An argument may be either a path to a directory or a txt file for parsing.
 * @param argument {string} A program argument (example1: path\to\list.txt example2: path\to\dir)
 */
function handleArgument(argument){
    //Check if the argument has an extension and if it is a .txt
    var extension = getExtension(argument);

    if (extension === "txt"){
        if(FILE_SYSTEM_OBJ.FileExists(argument)){
            var textFile = FILE_SYSTEM_OBJ.GetFile(argument);
            var files = presentationTxtListToArray(textFile);
            //Perform combination
            doCombine(files);
        }
        else {
            throw new Error("Supplied text file not found.");
        }
    }
    else if (extension.length === 0) {
        //If it is a directory that exists, get the presentations, filter out for ppt presentations, and doCombine them.
        if (FILE_SYSTEM_OBJ.FolderExists(argument)){
            var folder = FILE_SYSTEM_OBJ.GetFolder(argument);

            var presentations = getPresentations(folder);

            if (presentations.length > 0) {
                doCombine(presentations);
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
    //    /R: Recursive mode
    //    /?: Help dump
}

/**
 * Convert a text file containing file names or paths on each entry into an array of FileSystemObject.File's. The root folder is the folder the txt file is contained within.
 * @param textFile {FileSystemObject.File} A text file for parsing.
 * @returns {Array.<FileSystemObject.File>} The files found in the txt document.
 */
function presentationTxtListToArray(textFile){
    var presentations = [];

    //Reopen the textfile as a stream, keep a reference to the directoryPath it was contained in.
    var slideList = FILE_SYSTEM_OBJ.OpenTextFile(textFile.Path, 1);
    var directoryPath = textFile.ParentFolder.Path;

    //For each entry check if it can be parsed directly as an absolute path; otherwise, try it as a relative path.
    while (!slideList.AtEndOfStream){
        var entry = slideList.ReadLine();
        var relativePath = directoryPath + "\\" + entry;

        //If it's an absolute path
        if(FILE_SYSTEM_OBJ.FileExists(entry)){
             presentations.push(
                 FILE_SYSTEM_OBJ.GetFile(entry)
             );
        }
        else if(FILE_SYSTEM_OBJ.FileExists(relativePath)){
            presentations.push(
                FILE_SYSTEM_OBJ.GetFile(relativePath)
            );
        }
        else {
            //TODO: Implement a proper error handler.
            throw new Error("Could not parse a line");
        }
    }

    return presentations;
}

//Core functions

/**
 * Combine the supplied presentation files into one presentation.
 * @param presentations {Array.<FileSystemObject.File>} The presentations to merge
 */
function doCombine(presentations){
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
 * @param filename {string} A filename (or path).
 * @returns {string} The extracted extension (e.g. bat, js, ppt)
 */
function getExtension(filename){
    //Extract extension regex
    var re = /(?:\.([^.]+))?$/;
    return re.exec(filename)[1];
}

/**
 * Returns <code>true</code> if the file may be merged by this program.
 * @param file {FileSystemObject.File} The file to check.
 * @returns {boolean} If the file may be merged.
 */
function canMerge(file){
    //TODO: Reimplement this as an array of allowed filenames (e.g. IndexOf)
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
 * Returns the presentations contained within the supplied directory.
 * @param directory {FileSystemObject.Folder} The directory to parse.
 * @returns {Array.<FileSystemObject.File>} The FileSystemObject's found.
 */
function getPresentations(directory){
    var presentations = [];

    var filesEnumerator = new Enumerator(directory.Files);

    forEachEnumerable(filesEnumerator, function(item){
        if(canMerge(item)){
            presentations.push(item);
        }
    });

    return presentations;
}

/**
 * Iterate over an Enumerator object, applying a function to each enumerated item.
 * @param enumerable {Enumerator} An enumerable object
 * @param func {function(Object)} A function to apply to each item in the Enumerator.
 */
function forEachEnumerable(enumerable, func){
    for(; !enumerable.atEnd(); enumerable.moveNext()){
        func(enumerable.item());
    }
}

var HELP_TEXT = "Not yet implemented!";