import java.util.ArrayList;

import junit.framework.TestCase;

import org.junit.Assert;
import org.junit.Test;

import com.vividsolutions.jts.geom.Geometry;
import com.vividsolutions.jts.geom.GeometryFactory;
import com.vividsolutions.jts.geom.PrecisionModel;
import com.vividsolutions.jts.io.WKTReader;
import com.vividsolutions.jts.operation.linemerge.LineSequencer;

public class LineSequencerTest extends TestCase {

	private static final WKTReader rdr = new WKTReader(new GeometryFactory(
			new PrecisionModel(PrecisionModel.FIXED)));

	@Test
	public void testSimpleLoop() {
		String[] wkt = { "LINESTRING ( 0 0, 0 10 )",
				"LINESTRING ( 0 10, 0 0 )", };
		String result = "MULTILINESTRING ((0 0, 0 10), (0 10, 0 0))";
		runLineSequencer(wkt, result);
	}

	@Test
	public void testTwoSimpleLoops() {
		String[] wkt = { "LINESTRING ( 0 0, 0 10 )",
				"LINESTRING ( 0 10, 0 0 )", "LINESTRING ( 0 0, 0 20 )",
				"LINESTRING ( 0 20, 0 0 )", };
		String result = "MULTILINESTRING ((0 10, 0 0), (0 0, 0 20), (0 20, 0 0), (0 0, 0 10))";
		runLineSequencer(wkt, result);
	}

	@Test
	public void testSimpleBigLoop() {
		String[] wkt = { "LINESTRING ( 0 0, 0 10 )",
				"LINESTRING ( 0 20, 0 30 )", "LINESTRING ( 0 30, 0 00 )",
				"LINESTRING ( 0 10, 0 20 )", };
		String result = "MULTILINESTRING ((0 0, 0 10), (0 10, 0 20), (0 20, 0 30), (0 30, 0 0))";
		runLineSequencer(wkt, result);
	}

	@Test
	public void testLineWithRing() {
		String[] wkt = { "LINESTRING ( 0 0, 0 10 )",
				"LINESTRING ( 0 10, 10 10, 10 20, 0 10 )",
				"LINESTRING ( 0 30, 0 20 )", "LINESTRING ( 0 20, 0 10 )", };
		String result = "MULTILINESTRING ((0 0, 0 10), (0 10, 10 10, 10 20, 0 10), (0 10, 0 20), (0 20, 0 30))";
		runLineSequencer(wkt, result);
	}

	private static void runLineSequencer(String[] inputWKT, String expectedWKT) {
		try {
			ArrayList<Geometry> inputGeoms = FromWKT(inputWKT);
			LineSequencer sequencer = new LineSequencer();
			sequencer.add(inputGeoms);

			if (!sequencer.isSequenceable())
				Assert.assertNotNull(expectedWKT);
			else {
				Geometry expected = rdr.read(expectedWKT);
				Geometry result = sequencer.getSequencedLineStrings();
				boolean isTrue = expected.equalsExact(result);
				Assert.assertTrue(isTrue);

				boolean isSequenced = LineSequencer.isSequenced(result);
				Assert.assertTrue("result is not sequenced", isSequenced);
			}
		} catch (Exception ex) {
			Assert.fail(ex.toString());
		}
	}

	private static ArrayList<Geometry> FromWKT(String[] wkts) {
		ArrayList<Geometry> geomList = new ArrayList<Geometry>();
		for (int i = 0; i < wkts.length; i++) {
			String wkt = wkts[i];
			try {
				geomList.add(rdr.read(wkt));
			} catch (Exception ex) {
				Assert.fail(ex.toString());
			}
		}
		return geomList;
	}
}
