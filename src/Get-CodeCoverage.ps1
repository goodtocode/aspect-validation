####################################################################################
# To execute
#   1. In powershell, set security policy for this script: 
#      Set-ExecutionPolicy Unrestricted -Scope Process -Force
#   2. Change directory to the script folder:
#      CD src (wherever your script is)
#   3. In powershell, run script: 
#      .\Get-CodeCoverage.ps1 -TestProjectFilter 'Tests.*.csproj' -ProdPackagesOnly -ProductionAssemblies 'MyApp.Core','MyApp.Web'
# This script uses native .NET 10 code coverage (Microsoft.Testing.Platform)
####################################################################################

Param(
    [string]$TestProjectFilter = 'Tests.*.csproj',    
    [switch]$ProdPackagesOnly = $false,    
    [string[]]$ProductionAssemblies = @(
        "Goodtocode.Validation"
    )
)
####################################################################################
if ($IsWindows) {Set-ExecutionPolicy Unrestricted -Scope Process -Force}
$VerbosePreference = 'SilentlyContinue' # 'Continue'
####################################################################################

# Only need reportgenerator for HTML reports
& dotnet tool install -g dotnet-reportgenerator-globaltool

$timestamp = Get-Date -Format "yyyyMMdd-HHmmss"
$scriptPath = Get-Item -Path $PSScriptRoot
$reportOutputPath = Join-Path $scriptPath "TestResults\Reports\$timestamp"

New-Item -ItemType Directory -Force -Path $reportOutputPath 

# Find test projects
$testProjects = Get-ChildItem $scriptPath -Filter $TestProjectFilter -Recurse
Write-Host "Found $($testProjects.Count) test projects."

foreach ($project in $testProjects) {
    $testProjectPath = $project.FullName
    Write-Host "Running tests with coverage for project: $($project.BaseName)"
    
    # Native .NET code coverage - outputs to TestResults in project directory
    & dotnet test $testProjectPath --configuration Debug
}

# Collect all coverage files (Microsoft.Testing.Platform outputs .coverage files)
$coverageFiles = Get-ChildItem -Path $scriptPath -Filter "*.coverage" -Recurse | Select-Object -ExpandProperty FullName

if ($coverageFiles.Count -eq 0) {
    Write-Warning "No coverage files found. Make sure your test projects have Microsoft.Testing.Extensions.CodeCoverage package installed."
    exit 1
}

Write-Host "Found $($coverageFiles.Count) coverage file(s)"

# Generate HTML report
$coverageFilesArg = ($coverageFiles -join ";")

if ($ProdPackagesOnly) {
    $assemblyFilters = ($ProductionAssemblies | ForEach-Object { "+$_" }) -join ";"
    & reportgenerator -reports:$coverageFilesArg -targetdir:$reportOutputPath -reporttypes:Html -assemblyfilters:$assemblyFilters
}
else {
    & reportgenerator -reports:$coverageFilesArg -targetdir:$reportOutputPath -reporttypes:Html
}

Write-Host "Code coverage report generated at: $reportOutputPath"

$reportIndexHtml = Join-Path $reportOutputPath "index.html"
if (Test-Path $reportIndexHtml) {
    Invoke-Item -Path $reportIndexHtml
}
else {
    Write-Warning "Report index.html not found at: $reportIndexHtml"
}