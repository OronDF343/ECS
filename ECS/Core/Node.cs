using System.Collections.Generic;

namespace ECS.Core
{
    /// <summary>
    /// A junction between two or more circuit components.
    /// </summary>
    public class Node : IMarkedObject
    {
        /// <summary>
        /// Creates a new <see cref="Node"/>.
        /// </summary>
        /// <param name="id">The unique identifier of the node.</param>
        public Node(int id)
        {
            Id = id;
            Components = new HashSet<Component>();
        }

        /// <summary>
        /// Creates a new <see cref="Node"/> connected to given components.
        /// </summary>
        /// <param name="id">The unique identifier of the node.</param>
        /// <param name="components">The components which are connected to the node.</param>
        public Node(int id, IEnumerable<Component> components)
        {
            Id = id;
            Components = new HashSet<Component>(components);
        }

        /// <inheritdoc/>
        public bool Mark { get; set; }
        /// <inheritdoc/>
        public int Id { get; }

        /// <summary>
        /// Gets a list of <see cref="Component"/>s connected to this node.
        /// </summary>
        public HashSet<Component> Components { get; }
        
        /// <summary>
        /// Gets or sets the voltage at this node.
        /// </summary>
        public double Voltage { get; set; }

        public override int GetHashCode()
        {
            return Id;
        }

        public override bool Equals(object obj)
        {
            return obj is Node && obj.GetHashCode() == GetHashCode();
        }
    }
}
