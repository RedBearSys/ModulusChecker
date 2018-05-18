using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ModulusChecking.Loaders
{
    internal class SortCodeSubstitution
    {
        private Dictionary<string, string> _sortCodeSubstitutionSource;

        private void SetupDictionary()
        {
            if (_sortCodeSubstitutionSource != null) return;

            var assembly = Assembly.GetExecutingAssembly();
            string mappings;

            using (var stream = assembly.GetManifestResourceStream("ModulusChecking.Resources.scsubtab.txt"))
                // ReSharper disable once AssignNullToNotNullAttribute
            using (var reader = new StreamReader(stream))
            {
                mappings = reader.ReadToEnd();
            }

            _sortCodeSubstitutionSource = mappings
                .Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)
                .Select(row => row.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries))
                .Where(items => items.Length == 2)
                .ToDictionary(r => r[0], r => r[1]);
        }

        public string GetSubstituteSortCode(string original)
        {
            if (_sortCodeSubstitutionSource == null) {SetupDictionary();}
            string sub;
            Debug.Assert(_sortCodeSubstitutionSource != null, "_sortCodeSubstitutionSource != null");
            return _sortCodeSubstitutionSource.TryGetValue(original, out sub) ? sub : original;
        }
    }
}
