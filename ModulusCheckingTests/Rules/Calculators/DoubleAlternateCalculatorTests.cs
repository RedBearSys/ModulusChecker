using System.Linq;
using FakeItEasy;
using ModulusChecking.Loaders;
using ModulusChecking.Models;
using ModulusChecking.Steps.Calculators;
using Xunit;

namespace ModulusCheckingTests.Rules.Calculators
{
    public class DoubleAlternateCalculatorTests
    {
        private readonly IModulusWeightTable _fakedModulusWeightTable;
        private readonly FirstDoubleAlternateCalculator _firstStepDblAlCalculator;
        private readonly SecondDoubleAlternateCalculator _secondStepDblAlCalculator;

        public DoubleAlternateCalculatorTests()
        {
            var mappingSource = A.Fake<IRuleMappingSource>();

            A.CallTo(() => mappingSource.GetModulusWeightMappings).Returns([
                ModulusWeightMapping.From(
                    "230872 230872 DBLAL    2    1    2    1    2    1    2    1    2    1    2    1    2    1"),
                ModulusWeightMapping.From(
                    "499273 499273 DBLAL    2   1    2    1    2    1    2    1    2    1    2    1    2    1   "),
                ModulusWeightMapping.From(
                    "499273 499273 DBLAL    2   1    2    1    2    1    2    1    2    1    2    1    2    1   "),
                ModulusWeightMapping.From(
                    "200000 200002 DBLAL    2    1    2    1    2    1    2    1    2    1    2    1    2    1   6")
            ]);

            _fakedModulusWeightTable = A.Fake<IModulusWeightTable>();
            A.CallTo(() => _fakedModulusWeightTable.RuleMappings).Returns(mappingSource.GetModulusWeightMappings.ToList());

            A.CallTo(() => _fakedModulusWeightTable.GetRuleMappings(new SortCode("499273"))).Returns([
                ModulusWeightMapping.From
                        ("499273 499273 DBLAL    2   1    2    1    2    1    2    1    2    1    2    1    2    1   "),
                    ModulusWeightMapping.From
                        ("499273 499273 DBLAL    2   1    2    1    2    1    2    1    2    1    2    1    2    1   ")
            ]);

            A.CallTo(() => _fakedModulusWeightTable.GetRuleMappings(new SortCode("118765"))).Returns([
                ModulusWeightMapping.From
                        ("110000 119280 DblAl    0   0    2    1    2    1    2    1    2    1    2    1    2    1   1")
            ]);

            _firstStepDblAlCalculator = new FirstDoubleAlternateCalculator();
            _secondStepDblAlCalculator = new SecondDoubleAlternateCalculator();
        }

        [Fact]
        public void CanProcessDoubleAlternateCheck()
        {
            var accountDetails = new BankAccountDetails("499273", "12345678");
            accountDetails.WeightMappings = _fakedModulusWeightTable.GetRuleMappings(accountDetails.SortCode);
            var result = _firstStepDblAlCalculator.Process(accountDetails);
            Assert.True(result);
        }

        [Fact]
        public void CanProcessVocaLinkDoubleAlternateWithExceptionOne()
        {

            var accountDetails = new BankAccountDetails("118765", "64371389");
            accountDetails.WeightMappings = _fakedModulusWeightTable.GetRuleMappings(accountDetails.SortCode);
            var result = _firstStepDblAlCalculator.Process(accountDetails);
            Assert.True(result);
        }

        [Fact]
        public void ExceptionFiveSecondCheckDigitIncorrect()
        {

            var accountDetails = new BankAccountDetails("938063", "15764273");
            accountDetails.WeightMappings = ModulusWeightTable.GetInstance.GetRuleMappings(accountDetails.SortCode);
            var result = _firstStepDblAlCalculator.Process(accountDetails);
            Assert.False(result);
        }

        [Fact]
        public void ExceptionFiveWhereFirstCheckPasses()
        {

            var accountDetails = new BankAccountDetails("938611", "07806039");
            accountDetails.WeightMappings = ModulusWeightTable.GetInstance.GetRuleMappings(accountDetails.SortCode);
            var result = _firstStepDblAlCalculator.Process(accountDetails);
            Assert.False(result);
        }

        [Fact]
        public void ExceptionThreeWhereCisNeitherSixNorNine()
        {
            var accountDetails = new BankAccountDetails("827101", "28748352");
            accountDetails.WeightMappings = ModulusWeightTable.GetInstance.GetRuleMappings(accountDetails.SortCode);
            var result = _secondStepDblAlCalculator.Process(accountDetails);
            Assert.True(result);
        }

        [Fact]
        public void ExceptionSixButNotAForeignAccount()
        {
            var accountDetails = new BankAccountDetails("202959", "63748472");
            accountDetails.WeightMappings = ModulusWeightTable.GetInstance.GetRuleMappings(accountDetails.SortCode);
            var result = _secondStepDblAlCalculator.Process(accountDetails);
            Assert.True(result);
        }
    }
}
