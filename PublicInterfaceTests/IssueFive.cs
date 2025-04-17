using ModulusChecking;
using Xunit;

namespace PublicInterfaceTests
{
    /// <summary>
    /// See https://github.com/pauldambra/ModulusChecker/issues/5
    /// </summary>
    public class IssueFive
    {
        private const string Sortcode = "775024";
        private const string AccNumber = "26862368";

        [Fact]
        public void ItCanRevalidateDetailsOnImmediateRepeat()
        {
            var checker = new ModulusChecker();

            Assert.True(checker.CheckBankAccount(Sortcode, AccNumber));
            Assert.True(checker.CheckBankAccount(Sortcode, AccNumber));
        }

        [Theory]
        [InlineData("776203", "01193899")]
        [InlineData("089999", "66374958")]
        public void SeparatingCheckPassesInIsolation(string sc, string an)
        {
            var checker = new ModulusChecker();
            Assert.True(checker.CheckBankAccount(sc, an));
        }

        [Theory]
        [InlineData("776203", "01193899")]
        [InlineData("089999", "66374958")]
        public void ItCanRevalidateDetailsOnSeparatedRepeat(string sc, string an)
        {
            var checker = new ModulusChecker();

            Assert.True(checker.CheckBankAccount(Sortcode, AccNumber), string.Format("first check should have passed for {0} and {1}", Sortcode, AccNumber));
            Assert.True(checker.CheckBankAccount(sc, an), string.Format("separating check should have passed for {0} and {1}", sc, an));
            Assert.True(checker.CheckBankAccount(Sortcode, AccNumber), string.Format("second check should have passed for {0} and {1}", Sortcode, AccNumber));
        }
    }
}