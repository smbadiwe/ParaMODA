using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODA.Impl.Tools
{
    public abstract class UserDataContainer : ICloneable
    {
        public abstract void addUserDatum(object var1, object var2, CopyAction var3);

        public abstract void importUserData(UserDataContainer var1);

        //Iterator getUserDatumKeyIterator();

        public abstract CopyAction getUserDatumCopyAction(object var1);

        public abstract object getUserDatum(object var1);

        public abstract void setUserDatum(object var1, object var2, CopyAction var3);

        public abstract object removeUserDatum(object var1);

        public abstract bool containsUserDatumKey(object var1);

        public abstract object Clone();
    }

    public interface CopyAction
    {
        object onCopy(object var1, UserDataContainer var2, UserDataContainer var3);
    }
    public class Remove : CopyAction
    {
        public Remove()
        {
        }

        public object onCopy(object value, UserDataContainer source, UserDataContainer target)
        {
            return null;
        }
    }

    public class Shared : CopyAction
    {
        public Shared()
        {
        }

        public object onCopy(object value, UserDataContainer source, UserDataContainer target)
        {
            return value;
        }
    }

    public class Clone : CopyAction
    {
        public Clone()
        {
        }

        public object onCopy(object value, UserDataContainer source, UserDataContainer target)
        {
            try
            {
                if (!(value is ICloneable))
                {
                    throw new InvalidOperationException("Not cloneable interface: This used to just return a shared reference.");
                }
                else
                {
                    var e = (value as ICloneable).Clone();
                    if (e != null)
                    {
                        return e;
                    }
                    else
                    {
                        throw new InvalidOperationException("No clone method implemented: This used to just return a shared reference.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Cloning failure", ex);
            }
        }
    }
}
