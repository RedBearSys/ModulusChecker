using ModulusChecking.Loaders;
using ModulusChecking.Models;
using NUnit.Framework;
using System.IO;
using System.Linq;

namespace ModulusCheckingTests.Loaders
{
    public class ModulusWeightTests
    {
        [Test]
        public void CanGetRuleMappings()
        {
            var modulusWeight = ModulusWeightTable.GetInstance;
            Assert.NotNull(modulusWeight.RuleMappings);

            var expected = 0;

            using (var stream = typeof(ModulusWeightTable).Assembly.GetManifestResourceStream("ModulusChecking.Resources.valacdos.txt")) 
                // ReSharper disable once AssignNullToNotNullAttribute
            using (var reader = new StreamReader(stream))
            {
                var line = reader.ReadLine();

                while (!string.IsNullOrEmpty(line))
                {
                    expected++;
                    line = reader.ReadLine();
                }
            }

            // Number of populated lines in ModulusChecking\Resources\valacdos.txt
            Assert.AreEqual(expected, modulusWeight.RuleMappings.Count());
            Assert.IsInstanceOf<ModulusWeightMapping>(modulusWeight.RuleMappings.ElementAt(0));
        }

        [Test]
        public void ThereAreNoMod10MappingsWithExceptionFive()
        {
            var modulusWeight = ModulusWeightTable.GetInstance;
            Assert.IsFalse(modulusWeight.RuleMappings.Any(rm=>rm.Exception==5 && rm.Algorithm==ModulusAlgorithm.Mod10));
        }

        [Test]
        public void AllExceptionNineRowsAreModEleven()
        {
            var modulusWeight = ModulusWeightTable.GetInstance;
            var exceptionNineRows = modulusWeight.RuleMappings.Where(rm => rm.Exception == 9).ToList();
            Assert.IsTrue(exceptionNineRows.All(r => r.Algorithm == ModulusAlgorithm.Mod11));
        }
    }
}
