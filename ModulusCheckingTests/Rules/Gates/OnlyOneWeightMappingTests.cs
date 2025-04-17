using FakeItEasy;
using ModulusChecking;
using ModulusChecking.Models;
using ModulusChecking.Steps.Gates;
using Xunit;

namespace ModulusCheckingTests.Rules.Gates
{
    public class OnlyOneWeightMappingTests
    {
        private readonly IProcessAStep _nextStep;
        private readonly OnlyOneWeightMappingGate _onlyOneWeightMappingGate;
        private readonly BankAccountDetails _bankAccountDetails;

        public OnlyOneWeightMappingTests()
        {
            _nextStep = A.Fake<IProcessAStep>();
            _onlyOneWeightMappingGate = new OnlyOneWeightMappingGate(_nextStep);
            _bankAccountDetails = new BankAccountDetails("000000", "00000000");
        }
        
        [Fact]
        public void IfThereIsOnlyOneMappingItReturns()
        {
            _bankAccountDetails.WeightMappings =
            [
                BankDetailsTestMother.AnyModulusWeightMapping()
            ];

            _onlyOneWeightMappingGate.Process(_bankAccountDetails);
            
            A.CallTo(() => _nextStep.Process(A<BankAccountDetails>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public void ItCanExplainThatThereWasOnlyOneMapping()
        {
            _bankAccountDetails.WeightMappings =
            [
                BankDetailsTestMother.AnyModulusWeightMapping()
            ];

            var modulusCheckOutcome = _onlyOneWeightMappingGate.Process(_bankAccountDetails);

            Assert.Equal("not proceeding to the second check as there is only one weight mapping", modulusCheckOutcome.Explanation);
        }

        [Fact]
        public void IfThereAreTwoMappingsItCallsTheNextStep()
        {
            _bankAccountDetails.WeightMappings =
            [
                BankDetailsTestMother.AnyModulusWeightMapping(),
                BankDetailsTestMother.AnyModulusWeightMapping()
            ];

            _onlyOneWeightMappingGate.Process(_bankAccountDetails);
            
            A.CallTo(() => _nextStep.Process(_bankAccountDetails)).MustHaveHappenedOnceExactly();
        }
        
        
    }
}