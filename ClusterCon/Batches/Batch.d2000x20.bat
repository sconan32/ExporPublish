@echo off

echo Expor Batch Files
echo @copyleft Socona
echo .
FOR  %%I IN (..\configs\*.txt) DO ..\expor.exe -file %%I > ..\%%~nI.out.txt
echo Done.
pause > nul