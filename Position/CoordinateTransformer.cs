namespace Poly2Tri {
	public interface CoordinateTransformer {
		double toX(TriangulationPoint p);
		double toY(TriangulationPoint p);
		double toZ(TriangulationPoint p);
		float toXf(TriangulationPoint p);
		float toYf(TriangulationPoint p);
		float toZf(TriangulationPoint p);

		void transform(TriangulationPoint p, TriangulationPoint store);
		void transform(TriangulationPoint p);
	}
}
