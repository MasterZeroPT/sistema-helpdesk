/**
 * @author Pedro Andrade
 * @email a31497@alunos.ipca.pt
 * @create date 2025-11 
 * @desc Interface de console para o sistema de helpdesk
 */

using Callcenter.HelpdeskLibrary.Entidades;
using Callcenter.HelpdeskLibrary.Enums;
using Callcenter.HelpdeskLibrary.Servicos;
using System;

namespace Callcenter.UI;

/// <summary>
/// Classe responsável pela interface de usuário em modo console.
/// Oferece três modos de operação:
/// 1. Modo Interativo (menu com opções)
/// 2. Modo Comando Contínuo (loop de comandos CLI)
/// 3. Modo Comando Único (executa um comando e fecha)
/// </summary>
public class ConsoleUI
{
    private readonly SistemaHelpdesk sistema;  // Instância do sistema de helpdesk
    private const string CaminhoFicheiro = "helpdesk_data.json";  // Arquivo de persistência
    
    /// <summary>
    /// KILL SWITCH: Controla o modo de operação da aplicação
    /// true  = Modo Interativo (menu visual com opções 0/1/2)
    /// false = Modo Comando Contínuo (prompt > para comandos CLI)
    /// </summary>
    private const bool ModoInterativoAtivo = true;

    /// <summary>
    /// Construtor que recebe a instância do sistema de helpdesk via injeção de dependência.
    /// </summary>
    /// <param name="sistema">Sistema de helpdesk a ser gerenciado pela UI</param>
    public ConsoleUI(SistemaHelpdesk sistema)
    {
        this.sistema = sistema;
    }

    /// <summary>
    /// Ponto de entrada principal da interface de console.
    /// Carrega dados do ficheiro, executa o modo apropriado e salva os dados ao final.
    /// </summary>
    /// <param name="args">Argumentos da linha de comando (se houver)</param>
    public void Executar(string[] args)
    {
        // Carrega dados persistidos do ficheiro JSON
        sistema.CarregarDados(CaminhoFicheiro);

        // Decisão de qual modo executar baseado nos argumentos e na kill switch
        if (args.Length == 0 && ModoInterativoAtivo)
        {
            // Sem argumentos + modo interativo = Menu visual
            ExecutarModoInterativo();
        }
        else if (args.Length == 0 && !ModoInterativoAtivo)
        {
            // Sem argumentos + modo não interativo = Loop de comandos CLI
            ExecutarModoComandoContinuo();
        }
        else
        {
            // Com argumentos = Executa comando único e fecha
            ProcessarComandoLinha(args);
        }

        // Persiste dados no ficheiro JSON ao encerrar
        sistema.GuardarDados(CaminhoFicheiro);
    }

    /// <summary>
    /// Modo Interativo: Apresenta um menu visual com opções numeradas.
    /// Permite executar demonstração ou digitar comandos CLI manualmente.
    /// Loop continua até o usuário escolher "0" (Sair).
    /// </summary>
    private void ExecutarModoInterativo()
    {
        Console.WriteLine("=== Sistema de Helpdesk ===\n");

        // Garante que existem dados de teste para demonstração
        if (sistema.Clientes.Count == 0)
        {
            AdicionarDadosTeste();
        }

        bool continuar = true;

        while (continuar)
        {
            // Exibe menu de opções
            Console.WriteLine("\n1. Executar demonstração");
            Console.WriteLine("2. Executar comando");
            Console.WriteLine("0. Sair");
            Console.Write("\nOpção: ");
            
            string? opcao = Console.ReadLine();

            if (opcao == "0")
            {
                // Encerra o loop e salva dados
                continuar = false;
                Console.WriteLine("\nEncerrando...");
            }
            else if (opcao == "1")
            {
                // Executa fluxo completo de demonstração
                ExecutarDemonstracao();
            }
            else if (opcao == "2")
            {
                // Permite digitar comando CLI dentro do modo interativo
                Console.Write("\nDigite o comando (ex: -buscar 1): ");
                string? comandoInput = Console.ReadLine();
                
                if (!string.IsNullOrWhiteSpace(comandoInput))
                {
                    // Divide o comando em palavras (argumentos)
                    string[] args = comandoInput.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    ProcessarComandoLinha(args);
                }
            }
            else
            {
                Console.WriteLine("\nOpção inválida!");
            }
        }
    }

