# Base de données DataShare

Le projet utilise PostgreSQL avec Docker.

## Lancer PostgreSQL

Depuis la racine du projet :

```powershell
docker compose up -d postgres
```

## Configuration Docker

| Paramètre | Valeur |
|---|---|
| Base | datasharedb |
| Utilisateur | adeluser |
| Mot de passe | adelpass123 |
| Port hôte | 5434 |
| Port conteneur | 5432 |

## Appliquer les migrations Entity Framework

```powershell
cd backend\DataShare.Api
dotnet ef database update
```

Alternative si `dotnet ef` n'est pas dans le PATH :

```powershell
& "$env:USERPROFILE\.dotnet\tools\dotnet-ef.exe" database update
```

## Script SQL de référence

Le fichier `database/init.sql` décrit les tables principales si une création manuelle est nécessaire. La méthode recommandée reste Entity Framework Core, car elle garde la base alignée avec les modèles C# et les migrations.
