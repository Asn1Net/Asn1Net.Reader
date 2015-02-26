/*
 *  Asn1Net.Reader - Managed ASN.1 Parsing library
 *  Copyright (c) 2014-2015 Peter Polacko
 *  Author: Peter Polacko <peter.polacko+asn1net@gmail.com>
 *  
 *  Licensing for open source projects:
 *  Asn1Net.Reader is available under the terms of the GNU Affero General 
 *  Public License version 3 as published by the Free Software Foundation.
 *  Please see <http://www.gnu.org/licenses/agpl-3.0.html> for more details.
 *
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
