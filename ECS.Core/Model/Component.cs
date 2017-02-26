using JetBrains.Annotations;

namespace ECS.Core.Model
{
    /// <summary>
    ///     A circuit component with 2 connections.
    /// </summary>
    public abstract class Component : CircuitObject
    {
        /// <summary>
        ///     Gets or sets the first <see cref="Node" /> connected to this component.
        ///     If this component has polarity, this will be on the plus side.
        /// </summary>
        [CanBeNull]
        public Node Node1 { get; set; }

        /// <summary>
        ///     Gets or sets the second <see cref="Node" /> connected to this component.
        ///     If this component has polarity, this will be on the minus side.
        /// </summary>
        [CanBeNull]
        public Node Node2 { get; set; }
    }
}
