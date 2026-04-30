@echo off
setlocal

pushd "%~dp0..\frontend"
npm run dev
set EXIT_CODE=%ERRORLEVEL%
popd

exit /b %EXIT_CODE%