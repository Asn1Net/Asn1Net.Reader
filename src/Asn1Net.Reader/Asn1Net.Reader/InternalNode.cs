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
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;

namespace Net.Asn1.Reader
{
    /// <summary>
    /// Class holds information about parsed ASN.1 node.
    /// </summary>
    public class InternalNode
    {
        /// <summary>
        /// Identifier of ASN.1 object
        /// </summary>
        public Identifier Identifier { get; set; }

        /// <summary>
        /// Length of ASN.1 node data. Identifier and Length octets of TLV are excluded. It is length of the Value octets.
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// Value of ASN.1 node in bytes. This property is not set automatically by Reader. 
        /// User should set it by calling specific method on Reader to read the value.
        /// </summary>
        public byte[] RawValue { get; set; }

        /// <summary>
        /// Start position of ASN.1 node in the inner stream.
        /// </summary>
        public long StartPosition { get; set; }

        /// <summary>
        /// End position of ASN.1 node in the inner stream.
        /// </summary>
        public long EndPosition { get; set; }

        /// <summary>
        /// Flag indicating indefinite length of ASN.1 node.
        /// </summary>
        public bool HasIndefiniteLength { get { return Length == -1; } }

        /// <summary>
        /// Position to the inner stream where Value octets begin.
        /// </summary>
        public long DataOffsetToStream { get; set; }

        /// <summary>
        /// Flag indicating that this node is not <see cref="Asn1NodeType.Primitive"/>
        /// </summary>
        public bool IsConstructed
        {
            get { return NodeType != Asn1NodeType.Primitive; }
        }

        /// <summary>
        /// Type of the ASN.1 node
        /// </summary>
        public Asn1NodeType NodeType { get; set; }

        /// <summary>
        /// Internal list of child nodes of this ASN.1 node.
        /// </summary>
        private readonly List<InternalNode> _childNodes = new List<InternalNode>();

        /// <summary>
        /// Gets list of childs of this ASN.1 node.
        /// </summary>
        public List<InternalNode> ChildNodes { get { return _childNodes; } }


        /// <summary>
        /// Read the value of the given node as BigInteger.
        /// </summary>
        /// <returns>Content of the given node.</returns>
        public BigInteger ReadContentAsBigInteger()
        {
            if (RawValue == null) throw new ArgumentNullException("RawValue");
            if (Identifier == null) throw new ArgumentNullException("Identifier");
            if (Identifier.Tag != Asn1Type.Integer)
                throw new InvalidOperationException("Can only read value of INTEGER node.");

            var resp = new BigInteger(RawValue);
            return resp;
        }


        /// <summary>
        /// Read the value of the given node as string.
        /// </summary>
        /// <returns>Value of the given node as string.</returns>
        public string ReadContentAsObjectIdentifier()
        {
            if (RawValue == null) throw new ArgumentNullException("RawValue");
            if (Identifier == null) throw new ArgumentNullException("Identifier");

            if (Identifier.Tag != Asn1Type.ObjectIdentifier)
                throw new InvalidOperationException("Can only read value of OBJECT IDENTIFIER node.");

            var resp = Helpers.ParseEncodedOid(RawValue);
            return resp;
        }

        /// <summary>
        /// Read the value of the given node as string.
        /// </summary>
        /// <returns>Value of the given node as string.</returns>
        public string ReadContentAsString()
        {
            if (RawValue == null) throw new ArgumentNullException("RawValue");

            var resp = Encoding.UTF8.GetString(RawValue, 0, RawValue.Length);
            return resp;
        }

        Regex numericStringRegex = new Regex(@"^[\d ]*$", RegexOptions.ECMAScript);
        /// <summary>
        /// Read the value of the given node as numeric string.
        /// </summary>
        /// <returns>Value of the given node as string.</returns>
        public string ReadContentAsNumericString()
        {
            var resp = ReadContentAsString();

            if (numericStringRegex.IsMatch(resp) == false)
                throw new FormatException("Numeric string can contain only these characters: 0 1 2 3 4 5 6 7 8 9 0 SPACE.");

            return resp;
        }

