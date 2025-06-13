
using System;

namespace MP
{
    /// <summary>
    /// Defines an item in the playlist's metadata. <br />
    /// The playlist metadata is a set of states employed by the playlist format to store cached data about it's current state. <br />
    /// Unlike the playlist preferences , these are internal caches used to affect the behavior of a playlist. <br />
    /// Each item is in the logic of a key-value pair, where it's type of the value stored cannot be changed after specified.
    /// </summary>
    public sealed class PlaylistMetadataItem
    {
        private readonly System.String name;
        private System.Object value;
        private MetadataItemTypeCode typecodeactual;

        /// <summary>
        /// Creates a new <see cref="PlaylistMetadataItem"/> instance that is initialized from the specified string that will be the name of this new item.
        /// </summary>
        /// <param name="name">The name of the newly created metadata item.</param>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> was <see langword="null"/> or represented the empty string (&quot;&quot;).</exception>
        public PlaylistMetadataItem(System.String name) : this(name , null) { }

        /// <summary>
        /// Creates a new <see cref="PlaylistMetadataItem"/> instance that is initialized from the specified string that will be the name of this new item, 
        /// and the initial value of this metadata item.
        /// </summary>
        /// <param name="name">The name of the newly created metadata item.</param>
        /// <param name="value">The value of the newly created metadata item. If <paramref name="value"/> is null , then you are allowed to set it's value later by using the <see cref="Value"/> property.</param>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> was <see langword="null"/> or represented the empty string (&quot;&quot;).</exception>
        public PlaylistMetadataItem(System.String name, System.Object value)
        {
            if (System.String.IsNullOrEmpty(name)) {
                throw new ArgumentNullException(nameof(name));
            }
            this.name = name;
            typecodeactual = MetadataItemTypeCode.Undefined;
            Value_SetValue(value);
        }

        [System.Diagnostics.StackTraceHidden]
        private void Value_SetValue(System.Object value)
        {
            MetadataItemTypeCode typecodedet;
            switch (value)
            {
                case null:
                    this.value = null;
                    return;
                case System.String:
                    typecodedet = MetadataItemTypeCode.String;
                    break;
                case System.SByte:
                    typecodedet = MetadataItemTypeCode.SByte;
                    break;
                case System.Byte:
                    typecodedet = MetadataItemTypeCode.Byte;
                    break;
                case System.Int16:
                    typecodedet = MetadataItemTypeCode.Int16;
                    break;
                case System.Int32:
                    typecodedet = MetadataItemTypeCode.Int32;
                    break;
                case System.Int64:
                    typecodedet = MetadataItemTypeCode.Int64;
                    break;
                case System.UInt16:
                    typecodedet = MetadataItemTypeCode.UInt16;
                    break;
                case System.UInt32:
                    typecodedet = MetadataItemTypeCode.UInt32;
                    break;
                case System.UInt64:
                    typecodedet = MetadataItemTypeCode.UInt64;
                    break;
                case System.Single:
                    typecodedet = MetadataItemTypeCode.Single;
                    break;
                case System.Double:
                    typecodedet = MetadataItemTypeCode.Double;
                    break;
                case System.Decimal:
                    typecodedet = MetadataItemTypeCode.Decimal;
                    break;
                case System.Boolean:
                    typecodedet = MetadataItemTypeCode.Boolean;
                    break;
                default:
                    throw new ArgumentException("The value of a metadata item is allowed to be only one of the primitive types , the String type or the floating-point types.");
            }
            if (typecodeactual == MetadataItemTypeCode.Undefined) {
                // This is the first time we init this metadata item , so allow any type.
                typecodeactual = typecodedet;
                // the if statement will exit and go to the assignment instruction , as we expect it to otherwise do so.
            } else if (typecodeactual != typecodedet) {
                // If the determined code is different that the expected type , throw a mismatch exception
                throw new ArgumentException($"Value's underlying type expected to be {typecodeactual} but was {typecodedet}.");
            }
            this.value = value;
        }

        /// <summary>
        /// Gets the name identifier for the current metadata item. <br />
        /// Can be only set during construction time
        /// </summary>
        public System.String Name => name;

        /// <summary>
        /// Gets the value of the current metadata item. <br />
        /// Once set to a specific type , you cannot change then the underlying type of the <see cref="Value"/> property.
        /// </summary>
        public System.Object Value
        {
            get => value;
            set => Value_SetValue(value);
        }

        /// <summary>
        /// Gets the <see cref="MetadataItemTypeCode"/> of the <see cref="Value"/> property for this instance.
        /// </summary>
        public MetadataItemTypeCode TypeCode => 
            // Whence undefined , return as if it was null instead.
            typecodeactual == MetadataItemTypeCode.Undefined ? MetadataItemTypeCode.Null : typecodeactual;
    }

    /// <summary>
    /// For the <see cref="PlaylistMetadataItem"/> class instance it defines a type code 
    /// which it assists for the immutability and encodability of such an instance.
    /// </summary>
    public enum MetadataItemTypeCode : System.Byte
    {
        /// <summary>
        /// Special internal constant for the <see cref="PlaylistMetadataItem"/> class.
        /// </summary>
        Undefined,
        /// <summary>Represents the <see langword="null"/> value.</summary>
        Null,
        /// <summary>
        /// Represents a string value represented by the <see cref="System.String"/> class.
        /// </summary>
        String,
        /// <summary>
        /// Represents a boolean value , which is a value which can only take two distinct values - <see langword="true"/> and <see langword="false"/>.
        /// </summary>
        Boolean,
        /// <summary>Represents an unsigned number that has a length of one byte.</summary>
        Byte,
        /// <summary>Represents a signed number that has a length of one byte.</summary>
        SByte,
        /// <summary>Represents a signed number that has a length of two bytes.</summary>
        Int16,
        /// <summary>Represents an unsigned number that has a length of two bytes.</summary>
        UInt16,
        /// <summary>Represents a signed number that has a length of four bytes.</summary>
        Int32,
        /// <summary>Represents an unsigned number that has a length of four bytes.</summary>
        UInt32,
        /// <summary>Represents a signed number that has a length of eight bytes.</summary>
        Int64,
        /// <summary>Represents an unsigned number that has a length of eight bytes.</summary>
        UInt64,
        /// <summary>Represents a single-precision floating number that has a length of four bytes.</summary>
        Single,
        /// <summary>Represents a double-precision floating number that has a length of eight bytes.</summary>
        Double,
        /// <summary>Represents a floating number that is suitable for economical operations.</summary>
        Decimal
    }
}