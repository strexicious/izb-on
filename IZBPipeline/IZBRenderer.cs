using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using apur_on;

namespace IZBPipeline
{
	class IZBRenderer : GLResource
	{
		private List<Mesh> Scene;
		private Camera DefaultCam;

		// first pass
		Shader SampleShader = new Shader("izbsample");
		Framebuffer SampleBuffer = new Framebuffer();
		Renderbuffer SampleImage;
		Renderbuffer AuxDepthBuffer;

		public IZBRenderer(List<Mesh> scene, Camera defaultCam)
		{
			Scene = scene;
			DefaultCam = defaultCam;

			SampleShader.SetMatrix4("view", DefaultCam.View);
			SampleShader.SetMatrix4("proj", DefaultCam.Projection);

			AuxDepthBuffer = new Renderbuffer(RenderbufferStorage.DepthComponent32, DefaultCam.Width, DefaultCam.Height);

			SampleImage = new Renderbuffer(RenderbufferStorage.Rgb32f, DefaultCam.Width, DefaultCam.Height);
			SampleBuffer.Attach(SampleImage, FramebufferAttachment.ColorAttachment0);
			SampleBuffer.Attach(AuxDepthBuffer, FramebufferAttachment.DepthAttachment);
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
			AuxDepthBuffer.Unassign();
		}
	}
}
