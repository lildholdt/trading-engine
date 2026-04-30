@echo off
setlocal

pushd "%~dp0..\backend"
dotnet run --project src\TradingEngine\TradingEngine.csproj
set EXIT_CODE=%ERRORLEVEL%
popd

exit /b %EXIT_CODE%