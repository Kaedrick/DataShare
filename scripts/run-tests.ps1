Set-Location $PSScriptRoot\..
Set-Location backend\DataShare.Api.Tests
dotnet test
Set-Location ..\..\frontend
npm run build
