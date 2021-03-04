using apur_on;
using OpenTK.Graphics.OpenGL;

namespace IZBPipeline
{
	class LightSpaceBinner : GLResource
	{
		private ComputeShader BinShader = new ComputeShader("binSamples");

		// number of bins and resolution to use during
		// lightspace rendering
		public const int LSWidth = 100;
		public const int LSHeight = 100;

		// lightspace tranform + building bins/lists
		private UIntTexture _BinsHeads; // indices to lists heads
		private UIntTexture _BinsLists;

		public int BinsHeads => _BinsHeads.TextureId;
		public int BinsLists => _BinsLists.TextureId;

		public LightSpaceBinner(int width, int height) {
			_BinsLists = new UIntTexture(width, height);
			_BinsHeads = new UIntTexture(LSWidth, LSHeight);
			GL.BindImageTexture(1, _BinsLists.TextureId, 0, false, 0, TextureAccess.ReadWrite, _BinsLists.InternalFormat());
			GL.BindImageTexture(2, _BinsHeads.TextureId, 0, false, 0, TextureAccess.ReadWrite, _BinsHeads.InternalFormat());

			BinShader.SetInt("pos_mask", 0);
		}

		public void Dispatch()
		{
			BinShader.Use();
			_BinsLists.Clear();
			_BinsHeads.Clear();
			GL.DispatchCompute(100, 50, 1);
		}

		public void Assign() { }

		public void Unassign()
		{
			_BinsHeads.Unassign();
			_BinsLists.Unassign();
			BinShader.Unassign();
		}
	}
}