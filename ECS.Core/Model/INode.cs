using System.Collections.Generic;

namespace ECS.Core.Model
{
    /// <summary>
    ///     A junction between two or more circuit components.
    /// </summary>
    public interface INode : ICircuitObject
    {
        /// <summary>
        ///     Gets a list of <see cref="Link" />s connected to this node.
        /// </summary>
        HashSet<Link> Links { get; }

        /// <summary>
        ///     Gets or sets the voltage at this node.
        /// </summary>
        double Voltage { get; set; }

        /// <summary>
        ///     Gets or sets whether this node will be used as a ground reference.
        /// </summary>
        bool IsReferenceNode { get; set; }
    }
}
