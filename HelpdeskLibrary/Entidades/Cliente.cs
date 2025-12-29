/**
 * @author Pedro Andrade
 * @email a31497@alunos.ipca.pt
 * @create date 2025-11 
 * @desc Classe que representa um cliente no sistema de helpdesk
 */
using System;

namespace Callcenter.HelpdeskLibrary.Entidades;

public class Cliente : Pessoa
{
    #region Attributes
    

    #endregion

    #region Methods
    #endregion

    #region Constructors
    public Cliente(int id, string nome, string email, string telefone) : base(id, nome, email, telefone)
    {
        DataRegisto = DateTime.Now;
    }

    #endregion

    #region Properties
    public DateTime DataRegisto { get; init; } = DateTime.Now;
    #endregion

    #region Overrides
    #endregion

    #region OtherMethods
    #endregion

    #region Destructor
    /// <summary>
    /// The destructor.
    /// </summary>

    #endregion
}