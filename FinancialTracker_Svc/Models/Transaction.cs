﻿using System;

namespace FinancialTracker_Svc.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public int ParentAccountId { get; set; }
        public int TransactionTypeId { get; set; }
        public int? CategoryItemId { get; set; }
        public string OwnerId { get; set; }
        public string Name { get; set; }
        public string Memo { get; set; }
        public decimal Amount { get; set; }
        public DateTime OccuredAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}