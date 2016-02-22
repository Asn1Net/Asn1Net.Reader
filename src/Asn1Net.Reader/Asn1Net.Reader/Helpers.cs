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

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
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
        /// Parse Oid value from Byte[]. More information here <![CDATA[http://www.itu.int/rec/dologin_pub.asp?lang=e&id=T-REC-X.690-200811-I!!PDF-E&type=items]]>
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



        /// <summary>
        /// Extract Real value from binary encoded data.
        /// </summary>
        /// <param name="firstOctet">Information octet with base, mantissa and exponent information.</param>
        /// <param name="rawValue">Raw encoded value.</param>
        /// <returns>Extracted value.</returns>
        internal static double ExtractRealFromBinaryEncoding(byte firstOctet, byte[] rawValue)
        {
            if (rawValue == null) throw new ArgumentNullException("rawValue");

            //  M = S × N × 2^F
            //  0 ≤ F < 4 
            //  S = +1 or –1

            // bit 7
            var bit7 = firstOctet & 64;
            var sign = bit7 == 64 ? -1 : +1;

            // bits 6 to 5
            var valueOfBase = 2;
            switch ((firstOctet & 48) >> 4)
            {
                case 0:
                    valueOfBase = 2;
                    break;
                case 1:
                    valueOfBase = 8;
                    break;
                case 2:
                    valueOfBase = 16;
                    break;
                default:
                    throw new NotSupportedException("Encoded value of base reserved for future use.");
            }

            // bits 4 to 3
            var ff = (firstOctet & 12) >> 2;

            var exponentLength = (firstOctet & 3) + 1;
            var exponentValueStartIdx = 1;
            if (exponentLength == 3)
            {
                exponentValueStartIdx = 2;
                exponentLength = (int)rawValue[1];
            }

            // read exponent

            var subtrahendIntegerBytes = new byte[exponentLength];
            subtrahendIntegerBytes[exponentLength - 1] = 128;

            if (exponentLength > sizeof(long))
                throw new PlatformNotSupportedException(
                    "Length of exponent is greater than current used integer type on the platform.");

            long baseInteger = 0;
            var subtrahendInteger = 0;

            for (var i = 0; i < exponentLength; i++, exponentValueStartIdx++)
            {
                baseInteger = (baseInteger << 8) | rawValue[exponentValueStartIdx];
                subtrahendInteger = (subtrahendInteger << 8) | subtrahendIntegerBytes[i];
            }

            var exponent = (baseInteger & ~(subtrahendInteger)) - subtrahendInteger;

            // read N to compute mantissa
            var N = 0;
            for (var i = 1 + exponentLength; i < rawValue.Length; i++)
                N = (N << 8) | rawValue[i];

            var mantissa = sign * N * Math.Pow(2, ff);
            var res = mantissa * Math.Pow(valueOfBase, exponent);
            return res;
        }

        /// <summary>
        /// Extract Real value from decimal encoded data.
        /// </summary>
        /// <param name="rawValue">Raw encoded value.</param>
        /// <returns>Extracted value.</returns>
        internal static double ExtractRealFromDecimalEncoding(byte[] rawValue)
        {
            if (rawValue == null) throw new ArgumentNullException("rawValue");

            var raw = new byte[rawValue.Length - 1];
            Array.Copy(rawValue, 1, raw, 0, rawValue.Length - 1);

            var base10Value = double.Parse(Encoding.UTF8.GetString(raw, 0, raw.Length), CultureInfo.InvariantCulture.NumberFormat);
            return base10Value;
        }

        /// <summary>
        /// Extract Real value from special encoded data.
        /// </summary>
        /// <param name="firstOctet">Information octet with base, mantissa and exponent information.</param>
        /// <param name="rawValue">Raw encoded value.</param>
        /// <returns>Extracted value.</returns>
        internal static double ExtractRealFromSpecialValueEncoding(byte firstOctet, byte[] rawValue)
        {
            if (rawValue == null) throw new ArgumentNullException("rawValue");

            if (rawValue.Length > 1)
                throw new FormatException("All SpecialRealValues MUST be encoded by just one information octet, without octets for mantissa end exponent.");

            switch (firstOctet & 0x43)
            {
                case 0x40:
                    return double.PositiveInfinity;
                case 0x41:
                    return double.NegativeInfinity;
                case 0x42:
                    return double.NaN;
                case 0x43:
                    return -1 * 0.0d; // minus zero
                default:
                    throw new NotSupportedException("Special value reserved for future use.");
            }
        }
    }
}
