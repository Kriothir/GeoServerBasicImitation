using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Globalization;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Web;

namespace HTTPSServer

{
    public partial class Form1 : Form
    {

        private Socket HTTP_Listener;
        private int port = 80;
        private Thread clientT;
        double x_pixel;
        double y_pixel;

        double x_starting;
        double y_starting;

        List<double> tfwInfo = new List<double>();


        string path = "";

        public Form1()
        {
            InitializeComponent();
        }

        private void startServerBtn_Click(object sender, EventArgs e)
        {
            serverLogsText.Text = "";

            try
            {
                HTTP_Listener = new Socket(SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    port = int.Parse(serverPortText.Text.ToString());

                 
                }
                catch(Exception ex)
                {
                    serverLogsText.Text = "Error while startng\n";
                }

                clientT = new Thread(new ThreadStart(this.connect));
                clientT.Start();

                startServerBtn.Enabled = false;
                stopServerBtn.Enabled = true;

            }
            catch(Exception ex)
            {
                Console.WriteLine("Error!");
            }

            serverLogsText.Text = "Server is online!";
        }

        private void ReadFile(string layer)
        {




        }
         void getFrom(string url)
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




            if (getLayer == "P100051")
            {
                Image toTrim = Image.FromFile(@"C:\Users\Admin\Desktop\Raster\PK1000_RS\P100051.TIF");
                foreach (string line in System.IO.File.ReadLines(@"C:\Users\Admin\Desktop\Raster\PK1000_RS\P100051.tfw"))
                {
                    tfwInfo.Add(double.Parse(line, CultureInfo.InvariantCulture));
                }
                Console.WriteLine("Here1");

                x_pixel = tfwInfo[0];
                y_pixel = tfwInfo[3];
                x_starting = tfwInfo[4];
                y_starting = tfwInfo[5];
                TrimImage(toTrim, listBBOX[0], listBBOX[1], listBBOX[2], listBBOX[3], resolution[0], resolution[1], getFormat);
            }
            if (getLayer == "P100052")
            {
                Image toTrim = Image.FromFile(@"C:\Users\Admin\Desktop\Raster\PK1000_RSI\P100052.TIF");

                foreach (string line in System.IO.File.ReadLines(@"C:\Users\Admin\Desktop\Raster\PK1000_RSI\P100052.tfw"))
                {
                    tfwInfo.Add(double.Parse(line, CultureInfo.InvariantCulture));

                }
                x_pixel = tfwInfo[0];
                y_pixel = tfwInfo[3];
                x_starting = tfwInfo[4];
                y_starting = tfwInfo[5];
                Console.WriteLine("Here2");

                TrimImage(toTrim, listBBOX[0], listBBOX[1], listBBOX[2], listBBOX[3], resolution[0], resolution[1], getFormat);

            }
            if (getLayer == "P100004")
            {
                Image toTrim = Image.FromFile(@"C:\Users\Admin\Desktop\Raster\PK1000_ZDR\P100004.TIF");

                foreach (string line in System.IO.File.ReadLines(@"C:\Users\Admin\Desktop\Raster\PK1000_ZDR\P100004.tfw"))
                {
                    tfwInfo.Add(double.Parse(line, CultureInfo.InvariantCulture));

                }
                x_pixel = tfwInfo[0];
                y_pixel = tfwInfo[3];
                x_starting = tfwInfo[4];
                y_starting = tfwInfo[5];
                Console.WriteLine("Here3");

                TrimImage(toTrim, listBBOX[0], listBBOX[1], listBBOX[2], listBBOX[3], resolution[0], resolution[1], getFormat);

            }


        }
        private String checkLayers()
        {
            String availableLayers = "";

            if(File.Exists(@"C:\Users\Admin\Desktop\Raster\PK1000_RS\P100051.tfw") == true && File.Exists(@"C:\Users\Admin\Desktop\Raster\PK1000_RS\P100051.TIF") == true)
            {
                availableLayers += "PK1000_RS ";
            }
            if (File.Exists(@"C:\Users\Admin\Desktop\Raster\PK1000_RSI\P100052.tfw") == true && File.Exists(@"C:\Users\Admin\Desktop\Raster\PK1000_RSI\P100052.TIF") == true)
            {
                availableLayers += "PK1000_RSI ";
            }
            if (File.Exists(@"C:\Users\Admin\Desktop\Raster\PK1000_ZDR\P100004.tfw") == true && File.Exists(@"C:\Users\Admin\Desktop\Raster\PK1000_ZDR\P100004.TIF") == true)
            {
                availableLayers += "PK1000_ZDR";
            }

            return availableLayers;

        }
        private void TrimImage(Image image, double x_start, double y_start, double x_end, double y_end, int width_crop, int height_crop, string format)
        {

            Bitmap bmpImage = new Bitmap(image);

            double start_pixel_x = (x_start - tfwInfo[4]) / tfwInfo[0];
            double end_pixel_x = (x_end - tfwInfo[4]) / tfwInfo[0];

            double start_pixel_y = (y_start - tfwInfo[5]) / tfwInfo[0];
            double end_pixel_y = (y_end - tfwInfo[5]) / tfwInfo[0];

            double width = end_pixel_x - start_pixel_x;
            double height = end_pixel_y - start_pixel_y;


            Rectangle bbox = new Rectangle((int)start_pixel_x, (int)start_pixel_y, (int)width, (int)height);
            Image newImage = bmpImage.Clone(bbox, bmpImage.PixelFormat);

            //bmpImage.Save(@"C:\Users\Admin\Desktop\Raster\PK1000_ZDR\P1000222.png", ImageFormat.Png);

            ResizeImage(newImage, width_crop, height_crop, format);

        }
        private void ResizeImage(Image image, int width, int height, string format) // Povzeto iz stackoverflowa
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

