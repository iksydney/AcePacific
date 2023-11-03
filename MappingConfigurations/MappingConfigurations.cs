using AcePacific.Data.Entities;
using AcePacific.Data.ViewModel;
using AutoMapper;

namespace AcePacific.API.MappingConfigurations
{
    public class MappingConfigurations : Profile
    {
        public MappingConfigurations()
        {
            CreateMap<User, RegisterUserModel>().ReverseMap();
        }
    }
}
