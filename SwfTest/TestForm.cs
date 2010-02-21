using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Poly2Tri;

namespace SwfTest {
	[System.ComponentModel.DesignerCategory("")] class TestForm : Form {
		List<Polygon> Polygons;

		int i;

		Timer rotation;

		public TestForm() {
			ClientSize = new Size(1000, 1000);
			DoubleBuffered = true;
			Text = "Just a test";
			Visible = true;

			Polygons = new List<Polygon>( ExampleData.Polygons.Select( data => new Polygon(data) ) );
			foreach ( var poly in Polygons ) P2T.Triangulate(poly);

			rotation = new Timer()
				{ Enabled = true
				, Interval = 5000
				};
			rotation.Tick += (o,e) => {
				i = (i+1)%Polygons.Count;
				Invalidate();
			};

			Invalidate();
		}

		protected override void OnPaint( PaintEventArgs e ) {
			float xmin = float.MaxValue, xmax = float.MinValue;
			float ymin = float.MaxValue, ymax = float.MinValue;

			foreach ( var point in Polygons[i].Points ) {
				xmin = Math.Min(xmin,point.Xf);
				xmax = Math.Max(xmax,point.Xf);
				ymin = Math.Min(ymin,point.Yf);
				ymax = Math.Max(ymax,point.Yf);
			}

			if ( xmin<xmax && ymin<ymax ) {
				var fx = e.Graphics;
				float zoom = 0.8f * Math.Min(ClientSize.Width/(xmax-xmin),ClientSize.Height/(ymax-ymin));
				fx.TranslateTransform(ClientSize.Width/2, ClientSize.Height/2); // center coordinate system on screen center
				fx.ScaleTransform(zoom, -zoom);
				fx.TranslateTransform(-(xmax+xmin)/2,-(ymax+ymin)/2); // center image

				using ( var pen = new Pen( Color.Green, 1.0f/zoom ) ) foreach ( var tri in Polygons[i].Triangles ) fx.DrawPolygon(pen, new PointF[]
					{ new PointF(tri.Points[0].Xf,tri.Points[0].Yf)
					, new PointF(tri.Points[1].Xf,tri.Points[1].Yf)
					, new PointF(tri.Points[2].Xf,tri.Points[2].Yf)
					});
				fx.ResetTransform();
				fx.DrawImage( ExampleData.Logo256x256, ClientSize.Width-256-10, 10, 256, 256 );
			}

			base.OnPaint(e);
		}

		protected override void OnResize( EventArgs e ) {
			Invalidate();
			base.OnResize(e);
		}

		[STAThread] static void Main() {
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new TestForm());
		}
	}
}
