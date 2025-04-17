using FakeItEasy;
using ModulusChecking.Models;
using ModulusChecking.Steps;
using ModulusChecking.Steps.Calculators;
using Xunit;

namespace ModulusCheckingTests.Rules
{
    public class FirstStepRouterTests
    {
        private readonly FirstStepRouter _targetRouter;
        private readonly FirstDoubleAlternateCalculator _mockFirstDoubleAlternator;
        private readonly FirstStandardModulusElevenCalculator _mockFirstStandardElevenCalculator;
        private readonly FirstStandardModulusTenCalculator _mockFirstStandardTenCalculator;
        private readonly FirstStandardModulusElevenCalculatorExceptionFive _mockFirstStandardElevenExceptionFiveCalculator;
        private readonly FirstDoubleAlternateCalculatorExceptionFive _mockFirstDoubleAlternateExceptionFiveCalculator;
        private readonly FirstStepRouter _targetRouterForExceptionFive;

        public FirstStepRouterTests()
        {
            _mockFirstStandardTenCalculator = A.Fake<FirstStandardModulusTenCalculator>();
            _mockFirstStandardElevenExceptionFiveCalculator =
                A.Fake<FirstStandardModulusElevenCalculatorExceptionFive>();
            _mockFirstStandardElevenCalculator =
                A.Fake<FirstStandardModulusElevenCalculator>(b => b.WithArgumentsForConstructor([
                    _mockFirstStandardElevenExceptionFiveCalculator
                ]));
            _mockFirstDoubleAlternateExceptionFiveCalculator =
                A.Fake<FirstDoubleAlternateCalculatorExceptionFive>();

            _mockFirstDoubleAlternator = A.Fake<FirstDoubleAlternateCalculator>(b => b.WithArgumentsForConstructor([
                _mockFirstDoubleAlternateExceptionFiveCalculator
            ]));

            _targetRouter = new FirstStepRouter(_mockFirstStandardTenCalculator,
                                                _mockFirstStandardElevenCalculator,
                                                _mockFirstDoubleAlternator);

            _targetRouterForExceptionFive = new FirstStepRouter(_mockFirstStandardTenCalculator,
                                                                new FirstStandardModulusElevenCalculator(
                                                                    _mockFirstStandardElevenExceptionFiveCalculator),
                                                                new FirstDoubleAlternateCalculator(
                                                                    _mockFirstDoubleAlternateExceptionFiveCalculator));
        }

        [Fact]
        public void CanProcessModulusTen()
        {
            var bankDetails = new BankAccountDetails("123456", "12345678")
            {
                WeightMappings = new[]
                {
                    ModulusWeightMapping.From(
                        "090150 090156 MOD10    0    0    0    0    0    9    8    7    6    5    4    3    2    1")
                }
            };
            _targetRouter.GetModulusCalculation(bankDetails);
            A.CallTo(() => _mockFirstStandardTenCalculator.Process(bankDetails)).MustHaveHappened();
        }

        [Fact]
        public void CanProcessModulusEleven()
        {
            var bankDetails = new BankAccountDetails("123456", "12345678")
            {
                WeightMappings = new[]
                {
                    ModulusWeightMapping.From(
                        "090150 090156 MOD11    0    0    0    0    0    9    8    7    6    5    4    3    2    1")
                }
            };
            _targetRouter.GetModulusCalculation(bankDetails);
            A.CallTo(() => _mockFirstStandardElevenCalculator.Process(bankDetails)).MustHaveHappened();
        }

        [Fact]
        public void CanProcessModulusElevenExceptionFive()
        {
            var bankDetails = new BankAccountDetails("123456", "12345678")
            {
                WeightMappings = new[]
                {
                    ModulusWeightMapping.From(
                        "090150 090156 MOD11    0    0    0    0    0    9    8    7    6    5    4    3    2    1    5")
                }
            };
            _targetRouterForExceptionFive.GetModulusCalculation(bankDetails);
            A.CallTo(() => _mockFirstStandardElevenExceptionFiveCalculator.Process(bankDetails)).MustHaveHappened();
        }

        [Fact]
        public void CanProcessDoubleAlternate()
        {
            var bankDetails = new BankAccountDetails("123456", "12345678")
            {
                WeightMappings = new[]
                {
                    ModulusWeightMapping.From(
                        "090150 090156 DBLAL    0    0    0    0    0    9    8    7    6    5    4    3    2    1")
                }
            };
            _targetRouter.GetModulusCalculation(bankDetails);
            A.CallTo(() => _mockFirstDoubleAlternator.Process(bankDetails)).MustHaveHappened();
        }

        [Fact]
        public void CanProcessDoubleAlternateWithExceptionFive()
        {
            var bankDetails = new BankAccountDetails("123456", "12345678")
            {
                WeightMappings = new[]
                {
                    ModulusWeightMapping.From(
                        "090150 090156 DBLAL    0    0    0    0    0    9    8    7    6    5    4    3    2    1    5")
                }
            };
            _targetRouterForExceptionFive.GetModulusCalculation(bankDetails);
            A.CallTo(() => _mockFirstDoubleAlternateExceptionFiveCalculator.Process(bankDetails)).MustHaveHappened();
        }
    }
}
