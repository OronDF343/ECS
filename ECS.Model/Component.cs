using System.Xml.Serialization;
using ECS.ViewModel;

namespace ECS.Model
{
    /// <summary>
    /// A circuit component with 2 connections.
    /// </summary>
    public abstract class Component : DiagramObject
    {
        private Node _node1;
        private Node _node2;
        private double _rotation;
        private int? _node1Id;
        private int? _node2Id;

        [XmlIgnore]
        public Node Node1
        {
            get { return _node1; }
            set
            {
                _node1 = value;
                OnPropertyChanged();
            }
        }

        [XmlIgnore]
        public Node Node2
        {
            get { return _node2; }
            set
            {
                _node2 = value;
                OnPropertyChanged();
            }
        }

        [XmlAttribute]
        public double Rotation
        {
            get { return _rotation; }
            set
            {
                _rotation = value;
                OnPropertyChanged();
            }
        }

        [XmlAttribute]
        public int? Node1Id
        {
            get { return _node1?.Id; }
            set { _node1Id = value; }
        }

        [XmlAttribute]
        public int? Node2Id
        {
            get { return _node2?.Id; }
            set { _node2Id = value; }
        }
    }
}