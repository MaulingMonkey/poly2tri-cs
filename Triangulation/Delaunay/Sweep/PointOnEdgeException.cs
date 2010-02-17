using System;

namespace Poly2Tri {
	public class PointOnEdgeException : RuntimeException {
		public PointOnEdgeException(String msg) : base(msg) { }
	}
}
