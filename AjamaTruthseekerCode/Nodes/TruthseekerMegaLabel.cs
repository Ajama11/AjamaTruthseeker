using Godot;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Assets;

namespace AjamaTruthseeker.AjamaTruthseekerCode.Nodes;

[GlobalClass]
public partial class TruthseekerMegaLabel : MegaLabel
{
	private const string MegaLabelFont = "res://themes/kreon_bold_glyph_space_one.tres";

	public override void _Ready()
	{
		AddThemeFontOverride(ThemeConstants.Label.Font, PreloadManager.Cache.GetAsset<Font>(MegaLabelFont));
		base._Ready();
	}
}
