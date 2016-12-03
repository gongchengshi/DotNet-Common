using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;

namespace Gongchengshi
{
    namespace BinaryFormatter
    {
        // The BinaryFormatter component encapsulates all of the work required to read/write a given binary 
        // protocol (C37.118, Modbus, COMTRADE, etc.)
        // To define a new protocol, simply create a Frame subclass, and create fields for each
        // of the protocol fields.  Example:
        // 
        //   public class MyRecord : Frame
        //   {
        //       public readonly Field<UInt32> AUInt32Field;
        //       public readonly Field<byte> AByteField;
        //   }
        // 
        // To read/write a field value, simply get/set the Field<T>.Value property.
        // The component also supports:
        //    * Arrays of Fields, by instantiating a FieldArray<T>.
        //    * Repeated groups of fields, by subclassing FieldStructure and instantiating a FieldStructureArray<T>.
        //    * Bitmapped fields, by subclassing BitmappedField<T>.
        //    * Arrays of bitmapped fields, but instantiating a BitmappedFieldArray<T>.
        // 
        // Instead of using the traditional serialization/deserialization approach, the Frame
        // contains a byte[] Buffer that has the current frame's data.  Any reads/writes to the 
        // field values access/changes the underlying buffer.  This approach was chosen to improve
        // performance:  A large frame can be read, and if only a few fields are accessed, the
        // processing time of reading the other fields is not wasted.
        //
        // Performance Note:  Unforunately, C#'s struct have limited capabilities.  So, the entire
        // component is based on classes, and thus, will require large amounts of memory allocations
        // and constructor calls.  To improve performance, one can instantiate the Frame
        // once, and reuse this instance with different data buffers.  Note, obviously, that this
        // would not work if you intend to have multiple frames in memory at a time.
        //
        // An alternative approach would be to use a more traditional serialization/deserialization
        // solution.  Although this may not solve most of the object instantiation issues unless
        // the API was dramatically changed.


        /// <summary>
        /// The base class for binary frame classes.  Subclass this and create members for
        /// each field in the protocol.  
        /// </summary>
        public abstract class Frame : FieldStructure
        {
            /// <summary>
            /// Construct a protocol frame for writing.
            /// </summary>
            /// <param name="networkToHostConversionRequired">Set to true to do network to host conversion, 
            ///   false otherwise.</param>
            public Frame(bool networkToHostConversionRequired) : this(null, networkToHostConversionRequired) { }

            /// <summary>
            /// Construct a protocol frame for reading based on the given buffer of data.
            /// </summary>
            /// <param name="buffer">Data (read from channel).  Once passed to this class, the buffer should
            ///   not be modified by the caller.</param>
            /// <param name="networkToHostConversionRequired">Set to true to do network to host conversion, 
            ///   false otherwise.</param>
            public Frame(byte[] buffer, bool networkToHostConversionRequired)
            {
                NetworkToHostConversion = networkToHostConversionRequired;
                // Instead of copying the entire buffer, use it directly.  The function header informs the
                // user of issues related to this.
                Buffer = buffer;
                Compile();
            }
            internal readonly bool NetworkToHostConversion;
            
            /// <summary>
            /// If there are fields in the protocol that are a function of other fields, call 
            /// Compile to update the derived fields when the source field values have changed.
            /// For example, if the protocol list contains a Names FieldArray, which is an 
            /// array of ChannelCount channel names, then whenever ChannelCount is changed,
            /// Compile() should be called to update the size of the Names array.
            /// This will increase the size of the buffer if necessary.
            /// </summary>
            public void Compile()
            {
                Configure(this, 0);
                
                if (_buffer != null && Size > _buffer.Length)
                    Array.Resize(ref _buffer, Size);

                OnCompileCompleted();
            }

            /// <summary>
            /// Write the data in this buffer to the given Stream.
            /// </summary>
            public void WriteToStream(Stream stream)
            {
                stream.Write(Buffer, 0, Size);
            }

