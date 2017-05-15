using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS.Core.Model
{
    public static class ExtensionMethods
    {
        public static INode OrEquivalent(this INode n)
        {
            return n.EquivalentNode ?? n;
        }
    }
}
