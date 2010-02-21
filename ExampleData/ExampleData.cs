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
using System.IO;

namespace Poly2Tri {
	public static class ExampleData {
		public static List<PolygonPoint> LoadDat( string filename, bool xflip, bool yflip ) {
			var points = new List<PolygonPoint>();
			foreach ( var line_ in File.ReadAllLines(filename) ) {
				string line = line_.Trim();
				if ( string.IsNullOrEmpty(line) ) continue;
				var xy = line.Split( new[]{' ',','}, StringSplitOptions.RemoveEmptyEntries );
				points.Add( new PolygonPoint( (xflip?-1:+1) * double.Parse(xy[0]), (yflip?-1:+1) * double.Parse(xy[1]) ) );
			}
			return points;
		}

		public static List<PolygonPoint> LoadDat( string filename ) { return LoadDat(filename,false,false); }

		static readonly Dictionary<string,List<PolygonPoint>> DatCache = new Dictionary<string,List<PolygonPoint>>();

		static List<PolygonPoint> CacheLoadDat( string filename, bool xflip, bool yflip ) {
			if (!DatCache.ContainsKey(filename)) DatCache.Add( filename, LoadDat(filename,xflip,yflip) );
			return DatCache[filename];
		}

		static List<PolygonPoint> CacheLoadDat( string filename ) { return CacheLoadDat(filename,false,false); }

		static readonly Dictionary<string,Image> ImageCache = new Dictionary<string,Image>();

		static Image CacheLoadImage( string filename ) {
			if (!ImageCache.ContainsKey(filename)) ImageCache.Add( filename, new Bitmap(filename) );
			return ImageCache[filename];
		}

		// These should all use +x = right, +y = up
		public static List<PolygonPoint> Two         { get { return CacheLoadDat(@"Data\2.dat"); } }
		public static List<PolygonPoint> Bird        { get { return CacheLoadDat(@"Data\bird.dat",false,true); } }
		public static List<PolygonPoint> Custom      { get { return CacheLoadDat(@"Data\custom.dat"); } }
		public static List<PolygonPoint> Debug       { get { return CacheLoadDat(@"Data\debug.dat"); } }
		public static List<PolygonPoint> Debug2      { get { return CacheLoadDat(@"Data\debug2.dat"); } }
		public static List<PolygonPoint> Diamond     { get { return CacheLoadDat(@"Data\diamond.dat"); } }
		public static List<PolygonPoint> Dude        { get { return CacheLoadDat(@"Data\dude.dat"); } }
		public static List<PolygonPoint> Funny       { get { return CacheLoadDat(@"Data\funny.dat"); } }
		public static List<PolygonPoint> NazcaHeron  { get { return CacheLoadDat(@"Data\nazca_heron.dat"); } }
		public static List<PolygonPoint> NazcaMonkey { get { return CacheLoadDat(@"Data\nazca_monkey.dat",false,true); } }
		public static List<PolygonPoint> Sketchup    { get { return CacheLoadDat(@"Data\sketchup.dat"); } }
		public static List<PolygonPoint> Star        { get { return CacheLoadDat(@"Data\star.dat"); } }
		public static List<PolygonPoint> Strange     { get { return CacheLoadDat(@"Data\strange.dat"); } }
		public static List<PolygonPoint> Tank        { get { return CacheLoadDat(@"Data\tank.dat"); } }
		public static List<PolygonPoint> Test        { get { return CacheLoadDat(@"Data\test.dat"); } }

		public static IEnumerable<List<PolygonPoint>> Polygons { get { return new[] { Two, Bird, Custom, Debug, Debug2, Diamond, Dude, Funny, NazcaHeron, NazcaMonkey, Sketchup, Star, Strange, Tank, Test }; }}

		public static Image Logo256x256 { get { return CacheLoadImage(@"Textures\poly2tri_logotype_256x256.png"); } }
	}
}
