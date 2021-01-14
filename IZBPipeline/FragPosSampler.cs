using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using apur_on;

namespace IZBPipeline
{
	class FragPosSampler : GLResource
	{
		private List<Mesh> Scene;
		private Camera DefaultCam;

		private Shader SampleShader = new Shader("izbsample");
		private Framebuffer SampleBuffer = new Framebuffer();
		private Texture SampleImage;
		private Renderbuffer SampleDepthStencilBuffer;

		public FragPosSampler(List<Mesh> scene, Camera defaultCam)
		{
			Scene = scene;
			DefaultCam = defaultCam;

			SampleShader.SetMatrix4("view", DefaultCam.View);
			SampleShader.SetMatrix4("proj", DefaultCam.Projection);

			SampleDepthStencilBuffer = new Renderbuffer(RenderbufferStorage.Depth24Stencil8, DefaultCam.Width, DefaultCam.Height);
			SampleImage = new ColorTexture2D(DefaultCam.Width, DefaultCam.Height);

			SampleBuffer.Bind();
			SampleImage.AttachToFramebuffer(FramebufferAttachment.ColorAttachment0);
			SampleDepthStencilBuffer.AttachToFramebuffer(FramebufferAttachment.DepthStencilAttachment);
			Framebuffer.BindDefault();
		}

		public void UpdateCam()
		{
			SampleShader.SetMatrix4("view", DefaultCam.View);
		}

		public void Render()
		{
			SampleBuffer.Bind();
			GL.Clear(ClearBufferMask.ColorBufferBit);
			GL.Clear(ClearBufferMask.DepthBufferBit);
			
			SampleShader.Use();
			Scene.ForEach(m => m.Draw());
			Framebuffer.BindDefault();
		}

		public void Assign() { }

		public void Unassign()
		{
			SampleShader.Unassign();
			SampleBuffer.Unassign();
			SampleImage.Unassign();
			SampleDepthStencilBuffer.Unassign();
		}
	}
}