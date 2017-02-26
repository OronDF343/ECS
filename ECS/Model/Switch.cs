using System.Xml.Serialization;
using ECS.Core.Model;

namespace ECS.Model
{
    public class Switch : Component, ISwitch
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
