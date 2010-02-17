using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using boolean = System.Boolean;


namespace Poly2Tri {
public class DTSweepDebugContext : TriangulationDebugContext
{
    /*
     * Fields used for visual representation of current triangulation
     */
    protected DelaunayTriangle _primaryTriangle;
    protected DelaunayTriangle _secondaryTriangle;
    protected TriangulationPoint _activePoint;
    protected AdvancingFrontNode _activeNode;
    protected DTSweepConstraint _activeConstraint;   
        
    public DTSweepDebugContext( DTSweepContext tcx ): base( tcx ) {}
    
    public boolean isDebugContext()
    {
        return true;
    }

    //  private Tuple2<TriangulationPoint,Double> m_circumCircle = new Tuple2<TriangulationPoint,Double>( new TriangulationPoint(), new Double(0) );
//  public Tuple2<TriangulationPoint,Double> getCircumCircle() { return m_circumCircle; }
    public DelaunayTriangle getPrimaryTriangle()
    {
        return _primaryTriangle;
    }

    public DelaunayTriangle getSecondaryTriangle()
    {
        return _secondaryTriangle;
    }
    
    public AdvancingFrontNode getActiveNode()
    {
        return _activeNode;
    }

    public DTSweepConstraint getActiveConstraint()
    {
        return _activeConstraint;
    }

    public TriangulationPoint getActivePoint()
    {
        return _activePoint;
    }

    public void setPrimaryTriangle( DelaunayTriangle triangle )
    {
        _primaryTriangle = triangle;        
        _tcx.update("setPrimaryTriangle");
    }

    public void setSecondaryTriangle( DelaunayTriangle triangle )
    {
        _secondaryTriangle = triangle;        
        _tcx.update("setSecondaryTriangle");
    }
    
    public void setActivePoint( TriangulationPoint point )
    {
        _activePoint = point;        
    }

    public void setActiveConstraint( DTSweepConstraint e )
    {
        _activeConstraint = e;
        _tcx.update("setWorkingSegment");
    }

    public void setActiveNode( AdvancingFrontNode node )
    {
        _activeNode = node;        
        _tcx.update("setWorkingNode");
    }

    public override void clear()
    {
        _primaryTriangle = null;
        _secondaryTriangle = null;
        _activePoint = null;
        _activeNode = null;
        _activeConstraint = null;   
    }
        
//  public void setWorkingCircumCircle( TriangulationPoint point, TriangulationPoint point2, TriangulationPoint point3 )
//  {
//          double dx,dy;
//          
//          CircleXY.circumCenter( point, point2, point3, m_circumCircle.a );
//          dx = m_circumCircle.a.X-point.X;
//          dy = m_circumCircle.a.Y-point.Y;
//          m_circumCircle.b = Double.valueOf( Math.sqrt( dx*dx + dy*dy ) );
//          
//  }
}
}
