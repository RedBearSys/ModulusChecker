using System.Collections.Generic;
using FakeItEasy;
using ModulusChecking;
using ModulusChecking.Models;
using ModulusChecking.Steps;
using ModulusChecking.Steps.Calculators;
using Xunit;

namespace ModulusCheckingTests.Rules
{
    public class FirstModulusCalculatorStepTests
    {
        private readonly FirstDoubleAlternateCalculator _firstDoubleAlternate = A.Fake<FirstDoubleAlternateCalculator>();
        private readonly FirstStandardModulusElevenCalculator _standardEleven = A.Fake<FirstStandardModulusElevenCalculator>();
        private readonly FirstStandardModulusTenCalculator _standardTen = A.Fake<FirstStandardModulusTenCalculator>();
        private readonly IProcessAStep _gates = A.Fake<IProcessAStep>();
        private readonly FirstModulusCalculatorStep _firstCalculatorStep;

        public FirstModulusCalculatorStepTests()
        {
            A.CallTo(() => _standardTen.Process(A<BankAccountDetails>.Ignored)).Returns(true);
            A.CallTo(() => _standardEleven.Process(A<BankAccountDetails>.Ignored)).Returns(true);
            A.CallTo(() => _firstDoubleAlternate.Process(A<BankAccountDetails>.Ignored)).Returns(true);
            
            var firstStepRouter = new FirstStepRouter(_standardTen, _standardEleven, _firstDoubleAlternate);
            _firstCalculatorStep = new FirstModulusCalculatorStep(firstStepRouter, _gates);
        }

        [Fact]
        public void CallsStandardTen()
        {
            var accountDetails = BankAccountDetailsForModulusCheck(ModulusAlgorithm.Mod10);
            
            _firstCalculatorStep.Process(accountDetails);

            A.CallTo(() => _standardTen.Process(accountDetails)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _standardEleven.Process(accountDetails)).MustNotHaveHappened();
            A.CallTo(() => _firstDoubleAlternate.Process(accountDetails)).MustNotHaveHappened();
            A.CallTo(() => _gates.Process(accountDetails)).MustHaveHappenedOnceExactly();
        }
        
        [Fact]
        public void CallsStandardEleven()
        {
            var accountDetails = BankAccountDetailsForModulusCheck(ModulusAlgorithm.Mod11);
            
            _firstCalculatorStep.Process(accountDetails);
   
            A.CallTo(() => _standardTen.Process(accountDetails)).MustNotHaveHappened();
            A.CallTo(() => _standardEleven.Process(accountDetails)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _firstDoubleAlternate.Process(accountDetails)).MustNotHaveHappened();
            A.CallTo(() => _gates.Process(accountDetails)).MustHaveHappenedOnceExactly();
        }
        
        [Fact]
        public void CallsDblAl()
        {
            var accountDetails = BankAccountDetailsForModulusCheck(ModulusAlgorithm.DblAl);
            
            _firstCalculatorStep.Process(accountDetails);
            
            A.CallTo(() => _standardTen.Process(accountDetails)).MustNotHaveHappened();
            A.CallTo(() => _standardEleven.Process(accountDetails)).MustNotHaveHappened();
            A.CallTo(() => _firstDoubleAlternate.Process(accountDetails)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _gates.Process(accountDetails)).MustHaveHappenedOnceExactly();
        }

        private static BankAccountDetails BankAccountDetailsForModulusCheck(ModulusAlgorithm modulusAlgorithm)
        {
            var accountDetails = new BankAccountDetails("010004", "12345678")
            {
                WeightMappings = new List<ModulusWeightMapping>
                {
                    new ModulusWeightMapping(
                        new SortCode("010004"),
                        new SortCode("010004"),
                        modulusAlgorithm,
                        new[] {1, 2, 1, 2, 1, 1, 2, 2, 1, 2,},
                        0)
                }
            };
            return accountDetails;
        }
    }
}