@echo Off
set config=%1
if "%config%" == "" (
   set config=Release
)

REM Package restore
%nuget% install src\Autofac.Integration.Mef\packages.config -OutputDirectory %cd%\packages -NonInteractive
%nuget% install tests\Autofac.Tests.Integration.Mef\packages.config -OutputDirectory %cd%\packages -NonInteractive

REM Build
%WINDIR%\Microsoft.NET\Framework\v4.0.30319\msbuild Autofac.Mef.sln /p:Configuration="%config%" /m /v:M /fl /flp:LogFile=msbuild.log;Verbosity=Normal /nr:false
if not "%errorlevel%"=="0" goto failure

REM Unit tests
"%GallioEcho%" tests\Autofac.Tests.Integration.Mef\bin\%config%\Autofac.Tests.Integration.Mef.dll
if not "%errorlevel%"=="0" goto failure

REM Package
mkdir Build
cmd /c %nuget% pack "src\Autofac.Integration.Mef\Autofac.Integration.Mef.csproj" -symbols -o Build -p Configuration=%config% -version %PackageVersion%
if not "%errorlevel%"=="0" goto failure

:success
exit 0

:failure
exit -1