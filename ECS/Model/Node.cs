using System.Collections.Generic;
using System.Xml.Serialization;
using ECS.Core.Model;

namespace ECS.Model
{
    /// <summary>
    ///     A junction between two or more circuit components.
    /// </summary>
    public class Node : DiagramObject, INode
    {
        private bool _isRefNode;
        private double _voltage;

        /// <inheritdoc />
        [XmlAttribute]
        public double Voltage
        {
            get => _voltage;
            set
            {
                _voltage = value;
                OnPropertyChanged();
            }
        }

        /// <inheritdoc />
        [XmlAttribute]
        public bool IsReferenceNode
        {
            get => _isRefNode;
            set
            {
                _isRefNode = value;
                OnPropertyChanged();
            }
        }

        [XmlIgnore]
        HashSet<IComponent> INode.Components { get; } = new HashSet<IComponent>();

        INode INode.EquivalentNode { get; set; }
    }
}
