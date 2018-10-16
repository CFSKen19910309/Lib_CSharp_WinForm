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
        static public List<TempleteMatchingBestOrder> DoTempleteMatching(Emgu.CV.Image<Emgu.CV.Structure.Bgr, byte> f_Source,
                                                                                        Emgu.CV.Image<Emgu.CV.Structure.Bgr, byte> f_Templete,
                                                                                        Emgu.CV.CvEnum.TemplateMatchingType f_TempleteMatchingType = Emgu.CV.CvEnum.TemplateMatchingType.CcoeffNormed,
                                                                                        float f_Threadhold = 0.9F,
                                                                                        int f_GetNumberOfBest = 10,
                                                                                        bool f_IsUsePyramid = false,
                                                                                        int f_PyramidLevel = 1)
        {
            List<TempleteMatchingBestOrder> t_SortScore = new List<TempleteMatchingBestOrder>();
            t_SortScore.Clear();
            Emgu.CV.Image<Emgu.CV.Structure.Gray, float> t_Result;
            if (f_IsUsePyramid == false)
            {
                t_Result = f_Source.Convert<Emgu.CV.Structure.Gray, byte>().MatchTemplate(f_Templete.Convert<Emgu.CV.Structure.Gray, byte>(), f_TempleteMatchingType);
                double[] t_MinValues, t_maxValues;
                Point[] t_MinLocations, t_MaxLocations;
                t_Result.MinMax(out t_MinValues, out t_maxValues, out t_MinLocations, out t_MaxLocations);
                TempleteMatchingBestOrder t_Temp;
                for (int i = 0; i < t_Result.Rows; i++)
                {
                    for (int j = 0; j < t_Result.Cols; j++)
                    {
                        float t_Value = t_Result.Data[i, j, 0];
                        if (t_Value >= f_Threadhold)
                        {
                            t_Temp.s_Location = new Point(j, i);
                            t_Temp.s_Score = t_Value;
                            t_SortScore.Add(t_Temp);
                        }
                    }
                }
            }
            else
            {
                Emgu.CV.Image<Emgu.CV.Structure.Gray, byte>[] t_PyramidSource = new Emgu.CV.Image<Emgu.CV.Structure.Gray, byte>[f_PyramidLevel];
                Emgu.CV.Image<Emgu.CV.Structure.Gray, byte>[] t_PyramidTemplete = new Emgu.CV.Image<Emgu.CV.Structure.Gray, byte>[f_PyramidLevel];
                t_PyramidSource[0] = f_Source.Convert<Emgu.CV.Structure.Gray, byte>().Clone();
                t_PyramidTemplete[0] = f_Templete.Convert<Emgu.CV.Structure.Gray, byte>().Clone();
                int[] t_Level = new int[f_PyramidLevel];
                t_Level[0] = 1;
                for (int i = 1; i < f_PyramidLevel; i++)
                {
                    t_Level[i] = t_Level[i - 1] * 2;
                    t_PyramidSource[i] = t_PyramidSource[i-1].PyrDown().Clone();
                    t_PyramidTemplete[i] = t_PyramidTemplete[i-1].PyrDown().Clone();
                }
                for(int i = f_PyramidLevel-1; i >= 0; i--)
                {
                    for (int j = f_PyramidLevel-1;  j >= 0; j--)
                    {
                        if(t_PyramidTemplete[j].Width > t_PyramidSource[i].Width || t_PyramidTemplete[j].Height > t_PyramidSource[i].Height)
                        {
                            continue;
                        }
                        t_Result = t_PyramidSource[i].MatchTemplate(t_PyramidTemplete[j], f_TempleteMatchingType);
                        double[] t_MinValues, t_maxValues;
                        Point[] t_MinLocations, t_MaxLocations;
                        t_Result.MinMax(out t_MinValues, out t_maxValues, out t_MinLocations, out t_MaxLocations);
                        if((float)(t_maxValues[0]) <= f_Threadhold)
                        {
                            continue;
                        }
                        TempleteMatchingBestOrder t_Temp;
                        for (int x = 0; x < t_Result.Rows; x++)
                        {
                            for (int y = 0; y < t_Result.Cols; y++)
                            {
                                float t_Value = t_Result.Data[x, y, 0];
                                if (t_Value >= f_Threadhold)
                                {
                                    t_Temp.s_Location = new Point(y * t_Level[i], x * t_Level[i]);
                                    t_Temp.s_Score = t_Value;
                                    t_SortScore.Add(t_Temp);
                                }
                            }
                        }
                    }
                }
            }
            if (t_SortScore.Count > f_GetNumberOfBest)
            {
                return t_SortScore.OrderByDescending(o => o.s_Score).ToList().GetRange(0, f_GetNumberOfBest);
            }
            else
            {
                return t_SortScore.OrderByDescending(o => o.s_Score).ToList().GetRange(0, t_SortScore.Count);
            }

        }
        public static Bitmap GetImageFromURL()
        {
            Bitmap t_Bitmap = null;
            try
            {
                string t_URL = string.Format(@"http://localhost:21173/getScreen?label={0}", "1");
                System.Net.WebClient t_WebClient = new System.Net.WebClient();
                byte[] data = t_WebClient.DownloadData(t_URL);
                using (var t_MemoryStream = new System.IO.MemoryStream(data))
                {
                    return new Bitmap(t_MemoryStream);
                }
            }
            catch(Exception ex)
            {
                
            }
            return t_Bitmap;
        }
    }
}
