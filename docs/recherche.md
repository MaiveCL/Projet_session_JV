# Projet de session — Phase 1 (Recherche)

## Titre : *Autoédition*

---

## Concept narratif

Vous êtes persuadé·e d’avoir pondu le meilleur roman de votre longue carrière.  
Pourtant, votre éditeur réagit mal. Est-ce la jalousie, le dégoût ou autre chose qui le pousse, devant vos yeux horrifiés, à saisir votre manuscrit — ainsi que votre contrat d’édition — pour les balancer ensemble dans la déchiqueteuse ?

Votre cœur chavire : comme vous aviez toute confiance en lui depuis tant d'années de collaboration, vous lui aviez fourni votre seule copie !

**But du jeu**

> Reconstruire le manuscrit, une languette à la fois, pour l'autopublier avant que le fanclub se vide.

---

# Objectifs et intérêts

- Créer un système de **génération automatique de niveaux**, avec des paramètres de difficulté incrémentables (ex. : nombre de languettes désordonnées vs. nombre total par section).
      Exemple, au début il y a 1 languette en désordre dans une section de 10 langettes, puis éventuellement 20 languettes en désordre "distribuées"" dans un total de 30 languettes consécutives.

- Introduire des **éléments de collection** (fragments du contrat) qui influencent la mécanique principale — réduire la vitesse de perte des fans.

- Permettre une **liberté de déplacement et de stratégie** (réparer tout de suite le roman ou chercher d’abord les preuves).

- Offrir parrallèllement des **actions contextuelles** (Tenir un bouton appuyé quelques secondes (ou faire une série de boutons dans l'ordre pour — ex. : envoyer un message au fanclub, supprimer/répondre à un troll — pour stabiliser la jauge de popularité).

- Réussir un niveau débloque l'éditeur de niveau : et pour l'ajouter au système, il faudra le réussir. Le niveau de difficulté pourra être évalué pour les mettre en ordre et reclassé selon le taux de réussite des joueurs.

- À la fin du jeu, proposer un lien pour lire la suite du roman (fonction « jeu-pub »).

-------

# Connaissances requises

###### Recherche 1 de 3 — Import automatique

Au début, je voulais automatiser l'importation d'un document Google Docs pour que le manuscrit principal du jeu puisse être modifiable. Après recherches, j’estime que cela risque d’entraîner une surcharge de travail dans le temps imparti. Voici le résumé de cette recherche :

- Utiliser l'[API Google Docs](https://developers.google.com/docs/api) pour extraire le contenu du document en version pdf. Via files export (nécessite de configurer un projet dans la console Google Cloud pour obtenir les identifiants.)

- Utiliser ensuite une autre API (exemple pdf2image) pour convertir le pdf en images.

- Sinon, chat gpt m'a proposé d'utiliser OS.execute() pour appeler un script **Python + Pillow** pour générer directement des images à partir du texte (la point fort c'est que cela permet de choisir une police manuscrite). Exemple rapide (non testé)  :

```python
from PIL import Image, ImageDraw, ImageFont

texte = "Ton texte long ici..."
img = Image.new('RGB', (800, 1200), color=(255,255,255))
draw = ImageDraw.Draw(img)
font = ImageFont.truetype("Cursive.ttf", 32)
draw.text((20,20), texte, fill=(0,0,0), font=font)
img.save("page1.png")
```

**Conclusion de la recherche 1 :** Je vais plutôt faire cela manuellement, quitte à n'avoir que du contenu fixe et ajouter cette fonctionalité à la fin si le temps le permet.

### Recherche 2 de 3

J’ai visionné plusieurs tutoriels pour les puzzles dans Godot :

- Le [premier](https://www.youtube.com/watch?v=aODh7LNiEbI) que j'ai visionné se trouvait à avoir une grande faiblesse : chaque pièce de puzzle était une image indépendante. 

- Alors j'ai visionné [celui-ci](https://www.youtube.com/watch?v=gfuflZ21FDU&list=PL5t0hR7ADzuk3drVsw-8BKx9JK35lp6Ix&index=1)), dont on voyait tout de suite que l'image de départ était complète. La façon de gérer le puzzle était plutôt utile, par contre il y aurra beaucoup d'adaptation a faire au niveau des contrôles utilisateurs, car ce n'est un puzzle basée sur une tuile vide vers laquelle les pièce se déplacent quand on clique dessus.

- Finalement, [ce tutoriel](https://www.youtube.com/watch?v=t27pntMNXf8) est plus complet, de par sa nature "drag and drop" de pieces, à convertir simplement en "parcourir, selectionner, switch(gauche ou droite) et lâcher(retour à parcourir)" dans le sens que l'arcade n'a pas de souris-clavier. Un autre point d'intérêt de ce tuto est que le nombre de pieces du puzzle est généré par un script, ce qui le rends davantage personalisable.

**Conclusion de la recherche 2 :** Si je désire ne personaliser que le mélange des pieces et non leur taille, je pourrais combiner le tuto 2 et 3.

### Recherche 3 de 3

J'ai eu beaucoup de mal à trouver les mots clefs exprimant la "navigation" parmis les fragments de romans afin de trouver des tutoriels. J'ai fini par trouver les [menu carousels](https://www.youtube.com/watch?v=z6sUvOBYpT4) très intéressants. Il ne me faudra que mixer ce principle avec celui du pooling pour naviger dans le jeu de façon optimisé.

## Pistes d’algorithmes ou mécaniques à explorer

| Objectif                                                               | Type                                  | Description / Référence                                                                                                                                                    |
| ---------------------------------------------------------------------- | ------------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Importation auto d'un texte                                            |                                       | Annulé, surcharge de tavail suspectée                                                                                                                                      |
| Génération automatique de niveaux                                      | **Algorithme procédural**             | Génération basée sur des paramètres (densité de désordre, plage de sections affectées). Inspiration : algorithmes de « shuffle ».                                          |
| Gestion de la popularité du fanclub                                    | **Système dynamique**                 | Décroissance continue modulée par actions (collecte d’objets, messages). Modélisable par une fonction mathématique (linéaire/exponentielle) avec événements modificateurs. |
| Reconstruction du texte                                                | **Mécanique de tri / reconstitution** | Système de permutation de fragments pour remettre les languettes en ordre.                                                                                                 |
| Éditeur de niveaux                                                     | **Outil intégré**                     | Interface pour ajuster les paramètres de génération du niveau.                                                                                                             |
| Gestion asynchrone d’actions contextuelles                             | Synchronisation / coroutines          | Permettre des appuis longs, combos, timers sans bloquer la boucle principale (coroutines / signals Godot)...                                                               |
| Permettre une **liberté de déplacement et de stratégie**               | Pooling pour afficher les slices      | Afficher uniquement les fragments visibles (object pooling) pour parcourir des manuscrits très longs sans ralentissement.                                                  |
| la "surface" sur laquelle on travail pourrait être générée par un algo |                                       |                                                                                                                                                                            |

---

## Étapes prévues

- [ ] Créer une première simulation de languettes désordonnées.
- [ ] Concevoir une logique simple de perte de fans dans le temps.

----------------------
