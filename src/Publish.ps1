param(
    [Parameter(Mandatory)][string]$Version,
    [Parameter(Mandatory)][string]$NugetServer,
    [Parameter(Mandatory)][string]$NugetApiKey
)

$project = Join-Path $PSScriptRoot "AspNetCore.DataProtection.RavenDB.csproj"

$Version = "$Version-beta"

dotnet pack $project --include-symbols --include-source "-p:Version=$Version" -c Release

dotnet nuget push (Join-Path $PSScriptRoot "\bin\Release\AspNetCore.DataProtection.RavenDB.$Version.nupkg") `
    --source $NugetServer --api-key $NugetApiKey