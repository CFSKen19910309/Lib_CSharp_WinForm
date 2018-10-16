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
    public partial class ZoomPanROIPictureBox : PictureBox
    {

        public ZoomPanROIPictureBox()
        {
            InitializeComponent();
            this.MouseWheel += ZoomPanROIPictureBox_MouseWheel;
            m_ListROI.Clear();

        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
        }

        public enum e_MouseMode
        {
            MOUSE_ZOOM_PAN,
            Mouse_DRAWROI
        }
        e_MouseMode m_MouseMode = e_MouseMode.MOUSE_ZOOM_PAN;
        //e_MouseMode m_MouseOldMode = e_MouseMode.MOUSE_ZOOM_PAN;

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

        private float m_ImageZoomRate = 1.0F;   
        private Point m_ImagePanPos = Point.Empty;      //current offset of image
        private Point m_ImageOldPanPos = Point.Empty;   //The offset of image when mouse was processed.
        private Point m_MouseDownPos = Point.Empty;     //
        private bool m_IsMousePressed = false;             //true as long as left mousebutton is processed.

        public List<Rectangle> m_ListROI = new List<Rectangle>();
        public Rectangle m_ROI;

        public List<Rectangle> GetROI()
        {
            return m_ListROI;
        }

        public void SetROI(Rectangle f_ROI)
        {
            m_ListROI.Add(f_ROI);
            this.Refresh();
        }

        //********************
        //MouseDownEvent
        //********************
        private void ZoomPanROIPictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (ModifierKeys == Keys.Control)
            {
                m_MouseMode = e_MouseMode.Mouse_DRAWROI;
            }
            else
            {
                m_MouseMode = e_MouseMode.MOUSE_ZOOM_PAN;
            }

            if (e.Button == MouseButtons.Left)
            {
                if (!m_IsMousePressed)
                {
                    m_IsMousePressed = true;
                    m_MouseDownPos = e.Location;
                    m_ImageOldPanPos = m_ImagePanPos;
                }
            }
        }
        //********************
        //MouseMoveEvent
        //********************
        private void ZoomPanROIPictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            switch (m_MouseMode)
            {
                case e_MouseMode.Mouse_DRAWROI:
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
                case e_MouseMode.MOUSE_ZOOM_PAN:
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
            switch (m_MouseMode)
            {
                case e_MouseMode.Mouse_DRAWROI:
                    m_ListROI.Add(m_ROI);
                    this.Refresh();
                    break;
                case e_MouseMode.MOUSE_ZOOM_PAN:
                    break;
                default:
                    break;
            }

                
            m_MouseMode = e_MouseMode.MOUSE_ZOOM_PAN;



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
            if (m_IsZoom == false)
            {
                m_ImageZoomRate = (float)ZoomImageFit(this.Size, this.Image.Size);
            }
            if (m_IsPan == false)
            {
                m_ImagePanPos = MoveImageToCenter(this.Size, this.Image.Size, m_ImageZoomRate);
            }

            if (this.Image != null)
            {
                t_Graphic.ScaleTransform(m_ImageZoomRate, m_ImageZoomRate);
                t_Graphic.DrawImage(this.Image, m_ImagePanPos.X, m_ImagePanPos.Y);
                if(m_IsMousePressed == true)
                {
                    t_Graphic.DrawRectangle(new Pen(Color.Blue, 2), m_ROI.X + m_ImagePanPos.X, m_ROI.Y + m_ImagePanPos.Y, m_ROI.Width, m_ROI.Height);
                }
                for (int i = 0; i < m_ListROI.Count; i ++)
                {
                    t_Graphic.DrawRectangle(new Pen(Color.Blue, 2), m_ListROI[i].X + m_ImagePanPos.X, m_ListROI[i].Y + m_ImagePanPos.Y, m_ListROI[i].Width, m_ListROI[i].Height);
                }
                
            }
        }

        public void FitImageToCenter()
        {
            m_ImageZoomRate = (float)ZoomImageFit(this.Size, this.Image.Size);
            m_ImagePanPos = MoveImageToCenter(this.Size, this.Image.Size, m_ImageZoomRate);
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
        

        
    }
}
