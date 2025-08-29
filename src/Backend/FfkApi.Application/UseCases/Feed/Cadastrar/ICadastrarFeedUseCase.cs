using FfkApi.Application.IUseCases;
using FfkApi.Communication.Requests;
using FfkApi.Communication.Responses;

namespace FfkApi.Application.UseCases.Feed.Cadastrar;

public interface ICadastrarFeedUseCase : ICadastrarUseCase<RequestCadastrarFeed, ResponseDadosFeed> { }
