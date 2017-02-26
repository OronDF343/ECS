using System;
using System.Collections.Generic;
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
        public SimulationCircuit([NotNull] IEnumerable<Node> nodes, [NotNull] IEnumerable<Component> components)
        {
            // Assign indexes to elements
            int rId = 0, vsId = 0, nId = 0, rnId = -1;
            var nodeList = new List<Node>(nodes);
            Node h = null;
            nodeList.ForEach(n =>
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
            
            foreach (var c in components)
            {
                // Create relevant links
                c.Node1?.Links.Add(new Link(c, true));
                c.Node2?.Links.Add(new Link(c, false));
                // Assign an index
                if (c is Resistor)
                    c.SimulationIndex = rId++;
                else if (c is VoltageSource)
                    c.SimulationIndex = vsId++;
            }

            Head = h;
            NodeCount = nId;
            SourceCount = vsId;
        }

        /// <summary>
        ///     Gets a node connected in the circuit.
        /// </summary>
        [NotNull]
        public Node Head { get; }

        /// <summary>
        ///     Gets the number of <see cref="Node" />s in the circuit (excluding the reference node).
        /// </summary>
        public int NodeCount { get; }

        /// <summary>
        ///     Gets the number of <see cref="VoltageSource" />s in the circuit.
        /// </summary>
        public int SourceCount { get; }
    }
}
