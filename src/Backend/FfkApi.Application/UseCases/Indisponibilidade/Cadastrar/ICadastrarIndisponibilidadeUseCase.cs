using FfkApi.Application.IUseCases;
using FfkApi.Communication.Requests;
using FfkApi.Communication.Responses;

namespace FfkApi.Application.UseCases.Indisponibilidade.Cadastrar;

public interface ICadastrarIndisponibilidadeUseCase : ICadastrarUseCase<RequestCadastrarIndisponibilidade, ResponseDadosIndisponibilidade> { }
