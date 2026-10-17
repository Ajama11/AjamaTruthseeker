using AjamaTruthseeker.AjamaTruthseekerCode.Singletons;
using Godot;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Runs;

namespace AjamaTruthseeker.AjamaTruthseekerCode.Nodes;

public partial class NObscuredCount : Control
{
	private MegaLabel? _label;
	private Player? _localPlayer;
	private PlayerCombatState? _combatState;

	public override void _Ready()
	{
		_label = GetNode<MegaLabel>("InnerContainer/Label");

		if (RunManager.Instance.State != null)
			_localPlayer = LocalContext.GetMe(RunManager.Instance.State.Players);

		ForesightSingleton.NumberOfObscuredCardsInForesightChanged += OnObscuredNumberChanged;

		Visible = false;
	}

	private void OnObscuredNumberChanged()
	{
		if (!_label.IsValid()) return;

		_combatState ??= _localPlayer?.PlayerCombatState;
		if (_combatState == null) return;

		int number = ForesightSingleton.NumberOfObscuredCardsInForesight[_combatState];

		_label!.SetTextAutoSize(number.ToString());

		Visible = number > 0;
	}

	public override void _ExitTree()
	{
		ForesightSingleton.NumberOfObscuredCardsInForesightChanged -= OnObscuredNumberChanged;
	}
}
