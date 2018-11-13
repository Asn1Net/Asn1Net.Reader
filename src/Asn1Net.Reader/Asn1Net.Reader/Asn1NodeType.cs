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

namespace Net.Asn1.Reader
{
    /// <summary>
    /// Internal enumeration of possible nodes when parsing ASN.1 structure.
    /// </summary>
    public enum Asn1NodeType
    {
        /// <summary>
        /// ASN.1 type contains primitive value such as Utf8String, Integer etc.
        /// </summary>
        Primitive,

        /// <summary>
        /// ASN.1 type contains another ASN.1 type. This could be Sequence, Set etc.
        /// </summary>
        ConstructedStart,

        /// <summary>
        /// Virtual type of node indicating that ASN.1 constructed node has been parsed completely.
        /// </summary>
        ConstructedEnd,

        /// <summary>
        /// Virtual type of node indicating that parser is at the beginning of ASN.1 structure.
        /// </summary>
        DocumentStart,

        /// <summary>
        /// Virtual type of node indicating that parsing of ASN.1 structure has been completed.
        /// </summary>
        DocumentEnd
    }
}