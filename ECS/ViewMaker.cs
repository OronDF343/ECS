using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using ECS.Model;
using ECS.ViewModel;

namespace ECS
{
    public static class ViewMaker
    {
        public static ResultDiagramView CreateResultDiagram(IEnumerable<DiagramObject> diagram, string name)
        {
            return new ResultDiagramView(new ResultDiagramViewModel(diagram, name));
        }

        public static ResultErrorView CreateResultError(string err)
        {
            return new ResultErrorView(new ResultErrorViewModel(err));
        }

        public static ResultsWindow CreateResults(ObservableCollection<TabItem> diagramViews)
        {
            return new ResultsWindow(new ResultsViewModel(diagramViews));
        }

        public static StatesEditorWindow CreateStatesEditor(List<CircuitState> states, IEnumerable<Switch> switches)
        {
            return new StatesEditorWindow(new StatesEditorViewModel(states, switches));
        }
    }
}
