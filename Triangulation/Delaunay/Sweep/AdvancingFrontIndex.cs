using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poly2Tri {
	public class AdvancingFrontIndex<A> where A : class {
		//double _min, _max;
		IndexNode<A> _root;

		public AdvancingFrontIndex(double min, double max, int depth) {
			if (depth > 5) depth = 5;
			_root = createIndex(depth);
		}

		private IndexNode<A> createIndex(int n) {
			IndexNode<A> node = null;
			if (n > 0) {
				node = new IndexNode<A>();
				node.bigger = createIndex(n - 1);
				node.smaller = createIndex(n - 1);
			}
			return node;
		}

		public A fetchAndRemoveIndex(A key) {
			return null;
		}

		public A fetchAndInsertIndex(A key) {
			return null;
		}

		class IndexNode<B> {
			//public B value;
			public IndexNode<B> smaller;
			public IndexNode<B> bigger;
			//public double range;
		}
	}
}
