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

using System;
using System.Collections.Generic;
using System.IO;

namespace Net.Asn1.Reader
{
    /// <summary>
    /// BER reader. Implementation works on top o a stream. The stream should be seekable.
    /// </summary>
    public class BerReader : IDisposable
    {
        /// <summary>
        /// Flag indicating whether instance has been disposed
        /// </summary>
        private bool _disposed = false;

        private bool _leaveStreamOpen = false;

        /// <summary>
        /// Stack to help going through the structure of ASN.1.
        /// </summary>
        private readonly Stack<InternalNode> _nodeStack = new Stack<InternalNode>();

        /// <summary>
        /// Current node that can be read.
        /// </summary>
        private InternalNode _current = new InternalNode() { NodeType = Asn1NodeType.DocumentStart, Identifier = new Identifier() };

        /// <summary>
        /// Inner stream on top of which this class operates.
        /// </summary>
        private readonly Stream _innerStream;

        /// <summary>
        /// Gets current node. Usable when reading of content of a ASN.1 tag is not so important at the moment.
        /// </summary>
        public InternalNode CurrentNode { get { return _current; } }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="stream">Stream that holds ASN.1 structure that shall be parsed.</param>
        /// <param name="leaveOpen">True to leave the stream open after the BerReader object is disposed; otherwise, false.</param>
        public BerReader(Stream stream, bool leaveOpen = false)
        {
            _innerStream = stream;
            _leaveStreamOpen = leaveOpen;
        }

        /// <summary>
        /// Value indicating the actual depth of parser in ASN.1 structure.
        /// </summary>
        public int Depth
        {
            get { return _nodeStack.Count; }
        }

        /// <summary>
        /// Parse next node in ASN.1 structure.
        /// </summary>
        /// <param name="readContent">Flag indicating that value of each node should be read as well.</param>
        /// <returns></returns>
        public InternalNode Read(bool readContent = false)
        {
            switch (_current.NodeType)
            {
                case Asn1NodeType.DocumentStart:
                    InternalReadElement(readContent);
                    break;
                case Asn1NodeType.Primitive:

                    _innerStream.Seek(_current.EndPosition, SeekOrigin.Begin);

                    InternalReadElement(readContent);
                    break;
                case Asn1NodeType.ConstructedStart:
                    _nodeStack.Push(_current);
                    InternalReadElement(readContent);
                    break;
                case Asn1NodeType.ConstructedEnd:
                    _nodeStack.Pop();
                    InternalReadElement(readContent);
                    break;
                default:
                    break;
            }

            return _current;
        }

        /// <summary>
        /// Parse ASN.1 node and sets the result to <see cref="InternalNode"/> class.
        /// <param name="readContent">Flag indicating that value of each node should be read as well.</param>
        /// </summary>
        private void InternalReadElement(bool readContent = false)
        {
            // This is the end of Asn1 constructed element. 
            if (_nodeStack.Count > 0 && _innerStream.Position >= _nodeStack.Peek().EndPosition)
            {
                _current = new InternalNode()
                {
                    Identifier = new Identifier
                    {
                        Class = _nodeStack.Peek().Identifier.Class,
                        Constructed = _nodeStack.Peek().Identifier.Constructed,
                        Tag = _nodeStack.Peek().Identifier.Tag
                    },
                    EndPosition = _innerStream.Position,
                    Length = 0,
                    DataOffsetToStream = _innerStream.Position,
                    NodeType = Asn1NodeType.ConstructedEnd
                };

                return;
            }

            var nodeStartPosition = _innerStream.Position;

            // Identifier
            var identifier = ReadIdentifierOctet();

            if (identifier == null)
            {
                // if no identifier was returned then it is the end of stream
                return;
            }

            // ReadIdentifierOctet(); may change _current to DocumentEnd type
            if (_current.NodeType == Asn1NodeType.DocumentEnd)
                return;

            var length = ReadLength();
            if (length >= _innerStream.Length)
                throw new Exception("Length of ASN.1 node exceeds length of current stream.");

            var hasIndefiniteLengthForm = false;
            if (length == -1)
            {
                hasIndefiniteLengthForm = true;
                length = DetermineLengthOfIndefiniteLengthNode();
            }

            // set what we parsed so far in internal node class
            _current = new InternalNode()
            {
                Identifier = identifier,
                StartPosition = nodeStartPosition,
                HasIndefiniteLength = hasIndefiniteLengthForm,
                EndPosition = _innerStream.Position + ((hasIndefiniteLengthForm) ? length +2: length), 
                Length = length,
                DataOffsetToStream = _innerStream.Position,
                NodeType = (identifier.Constructed) ? Asn1NodeType.ConstructedStart : Asn1NodeType.Primitive
            };

            // if content should be read then be it but only if the node is Primitive. Context Specific value should be read manually when appropriate
            if (readContent)
                _current.RawValue = (_current.NodeType == Asn1NodeType.Primitive)
                    ? ReadContentAsBuffer(_current)
                    : null;
        }

