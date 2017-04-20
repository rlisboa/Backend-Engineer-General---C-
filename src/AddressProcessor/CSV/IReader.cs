using System;
using System.Collections.Generic;

namespace AddressProcessing.CSV
{
    public interface IReader : IDisposable
    {
        string ReadLine();
        bool ReadLineColumnValues(out IList<string> colums);
    }
    
}
