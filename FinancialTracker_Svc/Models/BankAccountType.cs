using System.Collections.Generic;

namespace FinancialTracker_Svc.Models
{
    public class BankAccountType
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class BankAccountTypesContainer
    {
        public ICollection<BankAccountType> BankAccountTypes { get; set; }
        public BankAccountTypesContainer(ICollection<BankAccountType> types) {
            BankAccountTypes = types;
        }
    }
}