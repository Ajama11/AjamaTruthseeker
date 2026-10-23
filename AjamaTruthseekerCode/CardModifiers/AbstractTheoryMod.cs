using AjamaTruthseeker.AjamaTruthseekerCode.BundledHoverTips;
using AjamaTruthseeker.AjamaTruthseekerCode.BundledHoverTips.Core;
using AjamaTruthseeker.AjamaTruthseekerCode.Cards.Status;
using AjamaTruthseeker.AjamaTruthseekerCode.DynamicVars;
using AjamaTruthseeker.AjamaTruthseekerCode.Powers;
using AjamaTruthseeker.AjamaTruthseekerCode.Utils;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;

namespace AjamaTruthseeker.AjamaTruthseekerCode.CardModifiers;

public class AbstractTheoryMod : CardModifier, ICustomModel
{
    public virtual BundledHoverTipManager MyTheoryBundles => [];
    public virtual List<List<string>> BundleReorders => [];
    public virtual bool IsAttack => false;
    public virtual TargetType Targeting => TargetType.Self;
    
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
    
    public override LocString GetLoc(string subKey = "description")
    {
        LocString loc = base.GetLoc(subKey);
        
        if (Owner == null) return loc;
        
        AddExtraArgsToLoc(loc);

        return loc;
    }
    
    public BundledHoverTipManager BundlesToUse
    {
        get
        {
            BundledHoverTipManager bundles = [];

            if (DynamicVars.Values.Any(dv => dv is ForesightVar { SkipTooltip: false }))
            {
                bundles.Add(BundledHoverTipFactory.FromPower<ForesightPower>());
            }
            
            if (DynamicVars.Values.Any(dv => dv is ObscureVar { SkipTooltip: false }))
            {
                bundles.Add(new ObscureBundle());
            }
            
            if (DynamicVars.Values.Any(dv => dv is ProveVar { SkipTooltip: false }))
            {
                bundles.Add(new ProveBundle());
            }
            
            if (DynamicVars.Values.Any(dv => dv is CertifyVar { SkipTooltip: false }))
            {
                bundles.Add(new CertifyBundle());
            }
            
            if (DynamicVars.Values.Any(dv => dv is NihilityVar { SkipTooltip: false }))
            {
                bundles.Add(BundledHoverTipFactory.FromCard<Nihility>());
            }
            
            foreach (var myPowerVar in DynamicVars.Values.OfType<IPowerHoverTip>())
            {
                var powerHoverTip = myPowerVar.GetPowerHoverTip();
                if (powerHoverTip.SkipTooltip) continue;
                
                bundles.Add(new BundledHoverTip(powerHoverTip.Name, powerHoverTip.HoverTip));
            }

            foreach (var bundle in MyTheoryBundles)
            {
                bundles.Add(bundle);
            }

            foreach (var list in BundleReorders)
            {
                bundles.Reorder(list[0], list.Skip(1).ToArray());
            }

            bundles.SortHoverTips();
            return bundles;
        }
    }
}