using OpenTK.Mathematics;

namespace apur_on
{
	class Camera
	{
		private float zNear;
		private float zFar;
		private float aspect;
		private float fovy = MathHelper.PiOver2;
		private float xangle = 0.0f;
		private float yangle = 0.0f;
		
		public Camera(int width, int height, float zNear, float zFar)
		{
			this.zNear = zNear;
			this.zFar = zFar;
			aspect = (float)width / height;
		}

		public void MoveDirection(float dx, float dy)
		{
			// TODO: right now dx and dy are inversed from OpenTK in 4.0.5,
			// change this when it is fixed. for now we use this as a drop-in fix
			// float dx = -dx;
			// float dy = -dy;
			float eightyRads = MathHelper.DegreesToRadians(80);
			xangle = (xangle + MathHelper.DegreesToRadians(dx)) % MathHelper.TwoPi;
			yangle = MathHelper.Max(
				MathHelper.Min(
					yangle + MathHelper.DegreesToRadians(dy),
					eightyRads),
				-eightyRads
			);

			float cosx = (float) MathHelper.Cos(xangle);
			float sinx = (float) MathHelper.Sin(xangle);
			float cosy = (float) MathHelper.Cos(yangle);
			float siny = (float) MathHelper.Sin(yangle);
			Direction = new Vector3(sinx * cosy, -siny, -cosx * cosy);
		}

		public Vector3 Position { get; set; }
		private Vector3 _direction = -Vector3.UnitZ;
		public Vector3 Direction { 
			get => _direction;
			set => _direction = value.Normalized();
		}
		public Matrix4 View => Matrix4.LookAt(Position, Position+Direction, Vector3.UnitY);
		public Matrix4 Projection => Matrix4.CreatePerspectiveFieldOfView(fovy, aspect, zNear, zFar);
	}
}