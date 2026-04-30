@echo off
setlocal

set SCRIPT_DIR=%~dp0

start "Trading Engine Backend" cmd /k call "%SCRIPT_DIR%run-backend.cmd"
start "Trading Engine Frontend" cmd /k call "%SCRIPT_DIR%run-frontend.cmd"