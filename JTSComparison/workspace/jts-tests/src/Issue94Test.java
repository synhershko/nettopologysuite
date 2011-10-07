import com.vividsolutions.jts.geom.Coordinate;
import com.vividsolutions.jts.geom.Geometry;
import com.vividsolutions.jts.geom.GeometryFactory;
import com.vividsolutions.jts.geom.LineString;
import com.vividsolutions.jts.geom.PrecisionModel;

import junit.framework.Assert;
import junit.framework.TestCase;


public class Issue94Test extends TestCase 
{
	  public void testIntersectionWithLineCreatedWithLargeCoordinates()
      {
		  performTest(100d);
          performTest(99999999999999982650000000000d);   
          performTest(Double.MAX_VALUE);
      }

	  private static void performTest(double value) 
      {
          GeometryFactory factory = new GeometryFactory(new PrecisionModel(PrecisionModel.FLOATING));
          LineString ls1 = factory.createLineString(new Coordinate[] { new Coordinate(0, 0), new Coordinate(50, 50) });            
          LineString ls2 = factory.createLineString(new Coordinate[] { new Coordinate(10, value), new Coordinate(10, -value) });
          Geometry result = ls1.intersection(ls2);
          Geometry expected = factory.createPoint(new Coordinate(10, 10));
          Assert.assertTrue(result.equalsExact(expected));
      }
}
