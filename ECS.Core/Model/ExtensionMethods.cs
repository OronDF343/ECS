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
