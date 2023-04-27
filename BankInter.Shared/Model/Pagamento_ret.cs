using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankInter.Shared.Model
{
    public class Pagamento_ret
    {
        public string json;
        public string erro;

        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }
        public int IdPagamento { get; set; }
    }
}
