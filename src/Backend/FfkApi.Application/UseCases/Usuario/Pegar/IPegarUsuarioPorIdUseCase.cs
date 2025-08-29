using FfkApi.Application.IUseCases;
using FfkApi.Communication.Requests;
using FfkApi.Communication.Responses;

namespace FfkApi.Application.UseCases.Usuario.Pegar;

public interface IPegarUsuarioPorIdUseCase : IPegarUseCase<RequestPegarUsuario, ResponseDadosUsuario> { }
