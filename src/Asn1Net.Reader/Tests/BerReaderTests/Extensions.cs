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

using System.Text;

namespace Net.Asn1.Reader.Tests.BerReaderTests
{
    internal static class Extensions
    {
        /// <summary>
        /// Prints byte array as HEX string.
        /// </summary>
        /// <param name="ba">Byte array to process.</param>
        /// <returns>HEX string representation of byte array.</returns>
        internal static string ToHexString(this byte[] ba)
        {
            var hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:X2}", b);
            return hex.ToString();
        }
    }
}