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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Poly2Tri {
	[System.ComponentModel.DesignerCategory("")] class DebugForm : Form {
		PolygonInfo Info = new PolygonInfo( "Bird", new Polygon(ExampleData.Bird) );

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

		protected override void OnKeyDown( KeyEventArgs e ) {
			e.Handled = true;
			switch ( e.KeyCode ) {
			case Keys.G: GC.GetTotalMemory(true); break;
			default:
				e.Handled = false;
				base.OnKeyDown(e);
				break;
			}
		}

		protected override void OnPaint( PaintEventArgs e ) {
			var fx = e.Graphics;
			fx.Clear( BackColor );

			if ( Info == null ) {
				TextRenderer.DrawText( fx, "No polygon selected.", Font, ClientRectangle, ForeColor, Color.Transparent, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter );
			}
			else
			using ( var pointpen = new Pen( Color.White, 1.0f ) )
			{
				var now = DateTime.Now;

				Info.Triangulate();

				fx.SmoothingMode = SmoothingMode.AntiAlias;
				fx.TranslateTransform(400,300);

				double bounce = (now-PointBounceStart).TotalSeconds*5;
				if ( bounce > 1 ) {
					bounce = 0;
					PointBounceStart = now;
					++PointBounceIndex;
				}

				PointBounceIndex %= Info.Polygon.Points.Count;

				for ( int i = 0 ; i < Info.Polygon.Points.Count ; ++i ) {
					var point = Info.Polygon.Points[i];
					float r = 2.0f;
					if ( PointBounceIndex==i ) r += (float)(2-2*bounce);
					fx.DrawEllipse( pointpen, 20*point.Xf-r, 20*point.Yf-r, 2*r, 2*r );
				}

				LineY=10;
				AddText(fx,"Name: "+Info.Name);
				AddText(fx,"Triangulation time: "+Info.LastTriangulationDuration.TotalMilliseconds.ToString("N0")+"ms");
				AddText(fx,"Points: "+Info.Polygon.Points.Count);
				AddText(fx,"Triangles: "+Info.Polygon.Triangles.Count);
				AddText(fx,"Memory: "+(GC.GetTotalMemory(false)/1000000).ToString("N0")+"MB");
				string s = "Collections: ";
				for ( int i=0, g=GC.MaxGeneration ; i < g ; ++i ) s = s + "    Gen "+i+": "+GC.CollectionCount(i);
				AddText(fx,s);
			}

			Invalidate();
			base.OnPaint(e);
		}

		[STAThread] public static void Main() {
			Application.Run( new DebugForm() );
		}
	}
}
