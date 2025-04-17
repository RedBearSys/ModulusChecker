using ModulusChecking.Loaders;
using ModulusChecking.Models;
using System.IO;
using System.Linq;
using Xunit;

namespace ModulusCheckingTests.Loaders
{
    public class ModulusWeightTests
    {
        [Fact]
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
            Assert.Equal(expected, modulusWeight.RuleMappings.Count());
        }

        [Fact]
        public void ThereAreNoMod10MappingsWithExceptionFive()
        {
            var modulusWeight = ModulusWeightTable.GetInstance;
            Assert.DoesNotContain(modulusWeight.RuleMappings, rm => rm.Exception == 5 && rm.Algorithm == ModulusAlgorithm.Mod10);
        }

        [Fact]
        public void AllExceptionNineRowsAreModEleven()
        {
            var modulusWeight = ModulusWeightTable.GetInstance;
            var exceptionNineRows = modulusWeight.RuleMappings.Where(rm => rm.Exception == 9).ToList();
            Assert.True(exceptionNineRows.All(r => r.Algorithm == ModulusAlgorithm.Mod11));
        }
    }
}
