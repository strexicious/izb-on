using System;
using OpenTK.Mathematics;

namespace izb_on
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

		public void Merge(BoundingBox bbox)
		{
			this.MinPoint.X = Math.Min(bbox.MinPoint.X, this.MinPoint.X);
			this.MinPoint.Y = Math.Min(bbox.MinPoint.Y, this.MinPoint.Y);
			this.MinPoint.Z = Math.Min(bbox.MinPoint.Z, this.MinPoint.Z);
			this.MaxPoint.X = Math.Max(bbox.MaxPoint.X, this.MaxPoint.X);
			this.MaxPoint.Y = Math.Max(bbox.MaxPoint.Y, this.MaxPoint.Y);
			this.MaxPoint.Z = Math.Max(bbox.MaxPoint.Z, this.MaxPoint.Z);
		}

		public Vector3 Center()
		{
			return this.MinPoint + (this.MaxPoint - this.MinPoint) / 2;
		}
	}
}
