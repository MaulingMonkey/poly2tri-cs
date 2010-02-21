/* Poly2Tri
 * Copyright (c) 2009-2010, Poly2Tri Contributors
 * http://code.google.com/p/poly2tri/
 *
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without modification,
 * are permitted provided that the following conditions are met:
 *
 * * Redistributions of source code must retain the above copyright notice,
 *   this list of conditions and the following disclaimer.
 * * Redistributions in binary form must reproduce the above copyright notice,
 *   this list of conditions and the following disclaimer in the documentation
 *   and/or other materials provided with the distribution.
 * * Neither the name of Poly2Tri nor the names of its contributors may be
 *   used to endorse or promote products derived from this software without specific
 *   prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
 * A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
 * EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
 * PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
 * PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
 * LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

namespace Poly2Tri {
	public class TriangulationPoint {
		// List of edges this point constitutes an upper ending point (CDT)
		private ArrayList<DTSweepConstraint> edges;

		public TriangulationPoint( double x, double y ) { X=x; Y=y; }

		public override string ToString() {
			return "[" + X + "," + Y + "]";
		}

		public double X,Y;
		public float Xf { get { return (float)X; } set { X=value; } }
		public float Yf { get { return (float)Y; } set { Y=value; } }

		public ArrayList<DTSweepConstraint> getEdges() {
			return edges;
		}

		public void addEdge(DTSweepConstraint e) {
			if (edges == null) {
				edges = new ArrayList<DTSweepConstraint>();
			}
			edges.add(e);
		}

		public bool hasEdges() {
			return edges != null;
		}

		/**
		 * @param p - edge destination point
		 * @return the edge from this point to given point
		 */
		public DTSweepConstraint getEdge(TriangulationPoint p) {
			foreach (DTSweepConstraint c in edges) {
				if (c.P == p) {
					return c;
				}
			}
			return null;
		}

		//public override bool Equals(object obj) {
		//    if (obj is TriangulationPoint) {
		//        TriangulationPoint p = (TriangulationPoint)obj;
		//        return getX() == p.X && getY() == p.Y;
		//    }
		//    return base.Equals(obj);
		//}

		//public override int GetHashCode() {
		//    long bits = BitConverter java.lang.Double.doubleToLongBits(getX());
		//    bits ^= java.lang.Double.doubleToLongBits(getY()) * 31;
		//    return (((int)bits) ^ ((int)(bits >> 32)));
		//}
	}
}