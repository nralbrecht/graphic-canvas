using System;

using System.Drawing;
using GraphicCanvas.Graphic;

namespace GraphicCanvas.VM
{
    class Runtime : IDisposable
    {
        private Canvas canvas;
        private CanvasWindow window;
        private PerlinNoise perlinNoise;
        private SecureRandom rng;

        public Runtime(CanvasWindow window, Canvas canvas, int noiseSeed)
        {
            this.window = window;
            this.canvas = canvas;

            perlinNoise = new PerlinNoise(noiseSeed);
            rng = new SecureRandom();
        }

        #region Canvas

        public int CanvasHeight { get { return canvas.Height; } }
        public int CanvasWidth { get { return canvas.Width; } }

        public void Fill(Color color)
        {
            canvas.Fill(color);
        }

        public void DrawLine(int x1, int y1, int x2, int y2, int width, Color color)
        {
            canvas.DrawLine(x1, y1, x2, y2, width, color);
        }

        public void DrawRect(int x, int y, int w, int h, Color color)
        {
            canvas.DrawRect(x, y, w, h, color);
        }

        public void DrawPixel(int x, int y, Color color)
        {
            canvas.DrawPixel(x, y, color);
        }

        public void DrawCircle(int x, int y, int r, Color color)
        {
            canvas.DrawCircle(x, y, r, color);
        }

        public Color GetPixel(int x, int y)
        {
            return canvas.GetPixel(x, y);
        }

        public void Save(string filepath)
        {
            canvas.Save(filepath);
        }

        public void Write(float x, float y, int size, string text, Color color)
        {
            canvas.Write(x, y, size, text, color);
        }

        public void Reload()
        {
            window.Message(WindowMessage.RELOAD);
        }

        #endregion

        #region CanvasWindow
        
        public void Resize()
        {
            window.Message(WindowMessage.RESIZE);
        }

        #endregion

        #region PerlinNoise

        public int Noise(int x, int y)
        {
            double widthDivisor = 1 / (double)512;
            double heightDivisor = 1 / (double)512;

            double v =
                (perlinNoise.Noise(2 * x * widthDivisor, 2 * y * heightDivisor, -0.5) + 1) / 2 * 0.7 +
                (perlinNoise.Noise(4 * x * widthDivisor, 4 * y * heightDivisor, 0) + 1) / 2 * 0.2 +
                (perlinNoise.Noise(8 * x * widthDivisor, 8 * y * heightDivisor, +0.5) + 1) / 2 * 0.1;

            v = Math.Min(1, Math.Max(0, v));
            byte b = (byte)(v * 255);
            
            return b;
        }

        #endregion

        #region Random

        public int Random(int min, int max)
        {
            return rng.Next(min, max);
        }

        #endregion

        public void Dispose()
        {
            rng.Dispose();
        }
    }
}
