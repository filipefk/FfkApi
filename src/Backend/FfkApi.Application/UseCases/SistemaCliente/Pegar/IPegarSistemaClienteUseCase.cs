using FfkApi.Application.IUseCases;
using FfkApi.Communication.Requests;
using FfkApi.Communication.Responses;

namespace FfkApi.Application.UseCases.SistemaCliente.Pegar;

public interface IPegarSistemaClienteUseCase : IPegarUseCase<RequestPegarSistemaCliente, ResponseDadosSistemaCliente> { }
