using System;
using System.Linq;
using System.IO;

namespace AddressProcessing.CSV
{
	/*
        2) Refactor this class into clean, elegant, rock-solid & well performing code, without over-engineering.
           Assume this code is in production and backwards compatibility must be maintained.
		   
		I am taking "Backwards compatible" to mean the same behaviour, even when there are better alternatives
		The assumption here is that this class is now a shim over old functionality which may depend on the buggy behaviour and future refactors would switch to using Delimited Reader and Writer when it can be checked safe to move to them
    */

	public class CSVReaderWriter : IDisposable
	{
		private IDelimitedReader _reader = null;
		private IDelimitedWriter _writer = null;
		// This is called a CSVReaderWriter but actually handles tab delimited data, assuming we can't rename the class we want to make it clear we are delimiting on tab
		// This could be implemented as a constructor variable with a default in situations if we were encouraging future use of this class
		private readonly char _separator = '\t';


		[Flags]
		public enum Mode { Read = 1, Write = 2 };

		public void Open(string fileName, Mode mode)
		{
			switch (mode)
			{
				case Mode.Read:
					_reader = new DelimitedReader(fileName, false, _separator);
					break;
				case Mode.Write:
					_writer = new DelimitedWriter(fileName, false, _separator);
					break;
				default:
					/* Ideally one of three changes should be made here but they would break backwards compatability:
					 * 1. Mode would not be flags
					 * 2. There'd be a case for where it is both read and write though that would be an odd choice on the same file but if there is a requirement that is well specced it is fine
					 * 3. Should throw an ArgumentException
					 */
					throw new Exception("Unknown file mode for " + fileName);
			}
		}

		public void Write(params string[] columns)
		{
			_writer.Write(columns);
		}

		public bool Read(string column1, string column2)
		{
			var success = _reader.TryRead(out var strings);

			// The below checks are to maintain behaviour of the previous version

			if (strings == null)
			{
				throw new NullReferenceException();
			}

			if (strings.Count() == 1 || strings.All(s => s == String.Empty))
			{
				throw new IndexOutOfRangeException();
			}

			return success;
		}

		public bool Read(out string column1, out string column2)
		{
			var success = _reader.TryRead(out var strings);

			var stringArray = (strings ?? new string[] { null, null }).ToArray();

			/*
			 * The previous version had the following 
			
			const int FIRST_COLUMN = 0;
            const int SECOND_COLUMN = 1;
			
			 * While this  is uneccessary, if the class was taking columns1 and 2 because they are known data 
			 * it could be acceptable to do const int NAME = 0, const int ADDRESS = 1 for explicitness, though I'd expect the out variables to also change name
			*/

			column1 = stringArray[0];
			column2 = stringArray[1];
			return success;
		}

		public void Close()
		{
			Dispose();
		}

		public void Dispose()
		{
			_reader?.Dispose();
			_writer?.Dispose();
		}
	}
}
