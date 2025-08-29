using FfkApi.Application.IUseCases;
using FfkApi.Communication.Requests;
using FfkApi.Communication.Responses;

namespace FfkApi.Application.UseCases.Anexo.Cadastrar;

public interface ICadastrarAnexoUseCase : ICadastrarUseCase<RequestCadastrarAnexo, ResponseDadosAnexo> { }
