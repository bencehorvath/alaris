@echo off
cls
java -jar "C:\Program Files (x86)\SableCC\lib\sablecc.jar" -d gen -t csharp alaris.sablecc
echo
pause
exit