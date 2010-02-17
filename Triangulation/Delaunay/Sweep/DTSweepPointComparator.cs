using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poly2Tri {
	public class DTSweepPointComparator : IComparer<TriangulationPoint> {
		public int Compare(TriangulationPoint p1, TriangulationPoint p2) {
			if (p1.getY() < p2.getY()) {
				return -1;
			} else if (p1.getY() > p2.getY()) {
				return 1;
			} else {
				if (p1.getX() < p2.getX()) {
					return -1;
				} else if (p1.getX() > p2.getX()) {
					return 1;
				} else {
					return 0;
				}
			}
		}
	}
}
