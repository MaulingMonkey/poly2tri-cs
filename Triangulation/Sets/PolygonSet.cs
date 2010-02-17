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
namespace Poly2Tri {
public class TriangulationPolygonSet : ConstrainedPointSet
{
    ArrayList<TriangulationPoint> _steinerPoints;
    
    //public TriangulationPolygonSet( Polygon poly ): base( poly.getPoints(), poly.getIndex() ) {}
	public TriangulationPolygonSet( Polygon poly ): base(null,null) { throw new NotImplementedException(); }

    public void addSteinerPoint( TriangulationPoint point )
    {
        if( _steinerPoints == null )
        {
            _steinerPoints = new ArrayList<TriangulationPoint>();
        }
        _steinerPoints.add( point );        
    }
    
    public int pointCount()
    {
        int count = _points.size();
        if( _steinerPoints != null )
        {
            count += _steinerPoints.size();
        }
        return count;
    }
    /**
     * Setup edges according to the index array where every pair of 
     * index values form a separate edge
     * @param tcx
     * @param index
     */
    protected void initEdges( int[] index )
    {
        if( index != null )
        {
            //base.initEdges( index );
			throw new NotImplementedException( "I have no idea what super.initEdges was supposed to call but I know it ain't here!" );
        }
        else
        {            
            for( int i = 0; i < _points.size()-1 ; i++ )
            {
//                try
//                {
                    new DTSweepConstraint( _points.get( i ) , _points.get( i+1 ) );
//                }
//                catch( DuplicatePointException e )
//                {
//                    m_points.remove( i+1 );
//                    i--;
//                }
            }
            // Connect endpoints
//            try
//            {
                new DTSweepConstraint( _points.get( 0 ) , _points.get( _points.size()-1 ) );
//            }
//            catch( DuplicatePointException e )
//            {
//                throw new RuntimeException( "[Not Supported] First and last point in polygon is same");
//            }
        }
    }

    public void populate( ArrayList<TriangulationPoint> points )
    {
		throw new NotImplementedException("Uhh... there is no base.populate()");
        //base.populate( points );
		//if( _steinerPoints != null )
		//{
		//    points.addAll( _steinerPoints );
		//}
    }

}
}