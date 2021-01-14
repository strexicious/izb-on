using System;
using OpenTK.Graphics.OpenGL;

namespace apur_on
{
	class Framebuffer : GLResource
	{
		private int Fbo;

		public Framebuffer()
		{
			Assign();
		}

		public void Bind()
		{
			GL.BindFramebuffer(FramebufferTarget.Framebuffer, Fbo);
		}

		static public void BindDefault()
		{
			GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
		}

		public void Assign()
		{
			Fbo = GL.GenFramebuffer();
		}

		public void Unassign()
		{
			GL.DeleteFramebuffer(Fbo);
		}
	}
}