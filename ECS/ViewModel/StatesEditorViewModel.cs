using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using ECS.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Microsoft.Win32;

namespace ECS.ViewModel
{
    public class StatesEditorViewModel : ViewModelBase
    {
        public StatesEditorViewModel(ObservableCollection<CircuitState> states, IEnumerable<Switch> switches)
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
                Columns.Add(new DataGridCheckBoxColumn
                {
                    Header = sw.Name,
                    Binding = new Binding($"SwitchStates[{sw.Id}]")
                });
        }

        private readonly List<Switch> _switches;
        private OpenFileDialog _ofd;
        private SaveFileDialog _sfd;

        public ObservableCollection<CircuitState> States { get; set; }

        public ObservableCollection<DataGridColumn> Columns { get; }

        public ICommand InitNewItemCommand => new RelayCommand<InitializingNewItemEventArgs>(InitNewItem);

        public ICommand LoadCommand => new RelayCommand(Load);
        public ICommand SaveCommand => new RelayCommand(Save);

        private void InitNewItem(InitializingNewItemEventArgs obj)
        {
            _switches.ForEach(sw => ((CircuitState)obj.NewItem).SwitchStates.Add(sw.Id, false));
        }

        private void Save()
        {
            if (_sfd == null) _sfd = new SaveFileDialog { Filter = "CSV file|*.csv" };
            var dr = _sfd.ShowDialog(Application.Current.Windows.OfType<StatesEditorWindow>().FirstOrDefault());
            if (dr != true) return;

            File.WriteAllText(_sfd.FileName, CircuitState.Serialize(States));
        }

        private void Load()
        {
            if (_ofd == null) _ofd = new OpenFileDialog { Filter = "CSV file|*.csv" };
            var dr = _ofd.ShowDialog(Application.Current.Windows.OfType<StatesEditorWindow>().FirstOrDefault());
            if (dr != true) return;
            States.Clear();
            foreach (var cs in CircuitState.Deserialize(File.ReadAllText(_ofd.FileName))) States.Add(cs);
        }
    }
}
