using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AddressProcessing.CSV
{
    public class CSVReader : IReader
    {
        private StreamReader _reader;
        private readonly char _delimeter;

        //enabled default delimeter, but can also be specified
        public CSVReader(StreamReader reader, char delimeter = '\t')
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }
            _delimeter = delimeter;
            _reader = reader;
        }

        public string ReadLine()
        {
            return _reader.ReadLine();
        }

        public virtual bool ReadLineColumnValues(out IList<string> columns)
        {
            var line = ReadLine();
            if (!string.IsNullOrEmpty(line))
            {
                columns = line.Split(_delimeter);
                return columns.Any();
            }
            columns = null;
            return false;
        }

        public void Dispose()
        {
            if (_reader == null) return;
            _reader.Close();
            _reader = null;
        }
    }
}