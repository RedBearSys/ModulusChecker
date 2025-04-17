using FakeItEasy;
using ModulusChecking;
using ModulusChecking.Steps.Calculators;
using ModulusChecking.Steps.Gates;
using Xunit;

namespace ModulusCheckingTests.Rules.Gates
{
    public class ExceptionFourteenGateTests
    {
        public class WhenIsCoutts
        {
            private readonly StandardModulusExceptionFourteenCalculator _mockCalc = A.Fake<StandardModulusExceptionFourteenCalculator>();
            private readonly IProcessAStep _nextStep = A.Fake<IProcessAStep>();
            private readonly ExceptionFourteenGate _exceptionFourteenGate;

            public WhenIsCoutts()
            {
                _exceptionFourteenGate = new ExceptionFourteenGate(_mockCalc, _nextStep);
            }
                
            [Fact]
            public void ItReturnsFirstResultWhenThatPasses()
            {
                var details = BankDetailsTestMother.BankDetailsWithException(14);
                details.FirstResult = true;
                    
                _exceptionFourteenGate.Process(details);
                    
                A.CallTo(() => _nextStep.Process(details)).MustNotHaveHappened();
                A.CallTo(() => _mockCalc.Process(details)).MustNotHaveHappened();
            }
                
            [Fact]
            public void ItExplainsThatItReturnsFirstResultWhenThatPasses()
            {
                var details = BankDetailsTestMother.BankDetailsWithException(14);
                details.FirstResult = true;

                var modulusCheckOutcome = _exceptionFourteenGate.Process(details);
                    
                Assert.Equal("Coutts Account with passing first check", modulusCheckOutcome.Explanation);
            }

            [Fact]
            public void ItCallsTheExceptionCalculatorWhenTheFirstTestFails()
            {
                var details = BankDetailsTestMother.BankDetailsWithException(14);
                details.FirstResult = false;
                    
                _exceptionFourteenGate.Process(details);
            
                A.CallTo(() => _nextStep.Process(details)).MustNotHaveHappened();
                A.CallTo(() => _mockCalc.Process(details)).MustHaveHappenedOnceExactly();
            }
                
            [Fact]
            public void ItExplainsThatItCallsTheExceptionCalculatorWhenTheFirstTestFails()
            {
                var details = BankDetailsTestMother.BankDetailsWithException(14);
                details.FirstResult = false;

                var modulusCheckOutcome = _exceptionFourteenGate.Process(details);
                Assert.Equal("StandardModulusExceptionFourteenCalculator", modulusCheckOutcome.Explanation);
            }
        }

        public class WhenIsNotCoutts
        {
            [Fact]
            public void ItCallsTheNextStep()
            {
                var mockCalc = A.Fake<StandardModulusExceptionFourteenCalculator>();
                var nextStep = A.Fake<IProcessAStep>();
                var gate = new ExceptionFourteenGate(mockCalc, nextStep);

                var details = BankDetailsTestMother.BankDetailsWithException(0);

                gate.Process(details);
               
                A.CallTo(() => nextStep.Process(details)).MustHaveHappenedOnceExactly();
                A.CallTo(() => mockCalc.Process(details)).MustNotHaveHappened();
            }
        }

        
    }
}