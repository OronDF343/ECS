using System;
using System.Xml.Serialization;
using ECS.Core.Model;

namespace ECS.Model
{
    /// <summary>
    ///     A circuit component with 2 connections.
    /// </summary>
    public abstract class Component : DiagramObject, IComponent
    {
        private INode _node1;
        private Guid? _node1Id;
        private INode _node2;
        private Guid? _node2Id;
        private double _rotation;

        [XmlElement]
        public Guid? Node1Id { get => _node1?.Id ?? _node1Id; set => _node1Id = value; }

        [XmlElement]
        public Guid? Node2Id { get => _node2?.Id ?? _node2Id; set => _node2Id = value; }

        [XmlAttribute]
        public double Rotation
        {
            get => _rotation;
            set
            {
                _rotation = value;
                OnPropertyChanged();
            }
        }

        /// <inheritdoc />
        [XmlIgnore]
        public INode Node1
        {
            get => _node1;
            set
            {
                _node1 = value;
                OnPropertyChanged();
            }
        }

        /// <inheritdoc />
        [XmlIgnore]
        public INode Node2
        {
            get => _node2;
            set
            {
                _node2 = value;
                OnPropertyChanged();
            }
        }
    }
}
