using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace AjamaTruthseeker.AjamaTruthseekerCode.Systems;

public class FlagCardAction(Player player) : GameAction
{
    private readonly Player _player = player;
    
    public override ulong OwnerId => _player.NetId;
    public override GameActionType ActionType => GameActionType.Combat;
    
    public override INetAction ToNetAction() => new FlagCardNetAction();
    public override string ToString() => $"FlagCardAction for player {OwnerId}";

    protected override async Task ExecuteAction()
    {
        
    }
}