using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ECS.Model;
using ECS.ViewModel;

namespace ECS
{
    public static class ViewMaker
    {
        public static ResultDiagramSnapshotView CreateResultDiagramSnapshot(string name)
        {
            var cont = (Application.Current.MainWindow as MainWindow).EditingArea;
            int width = (int)cont.ActualWidth;
            int height = (int)cont.ActualHeight;
            
            var rtb = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
            var dv = new DrawingVisual();
            using (var context = dv.RenderOpen())
            {
                var brush = new VisualBrush(cont){Stretch = Stretch.None};
                context.DrawRectangle(brush, null, new Rect(0, 0, width, height));
            }
            rtb.Render(dv);
            return new ResultDiagramSnapshotView(new ResultDiagramSnapshotViewModel(new ImageBrush(rtb){Stretch = Stretch.None, AlignmentX = AlignmentX.Left, AlignmentY = AlignmentY.Top}, name));
        }

        public static ResultErrorView CreateResultError(string err)
        {
            return new ResultErrorView(new ResultErrorViewModel(err));
        }

        public static ResultsWindow CreateResults(ObservableCollection<TabItem> diagramViews)
        {
            return new ResultsWindow(new ResultsViewModel(diagramViews));
        }

        public static StatesEditorWindow CreateStatesEditor(ObservableCollection<CircuitState> states, IEnumerable<Switch> switches)
        {
            return new StatesEditorWindow(new StatesEditorViewModel(states, switches));
        }
    }
}
