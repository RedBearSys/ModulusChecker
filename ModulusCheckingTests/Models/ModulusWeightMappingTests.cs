using ModulusChecking.Models;
using System;
using System.Linq;
using Xunit;

namespace ModulusCheckingTests.Models
{
    public class ModulusWeightMappingTests
    {
        [Theory]
        [InlineData("123456 001234 MOD11 2 1 2 1 2 1 2 1 2 1 2 1 2 1", ModulusAlgorithm.Mod11)]
        [InlineData("123456 001234 MoD11 2 1 2 1 2 1 2 1 2 1 2 1 2 1", ModulusAlgorithm.Mod11)]
        [InlineData("123456 001234 MOD10 2 1 2 1 2 1 2 1 2 1 2 1 2 1", ModulusAlgorithm.Mod10)]
        [InlineData("123456 001234 mod10 2 1 2 1 2 1 2 1 2 1 2 1 2 1", ModulusAlgorithm.Mod10)]
        [InlineData("123456 001234 DBLAL 2 1 2 1 2 1 2 1 2 1 2 1 2 1", ModulusAlgorithm.DblAl)]
        [InlineData("123456 001234 dBlAl 2 1 2 1 2 1 2 1 2 1 2 1 2 1", ModulusAlgorithm.DblAl)]
        public void CanAddAlgorithm(string row, ModulusAlgorithm expected)
        {
            var actual = ModulusWeightMapping.From(row);
            Assert.Equal(expected, actual.Algorithm);
        }

        [Fact]
        public void CannotAddUnknownAlgorithm()
        {
            Assert.Throws<ArgumentException>(() =>
                ModulusWeightMapping.From("123456 001234 PLOPPY 2 1 2 1 2 1 2 1 2 1 2 1 2 1"));
        }
        
        [Fact]
        public void CanLoadWeightingValues()
        {
            var actual = ModulusWeightMapping.From("230872 230872 DBLAL    2    1    2    1    2    1    2    1    2    1    2    1    2    1");
            var expectedWeightValues = new[] {2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1};
            for(var i = 0; i<actual.WeightValues.Count(); i++)
            {
                Assert.Equal(actual.WeightValues.ElementAt(i),expectedWeightValues[i]);
            }
        }
    }
}
