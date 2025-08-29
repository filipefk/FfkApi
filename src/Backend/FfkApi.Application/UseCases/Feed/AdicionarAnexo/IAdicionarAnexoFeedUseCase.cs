using FfkApi.Application.IUseCases;
using FfkApi.Communication.Requests;
using FfkApi.Communication.Responses;

namespace FfkApi.Application.UseCases.Feed.AdicionarAnexo;

public interface IAdicionarAnexoFeedUseCase : ICadastrarUseCase<RequestAdicionarAnexoFeed, ResponseDadosAnexo> { }