    /// <summary>
    /// Modo Comando Contínuo: Apresenta um prompt ">" para entrada de comandos CLI.
    /// Loop continua até o usuário digitar "sair" ou "exit".
    /// Ideal para testar múltiplos comandos sequencialmente.
    /// </summary>
    private void ExecutarModoComandoContinuo()
    {
        Console.WriteLine("=== Sistema de Helpdesk - Modo Comando ===");
        Console.WriteLine("Digite comandos (ex: -buscar 1) ou 'sair' para encerrar\n");

        // Garante que existem dados de teste
        if (sistema.Clientes.Count == 0)
        {
            AdicionarDadosTeste();
        }

        bool continuar = true;

        while (continuar)
        {
            Console.Write("\n> ");
            string? comandoInput = Console.ReadLine();

            // Ignora entradas vazias
            if (string.IsNullOrWhiteSpace(comandoInput))
                continue;

            // Verifica se é comando de saída
            if (comandoInput.ToLower() == "sair" || comandoInput.ToLower() == "exit")
            {
                continuar = false;
                Console.WriteLine("\nEncerrando...");
            }
            else
            {
                // Divide o comando em argumentos e processa
                string[] args = comandoInput.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                ProcessarComandoLinha(args);
            }
        }
    }

    /// <summary>
    /// Executa uma demonstração completa do fluxo de uma assistência:
    /// 1. Abre assistência
    /// 2. Atribui operador
    /// 3. Resolve problema
    /// 4. Fecha assistência
    /// 5. Regista avaliação
    /// Útil para testar o sistema rapidamente.
    /// </summary>
    private void ExecutarDemonstracao()
    {
        try
        {
            Console.WriteLine("\n--- Executando demonstração ---\n");

            // Garante dados de teste
            if (sistema.Clientes.Count == 0)
            {
                AdicionarDadosTeste();
            }

            // 1. Abrir assistência (Cliente 1, Produto 1, tipo Técnica)
            var assistencia = sistema.AbrirNovaAssistencia(1, 1, TipoAssistencia.Tecnica, "Problema com o equipamento");
            Console.WriteLine("✓ Assistência aberta");

            // 2. Atribuir operador 1 ao ticket 1
            sistema.AtribuirOperadorAAssistencia(1, 1);
            Console.WriteLine("✓ Operador atribuído");

            // 3. Resolver ticket 1 com descrição da solução
            sistema.ResolverAssistencia(1, "Substituído componente defeituoso");
            Console.WriteLine("✓ Assistência resolvida");

            // 4. Fechar ticket 1
            sistema.FecharAssistencia(1);
            Console.WriteLine("✓ Assistência fechada");

            // 5. Avaliar ticket 1 com nota 5
            sistema.AvaliarAssistencia(1, 5);
            Console.WriteLine("✓ Avaliação registada");

            Console.WriteLine("\n✓ Demonstração concluída!");
        }
        catch (Exception ex)
        {
            // Captura e exibe qualquer erro durante a demonstração
            Console.WriteLine($"\n Erro: {ex.Message}");
        }
    }

    /// <summary>
    /// Adiciona dados de teste ao sistema:
    /// - 1 Cliente (João Silva)
    /// - 1 Produto (Smartphone XYZ)
    /// - 1 Operador (Pedro Andrade)
    /// Usado para garantir que a demonstração funcione sem setup manual.
    /// </summary>
    private void AdicionarDadosTeste()
    {
        var cliente = new Cliente(1, "João Silva", "joao@email.com", "+351912345678");
        sistema.AdicionarCliente(cliente);

        var produto = new Produto(1, "Smartphone XYZ", "2");
        sistema.AdicionarProduto(produto);

        var operador = new Operador(1, "Pedro Andrade", "pedro@empresa.com", 
                                      "+351913456789", "MasterZero", "Técnico", true);
        sistema.AdicionarOperador(operador);

        Console.WriteLine("✓ Dados de teste carregados\n");
    }

