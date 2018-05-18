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

            GetModulusWeightMappings = mappings
                .Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)
                .Where(row => row.Length > 0)
                .Select(row => ModulusWeightMapping.From(row))
                .ToArray();
        }
    }
}