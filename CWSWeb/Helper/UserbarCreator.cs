using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWSWeb.Helper
{
    static class UserbarCreator
    {
        static string baseFile = Path.Combine(Directory.GetCurrentDirectory(), "Content", "base.png");
        static string fontFile = Path.Combine(Directory.GetCurrentDirectory(), "Content", "font.ttf");
        static string outputFile = Path.Combine(Directory.GetCurrentDirectory(), "Files", "userbar.png");

        static Color fillColor = Color.FromArgb(175, 25, 25, 25);
        static Color onlineColor = Color.FromArgb(255, 0, 213, 55);
        static Color offlineColor = Color.FromArgb(255, 255, 63, 25);
        static Color genericColor = Color.White;

        static int rectangleHeight = 20;
        static int afterSpace = 5;
        static int spacing = 5;

        public static void GenerateUserbar(CachedVariables.Statistics data)
        {
            try
            {
                if (data.Enabled && File.Exists(baseFile) && File.Exists(fontFile))
                {
                    using (Image i = Image.FromFile(baseFile))
                    {
                        using(Bitmap b = new Bitmap(i))
                        {
                            PrivateFontCollection fonts = new PrivateFontCollection();
                            fonts.AddFontFile(fontFile);
                            FontFamily fontfamily = fonts.Families[0];                       

                            Rectangle fontArea = new Rectangle(0, b.Height - rectangleHeight, b.Width, rectangleHeight);
                            int fontSize = rectangleHeight - 4;
                            Font font = new Font(fontfamily, fontSize, GraphicsUnit.Pixel);

                            int marginTop = b.Height - rectangleHeight + 3;
                            int marginLeft = 2;

                            Graphics g = Graphics.FromImage(b);

                            g.FillRectangle(new SolidBrush(fillColor), fontArea);

                            g.DrawString("Status:", font, new SolidBrush(genericColor), new PointF(marginLeft, marginTop));
                            marginLeft += (int)g.MeasureString("Status:", font).Width + afterSpace;

                            if (data.IsAlive)
                            {
                                g.DrawString("Online", font, new SolidBrush(onlineColor), new PointF(marginLeft, marginTop));
                                marginLeft += (int)g.MeasureString("Online", font).Width + spacing * afterSpace;

                                if (data.PlayerStats.Current > 0)
                                {
                                    g.DrawString(data.PlayerStats.Current.ToString(), font, new SolidBrush(onlineColor), new PointF(marginLeft, marginTop));
                                    marginLeft += (int)g.MeasureString(data.PlayerStats.Current.ToString(), font).Width + afterSpace;

                                    g.DrawString("players online", font, new SolidBrush(genericColor), new PointF(marginLeft, marginTop));
                                    marginLeft += (int)g.MeasureString("players online", font).Width + spacing * afterSpace;
                                }

                                if (data.FormatedRuntime != "00:00:00")
                                {
                                    g.DrawString(data.FormatedRuntime, font, new SolidBrush(onlineColor), new PointF(marginLeft, marginTop));
                                    marginLeft += (int)g.MeasureString(data.FormatedRuntime, font).Width + afterSpace;

                                    g.DrawString("uptime", font, new SolidBrush(genericColor), new PointF(marginLeft, marginTop));
                                    marginLeft += (int)g.MeasureString("uptime", font).Width + spacing * afterSpace;
                                }
                            }
                            else
                            {
                                g.DrawString("Offline", font, new SolidBrush(offlineColor), new PointF(marginLeft, marginTop));
                                marginLeft += (int)g.MeasureString("Offline", font).Width + spacing * afterSpace;
                            }

                            g.Flush();

                            if (File.Exists(outputFile))
                                File.Delete(outputFile);


                            b.Save(outputFile);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not create userbar: {0}", ex.Message);
            }
        }
    }
}
