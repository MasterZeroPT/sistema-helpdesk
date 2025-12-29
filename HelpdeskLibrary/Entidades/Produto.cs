/**
 * @author Pedro Andrade
 * @email a31497@alunos.ipca.pt
 * @create date 2025-11 
 * @desc Classe que representa um produto associado a uma assistência
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Callcenter.HelpdeskLibrary.Entidades
{
   
    public class Produto
    {
        #region Constructors
        public Produto() {}
        public Produto(int id, string nome, string garantiaAnos)
        {
            Id = id;
            Nome = nome;
            Garantia = $"Validade da garantia: {garantiaAnos} anos";
        }
        #endregion


        #region Properties
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Garantia { get; set; } = string.Empty;
        #endregion
    }
}
