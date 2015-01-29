@rem Add path to MSBuild Binaries
setlocal

@rem preparing environment
call "C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\Tools\vsvars32.bat" || @goto :error

set SLNPATH=src\Asn1Net.Reader\Asn1Net.Reader.sln

@rem cleanin sln
msbuild %SLNPATH% /p:Configuration=Release /target:Clean || exit /b 1
@rem build desktop version (.NET 4.0)
msbuild %SLNPATH% /p:Configuration=Release /target:Build || exit /b 1
@rem build PCL version
msbuild %SLNPATH% /p:Configuration=Release /p:Portability=Desktop /target:Build || exit /b 1

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
copy %SRCDIR%\Release\Asn1Net.Reader.dll %OUTDIR%\desktop || exit /b 1

@rem set license variables
set LICENSEDIR=%OUTDIR%

@rem copy licenses to output directory
copy LICENSE %LICENSEDIR%\license.txt || exit /b 1
copy LICENSE-3RD-PARTY %LICENSEDIR%\3rd-party-license.txt || exit /b 1
copy agpl-3.0.txt %LICENSEDIR% || exit /b 1
copy README.md %LICENSEDIR%\Readme.txt || exit /b 1

@rem copy make_nuget.bat and nuspec file
set BUILDDIR=build
copy make_nuget.bat %BUILDDIR% || exit /b 1
copy Asn1Net.Reader.nuspec %BUILDDIR% || exit /b 1

endlocal

@echo BUILD SUCCEEDED !!!
@exit /b 0
