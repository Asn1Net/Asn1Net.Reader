/* 
 *  Asn1Net.Reader - Managed ASN.1 Parsing library
 *  Copyright (c) 2014-2016 Peter Polacko
 *  Author: Peter Polacko <peter.polacko+asn1net@gmail.com>
 *  
 *  Licensing for open source projects:
 *  Asn1Net.Reader is available under the terms of the GNU Affero General 
 *  Public License version 3 as published by the Free Software Foundation.
 *  Please see <http://www.gnu.org/licenses/agpl-3.0.html> for more details.
 *
 */

using System.IO;
using NUnit.Framework;

namespace Net.Asn1.Reader.Tests.BerReaderTests
{
    /// <summary>
    /// Test covering BerReader's ReadToEnd method
    /// </summary>
    [TestFixture]
    [Category("ReadToEndTests")]
    public class ReadToEndTests
    {
        /// <summary>
        /// Test simple ASN.1 structure. One root (sequence) with two children.
        /// </summary>
        [Test]
        public void OneRootSimpleTest()
        {
            var encoded = File.ReadAllBytes("asn1_sample_slim.asn1");
            // When
            var reader = Helpers.ReaderFromData(encoded);
            var result = reader.ReadToEnd();

            // there should be 
            // DocumentStart
            //      ->  Sequence
            //              ->  Integer
            //              ->  Utf8String
            Assert.IsTrue(result.ChildNodes.Count == 1);
            Assert.IsTrue(result.NodeType == Asn1NodeType.DocumentStart);
            Assert.IsTrue(result.ChildNodes[0].Identifier.Tag == Asn1Type.Sequence);
            Assert.IsTrue(result.ChildNodes[0].ChildNodes.Count == 2);
            Assert.IsTrue(result.ChildNodes[0].ChildNodes[0].Identifier.Tag == Asn1Type.Integer);
            Assert.IsTrue(result.ChildNodes[0].ChildNodes[0].ChildNodes.Count == 0);
            Assert.IsTrue(result.ChildNodes[0].ChildNodes[1].Identifier.Tag == Asn1Type.Utf8String);
            Assert.IsTrue(result.ChildNodes[0].ChildNodes[1].ChildNodes.Count == 0);
        }

        /// <summary>
        /// Test an ASN.1 structure that has two nodes at root level.
        /// </summary>
        [Test]
        public void MultiRootSimpleTest()
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
            var result = reader.ReadToEnd();
            // there should be 
            // DocumentStart
            //      ->  ObjectIdentifier
            //      ->  SET
            //          ->  Integer
            Assert.IsTrue(result.ChildNodes.Count == 2);
            Assert.IsTrue(result.NodeType == Asn1NodeType.DocumentStart);
            Assert.IsTrue(result.ChildNodes[0].Identifier.Tag == Asn1Type.ObjectIdentifier);
            Assert.IsTrue(result.ChildNodes[0].ChildNodes.Count == 0);
            Assert.IsTrue(result.ChildNodes[1].Identifier.Tag == Asn1Type.Set);
            Assert.IsTrue(result.ChildNodes[1].ChildNodes.Count == 1);
            Assert.IsTrue(result.ChildNodes[1].ChildNodes[0].Identifier.Tag == Asn1Type.Integer);
            Assert.IsTrue(result.ChildNodes[1].ChildNodes[0].ChildNodes.Count == 0);
        }

        /// <summary>
        /// Test a slightly complex scenario with two nodes at root level.
        /// </summary>
        [Test]
        public void MultiRootComplexTest()
        {
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
            var result = reader.ReadToEnd();
            // there should be 
            // DocumentStart
            //      ->  ObjectIdentifier
            //      ->  SET
            //          -> Sequence
            //              ->  Integer
            //              ->  Utf8String
            //              ->  Utf8String
            //              ->  Utf8String
            Assert.IsTrue(result.ChildNodes.Count == 2);
            Assert.IsTrue(result.NodeType == Asn1NodeType.DocumentStart);
            Assert.IsTrue(result.ChildNodes[0].Identifier.Tag == Asn1Type.ObjectIdentifier);
            Assert.IsTrue(result.ChildNodes[0].ChildNodes.Count == 0);
            Assert.IsTrue(result.ChildNodes[1].Identifier.Tag == Asn1Type.Set);
            Assert.IsTrue(result.ChildNodes[1].ChildNodes.Count == 1);
            Assert.IsTrue(result.ChildNodes[1].ChildNodes[0].Identifier.Tag == Asn1Type.Sequence);
            Assert.IsTrue(result.ChildNodes[1].ChildNodes[0].ChildNodes.Count == 4);
            Assert.IsTrue(result.ChildNodes[1].ChildNodes[0].ChildNodes[0].Identifier.Tag == Asn1Type.Integer);
            Assert.IsTrue(result.ChildNodes[1].ChildNodes[0].ChildNodes[0].ChildNodes.Count == 0);
            Assert.IsTrue(result.ChildNodes[1].ChildNodes[0].ChildNodes[1].Identifier.Tag == Asn1Type.Utf8String);
            Assert.IsTrue(result.ChildNodes[1].ChildNodes[0].ChildNodes[1].ChildNodes.Count == 0);
            Assert.IsTrue(result.ChildNodes[1].ChildNodes[0].ChildNodes[2].Identifier.Tag == Asn1Type.Utf8String);
            Assert.IsTrue(result.ChildNodes[1].ChildNodes[0].ChildNodes[2].ChildNodes.Count == 0);
            Assert.IsTrue(result.ChildNodes[1].ChildNodes[0].ChildNodes[3].Identifier.Tag == Asn1Type.Utf8String);
            Assert.IsTrue(result.ChildNodes[1].ChildNodes[0].ChildNodes[3].ChildNodes.Count == 0);
        }

        /// <summary>
        /// Test of a real life scenario. Chain was taken from SSL certificate of GitHub.
        /// </summary>
        [Test]
        public void P7ChainTest()
        {
            var encoded = File.ReadAllBytes("asn1_github_certificate_chain.asn1");
            // When
            using (var reader = Helpers.ReaderFromData(encoded))
            {
                var result = reader.ReadToEnd();

                // print
                string res = "+";
                const int depth = 1;
                foreach (var node in result.ChildNodes)
                {
                    res = reader.PrintStructure(node, res, depth, true);
                }
            }
        }
    }
}
