using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Jv.Web.OAuth
{
    /// <summary>
    /// Constroi corpo de requisição do tipo multi-part
    /// </summary>
    public class MultipartBuilder : IDisposable
    {
        public string ContentType { get { return "multipart/form-data; boundary=" + MultipartBuilder.Boundary; } }

        public bool IsEmpty { get { return _buffer.Length == 0; } }

        #region Variables
        private static byte[] FIELD_PARAM = Encoding.UTF8.GetBytes("Content-Disposition: form-data; name=");
        private static byte[] FILE_PARAM = Encoding.UTF8.GetBytes("; filename=");
        private static byte[] CONTENT_TYPE = Encoding.UTF8.GetBytes("Content-Type: ");
        private static byte[] QUOTE = Encoding.UTF8.GetBytes("\"");
        private static byte[] CRLF = Encoding.UTF8.GetBytes("\r\n");
        private static byte[] DASHDASH = Encoding.UTF8.GetBytes("--");

        private MemoryStream _buffer;
        /// <summary>
        /// Sequencia de caracteres para separação de seções multi-part
        /// </summary>
        public const string Boundary = "n1z2y3x4w5v6u7t";
        private byte[] _boundaryAsBytes;
        #endregion

        #region MultipartBuilder
        /// <summary>
        /// Constroi corpo de requisição do tipo multi-part
        /// </summary>
        public MultipartBuilder()
        {
            _buffer = new MemoryStream();
            _boundaryAsBytes = Encoding.UTF8.GetBytes(Boundary);
        }
        #endregion

        #region AddField
        /// <summary>
        /// Adicionar sessão do tipo texto ao multi-part
        /// </summary>
        /// <param name="name">Nome da sessão</param>
        /// <param name="value">Conteúdo da sessão</param>
        /// <returns>this</returns>
        public void AddField(string name, string value)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            byte[] nameBytes = Encoding.UTF8.GetBytes(name);
            byte[] valueBytes = Encoding.UTF8.GetBytes(value);

            _buffer.Write(DASHDASH, 0, DASHDASH.Length);
            _buffer.Write(_boundaryAsBytes, 0, _boundaryAsBytes.Length);
            _buffer.Write(CRLF, 0, CRLF.Length);
            _buffer.Write(FIELD_PARAM, 0, FIELD_PARAM.Length);
            _buffer.Write(QUOTE, 0, QUOTE.Length);
            _buffer.Write(nameBytes, 0, nameBytes.Length);
            _buffer.Write(QUOTE, 0, QUOTE.Length);
            _buffer.Write(CRLF, 0, CRLF.Length);
            _buffer.Write(CRLF, 0, CRLF.Length);
            _buffer.Write(valueBytes, 0, valueBytes.Length);
            _buffer.Write(CRLF, 0, CRLF.Length);
        }
        #endregion

        #region AddFile
        /// <summary>
        /// Adicionar arquivo às sessões multi-part
        /// </summary>
        /// <param name="name">Nome da sessão</param>
        /// <param name="filename">Nome do arquivo</param>
        /// <param name="contentType">Tipo do arquivo</param>
        /// <param name="value">Conteúdo do arquivo</param>
        /// <returns>this</returns>
        public async Task AddFileAsync(string name, StorageFile file)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            byte[] nameBytes = Encoding.UTF8.GetBytes(name);
            byte[] filenameBytes = Encoding.UTF8.GetBytes(file.Name);
            byte[] contentTypeBytes = Encoding.UTF8.GetBytes(file.ContentType);

            _buffer.Write(DASHDASH, 0, DASHDASH.Length);
            _buffer.Write(_boundaryAsBytes, 0, _boundaryAsBytes.Length);
            _buffer.Write(CRLF, 0, CRLF.Length);
            _buffer.Write(FIELD_PARAM, 0, FIELD_PARAM.Length);
            _buffer.Write(QUOTE, 0, QUOTE.Length);
            _buffer.Write(nameBytes, 0, nameBytes.Length);
            _buffer.Write(QUOTE, 0, QUOTE.Length);
            _buffer.Write(FILE_PARAM, 0, FILE_PARAM.Length);
            _buffer.Write(QUOTE, 0, QUOTE.Length);
            _buffer.Write(filenameBytes, 0, filenameBytes.Length);
            _buffer.Write(QUOTE, 0, QUOTE.Length);
            _buffer.Write(CRLF, 0, CRLF.Length);
            _buffer.Write(CONTENT_TYPE, 0, CONTENT_TYPE.Length);
            _buffer.Write(contentTypeBytes, 0, contentTypeBytes.Length);
            _buffer.Write(CRLF, 0, CRLF.Length);
            _buffer.Write(CRLF, 0, CRLF.Length);

            using (var readStream = await file.OpenReadAsync())
                await readStream.AsStream().CopyToAsync(_buffer);

            _buffer.Write(CRLF, 0, CRLF.Length);
        }
        #endregion

        #region Build
        bool _built;
        /// <summary>
        /// Adicionar rodapé ao conteúdo multi-part
        /// </summary>
        /// <returns></returns>
        void Build()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            if (!_built)
            {
                _buffer.Write(DASHDASH, 0, DASHDASH.Length);
                _buffer.Write(_boundaryAsBytes, 0, _boundaryAsBytes.Length);
                _buffer.Write(DASHDASH, 0, DASHDASH.Length);
                _buffer.Write(CRLF, 0, CRLF.Length);
                _buffer.Write(CRLF, 0, CRLF.Length);
                _built = true;
            }
            //return _buffer.ToArray();
        }
        #endregion

        public async Task CopyToAsync(Stream destination)
        {
            Build();
            await _buffer.CopyToAsync(destination);
        }

        #region IDisposable
        bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~MultipartBuilder()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose any IDisposable members here
                    _buffer.Dispose();
                }

                // Dispose any unmanaged members here

                _disposed = true;
            }
        }
        #endregion
    }
}
