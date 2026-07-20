# Stratégie de test

## Objectif

L'objectif des tests est de vérifier que les fonctionnalités principales de l'zpp fonctionnent correctement.

Les fonctionnalités critiques testées sont :

- création de compte utilisateur ;
- connexion utilisateur ;
- upload de fichier ;
- affichage des fichiers envoyés ;
- génération d'un lien de téléchargement ;
- téléchargement d'un fichier ;
- gestion d'un lien de téléchargement invalide ;
- suppression d'un fichier ;
- règles de sécurité côté backend.

## Tests backend

Ils vérifient principalement les services et les API.

Commande exécutée :

```bash
dotnet test --collect:"XPlat Code Coverage"
```

Résultat obtenu :

- 38 tests exécutés ;
- 38 tests réussis ;
- 0 test échoué ;
- couverture de lignes : 92,4 %.

## Couverture de code

Un rapport de couverture a été généré avec ReportGenerator.

Commande utilisée :
``` reportgenerator -reports:"TestResults\**\coverage.cobertura.xml" -targetdir:"coverage-report" -reporttypes:Html ```

Le rapport HTML est disponible dans :
``` backend/DataShare.Api.Tests/coverage-report/index.html ```

## Tests end-to-end

Les tests end2end sont réalisés avec Cypress.

Commande exécutée :
``` npm run test:e2e ```

Résultat obtenu :

- 2 tests exécutés ;
- 2 tests réussis ;
- 0 test échoué.

Scénarios Cypress testés :

- Scénario 1 : parcours principal utilisateur

(creates an account, uploads a file, lists it and downloads it)

Ce test vérifie qu'un utilisateur peut créer un compte, envoyer un fichier, voir ce fichier dans son espace et utiliser le lien de téléchargement.

- Scénario 2 : lien invalide
(shows an error when the download token is invalid)

Ce test vérifie que l'application affiche une erreur lorsqu'un lien de téléchargement invalide est utilisé.

## Conclusion

Les tests automatisés valident les fonctionnalités principales de l'application.

Les tests backend couvret la logique métier et les endpoints importants, et les tests Cypress vérifient le fonctionnement global de l'application depuis le pdv de l'user.

La couverture obtenue est aud-elà du seuil attendu.