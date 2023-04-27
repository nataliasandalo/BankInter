using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankInter.Shared.Model
{
    public class Pagamento
    {
        public int IdCobranca { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataPagamento { get; set; }
    }
}
