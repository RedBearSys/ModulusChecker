using ModulusChecking.Loaders;
using Xunit;

namespace ModulusCheckingTests.Loaders
{
    public class SortCodeSubstitutionTests
    {
        
        private readonly SortCodeSubstitution _substituter = new SortCodeSubstitution();

        [Theory]
        [InlineData("938289","938068")]
        [InlineData("938297","938076")]
        [InlineData("938600","938611")]
        [InlineData("938602","938343")]
        [InlineData("938604","938603")]
        [InlineData("938608","938408")]
        [InlineData("938609","938424")]
        [InlineData("938613","938017")]
        [InlineData("938616","938068")]
        [InlineData("123456","123456")]
        public void CanCorrectlySubstituteSortCodes(string orig, string sub)
        {
            Assert.Equal(sub, _substituter.GetSubstituteSortCode(orig));
        }
    }
}
