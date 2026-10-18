using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Multiplayer.Serialization;

namespace AjamaTruthseeker.AjamaTruthseekerCode.Systems;

public class FlagCardNetAction : INetAction
{
    public GameAction ToGameAction(Player player) => new FlagCardAction(player);
    
    public void Serialize(PacketWriter writer) { }
    public void Deserialize(PacketReader reader) { }

    public override string ToString() => nameof(FlagCardNetAction);
}