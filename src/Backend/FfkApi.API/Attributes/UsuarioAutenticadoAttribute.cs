using FfkApi.API.Filters;
using Microsoft.AspNetCore.Mvc;

namespace FfkApi.API.Attributes;

public class UsuarioAutenticadoAttribute : TypeFilterAttribute
{
    public UsuarioAutenticadoAttribute() : base(typeof(UsuarioAutenticadoFilter))
    {
    }

    public string? Permissao
    {
        get => Arguments?.Length > 0 ? Arguments[0] as string : null;
        set => Arguments = [value!];
    }
}
