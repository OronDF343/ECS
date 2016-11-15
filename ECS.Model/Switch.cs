using System.Xml.Serialization;

namespace ECS.Model
{
    public class Switch : Component
    {
        private bool _isClosed;

        [XmlAttribute]
        public bool IsClosed
        {
            get { return _isClosed; }
            set
            {
                _isClosed = value;
                OnPropertyChanged();
            }
        }
    }
}
