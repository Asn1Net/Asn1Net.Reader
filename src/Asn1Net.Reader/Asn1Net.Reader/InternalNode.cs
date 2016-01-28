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
        public bool HasIndefiniteLength { get; internal set; }

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
            if (Identifier.Tag != Asn1Type.NumericString)
                throw new InvalidOperationException("Can only read value of NumericString node.");

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
            if (Identifier.Tag != Asn1Type.Ia5String)
                throw new InvalidOperationException("Can only read value of Ia5String node.");

            return ReadContentAsString();
        }

        Regex printableStringRegex = new Regex(@"^[a-zA-Z0-9 \'\(\)\+\,\-\.\/\:\=\?]*$", RegexOptions.ECMAScript);
        /// <summary>
        /// Read the value of the given node as Printable string.
        /// </summary>
        /// <returns>Value of the given node as string.</returns>
        public string ReadContentAsPrintableString()
        {
            if (Identifier.Tag != Asn1Type.PrintableString)
                throw new InvalidOperationException("Can only read value of PrintableString node.");

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
            if (Identifier.Tag != Asn1Type.T61String)
                throw new InvalidOperationException("Can only read value of T61String node.");

            return ReadContentAsString();
        }

        /// <summary>
        /// Read the value of the given node as Graphic string.
        /// </summary>
        /// <returns>Value of the given node as string.</returns>
        public string ReadContentAsGraphicString()
        {
            if (Identifier.Tag != Asn1Type.GraphicString)
                throw new InvalidOperationException("Can only read value of GraphicString node.");

            return ReadContentAsString();
        }

        /// <summary>
        /// Read the value of the given node as General string.
        /// </summary>
        /// <returns>Value of the given node as string.</returns>
        public string ReadContentAsGeneralString()
        {
            if (Identifier.Tag != Asn1Type.GeneralString)
                throw new InvalidOperationException("Can only read value of GeneralString node.");

            return ReadContentAsString();
        }

        /// <summary>
        /// Read the value of the given node as BMPstring.
        /// </summary>
        /// <returns>Value of the given node as string.</returns>
        public string ReadContentAsBmpString()
        {
            if (RawValue == null) throw new ArgumentNullException("RawValue");

            if (Identifier.Tag != Asn1Type.BmpString)
                throw new InvalidOperationException("Can only read value of BmpString node.");

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

            if (Identifier.Tag != Asn1Type.UniversalString)
                throw new InvalidOperationException("Can only read value of UniversalString node.");

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
                    return Helpers.ExtractRealFromBinaryEncoding(firstOctet, RawValue);

                // decimal encoding
                case 0:
                    return Helpers.ExtractRealFromDecimalEncoding(RawValue);

                // SpecialRealValue
                case 64:
                    return Helpers.ExtractRealFromSpecialValueEncoding(firstOctet, RawValue);

                default:
                    throw new NotSupportedException("Unknown encoding type.");
            }
        }

        /// <summary>
        /// Read the value of the given node as Enumerated.
        /// </summary>
        /// <returns>Value of the given node as Enumerated value.</returns>
        public T ReadContentAsEnumerated<T>(T defaultValue = default(T)) where T : struct, IComparable, IFormattable
        {
            if (RawValue == null) throw new ArgumentNullException("RawValue");
            if (Identifier == null) throw new ArgumentNullException("Identifier");
            if (Identifier.Tag != Asn1Type.Enumerated)
                throw new InvalidOperationException("Can only read value of Enumerated node.");

            var bigValue = new BigInteger(RawValue);
            var stringValue = bigValue.ToString();

            if (Enum.IsDefined(typeof (T), Convert.ToInt32(stringValue)))
            {
                return (T) Enum.Parse(typeof (T), stringValue, true);
            }
            return defaultValue;
        }
    }
}