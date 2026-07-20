# DataShare

DataShare est un prototype de plateforme de transfert sécurisé de fichiers. L'application permet à un utilisateur connecté de téléverser un fichier, de générer un lien temporaire, de consulter son historique et de supprimer ses fichiers.

## Stack technique

| Front-end | React, Vite |
| Back-end | ASP.NET Core Web API (.NET 8) |
| Base de données | PostgreSQL |
| ORM | Entity Framework Core |
| Stockage fichiers | Système de fichiers local |
| Authentification | JWT |
| Tests back | xUnit, WebApplicationFactory, SQLite en mémoire, coverlet |
| Tests E2E | Cypress |
| Performance | k6 |

## Fonctionnalités implémentées

- Création de compte utilisateur.
- Connexion avec email et mot de passe.
- Génération d'un token JWT.
- Téléversement d'un fichier par un utilisateur connecté.
- Génération d'un lien public temporaire.
- Consultation de l'historique des fichiers de l'utilisateur.
- Téléchargement via lien unique.
- Protection optionnelle du téléchargement par mot de passe.
- Expiration configurable entre 1 et 7 jours.
- Suppression d'un fichier par son propriétaire.
- Validation côté client et côté serveur.

## Fonctionnalités non retenues dans le MVP

- Upload anonyme.
- Gestion des tags.
- Stockage AWS S3.
- Interface d'administration.
- Purge planifiée automatique par cron.

Ces éléments sont identifiés comme des évolutions possibles, mais le prototype reste concentré sur les user stories principales du MVP.

## Prérequis

- Node.js 20 ou supérieur.
- npm.
- .NET SDK 8.
- Docker Desktop.
- dotnet-ef 8.

Installation de dotnet-ef :

```powershell
dotnet tool install --global dotnet-ef --version 8.*
```

## Lancer PostgreSQL

Depuis la racine du projet :

```powershell
docker compose up -d postgres
```

Vérification :

```powershell
docker ps
```

Le conteneur expose PostgreSQL sur le port `5434` côté machin hôte.

## Configurer la base de données

Depuis le dossier API : 

```powershell
cd backend\DataShare.Api
dotnet restore
dotnet ef database update
```

Si la commande `dotnet ef` n'est pas reconnue :

```powershell
& "$env:USERPROFILE\.dotnet\tools\dotnet-ef.exe" database update
```

## Lancer le back-end

```powershell
cd backend\DataShare.Api
dotnet run
```


```txt
http://localhost:5000/api
```

Vérification :

```txt
http://localhost:5000/api/health
```

## Lancer le front-end

Dans un second terminal :

```powershell
cd frontend
npm install
npm run dev
```

Le front-end écoute sur :

```txt
http://localhost:5173
```

`frontend/.env.example` contient :

```env
VITE_API_URL=http://localhost:5000/api
```

Copie locale :

```powershell
Copy-Item .env.example .env
```

## Parcours de démonstration

1. Ouvrir `http://localhost:5173`.
2. Créer un compte.
3. Téléverser un fichier texte ou image.
4. Copier le lien généré.
5. Ouvrir le lien dans un nouvel onglet.
6. Télécharger le fichier.
7. Aller dans `Mes fichiers`.
8. Supprimer le fichier.

## Tests back-end

```powershell
cd backend\DataShare.Api.Tests
dotnet test
```

Avec couverture :

```powershell
dotnet test --collect:"XPlat Code Coverage" --settings coverlet.runsettings
```

Génération d'un rapport HTML :

```powershell
dotnet tool install --global dotnet-reportgenerator-globaltool
reportgenerator -reports:"TestResults\**\coverage.cobertura.xml" -targetdir:"coverage-report" -reporttypes:Html
```

Ouvrir ensuite :

```txt
backend/DataShare.Api.Tests/coverage-report/index.html
```

## Tests end-to-end Cypress

PostgreSQL, back-end et front-end doivent être lancés avant Cypress.

```powershell
cd frontend
npm run test:e2e
```

Mode interface graphique :

```powershell
npm run test:e2e:open
```

## Scan sécurité

Front-end :

```powershell
cd frontend
npm audit
```

Back-end :

```powershell
cd backend\DataShare.Api
dotnet list package --vulnerable
dotnet list package --outdated
```

## Test performance

Après démarrage de l'application :

```powershell
k6 run scripts\k6-upload.js
```

Le script teste un scénario d'inscription, d'upload puis de consultatin du lien.

## Livrables projet

- `docs/Documentation_technique_DataShare.pdf`
- `docs/TECHNICAL_DOCUMENTATION.md`
- `docs/DataShare_presentation.pptx`
- `docs/TESTING.md`
- `docs/SECURITY.md`
- `docs/PERF.md`
- `docs/MAINTENANCE.md`
- `docs/AI_USAGE.md`
- `repository-link.txt`
