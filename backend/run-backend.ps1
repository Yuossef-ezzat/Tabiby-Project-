# Run this from anywhere -- it locates itself and always restores + runs
# using the correct NuGet.Config, so there's no room for copy-paste or
# path mistakes.

$root = $PSScriptRoot
Write-Host "Project root: $root" -ForegroundColor Cyan

Write-Host "`nRestoring packages..." -ForegroundColor Cyan
dotnet restore --configfile "$root\NuGet.Config"

if ($LASTEXITCODE -ne 0) {
    Write-Host "`nRestore failed. Stopping here -- see the error above." -ForegroundColor Red
    exit 1
}

Write-Host "`nRestore succeeded. Starting the API..." -ForegroundColor Green
# Force the URL explicitly -- this backend's own JWT issuer/audience
# (appsettings.json) and the frontend's VITE_API_BASE_URL both assume
# https://localhost:7292, but dotnet run sometimes picks the "http" launch
# profile instead of "https" depending on how it's invoked.
dotnet run --project "$root\PL" --no-restore --urls "https://localhost:7292"
