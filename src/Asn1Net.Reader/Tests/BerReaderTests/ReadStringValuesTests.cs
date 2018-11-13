/*
 *  Copyright 2012-2018 The Asn1Net Project
 *
 *  Licensed under the Apache License, Version 2.0 (the "License");
 *  you may not use this file except in compliance with the License.
 *  You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 */
/*
 *  Written for the Asn1Net project by:
 *  Peter Polacko <peter.polacko+asn1net@gmail.com>
 */

using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Net.Asn1.Reader.Tests.BerReaderTests
{
    /// <summary>
    /// Test covering BerReader's ReadToEnd method
    /// </summary>
    [TestFixture]
    [Category("ReadStringTests")]
    public class ReadStringValuesTests
    {
        [Test]
        [TestCase("0C 29 61 62 63 64 2B C4 BE C5 A1 C4 8D C5 A5 C5 BE C3 BD C3 A1 C3 AD C3 A9 C3 BA C3 A4 C3 B4 C2 A7 C5 88 C3 B3 C5 95 61 62 63 64",
            ExpectedResult = "abcd+ľščťžýáíéúäô§ňóŕabcd")]
        public string Utf8StringTest(string example)
        {
            // Given
            var encoded = Helpers.GetExampleBytes(example);

            // When
            var reader = Helpers.ReaderFromData(encoded);
            var node = reader.Read(true);

            var realValue = node.ReadContentAsString();
            return realValue;
        }

        [Test]
        [TestCase("12 09 31 32 33 34 35 36 37 38 39", ExpectedResult = "123456789")]
        public string NumericStringTest(string example)
        {
            // Given
            var encoded = Helpers.GetExampleBytes(example);

            // When
            var reader = Helpers.ReaderFromData(encoded);
            var node = reader.Read(true);

            var realValue = node.ReadContentAsNumericString();
            return realValue;
        }


        [Test]
        [TestCase("13 04 61 62 63 64", ExpectedResult = "abcd")]
        public string PrintableStringTest(string example)
        {
            // Given
            var encoded = Helpers.GetExampleBytes(example);

            // When
            var reader = Helpers.ReaderFromData(encoded);
            var node = reader.Read(true);

            var realValue = node.ReadContentAsPrintableString();
            return realValue;
        }

        [Test]
        [TestCase("14 29 61 62 63 64 2B C4 BE C5 A1 C4 8D C5 A5 C5 BE C3 BD C3 A1 C3 AD C3 A9 C3 BA C3 A4 C3 B4 C2 A7 C5 88 C3 B3 C5 95 61 62 63 64",
            ExpectedResult = "abcd+ľščťžýáíéúäô§ňóŕabcd")]
        public string T61StringTest(string example)
        {
            // Given
            var encoded = Helpers.GetExampleBytes(example);

            // When
            var reader = Helpers.ReaderFromData(encoded);
            var node = reader.Read(true);

            var realValue = node.ReadContentAsT61String();
            return realValue;
        }

        [Test]
        [TestCase("16 04 61 62 63 64",
            ExpectedResult = "abcd")]
        public string IA5StringTest(string example)
        {
            // Given
            var encoded = Helpers.GetExampleBytes(example);

            // When
            var reader = Helpers.ReaderFromData(encoded);
            var node = reader.Read(true);

            var realValue = node.ReadContentAsIA5String();
            return realValue;
        }

        [Test]
        [TestCase("19 29 61 62 63 64 2B C4 BE C5 A1 C4 8D C5 A5 C5 BE C3 BD C3 A1 C3 AD C3 A9 C3 BA C3 A4 C3 B4 C2 A7 C5 88 C3 B3 C5 95 61 62 63 64",
            ExpectedResult = "abcd+ľščťžýáíéúäô§ňóŕabcd")]
        public string GraphicStringTest(string example)
        {
            // Given
            var encoded = Helpers.GetExampleBytes(example);

            // When
            var reader = Helpers.ReaderFromData(encoded);
            var node = reader.Read(true);

            var realValue = node.ReadContentAsGraphicString();
            return realValue;
        }

        [Test]
        [TestCase("1B 29 61 62 63 64 2B C4 BE C5 A1 C4 8D C5 A5 C5 BE C3 BD C3 A1 C3 AD C3 A9 C3 BA C3 A4 C3 B4 C2 A7 C5 88 C3 B3 C5 95 61 62 63 64",
            ExpectedResult = "abcd+ľščťžýáíéúäô§ňóŕabcd")]
        public string GeneralStringTest(string example)
        {
            // Given
            var encoded = Helpers.GetExampleBytes(example);

            // When
            var reader = Helpers.ReaderFromData(encoded);
            var node = reader.Read(true);

            var realValue = node.ReadContentAsGeneralString();
            return realValue;
        }

        [Test]
        [TestCase("1E 32 00 61 00 62 00 63 00 64 00 2B 01 3E 01 61 01 0D 01 65 01 7E 00 FD 00 E1 00 ED 00 E9 00 FA 00 E4 00 F4 00 A7 01 48 00 F3 01 55 00 61 00 62 00 63 00 64 ",
            ExpectedResult = "abcd+ľščťžýáíéúäô§ňóŕabcd")]
        public string BmpStringTest(string example)
        {
            // Given
            var encoded = Helpers.GetExampleBytes(example);

            // When
            var reader = Helpers.ReaderFromData(encoded);
            var node = reader.Read(true);

            var realValue = node.ReadContentAsBmpString();
            return realValue;
        }


        [Test]
        [TestCase("1C 64 00 00 00 61 00 00 00 62 00 00 00 63 00 00 00 64 00 00 00 2B 00 00 01 3E 00 00 01 61 00 00 01 0D 00 00 01 65 00 00 01 7E 00 00 00 FD 00 00 00 E1 00 00 00 ED 00 00 00 E9 00 00 00 FA 00 00 00 E4 00 00 00 F4 00 00 00 A7 00 00 01 48 00 00 00 F3 00 00 01 55 00 00 00 61 00 00 00 62 00 00 00 63 00 00 00 64 ",
            ExpectedResult = "abcd+ľščťžýáíéúäô§ňóŕabcd")]
        public string UniversalStringTest(string example)
        {
            // Given
            var encoded = Helpers.GetExampleBytes(example);

            // When
            var reader = Helpers.ReaderFromData(encoded);
            var node = reader.Read(true);

            var realValue = node.ReadContentAsUniversalString();
            return realValue;
        }
    }
}
