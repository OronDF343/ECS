using System;
using System.Linq;
using ECS.Core.SimulationModel;
using ECS.Model.Xml;
using JetBrains.Annotations;
using Component = ECS.Core.SimulationModel.Component;
using Node = ECS.Core.SimulationModel.Node;

namespace ECS.Core
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
        /// <exception cref="InvalidOperationException">If missing a non-reference node.</exception>
        /// <returns>An equivalent <see cref="Circuit"/>.</returns>
        [NotNull]
        public static Circuit FromXml([NotNull] CircuitXml cx)
        {
            // TODO: SERIOUS ERROR HANDLING REQUIRED!
            var nodes = cx.Nodes.ToDictionary(n => n.Id, n => new Node(n));
            var h = nodes.FirstOrDefault(n => n.Key > -1).Value;
            if (h == null) throw new InvalidOperationException("Missing non-reference node!");

            foreach (var r in cx.Resistors)
            {
                var res = new Resistor(r);
                Link1(res, nodes[r.Node1Id.Value]);
                Link2(res, nodes[r.Node2Id.Value]);
            }
            foreach (var v in cx.VoltageSources)
            {
                var vs = new VoltageSource(v);
                Link1(vs, nodes[v.Node1Id.Value]);
                Link2(vs, nodes[v.Node2Id.Value]);
            }

            return new Circuit(h, nodes.Keys.Count(n => n > -1), cx.VoltageSources.Count);
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