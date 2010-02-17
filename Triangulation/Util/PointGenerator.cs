using System;
using System.Collections.Generic;

namespace Poly2Tri {
	public class PointGenerator {
		static readonly Random RNG = new Random();
		public static List<TriangulationPoint> uniformDistribution(int n, double scale) {
			ArrayList<TriangulationPoint> points = new ArrayList<TriangulationPoint>();
			for (int i = 0; i < n; i++) {
				points.add(new TriangulationPoint(scale * (0.5 - RNG.NextDouble()), scale * (0.5 - RNG.NextDouble())));
			}
			return points;
		}

		public static List<TriangulationPoint> uniformGrid(int n, double scale) {
			double x = 0;
			double size = scale / n;
			double halfScale = 0.5 * scale;

			ArrayList<TriangulationPoint> points = new ArrayList<TriangulationPoint>();
			for (int i = 0; i < n + 1; i++) {
				x = halfScale - i * size;
				for (int j = 0; j < n + 1; j++) {
					points.add(new TriangulationPoint(x, halfScale - j * size));
				}
			}
			return points;
		}
	}
}
