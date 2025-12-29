/**
 * @author Pedro Andrade
 * @email a31497@alunos.ipca.pt
 * @create date 2025-11 
 * @desc Programa de um Sistema de gestão e apoio a assistências por telefone
 */

using Callcenter.HelpdeskLibrary.Servicos;
using Callcenter.UI;
using System;

namespace Callcenter;

internal static class Program
{
    static void Main(string[] args)
    {
        var sistema = new SistemaHelpdesk();
        var consoleUI = new ConsoleUI(sistema);
        
        consoleUI.Executar(args);
    }
}