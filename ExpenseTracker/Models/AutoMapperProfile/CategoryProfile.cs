using AutoMapper;
using ExpenseTracker.Data;

namespace ExpenseTracker.Models.AutoMapperProfile
{
    public class CategoryProfile: Profile
    {
        public CategoryProfile()
        {
            CreateMap<CategoryModel, CategoryModel>();
            CreateMap<CategoryModel, CategoryModel>();
        }
    }


}
