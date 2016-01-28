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
using System.Globalization;

namespace Net.Asn1.Reader.Tests.BerReaderTests
{
    /// <summary>
    /// Extension methods of BerReader class
    /// </summary>
    internal static class BerReaderExtensions
    {
        /// <summary>
        /// Prints structure of given ASN.1 node. Node should be the result of ReadToEnd method called before.
        /// </summary>
        /// <param name="reader">Extended object</param>
        /// <param name="node">Node to print structure of.</param>
        /// <param name="relativeResult">Relative result of this method. Used when iterating through the structure to build on top of previous results of this method.</param>
        /// <param name="depth">Actual depth of parser.</param>
        /// <param name="dumpValues">Flag indicating if values should be printed out.</param>
        /// <returns>Structure of ASN.1 node as string.</returns>
        internal static string PrintStructure(this BerReader reader, InternalNode node, string relativeResult, int depth, bool dumpValues)
        {
            // print offset in source stream and length of ASN.1 node
            var offsetAndLength = String.Format("({0},{1})",
                node.StartPosition.ToString(CultureInfo.InvariantCulture),
                node.Length.ToString(CultureInfo.InvariantCulture));

            // append tag name
            var structure = String.Format("{0} {1}",
                offsetAndLength,
                (node.Identifier.Class == Asn1Class.ContextSpecific)
                    ? String.Format("{0} ({1})", node.Identifier.Class, (int)node.Identifier.Tag)
                    : node.Identifier.Tag.ToString());

            // append value of ASN.1 node
            if (dumpValues)
            {
                if (node.NodeType == Asn1NodeType.Primitive)
                {
                    string stringValue;
                    node.RawValue = reader.ReadContentAsBuffer(node);

                    switch (node.Identifier.Tag)
                    {
                        case Asn1Type.ObjectIdentifier:
                            stringValue = node.ReadContentAsObjectIdentifier();
                            break;
                        case Asn1Type.PrintableString:
                        case Asn1Type.Ia5String:
                        case Asn1Type.Utf8String:
                            stringValue = node.ReadContentAsString();
                            break;
                        case Asn1Type.GeneralizedTime:
                        case Asn1Type.UtcTime:
                            stringValue = node.ReadConventAsDateTimeOffset().ToString();
                            break;
                        case Asn1Type.Integer:
                            stringValue = node.ReadContentAsBigInteger().ToString();
                            break;
                        case Asn1Type.Boolean:
                            stringValue = node.ReadContentAsBoolean().ToString();
                            break;
                        default:
                            stringValue = node.RawValue.ToHexString();
                            break;
                    }

                    structure = string.Format("{0} : {1}", structure, stringValue);
                }
            }

            // apply depth 
            for (int i = 0; i < depth; i++)
            {
                structure = "\t" + structure;
            }

            // append new line
            string res = relativeResult + Environment.NewLine + structure;

            // count new depth if node has children
            var innerdepth = (node.ChildNodes.Count > 0) ? depth + 1 : depth;

            // recursively go through children and print structure of them
            foreach (var innerNode in node.ChildNodes)
            {
                res = reader.PrintStructure(innerNode, res, innerdepth, dumpValues);
            }

            // return result
            return res;
        }
    }
}