@ECHO OFF
dotnet nuget push %~1 --api-key <api-key> --source https://api.nuget.org/v3/index.json
pause