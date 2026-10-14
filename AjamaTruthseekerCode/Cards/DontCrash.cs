using AjamaTruthseeker.AjamaTruthseekerCode.Extensions;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace AjamaTruthseeker.AjamaTruthseekerCode.Cards;

public abstract class DontCrash(CardType type, CardRarity rarity) : AjamaTruthseekerCard(0, type, rarity, TargetType.None)
{
    public override string CustomPortraitPath => "card.png".BigCardImagePath();

    public override string PortraitPath => "card.png".CardImagePath();

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CardPileCmd.Draw(choiceContext, Owner);

        if (DeckVersion is not null)
        {
            await CardPileCmd.RemoveFromDeck(DeckVersion);
        }
    }

    public override List<(string, string)> Localization => new CardLoc("Don't Crash", "Don't Crash.");
}


public class DontCrashCommonAtk1() : DontCrash(CardType.Attack, CardRarity.Common)
{
}


public class DontCrashCommonAtk2() : DontCrash(CardType.Attack, CardRarity.Common)
{
}


public class DontCrashCommonAtk3() : DontCrash(CardType.Attack, CardRarity.Common)
{
}

public class DontCrashCommonAtk4() : DontCrash(CardType.Attack, CardRarity.Common)
{
}


public class DontCrashCommonSkl1() : DontCrash(CardType.Skill, CardRarity.Common)
{
}

public class DontCrashCommonSkl2() : DontCrash(CardType.Skill, CardRarity.Common)
{
}

public class DontCrashCommonSkl3() : DontCrash(CardType.Skill, CardRarity.Common)
{
}

public class DontCrashCommonSkl4() : DontCrash(CardType.Skill, CardRarity.Common)
{
}

public class DontCrashUncommonAtk1() : DontCrash(CardType.Attack, CardRarity.Uncommon)
{
}


public class DontCrashUncommonAtk2() : DontCrash(CardType.Attack, CardRarity.Uncommon)
{
}


public class DontCrashUncommonAtk3() : DontCrash(CardType.Attack, CardRarity.Uncommon)
{
}


public class DontCrashUncommonSkl1() : DontCrash(CardType.Skill, CardRarity.Uncommon)
{
}

public class DontCrashUncommonSkl2() : DontCrash(CardType.Skill, CardRarity.Uncommon)
{
}

public class DontCrashUncommonSkl3() : DontCrash(CardType.Skill, CardRarity.Uncommon)
{
}


public class DontCrashUncommonPow1() : DontCrash(CardType.Power, CardRarity.Uncommon)
{
}

public class DontCrashUncommonPow2() : DontCrash(CardType.Power, CardRarity.Uncommon)
{
}

public class DontCrashUncommonPow3() : DontCrash(CardType.Power, CardRarity.Uncommon)
{
}


public class DontCrashRareAtk1() : DontCrash(CardType.Attack, CardRarity.Rare)
{
}


public class DontCrashRareAtk2() : DontCrash(CardType.Attack, CardRarity.Rare)
{
}


public class DontCrashRareAtk3() : DontCrash(CardType.Attack, CardRarity.Rare)
{
}


public class DontCrashRareSkl1() : DontCrash(CardType.Skill, CardRarity.Rare)
{
}

public class DontCrashRareSkl2() : DontCrash(CardType.Skill, CardRarity.Rare)
{
}

public class DontCrashRareSkl3() : DontCrash(CardType.Skill, CardRarity.Rare)
{
}


public class DontCrashRarePow1() : DontCrash(CardType.Power, CardRarity.Rare)
{
}

public class DontCrashRarePow2() : DontCrash(CardType.Power, CardRarity.Rare)
{
}

public class DontCrashRarePow3() : DontCrash(CardType.Power, CardRarity.Rare)
{
}

public class DontCrashAncientOrobas() : DontCrash(CardType.Power, CardRarity.Ancient)
{
}

public class DontCrashAncientDarv() : DontCrash(CardType.Power, CardRarity.Ancient)
{
}