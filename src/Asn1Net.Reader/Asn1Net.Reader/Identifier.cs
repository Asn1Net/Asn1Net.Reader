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
