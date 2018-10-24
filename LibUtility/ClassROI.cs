using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibUtility
{
    public class ClassROI
    {
        static public System.Drawing.Pen m_DefaultPen = new System.Drawing.Pen(System.Drawing.Color.Blue, 2);
        static public System.Drawing.Pen m_SelectedPen = new System.Drawing.Pen(System.Drawing.Color.Green, 2);
        static public System.Drawing.Pen m_DeletedPen = new System.Drawing.Pen(System.Drawing.Color.Red, 2);

        public class S_ROI
        {
            public bool s_IsSelected;
            public bool s_IsDeleted;
            public bool s_IsShow;
            public bool s_IsUseForMask;
            public System.Drawing.Pen s_Pen;
            public System.Drawing.Rectangle s_Rectangle;
            public S_ROI(System.Drawing.Rectangle t_Rectangle)
            {
                s_Rectangle = t_Rectangle;
                s_IsSelected = false;
                s_IsDeleted = false;
                s_IsShow = true;
                s_Pen = m_DefaultPen;
            }
        }

        public List<S_ROI> m_ListAllROI = new List<S_ROI>();
        public List<S_ROI> m_ListCurrentSelectROI = new List<S_ROI>();
        public List<int> m_ListCurrentSelectROIIndex = new List<int>();
        public ClassROI()
        {
            m_DefaultPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            m_SelectedPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            m_DeletedPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            m_ListAllROI.Clear();
            m_ListCurrentSelectROI.Clear();
            m_ListCurrentSelectROIIndex.Clear();
        }

        public void AddROI(System.Drawing.Rectangle f_Rectangle)
        {
            S_ROI t_ROI = new S_ROI(f_Rectangle);
            m_ListAllROI.Add(t_ROI);
        }

        public void ClearAllROI()
        {
            m_ListAllROI.Clear();
        }

        public void DeleteROI()
        {
            for (int i = 0; i < m_ListAllROI.Count; i++)
            {
                if (m_ListAllROI[i].s_IsSelected == true)
                {
                    m_ListAllROI[i].s_IsDeleted = true;
                }
            }
        }
        
        public void ModifyROI(int f_Index, System.Drawing.Rectangle f_Rectangle)
        {
            m_ListAllROI[f_Index].s_Rectangle = f_Rectangle;
        }

        public void SelectROI(System.Drawing.Point f_Point)
        {
            for(int i = 0; i < m_ListAllROI.Count; i++)
            {
                if (ThePointInTheRectangle(f_Point, m_ListAllROI[i].s_Rectangle) == true)
                {
                    m_ListCurrentSelectROIIndex.Add(i);
                    m_ListCurrentSelectROI.Add(m_ListAllROI[i]);
                    m_ListAllROI[i].s_IsSelected = true;
                    m_ListAllROI[i].s_Pen = m_SelectedPen;
                }
            }
        }
        public void SpecifySelectROI(int f_ROIIndex)
        {
            m_ListAllROI[f_ROIIndex].s_IsSelected = true;
            m_ListCurrentSelectROI.Clear();
            m_ListCurrentSelectROIIndex.Clear();
            m_ListCurrentSelectROIIndex.Add(f_ROIIndex);
            m_ListCurrentSelectROI.Add(m_ListAllROI[f_ROIIndex]);
        }

        public void CleanSelectState()
        {
            for (int i = 0; i < m_ListAllROI.Count; i++)
            {
                m_ListAllROI[i].s_IsSelected = false;
                m_ListCurrentSelectROI.Clear();
                m_ListCurrentSelectROIIndex.Clear();
            }
        }

        public void UpdateROI()
        {
            List<S_ROI> t_TempList = new List<S_ROI>();
            for (int i = 0; i < m_ListAllROI.Count; i++)
            {
                if (m_ListAllROI[i].s_IsDeleted == false)
                {
                    t_TempList.Add(m_ListAllROI[i]);
                }
            }
            m_ListAllROI = t_TempList.ToList<S_ROI>();
            t_TempList.Clear();
            t_TempList = null;
            m_ListCurrentSelectROI.Clear();
            m_ListCurrentSelectROIIndex.Clear();
        }

        public bool ThePointInTheRectangle(System.Drawing.Point f_Point, System.Drawing.Rectangle f_Rectangle)
        {
            if (f_Rectangle.X <= f_Point.X)
            {
                if(f_Rectangle.Y <= f_Point.Y)
                {
                    if((f_Rectangle.X + f_Rectangle.Width) >= f_Point.X)
                    {
                        if((f_Rectangle.Y + f_Rectangle.Height) >= f_Point.Y)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
