using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ComputerCraftImages
{
    public static class HttpHandler
    {
        public static Task OnHttpRequest(Microsoft.AspNetCore.Http.HttpContext e)
        {
            try
            {
                //Get the image width and height
                int width = int.Parse(e.Request.Query["width"]);
                int height = int.Parse(e.Request.Query["height"]);

                //Request the image
                byte[] imageData;
                using (WebClient wc = new WebClient())
                    imageData = wc.DownloadData(Program.imgUrl);

                //Open image
                Image<Rgba32> img = Image.Load<Rgba32>(imageData);
                img.Mutate(x => x.Resize(width, height));

                //Convert to string
                string data = "";
                bool s = true;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        /*int i = 2;
                        if (s)
                            i = 1;
                        i += y;
                        data += (i%9).ToString();
                        s = !s;*/

                        data += FindNearestColor(img[x, y]);
                    }
                }

                Console.WriteLine("Got request "+e.Request.QueryString);

                //Write
                return Program.QuickWriteToDoc(e, data);
            } catch
            {
                return Program.QuickWriteToDoc(e, "failed", "text/plain", 500);
            }
        }

        static readonly Dictionary<string, Rgba32> colors = new Dictionary<string, Rgba32>
        {
            {"0", Rgba32.FromHex("F0F0F0") },
            {"1", Rgba32.FromHex("F2B233") },
            {"2", Rgba32.FromHex("E57FD8") },
            {"3", Rgba32.FromHex("99B2F2") },
            {"4", Rgba32.FromHex("DEDE6C") },
            {"5", Rgba32.FromHex("7FCC19") },
            {"6", Rgba32.FromHex("F2B2CC") },
            {"7", Rgba32.FromHex("4C4C4C") },
            {"8", Rgba32.FromHex("999999") },
            {"9", Rgba32.FromHex("4C99B2") },
            {"a", Rgba32.FromHex("B266E5") },
            {"b", Rgba32.FromHex("3366CC") },
            {"c", Rgba32.FromHex("7F664C") },
            {"d", Rgba32.FromHex("57A64E") },
            {"e", Rgba32.FromHex("CC4C4C") },
            {"f", Rgba32.FromHex("191919") },
        };

        static string FindNearestColor(Rgba32 c)
        {
            //Find how close it is to all colors first.
            List<string> keys = new List<string>();
            List<double> similarities = new List<double>();
            foreach(var cc in colors)
            {
                keys.Add(cc.Key);
                //Console.WriteLine(CompareColors(c, cc.Value));
                similarities.Add(CompareColors(c, cc.Value));
            }

            //Find maximum
            string maxKey = "0";
            double maxSimilaritiy = -1;
            for(int i = 0; i<similarities.Count; i++)
            {
                if (similarities[i] > maxSimilaritiy)
                {
                    maxKey = keys[i];
                    maxSimilaritiy = similarities[i];
                }
            }

            //We now have the max key.
            return maxKey;
        }

        static double CompareColors(Rgba32 a, Rgba32 b)
        {
            //https://stackoverflow.com/questions/3968179/compare-rgb-colors-in-c-sharp
            return (
            1.0 - ((double)(
                Math.Abs(a.R - b.R) +
                Math.Abs(a.G - b.G) +
                Math.Abs(a.B - b.B)
            ) / (256.0 * 3))
        );
        }
    }
}
