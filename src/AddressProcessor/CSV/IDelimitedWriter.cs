using System;
using System.Collections.Generic;

namespace AddressProcessing.CSV
{
	public interface IDelimitedWriter : IDisposable
	{
		void Write(IEnumerable<string> columns);
		void Write(params string[] columns);
	}
}