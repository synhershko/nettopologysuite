import junit.framework.TestCase;

import com.vividsolutions.jts.geom.CoordinateSequence;
import com.vividsolutions.jts.geom.Geometry;
import com.vividsolutions.jts.geom.GeometryFactory;
import com.vividsolutions.jts.geom.Point;
import com.vividsolutions.jts.geom.PrecisionModel;
import com.vividsolutions.jts.geom.impl.CoordinateArraySequence;

public class GeometriesTest extends TestCase {

	public void test_get_interior_point_of_empty_point() {
		CoordinateSequence sequence = new CoordinateArraySequence(0);
		GeometryFactory factory = new GeometryFactory(new PrecisionModel(
				PrecisionModel.FLOATING));
		Geometry empty = factory.createPoint(sequence);
		assertNotNull(empty);
		assertTrue(empty.isValid());
		assertTrue(empty.isEmpty());
		Point interior = empty.getInteriorPoint();
		assertNotNull(interior);
		assertTrue(interior.isValid());
		assertTrue(interior.isEmpty());
	}
}