            /// <summary>
            /// Called whenever a Compile() call completes.  Subclasses can override this to 
            /// update the value of fields in the frame (a frame size field, for example).
            /// </summary>
            protected virtual void OnCompileCompleted() { }

            /// <summary>
            /// The data buffer containing the frame data.  
            /// NOTE:  Callers should not rely on the size of this buffer to determine the frame
            ///   size.  Use Size, instead.
            /// </summary>
            public byte[] Buffer
            {
                get
                {
                    if (_buffer == null)
                        _buffer = new byte[Size];
                    return _buffer;
                }
                set
                {
                    _buffer = value;
                }
            }
            private byte[] _buffer;
        }

        /// <summary>
        /// A structure of fields.  This is used when a protocol has a repeated section of more than
        /// one field.  This is also the base class of a Frame.
        /// </summary>
        public class FieldStructure
        {
            /// <summary>
            /// Configure all of the fields within this FieldStructure.
            /// This function computes the offsets of each field within the data buffer.
            /// </summary>
            /// <param name="frame">The owning Frame.</param>
            /// <param name="offset">The offset of this FieldStructure within the buffer.</param>
            /// <returns>The size of the entire FieldStructure.</returns>
            internal int Configure(Frame frame, int offset)
            {
                _frame = frame;
                int initialOffset = offset;
                GetFields(this.GetType());
                foreach (var field in _fields)
                {
                    // The field may be null because Compile is called from base class constructor,
                    // and subclass constructors haven't run yet....
                    if (field == null)
                        break;

                    offset += field.Configure(frame, offset);
                }
                Size = offset - initialOffset;
                return Size;
            }
            internal Frame _frame;
            
            /// <summary>
            /// The cached size of this FieldStructure, in bytes.
            /// </summary>
            public int Size { get; private set; }

            /// <summary>
            /// This recursive function uses reflection to return all C# fields on the given type 
            /// that implement IField, in base-class first order.
            /// </summary>
            private void GetFields(Type type)
            {
                _fields = new List<IField>();                
                var baseType = type.BaseType;
                if ((baseType != typeof(FieldStructure)) && (baseType != typeof(Frame)))
                    GetFields(type.BaseType);
                foreach (var fieldInfo in type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance |
                    BindingFlags.Public | BindingFlags.NonPublic))
                    if (typeof(IField).IsAssignableFrom(fieldInfo.FieldType))
                        _fields.Add((IField)fieldInfo.GetValue(this));
            }
            List<IField> _fields;            
        }

        /// <summary>
        /// The primary interface for all fields in a frame.
        /// </summary>
        public interface IField
        {
            /// <summary>
            /// Configure this field (compute size, update offset, etc.)
            /// </summary>
            /// <param name="frame">The Frame that contains this field.</param>
            /// <param name="offset">The starting index of this field in the frame's data buffer.</param>
            /// <returns>The size of this field.</returns>
            int Configure(Frame frame, int offset);

            /// <summary>
            /// This convenience property returns the offset at the end of a given field.
            /// It is used, for example, when one needs to know the number of bytes in part of 
            /// a frame, up to and including this field.
            /// </summary>
            int OffsetAtEnd { get; }
        }

        /// <summary>
        /// Generic Field for a given type T.  
        /// </summary>
        /// <typeparam name="T">The type to read/write from the data buffer.</typeparam>
        public class Field<T> : IField
        {
            /// <summary>
            /// Construct a Field using the default converter for the given type T.
            /// Will throw if there is not default converter defined.
            /// </summary>
            public Field() : this(DefaultConverter) { }

            /// <summary>
            /// Construct a Field using the given converter.
            /// </summary>
            public Field(IConverter<T> converter)
            {
                _converter = converter;
            }

            /// <summary>
            /// The converter used to convert the data of type T to/from data bytes.
            /// </summary>
            protected IConverter<T> _converter;

