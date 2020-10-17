using OpenTK.Mathematics;

namespace apur_on
{
	class Camera
	{
		private float zNear;
		private float zFar;
		private float aspect;
		private float fovy = MathHelper.PiOver2;
		
		public Camera(int width, int height, float zNear, float zFar)
		{
			this.zNear = zNear;
			this.zFar = zFar;
			aspect = (float)width / height;
		}

		public Vector3 Position { get; set; }
		private Vector3 _direction = -Vector3.UnitZ;
		public Vector3 Direction { 
			get => _direction;
			set => value.Normalized();
		}
		public Matrix4 View => Matrix4.LookAt(Position, Position+Direction, Vector3.UnitY);
		public Matrix4 Projection => Matrix4.CreatePerspectiveFieldOfView(fovy, aspect, zNear, zFar);
	}
}