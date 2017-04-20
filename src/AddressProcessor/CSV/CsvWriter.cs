using System;
using System.IO;
using System.Linq;

namespace AddressProcessing.CSV
{
    public class CSVWriter : IWriter
    {
        private StreamWriter _writer;
        private readonly char _delimeter;

        //enabled default delimeter, but can also be specified
        public CSVWriter(StreamWriter writer, char delimeter = '\t')
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }
            _delimeter = delimeter;
            _writer = writer;
        }

        public void Dispose()
        {
            if (_writer == null) return;
            _writer.Close();
            _writer = null;
       }

        public virtual void WriteLine(params string[] columns)
        {
            var line = string.Join(_delimeter.ToString(), columns.ToArray());
            _writer.WriteLine(line);
        }
    }
}
