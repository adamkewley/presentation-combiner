:: %cd% Gets current working directory
:: /b only gets files (with extns)
:: Type displays contents of a text file
:: findstr looks for a string in text file
::	/I Not case sensitive
::	/V Return all non-matched elements
::	/C: Search for a particular string
:: 	>> Output to file

set joinFolder=literature_slides

CD %cd%\%joinFolder%
dir /b | findstr /I /V /C:"list.txt" > list.txt

CD ..
::dir %joinfolder% /b | CScript pptCombiner.js
CScript pptCombiner.js "%cd%\%joinFolder%\list.txt"
pause