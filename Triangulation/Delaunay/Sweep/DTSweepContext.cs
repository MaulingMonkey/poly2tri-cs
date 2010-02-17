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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using boolean = System.Boolean;

namespace Poly2Tri {
	/**
	 * 
	 * @author Thomas Åhlén, thahlen@gmail.com
	 *
	 */
	public class DTSweepContext : TriangulationContext {
		// Inital triangle factor, seed triangle will extend 30% of 
		// PointSet width to both left and right.
		private readonly float ALPHA = 0.3f;

		/** Advancing front **/
		public AdvancingFront aFront;
		/** head point used with advancing front */
		private TriangulationPoint _head;
		/** tail point used with advancing front */
		private TriangulationPoint _tail;
		public Basin basin = new Basin();
		public EdgeEvent edgeEvent = new EdgeEvent();

		private DTSweepPointComparator _comparator = new DTSweepPointComparator();

		public DTSweepContext() {
			clear();
		}

		public override void isDebugEnabled(boolean b) {
			if (b) {
				if (_debug == null) {
					_debug = new DTSweepDebugContext(this);
				}
			}
			_debugEnabled = b;
		}

		public void removeFromList(DelaunayTriangle triangle) {
			_triList.remove(triangle);
			// TODO: remove all neighbor pointers to this triangle
			//        for( int i=0; i<3; i++ )
			//        {
			//            if( triangle.neighbors[i] != null )
			//            {
			//                triangle.neighbors[i].clearNeighbor( triangle );
			//            }
			//        }
			//        triangle.clearNeighbors();
		}

		public void meshClean(DelaunayTriangle triangle) {
			meshCleanReq(triangle);
		}

		private void meshCleanReq(DelaunayTriangle triangle) {
			if (triangle != null && !triangle.isInterior()) {
				triangle.isInterior(true);
				_triUnit.AddTriangle(triangle);
				for (int i = 0; i < 3; i++) {
					if (!triangle.cEdge[i]) {
						meshCleanReq(triangle.neighbors[i]);
					}
				}
			}
		}

		public override void clear() {
			base.clear();
			_triList.clear();
		}

		public AdvancingFront getAdvancingFront() {
			return aFront;
		}

		public void setHead(TriangulationPoint p1) { _head = p1; }
		public TriangulationPoint getHead() { return _head; }

		public void setTail(TriangulationPoint p1) { _tail = p1; }
		public TriangulationPoint getTail() { return _tail; }

		public void addNode(AdvancingFrontNode node) {
			//        Console.WriteLine( "add:" + node.key + ":" + System.identityHashCode(node.key));
			//        m_nodeTree.put( node.getKey(), node );
			aFront.AddNode(node);
		}

		public void removeNode(AdvancingFrontNode node) {
			//        Console.WriteLine( "remove:" + node.key + ":" + System.identityHashCode(node.key));
			//        m_nodeTree.delete( node.getKey() );
			aFront.RemoveNode(node);
		}

		public AdvancingFrontNode locateNode(TriangulationPoint point) {
			return aFront.LocateNode(point);
		}

		public void createAdvancingFront() {
			AdvancingFrontNode head, tail, middle;
			// Initial triangle
			DelaunayTriangle iTriangle = new DelaunayTriangle(_points.get(0),
															   getTail(),
															   getHead());
			addToList(iTriangle);

			head = new AdvancingFrontNode(iTriangle.points[1]);
			head.Triangle = iTriangle;
			middle = new AdvancingFrontNode(iTriangle.points[0]);
			middle.Triangle = iTriangle;
			tail = new AdvancingFrontNode(iTriangle.points[2]);

			aFront = new AdvancingFront(head, tail);
			aFront.AddNode(middle);

			// TODO: I think it would be more intuitive if head is middles next and not previous
			//       so swap head and tail
			aFront.Head.Next = middle;
			middle.Next = aFront.Tail;
			middle.Prev = aFront.Head;
			aFront.Tail.Prev = middle;
		}

		public class Basin {
			public AdvancingFrontNode leftNode;
			public AdvancingFrontNode bottomNode;
			public AdvancingFrontNode rightNode;
			public double width;
			public boolean leftHighest;
		}

		public class EdgeEvent {
			public DTSweepConstraint constrainedEdge;
			public boolean right;
		}

		/**
		 * Try to map a node to all sides of this triangle that don't have 
		 * a neighbor.
		 * 
		 * @param t
		 */
		public void mapTriangleToNodes(DelaunayTriangle t) {
			AdvancingFrontNode n;
			for (int i = 0; i < 3; i++) {
				if (t.neighbors[i] == null) {
					n = aFront.LocatePoint(t.pointCW(t.points[i]));
					if (n != null) {
						n.Triangle = t;
					}
				}
			}
		}

		public override void prepareTriangulation(Triangulatable t)
    {
        base.prepareTriangulation( t );

        double xmax, xmin;
        double ymax, ymin;

        xmax = xmin = _points.get(0).getX();
        ymax = ymin = _points.get(0).getY();
        // Calculate bounds. Should be combined with the sorting
        foreach ( TriangulationPoint p in _points )
        {
            if( p.getX() > xmax )
                xmax = p.getX();
            if( p.getX() < xmin )
                xmin = p.getX();
            if( p.getY() > ymax )
                ymax = p.getY();
            if( p.getY() < ymin )
                ymin = p.getY();
        }

        double deltaX = ALPHA * ( xmax - xmin );
        double deltaY = ALPHA * ( ymax - ymin );
        TPoint p1 = new TPoint( xmax + deltaX, ymin - deltaY );
        TPoint p2 = new TPoint( xmin - deltaX, ymin - deltaY );

        setHead( p1 );
        setTail( p2 );

//        long time = System.nanoTime();
        // Sort the points along y-axis
			_points.Sort(_comparator);
//        logger.info( "Triangulation setup [{}ms]", ( System.nanoTime() - time ) / 1e6 );
    }


		public void finalizeTriangulation() {
			_triUnit.AddTriangles(_triList);
			_triList.clear();
		}

		public override TriangulationConstraint newConstraint(TriangulationPoint a, TriangulationPoint b) {
			return new DTSweepConstraint(a, b);
		}

		public override TriangulationAlgorithm algorithm() {
			return TriangulationAlgorithm.DTSweep;
		}
	}
}
