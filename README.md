# Sistema de Helpdesk
[![.NET 9](https://img.shields.io/badge/.NET-9.0-512BD4?style=flat&logo=dotnet)](https://dotnet.microsoft.com/)
[![C# 13](https://img.shields.io/badge/C%23-13.0-239120?style=flat&logo=csharp)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![Build Status](https://github.com/MasterZeroPT/sistema-helpdesk/actions/workflows/dotnet.yml/badge.svg)](https://github.com/MasterZeroPT/sistema-helpdesk/actions)
Este projeto implementa um sistema de gestão de helpdesk em C# (.NET 9), permitindo o registo e acompanhamento de assistências técnicas, gestão de clientes, operadores e produtos.

## 📋 Funcionalidades

### Gestão de Entidades
- **Clientes**: Registo, consulta e remoção (com validação de tickets ativos).
- **Operadores**: Gestão de equipa técnica, incluindo promoções/despromoções e verificação de disponibilidade.
- **Produtos**: Catálogo de produtos suportados pelo sistema.

### Ciclo de Vida de Assistências (Tickets)
O sistema suporta um fluxo completo de resolução de problemas:
1. **Abertura**: Criação de ticket associado a um cliente, produto e tipo (Técnica, Comercial, Suporte).
2. **Atribuição**: Designação de um operador responsável (valida disponibilidade).
3. **Resolução**: Registo da solução aplicada (muda estado para Resolvido).
4. **Fecho**: Encerramento formal do ticket (regista hora de fecho).
5. **Avaliação**: Feedback do cliente através de nota de satisfação.

### Interface de Utilizador (Console)
A aplicação (`ConsoleUI`) oferece três modos de operação distintos:
1. **Modo Interativo**: Menu visual com opções numeradas para navegação fácil.
2. **Modo Comando Contínuo**: Shell interativa (prompt `>`) para execução sequencial de comandos.
3. **Modo CLI (Single Command)**: Execução de comandos únicos via argumentos de linha de comando e encerramento imediato.

## 🚀 Como Executar

### Pré-requisitos
- .NET 9 SDK
- Visual Studio 2026 ou VS Code

### Comandos Disponíveis
O sistema aceita diversos comandos para operação via terminal ou modo interativo:

| Comando | Sintaxe | Descrição |
|---------|---------|-----------|
| `-demo` | `-demo` | Executa uma demonstração automática de todo o fluxo. |
| `-abrir` | `-abrir <cliId> <prodId> <tipo> <desc>` | Abre uma nova assistência. |
| `-atribuir` | `-atribuir <ticketId> <opId>` | Atribui um operador a um ticket. |
| `-resolver` | `-resolver <ticketId> <solucao>` | Resolve um ticket com a descrição da solução. |
| `-fechar` | `-fechar <ticketId>` | Fecha um ticket resolvido. |
| `-avaliar` | `-avaliar <ticketId> <nota>` | Regista a avaliação do cliente. |
| `-listar` | `-listar <estado>` | Lista tickets (Aberto, EmProgresso, Resolvido, Fechado). |
| `-operadores` | `-operadores` | Lista todos os operadores cadastrados. |
| `-promover` | `-promover <opId>` | Promove um operador de nível. |

### Persistência de Dados
Os dados são automaticamente salvos e carregados de um arquivo JSON (`helpdesk_data.json`) na raiz da execução, garantindo que as informações não sejam perdidas entre sessões.

## 🛠️ Tecnologias
- **Linguagem**: C# 13.0
- **Framework**: .NET 9
- **Serialização**: System.Text.Json

## 📄 Licença
Este projeto foi desenvolvido no âmbito da disciplina de Programação Orientada a Objetos (IPCA).
