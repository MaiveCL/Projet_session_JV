# Projet de session — Phase 1 (Recherche)

## Titre : *Autoédition*

---

## Concept narratif

Vous êtes persuadé·e d’avoir pondu le meilleur roman de votre longue carrière.  
Pourtant, votre éditeur réagit mal. Est-ce la jalousie, le dégoût ou autre chose qui le pousse, devant vos yeux horrifiés, à saisir votre manuscrit — ainsi que votre contrat d’édition — pour les balancer ensemble dans la déchiqueteuse ?

Votre coeur chavire : comme vous aviez toute confiance en lui depuis tant d'années de collaboration, vous lui aviez fourni votre seule copie !

**But du jeu**

> Reconstruire le manuscrit, une languette à la fois, pour l'autopublier avant que le fanclub se vide.
> 
> -----------

# Objectifs et intérêts

- Créer un système de **génération automatique de niveaux**, avec des paramètres de difficulté incrémentaux (ex. : nombre de languettes désordonnées vs. nombre total par section).
      Exemple, au début il y a 1 languette en désordre dans une section de 10 langettes, dans une page contenant 30 langettes, puis éventuellement 20 languettes en désordre distribuées dans un total de 30 languettes consécutives.

- Introduire des **éléments de collection** (fragments du contrat) qui influencent la mécanique principale — réduire la vitesse de perte des fans.

- Permettre une **liberté de déplacement et de stratégie** (réparer tout de suite le roman ou chercher d’abord les preuves).

