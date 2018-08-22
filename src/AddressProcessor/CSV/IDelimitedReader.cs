using System;
using System.Collections.Generic;

namespace AddressProcessing.CSV
{
	public interface IDelimitedReader : IDisposable
	{
		IEnumerable<string> Read();
		bool TryRead(out IEnumerable<string> values);
	}
}