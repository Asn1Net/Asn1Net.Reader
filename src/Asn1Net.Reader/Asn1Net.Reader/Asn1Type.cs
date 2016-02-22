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

namespace Net.Asn1.Reader
{
    /// <summary>
    /// Enumeration of possible ASN.1 universal class tags in Identifier octet.
    /// </summary>
    public enum Asn1Type
    {
        /// <summary>
        /// UniversalClass
        /// </summary>
        Eoc = 0,
        /// <summary>
        /// Boolean
        /// </summary>
        Boolean = 1,

        /// <summary>
        /// Integer
        /// </summary>
        Integer = 2,

        /// <summary>
        /// BitString
        /// </summary>
        BitString = 3,

        /// <summary>
        /// OctetString
        /// </summary>
        OctetString = 4,

        /// <summary>
        /// NULL
        /// </summary>
        Null = 5,

        /// <summary>
        /// ObjectIdentifier
        /// </summary>
        ObjectIdentifier = 6,

        /// <summary>
        /// ObjectDescriptor
        /// </summary>
        ObjectDescriptor = 7,

        /// <summary>
        /// External
        /// </summary>
        External = 8,

        /// <summary>
        /// Real
        /// </summary>
        Real = 9,

        /// <summary>
        /// Enumerated
        /// </summary>
        Enumerated = 10,

        /// <summary>
        /// EmbeddedPdv
        /// </summary>
        EmbeddedPdv = 11,

        /// <summary>
        /// Utf8String
        /// </summary>
        Utf8String = 12,

        /// <summary>
        /// RelativeOid
        /// </summary>
        RelativeOid = 13,

        /// <summary>
        /// Sequence
        /// </summary>
        Sequence = 16,

        /// <summary>
        /// Set
        /// </summary>
        Set = 17,

        /// <summary>
        /// NumericString
        /// </summary>
        NumericString = 18,

        /// <summary>
        /// PrintableString
        /// </summary>
        PrintableString = 19,

        /// <summary>
        /// T61String
        /// </summary>
        T61String = 20,

        /// <summary>
        /// VideotexString
        /// </summary>
        VideotexString = 21,

        /// <summary>
        /// Ia5String
        /// </summary>
        Ia5String = 22,

        /// <summary>
        /// UtcTime
        /// </summary>
        UtcTime = 23,

        /// <summary>
        /// GeneralizedTime
        /// </summary>
        GeneralizedTime = 24,

        /// <summary>
        /// GraphicString
        /// </summary>
        GraphicString = 25,

        /// <summary>
        /// VisibleString
        /// </summary>
        VisibleString = 26,

        /// <summary>
        /// GeneralString
        /// </summary>
        GeneralString = 27,

        /// <summary>
        /// UniversalString
        /// </summary>
        UniversalString = 28,

        /// <summary>
        /// CharacterString
        /// </summary>
        CharacterString = 29,

        /// <summary>
        /// BmpString
        /// </summary>
        BmpString = 30,

        /// <summary>
        /// LongForm
        /// </summary>
        LongForm = 31
    }
}
