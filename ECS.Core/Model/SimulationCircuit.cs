using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace ECS.Core.Model
{
    /// <summary>
    ///     An electronic circuit.
    /// </summary>
    public class SimulationCircuit
    {
        /// <summary>
        ///     Prepares a collection of components for simulation.
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="components"></param>
        /// <exception cref="InvalidOperationException">If missing a non-reference node.</exception>
        public SimulationCircuit([NotNull] IEnumerable<INode> nodes, [NotNull] IEnumerable<IComponent> components)
        {
            // Assign indexes to elements
            int rId = 0, vsId = 0, nId = 0, rnId = -1;
            _nodes = new List<INode>(nodes);
            _components = new List<IComponent>(components);
            INode h = null;
            _nodes.ForEach(n =>
            {
                // Clear previous links
                n.Links.Clear();
                // Assign correct index:
                if (n.IsReferenceNode) n.SimulationIndex = rnId++;
                else
                {
                    // Optimization: Keep first node for future use
                    if (nId == 0) h = n;
                    n.SimulationIndex = nId++;
                }
            });
            if (h == null) throw new InvalidOperationException("Missing non-reference node!");
            
            foreach (var c in _components)
            {
                // Create relevant links
                c.Node1?.Links.Add(new Link(c, true));
                c.Node2?.Links.Add(new Link(c, false));
                // Assign an index
                if (c is IResistor)
                    c.SimulationIndex = rId++;
                else if (c is IVoltageSource)
                    c.SimulationIndex = vsId++;
            }

            Head = h;
            NodeCount = nId;
            SourceCount = vsId;
        }

        private List<INode> _nodes;
        private List<IComponent> _components;

        /// <summary>
        ///     Gets a node connected in the circuit.
        /// </summary>
        [NotNull]
        public INode Head { get; }

        /// <summary>
        ///     Gets the number of <see cref="INode" />s in the circuit (excluding the reference node).
        /// </summary>
        public int NodeCount { get; }

        /// <summary>
        ///     Gets the number of <see cref="IVoltageSource" />s in the circuit.
        /// </summary>
        public int SourceCount { get; }

        internal void UpdateValues()
        {
            // TODO: update when simulation is reversed
            foreach (var r in _components.OfType<IResistor>())
            {
                r.Voltage = Math.Abs((r.Node1?.Voltage ?? 0) - (r.Node2?.Voltage ?? 0));
                r.Current = r.Voltage / r.Resistance;
            }
        }
    }
}
