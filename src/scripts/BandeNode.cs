using Godot;
using System;

public partial class BandeNode : Node2D
{
	public Chapitre Chapitre => GetChapitre();
	public Chapitre GetChapitre()
	{
		Node current = this;
		while (current != null && !(current is Chapitre))
			current = current.GetParent<Node>();
		return current as Chapitre;
	}
	
	public Node2D Joueur => GetJoueur();

	public Node2D GetJoueur()
	{
		Node current = this;
		while (current != null && !(current is Node2D && current.Name == "Auteur"))
			current = current.GetParent<Node>();
		return current as Node2D;
	}

	public int ChapitreId { get; private set; } = 0;
	public int FrameIndex { get; private set; } = 0;
	public int TotalHFrames { get; private set; } = 1;

	private ColorRect ombrage;
	private Sprite2D tranche;
	private BandesStateMachine machine;

	public override void _Ready()
	{
		ombrage = GetNode<ColorRect>("ombrage");
		tranche = GetNode<Sprite2D>("tranche");
		machine = tranche.GetNode<BandesStateMachine>("BandesStateMachine");
	}

	public void Configure(int chapitreId, Texture2D texture, int totalHFrames, int frameIndex, ShaderMaterial shadowMaterial, Vector2 initialPosition, int largeurBande, int hauteurBande)
	{
		ChapitreId = chapitreId;
		FrameIndex = frameIndex;
		TotalHFrames = totalHFrames;

		Position = initialPosition;

		if (tranche != null)
		{
			tranche.Texture = texture;
			tranche.Hframes = TotalHFrames;
			tranche.Vframes = 1;
			tranche.Frame = FrameIndex;
			tranche.Centered = true;
			tranche.Position = Vector2.Zero;
		}

		// configure ombrage
		if (ombrage != null)
		{
			ombrage.Size = new Vector2(largeurBande, hauteurBande);
			if (shadowMaterial != null)
				ombrage.Material = shadowMaterial;
			ombrage.Color = new Color(1, 1, 1, 0);
			//ombrage.Position = Vector2.Zero;
			ombrage.Position = new Vector2(-largeurBande / 2f, -hauteurBande / 2f);

		}

		if (machine != null)
			machine.ChangeState("Idle");
	}

	public void SetFrame(int newFrame)
	{
		FrameIndex = newFrame;
		if (tranche != null)
			tranche.Frame = newFrame;
	}

	public void SetLocalPosition(Vector2 localPos)
	{
		Position = localPos;
	}

	public float GetWidth()
	{
		if (tranche?.Texture != null)
			return tranche.Texture.GetWidth() / Math.Max(1, tranche.Hframes);
		return 0f;
	}

	public void ShowShadow(bool value)
	{
		if (ombrage != null)
			ombrage.Visible = value;
	}

	public BandesStateMachine GetStateMachine()
	{
		return machine;
	}
}
