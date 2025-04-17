using FakeItEasy;
using ModulusChecking;
using ModulusChecking.Models;
using ModulusChecking.Steps.Gates;
using Xunit;

namespace ModulusCheckingTests.Rules.Gates
{
    public class ExceptionThreeGateTests
    {
        private readonly IProcessAStep _nextStep;
        private readonly IsExceptionThreeAndCanSkipSecondCheck _isExceptionThreeAndCanSkipSecondCheck;

        public ExceptionThreeGateTests()
        {
            _nextStep = A.Fake<IProcessAStep>();
            _isExceptionThreeAndCanSkipSecondCheck = new IsExceptionThreeAndCanSkipSecondCheck(_nextStep);
        }
        
        [Theory]
        [InlineData("6")] // When account number is 6
        [InlineData("9")] // When account number is 9
        public void CanSkipSecondCheckWhenExceptionThreeAndSixOrNineAtPositionTwoInAccountNumber(string accountNumber)
        {
            var bankAccountDetails = new BankAccountDetails("012345", $"00{accountNumber}00000")
            {
                WeightMappings = new[]
                {
                    BankDetailsTestMother.WeightMappingWithException(3),
                    BankDetailsTestMother.WeightMappingWithException(3)
                }
            };

            _isExceptionThreeAndCanSkipSecondCheck.Process(bankAccountDetails);

            A.CallTo(() => _nextStep.Process(bankAccountDetails)).MustNotHaveHappened();
        }

        [Fact]
        public void CanExplainSkippingSecondCheck()
        {
            var bankAccountDetails = new BankAccountDetails("012345", "00600000")
            {
                WeightMappings =
                [
                    BankDetailsTestMother.WeightMappingWithException(3),
                    BankDetailsTestMother.WeightMappingWithException(3)
                ]
            };

            var modulusCheckOutcome = _isExceptionThreeAndCanSkipSecondCheck.Process(bankAccountDetails);
            
            Assert.Equal("IsExceptionThreeAndCanSkipSecondCheck", modulusCheckOutcome.Explanation);
        }

        [Theory]
        [InlineData(1, "6")] // When Exception is not three
        [InlineData(3, "5")] // When account number character is not six or nine
        public void CallsNextStepWhenAccountRequiresSecondCheck(int exception, string accountNumberCharacter)
        {
            var bankAccountDetails = new BankAccountDetails("012345", $"00{accountNumberCharacter}00000")
            {
                WeightMappings =
                [
                    BankDetailsTestMother.WeightMappingWithException(3),
                    BankDetailsTestMother.WeightMappingWithException(exception)
                ]
            };

            _isExceptionThreeAndCanSkipSecondCheck.Process(bankAccountDetails);

            A.CallTo(() => _nextStep.Process(bankAccountDetails)).MustHaveHappenedOnceExactly();
        }
        
    }
}