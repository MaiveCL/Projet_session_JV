### Recherche 4 de 4 — Gestion dynamique de la popularité

Dans le jeu, la popularité du fanclub décroît continuellement au fil du temps, mais certaines actions du joueur peuvent ralentir ou inverser cette tendance (par exemple, répondre à un troll ou publier un extrait du roman).  

#### Objectif

Créer un système dynamique qui simule la **fluctuation de la popularité** en fonction du temps et des actions du joueur, afin d’ajouter une pression constante et un enjeu stratégique.

#### Sources et inspirations

- Les jeux de type *idle* ou *tower defense* utilisent souvent des modèles de décroissance **linéaire** ou **exponentielle** pour représenter la perte de ressources au fil du temps.  
- Le [système de réputation de *Civilization*](https://civilization.fandom.com/wiki/Reputation) et le [système de relations de *Stardew Valley*](https://stardewvalleywiki.com/Heart) montrent comment des valeurs chiffrées peuvent évoluer selon des événements positifs ou négatifs.  
- Dans Godot, ce type de mécanique peut être géré par un **Timer** ou une **coroutine** qui ajuste une variable globale sans bloquer la boucle principale du jeu.

#### Exemple de pseudo-code

```gdscript
var fans = 100.0
var decay_rate = 0.2
var bonus = 0.0

func _process(delta):
    fans -= decay_rate * delta
    fans = clamp(fans + bonus, 0, 100)