    /// <summary>
    /// Processa comandos da linha de comando (CLI).
    /// Suporta comandos como: -demo, -abrir, -buscar, -listar, etc.
    /// Cada comando tem validação de argumentos e tratamento de erros.
    /// </summary>
    /// <param name="args">Array de argumentos (args[0] = comando, args[1..n] = parâmetros)</param>
    private void ProcessarComandoLinha(string[] args)
    {
        try
        {
            // Comando principal está sempre no primeiro argumento (minúsculas)
            string comando = args[0].ToLower();

            switch (comando)
            {
                case "-demo":
                    // Executa demonstração completa
                    ExecutarDemonstracao();
                    break;

                case "-abrir":
                    // Abre nova assistência
                    // Sintaxe: -abrir <clienteId> <produtoId> <tipo> <descricao>
                    if (args.Length < 5)
                    {
                        Console.WriteLine("Uso: -abrir <clienteId> <produtoId> <tipo> <descricao>");
                        break;
                    }
                    int clienteId = int.Parse(args[1]);
                    int produtoId = int.Parse(args[2]);
                    TipoAssistencia tipo = Enum.Parse<TipoAssistencia>(args[3], true);
                    string descricao = args[4];
                    
                    var assistencia = sistema.AbrirNovaAssistencia(clienteId, produtoId, tipo, descricao);
                    Console.WriteLine($"✓ Assistência aberta! ID gerado será visível no próximo ticket.");
                    break;

                case "-atribuir":
                    // Atribui operador a um ticket
                    // Sintaxe: -atribuir <ticketId> <operadorId>
                    if (args.Length < 3)
                    {
                        Console.WriteLine("Uso: -atribuir <ticketId> <operadorId>");
                        break;
                    }
                    int ticketId = int.Parse(args[1]);
                    int operadorId = int.Parse(args[2]);
                    
                    if (sistema.AtribuirOperadorAAssistencia(ticketId, operadorId))
                        Console.WriteLine("✓ Operador atribuído!");
                    break;

                case "-resolver":
                    // Resolve uma assistência com descrição da solução
                    // Sintaxe: -resolver <ticketId> <resolucao>
                    if (args.Length < 3)
                    {
                        Console.WriteLine("Uso: -resolver <ticketId> <resolucao>");
                        break;
                    }
                    ticketId = int.Parse(args[1]);
                    string resolucao = args[2];
                    
                    if (sistema.ResolverAssistencia(ticketId, resolucao))
                        Console.WriteLine("✓ Assistência resolvida!");
                    break;

                case "-fechar":
                    // Fecha uma assistência
                    // Sintaxe: -fechar <ticketId>
                    if (args.Length < 2)
                    {
                        Console.WriteLine("Uso: -fechar <ticketId>");
                        break;
                    }
                    ticketId = int.Parse(args[1]);
                    
                    if (sistema.FecharAssistencia(ticketId))
                        Console.WriteLine("✓ Assistência fechada!");
                    break;

                case "-avaliar":
                    // Regista avaliação de satisfação
                    // Sintaxe: -avaliar <ticketId> <nota>
                    if (args.Length < 3)
                    {
                        Console.WriteLine("Uso: -avaliar <ticketId> <nota>");
                        break;
                    }
                    ticketId = int.Parse(args[1]);
                    int nota = int.Parse(args[2]);
                    
                    if (sistema.AvaliarAssistencia(ticketId, nota))
                        Console.WriteLine("✓ Avaliação registada!");
                    break;

                case "-buscar":
                    // Busca um ticket pelo ID
                    // Sintaxe: -buscar <ticketId>
                    if (args.Length < 2)
                    {
                        Console.WriteLine("Uso: -buscar <ticketId>");
                        break;
                    }
                    ticketId = int.Parse(args[1]);
                    var ticket = sistema.BuscarAssistenciaPorId(ticketId);
                    
                    if (ticket != null)
                        Console.WriteLine(ticket.ToString());  // Usa o ToString() da classe Assistencia
                    else
                        Console.WriteLine($"Ticket {ticketId} não encontrado.");
                    break;

                case "-listar":
                    // Lista todos os tickets de um determinado estado
                    // Sintaxe: -listar <estado>
                    // Estados válidos: Aberto, EmProgresso, Resolvido, Fechado
                    if (args.Length < 2)
                    {
                        Console.WriteLine("Uso: -listar <estado>");
                        Console.WriteLine("Estados: Aberto, EmProgresso, Resolvido, Fechado");
                        break;
                    }
                    EstadoAssistencia estado = Enum.Parse<EstadoAssistencia>(args[1], true);
                    var tickets = sistema.ListarTicketsPorEstado(estado);
                    
                    Console.WriteLine($"\n=== Tickets ({estado}) ===");
                    if (tickets.Count == 0)
                        Console.WriteLine("Nenhum ticket encontrado.");
                    else
                        tickets.ForEach(t => Console.WriteLine(t.ToString()));
                    break;

                case "-operadores":
                    // Lista todos os operadores cadastrados no sistema
                    // Sintaxe: -operadores
                    Console.WriteLine("\n=== Operadores ===");
                    if (sistema.Operadores.Count == 0)
                        Console.WriteLine("Nenhum operador cadastrado.");
                    else
                        sistema.Operadores.ForEach(o => Console.WriteLine(o.ToString()));
                    break;

                case "-promover":
                    // Promove um operador para o próximo nível de acesso
                    // Sintaxe: -promover <operadorId>
                    if (args.Length < 2)
                    {
                        Console.WriteLine("Uso: -promover <operadorId>");
                        break;
                    }
                    operadorId = int.Parse(args[1]);
                    
                    if (sistema.PromoverOperador(operadorId))
                    {
                        var op = sistema.BuscarOperadorPorId(operadorId);
                        Console.WriteLine($"✓ Operador promovido para {op?.Nivel}!");
                    }
                    break;

                case "-despromover":
                    // Despromove um operador para o nível de acesso anterior
                    // Sintaxe: -despromover <operadorId>
                    if (args.Length < 2)
                    {
                        Console.WriteLine("Uso: -despromover <operadorId>");
                        break;
                    }
                    operadorId = int.Parse(args[1]);
                    
                    if (sistema.DespromoverOperador(operadorId))
                    {
                        var op = sistema.BuscarOperadorPorId(operadorId);
                        Console.WriteLine($"✓ Operador despromovido para {op?.Nivel}.");
                    }
                    break;

                case "-ajuda":
                case "-h":
                    // Exibe menu de ajuda com todos os comandos disponíveis
                    MostrarAjuda();
                    break;

                default:
                    // Comando não reconhecido
                    Console.WriteLine($"Comando desconhecido: {comando}");
                    Console.WriteLine("Use -ajuda para ver os comandos disponíveis.");
                    break;
            }
        }
        catch (Exception ex)
        {
            // Captura qualquer erro durante o processamento (parsing, validação, etc.)
            Console.WriteLine($"Erro: {ex.Message}");
        }
    }

