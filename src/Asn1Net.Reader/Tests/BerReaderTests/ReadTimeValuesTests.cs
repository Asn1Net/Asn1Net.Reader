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
