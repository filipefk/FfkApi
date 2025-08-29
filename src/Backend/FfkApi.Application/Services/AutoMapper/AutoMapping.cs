using AutoMapper;
using FfkApi.Communication.Requests;
using FfkApi.Communication.Responses;
using FfkApi.Domain.Entities;

namespace FfkApi.Application.Services.AutoMapper;

public class AutoMapping : Profile
{
    public AutoMapping()
    {
        RequestToDomain();
        DomainToResponse();
    }

    private void RequestToDomain()
    {
        CreateMap<RequestCadastrarUsuario, Usuario>()
            .ForMember(dest => dest.Organizacao, opt => opt.Ignore())
            .ForMember(dest => dest.PerfisAcesso, opt => opt.Ignore())
            .ForMember(dest => dest.Permissoes, opt => opt.Ignore());

        CreateMap<RequestAlterarUsuario, Usuario>()
            .ForMember(dest => dest.Organizacao, opt => opt.Ignore())
            .ForMember(dest => dest.PerfisAcesso, opt => opt.Ignore())
            .ForMember(dest => dest.Permissoes, opt => opt.Ignore());

        CreateMap<RequestCadastrarOrganizacao, Organizacao>();
        CreateMap<RequestAlterarOrganizacao, Organizacao>();

        CreateMap<RequestAlterarSistemaCliente, SistemaCliente>();
        CreateMap<RequestCadastrarSistemaCliente, SistemaCliente>();

        CreateMap<RequestAlterarAnexo, Domain.Entities.Anexo>();
        CreateMap<RequestCadastrarAnexo, Domain.Entities.Anexo>();

        CreateMap<RequestCadastrarFeed, Feed>()
            .ForMember(dest => dest.Organizacao, opt => opt.Ignore())
            .ForMember(dest => dest.VisibilidadeUsuarios, opt => opt.Ignore())
            .ForMember(dest => dest.VisibilidadeEquipes, opt => opt.Ignore())
            .ForMember(dest => dest.ExpiraEm, opt => opt.Ignore());

        CreateMap<RequestAlterarFeed, Feed>()
            .ForMember(dest => dest.Organizacao, opt => opt.Ignore())
            .ForMember(dest => dest.VisibilidadeUsuarios, opt => opt.Ignore())
            .ForMember(dest => dest.VisibilidadeEquipes, opt => opt.Ignore())
            .ForMember(dest => dest.ExpiraEm, opt => opt.Ignore());

        CreateMap<RequestAlterarEquipe, Equipe>()
            .ForMember(dest => dest.Organizacao, opt => opt.Ignore())
            .ForMember(dest => dest.Membros, opt => opt.Ignore());

        CreateMap<RequestCadastrarEquipe, Equipe>()
            .ForMember(dest => dest.Organizacao, opt => opt.Ignore())
            .ForMember(dest => dest.Membros, opt => opt.Ignore());

        CreateMap<RequestAlterarIndisponibilidade, Indisponibilidade>()
            .ForMember(dest => dest.DataInicial, opt => opt.Ignore())
            .ForMember(dest => dest.DataFinal, opt => opt.Ignore())
            .ForMember(dest => dest.Usuario, opt => opt.Ignore());

        CreateMap<RequestCadastrarIndisponibilidade, Indisponibilidade>()
            .ForMember(dest => dest.DataInicial, opt => opt.Ignore())
            .ForMember(dest => dest.DataFinal, opt => opt.Ignore())
            .ForMember(dest => dest.Usuario, opt => opt.Ignore());
    }

    private void DomainToResponse()
    {
        CreateMap<Usuario, ResponseDadosUsuario>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.Organizacao, opt => opt.MapFrom(src => src.Organizacao.Nome))
            .ForMember(dest => dest.PerfisAcesso, opt => opt.MapFrom(src => src.PerfisAcesso.Select(perfilAcesso => perfilAcesso.Nome)))
            .ForMember(dest => dest.Permissoes, opt => opt.MapFrom(src => src.Permissoes.Select(permissao => permissao.Nome)));

        CreateMap<Usuario, ResponseDadosBasicosUsuario>()
            .ForMember(dest => dest.Organizacao, opt => opt.MapFrom(src => src.Organizacao.Nome));

        CreateMap<Checklist, ResponseChecklist>();
        CreateMap<ChecklistItem, ResponseChecklistItem>()
            .ForMember(dest => dest.DependeDeChecklistItem, opt => opt.MapFrom(src => src.DependeDeChecklistItem!.Descricao))
            .ForMember(dest => dest.GatilhosDeResposta, opt => opt.MapFrom(src => src.GatilhosDeResposta!.Select(g => g.RespostaGatilho.Descricao)));
        CreateMap<ChecklistRespostaPossivel, ResponseChecklistRespostaPossivel>();

        CreateMap<ChecklistPreenchido, ResponseChecklistPreenchido>();
        CreateMap<ChecklistPreenchidoItem, ResponseChecklistPreenchidoItem>();

        CreateMap<Organizacao, ResponseDadosOrganizacao>();

        CreateMap<SistemaCliente, ResponseDadosSistemaCliente>();

        CreateMap<Domain.Entities.Anexo, ResponseDadosAnexo>();

        CreateMap<Feed, ResponseDadosFeed>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.Organizacao, opt => opt.MapFrom(src => src.Organizacao.Nome))
            .ForMember(dest => dest.VisibilidadeUsuarios, opt => opt.MapFrom(src => src.VisibilidadeUsuarios.Select(usuario => usuario.Email)))
            .ForMember(dest => dest.VisibilidadeEquipes, opt => opt.MapFrom(src => src.VisibilidadeEquipes.Select(equipe => equipe.Nome)))
            .ForMember(dest => dest.ExpiraEm, opt => opt.MapFrom(src => src.ExpiraEm != null ? src.ExpiraEm.Value.ToString("dd/MM/yyyy") : null));

        CreateMap<Equipe, ResponseDadosEquipe>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.Organizacao, opt => opt.MapFrom(src => src.Organizacao.Nome));

        CreateMap<MembroEquipe, ResponseMembroEquipe>()
            .ForMember(dest => dest.IdUsuario, opt => opt.MapFrom(src => src.Usuario.Id))
            .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.Usuario.Nome))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Usuario.Email));

        CreateMap<Indisponibilidade, ResponseDadosIndisponibilidade>()
            .ForMember(dest => dest.DataInicial, opt => opt.MapFrom(src => src.DataInicial.ToString("dd/MM/yyyy")))
            .ForMember(dest => dest.DataFinal, opt => opt.MapFrom(src => src.DataFinal.ToString("dd/MM/yyyy")));
    }
}