        private int DetermineLengthOfIndefiniteLengthNode()
        {
            var length = 0;
            var streamPosition = _innerStream.Position;
            var firstByteofEocfound = false;
            while (true)
            {
                var oneByte = _innerStream.ReadByte();

                // reached end of stream
                if (oneByte == -1)
                    throw new ArgumentOutOfRangeException("Could not determine actual length of indefinite-length node before reaching end of stream.");

                // found possible EOC (0x00 00)
                if (oneByte == 0)
                {
                    // pretty sure it is EOC, second half of EOC
                    if (firstByteofEocfound)
                    {
                        _innerStream.Seek(streamPosition, SeekOrigin.Begin);
                        return length;
                    }

                    // first half of EOC
                    firstByteofEocfound = true;
                }
                else
                {
                    // reset EOC
                    firstByteofEocfound = false;
                    // increase length value
                    length++;
                }
            }
        }

        /// <summary>
        /// Reads length of given ASN.1 object.
        /// </summary>
        /// <returns>Length of ASN.1 object value.</returns>
        private int ReadLength()
        {
            // Length Octets
            // read one byte from stream. This should be the Length. Length can be encoded in short or long form.
            byte lengthStart = ReadByteOrThrow(_innerStream);

            // 0xff => the value 11111111 (binary) shall not be used
            if (lengthStart == 0xff)
                throw new FormatException("Invalid length format. The value 11111111 (binary) shall not be used.");

            int length = 0;
            // indefinite form of length
            if (lengthStart == 0x80)
            {
                return -1; // will be corrected later
            }

            // check if length is encoded in short form or long form
            bool isMultiByteLength = (lengthStart & 0x80) != 0;

            // short form length
            if (!isMultiByteLength)
            {
                length = lengthStart;
            }
            else // long form length
            {
                // All bits of the subsequent octets form the encoding of an unsigned binary integer equal to the number of octets in the contents octets.
                // get number of octets
                int numBytesLength = lengthStart & 0x7f;

                // in c# sizeof(int) is 4 bytes. We can not interpret more. We could use long, but reading data from stream expects int, not long
                // TODO: maybe support long instead of int in Length
                if (numBytesLength > 4)
                {
                    throw new FormatException("Invalid length value (too long)");
                }

                // read the value of length in long form
                while (numBytesLength > 0)
                {
                    length = (length << 8) | ReadByteOrThrow(_innerStream);
                    numBytesLength--;
                }
            }
            return length;
        }

