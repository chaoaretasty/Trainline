using System.IO;
using NUnit.Framework;
using AddressProcessing.CSV;
using System;
using System.Collections.Generic;

namespace Csv.Tests
{
    [TestFixture]
    public class DelimitedReaderTeests
    {
		private const string TestInputFileBlank = @"test_data\contacts.csv";
		private const string TestInputFileGap = @"test_data\contacts_gap.csv";
		private const string TestInputFileOneCol = @"test_data\contacts_onecol.csv";

		[Test]
		public void Reader_WithOut_Split()
		{
			IEnumerable<String> strings = null;
			bool success = false;
			using (var reader = new DelimitedReader(TestInputFileBlank))
			{
				success = reader.TryRead(out strings);
			}

			Assert.That(strings, Is.EquivalentTo( new[] { "Shelby Macias", "3027 Lorem St.|Kokomo|Hertfordshire|L9T 3D5|England", "1 66 890 3865-9584", "et@eratvolutpat.ca" }));
			Assert.That(success, Is.True);
		}

		[Test]
		public void Reader_ReturnVal_Split()
		{
			IEnumerable<String> strings = null;
			using (var reader = new DelimitedReader(TestInputFileBlank))
			{
				strings = reader.Read();
			}

			Assert.That(strings, Is.EquivalentTo(new[] { "Shelby Macias", "3027 Lorem St.|Kokomo|Hertfordshire|L9T 3D5|England", "1 66 890 3865-9584", "et@eratvolutpat.ca" }));
		}

		/* Testing the same file conditions as readerwriter but this should not include bugs */
		/* This is a bit overkill and in XUnit I would have used something like a Theory test to cover the various situations */
		#region Reading to end of file
		[Test]
		public void Reader_Count_WithGap()
		{
			var toEndReturnVal = CountLines(TestInputFileGap, false, false);
			var toGapReturnVal = CountLines(TestInputFileGap, false, true);
			var toEndOutVal = CountLines(TestInputFileGap, true, false);
			var toGapOutVal = CountLines(TestInputFileGap, true, true);

			Assert.That(new[] { toEndReturnVal.Item2, toGapReturnVal.Item2, toEndOutVal.Item2, toGapOutVal.Item2 }, Is.EquivalentTo(new object[] { null, null, null, null }));
			Assert.That(toEndReturnVal.Item1, Is.EqualTo(230));
			Assert.That(toGapReturnVal.Item1, Is.EqualTo(10));
			Assert.That(toEndOutVal.Item1, Is.EqualTo(230));
			Assert.That(toGapOutVal.Item1, Is.EqualTo(10));
		}

		[Test]
		public void Reader_Count_BlankEnd()
		{
			var toEndReturnVal = CountLines(TestInputFileBlank, false, false);
			var toGapReturnVal = CountLines(TestInputFileBlank, false, true);
			var toEndOutVal = CountLines(TestInputFileBlank, true, false);
			var toGapOutVal = CountLines(TestInputFileBlank, true, true);

			Assert.That(new[] { toEndReturnVal.Item2, toGapReturnVal.Item2, toEndOutVal.Item2, toGapOutVal.Item2 }, Is.EquivalentTo(new object[] { null, null, null, null }));
			Assert.That(toEndReturnVal.Item1, Is.EqualTo(229));
			Assert.That(toGapReturnVal.Item1, Is.EqualTo(229));
			Assert.That(toEndOutVal.Item1, Is.EqualTo(229));
			Assert.That(toGapOutVal.Item1, Is.EqualTo(229));
		}

		private Tuple<int, Exception> CountLines(string filename, bool withOutValues, bool endOnGap)
		{
			IEnumerable<string> strings = null;
			Exception ex = null;

			int lineCount = 0;

			using (var reader = new DelimitedReader(filename, endOnGap))
			{
				try
				{
					while (withOutValues ? reader.TryRead(out strings): (reader.Read() != null))
					{
						lineCount++;
					}
				}
				catch (Exception e)
				{
					ex = e;
				}
			}

			return new Tuple<int, Exception>(lineCount, ex);
		}
		#endregion
	}
}
