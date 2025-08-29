using AutoMapper;
using FfkApi.Application.Services.Anexo;
using FfkApi.Communication.Requests;
using FfkApi.Communication.Responses;
using FfkApi.Domain.Repositories;
using FfkApi.Exceptions.ExceptionsBase;

namespace FfkApi.Application.UseCases.Anexo.Cadastrar;

public class CadastrarAnexoUseCase : ICadastrarAnexoUseCase
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IArmazenadorDeAnexoService _armazenadorDeAnexoService;

    public CadastrarAnexoUseCase(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IArmazenadorDeAnexoService armazenadorDeAnexoService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _armazenadorDeAnexoService = armazenadorDeAnexoService;
    }

    public async Task<ResponseDadosAnexo> Execute(RequestCadastrarAnexo request, CancellationToken cancellationToken)
    {
        await Validar(request, cancellationToken);

        var anexo = await _armazenadorDeAnexoService.SalvarAsync(request, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return _mapper.Map<ResponseDadosAnexo>(anexo);
    }

    private static async Task Validar(RequestCadastrarAnexo request, CancellationToken cancellationToken)
    {
        var validator = new CadastrarAnexoValidator();

        var result = await validator.ValidateAsync(request, cancellationToken);

        if (!result.IsValid)
        {
            throw new ErrorOnValidationException(result.Errors.Select(e => e.ErrorMessage).Distinct().ToList());
        }
    }
}