            /// <summary>
            /// Configure this field within the data frame.  See IField for more info.
            /// </summary>
            public int Configure(Frame frame, int offset)
            {
                _offset = offset;
                _frame = frame;
                return _converter.Size;
            }
            protected Frame _frame;
            protected int _offset;

            /// <summary>
            /// An implicit cast operator to allow reading Field values without having to
            /// use ".Value".
            /// </summary>
            public static implicit operator T(Field<T> field)
            {
                return field.Value;
            }

            /// <summary>
            /// Get or set the field value.  This function calls the converter to do the 
            /// reading/writing of the data buffer.  It also handles network to host 
            /// conversion as required.
            /// </summary>
            virtual public T Value
            {
                get
                {
                    int size = _converter.Size;

                    // In some complex frame cases, during a compile the buffer will not yet be
                    // large enough for the full frame size.  Detect these cases and return 
                    // default(T).
                    if (_offset + size > _frame.Buffer.Length)
                    {
                        return default(T);
                    }

                    byte[] buffer;
                    int offset;
                    
                    if (DoNetworkToHostConversion(size))
                    {
                        buffer = new byte[size];
                        Array.Copy(_frame.Buffer, _offset, buffer, 0, size);
                        buffer.ReverseBytes(0, size);
                        offset = 0;
                    }
                    else
                    {
                        buffer = _frame.Buffer;
                        offset = _offset;
                    }
                    return _converter.GetValue(buffer, offset);
                }
                set
                {
                    _converter.GetBytes(value, _frame.Buffer, _offset);
                    int size = _converter.Size;
                    if (DoNetworkToHostConversion(size))
                        _frame.Buffer.ReverseBytes(_offset, size);
                }
            }

            /// <summary>
            /// Return the offset in the frame at the end of this field.
            /// </summary>
            public int OffsetAtEnd
            {
                get { return _offset + _converter.Size; }
            }

            /// <summary>
            /// Override ToString to make debugging easier.
            /// </summary>
            public override string ToString()
            {
                return Value.ToString();
            }

            /// <summary>
            /// Determine if network to host conversion should be done.  If:
            ///  1. It was specified on the Frame.
            ///  2. This size is > 1.
            ///  3. The converter does not indicate that no network to host conversion should occur 
            ///     (strings don't use network to host conversion, for example).
            /// </summary>
            private bool DoNetworkToHostConversion(int size)
            {
                return _frame.NetworkToHostConversion && (size > 1) &&
                    BitConverter.IsLittleEndian && !(_converter is INoNetworkToHostConversion);
            }

            /// <summary>
            /// Lookup the default converter for a given type T.
            /// </summary>
            public static IConverter<T> DefaultConverter
            {
                get
                {
                    if (_defaultConverter == null)
                    {
                        var type = typeof(T);
                        if (type == typeof(Byte))
                            _defaultConverter = (IConverter<T>)new ByteConverter();
                        else if (type == typeof(UInt16))
                            _defaultConverter = (IConverter<T>)new UInt16Converter();
                        else if (type == typeof(Int16))
                            _defaultConverter = (IConverter<T>)new Int16Converter();
                        else if (type == typeof(UInt32))
                            _defaultConverter = (IConverter<T>)new UInt32Converter();
                        else if (type == typeof(Int32))
                            _defaultConverter = (IConverter<T>)new Int32Converter();
                        else if (type == typeof(UInt64))
                            _defaultConverter = (IConverter<T>)new UInt64Converter();
                        else if (type == typeof(Int64))
                            _defaultConverter = (IConverter<T>)new Int64Converter();
                        else if (type == typeof(Single))
                            _defaultConverter = (IConverter<T>)new SingleConverter();
                        else if (type == typeof(Double))
                            _defaultConverter = (IConverter<T>)new DoubleConverter();
                        else
                            throw new InvalidOperationException("No default format converter for type: " + typeof(T));
                    }
                    return _defaultConverter;
                }
            }
            private static IConverter<T> _defaultConverter;
        }

