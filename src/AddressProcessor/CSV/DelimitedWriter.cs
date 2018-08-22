using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddressProcessing.CSV
{
	public class DelimitedWriter : IDisposable
	{
		private readonly char _separator;
		private readonly StreamWriter _streamWriter;

		public DelimitedWriter(string path) : this(path, false) { }
		public DelimitedWriter(string path, bool append) : this(path, append, '\t') { }
		public DelimitedWriter(string path, bool append, char separator)
		{
			var fileInfo = new FileInfo(path);
			_streamWriter = append ? fileInfo.AppendText() : fileInfo.CreateText();
			_separator = separator;
		}

		public void Dispose()
		{
			_streamWriter.Dispose();
		}

		public void Write(params string[] columns) => Write(columns.AsEnumerable());
		public void Write(IEnumerable<string> columns)
		{
			_streamWriter.WriteLine(String.Join(_separator.ToString(), columns));
		}
	}
}
