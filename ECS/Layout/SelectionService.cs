using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace ECS.Layout
{
    public class SelectionService
    {
        public SelectionService([NotNull] DesignerCanvas canvas)
        {
            _designerCanvas = canvas;
        }

        [NotNull]
        private readonly DesignerCanvas _designerCanvas;
        private List<ISelectable> _currentSelection;

        [NotNull]
        internal List<ISelectable> CurrentSelection
            => _currentSelection ?? (_currentSelection = new List<ISelectable>());

        internal void SelectItem(ISelectable item)
        {
            ClearSelection();
            AddToSelection(item);
        }

        internal void AddToSelection(ISelectable item)
        {
                item.IsSelected = true;
                CurrentSelection.Add(item);
        }

        internal void RemoveFromSelection(ISelectable item)
        {
                item.IsSelected = false;
                CurrentSelection.Remove(item);
        }

        internal void ClearSelection()
        {
            CurrentSelection.ForEach(item => item.IsSelected = false);
            CurrentSelection.Clear();
        }

        internal void SelectAll()
        {
            ClearSelection();
            CurrentSelection.AddRange(_designerCanvas.Children.OfType<ISelectable>());
            CurrentSelection.ForEach(item => item.IsSelected = true);
        }
    }
}
