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