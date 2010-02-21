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
using System.Linq;

namespace Poly2Tri {
	class PolygonInfo {
		public string Name { get; private set; }
		public TimeSpan LastTriangulationDuration { get; private set; }
		public Exception LastTriangulationException { get; private set; }

		public Polygon Polygon { get; private set; }

		public PolygonInfo( string name, Polygon polygon ) {
			Name = name;
			Polygon = polygon;
			Triangulate();
		}

		static Polygon CleanClone( Polygon polygon ) {
			var n = new Polygon(polygon.Points.Select( p => new PolygonPoint(p.X,p.Y) ) );
			if ( polygon.Holes!=null ) foreach ( var hole in polygon.Holes ) n.AddHole(CleanClone(hole));
			return n;
		}

		public void Triangulate() {
			var start = DateTime.Now;
			try {
				LastTriangulationException = null;
				var newpoly = CleanClone(Polygon);
				P2T.Triangulate(newpoly);
				Polygon = newpoly;
			} catch ( Exception e ) {
				LastTriangulationException = e;
			}
			var stop = DateTime.Now;
			LastTriangulationDuration = (stop-start);
		}
	}
}
