using System;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BankInter.DAO.Service;
using BankInter.DAO.DTO;
using BankInter.DAO.Model;
using BankInter.DAO.DTO.Boleto;

namespace ServicesTest
{
    [TestClass]
    public class ServicesTest
    {
        [TestMethod]
        public void ObterConfiguracaoDaEmpresa_DeveRetornarConfiguracaoValida()
        {
            // Arrange
            var configuracaoEsperada = new ConfiguracaoEmpresa
            {
                ClientId = "416f2e18-9da5-4589-8b45-6cf5c1409b40",
                ClientSecret = "9f7f2c06-7b85-4c68-8a3a-2f9341429b72",
                PathCertificado = "C:\\Users\\natal\\Desktop\\Sapiens\\Eduardo\\Bank Inter\\Inter_API-Chave_e_Certificado\\Inter API_Certificado.crt",
                SenhaCertificado = "123456",
            };

            // Act
            var configuracaoAtual = BancoInterService.ObterConfiguracaoDaEmpresa();

            // Assert
            Assert.AreEqual(configuracaoEsperada.ClientId, configuracaoAtual.ClientId);
            Assert.AreEqual(configuracaoEsperada.ClientSecret, configuracaoAtual.ClientSecret);
            Assert.AreEqual(configuracaoEsperada.PathCertificado, configuracaoAtual.PathCertificado);
            Assert.AreEqual(configuracaoEsperada.SenhaCertificado, configuracaoAtual.SenhaCertificado);
        }

        [TestMethod]
        public void ObterToken_DeveRetornarTokenValido()
        {
            // Arrange
            var escopo = "boleto-cobranca.read";

            // Act
            var token = BancoInterService.GetOAuthToken();

            // Assert
            //Assert.IsNotNull(token.access_token);
            //Assert.IsNotNull(token.token_type);
            //Assert.IsNotNull(token.refresh_token);
        }

        [TestMethod]
        public void RegistrarBoleto_DeveRegistrarBoleto()
        {
            // Arrange
            var boletoDTO = new RegistroBoletoDTO
            {
                nossoNumero = "123456",
                valorNominal = 100.0,
                dataVencimento = DateTime.Now.AddDays(7),
                pagador = new PagadorDTO()
                {
                    nome = ""
                    ,
                    cpfCnpj = "12345678901"
                    ,
                    email = "joao.silva@mail.com",
                },
                enderecoPagador = new EnderecoDTO
                {
                    Logradouro = "Rua da Amizade",
                    Numero = "123",
                    Bairro = "Centro",
                    Cidade = "São Paulo",
                    UF = "SP",
                    CEP = "01001-000",
                }
            };

            // Act
            var response = BancoInterService.RegistrarBoleto(boletoDTO).Result;

            // Assert
            Assert.IsFalse(response.TemErro);
            Assert.IsNotNull(response.nossoNumero);
            Assert.IsNotNull(response.linhaDigitavel);
        }

    }
}