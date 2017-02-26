using JetBrains.Annotations;

namespace ECS.Core.Model
{
    /// <summary>
    ///     A circuit component with 2 connections.
    /// </summary>
    public interface IComponent : ICircuitObject
    {
        /// <summary>
        ///     Gets or sets the first <see cref="Node" /> connected to this component.
        ///     If this component has polarity, this will be on the positive side.
        /// </summary>
        [CanBeNull]
        INode Node1 { get; set; }

        /// <summary>
        ///     Gets or sets the second <see cref="Node" /> connected to this component.
        ///     If this component has polarity, this will be on the negative side.
        /// </summary>
        [CanBeNull]
        INode Node2 { get; set; }
    }
}
