# Contrat d'interface API DataShare

## Base URL

```txt
http://localhost:5000/api
```

## Authentification

Les routes privées utilisent l'en-tête :

```txt
Authorization: Bearer <token>
```

## Endpoints

| Méthode | Route | Auth | Description |
|---|---|---|---|
| GET | `/health` | Non | Statut API |
| POST | `/auth/register` | Non | Création de compte |
| POST | `/auth/login` | Non | Connexion |
| POST | `/files/upload` | Oui | Upload d'un fichier |
| GET | `/files/me` | Oui | Historique utilisateur |
| DELETE | `/files/{id}` | Oui | Suppression fichier |
| GET | `/download/{token}` | Non | Métadonnées publiques |
| POST | `/download/{token}` | Non | Téléchargement avec body JSON |
| GET | `/download/{token}/file` | Non | Téléchargement direct si non protégé |

## Register

Requête :

```json
{
  "email": "user@example.com",
  "password": "Password123!"
}
```

Réponse :

```json
{
  "token": "jwt",
  "email": "user@example.com"
}
```

## Login

Requête :

```json
{
  "email": "user@example.com",
  "password": "Password123!"
}
```

Réponse :

```json
{
  "token": "jwt",
  "email": "user@example.com"
}
```

## Upload

Requête `multipart/form-data` :

| Champ | Type | Obligatoire |
|---|---|---|
| file | file | Oui |
| expirationDays | number | Non |
| password | string | Non |

Réponse :

```json
{
  "id": "uuid",
  "originalName": "document.txt",
  "size": 1234,
  "expiresAt": "2026-07-26T10:00:00Z",
  "downloadUrl": "http://localhost:5173/download/token"
}
```

## Historique

Réponse :

```json
[
  {
    "id": "uuid",
    "originalName": "document.txt",
    "size": 1234,
    "uploadedAt": "2026-07-19T10:00:00Z",
    "expiresAt": "2026-07-26T10:00:00Z",
    "isExpired": false,
    "downloadUrl": "http://localhost:5173/download/token"
  }
]
```

## Téléchargement public

Métadonnées :

```json
{
  "originalName": "document.txt",
  "size": 1234,
  "expiresAt": "2026-07-26T10:00:00Z",
  "isExpired": false,
  "requiresPassword": false
}
```

Téléchargement :

```json
{
  "password": "optionnel"
}
```

Réponse : contenu binaire du fichier.
