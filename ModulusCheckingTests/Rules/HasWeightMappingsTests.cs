using System.Collections.Generic;
using FakeItEasy;
using ModulusChecking;
using ModulusChecking.Models;
using ModulusChecking.Steps.ConfirmDetailsAreValid;
using Xunit;

namespace ModulusCheckingTests.Rules
{
    public class HasWeightMappingsTests
    {
        private readonly HasWeightMappings _hasWeightMappingsStep;
        private readonly IProcessAStep _nextStep;

        private readonly List<ModulusWeightMapping> _literallyAnyMapping =
        [
            new ModulusWeightMapping(
                new SortCode("010004"),
                new SortCode("010004"),
                ModulusAlgorithm.DblAl,
                [0, 1, 2],
                1)
        ];

        public HasWeightMappingsTests()
        {
            _nextStep = A.Fake<IProcessAStep>(); 
            _hasWeightMappingsStep = new HasWeightMappings(_nextStep);
        }

        [Fact]
        public void UnknownSortCodeIsNull()
        {
            const string sortCode = "123456";
            var accountDetails = new BankAccountDetails(sortCode, "12345678")
            {
                //unknown sort code loads no weight mappings
                WeightMappings = new List<ModulusWeightMapping>()
            };
            var result = _hasWeightMappingsStep.Process(accountDetails);
            Assert.Null(result.Result);
        }
        
        [Fact]
        public void UnknownSortCodeCanBeExplained()
        {
            var accountDetails = new BankAccountDetails("123456", "12345678")
            {
                //unknown sort code loads no weight mappings
                WeightMappings = new List<ModulusWeightMapping>()
            };
            
            var result = _hasWeightMappingsStep.Process(accountDetails);
            
            Assert.NotEmpty(result.Explanation);
        }

        [Fact]
        public void KnownSortCodeIsTested()
        {
            var accountDetails = new BankAccountDetails("010004", "12345678")
            {
                WeightMappings = _literallyAnyMapping
            };

            _hasWeightMappingsStep.Process(accountDetails);
           
            A.CallTo(() => _nextStep.Process(accountDetails)).MustHaveHappened();
        }
    }
}