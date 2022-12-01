@echo off

powershell -Command Set-ExecutionPolicy -ExecutionPolicy Bypass -Scope Process -Force
powershell -File ..\PSScripts\Windows11Installation.ps1

echo Installed xApt for Windows 11
echo To get support with xApt, type xapt shell, in shell press enter

pause