using Microsoft.CDS.Devices.Client.Transport;
using System;
using System.IO;
using System.Text;

namespace Microsoft.CDS.Devices.Client.MultipartFormData
{
    /// <summary>
    /// Extension methods for <see cref="System.IO.FileInfo"/>.
    /// </summary>
    public static class FileInfoExtensions
    {
        /// <summary>
        /// Template for a file item in multipart/form-data format.
        /// </summary>
        public const string HeaderFormFieldTemplate = "--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}\r\n";
        public const string HeaderFilePartTemplate = "--{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{2}\"\r\nContent-Type: {3}\r\n\r\n";
        
        /// <summary>
        /// Writes a file to a stream in multipart/form-data format.
        /// </summary>
        /// <param name="file">The file that should be written.</param>
        /// <param name="stream">The stream to which the file should be written.</param>
        /// <param name="mimeBoundary">The MIME multipart form boundary string.</param>
        /// <param name="mimeType">The MIME type of the file.</param>
        /// <param name="filename">The name of the form parameter corresponding to the file upload.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown if any parameter is <see langword="null" />.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Thrown if <paramref name="mimeBoundary" />, <paramref name="mimeType" />,
        /// or <paramref name="filename" /> is empty.
        /// </exception>
        /// <exception cref="System.IO.FileNotFoundException">
        /// Thrown if <paramref name="file" /> does not exist.
        /// </exception>
        public static void WriteMultipartFormData(this FileInfo file, Stream stream, string mimeBoundary, string mimeType, string filename)
        {
            if (file == null)
            {
                throw new ArgumentNullException("file");
            }
            if (!file.Exists)
            {
                throw new FileNotFoundException("Unable to find file to write to stream.", file.FullName);
            }
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            if (mimeBoundary == null)
            {
                throw new ArgumentNullException("mimeBoundary");
            }
            if (mimeBoundary.Length == 0)
            {
                throw new ArgumentException("MIME boundary may not be empty.", "mimeBoundary");
            }
            if (mimeType == null)
            {
                throw new ArgumentNullException("mimeType");
            }
            if (mimeType.Length == 0)
            {
                throw new ArgumentException("MIME type may not be empty.", "mimeType");
            }
            if (filename == null)
            {
                throw new ArgumentNullException("formKey");
            }
            if (filename.Length == 0)
            {
                throw new ArgumentException("Form key may not be empty.", "formKey");
            }

            Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            string contentStartTS = String.Format(HeaderFormFieldTemplate, mimeBoundary, RestfulAPIHelper.DEVICE_LOG_API_FORMKEY_STARTTS, unixTimestamp);
            string contentFileName = String.Format(HeaderFilePartTemplate, mimeBoundary, RestfulAPIHelper.DEVICE_LOG_API_FORMKEY_FILENAME, filename, mimeType);
            
            byte[] headerbytesStartTS = Encoding.UTF8.GetBytes(contentStartTS);
            stream.Write(headerbytesStartTS, 0, headerbytesStartTS.Length);
           
            byte[] headerbytesFileName = Encoding.UTF8.GetBytes(contentFileName);
            stream.Write(headerbytesFileName, 0, headerbytesFileName.Length);

            using (FileStream fileStream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
            {
                byte[] buffer = new byte[1024];
                int bytesRead = 0;
                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    stream.Write(buffer, 0, bytesRead);
                }
                fileStream.Close();
            }
            byte[] newlineBytes = Encoding.UTF8.GetBytes("\r\n");
            stream.Write(newlineBytes, 0, newlineBytes.Length);
        }
    }
}
