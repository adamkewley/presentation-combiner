# ![Screenshot of Presentation Combiner](https://github.com/adamk117/ppt-combiner/raw/master/img/entire-icon.png) Presentation Combiner
*Combine multiple presentation files into one.*

![Screenshot of Presentation Combiner](https://github.com/adamk117/ppt-combiner/raw/master/img/app-screenshot.png)

`Presentation Combiner` automates Microsoft PowerPoint to combine presentation files into one output. It currently supports `.ppt`, `.pptx`, `.pptm`, `.ppsx`, `.ppsm`, `.pps`, and `.odp` files. `Presentation Combiner` will open a Microsoft PowerPoint window with the combined slides shown.

## Installation
`Presentation Combiner` is provided as a standalone portable `.exe`. Download the latest version from the releases link and get going! `Presentation Combiner` should work on any Windows computer with `.NET 4.0` or later. **Microsoft PowerPoint is required for `Presentation Combiner` to work!**

## Building
`Presentation Combiner` is built using Visual Studio 2013. Its only dependency, `Costura.Fody`, should be automatically acquired through NUGet when building. No additional software should be required to build. 

`Presentation Combiner` is built in both C# and F#. The main C# project, `PresentationCombiner`, contains most of the frontend source code and mainly handles UI wireup. The F# project, `PresentationCombiner.FS` contains back-end implementations and helper algorythms.

## Known Bugs and ToDo

### Known Bugs

 - Error messages due to invalid data are not passed from Microsoft PowerPoint to `PresentationCombiner`. If a corrupt presentation file is passed to Microsoft PowerPoint it will discontinue merging without good feedback to `PresentationCombiner`

### ToDo
Improvements are ongoing but have no strict schedule (hobby project, sorry).
 - Drag & drop items in the merge list. Items can't be moved at the moment.
 - Re-sort items in the merge list.
 - Slightly more comprehensive undo/redo listings
 - Implement better tooltips and error messages:
     - Tooltips on disabled buttons (explain why they are disabled).
     - Tidier error messages if the user doesn't have PowerPoint installed
     - Propagated error messages from PowerPoint
 - Better user feedback on added merge targets:
     - Use background worker PowerPoint process to scan number of slides, errors, etc. of added paths rather than checking them when pressing "Merge".
