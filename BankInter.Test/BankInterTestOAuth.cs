//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Security.Cryptography.X509Certificates;
//using System.Text;
//using System.Threading.Tasks;

//namespace OAuth2Test
//{
//    [TestClass]
//    public class OAuth2Test
//    {
//        [TestMethod]
//        public void TestGetTokenAsync()
//        {
//            // Arrange
//            bool sandbox = true;
//            string clientID = "eee92e34-76d9-491a-836e-ea1e3f0533d7"; // Substitua pelo seu Client ID
//            string clientSecret = "ec80e0b7-0d78-4bfd-aa6a-a657e1a5449b"; // Substitua pelo seu Client Secret
//            string[] scopes = new string[] {
//                "consute-saldo",
//                "consultar-extrato",
//                "consultar-lotes-pagamentos",
//                "realizar-pagamentos-darf",
//                "realizar-pagamentos-lotes",
//                "realizar-pagamentos-pix",
//                "consultar-pagamentos-codigos-barras-darf",
//                "realizar-pagamentos-codigos-barras",
//                "emitir-editar-cobrancas-imediatas",
//                "consultar-pix-recebidos-devolucoes",
//                "consultar-payloads",
//                "consultar-cobrancas-imediatas",
//                "criar-editar-webhooks",
//                "solicitar-devolucao-pix",
//                "criar-editar-payloads",
//                "emitir-cancelar-boletos",
//                "consultar-boletos-exportar-pdf"
//            };

//            OAuth2 oAuth2 = new OAuth2(sandbox, clientID, clientSecret);

//            // Act
//            var token = oAuth2.GetTokenAsync("boleto-cobranca.read").Result;

//            // Assert
//            Assert.IsNotNull(token);
//            Assert.IsFalse(token.IsExpired());
//            Assert.IsNotNull(token.AccessToken);
//        }
//    }
//}