            if(format == "png")
            {
                destImage.Save(@"C:\Users\Admin\Desktop\Vaja1\Response.png", ImageFormat.Png);
                path = @"C:\Users\Admin\Desktop\Vaja1\Response.png";
            }
            else if(format == "gif")
            {
                destImage.Save(@"C:\Users\Admin\Desktop\Vaja1\Response.gif", ImageFormat.Gif);
                path = @"C:\Users\Admin\Desktop\Vaja1\Response.gif";

            }
            else if(format == "jpeg")
            {
                destImage.Save(@"C:\Users\Admin\Desktop\Vaja1\Response.jpeg", ImageFormat.Jpeg);
                path = @"C:\Users\Admin\Desktop\Vaja1\Response.jpeg";

            }
            Console.WriteLine("Done!");
            //return destImage;
        }

        private void stopServerBtn_Click(object sender, EventArgs e)
        {
            try
            {
                
                HTTP_Listener.Close();

                clientT.Abort();

                startServerBtn.Enabled = true;
                stopServerBtn.Enabled = false;
            }
            catch(Exception ex)
            {
                Console.WriteLine("Stopping Failed");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            stopServerBtn.Enabled = false;
        }

        private void connect()
        {
            try
            {
                IPEndPoint ep = new IPEndPoint(IPAddress.Any, port);
                HTTP_Listener.Bind(ep);
                HTTP_Listener.Listen(1);
                StartServer();
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error while binding!");
            }
        }

        private void StartServer()
        {
            while (true)
            {

                String layers = "";
                String request = "";

                byte[] bytes = new byte[2048];

                Socket clientSocket = HTTP_Listener.Accept(); // Blocking Statement

                // Reading the inbound connection data
                while (true)
                {
                    int numb = clientSocket.Receive(bytes);
                    request += Encoding.ASCII.GetString(bytes, 0, numb);

                    if (request.IndexOf("\r\n") > -1)
                    {
                        break;

                    }
                }
                int counter = 0;
                // Data Read

                serverLogsText.Invoke((MethodInvoker)delegate {
                    serverLogsText.Text += "\r\n\r\n";

                    serverLogsText.Text += request;

                    serverLogsText.Text += "\n\n------ End of Request -------";
                });

                if (request.Contains("=GetCapabilities"))
                {
                    counter++;

                    layers = checkLayers();
                    String resHeader = "HTTP/1.1 200 OK\nServer: GeoServer\nContent-Type: text/html; charset: UTF-8\n\n";
               
                    String resStr = resHeader + layers;

                    byte[] resData = Encoding.ASCII.GetBytes(resStr);

                    clientSocket.SendTo(resData, clientSocket.RemoteEndPoint);

                    clientSocket.Close();

                }
                else if (request.Contains("=GetMap"))
                {
                    //layers = checkLayers();
                    getFrom(request);
                    String resHeader = "HTTP/1.1 200 Everything is Fine\nServer: my_csharp_server\nContent-Type: text/html; charset: UTF-8\n\n";
                    byte[] imgdata = File.ReadAllBytes(path);
                    

                    var headers = Encoding.ASCII.GetBytes(resHeader);
                    clientSocket.Send(headers);
                    clientSocket.Send(imgdata);
                    //

                    //byte[] resData = Encoding.ASCII.GetBytes(resHeader) + imgdata;

                    //client.SendTo(resData, client.RemoteEndPoint);

                    clientSocket.Close();
                }

                else
                {
                    counter++;

                    layers = checkLayers();
                    String resHeader = "HTTP/1.1 200 Everything is Fine\nServer: my_csharp_server\nContent-Type: text/html; charset: UTF-8\n\n";
                    String resBody = "<!DOCTYE html> " +
                        "<html>" +
                        "<head><title>My Server</title>" + "<link rel=\"icon\" href =\"data:,\" > " +
                        "</head>" +
                        "<body>" +
                        "<h4> " +
                        "<form action=\"http://localhost:1337/geoserver/gis/wms?\" method =\"GET\" > " +
                        "<div>" +
                        "<label for=\"say\" > What greeting do you want to say?</label>" +
                        "<input type=\"text\" id =\"BBOX\" name =\"BBOX\" > " +
                       "<input type=\"text\" id =\"STYLES\" name =\"STYLES\" > " +
                       "<input type=\"text\" id =\"FORMAT\" name =\"FORMAT\" value=\"image/\" > " +
                       "<input type=\"radio\" id =\"REQUESTmap\" name =\"GetMap\" > " +
                        " <label for=\"REQUESTmap\">GetMap</label><br>" +

                        "<input type=\"radio\" id =\"REQUESTcap\" name =\"GetCapabilities\" > " +
                        " <label for=\"REQUESTcap\">GetCapabilities</label><br>" +

                       "<input type=\"text\" id =\"VERSION\" name =\"VERSION\" value= \"1.1.1\" > " +
                       "<input type=\"text\" id =\"LAYERS\" name =\"LAYERS\" > " +
                       "<input type=\"text\" id =\"WIDTH\" name =\"WIDTH\" > " +
                       "<input type=\"text\" id =\"HEIGHT\" name =\"HEIGHT\" > " +
                       "<input type=\"text\" id =\"SRS\" name =\"SRS\" value=\"XXX\" > " +

                       " </div><div> " +
                       "    <button>Send my greetings</button></ div ></ form > " +
                        "<p> Please follow <a href =\"http://localhost:1337/GetCapabilities \"> this link </a>.</p>" + request +
                        "</h4>" +
                        "<img src=\"https://www.google.com/url?sa=i&url=https%3A%2F%2Fhelpx.adobe.com%2Fsi%2Fphotoshop%2Fusing%2Fconvert-color-image-black-white.html&psig=AOvVaw1aWH5pwJmTKm5Kdz4oS1dI&ust=1649202454579000&source=images&cd=vfe&ved=0CAoQjRxqFwoTCJiBxoHM-_YCFQAAAAAdAAAAABAD \" />" +
                    "</body></html>";
                    String resStr = resHeader + resBody;

                    byte[] resData = Encoding.ASCII.GetBytes(resStr);

                    clientSocket.SendTo(resData, clientSocket.RemoteEndPoint);

                    clientSocket.Close();
                    counter++;
                }
                // Send back the Response
                

               
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
