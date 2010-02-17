using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poly2Tri {
	public interface Triangulatable {
		void prepare(TriangulationContext tcx);

		IEnumerable<DelaunayTriangle> getTriangles();
		void addTriangle(DelaunayTriangle t);
		void addTriangles(ArrayList<DelaunayTriangle> list);
		void clearTriangulation();

		TriangulationMode getTriangulationMode();
	}
}
