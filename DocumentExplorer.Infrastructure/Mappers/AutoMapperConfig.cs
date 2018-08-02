using AutoMapper;
using DocumentExplorer.Core.Domain;
using DocumentExplorer.Infrastructure.DTO;

namespace DocumentExplorer.Infrastructure.Mappers
{
    public static class AutoMapperConfig
    {
        public static IMapper Initialize()
            => new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<User, UserDto>();
                cfg.CreateMap<Order, OrderDto>();
                cfg.ReplaceMemberName("CreationDateString", "Date");
                cfg.CreateMap<File,FileDto>();
                cfg.CreateMap<Order, ExtendedOrderDto>();
                cfg.CreateMap<Permissions,PermissionsDto>();
                cfg.CreateMap<Log,LogDto>();
            })
            .CreateMapper();
    }
}