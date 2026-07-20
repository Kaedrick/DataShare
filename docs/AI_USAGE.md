# Utilisation de l'IA dans le développement

## Périmètre retenu

L'IA générative a été utilisée sur une seule user story du projet : **US02 - Téléchargement via lien**.

Cette user story a été choisie car elle permettait de confier une tâche limitée, vérifiable et isolable : relier le lien public généré après l'upload à une page de téléchargement fonctionnelle, tout en conservant les contrôles de sécurité côté serveur.

## Tâches confiées à l'IA

Les tâches demandées à l'IA ont été les suivantes :

- proposer la structure de la page front-end `/download/:token` ;
- proposer les appels API nécessaires pour récupérer les métadonnées d'un fichier ;
- proposer la logique de téléchargement du fichier depuis le navigateur ;
- aider à identifier la séparation entre la route de métadonnées et la route de téléchargement réel ;
- proposer une correction du lien public généré après upload pour qu'il ouvre la page React de téléchargement.

## Travail réalisé et supervision humaine

Le code proposé a été relu avant intégration. Les points vérifiés manuellement sont :

- le lien public ne doit pas donner accès à l'espace personnel ;
- un lien invalide doit retourner une erreur explicite ;
- un fichier expiré ne doit pas être téléchargeable ;
- un fichier protégé doit demander un mot de passe ;
- le mot de passe de fichier ne doit pas être stocké en clair ;
- le téléchargement doit renvoyer le fichier d'origine et non seulement ses métadonnées.

Après relecture, des ajustements ont été réalisés :

- distinction entre `GET /api/download/{token}` pour les métadonnées et `POST /api/download/{token}` pour le téléchargement ;
- ajout d'une route directe `GET /api/download/{token}/file` pour les fichiers non protégés ;
- correction du lien généré afin qu'il pointe vers le front-end ;
- vérification du comportement avec et sans mot de passe.

## Commits associés recommandés

Pour garder une trace claire de l'utilisation de l'IA, les commits peuvent être structurés ainsi :

```txt
feat(ai): add public download workflow for US02
fix: review download link generation and password handling
test: add integration tests for public download flow
docs: document AI usage for US02
```

## Limites de l'usage IA

L'IA n'a pas été utilisée pour décider du périmètre fonctionnel complet du MVP. Les choix suivants ont été conservés manuellement :

- priorisation des user stories obligatoires ;
- exclusion de l'upload anonyme et des tags ;
- choix du stockage local pour le prototype ;
- validation finale des routes, de la configuration et des documents de rendu.

L'IA a donc servi d'assistance ponctuelle sur US02, mais les choix d'architecture, d'intégration et de validation finale sont restés sous supervision humaine.
