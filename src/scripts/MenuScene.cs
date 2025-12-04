using Godot;
using System;

public partial class MenuScene : Control
{
	[Export] private Control Menu;      // le VBox principal
	[Export] private Control SousMenu;  // le VBox du sous-menu

	// Références exportées pour assigner dans l'inspecteur
	[Export] private Button startButton;
	[Export] private Button aide;
	[Export] private AcceptDialog dialog;
	[Export] private Button quitButton;

	// Nouvelles références pour le sous-menu Options
	[Export] private Button optionsButton;
	[Export] private Control optionsPanel; // Panel contenant les toggles
	[Export] private CheckButton musicToggle;
	[Export] private CheckButton ActionToggle;
	[Export] private CheckButton easyModeToggle;

	public override void _Ready()
	{
		startButton.Pressed += OnStartPressed;
		aide.Pressed += OnCommentJouerPressed;
		quitButton.Pressed += OnQuitPressed;

		// Sous-menu Options
		if (optionsButton != null)
			optionsButton.Pressed += OnOptionsPressed;

		if (optionsPanel != null)
			optionsPanel.Visible = false; // cacher au départ

		// Assign toggles
		if (musicToggle != null)
			musicToggle.Toggled += _OnMusicToggleToggled;
		if (ActionToggle != null)
			ActionToggle.Toggled += _OnActionToggleToggled;
		if (easyModeToggle != null)
			easyModeToggle.Toggled += _OnEasyModeToggleToggled;

		SetupParallaxForMenu();
	}

	private void OnStartPressed()
	{
		GetNode<MondeStateMachine>("/root/Main/MondeStateMachine")
			.ChangeState("GameState");
	}

	private void OnCommentJouerPressed()
	{
		// Affiche le popup
		dialog.Popup();
	}

	private void OnQuitPressed()
	{
		GetTree().Quit();
	}

	// Gestion du sous-menu Options
	private void OnOptionsPressed()
	{
		bool ouvrir = !SousMenu.Visible;

		SousMenu.Visible = ouvrir;
		Menu.Visible = !ouvrir;
	}

	private void _OnMusicToggleToggled(bool pressed)
	{
		var musique = GetNode<AudioStreamPlayer>("/root/Main/Musique");
		if (musique != null)
			musique.Playing = pressed;
	}

	private void _OnActionToggleToggled(bool pressed)
	{
		var sfx = GetNode<AudioStreamPlayer>("/root/Main/Action");
		if (sfx != null)
			sfx.Playing = pressed;
	}

	private void _OnEasyModeToggleToggled(bool pressed)
	{
		var easy = GetNode<AudioStreamPlayer>("/root/Main/Facile");
		if (easy != null)
			easy.Playing = pressed;

		// Stocker l'état dans Main pour que le jeu sache si EasyMode est activé
		var main = GetNode<Node>("/root/Main");
		if (main != null)
			main.Set("EasyModeEnabled", pressed);
	}

	private void SetupParallaxForMenu()
	{
		var tableJeu = GetNode<TableJeu>("TableJeu");
		if (tableJeu != null)
		{
			tableJeu.ScrollSpeed = new Vector2(-10, 0);
		}
	}
}
