using FakeItEasy;
using ModulusChecking;
using ModulusChecking.Models;
using ModulusChecking.Steps;
using ModulusChecking.Steps.Calculators;
using Xunit;

namespace ModulusCheckingTests.Rules
{
    public class SecondModulusCalculatorStepTests
    {
        private readonly SecondDoubleAlternateCalculator _doubleAlternate;
        private readonly SecondStandardModulusElevenCalculator _standardEleven;
        private readonly SecondStandardModulusTenCalculator _standardTen;
        private readonly SecondStepRouter _secondStepRouter;
        private readonly IProcessAStep _nextStep;

        public SecondModulusCalculatorStepTests()
        {
            _standardTen = A.Fake<SecondStandardModulusTenCalculator>();
            _standardEleven = A.Fake<SecondStandardModulusElevenCalculator>();
            _doubleAlternate = A.Fake<SecondDoubleAlternateCalculator>();

            A.CallTo(() => _standardTen.Process(A<BankAccountDetails>.Ignored)).Returns(true);
            A.CallTo(() => _standardEleven.Process(A<BankAccountDetails>.Ignored)).Returns(true);
            A.CallTo(() => _doubleAlternate.Process(A<BankAccountDetails>.Ignored)).Returns(true);

            _secondStepRouter = new SecondStepRouter(_standardTen, _standardEleven, _doubleAlternate);

            _nextStep = A.Fake<IProcessAStep>();
        }

        [Fact]
        public void CanChooseMod10()
        {
            var secondModulusCalculatorStep = new SecondModulusCalculatorStep(_secondStepRouter, _nextStep);
            var details = BankDetailsTestMother.BankDetailsWithAlgorithm(ModulusAlgorithm.Mod10);
            secondModulusCalculatorStep.Process(details);

            A.CallTo(() => _standardTen.Process(details)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _standardEleven.Process(details)).MustNotHaveHappened();
            A.CallTo(() => _doubleAlternate.Process(details)).MustNotHaveHappened();
            A.CallTo(() => _nextStep.Process(details)).MustHaveHappenedOnceExactly();
        }
        
        [Fact]
        public void CanChooseMod11()
        {
            var secondModulusCalculatorStep = new SecondModulusCalculatorStep(_secondStepRouter, _nextStep);
            var details = BankDetailsTestMother.BankDetailsWithAlgorithm(ModulusAlgorithm.Mod11);
            secondModulusCalculatorStep.Process(details);
            
            A.CallTo(() => _standardTen.Process(details)).MustNotHaveHappened();
            A.CallTo(() => _standardEleven.Process(details)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _doubleAlternate.Process(details)).MustNotHaveHappened();
            A.CallTo(() => _nextStep.Process(details)).MustHaveHappenedOnceExactly();
        }
        
        [Fact]
        public void CanChooseDblAl()
        {
            var secondModulusCalculatorStep = new SecondModulusCalculatorStep(_secondStepRouter, _nextStep);
            var details = BankDetailsTestMother.BankDetailsWithAlgorithm(ModulusAlgorithm.DblAl);
            secondModulusCalculatorStep.Process(details);
           
            A.CallTo(() => _standardTen.Process(details)).MustNotHaveHappened();
            A.CallTo(() => _standardEleven.Process(details)).MustNotHaveHappened();
            A.CallTo(() => _doubleAlternate.Process(details)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _nextStep.Process(details)).MustHaveHappenedOnceExactly();
        }
    }
}