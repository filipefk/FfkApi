using FfkApi.Application.IUseCases;
using FfkApi.Communication.Requests;
using FfkApi.Communication.Responses;

namespace FfkApi.Application.UseCases.Usuario.Cadastrar;

public interface ICadastrarUsuarioUseCase : ICadastrarUseCase<RequestCadastrarUsuario, ResponseDadosUsuario> { }