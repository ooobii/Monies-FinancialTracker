﻿using System.Collections.Generic;

namespace FinancialTracker_Svc.Models
{
    public class TransactionType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsIncome { get; set; }
    }

    public class TransactionTypesContainer
    {
        public ICollection<TransactionType> TransactionTypes { get; set; }

        public TransactionTypesContainer(ICollection<TransactionType> types) {
            TransactionTypes = types;
        }
    }
}