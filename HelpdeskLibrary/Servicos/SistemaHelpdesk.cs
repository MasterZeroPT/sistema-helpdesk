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

/// <summary>
/// Classe principal que gerencia todo o sistema de helpdesk.
/// Responsável por coordenar clientes, operadores, produtos e assistências (tickets).
/// </summary>
public class SistemaHelpdesk
{
    #region Attributes
    /// <summary>
    /// Contador automático para gerar IDs únicos para novos tickets.
    /// Incrementado cada vez que uma nova assistência é criada.
    /// </summary>
    private int proximoIdTicket = 1;
    
    /// <summary>
    /// Dicionário que armazena todos os tickets do sistema.
    /// Chave: ID do ticket, Valor: Objeto Assistencia correspondente.
    /// </summary>
    public Dictionary<int, Assistencia> Tickets { get; } = new();
    
    /// <summary>
    /// Lista de todos os operadores registrados no sistema.
    /// </summary>
    public List<Operador> Operadores { get; } = new();
    
    /// <summary>
    /// Lista de todos os clientes registrados no sistema.
    /// </summary>
    public List<Cliente> Clientes { get; } = new();
    
    /// <summary>
    /// Lista de todos os produtos disponíveis para assistência.
    /// </summary>
    public List<Produto> Produtos { get; } = new();
    #endregion

    #region Constructors
    /// <summary>
    /// Construtor padrão que inicializa um novo sistema de helpdesk vazio.
    /// </summary>
    public SistemaHelpdesk() { }
    #endregion

    #region Methods - Gestão de Clientes

    /// <summary>
    /// Adiciona um novo cliente ao sistema.
    /// </summary>
    /// <param name="cliente">Cliente a ser adicionado</param>
    /// <returns>True se o cliente foi adicionado com sucesso</returns>
    /// <exception cref="ArgumentNullException">Lançada quando o cliente é null</exception>
    /// <exception cref="InvalidOperationException">Lançada quando já existe um cliente com o mesmo ID</exception>
    public bool AdicionarCliente(Cliente cliente)
    {
        // Valida se o cliente não é nulo
        ArgumentNullException.ThrowIfNull(cliente);

        // Verifica se já existe um cliente com o mesmo ID
        if (Clientes.Any(c => c.Id == cliente.Id))
            throw new InvalidOperationException($"Cliente com ID {cliente.Id} já existe.");

        // Adiciona o cliente à lista
        Clientes.Add(cliente);
        return true;
    }

    /// <summary>
    /// Procura um cliente pelo seu ID.
    /// </summary>
    /// <param name="id">ID do cliente a procurar</param>
    /// <returns>O cliente encontrado ou null se não existir</returns>
    public Cliente? BuscarClientePorId(int id)
    {
        return Clientes.Find(c => c.Id == id);
    }

    /// <summary>
    /// Remove um cliente do sistema.
    /// Não permite remoção se o cliente tiver tickets ativos (não fechados).
    /// </summary>
    /// <param name="id">ID do cliente a remover</param>
    /// <returns>True se o cliente foi removido com sucesso, False se não foi encontrado</returns>
    /// <exception cref="InvalidOperationException">Lançada quando o cliente possui tickets ativos</exception>
    public bool RemoverCliente(int id)
    {
        // Busca o cliente pelo ID
        var cliente = BuscarClientePorId(id);
        if (cliente == null)
            return false;

        // Verifica se o cliente tem tickets ativos (não fechados)
        bool temTicketsAtivos = Tickets.Values.Any(t =>
            t.Cliente.Id == id &&
            t.Estado != EstadoAssistencia.Fechado
        );

        // Impede a remoção se houver tickets ativos
        if (temTicketsAtivos)
        {
            throw new InvalidOperationException($"O cliente {cliente.Nome} possui tickets ativos e não pode ser removido.");
        }

        // Remove o cliente da lista
        Clientes.Remove(cliente);
        return true;
    }

