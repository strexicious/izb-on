using System;
using System.Runtime.InteropServices;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;

namespace apur_on
{
	class Mesh : GLResource
	{
		int Vao;
		int Vbo;

		int vCount;

		public Mesh(Assimp.Mesh mesh)
		{
			Assign();

			GL.BindVertexArray(Vao);
			GL.EnableVertexAttribArray(0);
			GL.EnableVertexAttribArray(1);

			// positions + normals
			float[] vertexData = new float[mesh.VertexCount * 6];
			for (int i = 0; i < mesh.VertexCount; ++i)
			{
				vertexData[i * 6 + 0] = mesh.Vertices[i].X;
				vertexData[i * 6 + 1] = mesh.Vertices[i].Y;
				vertexData[i * 6 + 2] = mesh.Vertices[i].Z;

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
			GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 24, new IntPtr(0));

			vCount = mesh.VertexCount;
		}

		public void Draw()
		{
			GL.BindVertexArray(Vao);
			GL.DrawArrays(PrimitiveType.Triangles, 0, vCount);
		}

		public void Assign()
		{
			Vao = GL.GenVertexArray();
			Vbo = GL.GenBuffer();
		}

		public void Unassign()
		{
			GL.BindVertexArray(0);
			GL.BindBuffer(BufferTarget.ArrayBuffer, Vbo);

			GL.DeleteVertexArray(Vao);
			GL.DeleteBuffer(Vbo);
		}
	}
}