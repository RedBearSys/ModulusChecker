using ModulusChecking.Steps;
using Xunit;

namespace ModulusCheckingTests.Rules
{
    public class SecondStepPostProcessingStepTests
    {
        [Theory]
        [InlineData(false, false, false)]
        [InlineData(false, true, false)]
        [InlineData(true, false, false)]
        [InlineData(true, true, true)]
        public void ExceptionFiveBothChecksMustPass(bool firstCheck, bool secondCheck, bool expected)
        {
            var step = new PostProcessModulusCheckResult();

            var bankAccountDetails = new BankDetailsTestMother()
                .WithFirstWeightMapping(BankDetailsTestMother.WeightMappingWithException(5))
                .WithSecondWeightMapping(BankDetailsTestMother.WeightMappingWithException(5))
                .WithFirstCheckResult(firstCheck)
                .WithSecondCheckResult(secondCheck)
                .Build();

            var modulusCheckOutcome = step.Process(bankAccountDetails);
            
            Assert.Equal(expected, modulusCheckOutcome.Result);
            Assert.Equal("exception 5 - so first and second check must pass", modulusCheckOutcome.Explanation);
        }

        [Theory]
        [InlineData(false, true, true)]
        [InlineData(true, false, true)]
        [InlineData(false, false, false)]
        public void ExceptionTenAndElevenEitherCanPass(bool firstCheck, bool secondCheck, bool expected)
        {
            var step = new PostProcessModulusCheckResult();

            var bankAccountDetails = new BankDetailsTestMother()
                .WithFirstWeightMapping(BankDetailsTestMother.WeightMappingWithException(10))
                .WithSecondWeightMapping(BankDetailsTestMother.WeightMappingWithException(11))
                .WithFirstCheckResult(firstCheck)
                .WithSecondCheckResult(secondCheck)
                .Build();

            var modulusCheckOutcome = step.Process(bankAccountDetails);
            
            Assert.Equal(expected, modulusCheckOutcome.Result);
            Assert.Equal("exception 10 and 11 - so second or first check must pass", modulusCheckOutcome.Explanation);
        }

        [Theory]
        [InlineData(false, true, true)]
        [InlineData(true, false, true)]
        [InlineData(false, false, false)]
        public void ExceptionTwelveAndThirteenEitherCanPass(bool firstCheck, bool secondCheck, bool expected)
        {
            var step = new PostProcessModulusCheckResult();

            var bankAccountDetails = new BankDetailsTestMother()
                .WithFirstWeightMapping(BankDetailsTestMother.WeightMappingWithException(12))
                .WithSecondWeightMapping(BankDetailsTestMother.WeightMappingWithException(13))
                .WithFirstCheckResult(firstCheck)
                .WithSecondCheckResult(secondCheck)
                .Build();

            var modulusCheckOutcome = step.Process(bankAccountDetails);
            
            Assert.Equal(expected, modulusCheckOutcome.Result);
            Assert.Equal("exception 12 and 13 - so second or first check must pass", modulusCheckOutcome.Explanation);
        }

        [Theory]
        [InlineData(false, true, true)]
        [InlineData(false, false, false)]
        [InlineData(true, false, false)]
        [InlineData(true, true, true)]
        public void OtherwiseSecondCheckDeterminesResult(bool firstCheck, bool secondCheck, bool expected)
        {
            var step = new PostProcessModulusCheckResult();

            var bankAccountDetails = new BankDetailsTestMother()
                .WithFirstWeightMapping(BankDetailsTestMother.WeightMappingWithException(-1))
                .WithSecondWeightMapping(BankDetailsTestMother.WeightMappingWithException(-1))
                .WithFirstCheckResult(firstCheck)
                .WithSecondCheckResult(secondCheck)
                .Build();

            var modulusCheckOutcome = step.Process(bankAccountDetails);
            
            Assert.Equal(expected, modulusCheckOutcome.Result);
            Assert.Equal("no exceptions affect result - using second check result", modulusCheckOutcome.Explanation);
        }
    }
}