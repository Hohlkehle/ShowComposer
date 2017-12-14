using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ShowComposer.DraggableUI
{
    public class LayoutEditorSelectionEventArgs : EventArgs
    {
        private readonly UIElement m_UIElement;

        public UIElement UIElement
        {
            get { return m_UIElement; }
        }

        private readonly bool m_IsSelected;

        public bool IsSelected
        {
            get { return m_IsSelected; }
        }


        public LayoutEditorSelectionEventArgs(UIElement element, bool selected)
        {
            m_UIElement = element;
            m_IsSelected = selected;
        }
    }
}
