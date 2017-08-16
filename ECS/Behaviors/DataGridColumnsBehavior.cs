using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace ECS.Behaviors
{
    public class DataGridColumnsBehavior
    {
        public static readonly DependencyProperty BindableColumnsProperty =
            DependencyProperty.RegisterAttached("BindableColumns",
                                                typeof(ObservableCollection<DataGridColumn>),
                                                typeof(DataGridColumnsBehavior),
                                                new UIPropertyMetadata(null, BindableColumnsPropertyChanged));

        private static void BindableColumnsPropertyChanged(DependencyObject source,
                                                           DependencyPropertyChangedEventArgs e)
        {
            var dataGrid = source as DataGrid;
            var columns = e.NewValue as ObservableCollection<DataGridColumn>;
            dataGrid.Columns.Clear();

            if (columns == null) return;

            foreach (var column in columns) dataGrid.Columns.Add(column);

            columns.CollectionChanged += (sender, e2) =>
            {
                var ne = e2;
                switch (ne.Action)
                {
                    case NotifyCollectionChangedAction.Reset:
                        dataGrid.Columns.Clear();
                        foreach (DataGridColumn column in ne.NewItems) dataGrid.Columns.Add(column);
                        break;
                    case NotifyCollectionChangedAction.Add:
                        foreach (DataGridColumn column in ne.NewItems) dataGrid.Columns.Add(column);
                        break;
                    case NotifyCollectionChangedAction.Move:
                        dataGrid.Columns.Move(ne.OldStartingIndex, ne.NewStartingIndex);
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        foreach (DataGridColumn column in ne.OldItems) dataGrid.Columns.Remove(column);
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        dataGrid.Columns[ne.NewStartingIndex] = ne.NewItems[0] as DataGridColumn;
                        break;
                }
            };
        }

        public static void SetBindableColumns(DependencyObject element, ObservableCollection<DataGridColumn> value)
        {
            element.SetValue(BindableColumnsProperty, value);
        }

        public static ObservableCollection<DataGridColumn> GetBindableColumns(DependencyObject element)
        {
            return (ObservableCollection<DataGridColumn>)element.GetValue(BindableColumnsProperty);
        }
    }
}
