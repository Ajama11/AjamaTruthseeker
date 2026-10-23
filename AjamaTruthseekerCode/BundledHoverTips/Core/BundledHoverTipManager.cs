using MegaCrit.Sts2.Core.HoverTips;

namespace AjamaTruthseeker.AjamaTruthseekerCode.BundledHoverTips.Core;

public class BundledHoverTipManager : List<BundledHoverTip>
{
    public enum Category
    {
        Start = 0,
        Middle = 100,
        End = 1000
    }

    public List<IHoverTip> GetHoverTips()
    {
        List<IHoverTip> list = [];
        
        foreach (var bundle in this)
        {
            list = [..list, ..bundle.HoverTips];
        }

        return list;
    }

    private void Reorder(string anchor, params string[] otherBundles)
    {
        var matchingAnchor = Find(
            b => b.Name == anchor
        );

        if (matchingAnchor == null)
        {
            MainFile.Logger.Error("BundledHoverTipManager.Reorder() could not find anchor!");
            return;
        }

        for (int i = 0; i < otherBundles.Length; i++)
        {
            var matchingPackage = Find(
                b => b.Name == otherBundles[i]
            );

            matchingPackage?.SortOrder = matchingAnchor.SortOrder + i + 1;
        }
    }

    public void SortHoverTips(List<List<string>>? reorders = null)
    {
        if (reorders != null)
        {
            foreach (var reorder in reorders)
            {
                Reorder(reorder[0], reorder.Skip(1).ToArray());
            }
        }
        
        Sort((x, y) => x.InternalSortOrder - y.InternalSortOrder);
    }
}