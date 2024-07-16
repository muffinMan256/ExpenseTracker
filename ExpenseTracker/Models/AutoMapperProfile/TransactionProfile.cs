using AutoMapper;
using ExpenseTracker.Data;

namespace ExpenseTracker.Models.AutoMapperProfile
{
    public class TransactionProfile: Profile
    {
        public TransactionProfile()
        {
            CreateMap<Transaction, TransactionModel>();
            CreateMap<TransactionModel, Transaction>();
        }
    }


}
