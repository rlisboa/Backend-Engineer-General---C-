using System;

namespace AddressProcessing.CSV
{
    public interface IWriter : IDisposable
    {
        void WriteLine(params string[] columns);
    }
}
