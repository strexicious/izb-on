using System;
using OpenTK.Graphics.OpenGL;

namespace izb_on
{
	abstract class Texture : GLResource
	{
		protected int Txo;

		public int TextureId { get => Txo; }

		public Texture()
		{
			Assign();
		}

		public void Assign()
		{
			Txo = GL.GenTexture();
		}

		public void Unassign()
		{
			GL.DeleteTexture(Txo);
		}

		public abstract void Clear();
		public abstract void AttachToFramebuffer(FramebufferAttachment attachment);
		public abstract SizedInternalFormat InternalFormat();
	}

	class ColorTexture2D : Texture
	{
		public ColorTexture2D(int width, int height) : base()
		{
			GL.BindTexture(TextureTarget.Texture2D, Txo);
			GL.TexStorage2D(TextureTarget2d.Texture2D, 1, SizedInternalFormat.Rgba32f, width, height);
		}

		public override void AttachToFramebuffer(FramebufferAttachment attachment)
		{
			GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, attachment, TextureTarget.Texture2D, Txo, 0);
		}

		public override void Clear()
		{
			GL.ClearTexImage(Txo, 0, PixelFormat.Rgba, PixelType.Float, IntPtr.Zero);
		}

		public override SizedInternalFormat InternalFormat()
		{
			return SizedInternalFormat.Rgba32f;
		}
	}

	class DepthStencilTexture : Texture
	{
		public DepthStencilTexture(int width, int height) : base()
		{
			GL.BindTexture(TextureTarget.Texture2D, Txo);
			GL.TexStorage2D(TextureTarget2d.Texture2D, 1, (SizedInternalFormat)All.Depth24Stencil8, width, height);
		}

		public override void AttachToFramebuffer(FramebufferAttachment attachment)
		{
			GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, attachment, TextureTarget.Texture2D, Txo, 0);
		}

		public override void Clear()
		{
			GL.ClearTexImage(Txo, 0, PixelFormat.Rg, PixelType.Float, IntPtr.Zero);
		}

		public override SizedInternalFormat InternalFormat()
		{
			return (SizedInternalFormat)All.Depth24Stencil8;
		}
	}

	class UIntTexture : Texture
	{
		public UIntTexture(int width, int height) : base()
		{
			GL.BindTexture(TextureTarget.Texture2D, Txo);
			GL.TexStorage2D(TextureTarget2d.Texture2D, 1, SizedInternalFormat.R32ui, width, height);
		}

		public override void AttachToFramebuffer(FramebufferAttachment attachment)
		{
			throw new NotImplementedException();
		}

		public override void Clear()
		{
			GL.ClearTexImage(Txo, 0, PixelFormat.RedInteger, PixelType.UnsignedInt, IntPtr.Zero);
		}

		public override SizedInternalFormat InternalFormat()
		{
			return SizedInternalFormat.R32ui;
		}
	}
}