using System.Collections.Generic;

namespace ECS.Core.Model
{
    /// <summary>
    ///     A junction between two or more circuit components.
    /// </summary>
    public class Node : CircuitObject
    {
        /// <summary>
        ///     Creates a new <see cref="Node" />.
        /// </summary>
        public Node()
        {
            Links = new HashSet<Link>();
        }

        /// <summary>
        ///     Gets a list of <see cref="Link" />s connected to this node.
        /// </summary>
        public HashSet<Link> Links { get; }

        /// <summary>
        ///     Gets or sets the voltage at this node.
        /// </summary>
        public double Voltage { get; set; }
    }
}
