using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ModulusChecking.Models;

namespace ModulusChecking.Loaders
{
    public class ValacdosSource : IRuleMappingSource
    {
        public IEnumerable<ModulusWeightMapping> GetModulusWeightMappings { get; }

        public ValacdosSource()
        {
            var assembly = Assembly.GetExecutingAssembly();
            string mappings;

            using (var stream = assembly.GetManifestResourceStream("ModulusChecking.Resources.valacdos.txt"))
                // ReSharper disable once AssignNullToNotNullAttribute
            using (var reader = new StreamReader(stream))
            {
                mappings = reader.ReadToEnd();
            }

            if (mappings.Contains('\t'))
                throw new Exception(
                    "valacdos.txt contains a tab instead of a set of spaces.\n\nUse a text editor to search for regex \\t to identify the issue.");

            GetModulusWeightMappings = mappings
                .Trim()
                .Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)
                .Where(row => row.Length > 0)
                .Select(row => ModulusWeightMapping.From(row))
                .ToArray();
        }
    }
}