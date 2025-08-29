using FfkApi.Application.IUseCases;
using FfkApi.Communication.Requests;
using FfkApi.Communication.Responses;

namespace FfkApi.Application.UseCases.Indisponibilidade.Pegar;

public interface IPegarIndisponibilidadeUseCase : IPegarUseCase<RequestPegarIndisponibilidade, ResponseDadosIndisponibilidade> { }
