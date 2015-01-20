// /*
//  *  Asn1Net.Reader - Managed ASN.1 Parsing library
//  *  Copyright (c) 2014-2015 Peter Polacko
//  *  Author: Peter Polacko <peter.polacko+asn1net@gmail.com>
//  *
//  *  Licensing for open source projects:
//  *  Asn1Net.Reader is available under the terms of the GNU Affero General 
//  *  Public License version 3 as published by the Free Software Foundation.
//  *  Please see <http://www.gnu.org/licenses/agpl-3.0.html> for more details.
//  *
//  */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Net.Asn1.Reader
{
    /// <summary>
    /// Helper methods, mostly converters.
    /// </summary>
    public class Helpers
    {
        /// <summary>
        /// Regular expression for parsing UTCTime according to ITU-T X.690 recommendation
        /// </summary>
        private static Regex utcTimeRegex = new Regex(@"^(?<date>\d{6})(?<hour>([01][0-9])|(2[0-3]))(?<minute>[0-5][0-9])?(?<second>[0-5][0-9])Z$", RegexOptions.ECMAScript);

        /// <summary>
        /// Regular expression for parsing GeenralizedTime according to ITU-T X.690 recommendation
        /// </summary>
        private static Regex generalizedTimeRegex = new Regex(@"^(?<date>\d{8})(?<hour>([01][0-9])|(2[0-3]))(?<minute>[0-5][0-9])?(?<second>[0-5][0-9])(?<fraction>\.\d+)?Z$", RegexOptions.ECMAScript);

        /// <summary>
        /// Converts string value to DateTimeOffset. String value is representing UTCTime according to ITU-T X.680 recommendation
        /// </summary>
        /// <param name="time">UTCTime as string value.</param>
        /// <returns>Value converted to DateTimeOffset.</returns>
        public static DateTimeOffset ConvertFromUniversalTime(string time)
        {
            var match = utcTimeRegex.Match(time);
            if (match.Success == false)
                throw new FormatException("Time was not in correct format according to UTCTime ITU-T X.690 spec.");

            var newTime = time.Replace("Z", "+0000");

            var res = DateTimeOffset.ParseExact(newTime, "yyMMddHHmmsszzz", CultureInfo.InvariantCulture);
            return res;
        }

        /// <summary>
        /// Converts string value to DateTimeOffset. String value is representing GeneralizedTime according to ITU-T X.680 recommendation
        /// </summary>
        /// <param name="time">GeneralizedTime as string value.</param>
        /// <returns>Value converted to DateTimeOffset.</returns>
        public static DateTimeOffset ConvertFromGeneralizedTime(string time)
        {
            var match = generalizedTimeRegex.Match(time);
            if (match.Success == false)
                throw new FormatException("Time was not in correct format according to GeneralizedTime spec.");

            var hasFraction = match.Groups["fraction"].Success;
            var fractionPart = match.Groups["fraction"].Value;

            if (fractionPart.EndsWith("0"))
                throw new FormatException("Generalized time should not have fractions part with trailing zeros.");

            if (fractionPart.Length - 1 > 7) // platform limit for .NET DateTime.ParseExact
                throw new NotSupportedException("Can only process up to 7 digits in fraction part of Generalized time.");

            var newTime = time.Replace("Z", "+0000");
            var format = (hasFraction == false)
                ? "yyyyMMddHHmmsszzz"
                : String.Format("yyyyMMddHHmmss.{0}zzz", new String('f', fractionPart.Length - 1));

            var res = DateTimeOffset.ParseExact(newTime, format, CultureInfo.InvariantCulture);
            return res;
        }


        /// <summary>
        /// Parse Oid value from Byte[]. More information here http://www.itu.int/rec/dologin_pub.asp?lang=e&id=T-REC-X.690-200811-I!!PDF-E&type=items
        /// </summary>
        /// <param name="oid">Oid value in byte array.</param>
        /// <returns>Oid in string format</returns>
        public static string ParseEncodedOid(byte[] oid)
        {
            var oidValues = new List<int>();

            // first byte
            // The numerical value of the first subidentifier is derived from the values of the first two object identifier 
            //  components in the object identifier value being encoded, using the formula: (X*40) + Y
            if (oid.Length > 0)
            {
                oidValues.Add(oid[0] / 40);
                oidValues.Add(oid[0] % 40);
            }

            // Each subidentifier is represented as a series of (one or more) octets. Bit 8 of each octet indicates whether it is the last in 
            // the series: bit 8 of the last octet is zero; bit 8 of each preceding octet is one. Bits 7 to 1 of the octets in the series 
            // collectively encode the subidentifier. Conceptually, these groups of bits are concatenated to form an unsigned binary 
            // number whose most significant bit is bit 7 of the first octet and whose least significant bit is bit 1 of the last octet. The 
            // subidentifier shall be encoded in the fewest possible octets, that is, the leading octet of the subidentifier shall not have 
            // the value 80 HEX. 
            int current = 0;
            for (int i = 1; i < oid.Length; i++)
            {
                current = (current << 7) | oid[i] & 0x7F;

                //check if last byte
                if ((oid[i] & 0x80) == 0)
                {
                    oidValues.Add(current);
                    current = 0;
                }
            }

            // join values of oid with comma
            var oidValue = String.Join(".", oidValues);
            return oidValue;
        }
    }
}
