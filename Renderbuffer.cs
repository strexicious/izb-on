using System;
using OpenTK.Graphics.OpenGL;

namespace apur_on
{
	class Renderbuffer : GLResource
	{
		public int Rbo { get; private set; }

		public Renderbuffer(RenderbufferStorage format, int width, int height)
		{
			Assign();
			
			GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, Rbo);
			GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, format, width, height);
		}

		public void Assign()
		{
			Rbo = GL.GenRenderbuffer();
		}

		public void AttachToFramebuffer(FramebufferAttachment attachment)
		{
			GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, attachment, RenderbufferTarget.Renderbuffer, Rbo);
		}

		public void Unassign()
		{
			GL.DeleteRenderbuffer(Rbo);
		}
	}
}