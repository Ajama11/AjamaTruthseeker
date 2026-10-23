using MegaCrit.Sts2.Core.Models;

namespace AjamaTruthseeker.AjamaTruthseekerCode.Extensions;

public static class AbstractModelExtensions
{
    extension<T>(T model) where T : AbstractModel
    {
        public T TypedMutableClone()
        {
            return (T) model.MutableClone();
        }
    }
}