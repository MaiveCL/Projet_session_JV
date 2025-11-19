using Godot;
using System;

public partial class BandeNode : Node2D
{
	/// Métadonnées
	public int ChapitreId { get; private set; } = 0;   // index de la source d'image (0 = chap1)
	public int FrameIndex { get; private set; } = 0; // index dans la sprite sheet (frame)
	public int TotalHFrames { get; private set; } = 1;

	/// noeuds
	private ColorRect ombrage;
	private Sprite2D tranche;
	private StateMachine machine;

	public override void _Ready()
	{
		// récupérer les nodes existants
		ombrage = GetNode<ColorRect>("ombrage");
		tranche = GetNode<Sprite2D>("tranche");
		machine = tranche.GetNode<StateMachine>("StateMachine");

		// cacher tant qu'on configure
		Visible = true;
	}

	/// <summary>
	/// texture: sprite sheet
	/// totalHFrames: nombre total de frames (NbBandes)
	/// frameIndex: index assigné à cette bande
	/// </summary>
	public void Configure(int ChapitreId, Texture2D texture, int totalHFrames, int frameIndex, ShaderMaterial shadowMaterial, Vector2 initialPosition, int largeurBande, int hauteurBande)
	{
		ChapitreId = ChapitreId;
		FrameIndex = frameIndex;
		TotalHFrames = Math.Max(1, totalHFrames);

		// position initiale de la bande (local)
		Position = initialPosition;

		// configure slice / sprite
		if (tranche != null)
		{
			tranche.Texture = texture;
			tranche.Hframes = TotalHFrames;
			tranche.Vframes = 1;
			tranche.Frame = FrameIndex;
			tranche.Centered = false;
			tranche.Position = Vector2.Zero; // local inside this node
		}

		// configure ombrage
		if (ombrage != null)
		{
			ombrage.Size = new Vector2(largeurBande, hauteurBande);
			if (shadowMaterial != null)
				ombrage.Material = shadowMaterial;
			ombrage.Color = new Color(1, 1, 1, 0);
			ombrage.Position = Vector2.Zero;
		}

		if (machine != null)
			machine.ChangeState("Idle");
	}

	// API pour manipuler la bande depuis Chapitre / States
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

	public void ShowShadow(bool enabled)
	{
		if (ombrage != null)
			ombrage.Color = enabled ? new Color(0, 0, 0, 0.5f) : new Color(1, 1, 1, 0);
	}

	public StateMachine GetStateMachine()
	{
		return machine;
	}
}
