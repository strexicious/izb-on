using System;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;

namespace izb_on
{
	class Mesh : GLResource
	{
		private int Vao;
		private int Vbo;
		private int Ebo;

		public int VBO => Vbo;
		public int EBO => Ebo;

		private int vCount;
		private int iCount;

		public int TriangleCount => iCount / 3;

		public BoundingBox bbox;

		public Mesh(Assimp.Mesh mesh)
		{
			Assign();

			bbox = new BoundingBox(Vector3.PositiveInfinity, Vector3.NegativeInfinity);

			GL.BindVertexArray(Vao);
			GL.EnableVertexAttribArray(0);
			GL.EnableVertexAttribArray(1);

			int[] indices = mesh.GetIndices();
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, Ebo);
			GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(int) * indices.Length, indices, BufferUsageHint.StaticDraw);
			iCount = indices.Length;

			// positions + normals
			float[] vertexData = new float[mesh.VertexCount * 6];
			for (int i = 0; i < mesh.VertexCount; ++i)
			{
				vertexData[i * 6 + 0] = mesh.Vertices[i].X;
				vertexData[i * 6 + 1] = mesh.Vertices[i].Y;
				vertexData[i * 6 + 2] = mesh.Vertices[i].Z;

				bbox.MinPoint.X = Math.Min(bbox.MinPoint.X, mesh.Vertices[i].X);
				bbox.MinPoint.Y = Math.Min(bbox.MinPoint.Y, mesh.Vertices[i].Y);
				bbox.MinPoint.Z = Math.Min(bbox.MinPoint.Z, mesh.Vertices[i].Z);
				bbox.MaxPoint.X = Math.Max(bbox.MaxPoint.X, mesh.Vertices[i].X);
				bbox.MaxPoint.Y = Math.Max(bbox.MaxPoint.Y, mesh.Vertices[i].Y);
				bbox.MaxPoint.Z = Math.Max(bbox.MaxPoint.Z, mesh.Vertices[i].Z);

				try
				{
					vertexData[i * 6 + 3] = mesh.Normals[i].X;
					vertexData[i * 6 + 4] = mesh.Normals[i].Y;
					vertexData[i * 6 + 5] = mesh.Normals[i].Z;	
				}
				catch (System.ArgumentOutOfRangeException)
				{
					System.Console.Error.WriteLine("Failed indexing mesh normals");
					throw;
				}
			}

			GL.BindBuffer(BufferTarget.ArrayBuffer, Vbo);
			GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * vertexData.Length, vertexData, BufferUsageHint.StaticDraw);

			GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 24, new IntPtr(0));
			GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 24, new IntPtr(12));

			vCount = mesh.VertexCount;
		}

		public void Draw()
		{
			GL.BindVertexArray(Vao);
			GL.DrawElements(PrimitiveType.Triangles, iCount, DrawElementsType.UnsignedInt, 0);
		}

		public void Assign()
		{
			Vao = GL.GenVertexArray();
			Vbo = GL.GenBuffer();
			Ebo = GL.GenBuffer();
		}

		public void Unassign()
		{
			GL.BindVertexArray(0);
			GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

			GL.DeleteVertexArray(Vao);
			GL.DeleteBuffer(Vbo);
			GL.DeleteBuffer(Ebo);
		}
	}
}