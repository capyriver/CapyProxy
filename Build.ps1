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

$references = @(
    '/reference:System.dll',
    '/reference:System.Core.dll',
    '/reference:System.Security.dll',
    '/reference:System.Management.dll',
    '/reference:System.Drawing.dll',
    '/reference:System.Windows.Forms.dll',
    '/reference:System.Xaml.dll',
    '/reference:WindowsBase.dll',
    '/reference:PresentationCore.dll',
    '/reference:PresentationFramework.dll'
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
