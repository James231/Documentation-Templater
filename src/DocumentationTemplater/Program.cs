// -------------------------------------------------------------------------------------------------
// Documentation Templater - © Copyright 2020 - Jam-Es.com
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.IO;

namespace DocumentationTemplater
{
    /// <summary>
    /// Contains application entry point.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Main entry point of the application.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        public static void Main(string[] args)
        {
            // Getting the path to the exe needs to be done using GetCurrentProcess as for a published single file exe
            //   the assembly path is in a weird temporary folder.
            //   https://github.com/dotnet/runtime/issues/3704
            string exePath = Process.GetCurrentProcess().MainModule.FileName;
            string exeDir = Path.GetDirectoryName(exePath);

            string inputPath = Path.Combine(exeDir, "Input");
            string outputPath = Path.Combine(exeDir, "Output");
            string templatePath = Path.Combine(exeDir, "Template");
            Directory.CreateDirectory(inputPath);
            Directory.CreateDirectory(outputPath);
            Directory.CreateDirectory(templatePath);

            Generator generator = new Generator(inputPath, outputPath, templatePath);
            generator.Generate();

            Console.WriteLine("Finished.");
        }
    }
}
