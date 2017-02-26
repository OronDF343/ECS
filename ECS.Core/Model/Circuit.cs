using System;
using JetBrains.Annotations;

namespace ECS.Core.Model
{
    /// <summary>
    ///     An electronic circuit.
    /// </summary>
    public class Circuit
    {
        /// <summary>
        ///     Create a new circuit.
        /// </summary>
        /// <param name="head">A node connected in the circuit.</param>
        /// <param name="nodeCount">The number of <see cref="Node" />s in the circuit (excluding the reference node).</param>
        /// <param name="srcCount">The number of <see cref="VoltageSource" />s in the circuit.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     If <paramref name="nodeCount" /> or <paramref name="srcCount" /> are less
        ///     than 1.
        /// </exception>
        /// <exception cref="ArgumentNullException">If <paramref name="head" /> is equal to <code>null</code>.</exception>
        public Circuit([NotNull] Node head, int nodeCount, int srcCount)
        {
            if (head == null) throw new ArgumentNullException(nameof(head));
            if (head.Id < 0) throw new ArgumentException("Starting node must not be a reference node", nameof(head));
            if (nodeCount < 1)
                throw new ArgumentOutOfRangeException(nameof(nodeCount), nodeCount,
                                                      "Number of nodes must be greater than zero");
            if (srcCount < 1)
                throw new ArgumentOutOfRangeException(nameof(srcCount), srcCount,
                                                      "Number of voltage sources must be greater than zero");
            Head = head;
            NodeCount = nodeCount;
            SourceCount = srcCount;
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
