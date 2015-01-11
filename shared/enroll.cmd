@echo off
set /a x=0
goto :main

:process
set /a x+=1
::Fill in this line for your program
echo call yourProgram -a "user%x%" "%1"
goto :eof

:main
for /F "tokens=*" %%l in (Passwords.txt) do call :process %%l