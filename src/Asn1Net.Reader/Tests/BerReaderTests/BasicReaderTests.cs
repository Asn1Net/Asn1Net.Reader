// /*
//  *  Asn1Utils - Managed .NET utilities to parse or write asn.1 structures
//  *  Copyright (c) 2014-2015 Peter Polacko
//  *  Author: Peter Polacko <peter.polacko+asn1utils@gmail.com>
//  *
//  *  Licensing for open source projects:
//  *  Asn1Utils is available under the terms of the GNU Affero General 
//  *  Public License version 3 as published by the Free Software Foundation.
//  *  Please see <http://www.gnu.org/licenses/agpl-3.0.html> for more details.
//  *
//  */

using System;
using System.IO;
using System.Linq;
using System.Numerics;
using NUnit.Framework;

namespace Net.Asn1.Reader.Tests.BerReaderTests
{
    /// <summary>
    /// Basic test of BerReader functionality.
    /// </summary>
    [TestFixture]
    [Category("BasicReaderTests")]
    public class BasicReaderTests
    {
        /// <summary>
        /// Test if BerReade will not parse given data when instantiated.
        /// </summary>
        [Test]
        public void DoesNotTouchStreamBeforeRead()
        {
            // Given
            var memoryStream = new MemoryStream();

            // When
            var berReader = new BerReader(memoryStream);

            // Then
            Assert.AreEqual(0, memoryStream.Position);
        }

        /// <summary>
        /// BerReader will make virtual node DocumentStart. All processing will start from this node.
        /// </summary>
        [Test]
        public void NodeTypeIsInitiallyNone()
        {
            // Given
            var memoryStream = new MemoryStream();

            // When
            var berReader = new BerReader(memoryStream);

            // Then
            Assert.AreEqual(Asn1NodeType.DocumentStart, berReader.CurrentNode.NodeType);
        }

        /// <summary>
        /// BerReader will make virtual node DocumentStart. All processing will start from this node.
        /// </summary>
        [Test]
        public void DisposeTests()
        {
            // Given
            // Example taken from: http://msdn.microsoft.com/en-us/library/bb540809%28v=vs.85%29.aspx
            const string example = @"
                06 09                                ; OBJECT_ID (9 Bytes)
                |  2b 06 01 04 01 82 37 15  14       ;   1.3.6.1.4.1.311.21.20 
            ";

            var encoded = Helpers.GetExampleBytes(example);

            // Given
            var memoryStream = new MemoryStream(encoded);

            // When
            using (var berReader = new BerReader(memoryStream))
            {
                // Then
                Assert.AreEqual(Asn1NodeType.DocumentStart, berReader.CurrentNode.NodeType);    
            }

            bool objectDisposed = false;
            try
            {
                //should throw ObjectDisposedException
                var position = memoryStream.Position;
            }
            catch (ObjectDisposedException)
            {
                objectDisposed = true;
            }

            Assert.IsTrue(objectDisposed); 


            // Given
            memoryStream = new MemoryStream(encoded);

            // When
            using (var berReader = new BerReader(memoryStream, true))
            {
                // Then
                Assert.AreEqual(Asn1NodeType.DocumentStart, berReader.CurrentNode.NodeType);
            }

            Assert.IsTrue(memoryStream.Position == 0); //should NOT throw ObjectDisposedException
        }

        /// <summary>
        /// Test parsing of object identifier from ASN.1 but do not read the value yet.
        /// </summary>
        [Test]
        public void BeforeObjectIdenitifer()
        {
            // Given
            // Example taken from: http://msdn.microsoft.com/en-us/library/bb540809%28v=vs.85%29.aspx
            const string example = @"
                06 09                                ; OBJECT_ID (9 Bytes)
                |  2b 06 01 04 01 82 37 15  14       ;   1.3.6.1.4.1.311.21.20 
            ";

            var encoded = Helpers.GetExampleBytes(example);

            // When
            var reader = Helpers.ReaderFromData(encoded);
            var node = reader.Read();

            // Then
            Assert.AreEqual(Asn1NodeType.Primitive, node.NodeType);
            Assert.AreEqual(Asn1Type.ObjectIdentifier, node.Identifier.Tag);
            Assert.AreEqual(9, node.Length);
        }