        /// <summary>
        /// Use this class instead of Field<T> to create an array of fields.
        /// </summary>
        public class FieldArray<T> : IField, IEnumerable<T>
        {
            /// <summary>
            /// Construct a FieldArray for type T using the default converter and a 
            /// count lambda.
            /// </summary>
            /// <param name="countFunc">This will be called during Configure() to determine the
            ///   length of the field array.</param>
            public FieldArray(Func<int> countFunc) : this(countFunc, Field<T>.DefaultConverter) { }

            /// <summary>
            /// Construct a FieldArray for type T using the specified converter and a 
            /// count lambda.
            /// </summary>
            /// <param name="countFunc">This will be called during Configure() to determine the
            ///   length of the field array.</param>
            public FieldArray(Func<int> countFunc, IConverter<T> converter)
            {
                _countFunc = countFunc;
                _converter = converter;
            }
            private Func<int> _countFunc;
            private IConverter<T> _converter;

            /// <summary>
            /// Configure this field within the data frame.  See IField for more info.
            /// </summary>
            public int Configure(Frame frame, int offset)
            {
                _frame = frame;
                _offset = offset;
                _fieldSize = _converter.Size;
                _count = _countFunc();
                return _fieldSize * _count;
            }
            private Frame _frame;
            private int _fieldSize;
            private int _count;
            private int _offset;

            /// <summary>
            /// Get or set the field value for the field at the given index. 
            /// </summary>
            public T this[int index]
            {
                get
                {
                    var offset = _offset + index * _fieldSize;

                    // In some complex frame cases, during a compile the buffer will not yet be
                    // large enough for the full frame size.  Detect these cases and return 
                    // default(T).
                    if (offset + _fieldSize > _frame.Buffer.Length)
                    {
                        return default(T);
                    }

                    return _converter.GetValue(_frame.Buffer, offset);
                }
                set
                {
                    _converter.GetBytes(value, _frame.Buffer, _offset + index * _fieldSize);
                }
            }

            /// <summary>
            /// Return the offset in the frame at the end of this field.
            /// </summary>
            public int OffsetAtEnd
            {
                get { return _offset + _fieldSize * _count; }
            }

            // Implementation of IEnumerable<T>
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
            public IEnumerator<T> GetEnumerator()
            {
                for (int i = 0; i < _count; i++)
                {
                    yield return this[i];
                }
            }            
        }

        /// <summary>
        /// Use this class to create an array of (repeated) FieldStructures.
        /// </summary>
        /// <typeparam name="T">The FieldStructure subclass.</typeparam>
        public class FieldStructureArray<T> : IField, IEnumerable<T>
            where T : FieldStructure, new()
        {
            /// <summary>
            /// Construct a FieldStructureArray for FieldStructure subclass T.
            /// </summary>
            /// <param name="countFunc">This will be called during Configure() to determine the
            ///   length of the array.</param>
            public FieldStructureArray(Func<int> countFunc)
            {
                _countFunc = countFunc;
            }
            private Func<int> _countFunc;
            
            /// <summary>
            /// Configure this field within the data frame.  See IField for more info.
            /// This function creates a new T instance for each of count items.
            /// </summary>
            public int Configure(Frame frame, int offset)
            {
                _frame = frame;
                _offset = offset;
                int count;
                count = _countFunc();
                if (_fieldStructures == null || _fieldStructures.Length != count)
                    _fieldStructures = new T[count];
                for (int i = 0; i < count; i++)
                {
                    if (_fieldStructures[i] == null)
                        _fieldStructures[i] = new T();
                    offset += _fieldStructures[i].Configure(frame, offset);
                }
                OffsetAtEnd = offset;
                return offset - _offset;
            }
            private FieldStructure _frame;
            private int _offset;
            private T[] _fieldStructures;

            /// <summary>
            /// Return the T instance at the given index in the this array.
            /// </summary>
            public T this[int index]
            {
                get
                {
                    return _fieldStructures[index];
                }
            }

            /// <summary>
            /// Return the offset in the frame at the end of this field.
            /// </summary>
            public int OffsetAtEnd
            {
                get;
                private set; 
            }

            // Implementation of IEnumerable<T>.
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
            public IEnumerator<T> GetEnumerator()
            {
                return ((IEnumerable<T>)_fieldStructures).GetEnumerator();
            }            
        }

