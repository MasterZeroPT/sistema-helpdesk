/**
 * @author Pedro Andrade
 * @email a31497@alunos.ipca.pt
 * @create date 2025-11 
 * @desc Classe que representa uma assistência técnica no sistema de helpdesk
 */
using Callcenter.HelpdeskLibrary.Exceptions;
using System;
using Callcenter.HelpdeskLibrary.Interfaces;
using Callcenter.HelpdeskLibrary.Enums;

namespace Callcenter.HelpdeskLibrary.Entidades;

/// <summary>
/// Representa uma assistência técnica registada no sistema.
/// Uma assistência passa pelos estados: Aberto -> EmProgresso -> Resolvido -> Fechado
/// Implementa IAvaliavel para permitir avaliação de satisfação do cliente.
/// </summary>
public class Assistencia: IAvaliavel
{
    #region Attributes
    // Campos privados para armazenar as horas (não serializados para JSON automaticamente)
    TimeOnly horaAbertura;  // Hora em que a assistência foi aberta
    TimeOnly horaFecho;     // Hora em que a assistência foi fechada
    #endregion

    #region Methods
    /// <summary>
    /// Atribui um operador à assistência e muda o estado para EmProgresso.
    /// Só funciona se a assistência estiver no estado Aberto.
    /// </summary>
    /// <param name="op">Operador que será responsável pela assistência</param>
    /// <returns>true se atribuído com sucesso, false se estado não permitir</returns>
    public bool AtribuirOperador(Operador op)
    {
        // Só permite atribuir operador se a assistência ainda não foi atribuída
        if (Estado == EstadoAssistencia.Aberto)
        {
            OperadorAtual = op;
            Estado = EstadoAssistencia.EmProgresso;  // Muda estado para indicar que está sendo trabalhada
            return true;
        }
        return false;  // Retorna false se já tiver operador atribuído
    }

    /// <summary>
    /// Marca a assistência como resolvida com uma descrição da solução.
    /// Só funciona se a assistência estiver no estado EmProgresso.
    /// </summary>
    /// <param name="resolucao">Descrição da solução aplicada ao problema</param>
    /// <returns>true se resolvida com sucesso, false se estado não permitir</returns>
    public bool ResolverAssistencia(string resolucao)
    {
        // Só permite resolver se estiver em progresso (com operador atribuído)
        if (Estado == EstadoAssistencia.EmProgresso)
        {
            DescricaoProblema = resolucao;  // Atualiza a descrição com a solução
            Estado = EstadoAssistencia.Resolvido;  // Muda estado para resolvido
            return true;
        }
        return false;  // Retorna false se não estiver em progresso
    }

    /// <summary>
    /// Fecha a assistência definitivamente, registando a hora de fecho.
    /// Só funciona se a assistência estiver no estado Resolvido.
    /// </summary>
    /// <returns>true se fechada com sucesso, false se estado não permitir</returns>
    public bool FecharAssistencia()
    {
        // Só permite fechar se já estiver resolvida
        if (Estado == EstadoAssistencia.Resolvido)
        {
            Estado = EstadoAssistencia.Fechado;  // Muda estado final
            horaFecho = TimeOnly.FromDateTime(DateTime.Now);  // Regista hora de encerramento
            return true;
        }
        return false;  // Retorna false se não estiver resolvida
    }
    #endregion

    #region Constructors
    /// <summary>
    /// Construtor que cria uma nova assistência no estado Aberto.
    /// Automaticamente regista a hora de abertura.
    /// </summary>
    /// <param name="cliente">Cliente que solicitou a assistência</param>
    /// <param name="produto">Produto relacionado com o problema</param>
    /// <param name="descricaoProblema">Descrição do problema reportado</param>
    /// <param name="tipo">Tipo de assistência (Técnica, Comercial, Suporte)</param>
    public Assistencia(Cliente cliente, Produto produto, string descricaoProblema, TipoAssistencia tipo)
    {
        Cliente = cliente;
        Produto = produto;
        DescricaoProblema = descricaoProblema;
        horaAbertura = TimeOnly.FromDateTime(DateTime.Now);  // Regista hora atual
        Estado = EstadoAssistencia.Aberto;  // Inicia sempre no estado Aberto
        Tipo = tipo;
    }
    #endregion

    #region Properties
    /// <summary>
    /// Cliente que abriu a assistência
    /// </summary>
    public Cliente Cliente { get; set; }

    /// <summary>
    /// Produto associado à assistência
    /// </summary>
    public Produto Produto { get; set; }

    /// <summary>
    /// Operador atualmente responsável pela assistência (pode ser null se não atribuído)
    /// </summary>
    public Operador? OperadorAtual { get; set; }

    /// <summary>
    /// Identificador único do ticket (apenas inicialização, não pode ser alterado depois)
    /// </summary>
    public int IdTicket { get; init; }

    /// <summary>
    /// Descrição do problema (atualizada para descrição da solução quando resolvida)
    /// </summary>
    public string DescricaoProblema { get; set; } = string.Empty;

    /// <summary>
    /// Hora em que a assistência foi aberta
    /// </summary>
    public TimeOnly HoraAbertura { get; set; }

    /// <summary>
    /// Hora em que a assistência foi fechada
    /// </summary>
    public TimeOnly HoraFecho { get; set; }

    /// <summary>
    /// Nota de satisfação do cliente (escala de 1 a 5)
    /// </summary>
    public int AvaliacaoSatisfacao { get; set; }

    /// <summary>
    /// Regista a avaliação de satisfação do cliente.
    /// Implementação do método da interface IAvaliavel.
    /// </summary>
    /// <param name="nota">Nota de 1 a 5</param>
    /// <exception cref="AssistenciaInvalidaException">Lançada se a nota estiver fora do intervalo 1-5</exception>
    public void RegistarAvaliacao(int nota)
    {
        // Validação: nota deve estar entre 1 e 5
        if (nota < 1 || nota > 5)
            throw new AssistenciaInvalidaException("A nota deve ser entre 1 e 5!");
        AvaliacaoSatisfacao = nota;
    }

    /// <summary>
    /// Estado atual da assistência (Aberto, EmProgresso, Resolvido, Fechado)
    /// </summary>
    public EstadoAssistencia Estado { get; set; }

    /// <summary>
    /// Tipo de assistência (Técnica, Comercial, Suporte)
    /// </summary>
    public TipoAssistencia Tipo { get; set; }
    #endregion

    #region Overrides
    /// <summary>
    /// Representação textual da assistência para exibição.
    /// Substitui o comportamento padrão de ToString() para mostrar informações úteis.
    /// </summary>
    /// <returns>String formatada com os dados principais da assistência</returns>
    public override string ToString()
    {
        return $"Assistência [ID: {IdTicket}, Cliente: {Cliente.Nome}, Produto: {Produto.Nome}, Estado: {Estado}, Tipo: {Tipo}]";
    }
    #endregion

    #region OtherMethods
    // Reservado para métodos auxiliares futuros
    #endregion

    #region Destructor
    /// <summary>
    /// Destrutor (finalizer) - chamado pelo garbage collector antes de liberar a memória.
    /// Geralmente não é necessário em C# moderno, mas mantido por estrutura de código.
    /// </summary>
    #endregion
}
