/**
 * @author Pedro Andrade
 * @email a31497@alunos.ipca.pt
 * @create date 2025-11 
 * @desc Classe que representa um operador no sistema de helpdesk
 */
using Callcenter.HelpdeskLibrary.Enums;
using Callcenter.HelpdeskLibrary.Servicos;
using System;

namespace Callcenter.HelpdeskLibrary.Entidades
{
    /// <summary>
    /// Representa um operador de helpdesk no sistema.
    /// Herda de Pessoa e adiciona funcionalidades específicas:
    /// - Classificação por nível de acesso (automaticamente baseado em especialidade)
    /// - Controlo de disponibilidade (online/offline)
    /// - Histórico de tickets atendidos
    /// Usa construtor primário (C# 12+) com parâmetros default.
    /// </summary>
    public class Operador(
        int id,
        string nome,
        string email,
        string telefone,
        string nomeUtilizador,
        string especialidade,
        bool disponibilidade,
        NivelAcesso? nivel = null  // Parâmetro opcional - se null, classifica automaticamente
    ) : Pessoa(id, nome, email, telefone)  // Chama construtor da classe base
    {
        #region Properties
        /// <summary>
        /// Username usado pelo operador para login no sistema.
        /// </summary>
        public string NomeUtilizador { get; set; } = nomeUtilizador;

        /// <summary>
        /// Área de especialização do operador (ex: "Técnico", "Suporte", "Gestão").
        /// Usado para classificação automática do nível de acesso.
        /// </summary>
        public string Especialidade { get; set; } = especialidade;

        /// <summary>
        /// Indica se o operador está disponível para receber assistências.
        /// [field: NonSerialized] = não é salvo no ficheiro (estado temporário de sessão).
        /// Ao carregar dados, todos os operadores começam indisponíveis.
        /// </summary>
        [field: NonSerialized]
        public bool Disponibilidade { get; set; } = disponibilidade;

        /// <summary>
        /// Nível de acesso do operador (Nivel1, Nivel2, Nivel3).
        /// Se não fornecido explicitamente, usa CategoriaOperador.Classificar()
        /// Operador ?? = null-coalescing operator (usa esquerda se não-null, senão direita)
        /// </summary>
        public NivelAcesso Nivel { get; set; } = nivel ?? CategoriaOperador.Classificar(especialidade);

        /// <summary>
        /// Lista de IDs dos tickets que este operador já atendeu.
        /// Usado para histórico e estatísticas de desempenho.
        /// </summary>
        public List<int> HistoricoTickets { get; set; } = new();
        #endregion

        #region OtherMethods
        /// <summary>
        /// Alterna o estado de disponibilidade do operador (online/offline).
        /// Usa operador ! (negação lógica) para inverter o valor booleano.
        /// </summary>
        /// <returns>Novo estado de disponibilidade após a alteração</returns>
        public bool AlterarDisponibilidade()
        {
            Disponibilidade = !Disponibilidade;  // Inverte true <-> false
            return Disponibilidade;
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Representação textual do operador com informações específicas.
        /// Sobrescreve ToString() da classe base Pessoa.
        /// Usa operador ternário (condição ? true : false) para formatar disponibilidade.
        /// </summary>
        /// <returns>String formatada com dados do operador</returns>
        public override string ToString()
        {
            return $"Operador: {Nome} | Especialidade: {Especialidade} | Disponibilidade: {(Disponibilidade ? "Disponível" : "Indisponível")} | Nível de Acesso: {Nivel}";
        }
        #endregion
    }
}
