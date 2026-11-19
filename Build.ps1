$ErrorActionPreference = 'Stop'

$compiler = Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe'
if (-not (Test-Path -LiteralPath $compiler)) {
    $compiler = Join-Path $env:WINDIR 'Microsoft.NET\Framework\v4.0.30319\csc.exe'
}
if (-not (Test-Path -LiteralPath $compiler)) {
    throw '.NET Framework 4.x C# compiler was not found.'
}

$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$outDir = Join-Path $root 'dist'
New-Item -ItemType Directory -Force -Path $outDir | Out-Null
$output = Join-Path $outDir 'CapyProxy.exe'

$frameworkDir = Split-Path -Parent $compiler
$wpfDir = Join-Path $frameworkDir 'WPF'
$references = @(
    "/reference:$(Join-Path $frameworkDir 'System.dll')",
    "/reference:$(Join-Path $frameworkDir 'System.Core.dll')",
    "/reference:$(Join-Path $frameworkDir 'System.Security.dll')",
    "/reference:$(Join-Path $frameworkDir 'System.Management.dll')",
    "/reference:$(Join-Path $frameworkDir 'System.Drawing.dll')",
    "/reference:$(Join-Path $frameworkDir 'System.Windows.Forms.dll')",
    "/reference:$(Join-Path $frameworkDir 'System.Xaml.dll')",
    "/reference:$(Join-Path $wpfDir 'WindowsBase.dll')",
    "/reference:$(Join-Path $wpfDir 'PresentationCore.dll')",
    "/reference:$(Join-Path $wpfDir 'PresentationFramework.dll')"
)

$arguments = @(
    '/nologo',
    '/target:winexe',
    "/out:$output",
    "/win32icon:$root\assets\ghost.ico",
    "/resource:$root\assets\ghost.png,ghost",
    "/resource:$root\src\Studio.xaml,ui"
) + $references + @("$root\src\Studio.cs")

& $compiler @arguments
if ($LASTEXITCODE -ne 0) {
    throw "C# compiler failed with exit code $LASTEXITCODE."
}
Write-Host "Built $output"
