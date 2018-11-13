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
using NUnit.Framework;

namespace Net.Asn1.Reader.Tests.BerReaderTests
{
    /// <summary>
    /// Test covering BerReader's ReadToEnd method
    /// </summary>
    [TestFixture]
    [Category("ReadTimeValuesTests")]
    public class ReadTimeValuesTests
    {
        /// <summary>
        /// Test parsing UTCTime and converting to DateTimeOffset. Samples used here should all PASS.
        /// Samples are from IUT-T X.690 specification.
        /// </summary>
        [Test]
        public void UtcTimePositiveTest()
        {
            Dictionary<string, DateTimeOffset> times = new Dictionary<string, DateTimeOffset>()
            {
                {"920521000000Z", new DateTimeOffset(1992,5,21,0,0,0,new TimeSpan(0,0,0))},
                {"920622123421Z", new DateTimeOffset(1992,6,22,12,34,21,new TimeSpan(0,0,0))},
                {"920722132100Z", new DateTimeOffset(1992,7,22,13,21,0,new TimeSpan(0,0,0))}
            };


            foreach (var time in times)
            {
                var res = Reader.Helpers.ConvertFromUniversalTime(time.Key);
                Assert.IsTrue(res == time.Value);
            }
        }

        /// <summary>
        /// Test parsing UTCTime and converting to DateTimeOffset. Samples used here should all FAIL.
        /// Samples are from IUT-T X.690 specification.
        /// </summary>
        [Test]
        public void UtcTimeNegativeTest()
        {
            List<string> times = new List<string>()
            {
                "920520240000Z", // midnight represented incorrectly
                "9207221321Z" // seconds of "00" omitted
            };

            foreach (var time in times)
            {
                var failed = false;
                try
                {
                    var res = Reader.Helpers.ConvertFromUniversalTime(time);
                }
                catch (Exception)
                {
                    failed = true;
                }

                Assert.IsTrue(failed);
            }
        }


        /// <summary>
        /// Test parsing GeneralizedTime and converting to DateTimeOffset. Samples used here should all PASS.
        /// Samples are from IUT-T X.690 specification.
        /// </summary>
        [Test]
        public void GeneralizedTimePositiveTest()
        {
            Dictionary<string, DateTimeOffset> times = new Dictionary<string, DateTimeOffset>()
            {
                {"19920521000000Z", new DateTimeOffset(1992,5,21,0,0,0,new TimeSpan(0,0,0))},
                {"19920622123421Z", new DateTimeOffset(1992,6,22,12,34,21,new TimeSpan(0,0,0))},
                {"19920722132100.3Z", new DateTimeOffset(1992,7,22,13,21,0,300,new TimeSpan(0,0,0))}
            };


            foreach (var time in times)
            {
                var res = Reader.Helpers.ConvertFromGeneralizedTime(time.Key);
                Assert.IsTrue(res == time.Value);
            }
        }

        /// <summary>
        /// Test parsing GeneralizedTime and converting to DateTimeOffset. Samples used here should all FAIL.
        /// Samples are from IUT-T X.690 specification.
        /// </summary>
        [Test]
        public void GeneralizedTimeNegativeTest()
        {
            List<string> times = new List<string>()
            {
                "19920520240000Z", // midnight represented incorrectly
                "19920622123421.0Z", // spurious trailing zeros
                "19920722132100.30Z" // spurious trailing zeros
            };

            foreach (var time in times)
            {
                var failed = false;
                try
                {
                    var res = Reader.Helpers.ConvertFromGeneralizedTime(time);
                }
                catch (Exception)
                {
                    failed = true;
                }

                Assert.IsTrue(failed);
            }
        }
    }
}
