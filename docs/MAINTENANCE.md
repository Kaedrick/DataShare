# Maintenance

## Objectif

Ce document décrit les actions à prévoir pour maintenir DataShare après la livraison du MVP.

L'objectif est de garder l'application fonctionnelle, sécurisée et facile à faire évoluer.

## Périmètre de maintenance

La maintenance concerne :

- le backend .NET ;
- le frontend React ;
- la base de données PostgreSQL ;
- les dépendances NuGet et npm ;
- les tests automatisés ;
- la documentation du projet.

## Vérifications régulières

### Chaque évolution importante

Avant de valider une évolution, il faut relancer :

```bash
dotnet test --collect:"XPlat Code Coverage"
npm run test:e2e
```

Ces commandes permettent de vérifier que les fonctionnalités principales fonctionnent toujours.

### Après ajout ou mise à jour d'une dépendance

Il faut relancer les analyses de sécurité :

```bash
dotnet list package --vulnerable
npm audit
```

Si une vulnérabilité est détectée, la dépendance concernée doit être mise à jour ou remplacée.

### Après modification d'un endpoint important

Si un endpoint lié à l'authentification, à l'upload ou au téléchargement est modifié, il faut :

- vérifier les tests backend ;
- relancer les tests Cypress ;
- vérifier manuellement le parcours utilisateur concerné.

## Maintenance de la base de données

Les modifications du modèle de données doivent être faites avec des migrations Entity Framework.

Commande de création d'une migration :

```bash
dotnet ef migrations add NomDeLaMigration
```

Commande d'application des migrations :

```bash
dotnet ef database update
```

Avant une modification importante de la base, il est recommandé de sauvegarder les données.

## Maintenance des fichiers uploadés

Les fichiers envoyés par les utilisateurs sont stockés côté serveur.

Points à surveiller :

- espace disque disponible ;
- suppression des fichiers expirés ;
- cohérence entre les fichiers stockés et les entrées en base ;
- taille maximale autorisée pour les uploads.

Pour une version future, une tâche automatique pourrait être ajoutée afin de supprimer régulièrement les fichiers expirés.

## Risques identifiés

| Risque | Impact | Réponse prévue |
|---|---|---|
| Dépendance vulnérable | Risque de sécurité | Lancer régulièrement les scans et mettre à jour les packages |
| Régression sur l'upload | Fonction principale bloquée | Relancer les tests backend et Cypress |
| Base de données mal configurée | Connexion ou inscription impossible | Vérifier la chaîne de connexion et les migrations |
| Accumulation de fichiers expirés | Espace disque saturé | Prévoir une purge régulière |
| Modification non documentée | Projet difficile à reprendre | Mettre à jour le README et les documents de suivi |

## Fréquence conseillée

| Action | Fréquence |
|---|---|
| Tests automatisés | À chaque évolution |
| Scan de sécurité | À chaque ajout ou mise à jour de dépendance |
| Vérification de performance | Avant livraison ou changement important |
| Vérification de la documentation | Avant chaque rendu ou version importante |
| Nettoyage des fichiers expirés | À définir selon l'utilisation réelle |

## Procédure avant livraison

Avant de livrer une nouvelle version, il faut vérifier que :

- l'application démarre correctement ;
- les migrations sont appliquées ;
- les tests backend passent ;
- la couverture reste supérieure à 70 % ;
- les tests Cypress passent ;
- les scans de sécurité ne remontent pas de vulnérabilité bloquante ;
- le test de performance reste acceptable ;
- le README et les documents sont à jour.

## Conclusion

La maintenance de DataShare repose principalement sur des vérifications simples mais régulières : tests, sécurité, base de données et documentation.

Cette organisation permet de limiter les régressions et de garder le projet compréhensible pour une reprise future.