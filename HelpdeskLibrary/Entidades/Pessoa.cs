/**
 * @author Pedro Andrade
 * @email a31497@alunos.ipca.pt
 * @create date 2025-11 
 * @desc Classe que representa a pessoa genérica no sistema de helpdesk
 */
using Callcenter.HelpdeskLibrary.Interfaces;
using System;

namespace Callcenter.HelpdeskLibrary.Entidades;

/// <summary>
/// Classe base abstrata que representa uma pessoa no sistema.
/// Serve como classe pai para Cliente e Operador, fornecendo propriedades comuns.
/// Usa construtor primário (C# 12+) para inicialização simplificada.
/// Implementa IIdentificavel para garantir que todas as pessoas têm um ID único.
/// </summary>
public class Pessoa(int id, string nome, string email, string telefone): IIdentificavel
{
    #region Attributes
    // Não há atributos privados - tudo é gerido via propriedades init-only
    #endregion

    #region Constructors
    // O construtor primário está na declaração da classe
    // Sintaxe: public class Pessoa(params...) : Interface
    #endregion

    #region Properties

    /// <summary>
    /// Identificador único da pessoa no sistema.
    /// init = só pode ser definido na inicialização (imutável após criação)
    /// </summary>
    public int Id { get; init; } = id;

    /// <summary>
    /// Nome completo da pessoa.
    /// init = imutável após criação (evita alterações acidentais)
    /// </summary>
    public string Nome { get; init; } = nome;

    /// <summary>
    /// Endereço de email para contacto.
    /// init = imutável após criação
    /// </summary>
    public string Email { get; init; } = email;

    /// <summary>
    /// Número de telefone para contacto.
    /// init = imutável após criação
    /// </summary>
    public string Telefone { get; init; } = telefone;
    #endregion

    #region Overrides
    /// <summary>
    /// Representação textual da pessoa.
    /// Expression-bodied member (=>) para sintaxe concisa.
    /// </summary>
    /// <returns>String formatada com os dados da pessoa</returns>
    public override string ToString() => $"Id: {Id}, Nome: {Nome}, Email: {Email}, Telefone: {Telefone}";
    #endregion

    #region OtherMethods
    // Reservado para métodos auxiliares futuros
    #endregion

    #region Destructor
    /// <summary>
    /// Destrutor - raramente necessário em C# moderno.
    /// </summary>
    #endregion
}
