using ModulusChecking.Loaders;
using ModulusChecking.Models;
using ModulusChecking.Steps.ConfirmDetailsAreValid;

namespace ModulusChecking
{
    public interface IModulusChecker
    {
        bool? CheckBankAccount(string sortCode, string accountNumber);
        ModulusCheckOutcome CheckBankAccountWithExplanation(string sortCode, string accountNumber);
    }

    public class ModulusChecker : IModulusChecker
    {
        private readonly IModulusWeightTable _weightTable;

        public ModulusChecker()
        {
            _weightTable = ModulusWeightTable.GetInstance;
        }

        public bool? CheckBankAccount(string sortCode, string accountNumber) 
            => CheckBankAccountWithExplanation(sortCode, accountNumber);

        public ModulusCheckOutcome CheckBankAccountWithExplanation(string sortCode, string accountNumber)
        {
            var bankAccountDetails = new BankAccountDetails(sortCode, accountNumber);
            bankAccountDetails.WeightMappings = _weightTable.GetRuleMappings(bankAccountDetails.SortCode);
            return new HasWeightMappings().Process(bankAccountDetails);
        }
    }
}
