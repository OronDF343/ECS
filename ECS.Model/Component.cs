using System.Xml.Serialization;
using ECS.ViewModel;

namespace ECS.Model
{
    /// <summary>
    ///     A circuit component with 2 connections.
    /// </summary>
    public abstract class Component : DiagramObject
    {
        private Node _node1;
        private int? _node1Id;
        private Node _node2;
        private int? _node2Id;
        private double _rotation;

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

/*
             * cursor - M12 10c1.104 0 2 .896 2 2s-.896 2-2 2-2-.896-2-2 .896-2 2-2zm-3.857 3c-.084-.321-.143-.652-.143-1s.059-.679.143-1h-2.143v-4l-6 5 6 5v-4h2.143zm7.714-2c.084.321.143.652.143 1s-.059.679-.143 1h2.143v4l6-5-6-5v4h-2.143zm-2.857 4.857c-.321.084-.652.143-1 .143s-.679-.059-1-.143v2.143h-4l5 6 5-6h-4v-2.143zm-2-7.714c.321-.084.652-.143 1-.143s.679.059 1 .143v-2.143h4l-5-6-5 6h4v2.143z
             * 
             * plus - M24 10h-10v-10h-4v10h-10v4h10v10h4v-10h10z
             * 
             * link - M5 7c2.761 0 5 2.239 5 5s-2.239 5-5 5c-2.762 0-5-2.239-5-5s2.238-5 5-5zm15-4c0-1.657-1.344-3-3-3-1.657 0-3 1.343-3 3 0 .312.061.606.148.888l-4.209 3.157c.473.471.877 1.009 1.201 1.599l4.197-3.148c.477.317 1.048.504 1.663.504 1.656 0 3-1.343 3-3zm-5.852 17.112c-.087.282-.148.576-.148.888 0 1.657 1.343 3 3 3 1.656 0 3-1.343 3-3s-1.344-3-3-3c-.615 0-1.186.187-1.662.504l-4.197-3.148c-.324.59-.729 1.128-1.201 1.599l4.208 3.157zm6.852-5.05c1.656 0 3-1.343 3-3s-1.344-3-3-3c-1.281 0-2.367.807-2.797 1.938h-6.283c.047.328.08.66.08 1s-.033.672-.08 1h6.244c.395 1.195 1.508 2.062 2.836 2.062z
             */
}