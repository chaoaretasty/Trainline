using System;
using System.Collections.Generic;
using System.IO;

namespace AddressProcessing.CSV
{
	public class DelimitedReader : IDisposable
	{
		private readonly char[] _separator;
		private readonly StreamReader _streamReader;
		private readonly bool _endOnEmptyLine;

		public DelimitedReader(string path) : this(path, true) { }
		public DelimitedReader(string path, bool endOnEmptyLine) : this(path, endOnEmptyLine, new[] { '\t' }) { }
		public DelimitedReader(string path, bool endOnEmptyLine, params char[] separator)
		{
			_endOnEmptyLine = endOnEmptyLine;
			_streamReader = File.OpenText(path);
			_separator = separator;
		}

		public IEnumerable<string> Read()
		{
			var line = _streamReader.ReadLine();
			
			if( line==null || _endOnEmptyLine && line.Length == 0)
			{
				return null;
			}

			return line.Split(_separator);
		}

		public bool TryRead(out IEnumerable<string> values)
		{
			values = Read();
			return values != null;
		}

		public void Dispose()
		{
			_streamReader?.Dispose();
		}
	}
}
