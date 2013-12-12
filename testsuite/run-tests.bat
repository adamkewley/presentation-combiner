@echo off
REM Test variables
set TEMPDIR=C:\Temp\ppt-combiner-tests
set SCRIPTNAME=pptCombiner.js

REM Cleanup old test output, copy in dependencies (testdata and script)
rd %TEMPDIR% /S /Q
mkdir %TEMPDIR%
xcopy test-data "%TEMPDIR%\test-data" /e
copy "..\%SCRIPTNAME%" "%TEMPDIR%\%SCRIPTNAME%"

REM Navigate to test folder.
cd %TEMPDIR%

REM Begin tests!

REM Run normally, providing a relative path to the output.
set TESTDIR=test-1
mkdir %TESTDIR%
echo "Relative input directory, relative output directory" > %TESTDIR%\testdetails.txt
CScript %SCRIPTNAME% test-data\ /O:%TESTDIR%\output.ppt

REM Run with an absolute path argument, providing a relative path to the output.
set TESTDIR=test-2
mkdir %TESTDIR%
echo "Absolute input directory, relative output directory" > %TESTDIR%\testdetails.txt
CScript %SCRIPTNAME% %TEMPDIR%\test-data\ /O:%TESTDIR%\output.ppt

REM Run with an absolute path argument and an absolute output argument.
set TESTDIR=test-3
mkdir %TESTDIR%
echo "Absolute input directory, absolute output directory" > %TESTDIR%\testdetails.txt
CScript %SCRIPTNAME% %TEMPDIR%\test-data\ /O:%TESTDIR%\output.ppt

REM Run with relative input and relative output arguments. Recursive mode on.
set TESTDIR=test-4
mkdir %TESTDIR%
echo "Relative input directory, relative output directory, recursive mode on." > %TESTDIR%\testdetails.txt
CScript %SCRIPTNAME% test-data\ /O:%TESTDIR%\output.ppt /R

REM Run with a relative text file input, relative output directory.
set TESTDIR=test-5
mkdir %TESTDIR%
echo "Relative text file input, relative output directory" > %TESTDIR%\testdetails.txt
CScript %SCRIPTNAME% test-data\test-text-input.txt /O:%TESTDIR%\output.ppt /R

REM Run with a relative text file input, relative output directory, recursive mode on (for folder arguments in text file).
set TESTDIR=test-6
mkdir %TESTDIR%
echo "Relative text file input, relative output directory, recursive mode on (for folder arguments in text file)." > %TESTDIR%\testdetails.txt
CScript %SCRIPTNAME% test-data\test-text-input.txt /O:%TESTDIR%\output.ppt /R

REM Open the test directory to view the outputs.
%SystemRoot%\explorer.exe %TEMPDIR%