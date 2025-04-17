using ModulusChecking.Loaders;
using ModulusChecking.Models;
using ModulusChecking.Steps;
using Xunit;

namespace ModulusCheckingTests
{
    /// <summary>
    /// These are the test cases included in the Vocalink Validating Account Numbers document.
    /// </summary>
    public class VocalinkTestCases
    {

        private static void ValidateModulusCalculator(string sc, string an, bool expectedResult)
        {
            var accountDetails = new BankAccountDetails(sc, an);
            accountDetails.WeightMappings = ModulusWeightTable.GetInstance.GetRuleMappings(accountDetails.SortCode);
            var result = new ConfirmDetailsAreValidForModulusCheck().Process(accountDetails);
            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData("089999", "66374958", true)] // Pass modulus 10 check.
        [InlineData("107999", "88837491", true)] // Pass modulus 11 check.
        [InlineData("202959", "63748472", true)] // Pass modulus 11 and double alternate checks.
        [InlineData("203099", "66831036", false)] // Pass modulus 11 check and fail double alternate check.
        [InlineData("203099", "58716970", false)] // Fail modulus 11 check and pass double alternate check.
        [InlineData("089999", "66374959", false)] // Fail modulus 10 check.
        [InlineData("107999", "88837493", false)] // Fail modulus 11 check.
        public void CanPerformBasicCalculation(string sc, string an, bool expectedResult)
        {
            ValidateModulusCalculator(sc, an, expectedResult);
        }

        [Theory]
        [InlineData("871427", "46238510", true)] // Excptn 10 & 11 where first check passes and second fails
        [InlineData("872427", "46238510", true)] // Excptn 10 & 11 where first check fails and second passes
        [InlineData("871427", "09123496", true)] // Excptn 10 where acc. no. ab = 09 and g=9. first check passes and second fails
        [InlineData("871427", "99123496", true)] // Excptn 10 where acc. no. ab = 99 and g=9. first check passes and second fails
        public void CanPerformExceptionsTenAndElevenCalculation(string sc, string an, bool expectedResult)
        {
            ValidateModulusCalculator(sc, an, expectedResult);
        }

        /// <summary>
        /// 3 If c== 6or9 the double alternate check does not need to be carried out.
        /// Exception 3, and the sorting code is the start of a range. As c=6 the second check should be ignored.
        /// 820000 73688637 Y
        /// Exception 3, and the sorting code is the end of a range. As c=9 the second check should be ignored.
        /// 827999 73988638 Y
        /// Exception 3. As c != 6 or 9 perform both checks pass. 827101 28748352 Y
        /// </summary>
        [Theory]
        [InlineData("820000", "73688637", true)] // Excptn 3 and c = 6 so ignore second check
        [InlineData("827999", "73988638", true)] // Excptn 3 and c = 9 so ignore second check
        [InlineData("827101", "28748352", true)] // Excptn 3 and c is neither 6 nor 9. so run second check
        public void CanPerformExceptionThreeCalculation(string sc, string an, bool expectedResult)
        {
            ValidateModulusCalculator(sc, an, expectedResult);
        }

        /// <summary>
        /// Exception 4 where the remainder is equal to the checkdigit.
        /// </summary>
        [Fact]
        public void CanPerformExceptionFourCalculation()
        {
            ValidateModulusCalculator("134020", "63849203", true);
        }

        /// <summary>
        /// 
        /// and passes double alternate modulus check.
        /// </summary>
        [Theory]
        [InlineData("118765","64371389",true)] // Exception 1 – ensures that 27 has been added to the accumulated total
        [InlineData("118765", "64371388", false)] // Exception 1 where it fails double alternate check.
        public void CanPerformExceptionOneCalculation(string sc, string an, bool expectedResult)
        {
            ValidateModulusCalculator(sc, an, expectedResult);
        }

        /// <summary>
        /// Exception 6 where the account fails standard check but is a foreign currency account.
        /// </summary>
        [Fact]
        public void CanPerformExceptionSixCalculation()
        {
            ValidateModulusCalculator("200915", "41011166",true);
        }
        
        [Theory]
        [InlineData("938611","07806039", true)] // Exception 5 where the check passes
        [InlineData("938600", "42368003", true)] // Exception 5 where the check passes with substitution
        [InlineData("938063", "55065200", true)] // Exception 5 where both checks produce a remainder of 0 and pass
        [InlineData("938063", "15764273", false)] // Exception 5 where the first checkdigit is correct and the second incorrect.
        [InlineData("938063", "15764264", false)] // Exception 5 where the first checkdigit is incorrect and the second correct.
        public void CanPerformExceptionFiveCalculations(string sc, string an, bool expectedResult)
        {
            ValidateModulusCalculator(sc, an, expectedResult);
        }

        /// <summary>
        /// Exception 7 where passes but would fail the standard check.
        /// </summary>
        [Fact]
        public void CanPerformExceptionSevenCalculation()
        {
            ValidateModulusCalculator("772798", "99345694",true);
        }

        /// <summary>
        /// Exception 8 where the check passes.
        /// </summary>
        [Fact]
        public void CanPerformExceptionEightCalculation()
        {
            ValidateModulusCalculator("086090", "06774744",true);
        }

        [Theory]
        [InlineData("309070", "02355688", true)] // 2 and 9 where first passes and second fails
        [InlineData("309070", "12345668", true)] // 2 and 9 where first fails and second passes with substitution
        [InlineData("309070", "12345677", true)] // 2 and 9 second passes with no match weights
        [InlineData("309070", "99345694", true)] // 2 and 9 where second passes using one match weights
        public void CanPerformExceptionTwoAndNineCalculation(string sc, string an, bool expectedResult)
        {
            ValidateModulusCalculator(sc, an, expectedResult);
        }

        [Theory]
        [InlineData("074456", "12345112", true)] // Exception 12 and 13 where passes modulus 11 (would fail modulus 10)
        [InlineData("070116", "34012583", true)] // Exception 12 and 13 where passes modulus 11 (would pass modulus 10)
        [InlineData("074456", "11104102", true)] // Exception 12 and 13 where fails modulus 11 but passes modulus 10
        public void CanPerformExceptionTwelveAndThirteenCalculations(string sc, string an, bool expectedResult)
        {
            ValidateModulusCalculator(sc, an, expectedResult);
        }

        /// <summary>
        /// Exception 14 where the first check fails and the second check passes.
        /// </summary>
        [Theory]
        [InlineData("180002", "00000190", true)] // Exception 14 where the first check fails and the second check passes.
        [InlineData("180002", "98093517", true)] // Exception 14 where the first check passes.
        public void CanPerformExceptionFourteenCalculations(string sc, string an, bool expectedResult)
        {
            ValidateModulusCalculator(sc, an, expectedResult);
        }
    }
}
