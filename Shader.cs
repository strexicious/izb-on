using System;
using System.IO;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace izb_on
{
	class ShaderException : Exception { }
	
	class Shader : GLResource
	{
		private const string BasePath = "./res/shaders/";
		
		private string[] Source = new string[2];

		private int Program;
		
		public Shader(string name)
		{	
			using (StreamReader vshader = File.OpenText(BasePath + name + "/shader.vert"))
			{
				Source[0] = vshader.ReadToEnd();
			}
			
			using (StreamReader fshader = File.OpenText(BasePath + name + "/shader.frag"))
			{
				Source[1] = fshader.ReadToEnd();
			}

			Assign();

			Source = null;
		}

		public void Use()
		{
			GL.UseProgram(Program);
		}

		public void SetMatrix4(string name, Matrix4 matrix)
		{
			Use();
			
			int location = GL.GetUniformLocation(Program, name);
			GL.UniformMatrix4(location, false, ref matrix);
		}

		public void Assign()
		{
			int[] shaders = {
				GL.CreateShader(ShaderType.VertexShader),	// vertex
				GL.CreateShader(ShaderType.FragmentShader)	// fragment
			};

			GL.ShaderSource(shaders[0], Source[0]);
			GL.ShaderSource(shaders[1], Source[1]);

			GL.CompileShader(shaders[0]);
			GL.CompileShader(shaders[1]);

			CheckCompilationErrors(shaders);

			Program = GL.CreateProgram();
			GL.AttachShader(Program, shaders[0]);
			GL.AttachShader(Program, shaders[1]);
			GL.LinkProgram(Program);

			CheckLinkingErrors();

			// cleanup
			GL.DetachShader(Program, shaders[0]);
			GL.DetachShader(Program, shaders[1]);

			GL.DeleteShader(shaders[0]);
			GL.DeleteShader(shaders[1]);
		}

		private void CheckLinkingErrors()
		{
			int status;
			GL.GetProgram(Program, GetProgramParameterName.LinkStatus, out status);

			if (status == 0)
			{
				System.Console.WriteLine("Program linkage compilation failed:");
				System.Console.WriteLine(GL.GetShaderInfoLog(Program));
				throw new ShaderException();
			}
		}

		private void CheckCompilationErrors(int[] shaders)
		{
			foreach (int shader in shaders)
			{
				int status;
				GL.GetShader(shader, ShaderParameter.CompileStatus, out status);

				if (status == 0)
				{
					System.Console.WriteLine("Shader compilation failed:");
					System.Console.WriteLine(GL.GetShaderInfoLog(shader));
					throw new ShaderException();
				}
			}
		}

		public void Unassign()
		{
			GL.DeleteProgram(Program);
		}
	}

	class ComputeShader : GLResource
	{
		private const string BasePath = "./res/compute/";
		
		private string Source;

		private int Program;

		public ComputeShader(string name)
		{
			using (StreamReader vshader = File.OpenText(BasePath + name + ".comp"))
			{
				Source = vshader.ReadToEnd();
			}

			Assign();

			Source = null;
		}

		public void Use()
		{
			GL.UseProgram(Program);
		}

		public void SetInt(string name, int value)
		{
			Use();
			
			int location = GL.GetUniformLocation(Program, name);
			GL.Uniform1(location, value);
		}

		public void SetMatrix4(string name, Matrix4 value)
		{
			Use();
			
			int location = GL.GetUniformLocation(Program, name);
			GL.UniformMatrix4(location, false, ref value);
		}

		private void CheckLinkingErrors()
		{
			int status;
			GL.GetProgram(Program, GetProgramParameterName.LinkStatus, out status);

			if (status == 0)
			{
				System.Console.WriteLine("Program linkage compilation failed:");
				System.Console.WriteLine(GL.GetShaderInfoLog(Program));
				throw new ShaderException();
			}
		}

		private void CheckCompilationErrors(int shader)
		{
			int status;
			GL.GetShader(shader, ShaderParameter.CompileStatus, out status);

			if (status == 0)
			{
				System.Console.WriteLine("Shader compilation failed:");
				System.Console.WriteLine(GL.GetShaderInfoLog(shader));
				throw new ShaderException();
			}
		}

		public void Assign()
		{
			int shader = GL.CreateShader(ShaderType.ComputeShader);

			GL.ShaderSource(shader, Source);

			GL.CompileShader(shader);

			CheckCompilationErrors(shader);

			Program = GL.CreateProgram();
			GL.AttachShader(Program, shader);
			GL.LinkProgram(Program);

			CheckLinkingErrors();

			// cleanup
			GL.DetachShader(Program, shader);
			GL.DeleteShader(shader);
		}

		public void Unassign()
		{
			GL.DeleteProgram(Program);
		}
	}
}