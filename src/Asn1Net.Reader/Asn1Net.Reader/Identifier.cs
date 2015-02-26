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

namespace Net.Asn1.Reader
{
    /// <summary>
    /// Identifier Octet in ASN.1 object
    /// </summary>
    public class Identifier
    {
        /// <summary>
        /// Class of ASN.1 tag.
        /// </summary>
        public Asn1Class Class { get; set; }

        /// <summary>
        /// Flag P/C from Identifier octet of ASN.1 node.
        /// </summary>
        public bool Constructed { get; set; }

        /// <summary>
        /// Tag number of ASN.1 node
        /// </summary>
        public Asn1Type Tag { get; set; }
    }
}
