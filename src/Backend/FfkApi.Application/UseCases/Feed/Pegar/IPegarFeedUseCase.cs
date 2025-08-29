using FfkApi.Application.IUseCases;
using FfkApi.Communication.Requests;
using FfkApi.Communication.Responses;

namespace FfkApi.Application.UseCases.Feed.Pegar;

public interface IPegarFeedUseCase : IPegarUseCase<RequestPegarFeed, ResponseDadosFeed> { }
