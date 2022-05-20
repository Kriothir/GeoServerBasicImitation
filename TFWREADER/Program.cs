using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Globalization;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace TFWREADER
{
    class Program
    {
        static double x_pixel;
        static double y_pixel;

        static double x_starting;
        static double y_starting;

        static List<double> tfwInfo = new List<double>();


        static string path = "";


        static string CreateURL(string port, string minx, string miny, string maxx, string maxy, string format, string layer, string width, string height)
        {
            string url = "http://localhost:"+ port +"/geoserver/gis/wms?BBOX=" + minx + "," + miny + "," + maxx + "," + maxy + "&STYLES=&FORMAT=image/" + format + "&REQUEST=GetMap&VERSION=1.1.1&LAYERS=gis:" + layer + "&WIDTH=" + width + "&HEIGHT="
                + height + "&SRS=XXX";

            return url;
        }
        static  void TrimImage(Image image, double x_start, double x_end, double y_start, double y_end, int width_crop, int height_crop, string format)
        {

            Bitmap bmpImage = new Bitmap(image);

            double start_pixel_x = (x_start - tfwInfo[4]) / tfwInfo[0];
            double end_pixel_x = (x_end - tfwInfo[4]) / tfwInfo[0];

            double start_pixel_y = (y_start - tfwInfo[5]) / tfwInfo[0];
            double end_pixel_y = (y_end - tfwInfo[5]) / tfwInfo[0];

            double width = end_pixel_x - start_pixel_x;
            double height = end_pixel_y - start_pixel_y;


            Rectangle bbox = new Rectangle((int)start_pixel_x, (int)start_pixel_y, (int)width, (int)height);
            bmpImage.Clone(bbox, bmpImage.PixelFormat);

            //bmpImage.Save(@"C:\Users\Admin\Desktop\Raster\PK1000_ZDR\P1000222.png", ImageFormat.Png);

            ResizeImage(bmpImage, width_crop, height_crop, format);

        }
        static  void ResizeImage(Image image, int width, int height, string format)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var imageProperties = Graphics.FromImage(destImage))
            {
                imageProperties.CompositingMode = CompositingMode.SourceCopy;
                imageProperties.CompositingQuality = CompositingQuality.HighQuality;
                imageProperties.InterpolationMode = InterpolationMode.HighQualityBicubic;
                imageProperties.SmoothingMode = SmoothingMode.HighQuality;
                imageProperties.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    imageProperties.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            if (format == "png")
            {
                destImage.Save(@"C:\Users\Admin\Desktop\Vaja1\Response.png", ImageFormat.Png);
                path = @"C:\Users\Admin\Desktop\Vaja1\Response.png";
            }
            else if (format == "gif")
            {
                destImage.Save(@"C:\Users\Admin\Desktop\Vaja1\Response.gif", ImageFormat.Gif);
                path = @"C:\Users\Admin\Desktop\Vaja1\Response.gif";

            }
            else if (format == "jpeg")
            {
                destImage.Save(@"C:\Users\Admin\Desktop\Vaja1\Response.jpeg", ImageFormat.Jpeg);
                path = @"C:\Users\Admin\Desktop\Vaja1\Response.jpeg";

            }
            Console.WriteLine("Done!");
            //return destImage;
        }
        static void  getFrom(string url)
        {
            List<double> listBBOX = new List<double>();
            string getFormat = "";
            string bbox_nums = "";
            string getLayer = "";
            string getWidth = "";
            string getHeight = "";

            List<int> resolution = new List<int>();

            int bbox_Start = url.IndexOf("BBOX=");
            for (int i = bbox_Start + 5; i < url.Length; i++)
            {
                if (url[i] == '&')
                {

                    listBBOX.Add(double.Parse(bbox_nums, CultureInfo.InvariantCulture));

                    break;
                }
                if (url[i] == ',')
                {
                    listBBOX.Add(double.Parse(bbox_nums, CultureInfo.InvariantCulture));
                    bbox_nums = "";
                    i++;
                }
                bbox_nums += url[i];

            }
            for (int k = url.IndexOf("image/") + 6; k < url.Length; k++)
            {
                if (url[k] == '&')
                {
                    break;
                }
                getFormat += url[k];
            }
            for (int j = url.IndexOf("gis:") + 4; j < url.Length; j++)
            {

                if (url[j] == '&')
                {
                    break;
                }
                getLayer += url[j];
            }
            for (int j = url.IndexOf("WIDTH=") + 6; j < url.Length; j++)
            {
                if (url[j] == '&')
                {
                    resolution.Add(int.Parse(getWidth, CultureInfo.InvariantCulture));

                    break;
                }
                getWidth += url[j];
            }
            for (int j = url.IndexOf("HEIGHT=") + 7; j < url.Length; j++)
            {
                if (url[j] == '&')
                {
                    resolution.Add(int.Parse(getHeight, CultureInfo.InvariantCulture));

                    break;
                }
                getHeight += url[j];
            }

            Console.WriteLine(getLayer);
            Console.WriteLine(getFormat);

            foreach (var month in resolution)
            {
                Console.WriteLine(month);
            }
            foreach (var month in listBBOX)
            {
                Console.WriteLine(month);
            }


            x_pixel = tfwInfo[0];
            y_pixel = tfwInfo[3];
            x_starting = tfwInfo[4];
            y_starting = tfwInfo[5];

            if (getLayer == "P100051")
            {
                Image toTrim = Image.FromFile(@"C:\Users\Admin\Desktop\Raster\PK1000_RS\P100051.TIF");
                foreach (string line in System.IO.File.ReadLines(@"C:\Users\Admin\Desktop\Raster\PK1000_RS\P100051.tfw"))
                {
                    tfwInfo.Add(double.Parse(line, CultureInfo.InvariantCulture));
                }
                TrimImage(toTrim, listBBOX[0], listBBOX[1], listBBOX[2], listBBOX[3], resolution[0], resolution[1], getFormat);
            }
            if (getLayer == "P100052")
            {
                Image toTrim = Image.FromFile(@"C:\Users\Admin\Desktop\Raster\PK1000_RS\P100052.TIF");

                foreach (string line in System.IO.File.ReadLines(@"C:\Users\Admin\Desktop\Raster\PK1000_RSI\P100052.tfw"))
                {
                    tfwInfo.Add(double.Parse(line, CultureInfo.InvariantCulture));

                }
                TrimImage(toTrim, listBBOX[0], listBBOX[1], listBBOX[2], listBBOX[3], resolution[0], resolution[1], getFormat);

            }
            if (getLayer == "P100004")
            {
                Image toTrim = Image.FromFile(@"C:\Users\Admin\Desktop\Raster\PK1000_RS\P100004.TIF");

                foreach (string line in System.IO.File.ReadLines(@"C:\Users\Admin\Desktop\Raster\PK1000_ZDR\P100004.tfw"))
                {
                    tfwInfo.Add(double.Parse(line, CultureInfo.InvariantCulture));

                }
                TrimImage(toTrim, listBBOX[0], listBBOX[1], listBBOX[2], listBBOX[3], resolution[0], resolution[1], getFormat);

            }

        }
        static async Task Main(string[] args)
        {
           // string test = getFrom("GET /geoserver/gis/wms?BBOX=100.11,1000.33,1000.33434,10000.1213&STYLES=&FORMAT=image/png&REQUEST=GetMap&VERSION=1.1.1&LAYERS=gis:P100004&WIDTH=400&HEIGHT=300&SRS=XXX HTTP/1.1");

            Console.WriteLine("Enter GetMap or GetCapabilities  (m or c):");
            string command = Console.ReadLine();
            if (command == "c")
            {
                Uri dick = new Uri("http://localhost:1337/geoserver/ows?service=wms&version=1.1.1&request=GetCapabilities");
                WebRequest createURL = WebRequest.Create(dick);
                HttpWebResponse getResponse = (HttpWebResponse)createURL.GetResponse();
                Stream recieveCapabilities = getResponse.GetResponseStream();
                StreamReader getData = new StreamReader(recieveCapabilities);

                Console.WriteLine(getData.ReadToEnd());

            }
            else
            {
                Console.WriteLine("Enter port:");
                string port = Console.ReadLine();
                Console.WriteLine("Enter MinX:");
                string minx = Console.ReadLine();
                Console.WriteLine("Enter MinY:");
                string miny = Console.ReadLine();
                Console.WriteLine("Enter MaxX:");
                string maxx = Console.ReadLine();
                Console.WriteLine("Enter MaxY:");
                string maxy = Console.ReadLine();
                Console.WriteLine("Enter format:");
                string format = Console.ReadLine();
                Console.WriteLine("Enter layer:");
                string layer = Console.ReadLine();
                Console.WriteLine("Enter width:");
                string width = Console.ReadLine();
                Console.WriteLine("Enter height:");
                string height = Console.ReadLine();

                Uri dick = new Uri(CreateURL(port, minx, miny, maxx, maxy, format, layer, width, height));
                WebRequest createURL = WebRequest.Create(dick);
                HttpWebResponse getResponse = (HttpWebResponse)createURL.GetResponse();
                Stream recieveImage = getResponse.GetResponseStream();
                StreamReader getData = new StreamReader(recieveImage);

                Bitmap newimage = new Bitmap(recieveImage);



                newimage.Save(@"C:\Users\Admin\Desktop\Vaja1\recieved." + format);
                

            }


            Console.ReadLine();
        }
    }
}