    /// <summary>
    /// Exibe o menu de ajuda com todos os comandos CLI disponíveis e sua sintaxe.
    /// </summary>
    private void MostrarAjuda()
    {
        Console.WriteLine("\n=== Comandos Disponíveis ===\n");
        Console.WriteLine("Assistências:");
        Console.WriteLine("  -demo                                    Executar demonstração completa");
        Console.WriteLine("  -abrir <clienteId> <produtoId> <tipo> <descricao>");
        Console.WriteLine("  -atribuir <ticketId> <operadorId>");
        Console.WriteLine("  -resolver <ticketId> <resolucao>");
        Console.WriteLine("  -fechar <ticketId>");
        Console.WriteLine("  -avaliar <ticketId> <nota>");
        Console.WriteLine("  -buscar <ticketId>");
        Console.WriteLine("  -listar <estado>                         Estados: Aberto, EmProgresso, Resolvido, Fechado");
        Console.WriteLine("\nOperadores:");
        Console.WriteLine("  -operadores                              Listar todos os operadores");
        Console.WriteLine("  -promover <operadorId>");
        Console.WriteLine("  -despromover <operadorId>");
        Console.WriteLine("\nAjuda:");
        Console.WriteLine("  -ajuda ou -h                             Mostrar esta ajuda");
    }
}