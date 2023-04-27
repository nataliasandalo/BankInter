using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankInter.Shared.Model
{
    public class Conexao
    {
        public void Abrir()
        {
            // Implementação para abrir a conexão com o banco de dados ou API de pagamento
        }

        public void add(string v, string id)
        {
            throw new NotImplementedException();
        }

        public void encerrarConexao()
        {
            throw new NotImplementedException();
        }

        public void Fechar()
        {
            // Implementação para fechar a conexão com o banco de dados ou API de pagamento
        }

        public SqlDataReader oDR(string v)
        {
            throw new NotImplementedException();
        }
    }
}
