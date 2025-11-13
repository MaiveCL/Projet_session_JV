using Godot;
using System;
using System.Collections.Generic;

public partial class Chapitre : Node2D
{
	[Export] public Texture2D BandeTexture;
	[Export] public int NbBandes = 176;
	[Export] public int LargeurBande;
	[Export] public int HauteurBande;
	[Export] public int Marge = 40;          // espace visuel entre bandes
	[Export] public ShaderMaterial ShadowMaterial;

	public override void _Ready()
	{
		LargeurBande = BandeTexture.GetWidth() / NbBandes;
		HauteurBande = BandeTexture.GetHeight();

		for (int i = 0; i < NbBandes; i++)
		{
			// Création du ColorRect pour le shader sous la bande
			var shadowRect = new ColorRect();
			shadowRect.Size = new Vector2(LargeurBande, HauteurBande);
			shadowRect.Material = ShadowMaterial;
			shadowRect.Color = new Color(1, 1, 1, 0);
			shadowRect.Position = new Vector2(i * LargeurBande + i * Marge, 0);
			AddChild(shadowRect);

			// Sprite2D
			var bande = new Sprite2D();
			bande.Texture = BandeTexture;
			bande.Hframes = NbBandes;
			bande.Vframes = 1;
			bande.Frame = i;
			bande.Centered = false;
			bande.Position = shadowRect.Position;
			AddChild(bande);
		}
	}
	public override void _Process(double delta)
	{
		// Shuffle sur touche H
		if (Input.IsActionJustPressed("hasard"))
		{
			ShuffleBandes();
		}
	}
	
	private void ShuffleBandes()
	{
		// Récupérer tous les Sprite2D enfants
		var bandes = new List<Sprite2D>();
		foreach (Node child in GetChildren())
		{
			if (child is Sprite2D sprite)
				bandes.Add(sprite);
		}

		// Mélanger les Frames seulement
		var rand = new Random();
		for (int i = bandes.Count - 1; i > 0; i--)
		{
			int j = rand.Next(i + 1);

			// Échange les frames
			int tempFrame = bandes[i].Frame;
			bandes[i].Frame = bandes[j].Frame;
			bandes[j].Frame = tempFrame;
		}
	}
}
