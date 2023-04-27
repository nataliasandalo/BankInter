using BankInter.DAO.DTO.Boleto;
using System;

namespace BankInter.DAO.DTO
{
    public class RegistroBoletoDTO
    {
        public string nossoNumero { get; set; }
        public double valorNominal { get; set; }
        public decimal valorAbatimento { get; set; }
        public DateTime dataVencimento { get; set; }
        public int numDiasAgenda { get; set; }
        public PagadorDTO pagador { get; set; }
        public MensagemDTO mensagem { get; set; }
        public DescontoDTO desconto1 { get; set; }
        public DescontoDTO desconto2 { get; set; }
        public DescontoDTO desconto3 { get; set; }
        public MultaDTO multa { get; set; }
        public MoraDTO mora { get; set; }
        public EnderecoDTO enderecoPagador { get; set; }

        public String seuNumero { get; set; }

        public RegistroBoletoDTO()
        {
            pagador = new PagadorDTO();
            mensagem = new MensagemDTO();
            desconto1 = new DescontoDTO();
            desconto2 = new DescontoDTO();
            desconto3 = new DescontoDTO();
            multa = new MultaDTO();
            mora = new MoraDTO();
        }
    }
}
