using System;
using OpenTK.Graphics.OpenGL;

namespace apur_on
{
	class Renderbuffer : GLResource, FramebufferAttacheable
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

		public void AttachToFramebuffer(Action<int> attacher)
		{
			attacher(Rbo);
		}

		public void Unassign()
		{
			GL.DeleteRenderbuffer(Rbo);
		}
	}
}