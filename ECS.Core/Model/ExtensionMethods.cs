namespace ECS.Core.Model
{
    public static class ExtensionMethods
    {
        public static INode OrEquivalent(this INode n)
        {
            return n?.EquivalentNode ?? n;
        }

        public static INode OtherNode(this IComponent c, INode n)
        {
            return c?.Node1.OrEquivalent() == n ? c?.Node2 : c?.Node1;
        }
    }
}
