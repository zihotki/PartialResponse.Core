// Copyright (c) Arjen Post and contributors. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PartialResponse.Core
{
    /// <summary>
    /// Represents a collection of fields.
    /// </summary>
    /// <remarks>This type supports the <see cref="Fields"/> infrastructure and is not intended to be used directly
    /// from your code.</remarks>
    public struct Fields
    {
        private readonly IEnumerable<Field> values;

        private Fields(IEnumerable<Field> values)
        {
            this.values = values;
        }

        /// <summary>
        /// Gets a collection containing the fields.
        /// </summary>
        /// <returns>A collection containing the fields</returns>
        public IEnumerable<Field> Values
        {
            get { return this.values ?? Enumerable.Empty<Field>(); }
        }

        /// <summary>
        /// Converts to value to a <see cref="Fields"/> object.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="result">When this method returns, contains the <see cref="Fields"/> equivalent of the value,
        /// if the conversion succeeded, or null if the conversion failed.</param>
        /// <param name="options">Optional options which allow to specify custom delimiters. If no value provided
        /// a default options <see cref="DelimiterOptions.DefaultOptions"/> are used.</param>
        /// <returns>true if value was converted successfully; otherwise, false.</returns>
        public static bool TryParse(string value, out Fields result, DelimiterOptions options = null)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            using (var reader = new StringReader(value))
            {
                var context = new ParserContext(reader, options ?? DelimiterOptions.DefaultOptions);
                var parser = new Parser(context);

                parser.Parse();

                if (context.Error != null)
                {
                    result = default(Fields);

                    return false;
                }

                result = new Fields(context.Values);

                return true;
            }
        }

        /// <summary>
        /// Indicates whether a field matches the specified value.
        /// </summary>
        /// <param name="value">The value to match.</param>
        /// <param name="delimiterOptions">Delimiter options to use when matching. If no options provided then the
        /// default options <see cref="DelimiterOptions.DefaultOptions"/> are used.</param>
        /// <returns>true if a field matches the specified value; otherwise, false.</returns>
        public bool Matches(string value, DelimiterOptions delimiterOptions = null)
        {
            return this.Matches(value, false, delimiterOptions);
        }

        /// <summary>
        /// Indicates whether a field matches the specified value.
        /// </summary>
        /// <param name="value">The value to match.</param>
        /// <param name="ignoreCase">A value which indicates whether matching should be case-insensitive.</param>
        /// <param name="delimiterOptions">Delimiter options to use when matching. If no options provided then the
        /// default options <see cref="DelimiterOptions.DefaultOptions"/> are used.</param>
        /// <returns>true if a field matches the specified value; otherwise, false.</returns>
        public bool Matches(string value, bool ignoreCase, DelimiterOptions delimiterOptions = null)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (!this.Values.Any())
            {
                return false;
            }

            var separators = (delimiterOptions ?? DelimiterOptions.DefaultOptions).NestedFieldDelimiters;
            var parts = value.Split(separators);

            return this.Values.Any(field => field.Matches(parts, ignoreCase));
        }
    }
}
