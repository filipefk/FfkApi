using FfkApi.Application.IUseCases;
using FfkApi.Communication.Requests;
using FfkApi.Communication.Responses;

namespace FfkApi.Application.UseCases.Organizacao.Pegar;

public interface IPegarOrganizacaoUseCase : IPegarUseCase<RequestPegarOrganizacao, ResponseDadosOrganizacao> { }
