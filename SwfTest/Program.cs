using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Poly2Tri;
using System.Drawing;
using System.IO;

namespace SwfTest {
	[System.ComponentModel.DesignerCategory("")]
	class TestForm : Form {
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

			Monkey = new Polygon(new ArrayList<PolygonPoint>(monkeydata.Select(p => new PolygonPoint(p.X, p.Y))));
			Poly2Tri.Poly2Tri.triangulate(Monkey);
		}

		protected override void OnPaint(PaintEventArgs e) {
			var fx = e.Graphics;
			fx.TranslateTransform(500, 500);
			fx.ScaleTransform(1, -1);
			foreach ( var tri in Monkey.Triangles ) {
				fx.DrawLines(Pens.Green, new PointF[]
					{ new PointF(5*tri.points[0].getXf(),5*tri.points[0].getYf())
					, new PointF(5*tri.points[1].getXf(),5*tri.points[1].getYf())
					, new PointF(5*tri.points[2].getXf(),5*tri.points[2].getYf())
					, new PointF(5*tri.points[0].getXf(),5*tri.points[0].getYf())
					});
			}
			base.OnPaint(e);
		}
	}

	static class Program {
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() {
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new TestForm());
		}
	}
}
