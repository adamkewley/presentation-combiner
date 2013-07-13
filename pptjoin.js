// Set this to 1 to see lots of output
verbose = 0;

// Please modify STRINGS for your language
var STRINGS = {
	APP_TITLE : 'Powerpoint Join',
	COMBINED_CHARTS_FILENAME : "combined.pptx",
	PATH_TO_EXPLORER : "c:\\windows\\explorer.exe"
};
// end of STRINGS

// Main Program
debug("+-----------------------------------------------------+");
if(WScript.Arguments.length != 1) {
  m =  "Usage: pptjoin.js <filename>\n";
  m += "<filename> = text file listing charts to join";
  WScript.Echo(m);
  WScript.quit(1);
}
var filename = WScript.Arguments.Item(0)
if(validateFile(filename)) {
  processFiles();
}

debug("+-----------------------------------------------------+");
WScript.quit(0);


function validateFile() {
	debug("validating " + filename);
	if(filename.substr(filename.length - 3) == "txt") {
		debug("Selected Text file = " + filename);
    return true;
	} else if(filename.length == 0) {
    debug("No filename was given");
		return false;
	} else {
    debug("Filename should end with .txt");
		return false;
	}
}

function processFiles() {
	debug("Reading from file " + filename);
	var fso = new ActiveXObject("Scripting.FileSystemObject");
	var objPpt;
	var pptMain;
	var all_is_well = true;
	try {
		objPpt = new ActiveXObject("Powerpoint.Application");
		objPpt.Presentations.Add();
		objPpt.Visible = true;
		debug("PowerPoint.Application");
		var iMainCharts = 0;
		debug("NewPresentation");
		if(fso.FileExists(filename)) {
			var txt = fso.OpenTextFile(filename, 1);
			debug("OpenTextFile " + filename);
			var fullpath = fso.GetAbsolutePathName(filename);
			var directory = fso.GetParentFolderName(fullpath);
      debug("Directory " + directory);
			var pptMainFilename = directory + "\\" + STRINGS.COMBINED_CHARTS_FILENAME;
			debug("pptMainFilename = " + pptMainFilename);
			var firstppt = txt.ReadLine();
			debug("First ppt = " + firstppt);
			if(! fso.FileExists(firstppt) ) {
				firstppt = directory + "\\" + firstppt;
				if( ! fso.FileExists(firstppt) ) {
					debug("The first set of charts cannot be found");
					return;
				}
			}
			fso.CopyFile(firstppt, pptMainFilename);
			pptMain = objPpt.Presentations.Open(pptMainFilename);
			debug("Open");

			//pptMain.SaveAs(pptMainFilename);
			if(! fso.FileExists(pptMainFilename) ) {
				debug("Did not save correctly");
				all_is_well = false;
				return;
			} else {
				debug("Saved");
			}
			while (! txt.AtEndOfStream) {
				try {
					var line = txt.ReadLine();
					debug("Reading line " + line);
					var ppt = line;
					if(! fso.FileExists(ppt)) {
						debug("  it ain't " + ppt);
						if(! fso.FileExists(directory + "\\" + ppt) ) {
							debug("Charts " + ppt + " not found");
							break;
						} else {
							ppt = directory + "\\" + ppt;
						}
					}
					debug("  Found ppt '" + ppt + "'");
					iMainCharts = pptMain.Slides.Count;
					debug("iMainCharts = " + iMainCharts);
					debug("Inserting " + ppt);
					//pptMain.Slides.InsertFromFile(directory + "\\" + ppt, iMainCharts);
					pptMain.Slides.InsertFromFile(fso.GetAbsolutePathName(ppt), iMainCharts);
					debug("InsertFromFile");
				} catch (e) {
					debug("Oh No!")
          print_exception(e);
					all_is_well = false;
					break;
				}
			}
			if(all_is_well) {
				debug("File " + STRINGS.COMBINED_CHARTS_FILENAME + " is ready");
			}
			debug("Finished");
			txt = null;
		} else {
			debug("Can't find this file:  " + filename);
		}
	} catch (e) {
		debug("OH NO! \n" + e.description + "\n");
    WScript.quit(0);
	} 
	try {
		pptMain.Save();
		pptMain.Close();
		debug("closed");
		objPpt.Quit();
	} catch (e) {
		debug("Error cleaning up:  " + e.description);
	} finally {
		pptMain = null;
		objPpt = null;
		fso = null;
	}
}

function openCharts() {
	var pptcharts = STRINGS.COMBINED_CHARTS_FILENAME;
	var fso = new ActiveXObject("Scripting.FileSystemObject");
	var fullpath = fso.GetAbsolutePathName(filename);
	var directory = fso.GetParentPathName(fullpath);
	var found_charts = false;
	if(fso.FileExists(pptcharts)) {
		found_charts = true;
	} else {
		pptcharts = directory + "\\" + pptcharts;
		if(fso.FileExists(pptcharts) ) {
			found_charts = true;
		}
	}

	if(found_charts) {
		debug("opening " + pptcharts);
		var objPpt = new ActiveXObject("Powerpoint.Application");
		objPpt.Presentations.Add();
		objPpt.Visible = true;
		objPpt.Presentations.Open(pptcharts);

	}  else {
		debug("Still can't find " + pptcharts);
		debug("Can't find " + pptcharts);
		openFolder();
	}
	fso = null;
	sh = null;
}

function openFolder() {
	var fso = new ActiveXObject("Scripting.FileSystemObject");
	var fullpath = fso.GetAbsolutePathName(filename);
	var directory = fso.GetParentPathName(fullpath);
	fso = null;
	var sh = new ActiveXObject("WScript.Shell");
	var cmd = STRINGS.PATH_TO_EXPLORER + " " + directory;
	debug("Running " + cmd);
	sh.Run(cmd);
	sh = null;
}

function debug(msg) {
  if(verbose == 1) {
    WScript.echo(msg);
  }
}

function print_exception(e) {
  debug("+------- Exception --------");
  debug("| Name:    " + e.name);
  debug("| Raw No.: " + e.number);
  andno = e.number & 0xFFFF;
  debug("| & No.:   " + andno);
  debug("| Msg:     " + e.message);
  debug("| Desc.:   " + e.description);
  debug("+--------------------------");
}
