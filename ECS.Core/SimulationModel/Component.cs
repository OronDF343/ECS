using JetBrains.Annotations;

namespace ECS.Core.SimulationModel
{
    /// <summary>
    ///     A circuit component with 2 connections.
    /// </summary>
    public abstract class Component : IMarkedObject
    {
        /// <summary>
        ///     Creates a new <see cref="Component" />.
        /// </summary>
        /// <param name="id">The unique identifier of the component.</param>
        public Component(int id)
        {
            Id = id;
        }

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
            return obj is Node && (obj.GetHashCode() == GetHashCode());
        }
    }
}
