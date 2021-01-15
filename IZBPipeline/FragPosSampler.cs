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
		
		public ColorTexture2D SampleImage;
		public DepthStencilTexture SampleDepthStencilBuffer;

		public FragPosSampler(List<Mesh> scene, Camera defaultCam)
		{
			Scene = scene;
			DefaultCam = defaultCam;

			SampleShader.SetMatrix4("view", DefaultCam.View);
			SampleShader.SetMatrix4("proj", DefaultCam.Projection);

			SampleImage = new ColorTexture2D(DefaultCam.Width, DefaultCam.Height);
			SampleDepthStencilBuffer = new DepthStencilTexture(DefaultCam.Width, DefaultCam.Height);

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
			GL.Enable(EnableCap.StencilTest);

			GL.StencilFunc(StencilFunction.Always, 0x01, 0xFF);
			GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Replace);
			
			SampleBuffer.Bind();
			GL.Clear(ClearBufferMask.ColorBufferBit);
			GL.Clear(ClearBufferMask.DepthBufferBit);
			GL.Clear(ClearBufferMask.StencilBufferBit);
			
			SampleShader.Use();
			Scene.ForEach(m => m.Draw());

			Framebuffer.BindDefault();
			
			GL.Disable(EnableCap.StencilTest);
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