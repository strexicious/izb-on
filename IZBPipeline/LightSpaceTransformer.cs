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

		public LightSpaceTransformer(List<Mesh> scene, FragPosSampler posSampler, Vector3 lightDir)
		{
			GL.BindImageTexture(0, posSampler.SampleImage.TextureId, 0, false, 0, TextureAccess.WriteOnly, posSampler.SampleImage.InternalFormat());

			GL.ActiveTexture(TextureUnit.Texture0);
			GL.BindTexture(TextureTarget.Texture2D, posSampler.SampleDepthStencilBuffer.TextureId);
			GL.TextureParameter(posSampler.SampleDepthStencilBuffer.TextureId, TextureParameterName.DepthStencilTextureMode, (int)All.StencilIndex);

			TransformShader.SetInt("pos_samples", 0);
			TransformShader.SetInt("pos_mask", 0);

			Matrix4 lightTransform = ComputeLightTransform(scene, lightDir);
			TransformShader.SetMatrix4("lightTransform", lightTransform);
		}

		private static Matrix4 ComputeLightTransform(List<Mesh> scene, Vector3 lightDir)
		{
			BoundingBox sceneBbox = scene[0].bbox;

			foreach (Mesh m in scene)
			{
				sceneBbox.Merge(m.bbox);
			}

			float radius = (sceneBbox.MaxPoint - sceneBbox.MinPoint).Length;
			var lightDirLeft = Vector3.Cross(new Vector3(0, 1, 0), lightDir);
			var lightDirDown = Vector3.Cross(lightDirLeft, lightDir);
			var newCenter = (lightDirLeft + lightDirDown) * radius + sceneBbox.Center();
			// This gets us light space translation and rotation
			var spaceTransform = Matrix4.LookAt(newCenter - lightDir * radius, newCenter, new Vector3(0, 1, 0));
			// but we can add few extra scaling and translation to prepare it for binning
			var binTransform = spaceTransform * Matrix4.CreateScale(0.5f / radius);
			return binTransform;
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
