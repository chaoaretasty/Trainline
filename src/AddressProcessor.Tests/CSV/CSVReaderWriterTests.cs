using System.IO;
using NUnit.Framework;
using AddressProcessing.CSV;
using System;

namespace Csv.Tests
{
    [TestFixture]
    public class CSVReaderWriterTests
    {
		//Tests 
		//Read with ref
		//Read without ref
		//Read needs to stop either at end of file or where filesplit == 0
		//Write - Needs to check write append vs overwrite function

		/* I'm not familiar with NUnit as I've previously used XUnit so apologies if there are better ways of doing some things than I've managed here */

		private const string TestInputFileBlank = @"test_data\contacts.csv";
		private const string TestInputFileNoBlank = @"test_data\contacts_noblank.csv";
		private const string TestInputFileGap = @"test_data\contacts_gap.csv";
		private const string TestInputFileOneCol = @"test_data\contacts_onecol.csv";
		private const string TestOutputFile = @"test_data\output.csv";
		private const string TestExistingOutputFile = @"test_data\output-existing.csv";

		[SetUp]
		public void Setup()
		{
			var output = new FileInfo(TestOutputFile);
			var existing = new FileInfo(TestExistingOutputFile);

			if (output.Exists)
			{
				output.Delete();
			}

			if (existing.Exists)
			{
				existing.Delete();
			}

			using (var setupOutput = existing.CreateText())
			{
				setupOutput.WriteLine("Testing\tcontent");
			}
		}

		[Test]
		public void Read_WithOut_Split()
		{
			var csvReader = new CSVReaderWriter();
			string string1 = null;
			string string2 = null;
			csvReader.Open(TestInputFileBlank, CSVReaderWriter.Mode.Read);

			csvReader.Read(out string1, out string2);
			csvReader.Close();
			Assert.That(string1, Is.EqualTo("Shelby Macias"));
			Assert.That(string2, Is.EqualTo("3027 Lorem St.|Kokomo|Hertfordshire|L9T 3D5|England"));
		}

		/* As this is in production and needs to be backwards compatible we need to ensure that existing bugs are replicated */
		#region Reading to end of file
		[Test]
		public void Read_Lines_WithOut_WithGap()
		{
			var result = CountLines(TestInputFileGap, true);

			Assert.That(result.Item1, Is.EqualTo(10));
			Assert.That(result.Item2, Is.TypeOf<IndexOutOfRangeException>());
		}

		[Test]
		public void Read_Lines_WithOut_OneCol()
		{
			var result = CountLines(TestInputFileOneCol, true);

			Assert.That(result.Item1, Is.EqualTo(0));
			Assert.That(result.Item2, Is.TypeOf<IndexOutOfRangeException>());
		}

		[Test]
		public void Read_Lines_WithOut_BlankEnd()
		{
			var result = CountLines(TestInputFileBlank, true);
			Assert.That(result.Item1, Is.EqualTo(229));
			Assert.That(result.Item2, Is.Null);
		}

		[Test]
		public void Read_Lines_WithOut_NoBlankEnd()
		{
			var result = CountLines(TestInputFileNoBlank, true);
			Assert.That(result.Item1, Is.EqualTo(229));
			Assert.That(result.Item2, Is.Null);
		}

		[Test]
		public void Read_Lines_WithNoOut_WithGap()
		{
			var result = CountLines(TestInputFileGap, false);

			Assert.That(result.Item1, Is.EqualTo(10));
			Assert.That(result.Item2, Is.TypeOf<IndexOutOfRangeException>());
		}

		[Test]
		public void Read_Lines_WithNoOut_OneCol()
		{
			var result = CountLines(TestInputFileOneCol, false);

			Assert.That(result.Item1, Is.EqualTo(0));
			Assert.That(result.Item2, Is.TypeOf<IndexOutOfRangeException>());
		}

		[Test]
		public void Read_Lines_WithNoOut_BlankEnd()
		{
			var result = CountLines(TestInputFileBlank, false);

			Assert.That(result.Item1, Is.EqualTo(229));
			Assert.That(result.Item2, Is.TypeOf<NullReferenceException>());
		}

		[Test]
		public void Read_Lines_WithNoOut_NoBlankEnd()
		{
			var result = CountLines(TestInputFileNoBlank, false);

			Assert.That(result.Item1, Is.EqualTo(229));
			Assert.That(result.Item2, Is.TypeOf<NullReferenceException>());
		}

		private Tuple<int, Exception> CountLines(string filename, bool withOutValues)
		{
			var csvReader = new CSVReaderWriter();
			string string1 = null;
			string string2 = null;
			Exception ex = null;

			csvReader.Open(filename, CSVReaderWriter.Mode.Read);
			int lineCount = 0;

			try
			{
				while (withOutValues ? csvReader.Read(out string1, out string2) : csvReader.Read(string1, string2))
				{
					lineCount++;
				}
			}
			catch (Exception e)
			{
				ex = e;
			}

			csvReader.Close();

			return new Tuple<int, Exception>(lineCount, ex);
		}
		
		#endregion

		[Test]
		public void Read_WithOut_EmptyLine()
		{
			var csvReader = new CSVReaderWriter();
			string string1 = null;
			string string2 = null;

			csvReader.Open(TestInputFileBlank, CSVReaderWriter.Mode.Read);
			int lineCount = 0;

			while (csvReader.Read(out string1, out string2))
			{
				lineCount++;
			}

			csvReader.Close();

			Assert.That(lineCount, Is.EqualTo(229));
		}

		[Test]
		public void Write_NewFile()
		{
			var csvWriter = new CSVReaderWriter();
			csvWriter.Open(TestOutputFile, CSVReaderWriter.Mode.Write);

			csvWriter.Write("testing 1-1", "testing 1-2");
			csvWriter.Write("testing 2-1", "testing 2-2", "testing 2-3");

			csvWriter.Close();

			var expected = "testing 1-1\ttesting 1-2\r\ntesting 2-1\ttesting 2-2\ttesting 2-3\r\n";

			string output;
			using (var outFile = File.OpenText(TestOutputFile))
			{
				output = outFile.ReadToEnd();
			}

			Assert.That(expected, Is.EqualTo(output));
		}

		[Test]
		public void Write_ExistingFile()
		{
			var csvWriter = new CSVReaderWriter();
			csvWriter.Open(TestExistingOutputFile, CSVReaderWriter.Mode.Write);

			csvWriter.Write("testing 1-1", "testing 1-2");
			csvWriter.Write("testing 2-1", "testing 2-2", "testing 2-3");

			csvWriter.Close();

			var expected = "testing 1-1\ttesting 1-2\r\ntesting 2-1\ttesting 2-2\ttesting 2-3\r\n";

			string output;
			using (var outFile = File.OpenText(TestExistingOutputFile))
			{
				output = outFile.ReadToEnd();
			}
			Assert.That(expected, Is.EqualTo(output));
		}
	}
}
