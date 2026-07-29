# Run this from anywhere -- it locates itself, installs dependencies if
# needed, and starts the dev server on port 3000 (required to match the
# backend's CORS policy).

$root = $PSScriptRoot
Write-Host "Frontend root: $root" -ForegroundColor Cyan

if (-not (Test-Path "$root\node_modules")) {
    Write-Host "`nInstalling dependencies (first run only)..." -ForegroundColor Cyan
    Push-Location $root
    npm install
    Pop-Location
    if ($LASTEXITCODE -ne 0) {
        Write-Host "`nnpm install failed. Stopping here -- see the error above." -ForegroundColor Red
        exit 1
    }
}

Write-Host "`nStarting the frontend dev server..." -ForegroundColor Green
Push-Location $root
npm run dev
Pop-Location
