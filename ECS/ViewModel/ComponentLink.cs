using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace ECS.ViewModel
{
    public class ComponentLink : INotifyPropertyChanged
    {
        private Node _node;
        public Node Node
        {
            get { return _node; }
            set
            {
                if (_node != null)
                    _node.PropertyChanged -= Position_PropertyChanged;
                _node = value;
                if (_node != null)
                    _node.PropertyChanged += Position_PropertyChanged;
                OnPropertyChanged();
                OnPropertyChanged(nameof(OffsetX));
                OnPropertyChanged(nameof(OffsetY));
            }
        }

        private Component _component;
        public Component Component
        {
            get { return _component; }
            set
            {
                if (_component != null)
                    _component.PropertyChanged -= Position_PropertyChanged;
                _component = value;
                if (_component != null)
                    _component.PropertyChanged += Position_PropertyChanged;
                OnPropertyChanged();
                OnPropertyChanged(nameof(OffsetX));
                OnPropertyChanged(nameof(OffsetY));
            }
        }

        private void Position_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "X") OnPropertyChanged(nameof(OffsetX));
            if (e.PropertyName == "Y") OnPropertyChanged(nameof(OffsetY));
            if (e.PropertyName == "Node1" || e.PropertyName == "Node2") OnPropertyChanged(nameof(IsPlus));
        }

        public double OffsetX => Component.X - Node.X;
        public double OffsetY => Component.Y - Node.Y;


        /// <summary>
        /// Gets whether <see cref="Component"/> is connceted on the positive terminal.
        /// </summary>
        public bool IsPlus => Node == Component.Node1;

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
