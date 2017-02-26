using System;
using System.Collections.Generic;
using System.Linq;

namespace ECS.Controls
{
    public class SelectionService
    {
        public SelectionService(DesignerCanvas canvas)
        {
            _designerCanvas = canvas;
        }

        private readonly DesignerCanvas _designerCanvas;
        private List<ISelectable> _currentSelection;

        internal List<ISelectable> CurrentSelection
            => _currentSelection ?? (_currentSelection = new List<ISelectable>());

        internal void SelectItem(ISelectable item)
        {
            ClearSelection();
            AddToSelection(item);
        }

        internal void AddToSelection(ISelectable item)
        {
            if (item is IGroupable)
            {
                var groupItems = GetGroupMembers((IGroupable)item);

                foreach (ISelectable groupItem in groupItems)
                {
                    groupItem.IsSelected = true;
                    CurrentSelection.Add(groupItem);
                }
            }
            else
            {
                item.IsSelected = true;
                CurrentSelection.Add(item);
            }
        }

        internal void RemoveFromSelection(ISelectable item)
        {
            if (item is IGroupable)
            {
                var groupItems = GetGroupMembers((IGroupable)item);

                foreach (ISelectable groupItem in groupItems)
                {
                    groupItem.IsSelected = false;
                    CurrentSelection.Remove(groupItem);
                }
            }
            else
            {
                item.IsSelected = false;
                CurrentSelection.Remove(item);
            }
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

        internal List<IGroupable> GetGroupMembers(IGroupable item)
        {
            var list = _designerCanvas.Children.OfType<IGroupable>();
            var rootItem = GetRoot(list, item);
            return GetGroupMembers(list, rootItem);
        }

        internal IGroupable GetGroupRoot(IGroupable item)
        {
            var list = _designerCanvas.Children.OfType<IGroupable>();
            return GetRoot(list, item);
        }

        private static IGroupable GetRoot(IEnumerable<IGroupable> list, IGroupable node)
        {
            if (node == null || node.ParentId == Guid.Empty) return node;
            return (from item in list
                    where item.Id == node.ParentId
                    select GetRoot(list, item)).FirstOrDefault();
        }

        private List<IGroupable> GetGroupMembers(IEnumerable<IGroupable> list, IGroupable parent)
        {
            var groupMembers = new List<IGroupable> { parent };

            var children = list.Where(node => node.ParentId == parent.Id);

            foreach (var child in children) groupMembers.AddRange(GetGroupMembers(list, child));

            return groupMembers;
        }
    }
}
