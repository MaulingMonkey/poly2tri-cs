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

using boolean = System.Boolean;
using System.Collections.Generic;

namespace Poly2Tri {
	public abstract class TriangulationContext {
		protected TriangulationDebugContext _debug;

		protected ArrayList<DelaunayTriangle> _triList = new ArrayList<DelaunayTriangle>();

		public readonly ArrayList<TriangulationPoint> _points = new ArrayList<TriangulationPoint>(200);
		protected TriangulationMode _triangulationMode;
		protected Triangulatable _triUnit;

		private boolean _waitUntilNotified;

		private int _stepCount = 0;
		public int getStepCount() { return _stepCount; }

		public void done() {
			_stepCount++;
		}

		public abstract TriangulationAlgorithm Algorithm();

		public virtual void PrepareTriangulation(Triangulatable t) {
			_triUnit = t;
			_triangulationMode = t.TriangulationMode;
			t.Prepare(this);
		}

		public abstract TriangulationConstraint NewConstraint(TriangulationPoint a, TriangulationPoint b);

		public void addToList(DelaunayTriangle triangle) {
			_triList.add(triangle);
		}

		public List<DelaunayTriangle> getTriangles() {
			return _triList;
		}

		public Triangulatable getTriangulatable() {
			return _triUnit;
		}

		public ArrayList<TriangulationPoint> getPoints() {
			return _points;
		}

		public void update(string message) {
		}

		public virtual void Clear() {
			_points.clear();
			if (_debug != null) {
				_debug.Clear();
			}
			_stepCount = 0;
		}

		public TriangulationMode TriangulationMode { get { return _triangulationMode; }}

		public void waitUntilNotified(boolean b) {
			_waitUntilNotified = b;
		}

		public void terminateTriangulation() {
		}

		public virtual bool IsDebugEnabled { get; protected set; }

		public TriangulationDebugContext getDebugContext() {
			return _debug;
		}

		public DTSweepDebugContext getDebugContextAsDT() { return getDebugContext() as DTSweepDebugContext; }

		public void addPoints(List<TriangulationPoint> points) {
			_points.addAll(points);
		}
	}
}