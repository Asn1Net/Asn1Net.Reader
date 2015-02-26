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
    /// Enumeration of possible classes in Identifier octet of the ASN.1 node.
    /// </summary>
    public enum Asn1Class
    {
        /// <summary>
        /// The type is native to ASN.1.
        /// </summary>
        Universal = 0,

        /// <summary>
        /// The type is only valid for one specific application
        /// </summary>
        Application = 64,

        /// <summary>
        /// Meaning of this type depends on the context (such as within a sequence, set or choice)
        /// </summary>
        ContextSpecific = 128,

        /// <summary>
        /// Defined in private specifications
        /// </summary>
        Private = 192,
    }
}
