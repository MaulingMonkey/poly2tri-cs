using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poly2Tri {
	public class ArrayList<T> : List<T> {
		public ArrayList() {}
		public ArrayList( int count ): base(count) {} // XXX: Fixme?
		public ArrayList( IEnumerable<T> list ): base(list) {}

		public int size() { return Count; }
		public T get( int index ) { return this[index]; }
		public void remove( int index ) { RemoveAt(index); }
		public void remove( T element ) { Remove(element); }
		public void addAll( IEnumerable<T> list ) { AddRange(list); }
		public void addAll<U>( IEnumerable<U> list ) where U : T {
			foreach ( var item in list ) Add(item);
		}
		public void add( T element ) { Add(element); }
		public void add( int index, T element ) { Insert(index,element); }
		public int indexOf( T element ) { return IndexOf(element); }
		public void clear() { Clear(); }
	}
	public class RuntimeException : InvalidOperationException {
		public RuntimeException( string message ): base(message) {}
	}
}
