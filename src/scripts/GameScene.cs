using Godot;
using System;

public partial class GameScene : Node
{
	[Export] public float FansMax = 10000f;
	[Export] public float PerteParSeconde = 22f;

	private float fans;
	private Lecteurs overlay;
	private Label debugLabel;
	
	public event Action GameOverDetecte;

	public override void _Ready()
	{
		var settings = GetNode<Settings>("/root/Main");
		FansMax = settings.FansDepart;
		PerteParSeconde = settings.PerteParSeconde;

		fans = FansMax;
		overlay = GetNode<Lecteurs>("../GameInfos/Lecteurs");
		debugLabel = GetNode<Label>("../GameInfos/DebugLabel");
		debugLabel.Visible = false;
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

		// Toggle debug avec F12
		if (Input.IsActionJustPressed("debug"))
			debugLabel.Visible = !debugLabel.Visible;

		// Mise à jour debug si visible
		if (debugLabel.Visible)
		{
			int fps = (int)Performance.GetMonitor(Performance.Monitor.TimeFps);
			float mem = (float)(Performance.GetMonitor(Performance.Monitor.MemoryStatic) / (1024.0 * 1024.0));
			debugLabel.Text = $"FPS: {fps}\nRAM: {mem:F1} MB";
		}
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
