# Titre&thinsp;: *Autoédition*

*par Marie-Eve Bouchard*

---

## Scénario

Vous êtes persuadé·e avoir pondu le meilleur roman de votre longue carrière.  
Pourtant, votre éditeur réagit mal. Est-ce la jalousie, le dégoût ou autre chose qui le pousse, devant vos yeux horrifiés, à saisir votre manuscrit — ainsi que votre contrat d’édition — pour les balancer ensemble dans la déchiqueteuse&thinsp;?

Votre cœur chavire&thinsp;: comme vous aviez toute confiance en lui depuis tant d'années de collaboration, vous lui aviez fourni votre seule copie&thinsp;!

**But du jeu**

> Reconstruire le manuscrit, une languette à la fois, pour l'autopublier avant que le fanclub se vide.

---

## **Concepts de programmation utilisés**

##### 

##### Pooling

Créer entre 20 et plus de 300 tranches de manuscrit sur la scène principale n'était pas optimale, j'ai donc pensé à un système non destructifp pour recycler des objets utilisés pour afficher seulement les tranches proche du "joueur". 

Ainsi, les éléments du jeu débodent à peine de l'écran afin de laisser une certaine marge de manoeuvre et plutôt que de créer et détruire des éléments pour en garder un total décent, les bandes qui sont en dehors de l'écran se "téléportent" d'un côté à l'autre et se mettent à jour pour afficher la bonne image selon l'ordre global du tableau d'index des tranches du sprite du chapitre en cours. 

Ce tableau évolue et se met à jour authomatiquement au fil de la partie lorsque le joueur déplace des bandes physiques.

Le nombre d'objets dans le pooling n'est pas déterminé à l'avance, du nombre de tranche par page envoyé en paramètre à ma classe de construction du chapitre à reconstruire.

Pour créer cet algorythme, J'ai réalisé que je devais tout créer mon tableau par code alors j'ai simplement écrit plusieurs version d'un algo mixte de génération/pooling de plus en pseudocode, pour en simplifier les mécaniques jusqu'à ce que ce soit facile à programmer.



##### Génération automatique

Afin de réussir le pooling et la personalisation du niveau 2, je n'avais d'autre choix que de développer une logique de création de tranches automatisée, car, entre-autre, la taille du pool dépend de la quantité de divisions (de tranches) du manuscrit. 



##### Niveau personalisé

Le résultat combiné ci-haut visait entre autre à permettre la personalisation d'un niveau après avoir gagné le premier niveau. Ainsi, le joueur peut choisir le nombre de tranche par page, le nombre de membre du fanclub au départ et le nombre de fan perdu par secondes.

De plus, dans le menu il est aussi possible d'activer ou désactiver un son qui aide la joueur à détecter des bandes consécutives. Cette mécanique à aussi demandé le développement d'un algorythme personalisé.

---

##### Aide intégré

Le joueur peut consulter automatiquement l'aide intégré lorsqu'il met en pause le jeu.
