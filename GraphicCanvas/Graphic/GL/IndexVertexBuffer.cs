using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace GraphicCanvas.Graphic
{
    class IndexVertexBuffer
    {
        private int VBO;
        private int IBO;
        private int vertexCount;
        private int indexCount;
        private PrimitiveType primitive;
        private BufferUsageHint hint;
        
        public IndexVertexBuffer(Vertex[] verteces, uint[] indeces, PrimitiveType primitive, BufferUsageHint hint)
        {
            VBO = GL.GenBuffer();
            IBO = GL.GenBuffer();
            this.primitive = primitive;
            this.hint = hint;

            Update(verteces, indeces);
        }
        
        public void Update(Vertex[] verteces, uint[] indeces)
        {
            vertexCount = verteces.Length;
            indexCount = indeces.Length;

            Bind();
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(Vertex.SizeInBytes * vertexCount), verteces, hint);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(sizeof(uint) * indexCount), indeces, hint);
        }

        public void Draw()
        {
            Bind();

            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.TextureCoordArray);
            GL.EnableClientState(ArrayCap.ColorArray);
            GL.EnableClientState(ArrayCap.IndexArray);

            GL.VertexPointer(2, VertexPointerType.Float, Vertex.SizeInBytes, 0);
            GL.TexCoordPointer(2, TexCoordPointerType.Float, Vertex.SizeInBytes, Vector2.SizeInBytes);
            GL.ColorPointer(4, ColorPointerType.Float, Vertex.SizeInBytes, Vector2.SizeInBytes * 2);
            
            GL.DrawElements(primitive, indexCount, DrawElementsType.UnsignedInt, 0);
        }

        public void Bind()
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, IBO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
        }

        public void Unbind()
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }
    }
}
