using System;

using System.Drawing;
using System.Drawing.Imaging;

namespace GraphicCanvas.Graphic
{
    class Canvas : IDisposable
    {
        #region Variables
        
        private Texture2D canvas;
        private bool hasChanged = false;

        public int Width { get { return canvas.Width; } }
        public int Height { get { return canvas.Height; } }

        #endregion

        #region Constructors

        public Canvas(int width, int height)
            : this(new Bitmap(width, height))
        { }

        public Canvas(Bitmap bitmap)
        {
            this.canvas = new Texture2D(bitmap);
        }

        #endregion

        #region Controlls

        public void Init()
        {
            canvas.Init();
        }

        public void Reload(bool ignoreChange = false)
        {
            if (ignoreChange) 
            {
                canvas.Reload();
                return;
            }

            if (hasChanged)
            {
                canvas.Reload();
                hasChanged = false;
            }
        }

        public void Bind()
        {
            canvas.Bind();
        }

        public void Unbind()
        {
            canvas.Unbind();
        }

        public void Dispose()
        {
            canvas.Dispose();
        }

        #endregion

        #region Instructions

        public void Save(string filepath)
        {
            canvas.Texture.Save(filepath, ImageFormat.Bmp);

#if DEBUG
            Console.WriteLine("Saved to: " + filepath);
            System.Diagnostics.Process.Start(filepath);
#endif
        }

        public void Fill(Color color)
        {
            canvas.Graphics.Clear(color);
            hasChanged = true;
        }

        public void DrawPixel(int x, int y, Color color)
        {
            canvas.Texture.SetPixel(x, y, color);
            hasChanged = true;
        }

        public void DrawRect(int x, int y, int w, int h, Color color)
        {
            canvas.Graphics.FillRectangle(new SolidBrush(color), x, y, w, h);
            hasChanged = true;
        }

        public void DrawLine(int x1, int y1, int x2, int y2, int width, Color color)
        {
            canvas.Graphics.DrawLine(new Pen(color, width), x1, y1, x2, y2);
            hasChanged = true;
        }

        public void DrawCircle(int x, int y, int r, Color color)
        {
            canvas.Graphics.FillEllipse(new SolidBrush(color), new Rectangle(x - r, y - r, r * 2, r * 2));
            hasChanged = true;
        }

        public Color GetPixel(int x, int y)
        {
            return canvas.Texture.GetPixel(x, y);
        }

        public void Write(float x, float y, int size, string text, Color color)
        {
            canvas.Graphics.DrawString(text, new Font(FontFamily.GenericMonospace, size), new SolidBrush(color), x, y);
            hasChanged = true;
        }

        public void SetWidth(int width)
        {
            Bitmap nb = new Bitmap(width, canvas.Height);
            Graphics.FromImage(nb).DrawImage(canvas.Texture, 0, 0);
            canvas.Texture = nb;
            hasChanged = true;
        }

        public void SetHeight(int height)
        {
            Size sc = canvas.Texture.Size;
            Bitmap nb = new Bitmap(sc.Width, height);

            Graphics g = Graphics.FromImage(nb);
            g.Clear(Color.SteelBlue);
            canvas.Texture = nb;
            hasChanged = true;
        }

        #endregion

        public override int GetHashCode()
        {
            return canvas.GetHashCode();
        }
    }
}
