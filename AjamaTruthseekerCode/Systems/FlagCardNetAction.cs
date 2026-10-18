using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Multiplayer.Serialization;

namespace AjamaTruthseeker.AjamaTruthseekerCode.Systems;

public class FlagCardNetAction : INetAction
{
    public int Index;
    
    public GameAction ToGameAction(Player player) => new FlagCardAction(player, Index);

    public void Serialize(PacketWriter writer)
    {
        writer.WriteInt(Index);
    }

    public void Deserialize(PacketReader reader)
    {
        Index = reader.ReadInt();
    }

    public override string ToString() => nameof(FlagCardNetAction);
}