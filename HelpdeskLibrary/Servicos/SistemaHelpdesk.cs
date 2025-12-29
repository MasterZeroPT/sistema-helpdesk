/**
 * @author Pedro Andrade
 * @email a31497@alunos.ipca.pt
 * @create date 2025-11 
 * @desc Classe que representa o sistema de helpdesk
 */

using Callcenter.HelpdeskLibrary.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using Callcenter.HelpdeskLibrary.Entidades;
using Callcenter.HelpdeskLibrary.Enums;
using System.Linq;
using System.Text.Json;

namespace Callcenter.HelpdeskLibrary.Servicos;

public class SistemaHelpdesk
{
    #region Attributes
    private int proximoIdTicket = 1;
    public Dictionary<int, Assistencia> Tickets { get; } = new();
    public List<Operador> Operadores { get; } = new();
    public List<Cliente> Clientes { get; } = new();
    public List<Produto> Produtos { get; } = new();
    #endregion

    #region Constructors
    public SistemaHelpdesk() { }
    #endregion

    #region Methods - Gestão de Clientes

    public bool AdicionarCliente(Cliente cliente)
    {
        ArgumentNullException.ThrowIfNull(cliente);

        if (Clientes.Any(c => c.Id == cliente.Id))
            throw new InvalidOperationException($"Cliente com ID {cliente.Id} já existe.");

        Clientes.Add(cliente);
        return true;
    }

    public Cliente? BuscarClientePorId(int id)
    {
        return Clientes.Find(c => c.Id == id);
    }

    public bool RemoverCliente(int id)
    {
        var cliente = BuscarClientePorId(id);
        if (cliente == null)
            return false;

        bool temTicketsAtivos = Tickets.Values.Any(t =>
            t.Cliente.Id == id &&
            t.Estado != EstadoAssistencia.Fechado
        );

        if (temTicketsAtivos)
        {
            throw new InvalidOperationException($"O cliente {cliente.Nome} possui tickets ativos e não pode ser removido.");
        }

        Clientes.Remove(cliente);
        return true;
    }

    #endregion

    #region Methods - Gestão de Operadores

    public bool AdicionarOperador(Operador operador)
    {
        ArgumentNullException.ThrowIfNull(operador);

        if (Operadores.Any(o => o.Id == operador.Id))
            throw new InvalidOperationException($"Operador com ID {operador.Id} já existe.");

        Operadores.Add(operador);
        return true;
    }

    public Operador? BuscarOperadorPorId(int id)
    {
        return Operadores.Find(o => o.Id == id);
    }

    public bool PromoverOperador(int operadorId)
    {
        var operador = BuscarOperadorPorId(operadorId);
        if (operador == null)
            throw new InvalidOperationException($"Operador com ID {operadorId} não encontrado.");

        return CategoriaOperador.Promover(operador);
    }

    public bool DespromoverOperador(int operadorId)
    {
        var operador = BuscarOperadorPorId(operadorId);
        if (operador == null)
            throw new InvalidOperationException($"Operador com ID {operadorId} não encontrado.");

        return CategoriaOperador.Despromover(operador);
    }

    public List<Operador> ListarOperadoresDisponiveis()
    {
        return Operadores.Where(o => o.Disponibilidade).ToList();
    }

    #endregion

    #region Methods - Gestão de Produtos

    public bool AdicionarProduto(Produto produto)
    {
        ArgumentNullException.ThrowIfNull(produto);

        if (Produtos.Any(p => p.Id == produto.Id))
            throw new InvalidOperationException($"Produto com ID {produto.Id} já existe.");

        Produtos.Add(produto);
        return true;
    }

    public Produto? BuscarProdutoPorId(int id)
    {
        return Produtos.Find(p => p.Id == id);
    }

    #endregion

    #region Methods - Gestão de Assistências

    public Assistencia AbrirNovaAssistencia(int clienteId, int produtoId, TipoAssistencia tipo, string descricao)
    {
        var cliente = BuscarClientePorId(clienteId);
        if (cliente == null)
            throw new InvalidOperationException($"Cliente com ID {clienteId} não encontrado.");

        var produto = BuscarProdutoPorId(produtoId);
        if (produto == null)
            throw new InvalidOperationException($"Produto com ID {produtoId} não encontrado.");

        var novaAssistencia = new Assistencia(cliente, produto, descricao, tipo);
        int ticketId = proximoIdTicket++;
        Tickets.Add(ticketId, novaAssistencia);

        return novaAssistencia;
    }

