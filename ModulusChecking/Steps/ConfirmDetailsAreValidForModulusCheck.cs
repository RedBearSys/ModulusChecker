using ModulusChecking.Models;

namespace ModulusChecking.Steps
{
    internal class ConfirmDetailsAreValidForModulusCheck
    {
        // Fields
        private readonly FirstModulusCalculatorStep _firstModulusCalculatorStep;

        // Methods
        public ConfirmDetailsAreValidForModulusCheck()
        {
            this._firstModulusCalculatorStep = new FirstModulusCalculatorStep();
        }

        public ConfirmDetailsAreValidForModulusCheck(FirstModulusCalculatorStep nextStep)
        {
            this._firstModulusCalculatorStep = nextStep;
        }

        public bool Process(BankAccountDetails bankAccountDetails)
        {
            bool flag = bankAccountDetails.IsUncheckableForeignAccount();
            return ((!bankAccountDetails.IsValidForModulusCheck() | flag) || this._firstModulusCalculatorStep.Process(bankAccountDetails));
        }
    }
}
