using Godot;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace AjamaTruthseeker.AjamaTruthseekerCode.Nodes;

public partial class NSusMarker : TextureRect
{
	public TextureRect? Checkmark;
	public TextureRect? Flag;

	public override void _Ready()
	{
		Checkmark = GetNode<TextureRect>("Checkmark");
		Flag = GetNode<TextureRect>("Flag");
	}
}
