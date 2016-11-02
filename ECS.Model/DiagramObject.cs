using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using JetBrains.Annotations;

namespace ECS.Model
{
    public abstract class DiagramObject : INotifyPropertyChanged
    {
        private int _id;
        private bool _isNew;
        private bool _isHighlighted;
        private double _x;
        private double _y;

        [XmlAttribute]
        public int Id
        {
            get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged();
            }
        }

        [XmlIgnore]
        public bool IsNew
        {
            get { return _isNew; }
            set
            {
                _isNew = value;
                OnPropertyChanged();
            }
        }

        [XmlIgnore]
        public bool IsHighlighted
        {
            get { return _isHighlighted; }
            set
            {
                _isHighlighted = value;
                OnPropertyChanged();
            }
        }

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

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override int GetHashCode()
        {
            return Id;
        }

        public override bool Equals(object obj)
        {
            return (obj?.GetHashCode() == GetHashCode()) && (obj.GetType() == GetType());
        }
    }
}
