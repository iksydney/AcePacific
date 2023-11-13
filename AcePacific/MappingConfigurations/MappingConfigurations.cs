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
            CreateMap<CreatWalletViewModel, Wallet>().ReverseMap();
            CreateMap<IntraTransferDto, Wallet>().ReverseMap();
            CreateMap<UpdatePinModel, Wallet>().ReverseMap();
            CreateMap<GetWalletResponse, Wallet>().ReverseMap();
            CreateMap<TransactionLog, TransactionLogItem>().ReverseMap();
        }
    }
}
