using FfkApi.API.Filters;
using Microsoft.AspNetCore.Mvc;

namespace FfkApi.API.Attributes;

public class UsuarioAdministradorAttribute : TypeFilterAttribute
{
    public UsuarioAdministradorAttribute() : base(typeof(UsuarioAdministradorFilter))
    {
    }
}
