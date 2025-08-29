using FfkApi.Application.IUseCases;
using FfkApi.Communication.Requests;
using FfkApi.Communication.Responses;

namespace FfkApi.Application.UseCases.Equipe.Cadastrar;

public interface ICadastrarEquipeUseCase : ICadastrarUseCase<RequestCadastrarEquipe, ResponseDadosEquipe> { }
