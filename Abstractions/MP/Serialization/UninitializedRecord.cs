


using System;

namespace MP.Serialization
{
    /// <summary>
    /// Defines a special case of the <see cref="Record"/> class in that this one initializes lazily the record. <br />
    /// Mostly provided for security reasons, such as to avoid loading a possibly malicious record.
    /// </summary>
    public sealed class UninitializedRecord
    {
        private Record rc;
        private Func<Record> recordsupplier;

        /// <summary>
        /// Creates a new instance of the <see cref="UninitializedRecord"/> class, providing a function that internally has the appropriate information to load the record.
        /// </summary>
        /// <param name="supplier">The function to use for initializing the record.</param>
        /// <exception cref="ArgumentNullException"><paramref name="supplier"/> is <see langword="null"/>.</exception>
        public UninitializedRecord(Func<Record> supplier)
        {
            ArgumentNullException.ThrowIfNull(supplier);
            recordsupplier = supplier;
            rc = null;
        }

        /// <summary>
        /// Gets the actual value, after ensuring that the record has been initialized.
        /// </summary>
        public Record Value
        {
            get {
                if (rc is null) {
                    rc = recordsupplier();
                    recordsupplier = null;
                }
                return rc;
            }
        }
    }
}