///////////////////////////////////////////////////////////////////////////////
//  COPYRIGHT (c) 2011 Schweitzer Engineering Laboratories, Pullman, WA
//////////////////////////////////////////////////////////////////////////////

using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SEL.BinaryFormatter;
using SEL;
using System.Collections;

namespace Server.Common.UnitTests
{
    [TestClass]
    public class BinaryFormatter
    {
        /////////////////////////////////////////////////////////////////////
        // Test all default fields and their associated converters.
        /////////////////////////////////////////////////////////////////////
        
        public class AllDefaultFields : Frame
        {
            public AllDefaultFields(bool networkToHostConversion) : base(networkToHostConversion) { }
            public AllDefaultFields(byte[] buffer, bool networkToHostConversion) : base(buffer, networkToHostConversion) { }
            public Field<Byte> ByteField = new Field<Byte>();
            public Field<UInt16> UInt16Field = new Field<UInt16>();
            public Field<Int16> Int16Field = new Field<Int16>();
            public Field<UInt32> UInt32Field = new Field<UInt32>();
            public Field<Int32> Int32Field = new Field<Int32>();
            public Field<UInt64> UInt64Field = new Field<UInt64>();
            public Field<Int64> Int64Field = new Field<Int64>();
            public Field<Single> SingleField = new Field<Single>();
            public Field<Double> DoubleField = new Field<Double>();
            public Field<String> StringField = new Field<String>(new UTF8FixedWidthConverter(4));

            // For code coverage...
            public Int16 ANonFieldField = 35;
        }

        [TestMethod]
        public void WriteLittleEndian()
        {
            Assert.IsTrue(BitConverter.IsLittleEndian);
            var frame = new AllDefaultFields(false);
            SetFields(frame);
            CheckFrame(LittleEndian, frame);
        }

        [TestMethod]
        public void ReadLittleEndian()
        {
            var frame = new AllDefaultFields(LittleEndian, false);
            CheckFields(frame);
        }

        [TestMethod]
        public void WriteNetworkOrder()
        {
            var frame = new AllDefaultFields(true);
            SetFields(frame);
            CheckFrame(BigEndianNetworkOrder, frame);
        }

        [TestMethod]
        public void FrameWriteToStreamTest()
        {
            var frame = new AllDefaultFields(true);
            frame.Buffer = BigEndianNetworkOrder;

            System.IO.MemoryStream stream = new System.IO.MemoryStream();

            frame.WriteToStream(stream);
        }

        [TestMethod]
        public void ReadNetworkOrder()
        {
            var frame = new AllDefaultFields(true);
            frame.Buffer = BigEndianNetworkOrder;
            CheckFields(frame);

            // Verify implicit cast operator.
            int x = frame.Int32Field;
            Assert.AreEqual(2000000000, x);

            Assert.AreEqual("30000", frame.Int16Field.ToString());
        }

