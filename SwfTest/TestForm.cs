using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Poly2Tri;

namespace SwfTest {
	[System.ComponentModel.DesignerCategory("")] class TestForm : Form {
		Polygon Monkey;

		public TestForm() {
			ClientSize = new Size(1000, 1000);
			DoubleBuffered = true;
			Text = "Just a test";
			Visible = true;
			Invalidate();

			var monkeydata = new List<PointF>();
			foreach (var line in File.ReadAllLines(@"..\..\nazca_monkey.dat")) {
				var xy = line.Split(' ');
				monkeydata.Add(new PointF(float.Parse(xy[0]), float.Parse(xy[1])));
			}

			Monkey = new Polygon(monkeydata.Select(p => new PolygonPoint(p.X, p.Y)));
			Poly2Tri.Poly2Tri.Triangulate(Monkey);
		}

		protected override void OnPaint( PaintEventArgs e ) {
			var fx = e.Graphics;
			fx.TranslateTransform(500, 500);
			fx.ScaleTransform(1, -1);
			foreach ( var tri in Monkey.Triangles ) fx.DrawPolygon(Pens.Green, new PointF[]
				{ new PointF(5*tri.Points[0].Xf,5*tri.Points[0].Yf)
				, new PointF(5*tri.Points[1].Xf,5*tri.Points[1].Yf)
				, new PointF(5*tri.Points[2].Xf,5*tri.Points[2].Yf)
				});
			base.OnPaint(e);
		}

		[STAThread] static void Main() {
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new TestForm());
		}
	}
}
