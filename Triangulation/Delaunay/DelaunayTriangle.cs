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

/// Changes from the Java version
///   attributification
/// Future possibilities
///   Flattening out the number of indirections
///     Replacing arrays of 3 with fixed-length arrays?
///     Replacing bool[3] with a bit array of some sort?
///     Bundling everything into an AoS mess?
///     Hardcode them all as ABC ?

using System;
using System.Diagnostics;

namespace Poly2Tri {
	public class DelaunayTriangle {
		public struct FixedArray3<T> where T:class {
			public T _0, _1, _2;
			public T this[ int index ] { get {
				switch ( index ) {
				case 0: return _0;
				case 1: return _1;
				case 2: return _2;
				default: throw new IndexOutOfRangeException();
				}
			} set {
				switch ( index ) {
				case 0: _0 = value; break;
				case 1: _1 = value; break;
				case 2: _2 = value; break;
				default: throw new IndexOutOfRangeException();
				}
			}}
			public bool Contains( T value ) {
				for ( int i = 0 ; i < 3 ; ++i ) if ( this[i]==value ) return true;
				return false;
			}
			public int IndexOf( T value ) {
				for ( int i = 0 ; i < 3 ; ++i ) if ( this[i]==value ) return i;
				return -1;
			}
			public void Clear() {
				_0=_1=_2=null;
			}
			public void Clear( T value ) {
				for ( int i = 0 ; i < 3 ; ++i ) if ( this[i]==value ) this[i] = null;
			}
		}

		public FixedArray3<TriangulationPoint> Points;
		public FixedArray3<DelaunayTriangle  > Neighbors;
		//public readonly TriangulationPoint[] Points = new TriangulationPoint[3];
		//public readonly DelaunayTriangle[] Neighbors = new DelaunayTriangle[3];
		public readonly bool[] EdgeIsConstrained = new bool[] { false, false, false };
		public readonly bool[] EdgeIsDelauney    = new bool[] { false, false, false };
		public bool IsInterior { get; set; }

		public DelaunayTriangle(TriangulationPoint p1, TriangulationPoint p2, TriangulationPoint p3) {
			Points[0] = p1;
			Points[1] = p2;
			Points[2] = p3;
		}

		public int IndexOf(TriangulationPoint p) {
			int i = Points.IndexOf(p);
			if (i==-1) throw new RuntimeException("Calling index with a point that doesn't exist in triangle");
			return i;
		}

		public int IndexCWFrom(TriangulationPoint p) {
			int i = IndexOf(p);
			switch (i) {
			case 0: return 2;
			case 1: return 0;
			default: return 1;
			}
		}

		public int IndexCCWFrom(TriangulationPoint p) {
			int i = IndexOf(p);
			switch (i) {
			case 0: return 1;
			case 1: return 2;
			default: return 0;
			}
		}

		public bool Contains(TriangulationPoint p) {
			return (p == Points[0] || p == Points[1] || p == Points[2]);
		}

		/// <summary>
		/// Update neighbor pointers
		/// </summary>
		/// <param name="p1">Point 1 of the shared edge</param>
		/// <param name="p2">Point 2 of the shared edge</param>
		/// <param name="t">This triangle's new neighbor</param>
		private void MarkNeighbor( TriangulationPoint p1, TriangulationPoint p2, DelaunayTriangle t ) {
			int i = EdgeIndex(p1,p2);
			if ( i==-1 ) throw new Exception( "Error marking neighbors -- t doesn't contain edge p1-p2!" );
			Neighbors[i] = t;
		}

		/// <summary>
		/// Exhaustive search to update neighbor pointers
		/// </summary>
		public void MarkNeighbor( DelaunayTriangle t ) {
			// Points of this triangle also belonging to t
			bool a = t.Contains(Points[0]);
			bool b = t.Contains(Points[1]);
			bool c = t.Contains(Points[2]);

			if      (b&&c) { Neighbors[0]=t; t.MarkNeighbor(Points[1],Points[2],this); }
			else if (a&&c) { Neighbors[1]=t; t.MarkNeighbor(Points[0],Points[2],this); }
			else if (a&&b) { Neighbors[2]=t; t.MarkNeighbor(Points[0],Points[1],this); }
			else throw new Exception( "Failed to mark neighbor, doesn't share an edge!");
		}

