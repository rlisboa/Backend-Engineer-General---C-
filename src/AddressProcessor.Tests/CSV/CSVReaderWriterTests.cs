using System;
using System.Collections.Generic;
using System.IO;
using AddressProcessing.CSV;
using Moq;
using NUnit.Framework;

namespace AddressProcessing.Tests.CSV
{
    [TestFixture]
    public class CSVReaderWriterTests
    {
        private string _currentDirectory, _csvReadTestData,
                       _csvWriteTestData, _csvBlankTestData,
                       _nonExistingFile;

        [SetUp]
        public void Setup()
        {
            _currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            _csvReadTestData = @"\test_data\contacts.csv";
            _csvBlankTestData = @"\test_data\blank.csv";
            _csvWriteTestData = @"\test_data\write.csv";
            _nonExistingFile = @"\test_data\invalidfile.csv";
        }

        [Test]
        public void WhenInValidMode_Then_ThrowsException()
        {
            //declare both read and write mode
            var mode = CSVReaderWriter.Mode.Read | CSVReaderWriter.Mode.Write;

            using (var readerWriter = new CSVReaderWriter())
            {
                Assert.Throws<Exception>(() => readerWriter.Open(_currentDirectory + _csvReadTestData, mode));
            }
        }

        [Test]
        public void WhenInReadMode_WithNonExistingFileOrInvalidFilePath_Then_ThrowsFileNotFoundException()
        {
            using (var readerWriter = new CSVReaderWriter())
            {
                var mode = CSVReaderWriter.Mode.Read;
                Assert.Throws<FileNotFoundException>(() => readerWriter.Open(_currentDirectory + @"\sss", mode));
            }
        }

        [Test]
        public void WhenInReadMode_WithValidFilePathAndEmptyText_Then_ReturnsNull()
        {
            using (var readerWriter = new CSVReaderWriter())
            {
                var mode = CSVReaderWriter.Mode.Read;
                readerWriter.Open(_currentDirectory + _csvBlankTestData, mode);
                string col1, col2;
                var actual = readerWriter.Read(out col1, out col2);
                Assert.That(actual, Is.False);
                Assert.That(col1, Is.Null);
                Assert.That(col2, Is.Null);
            }
        }

        [Test]
        public void WhenInReadMode_WithValidFilePath_Then_ReadSuccesffully()
        {
            using (var readerWriter = new CSVReaderWriter())
            {
                readerWriter.Open(_currentDirectory + _csvReadTestData, CSVReaderWriter.Mode.Read);
                string col1;
                string col2;
                var actual = readerWriter.Read(out col1, out col2);
                Assert.That(actual, Is.True);
                Assert.That(col1, Is.EqualTo("Shelby Macias"));
                Assert.That(col2, Is.EqualTo (@"3027 Lorem St.|Kokomo|Hertfordshire|L9T 3D5|England"));
            }
        }

        [Test]
        public void WhenInReadMode_WritingToAFile_Then_ThrowsAnError()
        {
            using (var readerWriter = new CSVReaderWriter())
            {
                var mode = CSVReaderWriter.Mode.Read;
                readerWriter.Open(_currentDirectory + _csvWriteTestData, mode);
                Assert.Throws<ArgumentNullException>(() => readerWriter.Write());
            }
        }

        [Test]
        public void WhenInWriteMode_ReadingAFile_Then_ThrowsAnError()
        {
            using (var readerWriter = new CSVReaderWriter())
            {
                var mode = CSVReaderWriter.Mode.Write;
                string col1, col2;
                readerWriter.Open(_currentDirectory + _csvReadTestData, mode);
                Assert.Throws<ArgumentNullException>(() => readerWriter.Read(out col1, out col2));
            }
        }

        [Test]
        public void WhenInWriteMode_WithNonExistingFile_Then_NewFileIsCreated()
        {
            using (var readerWriter = new CSVReaderWriter())
            {
                var mode = CSVReaderWriter.Mode.Write;
                Assert.DoesNotThrow(() => readerWriter.Open(_currentDirectory + _nonExistingFile, mode));
            }
        }

        [Test]
        public void WhenInWriteMode_WithValidFilePath_Then_WritesSuccesffully()
        {
            using (var readerWriter = new CSVReaderWriter())
            {
                var mode = CSVReaderWriter.Mode.Write;
                var expectedWiteText1 = @"I am a writer";
                var expectedWiteText2 = @"I just wrote this";
                readerWriter.Open(_currentDirectory + _csvWriteTestData, mode);
                readerWriter.Write(expectedWiteText1, expectedWiteText2);

                //then open the file and verify written content
                mode = CSVReaderWriter.Mode.Read;
                string col1,col2;
                readerWriter.Open(_currentDirectory + _csvWriteTestData, mode);

                var actual = readerWriter.Read(out col1, out col2);
                Assert.IsTrue(actual);
                Assert.That(col1, Is.EqualTo(expectedWiteText1));
                Assert.That(col2, Is.EqualTo(expectedWiteText2));
            }

        }

        //[Test]
        //public void InvokingCSVReaderWriter_WithNullStreamsContructor_Then_ThrowsError()
        //{
        //    var mockReader = new Mock<IReader>();
        //    var mockWriter = new Mock<IWriter>();
        //    IList<string> cols;
        //    mockReader.Setup(r => r.ReadLineColumnValues(out cols));
        //    using (var readerWriter = new CSVReaderWriter(mockReader.Object, mockWriter.Object))
        //    {
        //        string col1, col2;
        //        var actual = readerWriter.Read(out col1, out col2);
        //        Assert.That(actual, Is.False);
        //        Assert.That(col1, Is.Null);
        //        Assert.That(col2, Is.Null);
        //    }

        //}


        [Test]
        public void InvokingCSVReaderWriter_WithStreamsContructor_Then_ReadsFileSuccesffully()
        {

            using (var readerWriter = new CSVReaderWriter(new CSVReader(new StreamReader(_currentDirectory + _csvReadTestData)), new CSVWriter(new StreamWriter(_currentDirectory + _csvWriteTestData))))
            {
                string col1, col2;
                var actual = readerWriter.Read(out col1, out col2);
                Assert.IsTrue(actual);
                Assert.That(col1, Is.EqualTo("Shelby Macias"));
                Assert.That(col2, Is.EqualTo("3027 Lorem St.|Kokomo|Hertfordshire|L9T 3D5|England"));
            }

        }
    }
}
