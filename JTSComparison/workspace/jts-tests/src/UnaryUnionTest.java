import junit.framework.TestCase;

import com.vividsolutions.jts.geom.Geometry;
import com.vividsolutions.jts.geom.GeometryFactory;
import com.vividsolutions.jts.geom.PrecisionModel;
import com.vividsolutions.jts.io.ParseException;
import com.vividsolutions.jts.io.WKTReader;
import com.vividsolutions.jts.operation.union.UnaryUnionOp;

public class UnaryUnionTest extends TestCase {
	public void testMultiPolygonInvalidWithTopologyCollapse()
			throws ParseException {
		GeometryFactory factory = new GeometryFactory(new PrecisionModel(
				PrecisionModel.FIXED));
		WKTReader reader = new WKTReader(factory);
		Geometry expected = reader
				.read("GEOMETRYCOLLECTION (LINESTRING (0 0, 20 0), POLYGON ((150 0, 20 0, 20 100, 180 100, 180 0, 150 0)))");
		Geometry geom = reader
				.read("MULTIPOLYGON (((0 0, 150 0, 150 1, 0 0)), ((180 0, 20 0, 20 100, 180 100, 180 0)))");
		Geometry result = UnaryUnionOp.union(geom);
		assertTrue(expected.equalsExact(result));
	}
}