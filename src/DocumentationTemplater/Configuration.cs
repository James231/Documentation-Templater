// -------------------------------------------------------------------------------------------------
// Documentation Templater - © Copyright 2020 - Jam-Es.com
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

namespace DocumentationTemplater
{
    /// <summary>
    /// Stores important contstants.
    /// </summary>
    public static class Configuration
    {
        public static string DefaultNewlineChar { get; } = "\n";

        public static string[] NewlineChars { get; } = { "\r\n", "\n", "\r" };

        public static string[] RequiredTemplateFiles { get; } = { "Page.html" };
    }
}
