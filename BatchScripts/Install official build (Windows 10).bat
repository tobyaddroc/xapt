@echo off

powershell -Command Set-ExecutionPolicy -ExecutionPolicy Bypass -Scope Process -Force
powershell -File ..\PSScripts\Windows10Installation.ps1

echo Installed xApt for Windows 10
echo To get support with xApt, type xapt shell, in shell press enter

pause