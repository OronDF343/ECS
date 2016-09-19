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
            Components = new List<Component>();
        }

        /// <summary>
        /// Creates a new <see cref="Node"/> connected to given components.
        /// </summary>
        /// <param name="id">The unique identifier of the node.</param>
        /// <param name="components">The components which are connected to the node.</param>
        public Node(int id, IEnumerable<Component> components)
        {
            Id = id;
            Components = new List<Component>(components);
        }

        /// <inheritdoc/>
        public bool Mark { get; set; }
        /// <inheritdoc/>
        public int Id { get; }

        /// <summary>
        /// Gets a list of <see cref="Component"/>s connected to this node.
        /// </summary>
        public List<Component> Components { get; }
    }
}
