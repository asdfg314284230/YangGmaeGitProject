del .\out\cpp\* /q
del .\out\csharp\* /q
del .\out\dat\*  /q
del .\out\lua\*  /q

.\tools\xproto\XProto.exe

del ..\..\PlantsVsZombies\Assets\Core\Scripts\Core\CfgModel\* /q
del ..\..\PlantsVsZombies\Assets\StreamingAssets\GameConfig\*.dat /q

copy .\out\csharp\*.cs   ..\..\PlantsVsZombies\Assets\Core\Scripts\Core\CfgModel\
copy .\out\dat\*.dat     ..\..\PlantsVsZombies\Assets\StreamingAssets\GameConfig\

pause