        /// <summary>
        /// Read the value of the given node as IA5string.
        /// </summary>
        /// <returns>Value of the given node as string.</returns>
        public string ReadContentAsIA5String()
        {
            return ReadContentAsString();
        }

        Regex printableStringRegex = new Regex(@"^[a-zA-Z0-9 \'\(\)\+\,\-\.\/\:\=\?]*$", RegexOptions.ECMAScript);
        /// <summary>
        /// Read the value of the given node as Printable string.
        /// </summary>
        /// <returns>Value of the given node as string.</returns>
        public string ReadContentAsPrintableString()
        {
            var resp = ReadContentAsString();

            if (printableStringRegex.IsMatch(resp) == false)
                throw new FormatException("Found invalid character in input value for Printable string. Please refer to ITU-T X.680 specification.");

            return resp;
        }

        /// <summary>
        /// Read the value of the given node as T61string.
        /// </summary>
        /// <returns>Value of the given node as string.</returns>
        public string ReadContentAsT61String()
        {
            return ReadContentAsString();
        }

        /// <summary>
        /// Read the value of the given node as Graphic string.
        /// </summary>
        /// <returns>Value of the given node as string.</returns>
        public string ReadContentAsGraphicString()
        {
            return ReadContentAsString();
        }

        /// <summary>
        /// Read the value of the given node as General string.
        /// </summary>
        /// <returns>Value of the given node as string.</returns>
        public string ReadContentAsGeneralString()
        {
            return ReadContentAsString();
        }

        /// <summary>
        /// Read the value of the given node as BMPstring.
        /// </summary>
        /// <returns>Value of the given node as string.</returns>
        public string ReadContentAsBmpString()
        {
            if (RawValue == null) throw new ArgumentNullException("RawValue");

            var resp = Encoding.BigEndianUnicode.GetString(RawValue, 0, RawValue.Length);

            return resp;
        }

        /// <summary>
        /// Read the value of the given node as Universalstring.
        /// </summary>
        /// <returns>Value of the given node as string.</returns>
        public string ReadContentAsUniversalString()
        {
            if (RawValue == null) throw new ArgumentNullException("RawValue");

            var enc = Encoding.GetEncoding("utf-32BE");
            if (enc == null)
                throw new PlatformNotSupportedException("UTF-32 encoding is not supported on this platform.");
            
            var resp = enc.GetString(RawValue, 0, RawValue.Length);

            return resp;
        }

        /// <summary>
        /// Read the value of the given node as DateTimeOffset.
        /// </summary>
        /// <returns>>Value of the given node as DateTimeOffset.</returns>
        public DateTimeOffset ReadConventAsDateTimeOffset()
        {
            if (RawValue == null) throw new ArgumentNullException("RawValue");
            if (Identifier == null) throw new ArgumentNullException("Identifier");

            var dateTimeString = ReadContentAsString();

            if (Identifier.Tag == Asn1Type.GeneralizedTime)
                return Helpers.ConvertFromGeneralizedTime(dateTimeString);

            if (Identifier.Tag == Asn1Type.UtcTime)
                return Helpers.ConvertFromUniversalTime(dateTimeString);

            throw new InvalidOperationException("Unsupported ASN.1 type. Only GeneralizedTime and UTCTime are supported at the moment.");
        }

        /// <summary>
        /// Read the value of the given node as Boolean.
        /// </summary>
        /// <returns>>Value of the given node as Boolean.</returns>
        public bool ReadContentAsBoolean()
        {
            if (RawValue == null) throw new ArgumentNullException("RawValue");
            if (Identifier == null) throw new ArgumentNullException("Identifier");

            if (Identifier.Tag != Asn1Type.Boolean)
                throw new InvalidOperationException("Can only read value of BOOLEAN node.");

            if (RawValue.Length != 1)
                throw new FormatException("Boolean ASN.1 type should have single octet value.");

            // BER Encoding rules
            // If the boolean value is FALSE the octet shall be zero.
            return RawValue[0] != 0x00;
        }

