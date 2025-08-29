using AutoMapper;
using FfkApi.Application.Extension;
using FfkApi.Communication.Requests;
using FfkApi.Communication.Responses;
using FfkApi.Domain.Repositories;
using FfkApi.Domain.Security.Criptografia;
using FfkApi.Exceptions.ExceptionsBase;

namespace FfkApi.Application.UseCases.SistemaCliente.Cadastrar;

public class CadastrarSistemaClienteUseCase : ICadastrarSistemaClienteUseCase
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISistemaClienteRepository _sistemaClienteRepository;
    private readonly IEncriptadorSenha _encriptadorSenha;

    public CadastrarSistemaClienteUseCase(
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

    public async Task<ResponseDadosSistemaCliente> Execute(RequestCadastrarSistemaCliente request, CancellationToken cancellationToken)
    {
        await Validar(request, cancellationToken);

        var sistemaCliente = _mapper.Map<Domain.Entities.SistemaCliente>(request);
        sistemaCliente.Senha = _encriptadorSenha.Encriptar(sistemaCliente.Senha!);

        await _sistemaClienteRepository.Adicionar(sistemaCliente, cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);

        return _mapper.Map<ResponseDadosSistemaCliente>(sistemaCliente);

    }

    private async Task Validar(RequestCadastrarSistemaCliente request, CancellationToken cancellationToken)
    {
        List<string> mensagensDeErro = [];
        mensagensDeErro.AddRange(await ValidarRequisicao(request, cancellationToken));

        if (mensagensDeErro.Count > 0)
        {
            throw new ErrorOnValidationException(mensagensDeErro.Distinct().ToList());
        }
    }

    private async Task<List<string>> ValidarRequisicao(RequestCadastrarSistemaCliente request, CancellationToken cancellationToken)
    {
        var validator = new CadastrarSistemaClienteValidator();

        var result = await validator.ValidateAsync(request, cancellationToken);

        if (!result.IsValid)
        {
            return result.ToListErros();
        }

        return [];
    }
}