		/// <param name="t">Opposite triangle</param>
		/// <param name="p">The point in t that isn't shared between the triangles</param>
		public TriangulationPoint OppositePoint(DelaunayTriangle t, TriangulationPoint p) {
			Debug.Assert(t != this, "self-pointer error");
			return PointCWFrom(t.PointCWFrom(p));
		}
		
		public DelaunayTriangle NeighborCWFrom    (TriangulationPoint point) { return Neighbors[(Points.IndexOf(point)+1)%3]; }
		public DelaunayTriangle NeighborCCWFrom   (TriangulationPoint point) { return Neighbors[(Points.IndexOf(point)+2)%3]; }
		public DelaunayTriangle NeighborAcrossFrom(TriangulationPoint point) { return Neighbors[ Points.IndexOf(point)     ]; }

		public TriangulationPoint PointCCWFrom(TriangulationPoint point) { return Points[(IndexOf(point)+1)%3]; }
		public TriangulationPoint PointCWFrom (TriangulationPoint point) { return Points[(IndexOf(point)+2)%3]; }

		/// <summary>
		/// Legalize triangle by rotating clockwise around oPoint
		/// </summary>
		/// <param name="oPoint">The origin point to rotate around</param>
		/// <param name="nPoint">???</param>
		public void Legalize(TriangulationPoint oPoint, TriangulationPoint nPoint) {
			if (oPoint == Points[0]) {
				Points[1] = Points[0];
				Points[0] = Points[2];
				Points[2] = nPoint;
			} else if (oPoint == Points[1]) {
				Points[2] = Points[1];
				Points[1] = Points[0];
				Points[0] = nPoint;
			} else if (oPoint == Points[2]) {
				Points[0] = Points[2];
				Points[2] = Points[1];
				Points[1] = nPoint;
			} else {
				throw new RuntimeException("legalization bug");
			}
		}

		public override string ToString() { return Points[0] + "," + Points[1] + "," + Points[2]; }

		/// <summary>
		/// Finalize edge marking
		/// </summary>
		public void MarkNeighborEdges() {
			for (int i = 0; i < 3; i++) if ( EdgeIsConstrained[i] && Neighbors[i] != null ) {
				Neighbors[i].MarkConstrainedEdge(Points[(i+1)%3], Points[(i+2)%3]);
			}
		}

		public void MarkEdge(DelaunayTriangle triangle) {
			for (int i = 0; i < 3; i++) if ( EdgeIsConstrained[i] ) {
				triangle.MarkConstrainedEdge(Points[(i+1)%3], Points[(i+2)%3]);
			}
		}

		public void MarkEdge(ArrayList<DelaunayTriangle> tList) {
			foreach ( DelaunayTriangle t in tList )
			for ( int i = 0; i < 3; i++ )
			if ( t.EdgeIsConstrained[i] )
			{
				MarkConstrainedEdge( t.Points[(i+1)%3], t.Points[(i+2)%3] );
			}
		}

		public void MarkConstrainedEdge(int index) {
			EdgeIsConstrained[index] = true;
		}

		public void MarkConstrainedEdge(DTSweepConstraint edge) {
			MarkConstrainedEdge(edge.p, edge.q);
		}

		/// <summary>
		/// Mark edge as constrained
		/// </summary>
		public void MarkConstrainedEdge(TriangulationPoint p, TriangulationPoint q) {
			int i = EdgeIndex(p,q);
			if ( i != -1 ) EdgeIsConstrained[i] = true;
		}

		public double Area() {
			double b = Points[0].X - Points[1].X;
			double h = Points[2].Y - Points[1].Y;

			return Math.Abs((b * h * 0.5f));
		}

		public TriangulationPoint Centroid() {
			double cx = (Points[0].X + Points[1].X + Points[2].X) / 3f;
			double cy = (Points[0].Y + Points[1].Y + Points[2].Y) / 3f;
			return new TriangulationPoint(cx, cy);
		}

