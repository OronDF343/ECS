using System;
using ECS.Core.Model;
using JetBrains.Annotations;

namespace ECS.Core
{
    /// <summary>
    ///     Provides extra methods to extend functionality of other classes.
    /// </summary>
    public static class CircuitUtils
    {

        /// <summary>
        ///     Link side 1 of a <see cref="Component" /> to a <see cref="Node" />. This is usually the plus side.
        /// </summary>
        /// <param name="c">A component to link.</param>
        /// <param name="n">A node to link to.</param>
        /// <exception cref="ArgumentNullException">If either <paramref name="c" /> or <paramref name="n" /> are <code>null</code>.</exception>
        public static void Link1([NotNull] Component c, [NotNull] Node n)
        {
            if (c == null) throw new ArgumentNullException(nameof(c));
            if (n == null) throw new ArgumentNullException(nameof(n));
            c.Node1 = n;
            n.Links.Add(new Link(c, true));
        }

        /// <summary>
        ///     Link side 2 of a <see cref="Component" /> to a <see cref="Node" />. This is usually the minus side.
        /// </summary>
        /// <param name="c">A component to link.</param>
        /// <param name="n">A node to link to.</param>
        /// <exception cref="ArgumentNullException">If either <paramref name="c" /> or <paramref name="n" /> are <code>null</code>.</exception>
        public static void Link2([NotNull] Component c, [NotNull] Node n)
        {
            if (c == null) throw new ArgumentNullException(nameof(c));
            if (n == null) throw new ArgumentNullException(nameof(n));
            c.Node2 = n;
            n.Links.Add(new Link(c, false));
        }
    }
}
