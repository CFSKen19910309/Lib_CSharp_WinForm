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
        }
        
        private float m_ImageZoomRate = 1.0F;   //
        private Point m_ImagePanPos = Point.Empty;      //current offset of image
        private Point m_ImageOldPanPos = Point.Empty;   //The offset of image when mouse was processed.
        private Point m_MouseDownPos = Point.Empty;     //
        private bool m_IsMousePressed = false;             //true as long as left mousebutton is processed.
        

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            
        }
        
        private void ZoomPanROIPictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
            {
                if(!m_IsMousePressed)
                {
                    m_IsMousePressed = true;
                    m_MouseDownPos = e.Location;
                    m_ImageOldPanPos = m_ImagePanPos;
                }
            }

        }

        private void ZoomPanROIPictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
            {
                Point t_MousePosNow = e.Location;
                //the distance the mouse has been moved since mouse was pressed.
                int t_DeltaX = (t_MousePosNow.X - m_MouseDownPos.X);
                int t_DeltaY = (t_MousePosNow.Y - m_MouseDownPos.Y);
                //The Calculate new offset of image based on the current zoom factor.
                m_ImagePanPos.X = (int)(m_ImageOldPanPos.X + (t_DeltaX / m_ImageZoomRate));
                m_ImagePanPos.X = (int)(m_ImageOldPanPos.Y + (t_DeltaY / m_ImageZoomRate));

                this.Refresh();
            }
        }

        private void ZoomPanROIPictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            m_IsMousePressed = false;
        }

        private void ZoomPanROIPictureBox_Paint(object sender, PaintEventArgs e)
        {
            Graphics t_Graphic = e.Graphics;
            //(Why) There are something Event triggered.
            //Graphics t_Graphic = this.CreateGraphics();
            t_Graphic.Clear(Color.White);
            if(this.Image != null)
            {
                t_Graphic.ScaleTransform(m_ImageZoomRate, m_ImageZoomRate);
                t_Graphic.DrawImage(this.Image, m_ImagePanPos.X, m_ImagePanPos.Y);
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
