using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using System.Drawing;

namespace GraphicCanvas.Graphic
{
    struct Vertex
    {
        public Vector2 position;
        public Vector2 texCord;
        public Vector4 color;

        public static int SizeInBytes { get { return Vector2.SizeInBytes * 2 + Vector4.SizeInBytes; } }

        public Vertex(Vector2 position, Vector2 texCord, Vector4 color)
        {
            this.position = position;
            this.texCord = texCord;
            this.color = color;
        }

        public Vertex(Vector2 position, Vector2 texCord, Color color)
        {
            this.position = position;
            this.texCord = texCord;
            this.color = new Vector4(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
        }
    }
}
