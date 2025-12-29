using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Callcenter.HelpdeskLibrary.Interfaces
{
    public interface IAvaliavel
    {
       int AvaliacaoSatisfacao {  get; set; }
        void RegistarAvaliacao(int nota);
    }
}
