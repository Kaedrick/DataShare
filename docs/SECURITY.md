# Analyse de sécurité

## Objectif

Objectif : vérifier qu'aucune dépendance connue comme vulnérable n'est utilisée dans le projet.

L'analyse concerne le backend .NET et le frontend React.

## Backend

Commande :

``` dotnet list package --vulnerable ```

Résultat :
*Le projet spécifié 'DataShare.Api' n'a aucun package vulnérable compte tenu des sources actuelles.*

## Frontend
Commande exécutée :
```npm audit```
Résultat : 
*Aucune vulnérabilité connue n'a été détectée dans les dépendances npm du frontend.*

## Mesures de sécurité présentes

L'application utilise plusieurs mesures de sécurité de base :

authentification utilisateur ;
mots de passe hashés ;
accès aux fichiers lié à l'utilisateur connecté ;
liens de téléchargement avec token ;
possibilité de protéger un téléchargement par mot de passe ;
expiration des liens de téléchargement ;
vérification des dépendances backend et frontend.

## Décision

Aucune correction de dépendance n'est nécessaire à ce stade.

Pour la maintenance du projet, les commandes de vérification devront être relancées régulièrement, notamment après l'ajout ou la mise à jour d'une dépendance.

## Conclusion

L'analyse réalisée ne montre pas de vulnérabilité connue dans les dépendances utilisées par l'application.

