using System.Collections.Generic;
using JetBrains.Annotations;

namespace ECS.Core.Model
{
    /// <summary>
    ///     An electronic circuit.
    /// </summary>
    internal class SimulationCircuit
    {
        private IEnumerable<INode> _nodes;

        public SimulationCircuit([NotNull] INode head, int nodeCount, int sourceCount, IEnumerable<INode> nodes)
        {
            Head = head;
            NodeCount = nodeCount;
            SourceCount = sourceCount;
            _nodes = nodes;
        }

        /// <summary>
        ///     Gets a node connected in the circuit.
        /// </summary>
        [NotNull]
        public INode Head { get; }

        /// <summary>
        ///     Gets the number of <see cref="INode" />s in the circuit (excluding
        ///     the reference node).
        /// </summary>
        public int NodeCount { get; }

        /// <summary>
        ///     Gets the number of <see cref="IVoltageSource" />s in the circuit.
        /// </summary>
        public int SourceCount { get; }

        public void ResetMarks()
        {
            foreach (var n in _nodes) n.Mark = false;
        }
    }
}
