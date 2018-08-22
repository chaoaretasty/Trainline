using System.IO;
using NUnit.Framework;
using AddressProcessing.CSV;
using System;

namespace Csv.Tests
{
    [TestFixture]
    public class DelimitedWriterTests
    {
		private const string TestOutputFile = @"test_data\output.csv";
		private const string TestSeparatorOutputFile = @"test_data\output-separator.csv";
		private const string TestExistingOutputFile = @"test_data\output-existing.csv";

		[SetUp]
		public void Setup()
		{
			var output = new FileInfo(TestOutputFile);
			var separator = new FileInfo(TestSeparatorOutputFile);
			var existing = new FileInfo(TestExistingOutputFile);

			if (output.Exists)
			{
				output.Delete();
			}

			if (separator.Exists)
			{
				separator.Delete();
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
		public void Writer_NewFile()
		{
			using (var writer = new DelimitedWriter(TestOutputFile))
			{
				writer.Write("testing 1-1", "testing 1-2");
				writer.Write("testing 2-1", "testing 2-2", "testing 2-3");
			}
			
			var expected = "testing 1-1\ttesting 1-2\r\ntesting 2-1\ttesting 2-2\ttesting 2-3\r\n";

			string output;
			using (var outFile = File.OpenText(TestOutputFile))
			{
				output = outFile.ReadToEnd();
			}

			Assert.That(expected, Is.EqualTo(output));
		}

		[Test]
		public void Writer_CustomSeparator()
		{
			using (var writer = new DelimitedWriter(TestOutputFile, false, ','))
			{
				writer.Write("testing 1-1", "testing 1-2");
				writer.Write("testing 2-1", "testing 2-2", "testing 2-3");
			}

			var expected = "testing 1-1,testing 1-2\r\ntesting 2-1,testing 2-2,testing 2-3\r\n";

			string output;
			using (var outFile = File.OpenText(TestOutputFile))
			{
				output = outFile.ReadToEnd();
			}

			Assert.That(expected, Is.EqualTo(output));
		}

		[Test]
		public void Writer_ExistingFile()
		{
			using (var writer = new DelimitedWriter(TestExistingOutputFile, true))
			{
				writer.Write("testing 1-1", "testing 1-2");
				writer.Write("testing 2-1", "testing 2-2", "testing 2-3");
			}

			var expected = "Testing\tcontent\r\ntesting 1-1\ttesting 1-2\r\ntesting 2-1\ttesting 2-2\ttesting 2-3\r\n";

			string output;
			using (var outFile = File.OpenText(TestExistingOutputFile))
			{
				output = outFile.ReadToEnd();
			}
			Assert.That(expected, Is.EqualTo(output));
		}
	}
}
