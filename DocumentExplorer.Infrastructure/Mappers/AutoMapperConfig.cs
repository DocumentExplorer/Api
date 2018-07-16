using AutoMapper;

namespace DocumentExplorer.Infrastructure.Mappers
{
    public static class AutoMapperConfig
    {
        public static IMapper Initialize()
            => new MapperConfiguration(cfg =>
            {
                //cfg.CreateMap<User, UserDto>();
            })
            .CreateMapper();
    }
}