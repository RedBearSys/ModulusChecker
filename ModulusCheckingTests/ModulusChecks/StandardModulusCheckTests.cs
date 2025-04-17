using ModulusChecking.Models;
using ModulusChecking.ModulusChecks;
using System.Linq;
using Xunit;

namespace ModulusCheckingTests.ModulusChecks
{
    public class StandardModulusCheckTests
    {
        private readonly StandardModulusCheck _checker = new StandardModulusCheck();

        [Theory]
        [InlineData("000000", "58177632", "012345 012346 mod10 0 0 0 0 0 0 7 5 8 3 4 6 2 1", 176)] // Basic Calculation
        [InlineData("938611", "07806039", "012345 012346 mod10 7 6 5 4 3 2 7 6 5 4 3 2 0 0", 250)] // Exception 5 where check passes
        [InlineData("827101", "28748352", "012345 012346 mod10 0 0 0 0 0 0 0 0 7 3 4 9 2 1", 132)] // Exception 3 perform both checks
        [InlineData("938063", "15764273", "012345 012346 mod10 7 6 5 4 3 2 7 6 5 4 3 2 0 0", 257)] // ExceptionFiveFirstCheckCorrectSecondIncorrect
        [InlineData("202959", "63748472", "012345 012346 mod10 0 0 0 0 0 0 0 7 6 5 4 3 2 1", 143)] // Can calculate modulus eleven sum
        public void CanCalculateSum(string sc, string an, string mappingString, int expected)
        {
            ValidateStandardModulusWeightSumCalculation(sc,an,mappingString,expected);
        }

        private void ValidateStandardModulusWeightSumCalculation(string sc, string an, string mappingString, int expected)
        {
            var details = new BankAccountDetails(sc, an)
                              {
                                  WeightMappings = new [] { ModulusWeightMapping.From(mappingString) }
                              };
            var actual = _checker.GetModulusSum(details, details.WeightMappings.First());
            Assert.Equal(expected, actual);
        }
    }
}
