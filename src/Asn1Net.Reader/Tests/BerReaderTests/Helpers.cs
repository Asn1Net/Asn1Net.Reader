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
using System.IO;
using System.Linq;

namespace Net.Asn1.Reader.Tests.BerReaderTests
{
    /// <summary>
    /// Helper class for test
    /// </summary>
    internal class Helpers
    {
        /// <summary>
        /// Instantiate BerReader class with some data to parse.
        /// </summary>
        /// <param name="data">Data to parse by BerReader.</param>
        /// <returns>BerReader instance.</returns>
        internal static BerReader ReaderFromData(byte[] data)
        {
            var stream = new MemoryStream(data);
            var reader = new BerReader(stream);

            return reader;
        }

        /// <summary>
        /// Transform examples used in tests from string to bytes to process by BerReader.
        /// </summary>
        /// <param name="example">String representation (mostly hex string and some comments) of ASN.1 structure to be parser by BerReader.</param>
        /// <returns>Byte array of ASN.1 structure.</returns>
        internal static byte[] GetExampleBytes(string example)
        {
            example = string.Join(" ",
                example.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(l => l.Split(';').First()));

            var codes = example.Split(new[] { ' ', '|' }, StringSplitOptions.RemoveEmptyEntries);

            return codes.Select(c => Convert.ToByte(c, 16)).ToArray();
        }
    }
}