- Offrir parrallèllement des **actions contextuelles** (Tenir un bouton appuyé quelques secondes (ou faire une série de boutons dans l'ordre pour ex. : envoyer un message au fanclub, supprimer le message d'un troll / lui répondre... pour stabiliser la jauge de popularité).

- Réussir le jeu mène à l'éditeur de niveau, ou l'on doit réussir le niveau créé pour pouvoir l'enregistrer dans le système. On pourrait faire en sorte que le niveau de difficulté soit calculer pour les mettre en ordre et inclure une variable dans le calcul pour faire un reclassement selon le nombre de personne qui ont réussit le niveau.

- À la fin du jeu on peut cliquer pour aller lire la suite du roman (un genre de jeu-pub).

-------

# Connaissances requises

### Recherche 1 de 3

Au début, je voulais automatiser l'importation d'un document googledoc pour que le texte puisse être modifiable, mais après une longue recherche j'ai déterminé que c'était trop long à implanter pour le cadre du cours. Voici quand même le résumé de cette recherche :

- Utiliser l'[API Google Docs](https://developers.google.com/docs/api) pour extraire le contenu du document en version pdf. Via files export (Configurer un projet dans la console Google Cloud et obtenir les identifiants nécessaires.)

- Utiliser une autre API (exemple pdf2image) pour  convertir le pdf en images.

- Sinon, chat gpt m'a proposé d'utiliser Python + Pillow
  "Pillow peut **rendre du texte sur une image**. Tu peux générer une image par page, avec la police que tu veux (ex. manuscrite). Exemple rapide :

`from PIL import Image, ImageDraw, ImageFont texte = "Ton texte long ici..." img = Image.new('RGB', (800, 1200), color=(255,255,255)) draw = ImageDraw.Draw(img) font = ImageFont.truetype("Cursive.ttf", 32) draw.text((20,20), texte, fill=(0,0,0), font=font) img.save("page1.png")`


Il m'a aussi expliqué la fonction OS.execute() pour appeler le script.

**Conclusion de la recherche 1 :** Je vais plutôt faire environ ces mêmes étapes, mais manuellement, quitte à n'avoir que du contenu fixe.



## Pistes d’algorithmes ou mécaniques à explorer

| Objectif                                                 | Type                                    | Description / Référence                                                                                                                                                                                                             |
| -------------------------------------------------------- | --------------------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Importation auto d'un texte                              |                                         | Annulé, surcharge de tavail suspectée                                                                                                                                                                                               |
| Génération automatique de niveaux                        | **Algorithme procédural**               | Génération basée sur des paramètres (densité de désordre, plage de sections affectées). Inspiration : algorithmes de « shuffle ».                                                                                                   |
| Gestion de la popularité du fanclub                      | **Système dynamique (IA simple)**       | Diminution continue du nombre de fans, ralentie par certaines actions (collection de preuves, messages). Peut être gérée via une fonction mathématique ajustable.                                                                   |
| Reconstruction du texte                                  | **Mécanique de tri / reconstitution**   | Système de glisser-déposer ou de permutation de fragments pour remettre les languettes en ordre.                                                                                                                                    |
| Éditeur de niveaux                                       | **Outil intégré**                       | Interface pour ajuster les paramètres de génération du niveau.                                                                                                                                                                      |
| Gestion asynchrone d’actions contextuelles               | **Algorithme de synchronisation**       | Permet d’effectuer des actions parallèles (ex. : maintenir un bouton appuyé pour “message au fanclub”) tout en continuant à interagir avec les slices. Gère la concurrence et les temporisations sans bloquer la boucle principale. |
| Permettre une **liberté de déplacement et de stratégie** | Pooling visuel pour afficher les slices | Le joueur peut parcourir un manuscrit de plusieurs centaines de slices sans ralentissement, tout en gardant la sensation d’un espace continu.                                                                                       |

---

## Étapes prévues

- [ ] Rechercher et tester des méthodes de découpage.
- [ ] Créer une première simulation de languettes désordonnées.
- [ ] Concevoir une logique simple de perte de fans dans le temps.
- [ ] Documenter les algorithmes choisis et leurs sources.

----------------------

Infos du prof : 
Exemple du prof pour la phase 1 de développement (phase de recherche) :

Je veux faire un jeu de type X. 

- Débuter par écouter des tutoriels, inscrire dans le document ceux qui sont d’intérêts et les moments clés du tutoriel. 

- Rechercher les mécaniques pour faire tel ou tel actions. Par exemple : 

- Comment faire suivre un trajet à des agents d’un « tower defense ». 

- Comment faire en sorte que des NPC (otages) suivent mon personnage principal. 

- Comment simuler le mode 7 sur Godot 

- Colliger les informations pertinentes dans le document avec leur source. 

Voici des exemples 

| Recherche de chemin dans un tower defense <br><br>Je voudrais que mes ennemis puissent se rendre vers la tour automatiquement même s’il y a des obstacles. <br><br>- Article « [Flow Field Pathfinding for Tower Defense](https://www.redblobgames.com/pathfinding/tower-defense/) » de RedBlobGames <br><br>- L’article parle de l’algorithme pour implanter le principe de champ de force dans une recherche de chemin.                                                          |
| ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Génération de carte procédurale <br><br>J’aimerais avoir des cartes qui se génèrent procéduralement. Dans la carte, je voudrais avoir des biomes différents. <br><br>- Article « [Making maps with noise functions](https://www.redblobgames.com/maps/terrain-from-noise/)» de RedBlobGames <br><br>- L’article traite sur une façon pour générer des cartes en utilisant des fonctions de bruits. Il parle aussi de la génération de taux d’humidité pour la génération de biome. |

Critères 

- L’information est colligée dans un document nommé « recherche.md » sous le dossier « docs ». 

- Chaque information possède au moins un lien vers la source 

- Chaque information possède un descriptif 

- Bonne attitude envers le projet. 

Remise 

- Avant la fin du cours du 9 octobre. 

- Sur Léa, vous remettez un fichier texte « remise.txt » avec un lien vers votre dépôt de code. 

- Sur GitHub, vous remplissez le formulaire « [Remise Git](http://remise.nicolasbourre.com/) ».

------

Points clefs dans l'énoncé du projet :


<title></title>

<style type="text/css">
 @page { size: 21.59cm 27.94cm; margin: 2cm }
 h3 { orphans: 2; color: #1f3763; line-height: 108%; text-align: left; page-break-inside: avoid; widows: 2; margin-top: 0.07cm; margin-bottom: 0cm; direction: ltr; background: transparent; page-break-after: avoid }
 h3.western { font-family: "Calibri Light", serif; font-size: 12pt }
 h3.cjk { font-size: 12pt; font-family: "游ゴシック Light" }
 h3.ctl { font-family: "Times New Roman"; font-size: 12pt }
 p { line-height: 115%; text-align: left; margin-bottom: 0.25cm; orphans: 2; widows: 2; direction: ltr; background: transparent }
 a:visited { color: #954f72; text-decoration: underline }
 a:link { color: #0563c1; text-decoration: underline }
 </style>

### Algorithmes et mécanismes de jeu

Vous
devez intégrer **au moins deux
algorithmes distincts**, développés **à
partir des principes de base**, sans
recourir à des bibliothèques externes. Par exemple :

- Génération
   de terrain ou de labyrinthe.

- Cinématique
   inverse.

- Intelligence
   artificielle.

Alternativement,
vous pouvez utiliser des **mécanismes
intégrés** à la plateforme de
création (ex. : Godot), comme le *raycasting*,
le *path following* ou le parallax. Dans ce cas, **le
double des points** sera requis pour
compenser l’absence de développement algorithmique.




<title></title>

<style type="text/css">
 @page { size: 21.59cm 27.94cm; margin: 2cm }
 p { line-height: 115%; text-align: left; margin-bottom: 0.25cm; orphans: 2; widows: 2; direction: ltr; background: transparent }
 a:visited { color: #954f72; text-decoration: underline }
 a:link { color: #0563c1; text-decoration: underline }
 </style>

Voici une liste non-exhaustive d’algorithmes que
des étudiants ont développé dans le passé :

|                                           |                                          |
| ----------------------------------------- | ---------------------------------------- |
| Algorithme<br> génétique                  | Générateur<br> de labyrinthe/donjon      |
| A*                                        | Réseau<br> de neurones                   |
| Essaimage<br> (Comportement d’agrégation) | Automate<br> cellulaire                  |
| Générateur<br> de piste de course         | Jeu<br> multi-joueur                     |
| Système<br> L-Tree                        | Cinématique<br> inverse                  |
| Génération<br> procédurale                | Agent<br> autonome                       |
| Fog<br> of war                            | Champ<br> de vision                      |
| Algorithme<br> de Prim                    | Système<br> de Voxel                     |
| Patron<br> de conception – State          | Path<br> following (Recherche de chemin) |
| Goal<br> Oriented Action Planning (GOAP)  | Patron<br> de conception – Object pool   |
| <br>                                      | <br>                                     |

Vous
pouvez aussi utiliser vos propres idées d’algorithme, il suffira
de me les faire approuver.