        private static void SetFields(AllDefaultFields frame)
        {
            frame.ByteField.Value = 1;
            frame.UInt16Field.Value = 60000;
            frame.Int16Field.Value = 30000;
            frame.UInt32Field.Value = 4000000000;
            frame.Int32Field.Value = 2000000000;
            frame.UInt64Field.Value = 18000000000000000000;
            frame.Int64Field.Value = 9000000000000000000;
            frame.SingleField.Value = 100.00000001f;
            frame.DoubleField.Value = 100.000000000000000001;
            frame.StringField.Value = "Hey!";
        }
        private static void CheckFields(AllDefaultFields frame)
        {
            Assert.AreEqual(frame.ByteField.Value, 1);
            Assert.AreEqual(frame.UInt16Field.Value, 60000);
            Assert.AreEqual(frame.Int16Field.Value, 30000);
            Assert.AreEqual(frame.UInt32Field.Value, 4000000000);
            Assert.AreEqual(frame.Int32Field.Value, 2000000000);
            Assert.AreEqual(frame.UInt64Field.Value, 18000000000000000000);
            Assert.AreEqual(frame.Int64Field.Value, 9000000000000000000);
            Assert.AreEqual(frame.SingleField.Value, 100.00000001f);
            Assert.AreEqual(frame.DoubleField.Value, 100.000000000000000001);
            Assert.AreEqual(frame.StringField.Value, "Hey!");
        }
        static readonly byte[] LittleEndian = new byte[45]
        {
            0x01,
            0x60, 0xea,
            0x30, 0x75,
            0x00, 0x28, 0x6b, 0xee,
            0x00, 0x94, 0x35, 0x77,
            0x00, 0x00, 0x08, 0xc5, 0xa1, 0xd8, 0xcc, 0xf9,
            0x00, 0x00, 0x84, 0xe2, 0x50, 0x6c, 0xe6, 0x7c,
            0x0, 0x0, 0xc8, 0x42, 
            0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x59, 0x40,
            0x48, 0x65, 0x79, 0x21
        };
        static readonly byte[] BigEndianNetworkOrder = new byte[45]
        {
            0x01,
            0xea, 0x60,
            0x75, 0x30,
            0xee, 0x6b, 0x28, 0x00,
            0x77, 0x35, 0x94, 0x00,
            0xf9, 0xcc, 0xd8, 0xa1, 0xc5, 0x08, 0x00, 0x00,
            0x7C, 0xE6, 0x6C, 0x50, 0xE2, 0x84, 0x00, 0x00,
            0x42, 0xc8, 0x0, 0x0, 
            0x40, 0x59, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0,
            0x48, 0x65, 0x79, 0x21
        };


        /////////////////////////////////////////////////////////////////////
        // Test all bitmapped fields.
        /////////////////////////////////////////////////////////////////////
        
        public class AllBitmappedFields : Frame
        {
            public AllBitmappedFields() : base(true) { }
            public My1Byte a1Byte = new My1Byte();
            public My2Byte a2Byte = new My2Byte();
            public My4Byte a4Byte = new My4Byte();
        }
        public class My1Byte : Bitmapped1Byte
        {
            public Bit ABit = new Bit();
            public BitField ABitField = new BitField(7);
        }
        public class My2Byte : Bitmapped2Byte
        {
            public BitField ABitField = new BitField(15);
            public Bit ABit = new Bit();
        }
        public class My4Byte : Bitmapped4Byte
        {
            public Bit ABit = new Bit();
            public BitField ABitField = new BitField(30);
            public Bit AnotherBit = new Bit();
        }

        [TestMethod]
        public void WriteBitmapped()
        {
            var allBitmapped = new AllBitmappedFields();
            allBitmapped.a1Byte.ABit.BitValue = true;
            allBitmapped.a1Byte.ABitField.Value = 0x41;
            allBitmapped.a2Byte.ABit.BitValue = false;
            allBitmapped.a2Byte.ABitField.Value = 0x3ffe;
            allBitmapped.a4Byte.ABit.BitValue = false;
            allBitmapped.a4Byte.ABitField.Value = 0x2aaad555;
            allBitmapped.a4Byte.AnotherBit.BitValue = false;
            CheckFrame(GoodBitmapped, allBitmapped);
        }

        [TestMethod]
        public void ReadBitmapped()
        {
            var allBitmapped = new AllBitmappedFields();
            allBitmapped.Buffer = GoodBitmapped;
            Assert.AreEqual(true, allBitmapped.a1Byte.ABit);
            Assert.AreEqual((UInt32)0x41, allBitmapped.a1Byte.ABitField);
            Assert.AreEqual(false, allBitmapped.a2Byte.ABit.BitValue);
            Assert.AreEqual((UInt32)0x3ffe, allBitmapped.a2Byte.ABitField);
            Assert.AreEqual(false, allBitmapped.a4Byte.ABit);
            Assert.AreEqual((UInt32)0x2aaad555, allBitmapped.a4Byte.ABitField);
            Assert.AreEqual(false, allBitmapped.a4Byte.ABit.BitValue);            
        }
        static readonly byte[] GoodBitmapped = new byte[7]
        {
            0x83,
            0x3f, 0xfe,
            0x55, 0x55, 0xaa, 0xaa
        };



        /////////////////////////////////////////////////////////////////////
        // Test a complex frame.
        /////////////////////////////////////////////////////////////////////        

        public class BaseFrame : Frame
        {
            public BaseFrame() : base(true) { }
            public Field<UInt16> Number = new Field<UInt16>();
        }

