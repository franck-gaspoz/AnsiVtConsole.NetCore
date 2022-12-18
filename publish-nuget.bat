@echo off
cls
echo --------------------------------------------------------------------------------------
echo publish-nuget
echo --------------------------------------------------------------------------------------
echo tag tge version and publish the corresponding nuget from version to github and nuget
echo parameter: version tag
echo exampple: publish-nuget 1.0.15
echo nugetkey=%nugetkey%
echo githubkey=%githubkey%
echo --------------------------------------------------------------------------------------

rem git tag -a "v1.0.15.0" -m "v1.0.15.0";
rem git push origin v1.0.15.0
rem nuget push AnsiVtConsole.NetCore/bin/Release/AnsiVtConsole.NetCore.1.0.15.nupkg oy2aew5pvvred4ogzazq2qqa45n36mfubanxwqjp2nxsli -Source https://api.nuget.org/v3/index.json -Verbosity detailed
rem dotnet nuget push "AnsiVtConsole.NetCore/bin/Release/AnsiVtConsole.NetCore.1.0.15.nupkg" --api-key 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 --source "github"