    #endregion

    #region Methods - Gestão de Operadores

    /// <summary>
    /// Adiciona um novo operador ao sistema.
    /// </summary>
    /// <param name="operador">Operador a ser adicionado</param>
    /// <returns>True se o operador foi adicionado com sucesso</returns>
    /// <exception cref="ArgumentNullException">Lançada quando o operador é null</exception>
    /// <exception cref="InvalidOperationException">Lançada quando já existe um operador com o mesmo ID</exception>
    public bool AdicionarOperador(Operador operador)
    {
        // Valida se o operador não é nulo
        ArgumentNullException.ThrowIfNull(operador);

        // Verifica se já existe um operador com o mesmo ID
        if (Operadores.Any(o => o.Id == operador.Id))
            throw new InvalidOperationException($"Operador com ID {operador.Id} já existe.");

        // Adiciona o operador à lista
        Operadores.Add(operador);
        return true;
    }

    /// <summary>
    /// Procura um operador pelo seu ID.
    /// </summary>
    /// <param name="id">ID do operador a procurar</param>
    /// <returns>O operador encontrado ou null se não existir</returns>
    public Operador? BuscarOperadorPorId(int id)
    {
        return Operadores.Find(o => o.Id == id);
    }

    /// <summary>
    /// Promove um operador para uma categoria superior.
    /// </summary>
    /// <param name="operadorId">ID do operador a promover</param>
    /// <returns>True se a promoção foi bem-sucedida</returns>
    /// <exception cref="InvalidOperationException">Lançada quando o operador não é encontrado</exception>
    public bool PromoverOperador(int operadorId)
    {
        // Busca o operador
        var operador = BuscarOperadorPorId(operadorId);
        if (operador == null)
            throw new InvalidOperationException($"Operador com ID {operadorId} não encontrado.");

        // Delega a promoção para a classe CategoriaOperador
        return CategoriaOperador.Promover(operador);
    }

    /// <summary>
    /// Despromove um operador para uma categoria inferior.
    /// </summary>
    /// <param name="operadorId">ID do operador a despromover</param>
    /// <returns>True se a despromoção foi bem-sucedida</returns>
    /// <exception cref="InvalidOperationException">Lançada quando o operador não é encontrado</exception>
    public bool DespromoverOperador(int operadorId)
    {
        // Busca o operador
        var operador = BuscarOperadorPorId(operadorId);
        if (operador == null)
            throw new InvalidOperationException($"Operador com ID {operadorId} não encontrado.");

        // Delega a despromoção para a classe CategoriaOperador
        return CategoriaOperador.Despromover(operador);
    }

    /// <summary>
    /// Lista todos os operadores que estão disponíveis para atender assistências.
    /// </summary>
    /// <returns>Lista de operadores disponíveis</returns>
    public List<Operador> ListarOperadoresDisponiveis()
    {
        // Filtra operadores cuja propriedade Disponibilidade é true
        return Operadores.Where(o => o.Disponibilidade).ToList();
    }

    #endregion

    #region Methods - Gestão de Produtos

    /// <summary>
    /// Adiciona um novo produto ao sistema.
    /// </summary>
    /// <param name="produto">Produto a ser adicionado</param>
    /// <returns>True se o produto foi adicionado com sucesso</returns>
    /// <exception cref="ArgumentNullException">Lançada quando o produto é null</exception>
    /// <exception cref="InvalidOperationException">Lançada quando já existe um produto com o mesmo ID</exception>
    public bool AdicionarProduto(Produto produto)
    {
        // Valida se o produto não é nulo
        ArgumentNullException.ThrowIfNull(produto);

        // Verifica se já existe um produto com o mesmo ID
        if (Produtos.Any(p => p.Id == produto.Id))
            throw new InvalidOperationException($"Produto com ID {produto.Id} já existe.");

        // Adiciona o produto à lista
        Produtos.Add(produto);
        return true;
    }

