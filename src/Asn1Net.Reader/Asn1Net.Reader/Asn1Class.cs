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
