cd /d "%~dp0"
icacls ..\ /grant "Network Service":(OI)(CI)W
if "%EMULATED%"=="true" exit 0
md "%programfiles(x86)%\nodejs"
copy /y node.exe "%programfiles(x86)%\nodejs" 
copy /y ..\Web.cloud.config ..\Web.config
start /w vcredist_x64.exe /q 
start /w msiexec.exe /quiet /i iisnode.msi