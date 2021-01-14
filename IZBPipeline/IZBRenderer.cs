using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using apur_on;

namespace IZBPipeline
{
	class IZBRenderer : GLResource
	{
		private List<Mesh> Scene;
		private Camera DefaultCam;

		private FragPosSampler PosSampler;

		// number of bins and resolution to use during
		// lightspace rendering
		private int LSWidth = 10;
		private int LSHeight = 10;

		// lightspace tranform + building bins/lists
		private Renderbuffer BinsBuffer; // indices to lists heads
		private Renderbuffer BinsLists;

		public IZBRenderer(List<Mesh> scene, Camera defaultCam)
		{
			Scene = scene;
			DefaultCam = defaultCam;

			BinsBuffer = new Renderbuffer(RenderbufferStorage.R32ui, LSWidth, LSHeight);
			// index into the SampleBuffer + index to next element in list
			BinsLists = new Renderbuffer(RenderbufferStorage.Rg32ui, DefaultCam.Width, DefaultCam.Height);

			PosSampler = new FragPosSampler(scene, defaultCam);
		}

		public void UpdateCam()
		{
			PosSampler.UpdateCam();
		}

		public void Render()
		{
			// GL.Enable(EnableCap.StencilTest);

			// GL.StencilFunc(StencilFunction.Always, 1, 0xFF);
			// GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Replace);
			
			// SampleBuffer.Bind();
			// GL.Clear(ClearBufferMask.ColorBufferBit);
			// GL.Clear(ClearBufferMask.DepthBufferBit);
			// GL.Clear(ClearBufferMask.StencilBufferBit);
			
			// SampleShader.Use();
			// Scene.ForEach(m => m.Draw());
			// Framebuffer.BindDefault();

			// GL.Disable(EnableCap.StencilTest);

			PosSampler.Render();
		}

		public void Assign() { }

		public void Unassign()
		{
			PosSampler.Unassign();
		}
	}
}