        /// <summary>
        /// Reads identifier octet from ASN.1 object.
        /// </summary>
        /// <returns></returns>
        private Identifier ReadIdentifierOctet()
        {
            // Identifier octets
            // read one byte from stream. This should represent Identifier Asn.1 tag (class and number)
            int identifierOctet = _innerStream.ReadByte();

            if (identifierOctet == -1)
            {
                // reached end of stream
                _current = new InternalNode() { NodeType = Asn1NodeType.DocumentEnd, Identifier = new Identifier() };
                return _current.Identifier;
            }

            //  |               |   (   63  ------------------------------->    |
            //  |               |   32  |   (   31  ----------------------->    |
            //  -----------------------------------------------------------------
            //  |   8   |   7   |   6   |   5   |   4   |   3   |   2   |   1   |
            //  |   class       |   P/C | Tag Number                            |
            //  -----------------------------------------------------------------
            var tagStart = (byte)identifierOctet;

            //parse Tag
            // bit 6 contains P/C (Primitive/Constructed) flag. 6bit => value 0x20 (32 decimal)
            bool isConstructed = (tagStart & 0x20) != 0;

            // Tag number allows values from 0 to 31. 31 states long-form
            bool moreBytes = (tagStart & (int)Asn1Type.LongForm) == (int)Asn1Type.LongForm;

            while (moreBytes)
            {
                byte b = ReadByteOrThrow(_innerStream);

                // TODO parse multi byte tag
                throw new NotSupportedException();

                moreBytes = (b & 0x80) != 0;
            }

            var classTag = (Asn1Class)(tagStart & ~63);
            var tagNumber = (Asn1Type)(tagStart & 31);

            var identifier = new Identifier
            {
                Class = classTag,
                Constructed = isConstructed,
                Tag = tagNumber
            };
            return identifier;
        }

        /// <summary>
        /// Reads one byte from given stream.
        /// </summary>
        /// <param name="stream">Stream to read from.</param>
        /// <returns>Read byte.</returns>
        private static byte ReadByteOrThrow(Stream stream)
        {
            int b = stream.ReadByte();

            if (b == -1)
                throw new EndOfStreamException();

            return (byte)b;
        }

        /// <summary>
        /// Parse whole ASN.1 structure. <see cref="InternalNode"/> class holds all the mappings. Values of the tags are not read.
        /// </summary>
        /// <param name="readContent">Flag indicating that value of each node should be read as well.</param>
        /// <returns>Internal node with map of parsed ASN.1 structure.</returns>
        public InternalNode ReadToEnd(bool readContent = false)
        {
            // this will help to fill internal children
            var nodeStack = new Stack<InternalNode>();

            // first put current node to stack. depth 1
            nodeStack.Push(_current);

            // set current level
            var currentLevel = _current;
            // we will return current node. All nodes under it will be its children nodes
            var result = _current;
            // set next read node as current for now
            var next = _current;

            // iterate through ASN1 structure, read nodes one by one and put them as child nodes or on the same level
            while (next.NodeType != Asn1NodeType.DocumentEnd)
            {
                next = Read(readContent);

                // only construction node or primitive node will be placed in the map
                if (next.NodeType == Asn1NodeType.ConstructedStart || next.NodeType == Asn1NodeType.Primitive)
                    currentLevel.ChildNodes.Add(next);

                // if we hit construction node, there will probably be child nodes under it. 
                // Push current head node in stack and set new current head
                if (next.NodeType == Asn1NodeType.ConstructedStart)
                {
                    nodeStack.Push(currentLevel);
                    currentLevel = next;
                }

                // if we hit construction end node then return to previous head node 
                // as we completed mapping of inner structure of construction node
                if (next.NodeType == Asn1NodeType.ConstructedEnd)
                {
                    currentLevel = nodeStack.Pop();
                }
            }

            // after all the mapping there should be only document start node in stack
            if (nodeStack.Count != 1)
                throw new Exception("There should be DocumentStart node in stack. Something is not right.");

            if (nodeStack.Peek().NodeType != Asn1NodeType.DocumentStart)
                throw new Exception("There should be DocumentStart node in stack. Something is not right.");

            // we want to be thorough
            nodeStack.Pop();

            return result;
        }


        #region Read values
        /// <summary>
        /// Read value of current node.
        /// </summary>
        /// <returns>Value read.</returns>
        public byte[] ReadContentAsBuffer()
        {
            if (_current == null) throw new ArgumentException("Current node is null.");

            var result = new byte[_current.Length];
            ReadContentAsBuffer(_current, result);
            return result;
        }

