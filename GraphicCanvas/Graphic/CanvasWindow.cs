using System;
using System.Collections.Generic;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using OpenTK.Input;

namespace GraphicCanvas.Graphic
{
    class CanvasWindow : IDisposable
    {
        private GameWindow window;
        private Canvas canvas;
        private float zoomLevel = 1;
        private Queue<WindowMessage> messages;
        private Vector3 focusPoint = Vector3.Zero;
        private IndexVertexBuffer textureCanvas;
#if DEBUG
        private VertexBuffer crossHair;
#endif
        public event EventHandler Load;

        #region Constructors

        public CanvasWindow(string title)
            : this(512, 512, title) { }

        public CanvasWindow(int width, int height, string title)
            : this(new Bitmap(width, height), title) { }

        public CanvasWindow(Bitmap bitmap, string title)
            : this(new Canvas(bitmap), title)
        { }

        public CanvasWindow(Canvas canvas, string title)
        {
            this.canvas = canvas;
            messages = new Queue<WindowMessage>();
            window = new GameWindow(canvas.Width, canvas.Height, GraphicsMode.Default, title, GameWindowFlags.FixedWindow, DisplayDevice.Default, 3, 1, GraphicsContextFlags.ForwardCompatible);
            textureCanvas = new IndexVertexBuffer(new Vertex[] {

                new Vertex(new Vector2(0, 0),                        new Vector2(1, 1), Color.White),
                new Vertex(new Vector2(window.Width, 0),             new Vector2(0, 1), Color.White),
                new Vertex(new Vector2(window.Width, window.Height), new Vector2(0, 0), Color.White),
                new Vertex(new Vector2(0, window.Height),            new Vector2(1, 0), Color.White)

            }, new uint[] {

                0, 1, 2,
                0, 2, 3

            }, PrimitiveType.Triangles, BufferUsageHint.DynamicDraw);

#if DEBUG
            crossHair = new VertexBuffer(new Vertex[]
            {
                new Vertex(new Vector2(focusPoint.X - 5, focusPoint.Y), new Vector2(0, 0), Color.Blue),
                new Vertex(new Vector2(focusPoint.X + 5, focusPoint.Y), new Vector2(0, 0), Color.Blue),
                new Vertex(new Vector2(focusPoint.X, focusPoint.Y + 5), new Vector2(0, 0), Color.Blue),
                new Vertex(new Vector2(focusPoint.X, focusPoint.Y - 5), new Vector2(0, 0), Color.Blue)
            }, PrimitiveType.Lines, BufferUsageHint.DynamicDraw);
#endif

            window.Load += OnLoad;
            window.UpdateFrame += OnUpdate;
            window.RenderFrame += OnRender;
            window.MouseWheel += OnScroll;
            window.Resize += OnResize;
            window.KeyUp += OnKeyUp;
        }

        #endregion

        #region Window Events

        private void OnKeyUp(object sender, KeyboardKeyEventArgs e)
        {
            if (e.Alt && e.Key == Key.F4)
            {
                window.Close();
            }
            if (e.Key == Key.S && e.Control)
            {
                canvas.Save(DateTime.Now.ToString("yyyyMMdd-Hmmss") + ".bmp");
            }
            if (e.Control && e.Key == Key.R)
            {
                canvas.Reload(true);
            }
            if (e.Key == Key.T)
            {
                canvas.DrawRect((int)focusPoint.X - 5, (int)focusPoint.Y - 5, 10, 10, Color.Red);
                canvas.Reload();
            }
        }
        
        private void OnScroll(object sender, MouseWheelEventArgs e)
        {
            float nextZoomLevel = zoomLevel - e.Delta * 0.01f;

            if (nextZoomLevel > 0.01 && nextZoomLevel <= 1)
            {
                zoomLevel = nextZoomLevel;
            }

#if DEBUG
            Console.WriteLine("zoom: " + zoomLevel);
#endif
        }

        private void OnRender(object sender, FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            Matrix4 viewMatrix = Matrix4.CreateTranslation(-focusPoint)
                * Matrix4.CreateScale(zoomLevel).Inverted()
                * Matrix4.CreateTranslation(focusPoint);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref viewMatrix);

            Matrix4 projMatrix = Matrix4.CreateOrthographicOffCenter(0, window.Width, window.Height, 0, 0, 1);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref projMatrix);

            canvas.Bind();
            textureCanvas.Draw();

#if DEBUG
            canvas.Unbind();
            crossHair.Draw();
#endif
            window.SwapBuffers();
            
            
        }

        private void OnUpdate(object sender, FrameEventArgs e)
        {
            #region Focus Point

            KeyboardState state = Keyboard.GetState();
            float speed = 10 * zoomLevel;

            if (state.IsKeyDown(Key.Left))
            {
                focusPoint.X -= speed;
            }
            if (state.IsKeyDown(Key.Right))
            {
                focusPoint.X += speed;
            }
            if (state.IsKeyDown(Key.Up))
            {
                focusPoint.Y -= speed;
            }
            if (state.IsKeyDown(Key.Down))
            {
                focusPoint.Y += speed;
            }

#if DEBUG
            crossHair.Update(new Vertex[]
            {
                new Vertex(new Vector2(focusPoint.X - 5, focusPoint.Y), new Vector2(0, 0), Color.Blue),
                new Vertex(new Vector2(focusPoint.X + 5, focusPoint.Y), new Vector2(0, 0), Color.Blue),
                new Vertex(new Vector2(focusPoint.X, focusPoint.Y + 5), new Vector2(0, 0), Color.Blue),
                new Vertex(new Vector2(focusPoint.X, focusPoint.Y - 5), new Vector2(0, 0), Color.Blue)
            });
#endif
            #endregion

            while (messages.Count > 0)
            {
                switch (messages.Dequeue())
                {
                    case WindowMessage.RELOAD:
                        canvas.Reload();
                        break;
                    case WindowMessage.RESIZE:
                        Resize();
                        break;
                    default:
                        break;
                }
            }
        }

        private void OnResize(object sender, EventArgs e)
        {
            textureCanvas.Update(new Vertex[] {

                new Vertex(new Vector2(0, 0),                        new Vector2(0, 0), Color.White),
                new Vertex(new Vector2(window.Width, 0),             new Vector2(1, 0), Color.White),
                new Vertex(new Vector2(window.Width, window.Height), new Vector2(1, 1), Color.White),
                new Vertex(new Vector2(0, window.Height),            new Vector2(0, 1), Color.White)

            }, new uint[] {

                0, 1, 2,
                0, 2, 3

            });
        }

        private void OnLoad(object sender, EventArgs e)
        {
            GL.Enable(EnableCap.Texture2D);
            canvas.Init();

            focusPoint.X = window.Width / 2;
            focusPoint.Y = window.Height / 2;

            Resize();
#if DEBUG
            Console.WriteLine("loaded!");
#endif
            Load?.Invoke(this, new EventArgs());
        }

        #endregion

        #region Controlls

        public void Run(double updateRate)
        {
            window.Run(16, updateRate);
        }

        public void Resize()
        {
            window.Width = canvas.Width;
            window.Height = canvas.Height;
            GL.Viewport(window.ClientRectangle);
            window.X = (DisplayDevice.Default.Width / 2) - window.Size.Width / 2;
            window.Y = (DisplayDevice.Default.Height / 2) - window.Size.Height / 2;
        }

        public void Close()
        {
            window.Close();
        }

        public void Dispose()
        {
            window.Dispose();
        }

        public void Message(WindowMessage message)
        {
            messages.Enqueue(message);
        }

        #endregion
    }
}
