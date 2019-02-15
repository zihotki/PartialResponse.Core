// Copyright (c) Arjen Post and contributors. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;

namespace PartialResponse.Core
{
    /// <summary>
    /// Represents delimiter options for parser
    /// </summary>
    public class DelimiterOptions
    {
        private static DelimiterOptions defaultOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelimiterOptions"/> class.
        /// </summary>
        /// <param name="fieldsDelimiters">Characters representing field delimiters.</param>
        /// <param name="nestedFieldDelimiters">Characters representing nested field delimiters.</param>
        /// <param name="fieldGroupStartDelimiters">Characters representing field group start delimiters.</param>
        /// <param name="fieldGroupEndDelimiters">Characters representing field group end delimiters.</param>
        public DelimiterOptions(char[] fieldsDelimiters, char[] nestedFieldDelimiters, char[] fieldGroupStartDelimiters, char[] fieldGroupEndDelimiters)
        {
            var map = new Dictionary<char, TokenType>();

            foreach (var c in fieldsDelimiters ?? throw new ArgumentNullException(nameof(fieldsDelimiters)))
            {
                map.Add(c, TokenType.FieldsDelimiter);
            }

            foreach (var c in nestedFieldDelimiters ?? throw new ArgumentNullException(nameof(nestedFieldDelimiters)))
            {
                map.Add(c, TokenType.NestedFieldDelimiter);
            }

            foreach (var c in fieldGroupStartDelimiters ?? throw new ArgumentNullException(nameof(fieldGroupStartDelimiters)))
            {
                map.Add(c, TokenType.FieldGroupStartDelimiter);
            }

            foreach (var c in fieldGroupEndDelimiters ?? throw new ArgumentNullException(nameof(fieldGroupEndDelimiters)))
            {
                map.Add(c, TokenType.FieldGroupEndDelimiter);
            }

            this.DelimiterToTokenTypeMap = map;

            this.FieldsDelimiters = fieldsDelimiters;
            this.NestedFieldDelimiters = nestedFieldDelimiters;
            this.FieldGroupStartDelimiters = fieldGroupStartDelimiters;
            this.FieldGroupEndDelimiters = fieldGroupEndDelimiters;
        }

        /// <summary>
        /// Gets default options for parser. Default options are:
        ///   field delimiters - ','
        ///   nested field delimiters - '/'
        ///   field group start delimiters - '('
        ///   field group end delimiters - ')'
        /// </summary>
        public static DelimiterOptions DefaultOptions
        {
            get
            {
                return defaultOptions
                    ?? (defaultOptions = new DelimiterOptions(new[] { ',' }, new[] { '/' }, new[] { '(' }, new[] { ')' }));
            }
        }

        /// <summary>
        /// Gets delimiter to token type map.
        /// </summary>
        public IReadOnlyDictionary<char, TokenType> DelimiterToTokenTypeMap { get; }

        /// <summary>
        /// Gets fields delimiters.
        /// </summary>
        public char[] FieldsDelimiters { get; }

        /// <summary>
        /// Gets nested field delimiters.
        /// </summary>
        public char[] NestedFieldDelimiters { get; }

        /// <summary>
        /// Gets nested field group start delimiters.
        /// </summary>
        public char[] FieldGroupStartDelimiters { get; }

        /// <summary>
        /// Gets nested field group end delimiters.
        /// </summary>
        public char[] FieldGroupEndDelimiters { get; }
    }
}