using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FfkApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Anexos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Nome = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Descricao = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    NomeArquivo = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    NomeArquivoArmazenamento = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Extensao = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    TamanhoBytes = table.Column<long>(type: "bigint", nullable: false, defaultValue: 0L),
                    MimeType = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Texto = table.Column<string>(type: "text", nullable: true),
                    DataCriacaoUtc = table.Column<DateTime>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Anexos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuditoriasSeguranca",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Evento = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Usuario = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    EnderecoIp = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    Caminho = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Metodo = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Detalhes = table.Column<string>(type: "text", nullable: true),
                    DataCriacaoUtc = table.Column<DateTime>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditoriasSeguranca", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Organizacoes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Nome = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Descricao = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    RemetenteEmail = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    RemetenteNome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ModeloEmailAtivacao = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ModeloEmailNovaSenha = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    DataCriacaoUtc = table.Column<DateTime>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organizacoes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PerfisAcesso",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Nome = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Descricao = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    DataCriacaoUtc = table.Column<DateTime>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerfisAcesso", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Permissoes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Nome = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Descricao = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    DataCriacaoUtc = table.Column<DateTime>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissoes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SistemasCliente",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    AppId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Nome = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Descricao = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Senha = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    DataCriacaoUtc = table.Column<DateTime>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SistemasCliente", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Checklists",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Nome = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Descricao = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    IdOrganizacao = table.Column<Guid>(type: "uuid", nullable: false),
                    DataCriacaoUtc = table.Column<DateTime>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Checklists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Checklists_Ref_Organizacoes",
                        column: x => x.IdOrganizacao,
                        principalTable: "Organizacoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Equipes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Nome = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Descricao = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    IdOrganizacao = table.Column<Guid>(type: "uuid", nullable: false),
                    DataCriacaoUtc = table.Column<DateTime>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Equipes_Ref_Organizacoes",
                        column: x => x.IdOrganizacao,
                        principalTable: "Organizacoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Feeds",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Nome = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Descricao = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    PalavrasChave = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    ExpiraEm = table.Column<DateOnly>(type: "date", nullable: true),
                    IdOrganizacao = table.Column<Guid>(type: "uuid", nullable: false),
                    DataCriacaoUtc = table.Column<DateTime>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feeds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Feeds_Ref_Organizacoes",
                        column: x => x.IdOrganizacao,
                        principalTable: "Organizacoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Pessoas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    TipoPessoa = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Nome = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Telefone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CpfCnpj = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: false),
                    IdOrganizacao = table.Column<Guid>(type: "uuid", nullable: false),
                    DataCriacaoUtc = table.Column<DateTime>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pessoa", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pessoas_Ref_Organizacoes",
                        column: x => x.IdOrganizacao,
                        principalTable: "Organizacoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Nome = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Senha = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Cpf = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: false),
                    Telefone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    IdOrganizacao = table.Column<Guid>(type: "uuid", nullable: false),
                    DataCriacaoUtc = table.Column<DateTime>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Usuarios_Ref_Organizacoes",
                        column: x => x.IdOrganizacao,
                        principalTable: "Organizacoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PerfisAcessoPermissoes",
                columns: table => new
                {
                    IdPerfilAcesso = table.Column<Guid>(type: "uuid", nullable: false),
                    IdPermissao = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerfisAcessoPermissoes", x => new { x.IdPerfilAcesso, x.IdPermissao });
                    table.ForeignKey(
                        name: "FK_PerfisAcessoPermissoes_Ref_PerfisAcesso",
                        column: x => x.IdPerfilAcesso,
                        principalTable: "PerfisAcesso",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PerfisAcessoPermissoes_Ref_Permissoes",
                        column: x => x.IdPermissao,
                        principalTable: "Permissoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChecklistItens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Ordem = table.Column<int>(type: "int", nullable: false),
                    Descricao = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    TipoChecklistItem = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    IdDependeDeChecklistItem = table.Column<Guid>(type: "uuid", nullable: true),
                    IdChecklist = table.Column<Guid>(type: "uuid", nullable: false),
                    DataCriacaoUtc = table.Column<DateTime>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChecklistItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChecklistItens_Ref_ChecklistItens",
                        column: x => x.IdDependeDeChecklistItem,
                        principalTable: "ChecklistItens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChecklistItens_Ref_Checklists",
                        column: x => x.IdChecklist,
                        principalTable: "Checklists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChecklistsPreenchidos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    NomeChecklist = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    DescricaoChecklist = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    IdChecklist = table.Column<Guid>(type: "uuid", nullable: false),
                    DataCriacaoUtc = table.Column<DateTime>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChecklistsPreenchidos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChecklistsPreenchidos_Ref_Checklists",
                        column: x => x.IdChecklist,
                        principalTable: "Checklists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "FeedsAnexos",
                columns: table => new
                {
                    IdFeed = table.Column<Guid>(type: "uuid", nullable: false),
                    IdAnexo = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeedsAnexos", x => new { x.IdFeed, x.IdAnexo });
                    table.ForeignKey(
                        name: "FK_FeedsAnexos_Ref_Anexos",
                        column: x => x.IdAnexo,
                        principalTable: "Anexos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FeedsAnexos_Ref_Feeds",
                        column: x => x.IdFeed,
                        principalTable: "Feeds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FeedsEquipes",
                columns: table => new
                {
                    IdFeed = table.Column<Guid>(type: "uuid", nullable: false),
                    IdEquipe = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeedsEquipes", x => new { x.IdFeed, x.IdEquipe });
                    table.ForeignKey(
                        name: "FK_FeedsEquipes_Ref_Equipes",
                        column: x => x.IdEquipe,
                        principalTable: "Equipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FeedsEquipes_Ref_Feeds",
                        column: x => x.IdFeed,
                        principalTable: "Feeds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FeedsUsuarios",
                columns: table => new
                {
                    IdFeed = table.Column<Guid>(type: "uuid", nullable: false),
                    IdUsuario = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeedsUsuarios", x => new { x.IdFeed, x.IdUsuario });
                    table.ForeignKey(
                        name: "FK_FeedsUsuarios_Ref_Feeds",
                        column: x => x.IdFeed,
                        principalTable: "Feeds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FeedsUsuarios_Ref_Usuarios",
                        column: x => x.IdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Filas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    IdEquipe = table.Column<Guid>(type: "uuid", nullable: true),
                    IdUsuario = table.Column<Guid>(type: "uuid", nullable: true),
                    DataCriacaoUtc = table.Column<DateTime>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Filas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Filas_Ref_Equipes",
                        column: x => x.IdEquipe,
                        principalTable: "Equipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Filas_Ref_Usuarios",
                        column: x => x.IdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Indisponibilidades",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Descricao = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    DataInicial = table.Column<DateOnly>(type: "date", nullable: false),
                    DataFinal = table.Column<DateOnly>(type: "date", nullable: false),
                    IdUsuario = table.Column<Guid>(type: "uuid", nullable: false),
                    DataCriacaoUtc = table.Column<DateTime>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Indisponibilidades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Indisponibilidades_Ref_Usuarios",
                        column: x => x.IdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MembrosEquipe",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Lider = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    IdEquipe = table.Column<Guid>(type: "uuid", nullable: false),
                    IdUsuario = table.Column<Guid>(type: "uuid", nullable: false),
                    DataCriacaoUtc = table.Column<DateTime>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MembrosEquipe", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MembrosEquipe_Ref_Equipes",
                        column: x => x.IdEquipe,
                        principalTable: "Equipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MembrosEquipe_Ref_Usuarios",
                        column: x => x.IdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Valor = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    IdUsuario = table.Column<Guid>(type: "uuid", nullable: false),
                    DataCriacaoUtc = table.Column<DateTime>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.UniqueConstraint("UK_RefreshTokens_IdUsuario", x => x.IdUsuario);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Ref_Usuarios",
                        column: x => x.IdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TokensAtivacao",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Valor = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    BaseExpiracaoUtc = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    IdUsuario = table.Column<Guid>(type: "uuid", nullable: false),
                    EmailEnviado = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    UltimaTentativaEnvioEmail = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    ErroEnvioEmail = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    DataCriacaoUtc = table.Column<DateTime>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TokensAtivacao", x => x.Id);
                    table.UniqueConstraint("UK_TokensAtivacao_IdUsuario", x => x.IdUsuario);
                    table.ForeignKey(
                        name: "FK_TokensAtivacao_Ref_Usuarios",
                        column: x => x.IdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TokensNovaSenha",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Valor = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    IdUsuario = table.Column<Guid>(type: "uuid", nullable: false),
                    EmailEnviado = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    UltimaTentativaEnvioEmail = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    ErroEnvioEmail = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    DataCriacaoUtc = table.Column<DateTime>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TokensNovaSenha", x => x.Id);
                    table.UniqueConstraint("UK_TokensNovaSenha_IdUsuario", x => x.IdUsuario);
                    table.ForeignKey(
                        name: "FK_TokensNovaSenha_Ref_Usuarios",
                        column: x => x.IdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsuariosPerfisAcesso",
                columns: table => new
                {
                    IdUsuario = table.Column<Guid>(type: "uuid", nullable: false),
                    IdPerfilAcesso = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuariosPerfisAcesso", x => new { x.IdUsuario, x.IdPerfilAcesso });
                    table.ForeignKey(
                        name: "FK_UsuariosPerfisAcesso_Ref_PerfisAcesso",
                        column: x => x.IdPerfilAcesso,
                        principalTable: "PerfisAcesso",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsuariosPerfisAcesso_Ref_Usuarios",
                        column: x => x.IdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsuariosPermissoes",
                columns: table => new
                {
                    IdUsuario = table.Column<Guid>(type: "uuid", nullable: false),
                    IdPermissao = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuariosPermissoes", x => new { x.IdUsuario, x.IdPermissao });
                    table.ForeignKey(
                        name: "FK_UsuariosPermissoes_Ref_Permissoes",
                        column: x => x.IdPermissao,
                        principalTable: "Permissoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsuariosPermissoes_Ref_Usuarios",
                        column: x => x.IdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChecklistRespostasPossiveis",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Ordem = table.Column<int>(type: "int", nullable: false),
                    Descricao = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    GeraInconformidade = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    IdChecklistItem = table.Column<Guid>(type: "uuid", nullable: false),
                    DataCriacaoUtc = table.Column<DateTime>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChecklistRespostaPossivel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChecklistRespostasPossiveis_Ref_ChecklistItens",
                        column: x => x.IdChecklistItem,
                        principalTable: "ChecklistItens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChecklistPreenchidoItens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    OrdemItem = table.Column<int>(type: "int", nullable: false),
                    DescricaoItem = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    DescricaoRespostaEscolhida = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    GeraInconformidade = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Observacao = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    IdChecklistPreenchido = table.Column<Guid>(type: "uuid", nullable: false),
                    DataCriacaoUtc = table.Column<DateTime>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChecklistPreenchidoItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChecklistPreenchidoItens_Ref_ChecklistPreenchido",
                        column: x => x.IdChecklistPreenchido,
                        principalTable: "ChecklistsPreenchidos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FilaItens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    IdFila = table.Column<Guid>(type: "uuid", nullable: false),
                    Posicao = table.Column<long>(type: "bigint", nullable: false),
                    DataCriacaoUtc = table.Column<DateTime>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilaItens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FilaItens_Ref_Filas",
                        column: x => x.IdFila,
                        principalTable: "Filas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChecklistGatilhosRespostasPossiveis",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    IdChecklistItem = table.Column<Guid>(type: "uuid", nullable: false),
                    IdChecklistRespostaPossivel = table.Column<Guid>(type: "uuid", nullable: false),
                    DataCriacaoUtc = table.Column<DateTime>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChecklistGatilhoRespostaPossivel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChecklistGatilhosRespostasPossiveis_Ref_ChecklistItens",
                        column: x => x.IdChecklistItem,
                        principalTable: "ChecklistItens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChecklistGatilhosRespostasPossiveis_Ref_ChecklistRespostasPossiveis",
                        column: x => x.IdChecklistRespostaPossivel,
                        principalTable: "ChecklistRespostasPossiveis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistGatilhosRespostasPossiveis_IdChecklistItem",
                table: "ChecklistGatilhosRespostasPossiveis",
                column: "IdChecklistItem");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistGatilhosRespostasPossiveis_IdChecklistRespostaPossivel",
                table: "ChecklistGatilhosRespostasPossiveis",
                column: "IdChecklistRespostaPossivel");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistItens_IdChecklist",
                table: "ChecklistItens",
                column: "IdChecklist");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistItens_IdDependeDeChecklistItem",
                table: "ChecklistItens",
                column: "IdDependeDeChecklistItem");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistPreenchidoItens_IdChecklistPreenchido",
                table: "ChecklistPreenchidoItens",
                column: "IdChecklistPreenchido");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistRespostasPossiveis_IdChecklistItem",
                table: "ChecklistRespostasPossiveis",
                column: "IdChecklistItem");

            migrationBuilder.CreateIndex(
                name: "IX_Checklists_IdOrganizacao",
                table: "Checklists",
                column: "IdOrganizacao");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistsPreenchidos_IdChecklist",
                table: "ChecklistsPreenchidos",
                column: "IdChecklist");

            migrationBuilder.CreateIndex(
                name: "IX_Equipes_IdOrganizacao",
                table: "Equipes",
                column: "IdOrganizacao");

            migrationBuilder.CreateIndex(
                name: "UK_Equipes_Nome_IdOrganizacao",
                table: "Equipes",
                columns: new[] { "Nome", "IdOrganizacao" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Feeds_IdOrganizacao",
                table: "Feeds",
                column: "IdOrganizacao");

            migrationBuilder.CreateIndex(
                name: "IX_FeedsAnexos_IdAnexo",
                table: "FeedsAnexos",
                column: "IdAnexo");

            migrationBuilder.CreateIndex(
                name: "IX_FeedsEquipes_IdEquipe",
                table: "FeedsEquipes",
                column: "IdEquipe");

            migrationBuilder.CreateIndex(
                name: "IX_FeedsUsuarios_IdUsuario",
                table: "FeedsUsuarios",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_FilaItens_IdFila",
                table: "FilaItens",
                column: "IdFila");

            migrationBuilder.CreateIndex(
                name: "IX_Filas_IdEquipe",
                table: "Filas",
                column: "IdEquipe",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Filas_IdUsuario",
                table: "Filas",
                column: "IdUsuario",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Indisponibilidades_IdUsuario",
                table: "Indisponibilidades",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_MembrosEquipe_IdEquipe",
                table: "MembrosEquipe",
                column: "IdEquipe");

            migrationBuilder.CreateIndex(
                name: "IX_MembrosEquipe_IdUsuario",
                table: "MembrosEquipe",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "UK_MembrosEquipe_IdEquipe_IdUsuario",
                table: "MembrosEquipe",
                columns: new[] { "IdEquipe", "IdUsuario" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UK_Organizacoes_Nome",
                table: "Organizacoes",
                column: "Nome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UK_PerfisAcesso_Nome",
                table: "PerfisAcesso",
                column: "Nome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PerfisAcessoPermissoes_IdPermissao",
                table: "PerfisAcessoPermissoes",
                column: "IdPermissao");

            migrationBuilder.CreateIndex(
                name: "UK_Permissoes_Nome",
                table: "Permissoes",
                column: "Nome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pessoas_IdOrganizacao",
                table: "Pessoas",
                column: "IdOrganizacao");

            migrationBuilder.CreateIndex(
                name: "UK_SistemasCliente_AppId",
                table: "SistemasCliente",
                column: "AppId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_IdOrganizacao",
                table: "Usuarios",
                column: "IdOrganizacao");

            migrationBuilder.CreateIndex(
                name: "UK_Usuarios_Email",
                table: "Usuarios",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsuariosPerfisAcesso_IdPerfilAcesso",
                table: "UsuariosPerfisAcesso",
                column: "IdPerfilAcesso");

            migrationBuilder.CreateIndex(
                name: "IX_UsuariosPermissoes_IdPermissao",
                table: "UsuariosPermissoes",
                column: "IdPermissao");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditoriasSeguranca");

            migrationBuilder.DropTable(
                name: "ChecklistGatilhosRespostasPossiveis");

            migrationBuilder.DropTable(
                name: "ChecklistPreenchidoItens");

            migrationBuilder.DropTable(
                name: "FeedsAnexos");

            migrationBuilder.DropTable(
                name: "FeedsEquipes");

            migrationBuilder.DropTable(
                name: "FeedsUsuarios");

            migrationBuilder.DropTable(
                name: "FilaItens");

            migrationBuilder.DropTable(
                name: "Indisponibilidades");

            migrationBuilder.DropTable(
                name: "MembrosEquipe");

            migrationBuilder.DropTable(
                name: "PerfisAcessoPermissoes");

            migrationBuilder.DropTable(
                name: "Pessoas");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "SistemasCliente");

            migrationBuilder.DropTable(
                name: "TokensAtivacao");

            migrationBuilder.DropTable(
                name: "TokensNovaSenha");

            migrationBuilder.DropTable(
                name: "UsuariosPerfisAcesso");

            migrationBuilder.DropTable(
                name: "UsuariosPermissoes");

            migrationBuilder.DropTable(
                name: "ChecklistRespostasPossiveis");

            migrationBuilder.DropTable(
                name: "ChecklistsPreenchidos");

            migrationBuilder.DropTable(
                name: "Anexos");

            migrationBuilder.DropTable(
                name: "Feeds");

            migrationBuilder.DropTable(
                name: "Filas");

            migrationBuilder.DropTable(
                name: "PerfisAcesso");

            migrationBuilder.DropTable(
                name: "Permissoes");

            migrationBuilder.DropTable(
                name: "ChecklistItens");

            migrationBuilder.DropTable(
                name: "Equipes");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Checklists");

            migrationBuilder.DropTable(
                name: "Organizacoes");
        }
    }
}
