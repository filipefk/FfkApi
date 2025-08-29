using FfkApi.Application.IUseCases;
using FfkApi.Communication.Requests;
using FfkApi.Communication.Responses;

namespace FfkApi.Application.UseCases.Feed.CadastrarEmLote;

public interface ICadastrarFeedEmLoteUseCase : ICadastrarEmLoteUseCase<RequestCadastrarFeed, RequestCadastrarFeedEmLote, ResponseDadosFeed> { }
