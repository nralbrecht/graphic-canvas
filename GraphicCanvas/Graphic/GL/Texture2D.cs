using System;

using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.Drawing.Imaging;

namespace GraphicCanvas.Graphic
{
    class Texture2D : IDisposable
    {
        private int? id = null;
        private Bitmap texture;
        private Size lastSize;

        public int Id {
            get {
                if (id.HasValue) {
                    return id.Value;
                }
                else
                {
                    return 0;
                }
            }
        }
        public Bitmap Texture
        {
            get { return texture; }
            set { texture = value; }
        }
        public int Width { get { return texture.Width; } }
        public int Height { get { return texture.Height; } }
        public Graphics Graphics { get { return Graphics.FromImage(texture); } }

        public Texture2D(int width, int height)
            : this(new Bitmap(width, height))
        { }

        public Texture2D(Bitmap texture)
        {
            this.texture = texture;
        }

        public Texture2D(string filename)
        {
            texture = new Bitmap(filename);
        }

        public void Init()
        {
            id = GL.GenTexture();

            Bind();

            BitmapData bmpData = texture.LockBits(
                new Rectangle(0, 0, texture.Width, texture.Height),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0,
                PixelInternalFormat.Rgba,
                texture.Width, texture.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                PixelType.UnsignedByte,
                bmpData.Scan0);

            texture.UnlockBits(bmpData);

            GL.TexParameter(TextureTarget.Texture2D,
                TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D,
                TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            lastSize = texture.Size;
        }

        public void Reload()
        {
            Bind();

            BitmapData bmpData = texture.LockBits(
                new Rectangle(0, 0, texture.Width, texture.Height),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            
            if (texture.Size == lastSize)
            {
                GL.TexSubImage2D(TextureTarget.Texture2D, 0,
                    0, 0,
                    texture.Width, texture.Height,
                    OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                    PixelType.UnsignedByte,
                    bmpData.Scan0);
            }
            else
            {
                GL.TexImage2D(TextureTarget.Texture2D, 0,
                    0, 0,
                    texture.Width, texture.Height,
                    OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                    PixelType.UnsignedByte,
                    bmpData.Scan0);
            }

            texture.UnlockBits(bmpData);

            lastSize = texture.Size;
        }

        public void Bind()
        {
            if (!id.HasValue)
            {
                throw new Exception("The Texture2D has not bin inititialized. Please call 'Init();' bevor anything other");
            }

            GL.BindTexture(TextureTarget.Texture2D, id.Value);
        }

        public void Unbind()
        {
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public void Dispose()
        {
            texture.Dispose();
        }
    }
}
