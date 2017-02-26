using System;

namespace ECS.Core.Model
{
    public abstract class CircuitObject
    {
        public CircuitObject()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        ///     Gets or sets whether this object has been marked.
        /// </summary>
        public bool Mark { get; set; }

        /// <summary>
        ///     Gets the unique id of this object.
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Gets or sets the user-defined name of this object.
        /// </summary>
        public string Name { get; set; }

        public int SimulationIndex { get; set; }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return ((CircuitObject)obj)?.Id == Id;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"\"{Name}\" (ID: {Id}, Index: {SimulationIndex})";
        }
    }
}
