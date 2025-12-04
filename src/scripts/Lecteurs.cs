using Godot;
using System;

public partial class Lecteurs : ProgressBar
{
	[Export] public float Marge = 40f; // marge au-dessus

	private Chapitre chapitreNode;

	public override void _Ready()
	{
		chapitreNode = GetNode<Chapitre>("/root/Main/SceneContainer/Monde/TableauJeu/Chapitre");
		AjusterTaille();
	}

	public override void _Process(double delta)
	{
		AjusterTaille(); // si la fenêtre change de taille
	}

	public void SetValeur(float ratio)
	{
		Value = ratio * MaxValue;
	}

	private void AjusterTaille()
	{
		if (chapitreNode == null || chapitreNode.BandesPool == null || chapitreNode.BandesPool.Count == 0)
			return;

		// Largeur → 100% du viewport
		float largeur = GetViewport().GetVisibleRect().Size.X;

		// Hauteur → distance entre le haut et la première bande moins la marge
		float hauteur = Math.Max(0, chapitreNode.BandesPool[0].GlobalPosition.Y - Marge);

		// On utilise CustomMinimumSize pour définir la taille
		CustomMinimumSize = new Vector2(largeur, hauteur);
	}
}
