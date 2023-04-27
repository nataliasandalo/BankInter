using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankInter.Shared.Model
{
    public class Transacao_ret
    {
        public int codigo { get; set; }
        public Data_data data { get; set; }
        public string mensagem { get; set; }

        public Transacao_ret(int codigo, Data_data data, string mensagem)
        {
            this.codigo = codigo;
            this.data = data;
            this.mensagem = mensagem;
        }
    }
}
