using System.Xml.Serialization;

namespace ECS.Model
{
    /// <summary>
    /// A circuit component with 2 connections.
    /// </summary>
    public abstract class Component
    {
        /// <inheritdoc/>
        [XmlAttribute]
        public int Id { get; set; }

        [XmlAttribute]
        public int? Node1Id { get; set; }

        [XmlAttribute]
        public int? Node2Id { get; set; }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return Id;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is Node && obj.GetHashCode() == GetHashCode();
        }
    }
}