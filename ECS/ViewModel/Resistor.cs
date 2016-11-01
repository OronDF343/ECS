using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS.ViewModel
{
    public class Resistor : Component
    {
        private double _resistance;
        private double _voltage;
        private double _current;

        public double Resistance
        {
            get { return _resistance; }
            set
            {
                _resistance = value; 
                OnPropertyChanged();
            }
        }

        public double Voltage
        {
            get { return _voltage; }
            set
            {
                _voltage = value;
                OnPropertyChanged();
            }
        }

        public double Current
        {
            get { return _current; }
            set
            {
                _current = value;
                OnPropertyChanged();
            }
        }
    }
}
