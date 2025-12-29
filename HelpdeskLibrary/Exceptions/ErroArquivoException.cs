using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Callcenter.HelpdeskLibrary.Exceptions
{
    
    public class ErroArquivoException : Exception
    {
        // Construtor padrão
        public ErroArquivoException() : base("Ocorreu um erro ao aceder ao ficheiro de dados.") { }

        // Construtor com mensagem personalizada
        public ErroArquivoException(string message) : base(message) { }

        //Construtor com mensagem personalizada e exceção original
        public ErroArquivoException(string message, Exception inner) : base(message, inner) { } // 'inner' é a exceção original que causou o erro

        // Necessário para a serialização da própria exceção
        protected ErroArquivoException(
          System.Runtime.Serialization.SerializationInfo info,
#pragma warning disable SYSLIB0051 // Obsoleto por motivos de segurança
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
#pragma warning restore SYSLIB0051 
    }
}
