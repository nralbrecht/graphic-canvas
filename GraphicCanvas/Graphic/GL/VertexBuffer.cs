using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace GraphicCanvas.Graphic
{
    class VertexBuffer
    {
        private int VBO;
        private int vertexCount;
        private PrimitiveType primitive;
        private BufferUsageHint hint;

        public VertexBuffer(Vertex[] verteces, PrimitiveType primitive, BufferUsageHint hint)
        {
            VBO = GL.GenBuffer();
            this.primitive = primitive;
            this.hint = hint;
            
            Update(verteces);
        }

        public void Update(Vertex[] verteces)
        {
            vertexCount = verteces.Length;

            Bind();
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(Vertex.SizeInBytes * vertexCount), verteces, hint);
        }

        public void Draw()
        {
            Bind();

            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.TextureCoordArray);
            GL.EnableClientState(ArrayCap.ColorArray);

            GL.VertexPointer(2, VertexPointerType.Float, Vertex.SizeInBytes, 0);
            GL.TexCoordPointer(2, TexCoordPointerType.Float, Vertex.SizeInBytes, Vector2.SizeInBytes);
            GL.ColorPointer(4, ColorPointerType.Float, Vertex.SizeInBytes, Vector2.SizeInBytes * 2);

            GL.DrawArrays(primitive, 0, vertexCount);
        }

        public void Bind()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
        }

        public void Unbind()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }
    }
}
