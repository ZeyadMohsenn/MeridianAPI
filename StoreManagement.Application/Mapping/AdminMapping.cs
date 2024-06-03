//using StoreManagement.Bases;
//using StoreManagement.Domain.Entities;
//using StoreManagement.Domain.Login_Token;

//namespace StoreManagement.Application.Mapping
//{
//    public class AdminMapping : MappingProfileBase
//    {
//        public AdminMapping()
//        {
//            CreateMap<RegisterDto, Admin>()
//                  .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.PhoneNumber))
//                  .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
//                  .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));
//        }
//    }
//}
