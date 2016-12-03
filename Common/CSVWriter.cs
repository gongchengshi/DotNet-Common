using System;
using System.Linq;
using System.IO;
using System.Text;

namespace Gongchengshi
{
    /// <summary>
    /// Comma Seperated Values file writer.
    /// </summary>
    public class CSVWriter : IDisposable
    {
        /// <summary>
        /// Construct a CSVWriter on a new output file.
        /// This will throw an exception if the file already exists.
        /// </summary>
        /// <param name="pathname">Full path to output file, including extension.</param>
        /// <param name="filemode">Filemode to use when creating file.</param>
        public CSVWriter(string pathname, FileMode filemode)
        {
            _StreamWriter = new StreamWriter(new FileStream(pathname, filemode));
        }

        /// <summary>
        /// Construct a CSVWriter.
        /// </summary>
        /// <param name="stream">Output stream to write to.</param>
        public CSVWriter(Stream stream)
        {
            _StreamWriter = new StreamWriter(stream);
        }

        /// <summary>
        /// The format string to use when outputting real numbers.
        /// </summary>
        public string RealNumberFormatString { get; set; }

        /// <summary>
        /// If true, the next write is at the start of a line.
        /// </summary>
        bool _StartOfLine = true;

        /// <summary>
        /// The underlying output StreamWriter.
        /// </summary>
        StreamWriter _StreamWriter;

        /// <summary>
        /// Write a field to the file.  Overloaded for common types.
        /// </summary>
        public void WriteField()
        {
            WriteField("");
        }
        public void WriteField(double value)
        {
            string stringValue;
            if (RealNumberFormatString != null)
                stringValue = value.ToString(RealNumberFormatString, System.Globalization.CultureInfo.InvariantCulture);
            else
                stringValue = value.ToString(System.Globalization.CultureInfo.InvariantCulture);
            WriteField(stringValue);
        }
        public void WriteField(UInt32 value)
        {
            WriteField(value.ToString(System.Globalization.CultureInfo.InvariantCulture));
        }
        public void WriteField(Int32 value)
        {
            WriteField(value.ToString(System.Globalization.CultureInfo.InvariantCulture));
        }
        public void WriteField(string value)
        {
            if (_StartOfLine)
                _StartOfLine = false;
            else
                _StreamWriter.Write(",");

            value = FormatDelimiterSeparatedValue(",", value);
            
            // Replace all common combinations of newlines with whitespace.
            value = value.Replace("\r\n", " ");
            value = value.Replace("\n", " ");
            value = value.Replace("\r", " ");

            _StreamWriter.Write(value);
        }

        /// <summary>
        /// Write an end of line to the file.
        /// </summary>
        public void WriteEndOfLine()
        {
            _StreamWriter.WriteLine();
            _StartOfLine = true;
        }

        /// <summary>
        /// Write an entire line of fields to the file,
        /// </summary>
        /// <param name="fields">The fields to write.</param>
        public void WriteLine(params object[] fields)
        {
            foreach (var field in fields)
            {
                if (Object.ReferenceEquals(field, null))
                    WriteField();
                else if (field is Single)
                    WriteField((Double)(Single)field);
                else if (field is Double)
                    WriteField((Double)field);
                else if (field is UInt32)
                    WriteField((UInt32)field);
                else if (field is Int32)
                    WriteField((Int32)field);
                else
                    WriteField(field.ToString());
            }
            WriteEndOfLine();
        }

        /// <summary>
        /// Dispose this writer.
        /// </summary>
        public void Dispose()
        {
            _StreamWriter.Dispose();
        }
        
        /// <summary>
        /// Used by DataTable.ToDelimitedFile() to ensure the value being 
        /// written is valid according to RFC-4180.
        /// Using comma literal here because we escape the use of commas in column contents
        /// so even if the locale requires the use of commas instead of periods for decimal points
        /// these files will still be readable by CSV readers.
        /// </summary>
        /// <param name="delimiter">Delimiter that will be placed between fields.</param>
        /// <param name="value">Value to format.</param>
        /// <returns>A valid string.</returns>
        public static string FormatDelimiterSeparatedValue(string delimiter, string value)
        {
            var sb = new StringBuilder(value);

            if ((value.Length >= 2) && value.EndsWith(_Quote) && value.StartsWith(_Quote))
            {
                // If the value is wrapped in double-quotes and contains double-quotes then the 
                // internal double-quotes need to be escaped with an additional double-quote
                sb.Replace(_Quote, _QuoteQuote, 1, value.Length - 2);
            }
            else
            {
                // If the field contains either the delimiter character, \r, or \n, the whole
                // field must be wrapped in 2 " characters and any " characters left in the middle
                // must be escaped with ". NOTE: " is the escape character, which is why it needs to
                // be escaped.
                if (value.Contains(delimiter) || value.Contains(_CarriageReturn) || value.Contains(_LineFeed))
                {
                    // Escape any double-quotes with an additional quote.
                    sb.Replace(_Quote, _QuoteQuote);

                    // Wrap the string in double-quotes
                    sb.Insert(0, _Quote).Append(_Quote);
                }                
            }

            return sb.ToString();
        }
        
        
        const char _CarriageReturn = '\r';
        const char _LineFeed = '\n';

        const string _Quote = "\"";
        const string _QuoteQuote = "\"\"";
    }
}
