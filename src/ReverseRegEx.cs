using System;
using Fare;

namespace Tynamix.ObjectFiller
{
    /// <summary>
    /// ObjectFiller plugin that generates matching strings for a given regular expression.
    /// </summary>
    public class ReverseRegEx : IRandomizerPlugin<string>
    {
        private readonly Xeger _regExGenerator;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReverseRegEx"/> class.
        /// </summary>
        /// <param name="regularExpression">The regular expression that generated strings must match.</param>
        public ReverseRegEx(string regularExpression)
        {
            if (regularExpression == null)
            {
                throw new ArgumentNullException(nameof(regularExpression), "Regular expression parameter must not be null.");
            }

            try
            {
                _regExGenerator = new Xeger(regularExpression);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Regular expression value is not supported.", e);
            }
        }

        /// <summary>
        /// Gets a random string that matches the given regular expression.
        /// </summary>
        /// <returns>
        /// Random string that matches the given regular expression.
        /// </returns>
        public string GetValue()
        {
            return _regExGenerator.Generate();
        }
    }
}
