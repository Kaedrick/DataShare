# Preuves à ajouter au rendu

Ce dossier est prévu pour conserver les captures ou exports générés localement avant la soutenance.

## Couverture

Ajouter une capture de :

```txt
backend/DataShare.Api.Tests/coverage-report/index.html
```

La capture doit montrer la line coverage au-dessus de 70 %.

## Tests

Ajouter une capture du terminal après :

```powershell
cd backend\DataShare.Api.Tests
dotnet test
```

Ajouter aussi une capture après :

```powershell
cd frontend
npm run test:e2e
```

## Sécurité

Ajouter les résultats de :

```powershell
npm audit
dotnet list package --vulnerable
```

## Performance

Ajouter le résultat de :

```powershell
k6 run scripts\k6-upload.js
```
