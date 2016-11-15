using System;
using System.Linq;
using ECS.Core.SimulationModel;
using ECS.Model.Xml;
using JetBrains.Annotations;

namespace ECS.Core
{
    /// <summary>
    ///     Provides extra methods to extend fuctionality of other classes.
    /// </summary>
    public static class CircuitUtils
    {
        /// <summary>
        ///     Creates a <see cref="Circuit" /> from a <see cref="CircuitXml" /> object.
        /// </summary>
        /// <param name="cx">A <see cref="CircuitXml" /> object.</param>
        /// <exception cref="InvalidOperationException">If missing a non-reference node.</exception>
        /// <returns>An equivalent <see cref="Circuit" />.</returns>
        [NotNull]
        public static Circuit FromXml([NotNull] CircuitXml cx)
        {
            var nodes = cx.Nodes.Where(n => n != null).ToDictionary(n => n.Id, n => new Node(n));
            var h = nodes.FirstOrDefault(n => n.Key > -1).Value;
            if (h == null) throw new InvalidOperationException("Missing non-reference node!");

            Node ln;
            foreach (var r in cx.Resistors.Where(r => r != null))
            {
                var res = new Resistor(r);
                if ((r.Node1Id != null) && nodes.TryGetValue(r.Node1Id.Value, out ln)) Link1(res, ln);
                if ((r.Node2Id != null) && nodes.TryGetValue(r.Node2Id.Value, out ln)) Link2(res, ln);
            }
            foreach (var v in cx.VoltageSources)
            {
                var vs = new VoltageSource(v);
                if ((v.Node1Id != null) && nodes.TryGetValue(v.Node1Id.Value, out ln)) Link1(vs, ln);
                if ((v.Node2Id != null) && nodes.TryGetValue(v.Node2Id.Value, out ln)) Link2(vs, ln);
            }

            foreach (var s in cx.Switches)
            {
                var sw = new Switch(s);
                if ((s.Node1Id != null) && nodes.TryGetValue(s.Node1Id.Value, out ln)) Link1(sw, ln);
                if ((s.Node2Id != null) && nodes.TryGetValue(s.Node2Id.Value, out ln)) Link2(sw, ln);
            }

            return new Circuit(h, nodes.Keys.Count(n => n > -1), cx.VoltageSources.Count);
        }

        /// <summary>
        ///     Gets the <see cref="Node" /> connected to a <see cref="Component" /> which isn't equal to a given
        ///     <see cref="Node" />.
        /// </summary>
        /// <param name="c">The <see cref="Component" />.</param>
        /// <param name="n">The connected <see cref="Node" /> which is NOT desired.</param>
        /// <returns>
        ///     The <see cref="Node" /> connected on the other side of the <see cref="Component" />. If either
        ///     <paramref name="c" /> or <paramref name="n" /> are <code>null</code>, this method returns <code>null</code>.
        /// </returns>
        [CanBeNull]
        public static Node OtherNode([CanBeNull] this Component c, [CanBeNull] Node n)
        {
            return Equals(c?.Node1, n) ? c?.Node2 : c?.Node1;
        }

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
            n.Components.Add(c);
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
            n.Components.Add(c);
        }
    }
}
