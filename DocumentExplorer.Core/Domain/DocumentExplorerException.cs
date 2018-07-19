using System;

namespace DocumentExplorer.Core.Domain
{
    public abstract class DocumentExplorerException : Exception
    {
        public string Code { get; }

        protected DocumentExplorerException()
        {
        }

        public DocumentExplorerException(string code)
        {
            Code = code;
        }

        public DocumentExplorerException(string message, params object[] args) : this(string.Empty, message, args)
        {
        }

        public DocumentExplorerException(string code, string message, params object[] args) : this(null , code, message, args)
        {
        }

        public DocumentExplorerException(Exception innerException, string message, params object[] args) : this(innerException, string.Empty, message, args)
        {
        }

        public DocumentExplorerException(Exception innerException, string code, string message, params object[] args) : base(string.Format(message, args), innerException)
        {
            Code = code;
        }
    }
}
