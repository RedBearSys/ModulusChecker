using ModulusChecking.Loaders;
using ModulusChecking.Models;
using ModulusChecking.Steps.Calculators;
using Xunit;

namespace ModulusCheckingTests.Rules.Calculators
{
    public class ExceptionFiveCalculationTests
    {
        private readonly DoubleAlternateCalculatorExceptionFive _secondDoubleAlternateExceptionFiveCalculator = new SecondDoubleAlternateCalculatorExceptionFive();
        private readonly FirstStandardModulusElevenCalculatorExceptionFive _standardExceptionFiveCalculator = new();

        [Fact]
        public void CanCalculateForExceptionFiveWhereCheckPasses()
        {
            var accountDetails = new BankAccountDetails("938611", "07806039");
            accountDetails.WeightMappings = ModulusWeightTable.GetInstance.GetRuleMappings(accountDetails.SortCode);
            var standardResult = _standardExceptionFiveCalculator.Process(accountDetails);
            var doubleResult = _secondDoubleAlternateExceptionFiveCalculator.Process(accountDetails);
            Assert.True(standardResult);
            Assert.True(doubleResult);
        }

        [Fact]
        public void CanCalculateForExceptionFiveWhereCheckPassesWithSubstitution()
        {
            var accountDetails = new BankAccountDetails("938600", "42368003");
            accountDetails.WeightMappings = ModulusWeightTable.GetInstance.GetRuleMappings(accountDetails.SortCode);
            var standardResult = _standardExceptionFiveCalculator.Process(accountDetails);
            var doubleResult = _secondDoubleAlternateExceptionFiveCalculator.Process(accountDetails);
            Assert.True(standardResult);
            Assert.True(doubleResult);
        }

        [Fact]
        public void CanCalculateForExceptionFiveWhereBothChecksPass()
        {
            var accountDetails = new BankAccountDetails("938063", "55065200");
            accountDetails.WeightMappings = ModulusWeightTable.GetInstance.GetRuleMappings(accountDetails.SortCode);
            var standardResult = _standardExceptionFiveCalculator.Process(accountDetails);
            var doubleResult = _secondDoubleAlternateExceptionFiveCalculator.Process(accountDetails);
            Assert.True(standardResult);
            Assert.True(doubleResult);
        }


        [Fact]
        public void CanCalculateForExceptionFiveWhereFirstCheckDigitIsCorrectAndSecondIncorrect()
        {
            var accountDetails = new BankAccountDetails("938063", "15764273");
            accountDetails.WeightMappings = ModulusWeightTable.GetInstance.GetRuleMappings(accountDetails.SortCode);
            var standardResult = _standardExceptionFiveCalculator.Process(accountDetails);
            var doubleResult = _secondDoubleAlternateExceptionFiveCalculator.Process(accountDetails);
            Assert.True(standardResult);
            Assert.False(doubleResult);
        }

        [Fact]
        public void CanCalculateForExceptionFiveWhereFirstCheckDigitIsIncorrectAndSecondIsCorrect()
        {
            var accountDetails = new BankAccountDetails("938063", "15764264");
            accountDetails.WeightMappings = ModulusWeightTable.GetInstance.GetRuleMappings(accountDetails.SortCode);
            var standardResult = _standardExceptionFiveCalculator.Process(accountDetails);
            var doubleResult = _secondDoubleAlternateExceptionFiveCalculator.Process(accountDetails);
            Assert.False(standardResult);
            Assert.True(doubleResult);
        }

        [Fact]
        public void CanCalculateForExceptionFiveWhereFirstCheckDigitIsIncorrectWithARemainderOfOne()
        {
            var accountDetails = new BankAccountDetails("938063", "15763217");
            accountDetails.WeightMappings = ModulusWeightTable.GetInstance.GetRuleMappings(accountDetails.SortCode);
            var result = _standardExceptionFiveCalculator.Process(accountDetails);
            var doubleResult = _secondDoubleAlternateExceptionFiveCalculator.Process(accountDetails);
            Assert.False(result);
            Assert.True(doubleResult);
        }
    }
}
