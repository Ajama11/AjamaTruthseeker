using AjamaTruthseeker.AjamaTruthseekerCode.BundledHoverTips.Core;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;

namespace AjamaTruthseeker.AjamaTruthseekerCode.CardModifiers;

public class AbstractTheoryMod : CardModifier, ICustomModel
{
    public virtual BundledHoverTipManager MyBundles => [];
    public virtual List<List<string>> BundleReorders => [];
    
    public virtual void AddExtraArgsToLoc(LocString loc) { }
    
    public CardModel Card => Owner!;
    public Player Player => Owner?.Owner!;
    public ICombatState? CombatState => Owner?.CombatState;
    public RunRngSet Rng => Player.RunState.Rng;
    
    public LocString SelectionScreenPrompt => GetLoc("selectionScreenPrompt");
    
    public override void ModifyDescription(Creature? target, ref string description)
    {
        if (!LocString.Exists("card_modifiers", $"{Id.Entry}.description"))
            return;
        
        if (description.Length > 0) description += "\n";
        description += GetLoc().GetFormattedText();
    }
    
    // Replace this with Theory handling the final collection! Otherwise the Bundles won't actually be sorted
    public override void AddTips(List<IHoverTip> tips)
    {
        tips.AddRange(MyBundles.GetHoverTips());
    }
    
    public override LocString GetLoc(string subKey = "description")
    {
        LocString loc = base.GetLoc(subKey);
        
        if (Owner == null) return loc;
        
        AddExtraArgsToLoc(loc);

        return loc;
    }
}