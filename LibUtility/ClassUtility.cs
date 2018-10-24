using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
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

        public enum E_ImageColor
        {
            e_Color,
            e_R,
            e_G,
            e_B,
            e_Gray
        }
        public static Emgu.CV.Mat  ConvertImageColor(Emgu.CV.Mat f_Image, int ConvertTo)
        {
            Emgu.CV.Mat t_ResultImage = new Emgu.CV.Mat();
            t_ResultImage = f_Image;
            if (f_Image.NumberOfChannels < 3)
            {
                return t_ResultImage;
            }
            switch (ConvertTo)
            {
                case (int)E_ImageColor.e_Color:
                    {
                        t_ResultImage = f_Image;
                        break;
                    }
                case (int)E_ImageColor.e_R:
                    {
                        Emgu.CV.Mat[] t_ImageSplit = f_Image.Split();
                        t_ResultImage = t_ImageSplit[2];
                        break;
                    }
                case (int)E_ImageColor.e_G:
                    {
                        Emgu.CV.Mat[] t_ImageSplit = f_Image.Split();
                        t_ResultImage = t_ImageSplit[1];
                        break;
                    }
                case (int)E_ImageColor.e_B:
                    {
                        Emgu.CV.Mat[] t_ImageSplit = f_Image.Split();
                        t_ResultImage = t_ImageSplit[0];
                        break;
                    }
                case (int)E_ImageColor.e_Gray:
                    {
                        f_Image.ConvertTo(t_ResultImage, Emgu.CV.CvEnum.DepthType.Cv8U);
                        break;
                    }
                default:
                    {
                        t_ResultImage = f_Image;
                        break;
                    }
            }
            return t_ResultImage;



        }
        public static void TesseractDownloadLangFile(String f_Folder, List<String> f_Lang)
        {
            String t_Folder = String.Format("{0}{1}", f_Folder, "tessdata");
            
            if(!System.IO.Directory.Exists(t_Folder))
            {
                System.IO.Directory.CreateDirectory(t_Folder);
            }
            foreach (String t_Lang in f_Lang)
            {
                String t_Destination = System.IO.Path.Combine(t_Folder, String.Format("{0}.traineddata", t_Lang));
                if(!System.IO.File.Exists(t_Destination))
                {
                    using (System.Net.WebClient t_WebClient = new System.Net.WebClient())
                    {
                        String t_Source = String.Format("https://github.com/tesseract-ocr/tessdata/blob/4592b8d453889181e01982d22328b5846765eaad/{0}.traineddata?raw=true", t_Lang);
                        Console.WriteLine(String.Format("Downloading file from '{0}' to '{1}'", t_Source, t_Destination));
                        t_WebClient.DownloadFile(t_Source, t_Destination);
                        Console.WriteLine(String.Format("Download {0} Finished", t_Lang));
                    }
                }
            }
        }
        //highlight : This is a Rule to define the Directory format
        //Format the Directory String with end of directory separactor or relately directory with begin of dot
        public static String CheckDirectoyFormat(String f_DirectoryString)
        {
            String t_DirectoryString = f_DirectoryString;
            if (t_DirectoryString.Length == 0)
            {
                t_DirectoryString = String.Format("{0}", '.');
            }
            //highlight System.IO.Path.DirectorySeparatorChar is a char, it is not a string.
            //if we want to comapre between String and Char, we wiil get a trouble.
            if (t_DirectoryString.Substring(t_DirectoryString.Length - 1, 1).Equals(System.IO.Path.DirectorySeparatorChar.ToString()) == false)
            {
                t_DirectoryString = String.Format("{0}{1}", t_DirectoryString, System.IO.Path.DirectorySeparatorChar);
            }
            return t_DirectoryString;
        }
        public static void InitialOCR(ref Emgu.CV.OCR.Tesseract f_OCR, String f_Folder, String f_Lang, Emgu.CV.OCR.OcrEngineMode f_OcrEngineMode = Emgu.CV.OCR.OcrEngineMode.TesseractLstmCombined)
        {
            try
            {
                //check OCR is if it is clear
                if (f_OCR != null)
                {
                    f_OCR.Dispose();
                    f_OCR = null;
                }
                //check Directory format is correct
                String t_Folder;
                t_Folder = CheckDirectoyFormat(f_Folder);

                //extract the language item
                List<String> t_DownloadLangFile = new List<String>();
                if (f_Lang.Length == 0)
                {
                    f_Lang = String.Format("{0}", "eng");
                    t_DownloadLangFile.Add("eng");
                }
                else
                {
                    String[] t_LangSplit;
                    t_LangSplit = (String[])f_Lang.Split('+');
                    t_DownloadLangFile.AddRange(t_LangSplit);
                }
                TesseractDownloadLangFile(t_Folder, t_DownloadLangFile);

                f_OCR = new Emgu.CV.OCR.Tesseract(t_Folder, f_Lang, f_OcrEngineMode);
            }
            catch (Exception e)
            {
                f_OCR = null;
                MessageBox.Show(e.Message, "Failed to initialize tesseract OCR engine", MessageBoxButtons.OK);
            }
        }
        
    }
}
