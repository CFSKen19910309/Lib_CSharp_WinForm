using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace LibUtility
{
    public enum E_LeftMouseButtonMode
    {
        e_MoveImage,
        e_AddROI,
        e_SelectROI,
        e_ModifyROI,
        e_DeleteROI
    };

    public partial class ZoomPanROIPictureBox : PictureBox
    {
        //Initaial ZoomPanROIPictureBox;

        public ZoomPanROIPictureBox()
        {
            InitializeComponent();
            this.MouseWheel += ZoomPanROIPictureBox_MouseWheel;
            
            m_ManagerROI.m_ListAllROI.Clear();
            m_ManagerROI.m_ListCurrentSelectROI.Clear();
            m_ManagerROI.m_ListCurrentSelectROIIndex.Clear();
        }
        //Left button mode include "Move Image", "Add ROI", "Select ROI", "Modify ROI", "Delete ROI"
        int m_LeftButtonMode;

        //Create a context menu strip
        ContextMenuStrip m_ContextMenuStrip = new ContextMenuStrip();
        private void CreateContextMenuStrip()
        {
            m_ContextMenuStrip.Items.Clear();
            m_ContextMenuStrip.Items.Add("Fit Image To PictureBox");
            m_ContextMenuStrip.Items[0].Click += new EventHandler(FitImageToCenter);
            m_ContextMenuStrip.Items.Add("-");
            m_ContextMenuStrip.Items.Add("Move Image");
            m_ContextMenuStrip.Items[2].Click += new EventHandler(ClickROIMode);
            m_ContextMenuStrip.Items.Add("Add ROI");
            m_ContextMenuStrip.Items[3].Click += new EventHandler(ClickROIMode);
            m_ContextMenuStrip.Items.Add("Select ROI");
            m_ContextMenuStrip.Items[4].Click += new EventHandler(ClickROIMode);
            m_ContextMenuStrip.Items.Add("Modify ROI");
            m_ContextMenuStrip.Items[5].Click += new EventHandler(ClickROIMode);
            m_ContextMenuStrip.Items.Add("Delete ROI");
            m_ContextMenuStrip.Items[6].Click += new EventHandler(DeleteSelectedROI);
            m_ContextMenuStrip.Items.Add("-");
            switch(m_LeftButtonMode)
            {
                case (int)E_LeftMouseButtonMode.e_MoveImage:
                    {
                        ((ToolStripMenuItem)m_ContextMenuStrip.Items[2]).Checked = true;
                        break;
                    }
                case (int)E_LeftMouseButtonMode.e_AddROI:
                    {
                        ((ToolStripMenuItem)m_ContextMenuStrip.Items[3]).Checked = true;
                        break;
                    }
                case (int)E_LeftMouseButtonMode.e_SelectROI:
                    {
                        ((ToolStripMenuItem)m_ContextMenuStrip.Items[4]).Checked = true;
                        break;
                    }
                case (int)E_LeftMouseButtonMode.e_ModifyROI:
                    {
                        ((ToolStripMenuItem)m_ContextMenuStrip.Items[5]).Checked = true;
                        break;
                    }
            }
            m_ContextMenuStrip.Show(MousePosition);
        }
        
        //the event from the ROI mode of context menu
        private void ClickROIMode(object sender, EventArgs e)
        {
            ToolStripMenuItem t_ToolStripMenuItem = sender as ToolStripMenuItem;
            if(t_ToolStripMenuItem.Text == "Move Image")
            {
                m_LeftButtonMode = (int)E_LeftMouseButtonMode.e_MoveImage;
            }
            if (t_ToolStripMenuItem.Text == "Add ROI")
            {
                t_ToolStripMenuItem.Checked = true;
                m_LeftButtonMode = (int)E_LeftMouseButtonMode.e_AddROI;
            }
            if (t_ToolStripMenuItem.Text == "Select ROI")
            {
                t_ToolStripMenuItem.Checked = true;
                m_LeftButtonMode = (int)E_LeftMouseButtonMode.e_SelectROI;
            }
            if (t_ToolStripMenuItem.Text == "Modify ROI")
            {
                t_ToolStripMenuItem.Checked = true;
                m_LeftButtonMode = (int)E_LeftMouseButtonMode.e_ModifyROI;
            }
            if (t_ToolStripMenuItem.Text == "Delete ROI")
            {
                t_ToolStripMenuItem.Checked = true;
                m_LeftButtonMode = (int)E_LeftMouseButtonMode.e_DeleteROI;
            }
        }

        public void DeleteSelectedROI(object sender, EventArgs e)
        {
            m_ManagerROI.DeleteROI();
            m_ManagerROI.UpdateROI();
            this.Refresh();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
        }
      
        //set the picture box property
        private bool m_IsPan = true;
        private bool m_IsZoom = true;
        private bool m_IsDraw = true;
        private bool m_IsSelect = true;
        public void SetPictureBoxProperty(bool f_IsPan = true, bool f_IsZoom = true, bool f_IsDraw = true, bool f_IsSelect = true)
        {
            m_IsPan = f_IsPan;
            m_IsZoom = f_IsZoom;
            m_IsDraw = f_IsZoom;
            m_IsSelect = f_IsSelect;
        }
        //picture box state
        private float m_ImageZoomRate = 1.0F;   
        private Point m_ImagePanPos = Point.Empty;      //current offset of image
        private Point m_ImageOldPanPos = Point.Empty;   //The offset of image when mouse was processed.
        private Point m_MouseDownPos = Point.Empty;     //
        private bool m_IsMousePressed = false;             //true as long as left mousebutton is processed.

        //the data is from ClasssROI
        public LibUtility.ClassROI m_ManagerROI = new LibUtility.ClassROI();
        public Rectangle m_ROI;
        public List<LibUtility.ClassROI.S_ROI> GetSelectedROI()
        {
            m_ManagerROI.m_ListCurrentSelectROI.Clear();
            for (int i = 0; i < m_ManagerROI.m_ListAllROI.Count; i++)
            {
                if(m_ManagerROI.m_ListAllROI[i].s_IsSelected == true)
                {
                    m_ManagerROI.m_ListCurrentSelectROI.Add(m_ManagerROI.m_ListAllROI[i]);
                }
            }
            return m_ManagerROI.m_ListCurrentSelectROI;
        }

        public void ClearROI()
        {
            m_ManagerROI.m_ListAllROI.Clear();
            m_ManagerROI.m_ListCurrentSelectROI.Clear();
            m_ManagerROI.m_ListCurrentSelectROIIndex.Clear();
            this.Refresh();
        }
        
        //********************
        //MouseDownEvent
        //********************
        private void ZoomPanROIPictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Right)
            {
                CreateContextMenuStrip();
            }
            if (e.Button == MouseButtons.Left)
            {
                if (m_LeftButtonMode == (int)E_LeftMouseButtonMode.e_AddROI ||
                    m_LeftButtonMode == (int)E_LeftMouseButtonMode.e_ModifyROI ||
                    m_LeftButtonMode == (int)E_LeftMouseButtonMode.e_MoveImage ||
                    m_LeftButtonMode == (int)E_LeftMouseButtonMode.e_SelectROI)
                {
                    if (!m_IsMousePressed)
                    {
                        m_IsMousePressed = true;
                        m_MouseDownPos = e.Location;
                        m_ImageOldPanPos = m_ImagePanPos;
                    }
                }
            }
        }
        //********************
        //MouseMoveEvent
        //********************
        private void ZoomPanROIPictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if(m_IsMousePressed == false)
            {
                return;
            }
            switch (m_LeftButtonMode)
            {
                case (int)E_LeftMouseButtonMode.e_AddROI:
                    if (e.Button == MouseButtons.Left)
                    {
                        Point t_MousePosNow = e.Location;

                        int t_DeltaX = (t_MousePosNow.X - m_MouseDownPos.X);
                        int t_DeltaY = (t_MousePosNow.Y - m_MouseDownPos.Y);

                        //m_ROI.X = (int)(m_MouseDownPos.X / m_ImageZoomRate);
                        //m_ROI.Y = (int)(m_MouseDownPos.Y / m_ImageZoomRate);
                        //m_ROI.Width = (int)(t_DeltaX / m_ImageZoomRate);
                        //m_ROI.Height = (int)(t_DeltaY / m_ImageZoomRate);

                        //m_ROI.X = (int)(m_MouseDownPos.X / m_ImageZoomRate - m_ImagePanPos.X / m_ImageZoomRate);
                        //m_ROI.Y = (int)(m_MouseDownPos.Y / m_ImageZoomRate - m_ImagePanPos.Y / m_ImageZoomRate);
                        m_ROI.X = (int)(m_MouseDownPos.X / m_ImageZoomRate - m_ImagePanPos.X);
                        m_ROI.Y = (int)(m_MouseDownPos.Y / m_ImageZoomRate - m_ImagePanPos.Y);
                        m_ROI.Width = (int)(t_DeltaX / m_ImageZoomRate);
                        m_ROI.Height = (int)(t_DeltaY / m_ImageZoomRate);
                        
                        this.Refresh();
                    }
                    break;
                case (int)E_LeftMouseButtonMode.e_MoveImage:
                    {
                        if (e.Button == MouseButtons.Left)
                        {
                            Point t_MousePosNow = e.Location;
                            //the distance the mouse has been moved since mouse was pressed.
                            int t_DeltaX = (t_MousePosNow.X - m_MouseDownPos.X);
                            int t_DeltaY = (t_MousePosNow.Y - m_MouseDownPos.Y);
                            //The Calculate new offset of image based on the current zoom factor.
                            m_ImagePanPos.X = (int)(m_ImageOldPanPos.X + (t_DeltaX / m_ImageZoomRate));
                            m_ImagePanPos.Y = (int)(m_ImageOldPanPos.Y + (t_DeltaY / m_ImageZoomRate));

                            this.Refresh();
                        }
                        break;
                    }
                case (int)E_LeftMouseButtonMode.e_ModifyROI:
                    {
                        
                        break;
                    }
                    
                default:
                    break;
            }
        }
        //********************
        //MouseUpEvent
        //********************
        private void ZoomPanROIPictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            m_IsMousePressed = false;
            if (m_LeftButtonMode == (int)E_LeftMouseButtonMode.e_AddROI)
            {
                m_ManagerROI.m_ListAllROI.Add(new LibUtility.ClassROI.S_ROI(m_ROI));
            }
            if(m_LeftButtonMode == (int)E_LeftMouseButtonMode.e_SelectROI)
            {
                Point t_Point = new Point();
                t_Point.X = (int)(e.X / m_ImageZoomRate - m_ImagePanPos.X);
                t_Point.Y = (int)(e.Y / m_ImageZoomRate - m_ImagePanPos.Y);
                m_ManagerROI.SelectROI(t_Point);
            }
            this.Refresh();
        }

        //********************
        //MouseWheelEvent
        //********************
        private void ZoomPanROIPictureBox_MouseWheel(object sender, MouseEventArgs e)
        {
            float t_OldImageZoom = m_ImageZoomRate;
            if (e.Delta > 0)
            {
                m_ImageZoomRate += 0.1F;
            }
            else
            {
                m_ImageZoomRate = Math.Max(m_ImageZoomRate - 0.1F, 0.05F);
            }

            Point t_MousePosNow = e.Location;
            //I Use the PictureBox Event, so the 
            // Where location of the mouse in the pictureframe
            //int x = t_MousePosNow.X - PictureBox.Location.X;
            //int y = t_MousePosNow.Y - PictureBox.Location.Y;

            // Where in the Image is it now
            int t_OldImagePanX = (int)(t_MousePosNow.X / t_OldImageZoom);
            int t_OldImagePanY = (int)(t_MousePosNow.Y / t_OldImageZoom);
            // Where in the IMAGE will it be when the new zoom I made

            int t_NewImagePanX = (int)(t_MousePosNow.X / m_ImageZoomRate);
            int t_NewImagePanY = (int)(t_MousePosNow.Y / m_ImageZoomRate);
            // Where to move image to keep focus on one point

            m_ImagePanPos.X = t_NewImagePanX - t_OldImagePanX + m_ImagePanPos.X;
            m_ImagePanPos.Y = t_NewImagePanY - t_OldImagePanY + m_ImagePanPos.Y;

            this.Refresh();
        }


        
        private void ZoomPanROIPictureBox_Paint(object sender, PaintEventArgs e)
        {
            Graphics t_Graphic = e.Graphics;
            //(Why) There are something Event triggered.
            //Graphics t_Graphic = this.CreateGraphics();
            t_Graphic.Clear(Color.White);

            if (this.Image != null)
            {
                if (m_IsZoom == false)
                {
                    m_ImageZoomRate = (float)ZoomImageFit(this.Size, this.Image.Size);
                }
                if (m_IsPan == false)
                {
                    m_ImagePanPos = MoveImageToCenter(this.Size, this.Image.Size, m_ImageZoomRate);
                }
                t_Graphic.ScaleTransform(m_ImageZoomRate, m_ImageZoomRate);
                t_Graphic.DrawImage(this.Image, m_ImagePanPos.X, m_ImagePanPos.Y);
                if (m_IsMousePressed == true)
                {
                    t_Graphic.DrawRectangle(new Pen(Color.Blue, 2), m_ROI.X + m_ImagePanPos.X, m_ROI.Y + m_ImagePanPos.Y, m_ROI.Width, m_ROI.Height);
                }
                for (int i = 0; i < m_ManagerROI.m_ListAllROI.Count; i ++)
                {
                    t_Graphic.DrawRectangle(m_ManagerROI.m_ListAllROI[i].s_Pen,
                        m_ManagerROI.m_ListAllROI[i].s_Rectangle.X + m_ImagePanPos.X, m_ManagerROI.m_ListAllROI[i].s_Rectangle.Y + m_ImagePanPos.Y,
                        m_ManagerROI.m_ListAllROI[i].s_Rectangle.Width, m_ManagerROI.m_ListAllROI[i].s_Rectangle.Height);
                }
            }
        }

        public void  FitImageToCenter(object o = null, EventArgs e = null)
        {
            m_ImageZoomRate = (float)ZoomImageFit(this.Size, this.Image.Size);
            
            m_ImagePanPos = MoveImageToCenter(this.Size, this.Image.Size, m_ImageZoomRate);
            this.Refresh();
        }

        public double ZoomImageFit(Size f_PictureBoxSize, Size f_ImageSize)
        {
            double t_PictureBoxHeight = f_PictureBoxSize.Height;
            double t_PictureBoxWidth = f_PictureBoxSize.Width;
            double t_ImageHeight = f_ImageSize.Height;
            double t_ImageWidth = f_ImageSize.Width;
            double t_ZoomHieght = t_PictureBoxHeight / t_ImageHeight;
            double t_ZoomWidth = t_PictureBoxWidth / t_ImageWidth;
            if (t_ZoomHieght > t_ZoomWidth)
            {
                return t_ZoomWidth;
            }
            else
            {
                return t_ZoomHieght;
            }
        }

        private Point MoveImageToCenter(Size f_PictureBoxSize, Size f_Image, float ZoomValue = 1)
        {
            Point t_CenterPoint = Point.Empty;
            t_CenterPoint.X = (int)(f_PictureBoxSize.Width - f_Image.Width * ZoomValue) / 2;
            t_CenterPoint.Y = (int)(f_PictureBoxSize.Height - f_Image.Height * ZoomValue) / 2;
            t_CenterPoint.X = (int)(t_CenterPoint.X / ZoomValue);
            t_CenterPoint.Y = (int)(t_CenterPoint.Y / ZoomValue);
            return t_CenterPoint;
        }

        private void ZoomPanROIPictureBox_Resize(object sender, EventArgs e)
        {
            this.Invalidate();
        }
        
    }
    
}
