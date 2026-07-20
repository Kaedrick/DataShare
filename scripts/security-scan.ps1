Set-Location $PSScriptRoot\..\frontend
npm audit
Set-Location ..\backend\DataShare.Api
dotnet list package --vulnerable
dotnet list package --outdated
