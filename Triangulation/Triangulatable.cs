using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poly2Tri {
	public interface Triangulatable {
		void Prepare(TriangulationContext tcx);

		IList<TriangulationPoint> Points { get; } // MM: Neither of these are used via interface (yet?)
		IList<DelaunayTriangle> Triangles { get; }

		void AddTriangle(DelaunayTriangle t);
		void AddTriangles(ArrayList<DelaunayTriangle> list);
		void ClearTriangles();

		TriangulationMode TriangulationMode { get; }
	}
}