        public class ComplexFrame : BaseFrame
        {
            public ComplexFrame()
            { 
                SomeBytes = new FieldArray<byte>(() => Number.Value);
                Structures = new FieldStructureArray<ComplexStructure>(() => Number.Value);
            }
            public FieldArray<byte> SomeBytes;
            public FieldStructureArray<ComplexStructure> Structures;
        }

        public class ComplexStructure : FieldStructure
        {
            public ComplexStructure()
            {
                ManyBitmappeds  = new BitmappedFieldArray<MyBitmappedField>(() => Number.Value);
            }
            public Field<Int16> Number = new Field<Int16>();
            public MyBitmappedField OneBitmapped = new MyBitmappedField();
            public BitmappedFieldArray<MyBitmappedField> ManyBitmappeds;
        }

        public class MyBitmappedField : Bitmapped1Byte
        {
            public BitField AThreeBitField = new BitField(3);
            public Bit ABit = new Bit();
            public BitField AFourBitField = new BitField(4);

            // For code coverage...
            public int NonField = 5;
        }

        [TestMethod]
        public void WriteComplex()
        {
            var complex = new ComplexFrame();
            Assert.AreEqual(2, complex.Size);
            complex.Number.Value = 1;
            complex.Compile();
            Assert.AreEqual(2 + 1 + (3), complex.Size);
            complex.Structures[0].Number.Value = 1;
            complex.Compile();
            Assert.AreEqual(2 + 1 + (3 + 1), complex.Size);
            complex.Number.Value = 2;
            complex.Compile();
            complex.Structures[0].Number.Value = 3;
            complex.Compile();
            complex.Structures[1].Number.Value = 1;
            complex.Compile();
            Assert.AreEqual(2 + 2 + (3 + 3) + (3 + 1), complex.Size);

            complex.SomeBytes[0] = 0x42;
            complex.SomeBytes[1] = 0x24;
            SetBitmapped(complex.Structures[0].OneBitmapped, 10, true, 3);
            SetBitmapped(complex.Structures[0].ManyBitmappeds[0], 15, false, 0);
            SetBitmapped(complex.Structures[0].ManyBitmappeds[1], 14, true, 1);
            SetBitmapped(complex.Structures[0].ManyBitmappeds[2], 13, false, 2);
            SetBitmapped(complex.Structures[1].OneBitmapped, 0, false, 0);
            SetBitmapped(complex.Structures[1].ManyBitmappeds[0], 15, true, 7);
            CheckFrame(ComplexBytes, complex);

            complex.Number.Value = 0;
            complex.Compile();
            Assert.AreEqual(2, complex.Size);            
        }

        [TestMethod]
        public void ReadComplex()
        {
            var complex = new ComplexFrame();
            complex.Buffer = ComplexBytes;
            complex.Compile();

            // Test for GetEnumerator method
            //(complex as IEnumerable).GetEnumerator();

            Assert.AreEqual(2 + 2 + (3 + 3) + (3 + 1), complex.Size);

            Assert.AreEqual(0x42, complex.SomeBytes[0]);
            Assert.AreEqual(0x24, complex.SomeBytes[1]);
            CheckBitmapped(complex.Structures[0].OneBitmapped, 10, true, 3);
            CheckBitmapped(complex.Structures[0].ManyBitmappeds[0], 15, false, 0);
            CheckBitmapped(complex.Structures[0].ManyBitmappeds[1], 14, true, 1);
            CheckBitmapped(complex.Structures[0].ManyBitmappeds[2], 13, false, 2);
            CheckBitmapped(complex.Structures[1].OneBitmapped, 0, false, 0);
            CheckBitmapped(complex.Structures[1].ManyBitmappeds[0], 15, true, 7);
        }

        private void SetBitmapped(MyBitmappedField myBitmappedField, uint fourBit, bool bit, uint threeBit)
        {
            myBitmappedField.ABit.BitValue = bit;
            myBitmappedField.AFourBitField.Value = fourBit;
            myBitmappedField.AThreeBitField.Value = threeBit;            
        }

