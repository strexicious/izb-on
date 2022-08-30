using System.Collections.Generic;
using izb_on;
using OpenTK.Graphics.OpenGL;

namespace IZBPipeline
{
	class ConservativeTester : GLResource
	{
		private Shader ShadowShader = new Shader("izbshadow");
		public UIntTexture ShadowMap;

		public ConservativeTester(LightSpaceTransformer transformer, int width, int height)
		{
			ShadowMap = new UIntTexture(width, height);
			GL.BindImageTexture(3, ShadowMap.TextureId, 0, false, 0, TextureAccess.ReadWrite, ShadowMap.InternalFormat());
			
			ShadowShader.SetMatrix4("lightTransform", transformer.LightTransform);
		}

		public void Dispatch(List<Mesh> scene)
		{
			Framebuffer.BindDefault();

			GL.Viewport(0 ,0, 100, 100);

			GL.Clear(ClearBufferMask.ColorBufferBit);
			ShadowMap.Clear();
			
			GL.Disable(EnableCap.DepthTest);
			GL.Enable((EnableCap)All.ConservativeRasterizationNv);

			ShadowShader.Use();
			foreach (var m in scene)
			{
				GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 0, m.VBO);
				GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 1, m.EBO);
				m.Draw();
			}

			GL.Disable((EnableCap)All.ConservativeRasterizationNv);
			GL.Enable(EnableCap.DepthTest);
		
			GL.Viewport(0 ,0, 1600, 800);
		}

		public void Assign() { }

		public void Unassign()
		{
			ShadowShader.Unassign();
			ShadowMap.Unassign();
		}
	}
}