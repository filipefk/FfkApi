using AutoMapper;
using FfkApi.Application.Services.AutoMapper;

namespace TestUtil.AutoMapper;

public class MapperBuilder
{
    public static IMapper Build()
    {
        return new MapperConfiguration(options =>
        {
            options.AddProfile(new AutoMapping());
        }).CreateMapper();
    }
}
