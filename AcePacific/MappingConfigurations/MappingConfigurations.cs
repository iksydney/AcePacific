using AcePacific.Common.Contract;
using AcePacific.Data.Entities;
using AcePacific.Data.ViewModel;
using AutoMapper;
using static AcePacific.Data.ViewModel.BankFilter;

namespace AcePacific.API.MappingConfigurations
{
    public class MappingConfigurations : Profile
    {
        public MappingConfigurations()
        {
            CreateMap<User, RegisterUserModel>().ReverseMap();
            CreateMap<User, CustomerItem>().ReverseMap();
            CreateMap<User, CustomerModel>().ReverseMap();
            CreateMap<CreatWalletViewModel, Wallet>().ReverseMap();
            CreateMap<IntraTransferDto, Wallet>().ReverseMap();
            CreateMap<UpdatePinModel, Wallet>().ReverseMap();
            CreateMap<GetWalletResponse, Wallet>().ReverseMap();
            CreateMap<TransactionLog, TransactionLogItem>().ReverseMap();
            CreateMap<Wallet, WalletItem>().ReverseMap();
            CreateMap<Bank, BankModel>().ReverseMap();
            CreateMap<Bank, CreateBank>().ReverseMap();
            
            
            CreateMap<UpdateUserView, User>().ReverseMap();
            /*CreateMap<Response<string>, User>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Result))
                .ForMember(dest => dest.ProfilePicture, opt => opt.Ignore());*/

        }
    }
}