    /// <summary>
    /// Procura um produto pelo seu ID.
    /// </summary>
    /// <param name="id">ID do produto a procurar</param>
    /// <returns>O produto encontrado ou null se não existir</returns>
    public Produto? BuscarProdutoPorId(int id)
    {
        return Produtos.Find(p => p.Id == id);
    }

    #endregion

    #region Methods - Gestão de Assistências

    /// <summary>
    /// Cria e abre uma nova assistência (ticket) no sistema.
    /// </summary>
    /// <param name="clienteId">ID do cliente que solicita a assistência</param>
    /// <param name="produtoId">ID do produto relacionado à assistência</param>
    /// <param name="tipo">Tipo de assistência (ex: Técnica, Comercial)</param>
    /// <param name="descricao">Descrição do problema ou solicitação</param>
    /// <returns>A assistência criada</returns>
    /// <exception cref="InvalidOperationException">Lançada quando o cliente ou produto não são encontrados</exception>
    public Assistencia AbrirNovaAssistencia(int clienteId, int produtoId, TipoAssistencia tipo, string descricao)
    {
        // Busca e valida o cliente
        var cliente = BuscarClientePorId(clienteId);
        if (cliente == null)
            throw new InvalidOperationException($"Cliente com ID {clienteId} não encontrado.");

        // Busca e valida o produto
        var produto = BuscarProdutoPorId(produtoId);
        if (produto == null)
            throw new InvalidOperationException($"Produto com ID {produtoId} não encontrado.");

        // Cria a nova assistência
        var novaAssistencia = new Assistencia(cliente, produto, descricao, tipo);
        
        // Gera um ID único para o ticket e adiciona ao dicionário
        int ticketId = proximoIdTicket++;
        Tickets.Add(ticketId, novaAssistencia);

        return novaAssistencia;
    }

    /// <summary>
    /// Atribui um operador a uma assistência existente.
    /// </summary>
    /// <param name="ticketId">ID do ticket</param>
    /// <param name="operadorId">ID do operador a atribuir</param>
    /// <returns>True se a atribuição foi bem-sucedida</returns>
    /// <exception cref="InvalidOperationException">Lançada quando o ticket ou operador não são encontrados, ou o operador não está disponível</exception>
    public bool AtribuirOperadorAAssistencia(int ticketId, int operadorId)
    {
        // Busca a assistência
        var assistencia = BuscarAssistenciaPorId(ticketId);
        if (assistencia == null)
            throw new InvalidOperationException($"Ticket {ticketId} não encontrado.");

        // Busca o operador
        var operador = BuscarOperadorPorId(operadorId);
        if (operador == null)
            throw new InvalidOperationException($"Operador com ID {operadorId} não encontrado.");

        // Verifica se o operador está disponível
        if (!operador.Disponibilidade)
            throw new InvalidOperationException($"Operador {operador.Nome} não está disponível.");

        // Atribui o operador à assistência
        return assistencia.AtribuirOperador(operador);
    }

    /// <summary>
    /// Marca uma assistência como resolvida, registrando a solução.
    /// </summary>
    /// <param name="ticketId">ID do ticket</param>
    /// <param name="resolucao">Descrição da resolução do problema</param>
    /// <returns>True se a resolução foi registrada com sucesso</returns>
    /// <exception cref="InvalidOperationException">Lançada quando o ticket não é encontrado</exception>
    public bool ResolverAssistencia(int ticketId, string resolucao)
    {
        // Busca a assistência
        var assistencia = BuscarAssistenciaPorId(ticketId);
        if (assistencia == null)
            throw new InvalidOperationException($"Ticket {ticketId} não encontrado.");

        // Delega a resolução para o objeto Assistencia
        return assistencia.ResolverAssistencia(resolucao);
    }

