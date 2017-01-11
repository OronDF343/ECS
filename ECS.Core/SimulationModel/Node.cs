using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace ECS.Core.SimulationModel
{
    /// <summary>
    ///     A junction between two or more circuit components.
    /// </summary>
    public class Node : IMarkedObject
    {
        /// <summary>
        ///     Creates a new <see cref="Node" />.
        /// </summary>
        /// <param name="id">The unique identifier of the node.</param>
        public Node(int id)
        {
            Id = id;
            Components = new HashSet<Component>();
        }

        /// <summary>
        ///     Creates a new <see cref="Node" /> connected to given components.
        /// </summary>
        /// <param name="id">The unique identifier of the node.</param>
        /// <param name="components">The components which are connected to the node.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="components" /> is null.</exception>
        public Node(int id, [NotNull] IEnumerable<Component> components)
        {
            if (components == null) throw new ArgumentNullException(nameof(components));
            Id = id;
            Components = new HashSet<Component>(components);
        }

        public Node([NotNull] Model.Node n)
            : this(n.Id)
        {
            Voltage = n.Voltage;
        }

        /// <summary>
        ///     Gets a list of <see cref="Component" />s connected to this node.
        /// </summary>
        [NotNull]
        public HashSet<Component> Components { get; }

        /// <summary>
        ///     Gets or sets the voltage at this node.
        /// </summary>
        public double Voltage { get; set; }

        /// <inheritdoc />
        public bool Mark { get; set; }

        /// <inheritdoc />
        public int Id { get; }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return Id;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is Node && obj.GetHashCode() == GetHashCode();
        }
    }
}
