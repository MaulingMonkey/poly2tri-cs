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
using System.Threading;
using System.Linq;
using boolean = System.Boolean;

namespace Poly2Tri {

	/**
	 * 
	 * @author Thomas Åhlén, thahlen@gmail.com
	 * 
	 */
	public class TriangulationProcess {
		private TriangulationAlgorithm _algorithm;

		private TriangulationContext _tcx;
		private Thread _thread;
		private boolean _isTerminated = false;
		private int _pointCount = 0;
		private DateTime _timestamp = DateTime.Now;
		private double _triangulationTime = 0;

		private boolean _awaitingTermination;
		private boolean _restart = false;

		private List<Triangulatable> _triangulations = new List<Triangulatable>();

		private List<TriangulationProcessListener> _listeners = new List<TriangulationProcessListener>();

		public void addListener(TriangulationProcessListener listener) {
			_listeners.Add(listener);
		}

		public void removeListener(TriangulationProcessListener listener) {
			_listeners.Remove(listener);
		}

		public void clearListeners() {
			_listeners.Clear();
		}

		/**
		 * Notify all listeners of this new event
		 * @param event
		 */
		private void sendEvent(TriangulationProcessEvent e) {
			foreach (TriangulationProcessListener l in _listeners) {
				l.triangulationEvent(e, _tcx.Triangulatable);
			}
		}

		public int getStepCount() {
			return _tcx.StepCount;
		}

		public DateTime getTimestamp() {
			return _timestamp;
		}

		public double getTriangulationTime() {
			return _triangulationTime;
		}

		/**
		 * Uses SweepLine algorithm by default
		 * @param algorithm
		 */
		public TriangulationProcess() : this(TriangulationAlgorithm.DTSweep) { }

		public TriangulationProcess(TriangulationAlgorithm algorithm) {
			_algorithm = algorithm;
			_tcx = Poly2Tri.CreateContext(algorithm);
		}

		/**
		 * This retriangulates same set as previous triangulation
		 * useful if you want to do consecutive triangulations with 
		 * same data. Like when you when you want to do performance 
		 * tests.
		 */
		//    public void triangulate()
		//    {
		//        start();
		//    }

		/**
		 * Triangulate a PointSet with eventual constraints 
		 * 
		 * @param cps
		 */
		public void triangulate(PointSet ps) {
			_triangulations.Clear();
			_triangulations.Add(ps);
			start();
		}

		/**
		 * Triangulate a PointSet with eventual constraints 
		 * 
		 * @param cps
		 */
		public void triangulate(ConstrainedPointSet cps) {
			_triangulations.Clear();
			_triangulations.Add(cps);
			start();
		}

		/**
		 * Triangulate a PolygonSet
		 * 
		 * @param ps
		 */
		public void triangulate(PolygonSet ps) {
			_triangulations.Clear();
			_triangulations.AddRange(ps.Polygons.Cast<Triangulatable>());
			start();
		}

		/**
		 * Triangulate a Polygon
		 * 
		 * @param ps
		 */
		public void triangulate(Polygon polygon) {
			_triangulations.Clear();
			_triangulations.Add(polygon);
			start();
		}

		/**
		 * Triangulate a List of Triangulatables
		 * 
		 * @param ps
		 */
		public void triangulate(List<Triangulatable> list) {
			_triangulations.Clear();
			_triangulations.AddRange(list);
			start();
		}

		private void start() {
			if (_thread == null || _thread.ThreadState == ThreadState.Stopped) {
				_isTerminated = false;
				_thread = new Thread(this.run);
				_thread.Name = _algorithm.ToString() + "." + _tcx.TriangulationMode.ToString();
				_thread.Start();
				sendEvent(TriangulationProcessEvent.Started);
			} else {
				// Triangulation already running. Terminate it so we can start a new
				shutdown();
				_restart = true;
			}
		}

		public void run() {
			_pointCount = 0;
			try {
				var time = DateTime.Now;
				foreach (Triangulatable t in _triangulations) {
					_tcx.Clear();
					_tcx.PrepareTriangulation(t);
					_pointCount += _tcx.Points.Count;
					Poly2Tri.Triangulate(_tcx);
				}
				//_triangulationTime = ( System.nanoTime() - time ) / 1e6;
				_triangulationTime = (DateTime.Now - time).TotalMilliseconds;
				//logger.info( "Triangulation of {} points [{}ms]", _pointCount, _triangulationTime );
				sendEvent(TriangulationProcessEvent.Done);
			} catch (Exception) {
				if (_awaitingTermination) {
					_awaitingTermination = false;
					//logger.info( "Thread[{}] : {}", _thread.getName(), e.getMessage() );
					sendEvent(TriangulationProcessEvent.Aborted);
				} else {
					//e.printStackTrace();
					sendEvent(TriangulationProcessEvent.Failed);
				}
			} finally {
				_timestamp = DateTime.Now;
				_isTerminated = true;
				_thread = null;
			}

			// Autostart a new triangulation?
			if (_restart) {
				_restart = false;
				start();
			}
		}

		public void resume() {
			if (_thread != null) _thread.Resume();
		}

		public void shutdown() {
			_awaitingTermination = true;
			resume();
		}

		public TriangulationContext getContext() {
			return _tcx;
		}

		public boolean isDone() {
			return _isTerminated;
		}

		public boolean isReadable() {
			if (_thread == null) {
				return true;
			} else {
				lock (_thread) {
					return (_thread.ThreadState == ThreadState.Suspended);
				}
			}
		}

		public int getPointCount() {
			return _pointCount;
		}
	}
}