    public bool AtribuirOperadorAAssistencia(int ticketId, int operadorId)
    {
        var assistencia = BuscarAssistenciaPorId(ticketId);
        if (assistencia == null)
            throw new InvalidOperationException($"Ticket {ticketId} não encontrado.");

        var operador = BuscarOperadorPorId(operadorId);
        if (operador == null)
            throw new InvalidOperationException($"Operador com ID {operadorId} não encontrado.");

        if (!operador.Disponibilidade)
            throw new InvalidOperationException($"Operador {operador.Nome} não está disponível.");

        return assistencia.AtribuirOperador(operador);
    }

    public bool ResolverAssistencia(int ticketId, string resolucao)
    {
        var assistencia = BuscarAssistenciaPorId(ticketId);
        if (assistencia == null)
            throw new InvalidOperationException($"Ticket {ticketId} não encontrado.");

        return assistencia.ResolverAssistencia(resolucao);
    }

    public bool FecharAssistencia(int ticketId)
    {
        var assistencia = BuscarAssistenciaPorId(ticketId);
        if (assistencia == null)
            throw new InvalidOperationException($"Ticket {ticketId} não encontrado.");

        return assistencia.FecharAssistencia();
    }

    public bool AvaliarAssistencia(int ticketId, int nota)
    {
        var assistencia = BuscarAssistenciaPorId(ticketId);
        if (assistencia == null)
            throw new InvalidOperationException($"Ticket {ticketId} não encontrado.");

        assistencia.RegistarAvaliacao(nota);
        return true;
    }

    public Assistencia? BuscarAssistenciaPorId(int id)
    {
        return Tickets.TryGetValue(id, out var assistencia) ? assistencia : null;
    }

    public List<Assistencia> ListarTicketsPorEstado(EstadoAssistencia estado)
    {
        return Tickets.Values.Where(t => t.Estado == estado).ToList();
    }

    public List<Assistencia> ListarTicketsPorCliente(int clienteId)
    {
        return Tickets.Values.Where(t => t.Cliente.Id == clienteId).ToList();
    }

    public List<Assistencia> ListarTicketsPorOperador(int operadorId)
    {
        return Tickets.Values.Where(t => t.OperadorAtual?.Id == operadorId).ToList();
    }

    #endregion

    #region Properties
    #endregion

    #region Overrides
    #endregion

    #region OtherMethods

    public bool GuardarDados(string nomeFicheiro)
    {
        try
        {
            // Serializa todos os dados do sistema
            var dadosCompletos = new DadosSistema
            {
                Tickets = this.Tickets,
                Operadores = this.Operadores,
                Clientes = this.Clientes,
                Produtos = this.Produtos,
                ProximoIdTicket = this.proximoIdTicket
            };

            var options = new JsonSerializerOptions 
            { 
                WriteIndented = true  // Formato legível (opcional)
            };

            string jsonString = JsonSerializer.Serialize(dadosCompletos, options);
            File.WriteAllText(nomeFicheiro, jsonString);
            
            return true;
        }
        catch (IOException e)
        {
            throw new ErroArquivoException($"Falha ao guardar os dados no ficheiro no caminho {nomeFicheiro}", e);
        }
        catch (Exception e)
        {
            throw new ErroArquivoException("Erro inesperado na serialização.", e);
        }
    }

    public bool CarregarDados(string nomeFicheiro)
    {
        if (!File.Exists(nomeFicheiro)) return false;

        try
        {
            string jsonString = File.ReadAllText(nomeFicheiro);
            var dadosCarregados = JsonSerializer.Deserialize<DadosSistema>(jsonString);

            if (dadosCarregados == null) return false;

            // Limpa os dados atuais
            this.Tickets.Clear();
            this.Operadores.Clear();
            this.Clientes.Clear();
            this.Produtos.Clear();

            // Carrega os dados
            foreach (var item in dadosCarregados.Tickets)
                this.Tickets.Add(item.Key, item.Value);

            this.Operadores.AddRange(dadosCarregados.Operadores);
            this.Clientes.AddRange(dadosCarregados.Clientes);
            this.Produtos.AddRange(dadosCarregados.Produtos);
            this.proximoIdTicket = dadosCarregados.ProximoIdTicket;

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    #endregion

    #region Destructor
    #endregion

    #region Classe interna para serialização
    private class DadosSistema
    {
        public Dictionary<int, Assistencia> Tickets { get; set; } = new();
        public List<Operador> Operadores { get; set; } = new();
        public List<Cliente> Clientes { get; set; } = new();
        public List<Produto> Produtos { get; set; } = new();
        public int ProximoIdTicket { get; set; }
    }
    #endregion
}