        /// <summary>
        /// A bitmapped field allows the definition of Bits and/or BitFields within one or
        /// more bytes.  To use a BitmappedField, subclass the appropriate Bitmapped*Byte
        /// class, and then create fields of type Bit or BitField.
        /// Example:
        /// This creates a single byte, with a one bit Polar at the LSB (index 0 in the byte), 
        /// and a 3 bit field from index 1 to 3.
        ///    public class FormatField : Bitmapped1Byte
        ///    {
        ///        public readonly Bit Polar = new Bit();  
        ///        public readonly BitField Range = new BitField(3);
        ///    }
        /// </summary>
        public abstract class BitmappedField<T> : Field<T>
        {
            /// <summary>
            /// Construct a BitmappedField.
            /// </summary>
            public BitmappedField()
            {
                int index = 0;
                foreach (var bitField in BitFields)
                    index += bitField.Configure(this, index);
            }
            
            /// <summary>
            /// Using reflection, find and return the list of BitFields in this 
            /// type.
            /// </summary>
            private List<BitField> BitFields
            {
                get
                {
                    var bitFields = new List<BitField>();
                    foreach (var fieldInfo in this.GetType().GetFields())
                    {
                        if (typeof(BitField).IsAssignableFrom(fieldInfo.FieldType))
                        {
                            bitFields.Add((BitField)fieldInfo.GetValue(this));
                        }
                    }
                    return bitFields;
                }
            }

            /// <summary>
            /// Retrieve the UInt32 value from the underlying type.
            /// </summary>
            protected abstract UInt32 UInt32Value { get; set; }


            /// <summary>
            /// A range of bits within a BitmappedField
            /// </summary>
            public class BitField
            {
                /// <summary>
                /// Construct a BitField given the range of bits.
                /// </summary>
                public BitField(int numBits)
                {
                    _numBits = numBits;
                    _offset = 0;
                    _bitmappedField = null;
                }
                
                /// <summary>
                /// Retrieve the value of the BitField from within the BitmappedField value.
                /// </summary>
                public UInt32 Value
                {
                    get
                    {
                        return (_bitmappedField.UInt32Value >> _offset) & ((UInt32)(1 << _numBits) - 1);
                    }
                    set
                    {
                        var mask = ~(((UInt32)(1 << _numBits) - 1) << _offset);
                        _bitmappedField.UInt32Value = (_bitmappedField.UInt32Value & mask) | value << _offset;
                    }
                }

                /// <summary>
                /// Configure this bit field within the parent BitmappedField.
                /// </summary>
                /// <param name="bitmappedField">Parent.</param>
                /// <param name="offset">Bit offset within parent bytes.</param>
                /// <returns>The size of this bit field.</returns>
                internal int Configure(BitmappedField<T> bitmappedField, int offset)
                {
                    _bitmappedField = bitmappedField;
                    _offset = offset;
                    return _numBits;
                }
                private int _numBits;
                private int _offset;
                private BitmappedField<T> _bitmappedField;

                /// <summary>
                /// An implicit cast operator to allow reading the bit field value without 
                /// having to use ".Value".
                /// </summary>
                public static implicit operator UInt32(BitField field)
                {
                    return field.Value;
                }
            }

