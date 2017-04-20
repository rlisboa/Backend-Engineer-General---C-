using System;
using System.Collections.Generic;
using System.IO;

namespace AddressProcessing.CSV
{
    /*
        2) Refactor this class into clean, elegant, rock-solid & well performing code, without over-engineering.
           Assume this code is in production and backwards compatibility must be maintained.
    */

    //I decided not to havethis class to inherit and interface in order to be testable because in doing so I will be breaking the single responsibility principle. 
    //Howeevr what I have done is to split the responsibilities in seperate interfaces, IReader, IWriter with the option to inject them  when this class is being instatiated.
    //Also Alternativaly the existing methods of this class would still work for backward compatibility, i just ensure the new interfaces IReader, I writer are called withing the existing methods where necessary more of adapter design pattern.
    public class CSVReaderWriter : IDisposable
    {
        private IWriter _writer;
        private IReader _reader;
        private StreamReader _readerStream;
        private StreamWriter _writerStream;

        //Injecting IReader, IWritter
        public CSVReaderWriter(IReader reader, IWriter writer)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            if (writer == null) throw new ArgumentNullException(nameof(writer));
            _writer = writer;
            _reader = reader;
        }

        //MAintain Backwardcompatibility
        public CSVReaderWriter()
        {
        }

        //this shhould be moved to seperate class idealy but left it for now
        [Flags]
        public enum Mode { Read = 1, Write = 2 }

        public void Open(string fileName, Mode mode)
        {
            //Switch used instead of "if-else"
            switch (mode)
            {
                case Mode.Read:
                    {
                        //instanciate the StreamReader object
                        _readerStream = new StreamReader(fileName);
                        break;
                    }
                case Mode.Write:
                    {
                        //instanciated the StreamWriterObject
                        _writerStream = new StreamWriter(fileName);
                        break;
                    }
                default:
                    {
                        throw new Exception($"Unknown file mode for {fileName}");
                    }
            }

        }

        public void Write(params string[] columns)
        {
            //this is to maintain backward compatibility
            //It will be null only when CSVReaderWriter is instanciated with no parameters.
            if (_writer == null)
            {
                _writer = new CSVWriter(_writerStream);
            }

            using (_writer)
            {
                _writer.WriteLine(columns);
            }

        }
        
        //This method does not return any data so the parameters are not required, 
        //however I have left the parameters to maintain backward compatibility
        public bool Read(string column1, string column2)
        {
            IList<string> data;
            return Read(out data);
        }

        public bool Read(out string column1, out string column2)
        {
            IList<string> data;
            var output = Read(out data);
            if (output)
            {
                column1 = data[0] ?? data[0];
                column2 = data[1] ?? data[1];
            }
            else
            {
                column1 = null;
                column2 = null;
            }

            return output;
        }


        public void Close()
        {
            if (_writerStream != null)
            {
                _writerStream.Close();
                _writerStream = null;
            }

            if (_readerStream != null)
            {
                _readerStream.Close();
                _readerStream = null;
            }

        }

        //Ensuring that the unmanaged resouces can be disposed
        public void Dispose()
        {
            Close();
        }

        //Reusable private method as it is shared between the two read methods
        private bool Read(out IList<string> data)
        {
            //this is to maintain backward compatibility
            //It will be null only when CSVReaderWriter is instanciated with no parameters.
            if (_reader == null)
            {
                _reader = new CSVReader(_readerStream);
            }

            return _reader.ReadLineColumnValues(out data);

        }
    }
}