		/// <summary>
		/// Get the index of the neighbor that shares this edge (or -1 if it isn't shared)
		/// </summary>
		/// <returns>index of the shared edge or -1 if edge isn't shared</returns>
		public int EdgeIndex(TriangulationPoint p1, TriangulationPoint p2) {
			int i1 = Points.IndexOf(p1);
			int i2 = Points.IndexOf(p2);

			// Points of this triangle in the edge p1-p2
			bool a = (i1==0 || i2==0);
			bool b = (i1==1 || i2==1);
			bool c = (i1==2 || i2==2);

			if (b&&c) return 0;
			if (a&&c) return 1;
			if (a&&b) return 2;
			return -1;
		}

		public bool GetConstrainedEdgeCCW(TriangulationPoint p) {
			if (p == Points[0]) {
				return EdgeIsConstrained[2];
			} else if (p == Points[1]) {
				return EdgeIsConstrained[0];
			}
			return EdgeIsConstrained[1];
		}

		public bool GetConstrainedEdgeCW(TriangulationPoint p) {
			if (p == Points[0]) {
				return EdgeIsConstrained[1];
			} else if (p == Points[1]) {
				return EdgeIsConstrained[2];
			}
			return EdgeIsConstrained[0];
		}

		public bool GetConstrainedEdgeAcross(TriangulationPoint p) {
			if (p == Points[0]) {
				return EdgeIsConstrained[0];
			} else if (p == Points[1]) {
				return EdgeIsConstrained[1];
			}
			return EdgeIsConstrained[2];
		}

		public void SetConstrainedEdgeCCW(TriangulationPoint p, bool ce) {
			if (p == Points[0]) {
				EdgeIsConstrained[2] = ce;
			} else if (p == Points[1]) {
				EdgeIsConstrained[0] = ce;
			} else {
				EdgeIsConstrained[1] = ce;
			}
		}

		public void SetConstrainedEdgeCW(TriangulationPoint p, bool ce) {
			if (p == Points[0]) {
				EdgeIsConstrained[1] = ce;
			} else if (p == Points[1]) {
				EdgeIsConstrained[2] = ce;
			} else {
				EdgeIsConstrained[0] = ce;
			}
		}

		public void SetConstrainedEdgeAcross(TriangulationPoint p, bool ce) {
			if (p == Points[0]) {
				EdgeIsConstrained[0] = ce;
			} else if (p == Points[1]) {
				EdgeIsConstrained[1] = ce;
			} else {
				EdgeIsConstrained[2] = ce;
			}
		}

		public bool GetDelunayEdgeCCW(TriangulationPoint p) {
			if (p == Points[0]) {
				return EdgeIsDelauney[2];
			} else if (p == Points[1]) {
				return EdgeIsDelauney[0];
			}
			return EdgeIsDelauney[1];
		}

		public bool GetDelunayEdgeCW(TriangulationPoint p) {
			if (p == Points[0]) {
				return EdgeIsDelauney[1];
			} else if (p == Points[1]) {
				return EdgeIsDelauney[2];
			}
			return EdgeIsDelauney[0];
		}

		public bool GetDelunayEdgeAcross(TriangulationPoint p) {
			if (p == Points[0]) {
				return EdgeIsDelauney[0];
			} else if (p == Points[1]) {
				return EdgeIsDelauney[1];
			}
			return EdgeIsDelauney[2];
		}

		public void SetDelunayEdgeCCW(TriangulationPoint p, bool e) {
			if (p == Points[0]) {
				EdgeIsDelauney[2] = e;
			} else if (p == Points[1]) {
				EdgeIsDelauney[0] = e;
			} else {
				EdgeIsDelauney[1] = e;
			}
		}

		public void SetDelunayEdgeCW(TriangulationPoint p, bool e) {
			if (p == Points[0]) {
				EdgeIsDelauney[1] = e;
			} else if (p == Points[1]) {
				EdgeIsDelauney[2] = e;
			} else {
				EdgeIsDelauney[0] = e;
			}
		}

		public void SetDelunayEdgeAcross(TriangulationPoint p, bool e) {
			if (p == Points[0]) {
				EdgeIsDelauney[0] = e;
			} else if (p == Points[1]) {
				EdgeIsDelauney[1] = e;
			} else {
				EdgeIsDelauney[2] = e;
			}
		}

		public void ClearDelunayEdges() {
			EdgeIsDelauney[0] = false;
			EdgeIsDelauney[1] = false;
			EdgeIsDelauney[2] = false;
		}
	}
}