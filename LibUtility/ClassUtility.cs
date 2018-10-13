using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace LibUtility
{
    public class ClassUtility
    {
        static public String SaveAndOpenFileFilter =    "bmp file (*.bmp)|*.bmp|" +
                                                        "jpeg file (*.jpeg)|*.jpeg|" +
                                                        "jpg file (*.jpg)|*.jpg|" +
                                                        "png file (*.jpg)|*.png|" +
                                                        "all file (*.*)|*.*";
        static public String OpenImageFile()
        {
            String t_OpenImagePath = String.Empty;
            using (OpenFileDialog t_OpenFileDialog = new OpenFileDialog())
            {
                t_OpenFileDialog.InitialDirectory = @"C:\Users\Public\Documents\MVTec\HALCON-13.0\examples\images";
                t_OpenFileDialog.Multiselect = false;
                t_OpenFileDialog.Filter = SaveAndOpenFileFilter;
                t_OpenFileDialog.FilterIndex = 5;
                t_OpenFileDialog.RestoreDirectory = false;
                if (t_OpenFileDialog.ShowDialog() != DialogResult.OK)
                {
                    return String.Empty;
                }
                return t_OpenFileDialog.FileName;
            }
        }
        static public String SaveImageFile()
        {
            String t_SaveImagePath = String.Empty;
            using (SaveFileDialog t_SaveFileDialog = new SaveFileDialog())
            {
                t_SaveFileDialog.Filter = SaveAndOpenFileFilter;
                t_SaveFileDialog.FilterIndex = 1;
                if(t_SaveFileDialog.ShowDialog() != DialogResult.OK)
                {
                    return String.Empty;
                }
                return t_SaveFileDialog.FileName;
            }
        }

        static public double ZoomImageFit(Size f_PictureBoxSize, Size f_ImageSize)
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
        static public void InitialComboBox(ref ComboBox f_ComboBox, String[] f_Items)
        {
            f_ComboBox.Items.Clear();
            f_ComboBox.Items.AddRange(f_Items);
            f_ComboBox.SelectedIndex = 0;
        }

        
    }
}
