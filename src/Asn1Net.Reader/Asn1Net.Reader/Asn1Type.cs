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
        // UniversalClass
        Eoc = 0,
        Boolean = 1,
        Integer = 2,
        BitString = 3,
        OctetString = 4,
        Null = 5,
        ObjectIdentifier = 6,
        ObjectDescriptor = 7,
        External = 8,
        Real = 9,
        Enumerated = 10,
        EmbeddedPdv = 11,
        Utf8String = 12,
        RelativeOid = 13,
        Sequence = 16,
        Set = 17,
        NumericString = 18,
        PrintableString = 19,
        T61String = 20,
        VideotexString = 21,
        Ia5String = 22,
        UtcTime = 23,
        GeneralizedTime = 24,
        GraphicString = 25,
        VisibleString = 26,
        GeneralString = 27,
        UniversalString = 28,
        CharacterString = 29,
        BmpString = 30,
        LongForm = 31
    }
}
