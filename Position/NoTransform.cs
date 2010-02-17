namespace Poly2Tri {
	public class NoTransform : CoordinateTransformer {
		public double toX(TriangulationPoint p) {
			return p.getX();
		}

		public double toY(TriangulationPoint p) {
			return p.getY();
		}

		public double toZ(TriangulationPoint p) {
			return p.getZ();
		}

		public float toXf(TriangulationPoint p) {
			return p.getXf();
		}

		public float toYf(TriangulationPoint p) {
			return p.getYf();
		}

		public float toZf(TriangulationPoint p) {
			return p.getZf();
		}

		public void transform(TriangulationPoint p, TriangulationPoint store) {
			store.set(p.getX(), p.getY(), p.getZ());
		}

		public void transform(TriangulationPoint p) {
		}
	}
}
