using OpenTK.Mathematics;

namespace apur_on
{
	struct BoundingBox
	{
		public Vector3 MinPoint;
		public Vector3 MaxPoint;

		public BoundingBox(Vector3 minPoint, Vector3 maxPoint)
		{
			MinPoint = minPoint;
			MaxPoint = maxPoint;
		}
	}
}
