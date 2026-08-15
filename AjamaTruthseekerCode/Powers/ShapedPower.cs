using AjamaTruthseeker.AjamaTruthseekerCode.Extensions;
using AjamaTruthseeker.AjamaTruthseekerCode.Powers;
using BaseLib.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
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

    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
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
    
    public void SetCurrentShape(ShapeType shapeType, bool reloadIcons = true)
    {
        GetInternalData<Data>().CurrentShape = shapeType;
        if (reloadIcons) ReloadIcons();
    }

    public ShapeType GetCurrentShape()
    {
        return GetInternalData<Data>().CurrentShape;
    }

    public bool IsShape(ShapeType shapeType)
    {
        return GetInternalData<Data>().CurrentShape == GetBaseShape(shapeType);
    }
    
    protected override object InitInternalData() => new Data();
    
    public class Data
    {
        public ShapeType CurrentShape = ShapeType.Abstract;
    }
}