using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuirkSoft
{
    public struct CursorData
    {
        private  int m_deltaX;

        public int DeltaX
        {
            get { return m_deltaX; }
            set { m_deltaX = value; }
        }

        private  int m_deltaY;

        public int DeltaY
        {
            get { return m_deltaY; }
            set { m_deltaY = value; }
        }

        private  Point m_position;

        public Point Position
        {
            get { return m_position; }
            set { m_position = value; }
        } 

        public CursorData(Point position, int deltaX = 0, int deltaY=0)
        {
            m_deltaX = deltaX;
            m_deltaY = deltaY;
            m_position = position;
        }
    }
}
