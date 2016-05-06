using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODA.Impl.Tools
{
    public abstract class UserData : UserDataContainer
    {
        public static readonly CopyAction CLONE = new Clone();
        public static readonly CopyAction SHARED = new Shared();
        public static readonly CopyAction REMOVE = new Remove();

    }
}
