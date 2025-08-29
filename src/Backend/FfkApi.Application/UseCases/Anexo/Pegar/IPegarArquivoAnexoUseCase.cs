using FfkApi.Application.IUseCases;
using FfkApi.Communication.Requests;
using FfkApi.Communication.Responses;

namespace FfkApi.Application.UseCases.Anexo.Pegar;

public interface IPegarArquivoAnexoUseCase : IPegarUseCase<RequestPegarArquivoAnexo, ResponseArquivoAnexo> { }
