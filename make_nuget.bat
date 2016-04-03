@rem Add path to MSBuild Binaries
setlocal
call "C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\Tools\vsvars32.bat" || @goto :error
@IF EXIST "c:\Program Files (x86)\Microsoft Visual Studio 10.0\Common7\Tools\vsvars32.bat" SET devenv="c:\Program Files (x86)\Microsoft Visual Studio 10.0\Common7\Tools\vsvars32.bat"
@IF EXIST "c:\Program Files\Microsoft Visual Studio 10.0\Common7\Tools\vsvars32.bat" SET devenv="c:\Program Files\Microsoft Visual Studio 10.0\Common7\Tools\vsvars32.bat"
@IF EXIST "c:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\Tools\vsvars32.bat" SET devenv="c:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\Tools\vsvars32.bat"
@IF EXIST "c:\Program Files\Microsoft Visual Studio 11.0\Common7\Tools\vsvars32.bat" SET devenv="c:\Program Files\Microsoft Visual Studio 11.0\Common7\Tools\vsvars32.bat"
@IF EXIST "c:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\Tools\vsvars32.bat" SET devenv="C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\Tools\vsvars32.bat"
@IF EXIST "C:\Program Files\Microsoft Visual Studio 12.0\Common7\Tools\vsvars32.bat" SET devenv="C:\Program Files\Microsoft Visual Studio 12.0\Common7\Tools\vsvars32.bat"

set cur_dir=%CD%
call %devenv% || exit /b 1


IF EXIST nuget.exe goto restore

echo Downloading nuget.exe
@powershell -NoProfile -ExecutionPolicy unrestricted -Command "$ProgressPreference = 'SilentlyContinue'; Invoke-WebRequest 'https://dist.nuget.org/win-x86-commandline/latest/nuget.exe' -OutFile 'nuget.exe'"

set NUGETDIR=nuget

rmdir /S /Q %NUGETDIR%
mkdir %NUGETDIR%\Asn1Net.Reader\lib\portable-net45+netcore45+wpa81+MonoAndroid1+MonoTouch1 || exit /b 1
mkdir %NUGETDIR%\Asn1Net.Reader\lib\net40 || exit /b 1

copy Asn1Net.Reader\pcl\*.dll "%NUGETDIR%\Asn1Net.Reader\lib\portable-net45+netcore45+wpa81+MonoAndroid1+MonoTouch1" || exit /b 1
copy Asn1Net.Reader\pcl\*.xml "%NUGETDIR%\Asn1Net.Reader\lib\portable-net45+netcore45+wpa81+MonoAndroid1+MonoTouch1" || exit /b 1
copy Asn1Net.Reader\desktop\*.dll %NUGETDIR%\Asn1Net.Reader\lib\net40 || exit /b 1
copy Asn1Net.Reader\desktop\*.xml %NUGETDIR%\Asn1Net.Reader\lib\net40 || exit /b 1

copy LICENSE.txt %NUGETDIR%\Asn1Net.Reader || exit /b 1
copy NOTICE.txt %NUGETDIR%\Asn1Net.Reader || exit /b 1
copy 3rd-party-license.txt %NUGETDIR%\Asn1Net.Reader || exit /b 1

copy Asn1Net.Reader.nuspec %NUGETDIR%\Asn1Net.Reader || exit /b 1

nuget pack %NUGETDIR%\Asn1Net.Reader\Asn1Net.Reader.nuspec || exit /b 1

endlocal

@echo NUGET BUILD SUCCEEDED !!!
@exit /b 0
