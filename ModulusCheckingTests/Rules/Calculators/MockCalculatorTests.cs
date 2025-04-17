using FakeItEasy;
using ModulusChecking.Loaders;
using ModulusChecking.Models;
using ModulusChecking.Steps.Calculators;
using Xunit;

namespace ModulusCheckingTests.Rules.Calculators
{
    public class MockCalculatorTests
    {
        private readonly IModulusWeightTable _fakedModulusWeightTable;

        public MockCalculatorTests()
        {
            var mappingSource = A.Fake<IRuleMappingSource>();
            A.CallTo(() => mappingSource.GetModulusWeightMappings).Returns([
                ModulusWeightMapping.From(
                    "000000 000100 MOD10 0 0 0 0 0 0 7 5 8 3 4 6 2 1 "),
                ModulusWeightMapping.From(
                    "499273 499273 DBLAL    0    0    2    1    2    1    2    1    2    1    2    1    2    1   1"),
                ModulusWeightMapping.From(
                    "200000 200002 DBLAL    2    1    2    1    2    1    2    1    2    1    2    1    2    1   6")
            ]);

            _fakedModulusWeightTable = A.Fake<IModulusWeightTable>();
            A.CallTo(() => _fakedModulusWeightTable.RuleMappings).Returns(mappingSource.GetModulusWeightMappings);
            A.CallTo(() => _fakedModulusWeightTable.GetRuleMappings(new SortCode("000000"))).Returns([
                ModulusWeightMapping.From("000000 000100 MOD10 0 0 0 0 0 0 7 5 8 3 4 6 2 1 ")
            ]);
        }

        [Fact]
        public void CanProcessStandardElevenCheck()
        {
            var accountDetails = new BankAccountDetails("000000", "58177632");
            accountDetails.WeightMappings = _fakedModulusWeightTable.GetRuleMappings(accountDetails.SortCode);
            var result = new FirstStandardModulusElevenCalculator().Process(accountDetails);
            Assert.True(result);
        }

        [Fact]
        //vocalink test case
        public void CanProcessVocalinkStandardTenCheck()
        {
            var accountDetails = new BankAccountDetails("089999", "66374958");
            accountDetails.WeightMappings = ModulusWeightTable.GetInstance.GetRuleMappings(accountDetails.SortCode);
            var result = new FirstStandardModulusTenCalculator().Process(accountDetails);
            Assert.True(result);
        }

        [Fact]
        public void CanProcessVocalinkStandardEleven()
        {
            var accountDetails = new BankAccountDetails("107999", "88837491");
            accountDetails.WeightMappings = ModulusWeightTable.GetInstance.GetRuleMappings(accountDetails.SortCode);
            var result = new FirstStandardModulusElevenCalculator().Process(accountDetails);
            Assert.True(result);
        }

    }
}
