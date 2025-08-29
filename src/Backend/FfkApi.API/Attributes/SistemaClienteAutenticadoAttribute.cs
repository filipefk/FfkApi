using FfkApi.API.Filters;
using Microsoft.AspNetCore.Mvc;

namespace FfkApi.API.Attributes;

public class SistemaClienteAutenticadoAttribute : TypeFilterAttribute
{
    public SistemaClienteAutenticadoAttribute() : base(typeof(SistemaClienteAutenticadoFilter))
    {
    }
}
