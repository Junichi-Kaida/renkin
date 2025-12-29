using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0) return;
        string path = args[0];
        
        using (Bitmap raw = new Bitmap(path))
        using (Bitmap bmp = new Bitmap(raw.Width, raw.Height))
        {
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.DrawImage(raw, 0, 0);
            }

            // 1. Flood Fill Transparency (Top-Left 0,0)
            Color targetColor = bmp.GetPixel(0, 0);
            if (IsTarget(targetColor))
            {
                Queue<Point> q = new Queue<Point>();
                q.Enqueue(new Point(0, 0));
                bmp.SetPixel(0, 0, Color.Transparent);
                
                while(q.Count > 0)
                {
                    Point p = q.Dequeue();
                    int[] dx = {1, -1, 0, 0};
                    int[] dy = {0, 0, 1, -1};
                    
                    for(int i=0; i<4; i++)
                    {
                        int nx = p.X + dx[i];
                        int ny = p.Y + dy[i];
                        
                        if (nx >= 0 && nx < bmp.Width && ny >= 0 && ny < bmp.Height)
                        {
                            Color c = bmp.GetPixel(nx, ny);
                            if (c.A != 0 && IsTarget(c))
                            {
                                bmp.SetPixel(nx, ny, Color.Transparent);
                                q.Enqueue(new Point(nx, ny));
                            }
                        }
                    }
                }
            }

            // 2. Auto Crop
            // Find bounds
            int minX = bmp.Width, maxX = 0, minY = bmp.Height, maxY = 0;
            bool found = false;

            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    if (bmp.GetPixel(x, y).A != 0)
                    {
                        if (x < minX) minX = x;
                        if (x > maxX) maxX = x;
                        if (y < minY) minY = y;
                        if (y > maxY) maxY = y;
                        found = true;
                    }
                }
            }

            if (found)
            {
                int width = maxX - minX + 1;
                int height = maxY - minY + 1;
                
                using (Bitmap cropped = new Bitmap(width, height))
                using (Graphics gCrop = Graphics.FromImage(cropped))
                {
                    gCrop.DrawImage(bmp, new Rectangle(0, 0, width, height), new Rectangle(minX, minY, width, height), GraphicsUnit.Pixel);
                    cropped.Save(path + ".tmp.png", ImageFormat.Png);
                    Console.WriteLine("Cropped " + path + " to " + width + "x" + height);
                }
            }
            else
            {
                // Empty image? Just save as is (or fully transparent)
                bmp.Save(path + ".tmp.png", ImageFormat.Png);
                Console.WriteLine("Image empty or full transparent: " + path);
            }
        }
        
        // Replace original
        System.IO.File.Delete(path);
        System.IO.File.Move(path + ".tmp.png", path);
    }
    
    static bool IsTarget(Color c)
    {
        // Treat white-ish as background
        return c.R > 240 && c.G > 240 && c.B > 240;
    }
}