        /// <summary>
        /// Test reading value of object identifier.
        /// </summary>
        [Test]
        public void ReadObjectIdenitifer()
        {
            // Given
            // Example taken from: http://msdn.microsoft.com/en-us/library/bb540809%28v=vs.85%29.aspx
            const string example = @"
                06 09                                ; OBJECT_ID (9 Bytes)
                |  2b 06 01 04 01 82 37 15  14       ;   1.3.6.1.4.1.311.21.20 
            ";

            var encoded = Helpers.GetExampleBytes(example);

            // When
            var reader = Helpers.ReaderFromData(encoded);
            var node = reader.Read();
            node.RawValue = reader.ReadContentAsBuffer(node);
            var oid = node.ReadContentAsObjectIdentifier();

            // Then
            Assert.AreEqual("1.3.6.1.4.1.311.21.20", oid);
        }

        /// <summary>
        /// Test reading value of Boolean.
        /// </summary>
        [Test]
        public void ReadBoolean()
        {
            var encoded = File.ReadAllBytes("bool.asn1");

            // When
            var reader = Helpers.ReaderFromData(encoded);
            var node = reader.Read();
            node.RawValue = reader.ReadContentAsBuffer(node);
            var isTrue = node.ReadContentAsBoolean();

            // Then
            Assert.IsTrue(isTrue);
        }

        /// <summary>
        /// Test reading value of bit string.
        /// </summary>
        [Test]
        public void ReadBitString()
        {
            var encoded = File.ReadAllBytes("bitstring.asn1");

            // When
            var reader = Helpers.ReaderFromData(encoded);
            var node = reader.Read();
            node.RawValue = reader.ReadContentAsBuffer(node);
            var allContent = reader.ReadContentAsBuffer(node);
            var skippedFirstByte = allContent.Skip(1);
            var res = node.ReadContentAsBitString();

            // Then
            Assert.IsTrue(res.SequenceEqual(skippedFirstByte));
        }

        /// <summary>
        /// Test reading value of REAL.
        /// </summary>
        [Test]
        [TestCase("09 05 02 33 2E 31 34", 5, 3.14)]
        [TestCase("09 03 80 FB 05", 3, 0.15625)]
        [TestCase("09 03 90 FE 0A", 3, 0.15625)]
        [TestCase("09 03 AC FE 05", 3, 0.15625)]
        [TestCase("09 01 40", 1, double.PositiveInfinity)]
        [TestCase("09 01 41", 1, double.NegativeInfinity)]
        [TestCase("09 01 42", 1, double.NaN)]
        [TestCase("09 01 43", 1, -0.0d)]
        public void ReadReal(string example, int nodeLenth, double result)
        {
            // Given
            var encoded = Helpers.GetExampleBytes(example);

            // When
            var reader = Helpers.ReaderFromData(encoded);
            var node = reader.Read(true);

            var realValue = node.ReadContentAsReal();

            // Then
            Assert.AreEqual(Asn1Type.Real, node.Identifier.Tag);
            Assert.AreEqual(nodeLenth, node.Length);
            Assert.AreEqual(result, realValue);
        }

        /// <summary>
        /// Test reading SET node
        /// </summary>
        [Test]
        public void ReadSetStart()
        {
            // Given
            // Example taken from: http://msdn.microsoft.com/en-us/library/bb540809%28v=vs.85%29.aspx
            const string example = @"
                06 09                                ; OBJECT_ID (9 Bytes)
                |  2b 06 01 04 01 82 37 15  14       ;   1.3.6.1.4.1.311.21.20 
                31 03                                ; SET (03 Bytes)
                   02 01                             ; INTEGER (1 Bytes)
                   |  09
            ";

            var encoded = Helpers.GetExampleBytes(example);

            // When
            var reader = Helpers.ReaderFromData(encoded);
            reader.Read();
            var node = reader.Read();

            // Then
            Assert.AreEqual(Asn1Type.Set, node.Identifier.Tag);
            Assert.AreEqual(0x03, node.Length);
            Assert.IsTrue(node.IsConstructed);
        }

        /// <summary>
        /// Test reading through SET node. Values of nodes are not being read.
        /// </summary>
        [Test]
        public void ReadSetEnd()
        {
            // Given
            // Example taken from: http://msdn.microsoft.com/en-us/library/bb540809%28v=vs.85%29.aspx
            const string example = @"
                06 09                                ; OBJECT_ID (9 Bytes)
                |  2b 06 01 04 01 82 37 15  14       ;   1.3.6.1.4.1.311.21.20 
                31 03                                ; SET (03 Bytes)
                   02 01                             ; INTEGER (1 Bytes)
                   |  09
            ";

            var encoded = Helpers.GetExampleBytes(example);

            // When
            var reader = Helpers.ReaderFromData(encoded);
            reader.Read(); // OID
            reader.Read(); // Start Set
            reader.Read(); // Integer
            var endNode = reader.Read(); // End Set

            // Then
            Assert.AreEqual(Asn1Type.Set, endNode.Identifier.Tag);
            Assert.AreEqual(0, endNode.Length);
            Assert.IsTrue(endNode.IsConstructed);
            Assert.AreEqual(Asn1NodeType.ConstructedEnd, endNode.NodeType);
        }

