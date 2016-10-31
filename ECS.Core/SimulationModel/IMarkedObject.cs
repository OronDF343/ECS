using System.Xml.Serialization;

namespace ECS.Core.SimulationModel
{
    /// <summary>
    /// An object with an ID that can be marked. Good for BFS, etc.
    /// </summary>
    public interface IMarkedObject
    {
        /// <summary>
        /// Gets or sets whether this object has been marked.
        /// </summary>
        [XmlIgnore]
        bool Mark { get; set; }

        /// <summary>
        /// Gets the unique id of this object.
        /// </summary>
        [XmlAttribute]
        int Id { get; }
    }
}
