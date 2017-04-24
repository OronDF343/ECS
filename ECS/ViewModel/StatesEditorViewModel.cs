using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using ECS.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace ECS.ViewModel
{
    public class StatesEditorViewModel : ViewModelBase
    {
        public StatesEditorViewModel(List<CircuitState> states, IEnumerable<Switch> switches)
        {
            _switches = switches.ToList();
            States = states;
            Columns = new ObservableCollection<DataGridColumn>
            {
                new DataGridTextColumn
                {
                    Header = "[State Name]",
                    Binding = new Binding("Name")
                }
            };
            foreach (var sw in _switches)
            {
                Columns.Add(new DataGridCheckBoxColumn
                {
                    Header = sw.Name,
                    Binding = new Binding($"SwitchStates[{sw.Id}]")
                });
            }
        }

        private readonly List<Switch> _switches;

        public List<CircuitState> States { get; }

        public ObservableCollection<DataGridColumn> Columns { get; }

        public ICommand InitNewItemCommand => new RelayCommand<InitializingNewItemEventArgs>(InitNewItem);

        private void InitNewItem(InitializingNewItemEventArgs obj)
        {
            _switches.ForEach(sw => ((CircuitState)obj.NewItem).SwitchStates.Add(sw.Id, false));
        }
    }
}
