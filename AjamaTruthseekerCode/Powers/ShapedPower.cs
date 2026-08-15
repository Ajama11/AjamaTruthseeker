using AjamaTruthseeker.AjamaTruthseekerCode.DynamicVars;
using AjamaTruthseeker.AjamaTruthseekerCode.Extensions;
using AjamaTruthseeker.AjamaTruthseekerCode.Powers;
using AjamaTruthseeker.AjamaTruthseekerCode.Utils;
using BaseLib.Extensions;
using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using static AjamaTruthseeker.AjamaTruthseekerCode.Utils.Shape;

namespace AjamaTruthseeker.AjamaTruthseekerCode.Powers;

public class ShapedPower() : AjamaTruthseekerPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Single;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<StrengthPower>(2),
        new ForesightVar(1),
        new EnergyVar(1)
    ];
    
    private async Task PerformEntranceEffect()
    {
        Data data = GetInternalData<Data>();

        switch (data.CurrentShape)
        {
            case ShapeType.Abstract:
                await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner.Player!);
                break;
            case ShapeType.Weapon:
                await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), 
                    Owner, DynamicVars.Power<StrengthPower>().BaseValue, 
                    Owner, null);
                break;
            case ShapeType.Friend:
                await PowerCmd.Apply<ForesightPower>(new ThrowingPlayerChoiceContext(), 
                    Owner, DynamicVars.Foresight.BaseValue, 
                    Owner, null);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public override Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner)) return Task.CompletedTask;
        
        Data data = GetInternalData<Data>();

        foreach (var shape in data.EntranceTriggeredThisTurn.Keys)
        {
            data.EntranceTriggeredThisTurn[shape] = false;
        }
        
        return Task.CompletedTask;
    }

    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        if (!Owner.IsPlayer)
        {
            await PowerCmd.Remove(this);
            return;
        }
        
        ReloadIcons();
    }

    private void ReloadIcons()
    {
        foreach (NPower nPower in NCombatRoom.Instance!.GetCreatureNode(Owner)!._stateDisplay._powerContainer._powerNodes)
        {
            if (nPower.Model != this) continue;
            
            nPower.Reload();
            break;
        }
    }
    
    public async Task SetCurrentShape(ShapeType shapeType, bool firstApplication = false)
    {
        shapeType = GetBaseShape(shapeType);
        Data data = GetInternalData<Data>();
        
        data.CurrentShape = shapeType;
        
        if (!firstApplication) ReloadIcons();

        if (!firstApplication && !data.EntranceTriggeredThisTurn[shapeType])
        {
            data.EntranceTriggeredThisTurn[shapeType] = true;
            await PerformEntranceEffect();
        }
    }

    public ShapeType GetCurrentShape()
    {
        return GetInternalData<Data>().CurrentShape;
    }

    public bool IsShape(ShapeType shapeType)
    {
        return GetInternalData<Data>().CurrentShape == GetBaseShape(shapeType);
    }
    
    protected override object InitInternalData()
    {
        Data data = new Data();

        foreach (var shape in Enum.GetValues<ShapeType>().Where(s => !IsRequired(s)))
        {
            data.EntranceTriggeredThisTurn[shape] = false;
        }
        
        return data;
    }

    public class Data
    {
        public ShapeType CurrentShape = ShapeType.Abstract;
        public Dictionary<ShapeType, bool> EntranceTriggeredThisTurn = [];
    }
    
    public override LocString Title =>
        IsCanonical ?
            new LocString("powers", $"{Id.Entry}_ABSTRACT.title") :
            new LocString("powers", $"{Id.Entry}_{GetName(GetInternalData<Data>().CurrentShape)}.title");

    protected override string SmartDescriptionLocKey =>
        IsCanonical ?
            $"{Id.Entry}_ABSTRACT.smartDescription" :
            $"{Id.Entry}_{GetName(GetInternalData<Data>().CurrentShape)}.smartDescription";

    public override string CustomPackedIconPath =>
        IsCanonical ?
            $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_abstract.png".PowerImagePath() :
            $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_{GetName(GetInternalData<Data>().CurrentShape).ToLowerInvariant()}.png".PowerImagePath();

    public override string CustomBigIconPath =>
        IsCanonical ?
            $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_abstract.png".BigPowerImagePath() :
            $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_{GetName(GetInternalData<Data>().CurrentShape).ToLowerInvariant()}.png".BigPowerImagePath();
}