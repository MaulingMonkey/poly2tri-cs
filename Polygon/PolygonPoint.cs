namespace Poly2Tri {
	public class PolygonPoint : TPoint {
		public PolygonPoint _next;
		public PolygonPoint _previous;

		public PolygonPoint(double x, double y): base(x,y) {}

		public void setPrevious(PolygonPoint p) {
			_previous = p;
		}

		public void setNext(PolygonPoint p) {
			_next = p;
		}

		public PolygonPoint getNext() {
			return _next;
		}

		public PolygonPoint getPrevious() {
			return _previous;
		}
	}
}
