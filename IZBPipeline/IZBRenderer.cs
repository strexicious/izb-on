using System.Collections.Generic;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;
using apur_on;

namespace IZBPipeline
{
	class IZBRenderer : GLResource
	{
		private List<Mesh> Scene;
		private Camera DefaultCam;
		private Vector3 LightDir;

		private FragPosSampler PosSampler;
		private LightSpaceTransformer LSTransformer;
		private LightSpaceBinner Binner;
		private ConservativeTester ConTester;
		private FinalPass FinalPasser;

		public IZBRenderer(List<Mesh> scene, Camera defaultCam)
		{
			Scene = scene;
			DefaultCam = defaultCam;
			LightDir = new Vector3(-1, -1, -1).Normalized();

			PosSampler = new FragPosSampler(scene, defaultCam);
			LSTransformer = new LightSpaceTransformer(scene, PosSampler, LightDir);
			Binner = new LightSpaceBinner(defaultCam.Width, defaultCam.Height);
			ConTester = new ConservativeTester(LSTransformer, defaultCam.Width, defaultCam.Height);
			FinalPasser = new FinalPass(PosSampler);
		}

		public void UpdateCam()
		{
			PosSampler.UpdateCam();
		}

		public void Render()
		{
			GL.ActiveTexture(TextureUnit.Texture0);
			GL.BindTexture(TextureTarget.Texture2D, PosSampler.SampleDepthStencilBuffer.TextureId);
			PosSampler.Render();
			LSTransformer.Dispatch();
			GL.MemoryBarrier(MemoryBarrierFlags.ShaderImageAccessBarrierBit);
			Binner.Dispatch();
			GL.MemoryBarrier(MemoryBarrierFlags.ShaderImageAccessBarrierBit);
			ConTester.Dispatch(Scene);
			GL.MemoryBarrier(MemoryBarrierFlags.ShaderImageAccessBarrierBit);
			FinalPasser.Render();
		}

		public void Assign() { }

		public void Unassign()
		{
			PosSampler.Unassign();
			LSTransformer.Unassign();
			Binner.Unassign();
		}
	}
}
