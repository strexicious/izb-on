using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using izb_on;

namespace IZBPipeline
{
	class FinalPass : GLResource
	{
		private Shader FinalShader = new Shader("izbfinal");

		public FinalPass(FragPosSampler posSampler) {
			GL.BindImageTexture(4, posSampler.ColorImage.TextureId, 0, false, 0, TextureAccess.ReadOnly, posSampler.ColorImage.InternalFormat());
		}

		public void Render()
		{
			GL.Disable(EnableCap.DepthTest);
			FinalShader.Use();
			GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
			GL.Enable(EnableCap.DepthTest);
		}

		public void Assign() { }

		public void Unassign() { }
	}
}