        /// <summary>
        /// Read the value of the given node as BitString.
        /// </summary>
        /// <returns>>Value of the given node as BitString.</returns>
        public byte[] ReadContentAsBitString()
        {
            if (RawValue == null) throw new ArgumentNullException("RawValue");
            if (Identifier == null) throw new ArgumentNullException("Identifier");
            if (Identifier.Tag != Asn1Type.BitString)
                throw new InvalidOperationException("Can only read value of BITSTRING node.");

            var padBitesPart = RawValue[0];

            if (padBitesPart > 7 || padBitesPart < 0)
                throw new FormatException("Value in initial octet shall be an unsigned integer in the range zero to seven");

            var result = new byte[Length - 1];
            Array.Copy(RawValue, 1, result, 0, result.Length);

            // clean last byte with the mask set by unused bits (first byte)
            var cleanedLastByte = (byte)(result[result.Length - 1] & ~padBitesPart);
            if (cleanedLastByte != result[result.Length - 1])
                throw new FormatException("There was more bits set in the last byte than it should be.");

            return result;
        }


        /// <summary>
        /// Read the value of the given node as Real (double).
        /// </summary>
        /// <returns>>Value of the given node as Real (double).</returns>
        public double ReadContentAsReal()
        {
            if (RawValue == null) throw new ArgumentNullException("RawValue");
            if (Identifier == null) throw new ArgumentNullException("Identifier");
            if (Identifier.Tag != Asn1Type.Real)
                throw new InvalidOperationException("Can only read value of REAL node.");

            var firstOctet = RawValue[0];

            // check bit 8 and bit 7
            var encodingType = firstOctet & 192;

            switch (encodingType)
            {
                // binary encoding
                case 128:
                    return ExtractRealFromBinaryEncoding(firstOctet);

                // decimal encoding
                case 0:
                    return ExtractRealFromDecimalEncoding();

                // SpecialRealValue
                case 64:
                    return ExtractRealFromSpecialValueEncoding(firstOctet);

                default:
                    throw new NotSupportedException("Unknown encoding type.");
            }
        }

        /// <summary>
        /// Extract Real value from binary encoded data.
        /// </summary>
        /// <param name="firstOctet">Information octet with base, mantissa and exponent information.</param>
        /// <returns>Extracted value.</returns>
        private double ExtractRealFromBinaryEncoding(byte firstOctet)
        {
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
                exponentLength = (int)RawValue[1];
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
                baseInteger = (baseInteger << 8) | RawValue[exponentValueStartIdx];
                subtrahendInteger = (subtrahendInteger << 8) | subtrahendIntegerBytes[i];
            }

            var exponent = (baseInteger & ~(subtrahendInteger)) - subtrahendInteger;

            // read N to compute mantissa
            var N = 0;
            for (var i = 1 + exponentLength; i < RawValue.Length; i++)
                N = (N << 8) | RawValue[i];

            var mantissa = sign * N * Math.Pow(2, ff);
            var res = mantissa * Math.Pow(valueOfBase, exponent);
            return res;
        }

        /// <summary>
        /// Extract Real value from decimal encoded data.
        /// </summary>
        /// <param name="firstOctet">Information octet with base, mantissa and exponent information.</param>
        /// <returns>Extracted value.</returns>
        private double ExtractRealFromDecimalEncoding()
        {
            var raw = new byte[RawValue.Length - 1];
            Array.Copy(RawValue, 1, raw, 0, RawValue.Length - 1);

            var base10Value = double.Parse(Encoding.UTF8.GetString(raw, 0, raw.Length), CultureInfo.InvariantCulture.NumberFormat);
            return base10Value;
        }

        /// <summary>
        /// Extract Real value from special encoded data.
        /// </summary>
        /// <param name="firstOctet">Information octet with base, mantissa and exponent information.</param>
        /// <returns>Extracted value.</returns>
        private double ExtractRealFromSpecialValueEncoding(byte firstOctet)
        {
            if (RawValue.Length > 1)
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