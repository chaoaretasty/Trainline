using System;
using System.Collections.Generic;
using System.IO;

namespace AddressProcessing.CSV
{
	public class DelimitedReader : IDelimitedReader
	{
		private readonly char[] _separator;
		private readonly StreamReader _streamReader;
		private readonly bool _endOnEmptyLine;

		// Previous class was callewd CSV... but worked on TSV data. This class can have custom delimiters and defaults to the behaviour of previous class
		// Previous version also was unclear on if it should end processing on a blank line or not due to causing exceptions and differences between the two Read variants, this can handle both
		public DelimitedReader(string path) : this(path, false) { }
		public DelimitedReader(string path, bool endOnEmptyLine) : this(path, endOnEmptyLine, new[] { '\t' }) { }
		public DelimitedReader(string path, bool endOnEmptyLine, params char[] separator)
		{
			_endOnEmptyLine = endOnEmptyLine;
			_streamReader = File.OpenText(path);
			_separator = separator;
		}

		//Implements Read and TryRead variants
		//Using IEnumerable rather than assuming it will only ever want/need 2 values per line as this class only cares about reading
		//Up to the caller to decide how they want that data
		public IEnumerable<string> Read()
		{
			var line = _streamReader.ReadLine();
			
			if( line == null || _endOnEmptyLine && line.Length == 0)
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

		public void Dispose() => _streamReader?.Dispose();
	}
}
