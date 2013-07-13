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

//TODO: txt file parse, flags.

// Globals
var FILE_SYSTEM_OBJ = new ActiveXObject("Scripting.FileSystemObject");
var WSCRIPT_SHELL = WScript.CreateObject("WScript.Shell");

var FLAG_FILENAME = "combined.ppt";
var FLAG_PATH = WSCRIPT_SHELL.CurrentDirectory;

/**
 * Application entry point (main).
 */
{
    var namedArguments = WScript.Arguments.Named;
    var unnamedArguments = WScript.Arguments.Unnamed;

    //Handle flags (named arguments, e.g. /I) handle them before anything else.
    if(namedArguments.Count > 0){
        flagHandler(namedArguments);
    }

    //Handle arguments
    if (unnamedArguments.Count !== 0) {
        //If there are unnamed arguments handle each.
        for (var i = 0; i < unnamedArguments.Count; i++) {
            argumentHandler(unnamedArguments.Item(i));
        }
    } else {
        //If there's no arguments then attempt to merge ppt files in the current working directory.
        var slidePaths = handleDirectory(FILE_SYSTEM_OBJ.GetFolder(
            WSCRIPT_SHELL.CurrentDirectory
        ));

        //If there's files to combine then perform the combination.
        if (slidePaths.length > 0) {
            combine(slidePaths);
        }
        else {
            //TODO: Implement workflow for this
            throw new Error("NYI: No PPT files found, prompt user or show error.");
        }
    }

    WScript.quit(0);
}

//Interfacial functions

/**
 * Handles program arguments. Each argument can be either a path to a directory for combination or a txt file for parsing and combination.
 * @param argument A program argument (example1: path\to\list.txt example2: path\to\dir)
 */
function argumentHandler(argument){
    //Check if the argument has an extension and if it is a .txt
    var extension = getExtension(argument);
    if (extension === "txt"){
        if(FILE_SYSTEM_OBJ.FileExists(argument)){
            var files = txtListToArray(argument);
            //Perform combination
            combine(files);
        }
        else {
            //Txt file not found
        }
    }
    else if (extension.length === 0) {
        //If it is a directory that exists, get the pptFiles, filter out for ppt pptFiles, and combine them.
        if (FILE_SYSTEM_OBJ.FolderExists(argument)){
            var folder = FILE_SYSTEM_OBJ.GetFolder(argument);
            var pptFiles = [];

            //For each file in the directory
            var filesEnumerator = new Enumerator(folder.Files);
            for(; !filesEnumerator.atEnd(); filesEnumerator.moveNext()){
                var currentFile = filesEnumerator.item();
                //If it's a ppt file, keep it; otherwise, ignore.
                if(getExtension(currentFile) === "ppt"){
                    pptFiles.push(currentFile.Path);
                }
            }

            if (pptFiles.length > 0) {
                combine(pptFiles);
            }
            else {
                //No ppt files found.
            }
        }
        else {
            //Non existient folder, discontinue
        }
    }
    else {
        //Invalid argument
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
 * Convert a text file with file names on each line into an array containing the absolute path of each file (assuming they are in the same directory).
 * @param slideListPath An absolute path to a text file (in the format described).
 * @returns {Array} An array containing the absolute path for each file found in the txt argument.
 */
function txtListToArray(slideListPath){
    var returnValue = [];

    if (!FILE_SYSTEM_OBJ.FileExists(slideListPath)){
        throw new Error("File list txt does not exist!");
    }
    else {
        //Open the file
        var slideList = FILE_SYSTEM_OBJ.OpenTextFile(slideListPath, 1);

        //Get the directory of the file, we're looking in here for the filenames found in the txt file.
        var slideDirectory = FILE_SYSTEM_OBJ.GetParentFolderName(slideListPath) + "\\";

        //For each entry in the txt file, check if it exists (required), append the absolute path, push it onto the return value.
        while (!slideList.AtEndOfStream){
            var fileName = slideList.ReadLine();
            var fullPath = slideDirectory + fileName;

            if (!FILE_SYSTEM_OBJ.FileExists(fullPath))  {
                throw new Error("Slide file: " + fileName + "not found!");
            }
            else{
                returnValue.push(fullPath);
            }
        }

        return returnValue;
    }
}

/**
 * Validates the supplied txt file.
 * @param slideListPath Path to the txt file which contains file names
 * @returns {boolean} If the file was sucessfully validated.
 * @throws {Error} If validation fails.
 */
function validateFile(slideListPath) {
    //Blank check: No argument supplied
    if (slideListPath.length == 0) {
        throw new Error("No txt file argument supplied!");
    }
    //Extension check: Must be a .txt file.
    else if (slideListPath.substr(slideListPath.length - 3) != "txt") {
        throw new Error("The file argument supplied had an incorrect extension (expected: txt)");
    }
    //Existence check: The .txt file must actually exist.
    else if (!FILE_SYSTEM_OBJ.FileExists(slideListPath)) {
        throw new Error("The file argument supplied does not exist!");
    }
    //Verified, continue.
    else {
        return true;
    }
}

//Core functions

/**
 * Combine the supplied presentation files into one presentation.
 * @param presentations {Array} An array containing the absolute path (string) of presentations to be combined.
 */
function combine(presentations){
    //Open powerpoint, create a target document for combination, and show the window
    var powerPointApplication = new ActiveXObject("Powerpoint.Application");
    var target = powerPointApplication.Presentations.Add();
    powerPointApplication.Visible = true;

    //Perform the merge.
    for (var i = 0; i < presentations.length; i++){
        target.Slides.InsertFromFile(presentations[i], target.Slides.Count);
    }

    //Save & Close
    target.SaveAs(FLAG_PATH + "\\" + FLAG_FILENAME);
    target.Close();
    powerPointApplication.Quit();
}

//General functions

/**
 * Extracts the extension of a supplied path or filename.
 * @param filename {String} A filename (or path). May contain multiple dots with no problems
 * @returns {String} The extracted extension (e.g. bat, js, ppt)
 */
function getExtension(filename){
    //Extract extension regex
    var re = /(?:\.([^.]+))?$/;
    return re.exec(filename)[1];
}

/**
 * Gets all the ppt files in the specified directory.
 * @param directory {FileSystemObject.Folder} Folder object containing the ppts to be extracted
 * @returns {Array} A list of absolute paths to each ppt file found in the directory
 */
function handleDirectory(directory){
    var pptFiles = [];

    var filesEnumerator = new Enumerator(directory.Files);
    for(; !filesEnumerator.atEnd(); filesEnumerator.moveNext()){
        var currentFile = filesEnumerator.item();
        if (getExtension(currentFile) === "ppt"){
            pptFiles.push(currentFile.Path);
        }
    }

    return pptFiles;
}