    /// <summary>
    /// Fecha uma assistência, finalizando o atendimento.
    /// </summary>
    /// <param name="ticketId">ID do ticket</param>
    /// <returns>True se a assistência foi fechada com sucesso</returns>
    /// <exception cref="InvalidOperationException">Lançada quando o ticket não é encontrado</exception>
    public bool FecharAssistencia(int ticketId)
    {
        // Busca a assistência
        var assistencia = BuscarAssistenciaPorId(ticketId);
        if (assistencia == null)
            throw new InvalidOperationException($"Ticket {ticketId} não encontrado.");

        // Delega o fechamento para o objeto Assistencia
        return assistencia.FecharAssistencia();
    }

    /// <summary>
    /// Registra uma avaliação (nota) para uma assistência.
    /// </summary>
    /// <param name="ticketId">ID do ticket</param>
    /// <param name="nota">Nota da avaliação</param>
    /// <returns>True se a avaliação foi registrada com sucesso</returns>
    /// <exception cref="InvalidOperationException">Lançada quando o ticket não é encontrado</exception>
    public bool AvaliarAssistencia(int ticketId, int nota)
    {
        // Busca a assistência
        var assistencia = BuscarAssistenciaPorId(ticketId);
        if (assistencia == null)
            throw new InvalidOperationException($"Ticket {ticketId} não encontrado.");

        // Registra a avaliação
        assistencia.RegistarAvaliacao(nota);
        return true;
    }

    /// <summary>
    /// Procura uma assistência pelo seu ID.
    /// </summary>
    /// <param name="id">ID do ticket</param>
    /// <returns>A assistência encontrada ou null se não existir</returns>
    public Assistencia? BuscarAssistenciaPorId(int id)
    {
        // Tenta obter o valor do dicionário, retorna null se não existir
        return Tickets.TryGetValue(id, out var assistencia) ? assistencia : null;
    }

    /// <summary>
    /// Lista todas as assistências que estão num determinado estado.
    /// </summary>
    /// <param name="estado">Estado das assistências a filtrar (ex: Aberto, EmAtendimento, Fechado)</param>
    /// <returns>Lista de assistências no estado especificado</returns>
    public List<Assistencia> ListarTicketsPorEstado(EstadoAssistencia estado)
    {
        // Filtra os tickets pelo estado
        return Tickets.Values.Where(t => t.Estado == estado).ToList();
    }

    /// <summary>
    /// Lista todas as assistências de um cliente específico.
    /// </summary>
    /// <param name="clienteId">ID do cliente</param>
    /// <returns>Lista de assistências do cliente</returns>
    public List<Assistencia> ListarTicketsPorCliente(int clienteId)
    {
        // Filtra os tickets pelo ID do cliente
        return Tickets.Values.Where(t => t.Cliente.Id == clienteId).ToList();
    }

    /// <summary>
    /// Lista todas as assistências atribuídas a um operador específico.
    /// </summary>
    /// <param name="operadorId">ID do operador</param>
    /// <returns>Lista de assistências do operador</returns>
    public List<Assistencia> ListarTicketsPorOperador(int operadorId)
    {
        // Filtra os tickets pelo ID do operador (pode ser null se não atribuído)
        return Tickets.Values.Where(t => t.OperadorAtual?.Id == operadorId).ToList();
    }

    #endregion

    #region Properties
    #endregion

    #region Overrides
    #endregion

    #region OtherMethods

