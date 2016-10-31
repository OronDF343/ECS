using System;
using System.Linq;
using ECS.Core.Xml;
using JetBrains.Annotations;

namespace ECS.Core.Model
{
    /// <summary>
    /// Provides extra methods to extend fuctionality of other classes.
    /// </summary>
    public static class CircuitUtils
    {
        /// <summary>
        /// Creates a <see cref="Circuit"/> from a <see cref="CircuitXml"/> object.
        /// </summary>
        /// <param name="cx">A <see cref="CircuitXml"/> object.</param>
        /// <exception cref="ArgumentException">If missing a reference node.</exception>
        /// <returns>An equivalent <see cref="Circuit"/>.</returns>
        [NotNull]
        public static Circuit FromXml([NotNull] CircuitXml cx)
        {
            var h = cx.Nodes.FirstOrDefault(n => n.Id > -1);
            if (h == null) throw new ArgumentException("Missing reference node!", nameof(cx));
            return new Circuit(h, cx.Nodes.Count(n => n.Id > -1), cx.VoltageSources.Count);
        }

        /// <summary>
        /// Gets the <see cref="Node"/> connected to a <see cref="Component"/> which isn't equal to a given <see cref="Node"/>.
        /// </summary>
        /// <param name="c">The <see cref="Component"/>.</param>
        /// <param name="n">The connected <see cref="Node"/> which is NOT desired.</param>
        /// <returns>The <see cref="Node"/> connected on the other side of the <see cref="Component"/>. If either <paramref name="c"/> or <paramref name="n"/> are <code>null</code>, this method returns <code>null</code>.</returns>
        [CanBeNull]
        public static Node OtherNode([CanBeNull] this Component c, [CanBeNull] Node n)
        {
            return Equals(c?.Node1, n) ? c?.Node2 : c?.Node1;
        }

        /// <summary>
        /// Link side 1 of a <see cref="Component"/> to a <see cref="Node"/>. This is usually the plus side.
        /// </summary>
        /// <param name="c">A component to link.</param>
        /// <param name="n">A node to link to.</param>
        /// <exception cref="ArgumentNullException">If either <paramref name="c"/> or <paramref name="n"/> are <code>null</code>.</exception>
        public static void Link1([NotNull] Component c, [NotNull] Node n)
        {
            if (c == null) throw new ArgumentNullException(nameof(c));
            if (n == null) throw new ArgumentNullException(nameof(n));
            c.Node1 = n;
            n.Components.Add(c);
        }

        /// <summary>
        /// Link side 2 of a <see cref="Component"/> to a <see cref="Node"/>. This is usually the minus side.
        /// </summary>
        /// <param name="c">A component to link.</param>
        /// <param name="n">A node to link to.</param>
        /// <exception cref="ArgumentNullException">If either <paramref name="c"/> or <paramref name="n"/> are <code>null</code>.</exception>
        public static void Link2([NotNull] Component c, [NotNull] Node n)
        {
            if (c == null) throw new ArgumentNullException(nameof(c));
            if (n == null) throw new ArgumentNullException(nameof(n));
            c.Node2 = n;
            n.Components.Add(c);
        }
    }
}