            /// <summary>
            /// A single bit within a BitmappedField.
            /// </summary>
            public class Bit : BitField
            {
                public Bit() : base(1) { }
                public bool BitValue
                {
                    get { return Value == 0 ? false : true; }
                    set { Value = value ? (UInt32)1 : 0; }
                }
                public static implicit operator bool(Bit bit)
                {
                    return bit.BitValue;
                }
            }
        }


        // These BitmappedField<T> subclasses exist because C# does not have a way to 
        // say that a given T must be convertible to/from UInt32.
        
        /// <summary>
        /// A one byte bitmapped field.  See BitmappedField<T> for more info.
        /// </summary>
        public class Bitmapped1Byte : BitmappedField<byte>
        {
            protected override UInt32 UInt32Value
            {
                get { return (UInt32)Value; }
                set { Value = (byte)value; }
            }
        }

        /// <summary>
        /// A two byte bitmapped field. See BitmappedField<T> for more info.
        /// </summary>
        public class Bitmapped2Byte : BitmappedField<UInt16>
        {
            protected override UInt32 UInt32Value
            {
                get { return (UInt32)Value; }
                set { Value = (UInt16)value; }
            }
        }

        /// <summary>
        /// A four byte bitmapped field. See BitmappedField<T> for more info.
        /// </summary>       
        public class Bitmapped4Byte : BitmappedField<UInt32>
        {
            protected override UInt32 UInt32Value
            {
                get { return Value; }
                set { Value = value; }
            }
        }


        /// <summary>
        /// An array of BitmappedFields.
        /// </summary>
        /// <typeparam name="T">A BitmappedField subclass.</typeparam>
        public class BitmappedFieldArray<T> : IField
            where T : IField, new()
        {
            /// <summary>
            /// Construct a BitmappedFieldArray.
            /// </summary>
            /// <param name="countFunc">This will be called during Configure() to determine the
            ///   length of the array.</param>
            public BitmappedFieldArray(Func<int> countFunc)
            {
                _countFunc = countFunc;
            }
            private Func<int> _countFunc;

            /// <summary>
            /// Configure this field within the data frame.  See IField for more info.
            /// This function creates a new T instance for each of count items.
            /// </summary>
            public int Configure(Frame frame, int offset)
            {
                _offset = offset;
                int count;
                count = _countFunc();
                if (_fields == null || _fields.Length != count)
                    _fields = new T[count];
                for (int i = 0; i < count; i++)
                {
                    if (_fields[i] == null)
                        _fields[i] = new T();
                    offset += _fields[i].Configure(frame, offset);
                }
                OffsetAtEnd = offset;
                return offset - _offset;
            }
            private int _offset;
            private T[] _fields;

            /// <summary>
            /// Access the given BitmappedField within the array at index.
            /// </summary>
            public T this[int index]
            {
                get
                {
                    return _fields[index];
                }
            }

            /// <summary>
            /// Return the offset in the frame at the end of this field.
            /// </summary>
            public int OffsetAtEnd
            {
                get;
                private set;
            }
        }

        /// <summary>
        /// IConverters convert from a give type T to the bytes in the data frame.
        /// </summary>
        public interface IConverter<T>
        {
            int Size { get; }
            void GetBytes(T value, byte[] buffer, int index);
            T GetValue(byte[] buffer, int index);
        }

