using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poly2Tri {
	public class DTSweepPointComparator : IComparer<TriangulationPoint> {
		public int Compare(TriangulationPoint p1, TriangulationPoint p2) {
			if (p1.Y < p2.Y) {
				return -1;
			} else if (p1.Y > p2.Y) {
				return 1;
			} else {
				if (p1.X < p2.X) {
					return -1;
				} else if (p1.X > p2.X) {
					return 1;
				} else {
					return 0;
				}
			}
		}
	}
}
