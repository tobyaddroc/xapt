@echo off

cd ..\Build\Release\net6.0-windows10.0.19041.0

del C:\Windows\System32\xapt.deps.json > NUL
del C:\Windows\System32\xapt.dll > NUL
del C:\Windows\System32\xapt.exe > NUL
del C:\Windows\System32\xapt.pdb > NUL
del C:\Windows\System32\xapt.runtimeconfig.json > NUL

copy xapt.deps.json C:\Windows\System32 > NUL
copy xapt.dll C:\Windows\System32 > NUL
copy xapt.exe C:\Windows\System32 > NUL
copy xapt.pdb C:\Windows\System32 > NUL
copy xapt.runtimeconfig.json C:\Windows\System32 > NUL

echo Installed dev build from Build\Release\net6.0-windows10.0.19041.0

pause