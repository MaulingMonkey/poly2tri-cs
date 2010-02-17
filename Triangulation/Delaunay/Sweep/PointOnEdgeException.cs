using System;

namespace Poly2Tri {
	public class PointOnEdgeException : NotImplementedException {
		public PointOnEdgeException(String msg) : base(msg) { }
	}
}
