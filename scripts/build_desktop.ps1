$Root = Split-Path -Parent $MyInvocation.MyCommand.Path
$ProjectRoot = Split-Path -Parent $Root
$Output = Join-Path $ProjectRoot "build/desktop"
New-Item -ItemType Directory -Force -Path $Output | Out-Null

Write-Host "Building desktop bundles (stub)..."
Write-Host "Windows EXE/MSIX and macOS .app would be produced here."
Get-ChildItem -Recurse (Join-Path $ProjectRoot "models") -Filter manifest.json
