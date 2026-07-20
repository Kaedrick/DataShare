# Test de performance

## Objectif

L'objectif du test de performance est de vérifier que l'API reste stable et réactive lorsqu'elle reçoit plusieurs requêtes sur une fonctionnalité importante.

La fonctionnalité testée est l'upload de fichier, car il s'agit d'une des fonctionnalités principales de DataShare.

## Endpoint testé

```http POST /api/files/upload ```

## Outil utilisé

Le test de performance est réalisé avec k6.

## Scénario testé

Le scénario simule plusieurs utilisateurs qui utilisent l'application en même temps.

Chaque utilisateur virtuel effectue les actions suivantes :

création d'un compte utilisateur ;
récupération du token d'authentification ;
upload d'un petit fichier texte ;
vérification de la réponse de l'API.

## Paramètres du test

- 5 utilisateurs virtuels ;
- durée du test : 30 secondes ;
- seuil d'erreur accepté : inférieur à 5 % ;
- temps de réponse attendu : 95 % des requêtes sous 1 seconde.

**Commande exécutée :**
```k6 run scripts/k6-upload.js```

**Seuils définis :**
http_req_failed < 5 %
p95 http_req_duration < 1000 ms

## Résultat attendu

Le test est considéré comme réussi si :

l'API répond correctement aux créations de compte ;
l'upload de fichier fonctionne ;
le taux d'erreur reste inférieur au seuil défini ;
les temps de réponse restent acceptables.

## Résultat obtenu

Le test k6 s'est terminé avec succès.

Résultats principaux :

- 5 itérations complétées ;
- 0 requête échouée ;
- 25 vérifications réussies sur 25 ;
- taux d'échec HTTP : 0 % ;
- temps de réponse moyen : 53,04 ms ;
- temps de réponse p95 : 158,2 ms ;
- seuil p95 attendu : inférieur à 1500 ms ;
- seuil d'erreur attendu : inférieur à 5 %.

Les vérifications suivantes sont passées :

- création de compte réussie ;
- récupération du token ;
- upload du fichier réussi ;
- génération du lien de téléchargement ;
- récupération des métadonnées du fichier.

## Conclusion

Ce test permet de valider que la fonctionnalité d'upload reste utilisable sous une charge légère.
Toutes les vérifications sont passées, aucune requête HTTP n'a échoué et le temps de réponse p95 est resté largement inférieur au seuil fixé.
Ce test ne remplace pas un audit de performance complet, mais il permet de confirmer que le MVP reste réactif et utilisable sur un scénario critique.