        // Converters for each of the common types.  These are necessary because
        // BitConverter does not have a generic interface.
        class ByteConverter : IConverter<Byte>
        {
            public int Size { get { return sizeof(Byte); } }
            public void GetBytes(byte value, byte[] buffer, int index)
            {
                buffer[index] = value;
            }
            public byte GetValue(byte[] buffer, int index)
            {
                return buffer[index];
            }
        }
        class UInt16Converter : IConverter<UInt16>
        {
            public int Size { get { return sizeof(UInt16); } }
            public void GetBytes(UInt16 value, byte[] buffer, int index)
            {
                BitConverter.GetBytes(value).CopyTo(buffer, index);
            }
            public UInt16 GetValue(byte[] buffer, int index)
            {
                return BitConverter.ToUInt16(buffer, index);
            }
        }
        class Int16Converter : IConverter<Int16>
        {
            public int Size { get { return sizeof(Int16); } }
            public void GetBytes(Int16 value, byte[] buffer, int index)
            {
                BitConverter.GetBytes(value).CopyTo(buffer, index);
            }
            public Int16 GetValue(byte[] buffer, int index)
            {
                return BitConverter.ToInt16(buffer, index);
            }
        }
        class UInt32Converter : IConverter<UInt32>
        {
            public int Size { get { return sizeof(UInt32); } }
            public void GetBytes(UInt32 value, byte[] buffer, int index)
            {
                BitConverter.GetBytes(value).CopyTo(buffer, index);
            }
            public UInt32 GetValue(byte[] buffer, int index)
            {
                return BitConverter.ToUInt32(buffer, index);
            }
        }
        class Int32Converter : IConverter<Int32>
        {
            public int Size { get { return sizeof(Int32); } }
            public void GetBytes(Int32 value, byte[] buffer, int index)
            {
                BitConverter.GetBytes(value).CopyTo(buffer, index);
            }
            public Int32 GetValue(byte[] buffer, int index)
            {
                return BitConverter.ToInt32(buffer, index);
            }
        }
        class UInt64Converter : IConverter<UInt64>
        {
            public int Size { get { return sizeof(UInt64); } }
            public void GetBytes(UInt64 value, byte[] buffer, int index)
            {
                BitConverter.GetBytes(value).CopyTo(buffer, index);
            }
            public UInt64 GetValue(byte[] buffer, int index)
            {
                return BitConverter.ToUInt64(buffer, index);
            }
        }
        class Int64Converter : IConverter<Int64>
        {
            public int Size { get { return sizeof(Int64); } }
            public void GetBytes(Int64 value, byte[] buffer, int index)
            {
                BitConverter.GetBytes(value).CopyTo(buffer, index);
            }
            public Int64 GetValue(byte[] buffer, int index)
            {
                return BitConverter.ToInt64(buffer, index);
            }
        }
        class SingleConverter : IConverter<Single>
        {
            public int Size { get { return sizeof(Single); } }
            public void GetBytes(Single value, byte[] buffer, int index)
            {
                BitConverter.GetBytes(value).CopyTo(buffer, index);
            }
            public Single GetValue(byte[] buffer, int index)
            {
                return BitConverter.ToSingle(buffer, index);
            }
        }
        class DoubleConverter : IConverter<Double>
        {
            public int Size { get { return sizeof(Double); } }
            public void GetBytes(Double value, byte[] buffer, int index)
            {
                BitConverter.GetBytes(value).CopyTo(buffer, index);
            }
            public Double GetValue(byte[] buffer, int index)
            {
                return BitConverter.ToDouble(buffer, index);
            }
        }


        /// <summary>
        /// This indicates to the frame that no network to host conversion should be
        /// done on this field.
        /// </summary>
        public interface INoNetworkToHostConversion { }

        /// <summary>
        /// Convert to a UTF-8 fixed width string (padded with \0).
        /// </summary>
        public class UTF8FixedWidthConverter : IConverter<String>, INoNetworkToHostConversion
        {
            public UTF8FixedWidthConverter(int size)
            {
                _size = size;
            }
            private int _size;
            public int Size { get { return _size; } }
            public void GetBytes(String value, byte[] buffer, int index)
            {
                var bytes = new byte[_size];
                Encoding.UTF8.GetBytes(value, 0, value.Length, bytes, 0);
                bytes.CopyTo(buffer, index);
            }
            public virtual String GetValue(byte[] buffer, int index)
            {
                int length;
                for (length = _size; (length > 0) && (buffer[index + length - 1] == 0); length--) ;
                return Encoding.UTF8.GetString(buffer, index, length);
            }
        }
    }
}
