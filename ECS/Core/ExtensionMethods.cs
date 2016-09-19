namespace ECS.Core
{
    /// <summary>
    /// Provides extension methods to extend fuctionality of other classes.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Gets the <see cref="Node"/> connected to a <see cref="Component"/> which isn't equal to a given <see cref="Node"/>.
        /// </summary>
        /// <param name="c">The <see cref="Component"/>.</param>
        /// <param name="n">The connected <see cref="Node"/> which is NOT desired.</param>
        /// <returns>The <see cref="Node"/> connected on the other side of the <see cref="Component"/>.</returns>
        public static Node OtherNode(this Component c, Node n)
        {
            return c.Node1 == n ? c.Node2 : c.Node1;
        }
    }
}