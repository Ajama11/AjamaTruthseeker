using AjamaTruthseeker.AjamaTruthseekerCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace AjamaTruthseeker.AjamaTruthseekerCode.Utils;

public class Shape
{
    public enum ShapeType
    {
        Abstract,
        AbstractRequired,
        Weapon,
        WeaponRequired,
        Friend,
        FriendRequired
    }

    public static IEnumerable<IHoverTip> GetHoverTips(ShapeType shapeType)
    {
        return shapeType switch
        {
            ShapeType.Abstract =>
            [
                HoverTipFactory.Static(MyEnums.AbstractShape), HoverTipFactory.FromPower<ShapedPowerAbstract>()
            ],
            ShapeType.AbstractRequired =>
            [
                HoverTipFactory.Static(MyEnums.AbstractRequired), HoverTipFactory.FromPower<ShapedPowerAbstract>()
            ],
            ShapeType.Weapon =>
            [
                HoverTipFactory.Static(MyEnums.WeaponShape), HoverTipFactory.FromPower<ShapedPowerWeapon>()
            ],
            ShapeType.WeaponRequired =>
            [
                HoverTipFactory.Static(MyEnums.WeaponRequired), HoverTipFactory.FromPower<ShapedPowerWeapon>()
            ],
            ShapeType.Friend =>
            [
                HoverTipFactory.Static(MyEnums.FriendShape), HoverTipFactory.FromPower<ShapedPowerFriend>()
            ],
            ShapeType.FriendRequired =>
            [
                HoverTipFactory.Static(MyEnums.FriendRequired), HoverTipFactory.FromPower<ShapedPowerFriend>()
            ],
            _ => throw new ArgumentOutOfRangeException(nameof(shapeType), shapeType, null)
        };
    }
    
    public static PowerModel GetDummyPower(ShapeType shapeType)
    {
        return shapeType switch
        {
            ShapeType.Abstract or ShapeType.AbstractRequired =>
                ModelDb.Power<ShapedPowerAbstract>(),
            ShapeType.Weapon or ShapeType.WeaponRequired =>
                ModelDb.Power<ShapedPowerWeapon>(),
            ShapeType.Friend or ShapeType.FriendRequired =>
                ModelDb.Power<ShapedPowerFriend>(),
            _ => throw new ArgumentOutOfRangeException(nameof(shapeType), shapeType, null)
        };
    }

    public static bool IsRequired(ShapeType shapeType)
    {
        return shapeType switch
        {
            ShapeType.Abstract or 
                ShapeType.Weapon or 
                ShapeType.Friend 
                => false,
            ShapeType.AbstractRequired or 
                ShapeType.WeaponRequired or 
                ShapeType.FriendRequired 
                => true,
            _ => throw new ArgumentOutOfRangeException(nameof(shapeType), shapeType, null)
        };
    }
    
    public static string GetName(ShapeType shapeType)
    {
        return shapeType switch
        {
            ShapeType.Abstract or ShapeType.AbstractRequired =>
                "ABSTRACT",
            ShapeType.Weapon or ShapeType.WeaponRequired =>
                "WEAPON",
            ShapeType.Friend or ShapeType.FriendRequired =>
                "FRIEND",
            _ => throw new ArgumentOutOfRangeException(nameof(shapeType), shapeType, null)
        };
    }
    
    /// <summary>
    /// Turns the Required shapes into their basic counterparts. For example, WeaponRequired becomes Weapon.
    /// </summary>
    public static ShapeType GetBaseShape(ShapeType shapeType)
    {
        return shapeType switch
        {
            ShapeType.Abstract or ShapeType.AbstractRequired =>
                ShapeType.Abstract,
            ShapeType.Weapon or ShapeType.WeaponRequired =>
                ShapeType.Weapon,
            ShapeType.Friend or ShapeType.FriendRequired =>
                ShapeType.Friend,
            _ => throw new ArgumentOutOfRangeException(nameof(shapeType), shapeType, null)
        };
    }

    public static async Task SetShape(Creature creature, ShapeType shapeType, PlayerChoiceContext choiceContext)
    {
        ShapedPower? shapedPower = creature.GetPower<ShapedPower>();
        
        if (shapedPower == null)
        {
            shapedPower = ((ShapedPower) ModelDb.Power<ShapedPower>().ToMutable());
                    
            await shapedPower.SetCurrentShape(shapeType, true);
                    
            await PowerCmd.Apply(choiceContext, shapedPower, creature, 1, creature, null);
        }
        else
        {
            await shapedPower.SetCurrentShape(shapeType);
                
            await PowerCmd.Apply<ShapedPower>(choiceContext,
                creature, 1,
                creature, null);
        }
    }
}