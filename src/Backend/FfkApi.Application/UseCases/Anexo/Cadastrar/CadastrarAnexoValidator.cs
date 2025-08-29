using FfkApi.Communication.Requests;
using FfkApi.Domain.Configurations;
using FfkApi.Exceptions;
using FluentValidation;

namespace FfkApi.Application.UseCases.Anexo.Cadastrar;

public class CadastrarAnexoValidator : AbstractValidator<RequestCadastrarAnexo>
{
    public CadastrarAnexoValidator()
    {
        RuleFor(request => request.Nome).NotEmpty().WithMessage(ResourceMessagesException.NOME_VAZIO);
        RuleFor(request => request.Descricao).NotEmpty().WithMessage(ResourceMessagesException.DESCRICAO_VAZIA);
        RuleFor(request => request.Arquivo).NotNull().WithMessage(ResourceMessagesException.ARQUIVO_VAZIO);
        When(request => request.Arquivo is not null, () =>
        {
            RuleFor(request => request.Arquivo)
                .Must(arquivo => arquivo!.Length > 0).WithMessage(ResourceMessagesException.ARQUIVO_VAZIO)
                .Must(arquivo => arquivo!.Length <= ConfiguracaoArquivoAnexo.TamanhoMaximoBytes)
                    .WithMessage(ResourceMessagesException.ARQUIVO_MUITO_GRANDE.Replace("{tamanho-maximo}", ConfiguracaoArquivoAnexo.TamanhoMaximoBytesTexto));
        });
    }
}
