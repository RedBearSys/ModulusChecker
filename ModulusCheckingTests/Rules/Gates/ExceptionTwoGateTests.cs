using FakeItEasy;
using ModulusChecking;
using ModulusChecking.Steps.Gates;
using Xunit;

namespace ModulusCheckingTests.Rules.Gates
{
    /// <summary>
    ///          public bool IsExceptionTwoAndFirstCheckPassed()
    ///        {
    ///            return FirstResult &amp;&amp; WeightMappings.First().Exception == 2;
    ///        }
    /// </summary>
    public class ExceptionTwoGateTests
    {
        private readonly IProcessAStep _nextStep;
        private readonly IsExceptionTwoAndFirstCheckPassedGate _isExceptionTwoAndFirstCheckPassedGate;

        public ExceptionTwoGateTests()
        {
            _nextStep = A.Fake<IProcessAStep>();
            _isExceptionTwoAndFirstCheckPassedGate = new IsExceptionTwoAndFirstCheckPassedGate(_nextStep);
        }
        
        [Fact]
        public void CanSkipSecondCheckForExceptionTwoWithPassedFirstCheck()
        {
            var details = BankDetailsTestMother.BankDetailsWithException(2);
            details.FirstResult = true;
            
            _isExceptionTwoAndFirstCheckPassedGate.Process(details);
            
            A.CallTo(() => _nextStep.Process(details)).MustNotHaveHappened();
        }

        [Fact]
        public void CanExplainSkippingSecondCheck()
        {
            var details = BankDetailsTestMother.BankDetailsWithException(2);
            details.FirstResult = true;

            var modulusCheckOutcome = _isExceptionTwoAndFirstCheckPassedGate.Process(details);
            
            Assert.Equal("IsExceptionTwoAndFirstCheckPassed", modulusCheckOutcome.Explanation);
        }

        [Theory]
        [InlineData(1, true)] // When exception is not 2
        [InlineData(2, false)] // When first check failed
        public void CanCallNextStepWhenAccountDoesNotQualifyToSkip(int exception, bool firstCheck)
        {
            var details = BankDetailsTestMother.BankDetailsWithException(exception);
            details.FirstResult = firstCheck;
            
            _isExceptionTwoAndFirstCheckPassedGate.Process(details);
            
            A.CallTo(() => _nextStep.Process(details)).MustHaveHappenedOnceExactly();
        }
    }
}