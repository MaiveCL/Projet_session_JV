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
	[Export] private Button BackButton;
	
	private Settings settings;
	private MondeStateMachine stateMachine;
	private AudioStreamPlayer musique;
	private AudioStreamPlayer action;
	private AudioStreamPlayer facile;
	
	public void Inject(Settings s, MondeStateMachine sm,
		AudioStreamPlayer m, AudioStreamPlayer a, AudioStreamPlayer f)
	{
		settings = s;
		stateMachine = sm;
		musique = m;
		action = a;
		facile = f;
	}

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
		if (BackButton != null)
			BackButton.Pressed += OnBackPressed;

		SetupParallaxForMenu();
	}

	private void OnStartPressed()
	{
		stateMachine.ChangeState("GameState");
	}

	private void OnCommentJouerPressed()
	{
		dialog.Popup();
	}

	private void OnQuitPressed()
	{
		GetTree().Quit();
	}

	private void OnOptionsPressed()
	{
		bool ouvrir = !SousMenu.Visible;

		SousMenu.Visible = ouvrir;
		Menu.Visible = !ouvrir;

		if (ouvrir)
			SynchroniserOptionsAvecLeJeu();
	}

	private void _OnMusicToggleToggled(bool pressed)
	{
		if (musique != null)
			musique.Playing = pressed;

		if (settings != null)
			settings.MusicEnabled = pressed;
	}

	private void _OnActionToggleToggled(bool pressed)
	{
		if (action != null)
			action.Playing = pressed;

		if (settings != null)
			settings.ActionEnabled = pressed;
	}

	private void _OnEasyModeToggleToggled(bool pressed)
	{
		if (facile != null)
			facile.Playing = pressed;

		if (settings != null)
			settings.EasyModeEnabled = pressed;
	}

	private void SetupParallaxForMenu()
	{
		var tableJeu = GetNode<TableJeu>("TableJeu");
		if (tableJeu != null)
		{
			tableJeu.ScrollSpeed = new Vector2(-10, 0);
		}
	}
	
	private void SynchroniserOptionsAvecLeJeu()
	{
		if (settings == null) return;

		if (musicToggle != null)
			musicToggle.ButtonPressed = settings.MusicEnabled;

		if (ActionToggle != null)
			ActionToggle.ButtonPressed = settings.ActionEnabled;

		if (easyModeToggle != null)
			easyModeToggle.ButtonPressed = settings.EasyModeEnabled;

		if (musique != null) musique.Playing = settings.MusicEnabled;
		if (action != null) action.Playing = settings.ActionEnabled;
		if (facile != null) facile.Playing = settings.EasyModeEnabled;
	}
	
	private void OnBackPressed()
	{
		SousMenu.Visible = false;
		Menu.Visible = true;
	}

}