    /// <summary>
    /// Guarda todos os dados do sistema num ficheiro JSON.
    /// Persiste clientes, operadores, produtos, tickets e o próximo ID de ticket.
    /// </summary>
    /// <param name="nomeFicheiro">Caminho do ficheiro onde guardar os dados</param>
    /// <returns>True se os dados foram guardados com sucesso</returns>
    /// <exception cref="ErroArquivoException">Lançada quando ocorre um erro ao escrever no ficheiro</exception>
    public bool GuardarDados(string nomeFicheiro)
    {
        try
        {
            // Cria um objeto com todos os dados do sistema para serialização
            var dadosCompletos = new DadosSistema
            {
                Tickets = this.Tickets,
                Operadores = this.Operadores,
                Clientes = this.Clientes,
                Produtos = this.Produtos,
                ProximoIdTicket = this.proximoIdTicket
            };

            // Configura opções de serialização (formato legível com indentação)
            var options = new JsonSerializerOptions 
            { 
                WriteIndented = true
            };

            // Serializa o objeto para JSON
            string jsonString = JsonSerializer.Serialize(dadosCompletos, options);
            
            // Escreve o JSON no ficheiro
            File.WriteAllText(nomeFicheiro, jsonString);
            
            return true;
        }
        catch (IOException e)
        {
            // Erro específico de I/O (permissões, disco cheio, etc.)
            throw new ErroArquivoException($"Falha ao guardar os dados no ficheiro no caminho {nomeFicheiro}", e);
        }
        catch (Exception e)
        {
            // Outros erros (serialização, etc.)
            throw new ErroArquivoException("Erro inesperado na serialização.", e);
        }
    }

    /// <summary>
    /// Carrega todos os dados do sistema a partir de um ficheiro JSON.
    /// Restaura clientes, operadores, produtos, tickets e o próximo ID de ticket.
    /// </summary>
    /// <param name="nomeFicheiro">Caminho do ficheiro de onde carregar os dados</param>
    /// <returns>True se os dados foram carregados com sucesso, False se o ficheiro não existir ou houver erro</returns>
    public bool CarregarDados(string nomeFicheiro)
    {
        // Verifica se o ficheiro existe
        if (!File.Exists(nomeFicheiro)) return false;

        try
        {
            // Lê o conteúdo JSON do ficheiro
            string jsonString = File.ReadAllText(nomeFicheiro);
            
            // Desserializa o JSON para o objeto DadosSistema
            var dadosCarregados = JsonSerializer.Deserialize<DadosSistema>(jsonString);

            // Valida se a desserialização foi bem-sucedida
            if (dadosCarregados == null) return false;

            // Limpa todos os dados atuais do sistema
            this.Tickets.Clear();
            this.Operadores.Clear();
            this.Clientes.Clear();
            this.Produtos.Clear();

            // Carrega os tickets do ficheiro para o dicionário
            foreach (var item in dadosCarregados.Tickets)
                this.Tickets.Add(item.Key, item.Value);

            // Carrega as listas de entidades
            this.Operadores.AddRange(dadosCarregados.Operadores);
            this.Clientes.AddRange(dadosCarregados.Clientes);
            this.Produtos.AddRange(dadosCarregados.Produtos);
            
            // Restaura o contador de IDs
            this.proximoIdTicket = dadosCarregados.ProximoIdTicket;

            return true;
        }
        catch (Exception)
        {
            // Retorna false em caso de qualquer erro (ficheiro corrompido, formato inválido, etc.)
            return false;
        }
    }

    #endregion

    #region Destructor
    #endregion

    #region Classe interna para serialização
    /// <summary>
    /// Classe auxiliar interna usada para serializar/desserializar todos os dados do sistema.
    /// Encapsula todas as coleções e contadores necessários para persistência.
    /// </summary>
    private class DadosSistema
    {
        /// <summary>Dicionário de tickets (ID -> Assistencia)</summary>
        public Dictionary<int, Assistencia> Tickets { get; set; } = new();
        
        /// <summary>Lista de operadores</summary>
        public List<Operador> Operadores { get; set; } = new();
        
        /// <summary>Lista de clientes</summary>
        public List<Cliente> Clientes { get; set; } = new();
        
        /// <summary>Lista de produtos</summary>
        public List<Produto> Produtos { get; set; } = new();
        
        /// <summary>Próximo ID disponível para novo ticket</summary>
        public int ProximoIdTicket { get; set; }
    }
    #endregion
}
