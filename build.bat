@rem Add path to MSBuild Binaries
setlocal

@rem preparing environment

@rem Initialize Visual Studio build environment:
@rem - Visual Studio 2017 Community/Professional/Enterprise is the preferred option
@rem - Visual Studio 2015 is the fallback option (which might or might not work)
@set tools=
@set tmptools="c:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\Tools\vsvars32.bat"
@if exist %tmptools% set tools=%tmptools%
@set tmptools="c:\Program Files (x86)\Microsoft Visual Studio\2017\Community\Common7\Tools\VsMSBuildCmd.bat"
@if exist %tmptools% set tools=%tmptools%
@set tmptools="c:\Program Files (x86)\Microsoft Visual Studio\2017\Professional\Common7\Tools\VsMSBuildCmd.bat"
@if exist %tmptools% set tools=%tmptools%
@set tmptools="c:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\Common7\Tools\VsMSBuildCmd.bat"
@if exist %tmptools% set tools=%tmptools%
@if not defined tools goto :error
call %tools% || exit /b 1


set cur_dir=%CD%

set SLNPATH=src\Asn1Net.Reader\Asn1Net.Reader.sln

IF EXIST .nuget\nuget.exe goto restore

:restore
IF EXIST packages goto run
.nuget\NuGet.exe restore %SLNPATH%

:run
@rem cleanin sln
msbuild %SLNPATH% /p:Configuration=Release /target:Clean || exit /b 1
@rem build desktop version (.NET 4.0)
msbuild %SLNPATH% /p:Configuration=Release /target:Restore /target:Build || exit /b 1
@rem build PCL version
msbuild %SLNPATH% /p:Configuration=Release /p:Portability=Desktop /target:Restore /target:Build || exit /b 1

endlocal

setlocal

@rem set variables
set OUTDIR=build\Asn1Net.Reader
set SRCDIR=src\Asn1Net.Reader\Asn1Net.Reader\bin\

@rem prepare output directory
rmdir /S /Q %OUTDIR%
mkdir %OUTDIR%\pcl || exit /b 1
mkdir %OUTDIR%\desktop || exit /b 1

@rem copy files to output directory
copy %SRCDIR%\pcl\Release\Asn1Net.Reader.dll %OUTDIR%\pcl || exit /b 1
copy %SRCDIR%\pcl\Release\Asn1Net.Reader.XML %OUTDIR%\pcl || exit /b 1
copy %SRCDIR%\Release\Asn1Net.Reader.dll %OUTDIR%\desktop || exit /b 1
copy %SRCDIR%\Release\Asn1Net.Reader.XML %OUTDIR%\desktop || exit /b 1

@rem set license variables
set BUILDDIR=build

@rem copy licenses to output directory
copy %SRCDIR%\Release\LICENSE.txt %BUILDDIR% || exit /b 1
copy %SRCDIR%\Release\NOTICE.txt %BUILDDIR% || exit /b 1
copy LICENSE-3RD-PARTY %BUILDDIR%\3rd-party-license.txt || exit /b 1
copy README.md %BUILDDIR%\Readme.txt || exit /b 1

@rem copy make_nuget.bat and nuspec file
copy make_nuget.bat %BUILDDIR% || exit /b 1
copy Asn1Net.Reader.nuspec %BUILDDIR% || exit /b 1

endlocal

@echo BUILD SUCCEEDED !!!
@exit /b 0
