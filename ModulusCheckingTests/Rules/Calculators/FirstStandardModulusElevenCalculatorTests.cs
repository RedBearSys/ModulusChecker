using ModulusChecking.Loaders;
using ModulusChecking.Models;
using ModulusChecking.Steps.Calculators;
using Xunit;

namespace ModulusCheckingTests.Rules.Calculators
{
    public class FirstStandardModulusElevenCalculatorTests
    {
        private readonly FirstStandardModulusElevenCalculator _calculator = new FirstStandardModulusElevenCalculator();

        [Fact]
        public void ExceptionThreeWhereCisNeitherSixNorNine()
        {
            var accountDetails = new BankAccountDetails("827101", "28748352");
            accountDetails.WeightMappings = ModulusWeightTable.GetInstance.GetRuleMappings(accountDetails.SortCode);
            var result = _calculator.Process(accountDetails);
            Assert.True(result);
        }

        [Fact]
        public void CanPassBasicModulus11Test()
        {
            var accountDetails = new BankAccountDetails("202959", "63748472");
            accountDetails.WeightMappings = ModulusWeightTable.GetInstance.GetRuleMappings(accountDetails.SortCode);
            var result = _calculator.Process(accountDetails);
            Assert.True(result);
        }
    }
}
