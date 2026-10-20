using AjamaTruthseeker.AjamaTruthseekerCode.Extensions;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace AjamaTruthseeker.AjamaTruthseekerCode.Systems;

public class FlagCardAction(Player player, int index) : GameAction
{
    private readonly Player _player = player;
    private readonly int _index = index;
    
    public override ulong OwnerId => _player.NetId;
    public override GameActionType ActionType => GameActionType.Combat;
    
    public override INetAction ToNetAction() => new FlagCardNetAction()
    {
        Index = _index
    };
    
    public override string ToString() => $"FlagCardAction for player {OwnerId} for Foresight card {_index}";

    protected override Task ExecuteAction()
    {
        if (_player.DrawPile.Count <= _index) return Task.CompletedTask;
        
        CardModel card = _player.DrawPile[_index];
        
        Proven.ToggleFlag(card);

        return Task.CompletedTask;
    }
}