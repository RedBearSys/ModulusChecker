using FakeItEasy;
using ModulusChecking;
using ModulusChecking.Steps.Gates;
using Xunit;

namespace ModulusCheckingTests.Rules.Gates
{
    public class NoSecondCheckRequiredGateTests
    {
        private readonly IProcessAStep _nextStep;
        private readonly IsSecondCheckRequiredGate _isSecondCheckRequiredGate;

        public NoSecondCheckRequiredGateTests()
        {
            _nextStep = A.Fake<IProcessAStep>();
            _isSecondCheckRequiredGate = new IsSecondCheckRequiredGate(_nextStep);
        }
        
        [Theory]
        [InlineData(2)]
        [InlineData(9)]
        [InlineData(10)]
        [InlineData(11)]
        [InlineData(12)]
        [InlineData(13)]
        [InlineData(14)]
        public void ExceptionRequiresSecondCheckItCallsNext(int exception)
        {
            var bankAccountDetails = BankDetailsTestMother.BankDetailsWithException(exception);

            _isSecondCheckRequiredGate.Process(bankAccountDetails);
            
            A.CallTo(() => _nextStep.Process(bankAccountDetails)).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(15)]
        public void DoesNotCallSecondCheckIfExceptionDoesNotRequireIt(int exception)
        {
            var bankAccountDetails = BankDetailsTestMother.BankDetailsWithException(exception);

            _isSecondCheckRequiredGate.Process(bankAccountDetails);

            A.CallTo(() => _nextStep.Process(bankAccountDetails)).MustNotHaveHappened();
        }

        [Fact]
        public void CanExplainWhyItDoesNotCallSecondCheck()
        {
            var bankAccountDetails = BankDetailsTestMother.BankDetailsWithException(3);

            var modulusCheckOutcome = _isSecondCheckRequiredGate.Process(bankAccountDetails);
            
            Assert.Equal("first weight mapping exception does not require second check", modulusCheckOutcome.Explanation);
        }
    }
}