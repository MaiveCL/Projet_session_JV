using Godot;
using System;

public partial class GameScene : Node
{
	[Export] public float FansMax = 10000f;
	[Export] public float PerteParSeconde = 12f;

	private float fans;
	private Lecteurs overlay;
	
	public event Action GameOverDetecte;

	public override void _Ready()
	{
		fans = FansMax;
		overlay = GetNode<Lecteurs>("../GameInfos/Lecteurs");
		MAJUI();
	}

	public override void _Process(double delta)
	{
		var settings = GetNode<Settings>("/root/Main");
		if (settings.IsPaused)
			return;
			
		// Logique normale
		fans -= PerteParSeconde * (float)delta;
		fans = Mathf.Max(fans, 0);
		MAJUI();
		if (fans <= 0)
			GameOver();
			
		if (Input.IsActionJustPressed("triche_defaite"))
			fans = 50;
	}

	public void AjouterFans(float valeur)
	{
		fans = Mathf.Clamp(fans + valeur, 0, FansMax);
		MAJUI();
	}

	private void MAJUI()
	{
		overlay?.SetValeur(fans / FansMax);
	}

	private void GameOver()
	{
		// GD.Print("Le public s'est évaporé.");
		GameOverDetecte?.Invoke();
	}
}
