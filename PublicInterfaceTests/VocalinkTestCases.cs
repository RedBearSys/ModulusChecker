using ModulusChecking;
using Xunit;
using Xunit.Abstractions;

namespace PublicInterfaceTests
{
    public class VocalinkTestCases
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly ModulusChecker _modulusChecker = new ModulusChecker();

        public VocalinkTestCases(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Theory]
        [InlineData("089999", "66374958", true)] // 1. Pass modulus 10 check.
        [InlineData("107999", "88837491", true)] // 2. Pass modulus 11 check.
        [InlineData("202959", "63748472", true)] // 3. Pass modulus 11 and double alternate checks.
        [InlineData("871427", "46238510", true)] // 4. Exception 10 & 11 where first check passes and second fails
        [InlineData("872427", "46238510", true)] // 5. Exception 10 & 11 where first check fails and second passes
        [InlineData("871427", "09123496", true)] // 6. Exception 10 where acc. no. ab = 09 and g=9. first check passes and second fails
        [InlineData("871427", "99123496", true)] // 7. Exception 10 where acc. no. ab = 99 and g=9. first check passes and second fails
        [InlineData("820000", "73688637", true)] // 8. Exception 3 and c = 6 so ignore second check
        [InlineData("827999", "73988638", true)] // 9. Exception 3 and c = 9 so ignore second check
        [InlineData("827101", "28748352", true)] // 10. Exception 3 and c is neither 6 nor 9. so run second check
        [InlineData("134020", "63849203", true)] // 11. Exception 4 where the remainder is equal to the checkdigit.
        [InlineData("118765", "64371389", true)] // 12. Exception 1 – ensures that 27 has been added to the accumulated total
        [InlineData("200915", "41011166", true)] // 13. Exception 6 where the account fails standard check but is a foreign currency account.
        [InlineData("938611", "07806039", true)] // 14. Exception 5 where the check passes
        [InlineData("938600", "42368003", true)] // 15. Exception 5 where the check passes with substitution
        [InlineData("938063", "55065200", true)] // 16. Exception 5 where both checks produce a remainder of 0 and pass
        [InlineData("772798", "99345694", true)] // 17. Exception 7 where passes but would fail the standard check.
        [InlineData("086090", "06774744", true)] // 18. Exception 8 where the check passes.
        [InlineData("309070", "02355688", true)] // 19. 2 and 9 where first passes and second fails
        [InlineData("309070", "12345668", true)] // 20. 2 and 9 where first fails and second passes with substitution
        [InlineData("309070", "12345677", true)] // 21. 2 and 9 second passes with no match weights
        [InlineData("309070", "99345694", true)] // 22. 2 and 9 where second passes using one match weights
        [InlineData("938063", "15764273", false)] // 23. Exception 5 where the first checkdigit is correct and the second incorrect.
        [InlineData("938063", "15764264", false)] // 24. Exception 5 where the first checkdigit is incorrect and the second correct.
        [InlineData("938063", "15763217", false)] // 25. Exception 5 where the first checkdigit is incorrect with a remainder of 1.
        [InlineData("118765", "64371388", false)] // 26. Exception 1 where it fails double alternate check.
        [InlineData("203099", "66831036", false)] // 27. Pass modulus 11 check and fail double alternate check.
        [InlineData("203099", "58716970", false)] // 28. Fail modulus 11 check and pass double alternate check.
        [InlineData("089999", "66374959", false)] // 29. Fail modulus 10 check.
        [InlineData("107999", "88837493", false)] // 30. Fail modulus 11 check.
        [InlineData("074456", "12345112", true)] // 31. Exception 12 and 13 where passes modulus 11 (would fail modulus 10)
        [InlineData("070116", "34012583", true)] // 32. Exception 12 and 13 where passes modulus 11 (would pass modulus 10)
        [InlineData("074456", "11104102", true)] // 33. Exception 12 and 13 where fails modulus 11 but passes modulus 10
        [InlineData("180002", "00000190", true)] // 34. Exception 14 where the first check fails and the second check passes.
        public void CanPassCurrentVocalinkTestCases(string sc, string an, bool expectedResult)
        {
            Assert.Equal(expectedResult,_modulusChecker.CheckBankAccount(sc, an));

            var outcomeWithExplanation = _modulusChecker.CheckBankAccountWithExplanation(sc, an);
            Assert.Equal(expectedResult,outcomeWithExplanation.Result);
            
            _testOutputHelper.WriteLine(outcomeWithExplanation.Explanation);
            Assert.NotEmpty(outcomeWithExplanation.Explanation);
        }
    }
}