        /// <summary>
        /// Test processing complex ASN.1 structure. Also read node's value.
        /// </summary>
        [Test]
        public void ComplexParseTest()
        {
            // Given
            // Example taken from: http://msdn.microsoft.com/en-us/library/bb540809%28v=vs.85%29.aspx
            const string example = @"
                06 09                                ; OBJECT_ID (9 Bytes)
                |  2b 06 01 04 01 82 37 15  14       ;   1.3.6.1.4.1.311.21.20 
                31 4a                                ; SET (4a Bytes)
                   30 48                             ; SEQUENCE (48 Bytes)
                      02 01                          ; INTEGER (1 Bytes)
                      |  09
                      0c 23                          ; UTF8_STRING (23 Bytes)
                      |  76 69 63 68 33 64 2e 6a     ;   vich3d.j
                      |  64 6f 6d 63 73 63 2e 6e     ;   domcsc.n
                      |  74 74 65 73 74 2e 6d 69     ;   ttest.mi
                      |  63 72 6f 73 6f 66 74 2e     ;   crosoft.
                      |  63 6f 6d                    ;   com
                      0c 15                          ; UTF8_STRING (15 Bytes)
                      |  4a 44 4f 4d 43 53 43 5c     ;   JDOMCSC\
                      |  61 64 6d 69 6e 69 73 74     ;   administ
                      |  72 61 74 6f 72              ;   rator
                      0c 07                          ; UTF8_STRING (7 Bytes)
                         63 65 72 74 72 65 71        ;   certreq
            ";

            var encoded = Helpers.GetExampleBytes(example);
            var reader = Helpers.ReaderFromData(encoded);

            // Then
            AssertReadSod(reader);
            AssertReadPri(reader, 0, Asn1Type.ObjectIdentifier, 9, "1.3.6.1.4.1.311.21.20");
            AssertReadSta(reader, 0, Asn1Type.Set, 0x4a);
            AssertReadSta(reader, 1, Asn1Type.Sequence, 0x48);
            AssertReadPri(reader, 2, Asn1Type.Integer, 1, new BigInteger(0x09));
            AssertReadPri(reader, 2, Asn1Type.Utf8String, 0x23, "vich3d.jdomcsc.nttest.microsoft.com");
            AssertReadPri(reader, 2, Asn1Type.Utf8String, 0x15, @"JDOMCSC\administrator");
            AssertReadPri(reader, 2, Asn1Type.Utf8String, 0x07, @"certreq");
            AssertReadEnd(reader, 2, Asn1Type.Sequence);
            AssertReadEnd(reader, 1, Asn1Type.Set);
            AssertReadEod(reader);
        }

        #region Helper Asserts

        /// <summary>
        /// Assert DocumentStart and move to next node.
        /// </summary>
        /// <param name="reader">BerReader instance.</param>
        private void AssertReadSod(BerReader reader)
        {
            Assert.AreEqual(Asn1NodeType.DocumentStart, reader.CurrentNode.NodeType);
            Assert.AreEqual(Asn1Type.Eoc, reader.CurrentNode.Identifier.Tag);
            Assert.AreEqual(0, reader.CurrentNode.Length);
            Assert.IsTrue(reader.CurrentNode.IsConstructed);
            //Assert.AreEqual(Asn1Type.);
            reader.Read();
        }

        /// <summary>
        /// Assert End of ASN.1 structure.
        /// </summary>
        /// <param name="reader">BerReader instance.</param>
        private void AssertReadEod(BerReader reader)
        {
            Assert.IsTrue(reader.CurrentNode.NodeType == Asn1NodeType.DocumentEnd);
            Assert.AreEqual(Asn1NodeType.DocumentEnd, reader.CurrentNode.NodeType);
            Assert.AreEqual(Asn1Type.Eoc, reader.CurrentNode.Identifier.Tag);
            Assert.AreEqual(0, reader.CurrentNode.Length);
            Assert.IsTrue(reader.CurrentNode.IsConstructed);
        }

