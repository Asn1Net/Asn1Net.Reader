/*
 *  Copyright 2012-2016 The Asn1Net Project
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
