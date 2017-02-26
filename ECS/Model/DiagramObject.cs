using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using ECS.Core.Model;
using JetBrains.Annotations;

namespace ECS.Model
{
    public abstract class DiagramObject : ICircuitObject, INotifyPropertyChanged
    {
        public DiagramObject()
        {
            Id = Guid.NewGuid();
        }

        private string _name;

        private double _x;
        private double _y;

        [XmlAttribute]
        public double X
        {
            get { return _x; }
            set
            {
                _x = value;
                OnPropertyChanged();
            }
        }

        [XmlAttribute]
        public double Y
        {
            get { return _y; }
            set
            {
                _y = value;
                OnPropertyChanged();
            }
        }

        [XmlIgnore]
        public bool Mark { get; set; }

        [XmlAttribute]
        public Guid Id { get; set; }

        [XmlAttribute]
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        [XmlIgnore]
        public int SimulationIndex { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return (obj as DiagramObject)?.Id == Id;
        }
    }
}
