using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace Tynamix.ObjectFiller.RegExPlugin.Tests.UnitTests
{
    [TestFixture]
    public class ReverseRegExTests
    {
        [Test]
        public void ConstructorWithNullParameterShallThrowExceptionTest()
        {
            Action constructorCall = () => new ReverseRegEx(null);

            constructorCall.Should()
                .Throw<ArgumentNullException>("because constructor cannot be called with null as regular expression.");
        }

        [Test]
        public void ConstructorWithInvalidParameterShallThrowExceptionTest()
        {
            Action constructorCall = () => new ReverseRegEx("[");

            constructorCall.Should()
                .Throw<InvalidOperationException>("because constructor parameter must be a valid regular expression.");
        }

        [TestCase(@"^[a-zA-Z0-9]{8,16}$")]
        [TestCase(@"^([a-z0-9\.\-]+)@([a-z0-9\-]+)((\.([a-z]){2,3})+)$")]
        [TestCase(@"^\+?(\d[\d-. ]+)?(\([\d-. ]+\))?[\d-. ]+\d$")]
        public void GetValueShallReturnMatchingStringTest(string regEx)
        {
            var sut = new ReverseRegEx(regEx);

            var result = sut.GetValue();

            result.Should().Match(r => Regex.IsMatch(result, regEx), "because the generated string must match the regular expression.");
        }
    }
}
