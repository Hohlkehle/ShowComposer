using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowComposer.DraggableUI
{
    public class DeskLayoutEventArgs : EventArgs
    {
        private UIElement m_UIElement;
        private UIElementEventType m_UIElementEventType;

        public UIElement UIElement
        {
            set { m_UIElement = value; }
            get { return m_UIElement; }
        }

        public UIElementEventType UIElementEventType
        {
            set { m_UIElementEventType = value; }
            get { return m_UIElementEventType; }
        }

        public DeskLayoutEventArgs(UIElement element)
        {
            m_UIElement = element;
        }
    }
}
