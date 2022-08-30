using OpenTK.Mathematics;

namespace izb_on
{
	class Camera
	{
		private float ZNear;
		private float ZFar;
		private float Fovy = MathHelper.PiOver2;
		private float XAngle = 0.0f;
		private float YAngle = 0.0f;
		private float Aspect => (float)Width / Height;

		public int Width;
		public int Height;
		
		public Camera(int width, int height, float zNear, float zFar)
		{
			Width = width;
			Height = height;
			ZNear = zNear;
			ZFar = zFar;
		}

		public void MoveDirection(float dx, float dy)
		{
			float eightyRads = MathHelper.DegreesToRadians(80);
			XAngle = (XAngle + MathHelper.DegreesToRadians(dx)) % MathHelper.TwoPi;
			YAngle = MathHelper.Max(
				MathHelper.Min(
					YAngle + MathHelper.DegreesToRadians(dy),
					eightyRads),
				-eightyRads
			);

			float cosx = (float) MathHelper.Cos(XAngle);
			float sinx = (float) MathHelper.Sin(XAngle);
			float cosy = (float) MathHelper.Cos(YAngle);
			float siny = (float) MathHelper.Sin(YAngle);
			Direction = new Vector3(sinx * cosy, -siny, -cosx * cosy);
		}

		public Vector3 Position { get; set; }
		private Vector3 _direction = -Vector3.UnitZ;
		public Vector3 Direction { 
			get => _direction;
			set => _direction = value.Normalized();
		}
		public Matrix4 View => Matrix4.LookAt(Position, Position+Direction, Vector3.UnitY);
		public Matrix4 Projection => Matrix4.CreatePerspectiveFieldOfView(Fovy, Aspect, ZNear, ZFar);
	}
}