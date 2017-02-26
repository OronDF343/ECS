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
        private double _voltage;
        private bool _isRefNode;
        public Node()
        {
            Links = new HashSet<Link>();
        }

        /// <inheritdoc />
        [XmlAttribute]
        public double Voltage
        {
            get { return _voltage; }
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
            get { return _isRefNode; }
            set
            {
                _isRefNode = value;
                OnPropertyChanged();
            }
        }

        [XmlIgnore]
        public HashSet<Link> Links { get; }
    }
}
