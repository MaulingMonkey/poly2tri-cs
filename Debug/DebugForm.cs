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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace Poly2Tri {
	[System.ComponentModel.DesignerCategory("")] class DebugForm : Form {
		List<PolygonInfo> Infos = new List<PolygonInfo>()
			{ new PolygonInfo( "Two", (ExampleData.Two ))
			, new PolygonInfo( "Bird", (ExampleData.Bird ))
			, new PolygonInfo( "Custom", (ExampleData.Custom ))
			, new PolygonInfo( "Debug", (ExampleData.Debug ))
			, new PolygonInfo( "Debug2", (ExampleData.Debug2 ))
			, new PolygonInfo( "Diamond", (ExampleData.Diamond ))
			, new PolygonInfo( "Dude", (ExampleData.Dude ))
			, new PolygonInfo( "Funny", (ExampleData.Funny ))
			, new PolygonInfo( "NazcaHeron", (ExampleData.NazcaHeron ))
			, new PolygonInfo( "NazcaMonkey", (ExampleData.NazcaMonkey ))
			, new PolygonInfo( "Sketchup", (ExampleData.Sketchup ))
			, new PolygonInfo( "Star", (ExampleData.Star ))
			, new PolygonInfo( "Strange", (ExampleData.Strange ))
			, new PolygonInfo( "Tank", (ExampleData.Tank ))
			, new PolygonInfo( "Test", (ExampleData.Test ))
			};

		int InfoI = 0;
		public PolygonInfo Info { get { return Infos[InfoI]; } }

		DateTime PointBounceStart = DateTime.Now;
		int      PointBounceIndex;

		DebugForm() {
			ClientSize = new Size(800,600);
			BackColor = Color.Black;
			ForeColor = Color.White;
			DoubleBuffered = true;
			StartPosition = FormStartPosition.CenterScreen;
			Text = "Poly2Tri Example & Debug setup";
		}

		int LineY;
		void AddText( Graphics fx, string text ) {
			TextRenderer.DrawText( fx, text, Font, new Point(10,LineY), ForeColor );
			LineY += TextRenderer.MeasureText( text, Font ).Height;
		}

		void AddText( Graphics fx, string format, params object[] args ) { AddText(fx,String.Format(format,args)); }

		void AddText( Graphics fx, Exception e ) {
			if ( e == null ) return;

			string text = "Exception: "+e.Message;
			TextRenderer.DrawText( fx, text, Font, new Point(10,LineY), Color.Red );
			LineY += TextRenderer.MeasureText( text, Font ).Height;

			while ( e.InnerException != null ) {
				e = e.InnerException;
				text = "Innert Exception: "+e.Message;
				TextRenderer.DrawText( fx, text, Font, new Point(10,LineY), Color.Red );
				LineY += TextRenderer.MeasureText( text, Font ).Height;
			}
		}

		protected override void OnKeyDown( KeyEventArgs e ) {
			e.Handled = true;
			switch ( e.KeyCode ) {
			case Keys.G: GC.GetTotalMemory(true); break;
			case Keys.Left:
				if ( --InfoI < 0 ) InfoI += Infos.Count;
				break;
			case Keys.Right:
				if ( ++InfoI >= Infos.Count ) InfoI -= Infos.Count;
				break;
			default:
				e.Handled = false;
				base.OnKeyDown(e);
				break;
			}
		}

		static int FrameI=0;

		Bitmap FrameCache;
		DateTime PreviousFrame = DateTime.Now;
		Action<Bitmap> EachFrame; // = (b) => { b.Save(string.Format(@"C:\frames\{0}.png",FrameI++),ImageFormat.Png); };
		protected override void OnPaint( PaintEventArgs e ) {
			var now = (EachFrame==null) ? DateTime.Now : PreviousFrame.AddSeconds(1.0/60); // change logical framerate to 60fps if frame capturing

			var fx = e.Graphics;
			fx.Clear( BackColor );

			var framestart = DateTime.Now;

			if ( Info == null )
			{
				TextRenderer.DrawText( fx, "No polygon selected.", Font, ClientRectangle, ForeColor, Color.Transparent, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter );
			}
			else
			using ( var pointpen   = new Pen( Color.White,  1.0f ) )
			using ( var outlinepen = new Pen( Color.Blue ,  3.0f ) )
			using ( var holepen    = new Pen( Color.Red  ,  3.0f ) )
			using ( var tripen     = new Pen( Color.Green,  1.0f ) )
			using ( var poeepen    = new Pen( Color.Red  , 10.0f ) )
			using ( var boblinepen = new Pen( Color.Blue , 10.0f ) )
			{
				foreach ( var pen in new[] { pointpen, outlinepen, holepen, tripen, poeepen, boblinepen } ) {
					pen.StartCap = pen.EndCap = LineCap.Round;
					pen.LineJoin = LineJoin.Round;
				}

				Info.Triangulate();

				fx.SmoothingMode = SmoothingMode.AntiAlias;
				fx.TranslateTransform(ClientSize.Width/2,ClientSize.Height/2);

				float bounce = (float)((now-PointBounceStart).TotalSeconds*5);
				if ( bounce > 1 ) {
					bounce = 0;
					PointBounceStart = now;
					++PointBounceIndex;
				}

				float xmin = float.MaxValue, xmax = float.MinValue;
				float ymin = float.MaxValue, ymax = float.MinValue;

				if ( Info.Polygon.Points != null ) foreach ( var point in Info.Polygon.Points ) {
					xmin = Math.Min(xmin,point.Xf);
					xmax = Math.Max(xmax,point.Xf);
					ymin = Math.Min(ymin,point.Yf);
					ymax = Math.Max(ymax,point.Yf);
				}

				float zoom = 0.8f * Math.Min(ClientSize.Width/(xmax-xmin),ClientSize.Height/(ymax-ymin));
				float xmid = (xmin+xmax)/2;
				float ymid = (ymin+ymax)/2;

				Func<TriangulationPoint,PointF> f = (p) => new PointF( (p.Xf-xmid)*zoom, (p.Yf-ymid)*zoom );

				if ( Info.Polygon.Points != null ) {
					PointBounceIndex %= Info.Polygon.Points.Count;
					fx.DrawPolygon( outlinepen, Info.Polygon.Points.Select(f).ToArray() );

					if ( Info.Polygon.Holes != null ) foreach ( var hole in Info.Polygon.Holes ) fx.DrawPolygon( holepen, hole.Points.Select(f).ToArray() );

					boblinepen.Width = 3.0f + 9-9*bounce;
					fx.DrawLine( boblinepen, f(Info.Polygon.Points[(PointBounceIndex+Info.Polygon.Points.Count-1)%Info.Polygon.Points.Count]), f(Info.Polygon.Points[PointBounceIndex]) );
				}

				if ( Info.Polygon.Triangles != null )
				foreach ( var tri in Info.Polygon.Triangles )
				{
					fx.DrawPolygon( tripen, tri.Points.Select(f).ToArray() );
				}

				if ( Info.Polygon.Points != null ) {
					foreach ( var poly in new[]{Info.Polygon}.Concat(Info.Polygon.Holes ?? new Polygon[]{}) )
					for ( int i = 0 ; i < poly.Points.Count ; ++i )
					{
						var point = f(poly.Points[i]);
						float r = 2.0f;
						if ( PointBounceIndex==i ) r += 2-2*bounce;
						fx.DrawEllipse( pointpen, point.X-r, point.Y-r, 2*r, 2*r );
					}
				}

				var poee = Info.LastTriangulationException as PointOnEdgeException;
				if ( poee != null ) {
					var line = new PointF[] { f(poee.A), f(poee.B), f(poee.C) };
					fx.DrawLines( poeepen, line );
					foreach ( var p in line ) fx.DrawEllipse( poeepen, p.X-2, p.Y-2, 4, 4 );
				}

				var polyrenderend = DateTime.Now; // not counting all the text processing
				fx.ResetTransform();
				LineY=10;
				AddText(fx, "{0}    Points: {1}    Triangles: {2}"
					, Info.Name
					, (Info.Polygon.Points   ==null ? 0 : Info.Polygon.Points.Count)
					, (Info.Polygon.Triangles==null ? 0 : Info.Polygon.Triangles.Count)
					);
				AddText(fx," ");

				AddText(fx,"Memory: "+(GC.GetTotalMemory(false)/1000000).ToString("N0")+"MB");
				string s = "Collections    ";
				for ( int i=0, g=GC.MaxGeneration ; i < g ; ++i ) s = s + "    Gen"+i+": "+GC.CollectionCount(i);
				AddText(fx,s);

				AddText(fx,"Time    Triangulation: {0}ms    Render: {1}ms"
					, Info.LastTriangulationDuration.TotalMilliseconds.ToString("N0")
					, (polyrenderend-framestart).TotalMilliseconds.ToString("N0")
					);
				AddText(fx,Info.LastTriangulationException);
				
				fx.InterpolationMode = InterpolationMode.High;
				float size = Math.Min( 256.0f, Math.Min( ClientSize.Width, ClientSize.Height )/8.0f );
				float pad = 10.0f;
				fx.DrawImage( ExampleData.Logo256x256, ClientSize.Width-size-pad, pad, size, size );
			}

			if ( EachFrame==null ) {
				PreviousFrame = now;
			} else if ( PreviousFrame < now ) {
				if ( FrameCache == null || FrameCache.Size != Size ) {
					using ( FrameCache ) {}
					FrameCache = new Bitmap( Size.Width, Size.Height, PixelFormat.Format24bppRgb );
				}

				using ( var g = Graphics.FromImage(FrameCache) ) g.CopyFromScreen( Location, Point.Empty, Size );

				while ( PreviousFrame < now ) {
					EachFrame(FrameCache);
					PreviousFrame = PreviousFrame.AddSeconds(1.0/60);
				}
			}

			Invalidate();
			base.OnPaint(e);
		}

		[STAThread] public static void Main() {
			Application.Run( new DebugForm() );
		}
	}
}
