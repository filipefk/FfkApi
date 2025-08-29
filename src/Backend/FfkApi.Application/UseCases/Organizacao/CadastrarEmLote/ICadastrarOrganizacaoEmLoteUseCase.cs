using FfkApi.Application.IUseCases;
using FfkApi.Communication.Requests;
using FfkApi.Communication.Responses;

namespace FfkApi.Application.UseCases.Organizacao.CadastrarEmLote;

public interface ICadastrarOrganizacaoEmLoteUseCase : ICadastrarEmLoteUseCase<RequestCadastrarOrganizacao, RequestCadastrarOrganizacaoEmLote, ResponseDadosOrganizacao> { }
