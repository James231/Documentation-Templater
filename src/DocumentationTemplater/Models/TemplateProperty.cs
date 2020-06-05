// -------------------------------------------------------------------------------------------------
// Documentation Templater - © Copyright 2020 - Jam-Es.com
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

namespace DocumentationTemplater.Models
{
    /// <summary>
    /// Model for property within template file.
    /// </summary>
    public class TemplateProperty
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateProperty"/> class.
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <param name="defaultValue">Default value of the property.</param>
        public TemplateProperty(string name, string defaultValue)
        {
            Name = name;
            DefaultValue = defaultValue;
        }

        /// <summary>
        /// Gets or sets name for the template property.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the default value for the property.
        /// </summary>
        public string DefaultValue { get; set; }
    }
}
