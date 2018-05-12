using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;
using Tynamix.ObjectFiller.RegExPlugin.Tests.IntegrationTests.Models;

namespace Tynamix.ObjectFiller.RegExPlugin.Tests.IntegrationTests
{
    [TestFixture]
    public class IntegrationTests
    {
        [Test]
        public void RegExMatchingStringGenerationTest()
        {
            var assertRegEx = @"^([a-z0-9\.\-]+)@([a-z0-9\-]+)((\.([a-z]){2,3})+)$";

            var testPersonFiller = new Filler<TestPersonModel>();
            testPersonFiller.Setup()
                .OnProperty(t => t.Email).Use(new ReverseRegEx(assertRegEx));

            var result = testPersonFiller.Create(1000);

            result.Should().HaveCount(1000);
            result.Should().OnlyContain(t => Regex.IsMatch(t.Email, assertRegEx));
        }
    }
}
