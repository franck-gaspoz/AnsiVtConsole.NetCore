@echo off
cls
echo --------------------------------------------------------------------------------------
echo publish-nuget
echo --------------------------------------------------------------------------------------
echo tag the version and publish the corresponding nuget from version to github and nuget
echo parameter: version tag
echo example: publish-nuget 1.0.15
echo nugetkey=%nugetkey%
echo githubkey=%githubkey%
echo version=%1%
echo --------------------------------------------------------------------------------------

git tag -a "v%0%" -m "v%0%";
git push origin v%0%
rem nuget push AnsiVtConsole.NetCore/bin/Release/AnsiVtConsole.NetCore.1.0.15.nupkg oy2aew5pvvred4ogzazq2qqa45n36mfubanxwqjp2nxsli -Source https://api.nuget.org/v3/index.json -Verbosity detailed
rem dotnet nuget push "AnsiVtConsole.NetCore/bin/Release/AnsiVtConsole.NetCore.1.0.15.nupkg" --api-key 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 --source "github"
