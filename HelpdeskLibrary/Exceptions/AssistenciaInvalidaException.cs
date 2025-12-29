/**
 * @author Pedro Andrade
 * @email a31497@alunos.ipca.pt
 * @create date 2025-11 
 * @desc Classe de exceção personalizada para assistências inválidas
 */
using System;
using System.Runtime.Serialization;

namespace Callcenter.HelpdeskLibrary.Exceptions
{
    
    public class AssistenciaInvalidaException : Exception
    {   // Construtor padrão
        public AssistenciaInvalidaException() { }
        // Construtor com mensagem personalizada
        public AssistenciaInvalidaException(string message) : base(message) { }
        // Construtor com mensagem personalizada e exceção original
        public AssistenciaInvalidaException(string message, Exception inner) : base(message, inner) { }
        // Necessário para a serialização da própria exceção
        protected AssistenciaInvalidaException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
