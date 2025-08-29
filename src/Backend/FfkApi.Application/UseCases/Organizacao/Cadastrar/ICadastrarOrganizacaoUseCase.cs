using FfkApi.Application.IUseCases;
using FfkApi.Communication.Requests;
using FfkApi.Communication.Responses;

namespace FfkApi.Application.UseCases.Organizacao.Cadastrar;

public interface ICadastrarOrganizacaoUseCase : ICadastrarUseCase<RequestCadastrarOrganizacao, ResponseDadosOrganizacao> { }
