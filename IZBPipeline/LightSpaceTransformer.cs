using System.Collections.Generic;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;
using apur_on;

namespace IZBPipeline
{
	// This class already assumes a resolution of 1600x800
	class LightSpaceTransformer : GLResource
	{
		private ComputeShader TransformShader = new ComputeShader("toLightSpace");
		private Matrix4 _lightTransform;
		public Matrix4 LightTransform => _lightTransform;

		public LightSpaceTransformer(List<Mesh> scene, FragPosSampler posSampler, Vector3 lightDir)
		{
			GL.BindImageTexture(0, posSampler.SampleImage.TextureId, 0, false, 0, TextureAccess.ReadWrite, posSampler.SampleImage.InternalFormat());

			GL.ActiveTexture(TextureUnit.Texture0);
			GL.BindTexture(TextureTarget.Texture2D, posSampler.SampleDepthStencilBuffer.TextureId);
			GL.TextureParameter(posSampler.SampleDepthStencilBuffer.TextureId, TextureParameterName.DepthStencilTextureMode, (int)All.StencilIndex);

			ComputeLightTransform(scene, lightDir);
			TransformShader.SetMatrix4("lightTransform", _lightTransform);
		}

		private void ComputeLightTransform(List<Mesh> scene, Vector3 lightDir)
		{
			BoundingBox sceneBbox = scene[0].bbox;

			foreach (Mesh m in scene)
			{
				sceneBbox.Merge(m.bbox);
			}

			float radius = (sceneBbox.MaxPoint - sceneBbox.MinPoint).Length / 2;
			// This gets us light space translation and rotation
			var lightRotation = Matrix4.LookAt(sceneBbox.Center(), sceneBbox.Center()+lightDir, new Vector3(0, 1, 0));
			var sphereScale = Matrix4.CreateScale(1f/radius);
			// For future, for some reason the matrices are "transponsed", column are rows?
			// but the data is uploaded "correctly" on GPU. That is why the
			// matrix composition is inverse but no need to explicitly transpose for uniforms
			_lightTransform = lightRotation * sphereScale;
		}

		public void Dispatch()
		{
			TransformShader.Use();
			GL.DispatchCompute(100, 50, 1);
		}

		public void Assign() { }

		public void Unassign()
		{
			TransformShader.Unassign();
		}
	}
}
