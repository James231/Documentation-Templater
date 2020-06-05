// -------------------------------------------------------------------------------------------------
// Documentation Templater - © Copyright 2020 - Jam-Es.com
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

namespace DocumentationTemplater.Models
{
    /// <summary>
    /// Model for properties declared in pages.
    /// </summary>
    public class PProperty
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PProperty"/> class.
        /// </summary>
        /// <param name="name">Name for the property.</param>
        /// <param name="value">Value for the property.</param>
        public PProperty(string name, string value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// Gets or sets a name for the property.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value for the property.
        /// </summary>
        public string Value { get; set; }
    }
}