        /// <summary>
        /// Read value of given node. Underlying stream in BER reader has to be seekable to do that.
        /// </summary>
        /// <param name="node">Node  whose value shall be read.</param>
        /// <returns>Content of the given node.</returns>
        public byte[] ReadContentAsBuffer(InternalNode node)
        {
            if (node == null) throw new ArgumentNullException("node");

            var result = new byte[node.Length];
            ReadContentAsBuffer(node, result);
            return result;
        }

        /// <summary>
        /// Read value of current node.
        /// </summary>
        /// <param name="outputBuffer">Reader will read content to this parameter. 
        /// The parameter need to be initialized beforehand with length of actual read node.</param>
        /// <returns>Value read.</returns>
        public void ReadContentAsBuffer(byte[] outputBuffer)
        {
            ReadContentAsBuffer(_current, outputBuffer);
        }

        /// <summary>
        /// Read value of given node. Underlying stream in BER reader has to be seekable to do that.
        /// </summary>
        /// <param name="node">Node  whose value shall be read.</param>
        /// <param name="outputBuffer">Reader will read content to this parameter. 
        /// The parameter need to be initialized beforehand with length of actual read node.</param>
        /// <remarks>
        /// Basically it can be used only on nodes that have definite length and are primitive, i.e. Integer,BitString,OctetString etc. 
        /// Sequence and Set are construct types so they are excluded.
        /// </remarks>
        /// <returns>Content of the given node.</returns>
        public void ReadContentAsBuffer(InternalNode node, byte[] outputBuffer)
        {
            if (node == null) throw new ArgumentNullException("node");
            if (outputBuffer == null) throw new ArgumentNullException("outputBuffer");
            if (outputBuffer.Length != node.Length) throw new ArgumentException("Parameter outputBuffer needs to be initialized to length of actual read node.");

            if (node.NodeType != Asn1NodeType.Primitive && node.Identifier.Class != Asn1Class.ContextSpecific)
            {
                throw new InvalidOperationException("Content cannot be read at this position");
            }

            if ((node.IsConstructed && node.Identifier.Class != Asn1Class.ContextSpecific) || node.HasIndefiniteLength)
            {
                throw new InvalidOperationException("Cannot read value from constructed type");
            }

            if (node.Length > int.MaxValue)
            {
                throw new InvalidOperationException("Cannot read values larger than 2GB into buffer");
            }

            _innerStream.Seek(node.DataOffsetToStream, SeekOrigin.Begin);
            _innerStream.Read(outputBuffer, 0, node.Length);
        }

        /// <summary>
        /// Get raw ASN.1 node data (whole TLV => Identifier, Length and Value octets).
        /// </summary>
        /// <param name="node">Node that shall be read.</param>
        /// <returns>Raw data of the given ASN.1 node including Identifier and Length octets.</returns>
        public byte[] ExtractAsn1NodeAsRawData(InternalNode node)
        {
            if (node == null) throw new ArgumentNullException("node");

            byte[] outputBuffer = new byte[node.EndPosition - node.StartPosition];

            if (node.Length > int.MaxValue)
            {
                throw new InvalidOperationException("Cannot read values larger than 2GB into buffer");
            }

            _innerStream.Seek(node.StartPosition, SeekOrigin.Begin);
            _innerStream.Read(outputBuffer, 0, Convert.ToInt32(node.EndPosition - node.StartPosition));

            return outputBuffer;
        }

        #endregion

        #region IDisposable

        /// <summary>
        /// Disposes object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes object.
        /// </summary>
        /// <param name="disposing">Flag indicating if it is safe to dispose managed resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                // Dispose managed objects
                if (!_leaveStreamOpen)
                    _innerStream.Dispose();
            }

            // Dispose unmanaged objects

            _disposed = true;
        }

        /// <summary>
        /// Class destructor that disposes object if caller forgot to do so
        /// </summary>
        ~BerReader()
        {
            Dispose(false);
        }

        #endregion
    }
}
