using FfkApi.Application.IUseCases;
using FfkApi.Communication.Requests;
using FfkApi.Communication.Responses;

namespace FfkApi.Application.UseCases.Equipe.Pegar;

public interface IPegarEquipeUseCase : IPegarUseCase<RequestPegarEquipe, ResponseDadosEquipe> { }
