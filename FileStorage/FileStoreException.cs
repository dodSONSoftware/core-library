using System;
using System.Runtime.Serialization;

namespace dodSON.Core.FileStorage
{
    /// <summary>
    /// The exception that is thrown when a rule is broken in the store.
    /// </summary>
    [Serializable()]
    public class FileStoreException
        : Exception, ISerializable
    {
        #region Ctor
        /// <summary>
        /// Protected default constructor.
        /// </summary>
        protected FileStoreException()
            : base() { }
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="offendingItem">The ItemType involved in the error.</param>
        /// <param name="message">The message that describes the error.</param>
        public FileStoreException(IFileStoreItem offendingItem, string message)
            : base(message)
        {
            OffendingItem = offendingItem;
        }
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="offendingRootFilename">The name of the item involved in the error.</param>
        /// <param name="message">The message that describes the error.</param>
        public FileStoreException(string offendingRootFilename, string message)
            : base(message)
        {
            _OffendingRootFilename = offendingRootFilename;
        }
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="offendingItem">The ItemType involved in the error.</param>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">Gets the exception instance that caused the current exception.</param>
        public FileStoreException(IFileStoreItem offendingItem, string message, Exception innerException)
            : base(message, innerException)
        {
            OffendingItem = offendingItem;
        }
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="offendingRootFilename"></param>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">An <see cref="Exception"/> to be added to the current <see cref="Exception"/>.</param>
        public FileStoreException(string offendingRootFilename, string message, Exception innerException)
            : base(message, innerException)
        {
            _OffendingRootFilename = offendingRootFilename;
        }
        /// <summary>
        /// Protected serialization constructor.
        /// </summary>
        /// <param name="info">Stores all the data needed to serialize or deserialize an object.</param>
        /// <param name="context">Describes the source and destination of a given serialized stream.</param>
        protected FileStoreException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            if (info != null)
            {
                OffendingItem = info.GetValue("_OffendingItem", typeof(IFileStoreItem)) as IFileStoreItem;
                _OffendingRootFilename = info.GetValue("_OffendingRootFilename", typeof(string)) as string;
            }
        }
        #endregion
        #region Private Fields
        private string _OffendingRootFilename = string.Empty;
        #endregion
        #region Public Methods
        /// <summary>
        /// The ItemType involved in the error.
        /// </summary>
        public IFileStoreItem OffendingItem { get; } = null;
        /// <summary>
        /// The name of the item involved in the error.
        /// </summary>
        public string OffendingItemRootFilename
        {
            get
            {
                if (OffendingItem != null) { return OffendingItem.RootFilename; }
                return _OffendingRootFilename;
            }
        }
        #endregion
        #region System.Runtime.Serialization.ISerializable Methods
        /// <summary>
        /// Implements the <see cref="System.Runtime.Serialization.ISerializable"/> interface and returns the data needed to serialize the state for this instance.
        /// </summary>
        /// <param name="info">A <see cref="System.Runtime.Serialization.SerializationInfo"/> object that contains the information required to serialize the state instance.</param>
        /// <param name="context">A <see cref="System.Runtime.Serialization.StreamingContext"/> that contains the source and destination of the serialized stream associated with the state instance.</param>
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            if (info != null)
            {
                info.AddValue("_OffendingItem", OffendingItem, typeof(IFileStoreItem));
                info.AddValue("_OffendingRootFilename", _OffendingRootFilename, typeof(string));
            }
        }
        #endregion
    }
}
