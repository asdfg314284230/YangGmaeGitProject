del .\out\cpp\* /q
del .\out\csharp\* /q
del .\out\dat\*  /q
del .\out\lua\*  /q

.\tools\xproto\XProto.exe

del ..\GameConfig\*.dat /q

copy .\out\dat\*.dat     ..\GameConfig\

pause