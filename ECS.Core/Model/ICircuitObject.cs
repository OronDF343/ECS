using System;

namespace ECS.Core.Model
{
    public interface ICircuitObject
    {
        /// <summary>
        ///     Gets or sets whether this object has been marked.
        /// </summary>
        bool Mark { get; set; }

        /// <summary>
        ///     Gets the unique id of this object.
        /// </summary>
        Guid Id { get; set; }

        /// <summary>
        ///     Gets or sets the user-defined name of this object.
        /// </summary>
        string Name { get; set; }

        int SimulationIndex { get; set; }
    }
}
