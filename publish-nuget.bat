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
echo version=%1
echo --------------------------------------------------------------------------------------

git tag -a v%1 -m v%1
git push origin v%1
nuget push CommandLine.NetCore/bin/Release/CommandLine.NetCore.%1.nupkg %nugetkey% -SkipDuplicate -Source https://api.nuget.org/v3/index.json -Verbosity detailed
dotnet nuget push "CommandLine.NetCore/bin/Release/CommandLine.NetCore.%1.nupkg" --skip-duplicate --api-key %githubkey% --source "github"
