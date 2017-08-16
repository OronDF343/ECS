using JetBrains.Annotations;

namespace ECS.Core.Model
{
    /// <summary>
    ///     An electronic circuit.
    /// </summary>
    internal class SimulationCircuit
    {
        public SimulationCircuit([NotNull] INode head, int nodeCount, int sourceCount)
        {
            Head = head;
            NodeCount = nodeCount;
            SourceCount = sourceCount;
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
    }
}
