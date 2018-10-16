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
        public struct TempleteMatchingBestOrder
        {
            //public int s_Index;
            public Point s_Location;
            public double s_Score;
        };
        static public List<TempleteMatchingBestOrder> DoTempleteMatching(Emgu.CV.Image<Emgu.CV.Structure.Gray, Byte> f_Srouce,
                                                                                        Emgu.CV.Image<Emgu.CV.Structure.Gray, Byte> f_Templete,
                                                                                        Emgu.CV.CvEnum.TemplateMatchingType f_TempleteMatchingType,
                                                                                        float f_Threadhold,
                                                                                        int f_GetNumberOfBest)
        {
            List<TempleteMatchingBestOrder> t_SortScore = new List<TempleteMatchingBestOrder>();
            t_SortScore.Clear();
            Emgu.CV.Image<Emgu.CV.Structure.Gray, float> t_Result;
            using (t_Result = f_Srouce.MatchTemplate(f_Templete, f_TempleteMatchingType))
            {
                double[] t_MinValues, t_maxValues;
                Point[] t_MinLocations, t_MaxLocations;
                t_Result.MinMax(out t_MinValues, out t_maxValues, out t_MinLocations, out t_MaxLocations);
                TempleteMatchingBestOrder t_Temp;
                //t_Temp.s_Index = 0;
                t_Temp.s_Location = t_MaxLocations[0];
                t_Temp.s_Score = t_maxValues[0];
                t_SortScore.Add(t_Temp);
                for (int i = 0; i < t_Result.Rows; i++)
                {
                    for (int j = 0; j < t_Result.Cols; j++)
                    {
                        float t_Value = t_Result.Data[i, j, 0];
                        if (t_Value >= f_Threadhold)
                        {
                            t_Temp.s_Location = new Point(i, j);
                            t_Temp.s_Score = t_Value;
                            t_SortScore.Add(t_Temp);
                        }
                    }
                }
            }
            if (t_SortScore.Count > f_GetNumberOfBest)
            {
                return t_SortScore.OrderByDescending(o => o.s_Score).ToList().GetRange(0, 10);
            }
            else
            {
                return t_SortScore.OrderByDescending(o => o.s_Score).ToList().GetRange(0, t_SortScore.Count);
            }

        }

    }
}
