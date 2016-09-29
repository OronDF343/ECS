namespace ECS.Core
{
    /// <summary>
    /// Provides extension methods to extend fuctionality of other classes.
    /// </summary>
    public static class CircuitUtils
    {
        /// <summary>
        /// Gets the <see cref="Node"/> connected to a <see cref="Component"/> which isn't equal to a given <see cref="Node"/>.
        /// </summary>
        /// <param name="c">The <see cref="Component"/>.</param>
        /// <param name="n">The connected <see cref="Node"/> which is NOT desired.</param>
        /// <returns>The <see cref="Node"/> connected on the other side of the <see cref="Component"/>.</returns>
        public static Node OtherNode(this Component c, Node n)
        {
            return Equals(c.Node1, n) ? c.Node2 : c.Node1;
        }

        /// <summary>
        /// Link side 1 of a <see cref="Component"/> to a <see cref="Node"/>. This is usually the plus side.
        /// </summary>
        /// <param name="c">A component to link.</param>
        /// <param name="n">A node to link to.</param>
        public static void Link1(Component c, Node n)
        {
            c.Node1 = n;
            n.Components.Add(c);
        }

        /// <summary>
        /// Link side 2 of a <see cref="Component"/> to a <see cref="Node"/>. This is usually the minus side.
        /// </summary>
        /// <param name="c">A component to link.</param>
        /// <param name="n">A node to link to.</param>
        public static void Link2(Component c, Node n)
        {
            c.Node2 = n;
            n.Components.Add(c);
        }
    }
}