        private void CheckBitmapped(MyBitmappedField myBitmappedField, uint fourBit, bool bit, uint threeBit)
        {
            Assert.AreEqual(bit, myBitmappedField.ABit.BitValue);
            Assert.AreEqual(bit, myBitmappedField.ABit);
            Assert.AreEqual(fourBit, myBitmappedField.AFourBitField.Value);
            Assert.AreEqual(threeBit, myBitmappedField.AThreeBitField);
        }

        static readonly byte[] ComplexBytes = new byte[14]
        {
            0x00, 0x02,
            0x42, 0x24,
            0x00, 0x03, 0xab, 0xf0, 0xe9, 0xd2,
            0x00, 0x01, 0x00, 0xff
        };


        /////////////////////////////////////////////////////////////////////
        // Misc. tests to hit remaining corner cases.
        /////////////////////////////////////////////////////////////////////        
        [TestMethod]
        public void TestOffsetAtEnd()
        {
            var complex = new ComplexFrame();
            complex.Number.Value = 4;
            complex.Compile();
            complex.Structures[0].Number.Value = 2;
            complex.Compile();
            Assert.AreEqual(2, complex.Number.OffsetAtEnd);
            Assert.AreEqual(2 + 4, complex.SomeBytes.OffsetAtEnd);
            Assert.AreEqual(complex.Size, complex.Structures.OffsetAtEnd);
            Assert.AreEqual(2 + 4 + (2), complex.Structures[0].Number.OffsetAtEnd);
            Assert.AreEqual(2 + 4 + (2 + 1 + 2) + 3 + 3 + 2, complex.Structures[3].Number.OffsetAtEnd);
        }

        [TestMethod]
        public void TestEnumerators()
        {
            var complex = new ComplexFrame();
            complex.Number.Value = 2;
            complex.Compile();
            complex.Structures[0].Number.Value = 1;
            complex.Structures[1].Number.Value = 2;
            complex.Compile();
            
            complex.SomeBytes[0] = 87;
            complex.SomeBytes[1] = 209;
            CollectionAssert.AreEqual(new byte[] { 87, 209 }, 
                new List<byte>(((System.Collections.IEnumerable)complex.SomeBytes).Cast<byte>()));
            
            short i = 0;
            foreach (var structure in complex.Structures)
                structure.Number.Value = i++;

            i = 0;
            foreach (var structure in (IEnumerable)complex.Structures)
                Assert.AreEqual(i++, ((ComplexStructure)structure).Number);            
        }

        [TestMethod]
        public void TestIndexOutOfRange()
        {
            var complex = new ComplexFrame();
            complex.Number.Value = 1;
            complex.Compile();
            var x = complex.SomeBytes[20];
            // For now this returns 0 in these cases (instead of throwing),
            // because they can happen during a compile.
            Assert.AreEqual(0, x);
        }        
    
        // This method is being used by other test methods.
        private void CheckFrame(byte[] expected, Frame frame)
        {
            Assert.AreEqual(frame.Size, expected.Length, "Array length mismatch.");
            bool match = true;
            var mismatches = new StringBuilder("[ind] {expected} != {actual}\n");
            for (int i = 0; i < expected.Length; i++)
            {
                if (frame.Buffer[i] != expected[i])
                {
                    match = false;
                    mismatches.AppendLine(String.Format("[{0}] 0x{1:x2} != 0x{2:x2}", i, frame.Buffer[i], expected[i]));
                }
            }
            Assert.IsTrue(match, mismatches.ToString());
        }

        /// <summary>
        /// Just make sure that the GetEnumerator call does not throw exception.
        /// </summary>
        [TestMethod]
        public void GetEnumeratorTest()
        {
            FieldArray<UInt32> array = new FieldArray<UInt32>(null);
            (array as IEnumerable).GetEnumerator();            
        }

        [TestMethod]
        public void FieldConverterTest(){
            Field<int> f = new Field<int>();

            // This line should be fine
            IConverter<int> converter = Field<int>.DefaultConverter;

            bool exceptionThrown = false;

            // This line should throw an InvalidOperationException
            try
            {
                IConverter<ConsoleColor> converterBad = Field<ConsoleColor>.DefaultConverter;
            }
            catch (InvalidOperationException)
            {
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown);
        }
    }
}
