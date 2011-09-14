import org.junit.Test;

import com.vividsolutions.jts.geom.Coordinate;
import com.vividsolutions.jts.geom.Geometry;
import com.vividsolutions.jts.geom.GeometryFactory;
import com.vividsolutions.jts.geom.LineString;
import com.vividsolutions.jts.geom.MultiLineString;
import com.vividsolutions.jts.geom.PrecisionModel;
import com.vividsolutions.jts.io.ParseException;
import com.vividsolutions.jts.io.WKTReader;

import junit.framework.Assert;
import junit.framework.TestCase;

public class ReverseTest extends TestCase {
	
	@Test
	public void testMultiLineStringReverse() throws ParseException {
		GeometryFactory factory = new GeometryFactory(new PrecisionModel(PrecisionModel.FLOATING));		
		
		LineString lineString1 = factory.createLineString(new Coordinate[] 
		                                                                 { 
		                                                                 	new Coordinate(10, 10), 
		                                                                    new Coordinate(20, 20), 
		                                                                    new Coordinate(20, 30), 
		                                                                 });
		LineString lineString2 = factory.createLineString(new Coordinate[] 
		                                                                 { 
																			new Coordinate(12, 12), 
																			new Coordinate(24, 24), 
																			new Coordinate(24, 36), 
		                                                                 });
       MultiLineString multiLineString = factory.createMultiLineString(new LineString[] { lineString1, lineString2, });
       MultiLineString reverse = (MultiLineString)multiLineString.reverse();

       Assert.assertTrue(multiLineString.equals(reverse));
       Assert.assertFalse(multiLineString.equalsExact(reverse));

       Geometry result2 = reverse.getGeometryN(1);
       Assert.assertTrue(lineString1.equals(result2));
       Assert.assertFalse(lineString1.equalsExact(result2));

       Geometry result1 = reverse.getGeometryN(0);
       Assert.assertTrue(lineString2.equals(result1));
       Assert.assertFalse(lineString2.equalsExact(result1));
	}
}
