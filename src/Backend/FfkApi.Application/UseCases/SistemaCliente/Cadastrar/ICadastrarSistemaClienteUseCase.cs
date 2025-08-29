using FfkApi.Application.IUseCases;
using FfkApi.Communication.Requests;
using FfkApi.Communication.Responses;

namespace FfkApi.Application.UseCases.SistemaCliente.Cadastrar;

public interface ICadastrarSistemaClienteUseCase : ICadastrarUseCase<RequestCadastrarSistemaCliente, ResponseDadosSistemaCliente> { }
