$ErrorActionPreference = "Stop"

$repoRoot = Get-Location
$programFile = Join-Path $repoRoot "BackEnd\KuroOpti.API\Program.cs"

if (-not (Test-Path (Join-Path $repoRoot "KuroOpti.sln"))) {
    throw "Run this script from the KuroOpti repository root."
}

if (-not (Test-Path $programFile)) {
    throw "Program.cs was not found at BackEnd\KuroOpti.API\Program.cs."
}

$content = Get-Content $programFile -Raw

if ($content -notmatch "using KuroOpti\.API\.Extensions;") {
    $content = "using KuroOpti.API.Extensions;`r`n" + $content
}

if ($content -notmatch "ApplyDatabaseMigrationsAsync") {
    $patterns = @(
        "var app = builder\.Build\(\);",
        "WebApplication app = builder\.Build\(\);"
    )

    $updated = $false

    foreach ($pattern in $patterns) {
        if ($content -match $pattern) {
            $replacement = '$0' + "`r`n`r`nawait app.ApplyDatabaseMigrationsAsync();"
            $content = [regex]::Replace($content, $pattern, $replacement, 1)
            $updated = $true
            break
        }
    }

    if (-not $updated) {
        throw "Could not find 'var app = builder.Build();' in Program.cs. Add 'await app.ApplyDatabaseMigrationsAsync();' manually after app creation."
    }
}

Set-Content -Path $programFile -Value $content -Encoding UTF8

Write-Host ""
Write-Host "Program.cs updated successfully."
Write-Host ""
Write-Host "Next commands:"
Write-Host "  Copy-Item .env.example .env"
Write-Host "  docker compose up --build -d"
Write-Host "  docker compose logs -f api"