        /// <summary>
        /// Assert Constructed type of node, i.e. SEQUENCE, SET. Then move to next node.
        /// </summary>
        /// <param name="reader">BerReader instance.</param>
        /// <param name="level">Depth of node in ASN.1 structure.</param>
        /// <param name="type">Type of given node to assert.</param>
        /// <param name="length">Length of given node to assert.</param>
        private static void AssertReadSta(BerReader reader, int level, Asn1Type type, int length)
        {
            Assert.AreEqual(level, reader.Depth, "Depth");
            Assert.AreEqual(Asn1NodeType.ConstructedStart, reader.CurrentNode.NodeType, "Node Tag");
            Assert.AreEqual(type, reader.CurrentNode.Identifier.Tag, "Tag");
            Assert.AreEqual(length, reader.CurrentNode.Length, "Length");
            Assert.IsTrue(reader.CurrentNode.IsConstructed, "IsConstructed");
            Assert.IsFalse(reader.CurrentNode.NodeType == Asn1NodeType.DocumentEnd, "EndOfDocument");

            reader.Read();
        }

        /// <summary>
        /// Assert end of Constructed type of node, i.e. SEQUENCE, SET. Then move to next node.
        /// </summary>
        /// <param name="reader">BerReader instance.</param>
        /// <param name="level">Depth of node in ASN.1 structure.</param>
        /// <param name="type">Type of given node to assert.</param>
        private static void AssertReadEnd(BerReader reader, int level, Asn1Type type)
        {
            Assert.AreEqual(level, reader.Depth, "Depth");
            Assert.AreEqual(Asn1NodeType.ConstructedEnd, reader.CurrentNode.NodeType, "Node Tag");
            Assert.AreEqual(type, reader.CurrentNode.Identifier.Tag, "Tag");
            Assert.AreEqual(0, reader.CurrentNode.Length, "Length");
            Assert.IsTrue(reader.CurrentNode.IsConstructed, "IsConstructed");
            Assert.IsFalse(reader.CurrentNode.NodeType == Asn1NodeType.DocumentEnd, "EndOfDocument");

            reader.Read();
        }

        /// <summary>
        /// Assert Primary type of node. Also read the value of the node and assert with given value. Then move to next node.
        /// </summary>
        /// <param name="reader">BerReader instance.</param>
        /// <param name="level">Depth of node in ASN.1 structure.</param>
        /// <param name="type">Type of given node to assert.</param>
        /// <param name="length">Length of given node to assert.</param>
        /// <param name="value">Value of the node to assert.</param>
        private static void AssertReadPri(BerReader reader, int level, Asn1Type type, int length, object value)
        {
            Assert.AreEqual(level, reader.Depth, "Depth");
            Assert.AreEqual(Asn1NodeType.Primitive, reader.CurrentNode.NodeType, "Node Tag");
            Assert.AreEqual(type, reader.CurrentNode.Identifier.Tag, "Tag");
            Assert.AreEqual(length, reader.CurrentNode.Length, "Length");
            Assert.IsFalse(reader.CurrentNode.IsConstructed, "IsConstructed");
            Assert.IsFalse(reader.CurrentNode.NodeType == Asn1NodeType.DocumentEnd, "EndOfDocument");

            reader.CurrentNode.RawValue = reader.ReadContentAsBuffer();

            switch (reader.CurrentNode.Identifier.Tag)
            {
                case Asn1Type.ObjectIdentifier:
                    Assert.AreEqual(value, reader.CurrentNode.ReadContentAsObjectIdentifier());
                    break;
                case Asn1Type.PrintableString:
                case Asn1Type.Ia5String:
                case Asn1Type.Utf8String:
                    Assert.AreEqual(value, reader.CurrentNode.ReadContentAsString());
                    break;
                case Asn1Type.GeneralizedTime:
                case Asn1Type.UtcTime:
                    Assert.AreEqual(value, reader.CurrentNode.ReadConventAsDateTimeOffset());
                    break;
                case Asn1Type.Integer:
                    Assert.AreEqual(value, reader.CurrentNode.ReadContentAsBigInteger());
                    break;
                case Asn1Type.Boolean:
                    Assert.AreEqual(value, reader.CurrentNode.ReadContentAsBoolean());
                    break;
                default:
                    Assert.AreEqual(value, reader.CurrentNode.RawValue);
                    break;
            }

            reader.Read();
        }
        #endregion
    }
}
