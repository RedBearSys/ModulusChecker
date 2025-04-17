using FakeItEasy;
using ModulusChecking;
using ModulusChecking.Models;
using ModulusChecking.Steps.ConfirmDetailsAreValid;
using Xunit;

namespace ModulusCheckingTests.Rules
{
    public class IsUncheckableForeignAccountTests
    {
        private readonly IProcessAStep _nextStep = A.Fake<IProcessAStep>();
        private readonly IsUncheckableForeignAccount _isUncheckableForeignAccountSteps;

        private static readonly ModulusWeightMapping[] ModulusWeightMappings =
        [
            new ModulusWeightMapping(
                new SortCode("010007"),
                new SortCode("010010"),
                ModulusAlgorithm.DblAl,
                [2,1,2,1,2,1,2,1,2,1,2,1,2,1], 
                6)
        ];

        private readonly BankAccountDetails _bankAccountDetails = new BankAccountDetails("200915", "41011166")
        {
            WeightMappings = ModulusWeightMappings
        };

        public IsUncheckableForeignAccountTests()
        {
            _isUncheckableForeignAccountSteps = new IsUncheckableForeignAccount(_nextStep);
        }

        [Fact]
        public void CheckableAccountRunsNextStep()
        {
            var accountDetails = new BankAccountDetails("010008", "400000000")
            {
                WeightMappings = ModulusWeightMappings
            };
            
            _isUncheckableForeignAccountSteps.Process(accountDetails);

            A.CallTo(() => _nextStep.Process(accountDetails)).MustHaveHappened();
        }

        [Fact]
        public void CorrectlySkipsUncheckableForeignAccount()
        {
            var outcome = _isUncheckableForeignAccountSteps.Process(_bankAccountDetails);
            Assert.True(outcome);
            A.CallTo(() => _nextStep.Process(_bankAccountDetails)).MustNotHaveHappened();
        }
        
        [Fact]
        public void CanExplainUncheckableForeignAccount()
        {
            var outcome = _isUncheckableForeignAccountSteps.Process(_bankAccountDetails);
            Assert.NotEmpty(outcome.Explanation);
        }
    }
}