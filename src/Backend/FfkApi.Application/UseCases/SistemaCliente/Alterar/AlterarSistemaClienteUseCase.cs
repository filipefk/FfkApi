using AutoMapper;
using FfkApi.Application.Extension;
using FfkApi.Application.Validators;
using FfkApi.Communication.Requests;
using FfkApi.Domain.Repositories;
using FfkApi.Domain.Security.Criptografia;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;

namespace FfkApi.Application.UseCases.SistemaCliente.Alterar;

public class AlterarSistemaClienteUseCase : IAlterarSistemaClienteUseCase
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISistemaClienteRepository _sistemaClienteRepository;
    private readonly IEncriptadorSenha _encriptadorSenha;

    public AlterarSistemaClienteUseCase(
        ISistemaClienteRepository sistemaClienteRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IEncriptadorSenha encriptadorSenha)
    {
        _sistemaClienteRepository = sistemaClienteRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _encriptadorSenha = encriptadorSenha;
    }

    public async Task Execute(RequestAlterarSistemaCliente request, CancellationToken cancellationToken)
    {
        await Validar(request, cancellationToken);

        var sistemaCliente = await _sistemaClienteRepository.PegarSistemaClientePorId(Guid.Parse(request.Id!), cancellationToken);
        _mapper.Map(request, sistemaCliente);
        sistemaCliente!.Senha = _encriptadorSenha.Encriptar(sistemaCliente.Senha!);

        await _unitOfWork.CommitAsync(cancellationToken);
    }

    private async Task Validar(RequestAlterarSistemaCliente request, CancellationToken cancellationToken)
    {
        List<string> mensagensDeErro = [];
        mensagensDeErro.AddRange(await ValidarRequisicao(request, cancellationToken));
        mensagensDeErro.AddRange(await ValidarSistemaCliente(request, cancellationToken));

        if (mensagensDeErro.Count > 0)
        {
            throw new ErrorOnValidationException(mensagensDeErro.Distinct().ToList());
        }
    }

    private async Task<List<string>> ValidarRequisicao(RequestAlterarSistemaCliente request, CancellationToken cancellationToken)
    {
        var validator = new AlterarSistemaClienteValidator();

        var result = await validator.ValidateAsync(request, cancellationToken);

        if (!result.IsValid)
        {
            return result.ToListErros();
        }

        return [];
    }

    private async Task<List<string>> ValidarSistemaCliente(RequestAlterarSistemaCliente request, CancellationToken cancellationToken)
    {
        if (IdValidator.IdEstaValido(request.Id!))
        {
            var sistemaCliente = await _sistemaClienteRepository.PegarSistemaClientePorId(Guid.Parse(request.Id!), cancellationToken);
            if (sistemaCliente == null)
                return [ResourceMessagesException.SISTEMACLIENTE_NAO_ENCONTRADO];

            if (!string.IsNullOrWhiteSpace(request.AppId) && request.AppId != sistemaCliente.AppId.ToString() && await _sistemaClienteRepository.ExisteSistemaClienteComAppId(Guid.Parse(request.AppId), cancellationToken))
                return [ResourceMessagesException.APP_ID_JA_EXISTE];
        }

        return [];
    }
}
