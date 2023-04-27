using BancoInterDotNet.DAO.Service;
using BankInter.DAO.DTO;
using BankInter.DAO.Model;
using BankInter.DAO.Response;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace BankInter.DAO.Service
{
    public static class BancoInterService
    {
        private const String URL_BASE_COBRANCA = "https://cdpj.partners.bancointer.com.br/cobranca/v2/";

        private const String URL_CONSULTAR_SALDO = "https://cdpj.partners.bancointer.com.br/banking/v2/saldo";

        private const String URL_TOKEN = "https://cdpj.partners.bancointer.com.br/oauth/v2/token";

        private const String URL = "https://cdpj.partners.bancointer.com.br/";

        private const String ESCOPO_CRIAR_BOLETO = "boleto-cobranca.write";
        private const String ESCOPO_CONSULTAR_BOLETO = "boleto-cobranca.read";
        private const String ESCOPO_CANCELAR_BOLETO = "boleto-cobranca.write";
        private const String ESCOPO_PDF_BOLETO = "boleto-cobranca.read";

        public static ConfiguracaoEmpresa ObterConfiguracaoDaEmpresa()
        {
            return ConfiguracaoEmpresaService.ObterConfiguracao();
        }

        public static string ObterWebClient()
        {
            var configuracao = ConfiguracaoEmpresaService.ObterConfiguracao();
            var webClient = new WebClient();
            //Seta o certificado que irá junto com a requisição
            var certificate = new X509Certificate2(configuracao.PathCertificado, configuracao.SenhaCertificado);
            webClient.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + Convert.ToBase64String(Encoding.ASCII.GetBytes($"{configuracao.ClientId}:{configuracao.ClientSecret}")));
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(URL);
            httpWebRequest.ClientCertificates.Add(certificate);
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            var responseStream = httpResponse.GetResponseStream();
            var streamReader = new StreamReader(responseStream, Encoding.UTF8);
            var responseText = streamReader.ReadToEnd();
            return responseText;
        }

        public static async Task<DefaultTokenDTO> ObterToken(String Escopo)
        {
            DefaultTokenDTO defaultToken = null;
            try
            {
                var clientId = "416f2e18-9da5-4589-8b45-6cf5c1409b40";
                var clientSecret = "9f7f2c06-7b85-4c68-8a3a-2f9341429b72";
                var pathCertificado = @"C:\Users\natal\Desktop\Sapiens\Eduardo\Bank Inter\Inter_API-Chave_e_Certificado\Inter API_Certificado.crt";
                var senhaCertificado = "123456";
                var url = "https://cdpj.partners.bancointer.com.br/oauth/v2/token";
                
                // Configura o certificado
                var certificado = new X509Certificate2(pathCertificado, senhaCertificado);
                // Disable SSL/TLS certificate validation
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                // Force the use of TLS 1.2
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                // Cria a solicitação HTTP POST
                var requisicao = WebRequest.Create(url) as HttpWebRequest;

                requisicao.Method = "POST";
                requisicao.ContentType = "application/x-www-form-urlencoded";
                // Adiciona os parâmetros ao corpo da solicitação
                var parametros = string.Format("client_id={0}&client_secret={1}&grant_type=client_credentials&scope=boleto-cobranca.read", clientId, clientSecret);
                var dados = Encoding.ASCII.GetBytes(parametros);
                requisicao.ContentLength = dados.Length;

                using (var stream = requisicao.GetRequestStream())
                {
                    stream.Write(dados, 0, dados.Length);
                }

                // Envia a solicitação e analisa a resposta
                var resposta = requisicao.GetResponse() as HttpWebResponse;
                using (var stream = resposta.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    var json = reader.ReadToEnd();

                    DefaultTokenDTO obj = JsonConvert.DeserializeObject<DefaultTokenDTO>(json);

                    defaultToken = new DefaultTokenDTO();
                    defaultToken.access_token = obj.access_token;
                    defaultToken.token_type = obj.token_type;
                    defaultToken.refresh_token = obj.refresh_token;
                }

                //var conteudoKVP = new List<KeyValuePair<String, String>>
                //{
                //    new KeyValuePair<String, String>("client_id", ObterConfiguracaoDaEmpresa().ClientId),
                //    new KeyValuePair<String, String>("client_secret", ObterConfiguracaoDaEmpresa().ClientSecret),
                //    new KeyValuePair<String, String>("grant_type", "client_credentials"),
                //    new KeyValuePair<String, String>("scope", Escopo),
                //};

                //var conteudo = new FormUrlEncodedContent(conteudoKVP);

                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12; // add this line

                //using (var httpClient = new HttpClient())
                //{
                //    var respostaRequisicao = await httpClient.PostAsync(URL_TOKEN, conteudo);

                //    if (respostaRequisicao.IsSuccessStatusCode)
                //    {
                //        var responseString = await respostaRequisicao.Content.ReadAsStringAsync();
                //        DefaultTokenDTO obj = JsonConvert.DeserializeObject<DefaultTokenDTO>(responseString);

                //        defaultToken = new DefaultTokenDTO();
                //        defaultToken.access_token = obj.access_token;
                //        defaultToken.token_type = obj.token_type;
                //        defaultToken.refresh_token = obj.refresh_token;
                //    }
                //}
            }
            catch (Exception ex)
            {
                defaultToken = new DefaultTokenDTO();
                defaultToken.TemErro = true;
                defaultToken.Mensagem = ex.Message;
            }

            return defaultToken;
        }

        public static async Task<RegistroBoletoResponseDTO> RegistrarBoleto(RegistroBoletoDTO emissaoBoletoDTO)
        {
            RegistroBoletoResponseDTO response = new RegistroBoletoResponseDTO();

            try
            {
                var objToken = await ObterToken(ESCOPO_CRIAR_BOLETO);

                if (objToken.TemErro)
                {
                    response.TemErro = true;
                    response.Mensagem = objToken.Mensagem;
                    return response;
                }

                var httpClient = new HttpClient();

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", objToken.access_token);

                var boletoSerializado = JsonConvert.SerializeObject(emissaoBoletoDTO);
                var conteudo = new StringContent(boletoSerializado, Encoding.UTF8, "application/json");
                var url = String.Concat(URL_BASE_COBRANCA, "boletos");
                var message = await httpClient.PostAsync(url, conteudo);
                var resultadoStr = await message.Content.ReadAsStringAsync();
                message.EnsureSuccessStatusCode();
                response = JsonConvert.DeserializeObject<RegistroBoletoResponseDTO>(resultadoStr);
            }
            catch (Exception ex)
            {
                response.TemErro = true;
                response.Mensagem = ex.Message;
            }

            return response;
        }

        public static async Task<ConsultaBoletoDetalhadoResponseDTO> ConsultarBoletoDetalhado(String nossoNumero)
        {
            ConsultaBoletoDetalhadoResponseDTO response = new ConsultaBoletoDetalhadoResponseDTO();

            try
            {
                var httpClient = new HttpClient();
                var objToken = await ObterToken(ESCOPO_CONSULTAR_BOLETO);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", objToken.access_token);
                var url = URL_BASE_COBRANCA + "boletos/" + nossoNumero;
                var message = await httpClient.GetAsync(url);

                var resultadoStr = await message.Content.ReadAsStringAsync();
                message.EnsureSuccessStatusCode();
                response = JsonConvert.DeserializeObject<ConsultaBoletoDetalhadoResponseDTO>(resultadoStr);

            }
            catch (Exception ex)
            {
                response.TemErro = true;
                response.Mensagem = ex.Message;
            }

            return response;
        }

        public static async Task<ConsultaBoletoEmLoteResponseDTO> ConsultarBoletoBancarioEmLote(String dataInicial, String dataFinal)
        {
            ConsultaBoletoEmLoteResponseDTO response = new ConsultaBoletoEmLoteResponseDTO();

            try
            {
                var httpClient = new HttpClient();
                var objToken = await ObterToken(ESCOPO_CONSULTAR_BOLETO);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", objToken.access_token);
                var url = URL_BASE_COBRANCA + "boletos";
                var parametros = new Dictionary<String, String>
                                    {
                                        { "dataInicial", dataInicial},
                                        { "dataFinal", dataFinal }
                                    };
                var queryString = string.Join("&", parametros.Select(x => x.Key + "=" + Uri.EscapeDataString(x.Value)));
                url = url + "?" + queryString;
                var message = await httpClient.GetAsync(url);
                var resultadoStr = await message.Content.ReadAsStringAsync();
                message.EnsureSuccessStatusCode();
                response = JsonConvert.DeserializeObject<ConsultaBoletoEmLoteResponseDTO>(resultadoStr);
            }
            catch (Exception ex)
            {
                response.TemErro = true;
                response.Mensagem = ex.Message;
            }

            return response;
        }

        public static async Task<RespostaComMensagemDTO> BaixarBoleto(String nossoNumero, String motivoBaixa)
        {
            RespostaComMensagemDTO response = new RespostaComMensagemDTO();

            try
            {
                var objToken = await ObterToken(ESCOPO_CANCELAR_BOLETO);
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", objToken.access_token);

                var parametros = new Dictionary<String, String>();
                parametros.Add("motivoCancelamento", motivoBaixa);
                var url = String.Format(String.Concat(URL_BASE_COBRANCA, "boletos/{0}/cancelar"), nossoNumero);
                var content = new StringContent(JsonConvert.SerializeObject(parametros), Encoding.UTF8, "application/json");

                var message = await httpClient.PostAsync(url, content);
                message.EnsureSuccessStatusCode();

                var resultadoStr = await message.Content.ReadAsStringAsync();
                response.TemErro = false;
                return response;

            }
            catch (Exception ex)
            {
                response.TemErro = true;
                response.Mensagem = ex.Message;
            }

            return response;
        }

        public static async Task<RespostaComPDFDTO> ObterPDFBoleto(String nossoNumero)
        {
            RespostaComPDFDTO response = new RespostaComPDFDTO();

            try
            {
                var httpClient = new HttpClient();
                var objToken = await ObterToken(ESCOPO_PDF_BOLETO);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", objToken.access_token);
                var url = String.Concat(URL_BASE_COBRANCA, String.Format("boletos/{0}/pdf", nossoNumero));
                var message = await httpClient.GetAsync(url);
                var resultadoBytes = await message.Content.ReadAsByteArrayAsync();
                message.EnsureSuccessStatusCode();

                response.TemErro = false;
                response.pdf = Encoding.UTF8.GetString(resultadoBytes);

            }
            catch (Exception ex)
            {
                response.TemErro = true;
                response.Mensagem = ex.Message;
            }

            return response;
        }

        public static async Task<string> GetOAuthToken()
        {

            // Credenciais da aplicação
            string clientId = "416f2e18-9da5-4589-8b45-6cf5c1409b40";
            string clientSecret = "9f7f2c06-7b85-4c68-8a3a-2f9341429b72";

            // Caminho do certificado e da chave privada
            string certPath = @"C:\Users\natal\Desktop\Sapiens\Eduardo\Bank Inter\Inter_API-Chave_e_Certificado\Inter API_Certificado.crt";
            string keyPath = @"C:\Users\natal\Desktop\Sapiens\Eduardo\Bank Inter\Inter_API-Chave_e_Certificado\Inter API_Chave.key";

            // Carrega o certificado e a chave privada
            X509Certificate2 certi = new X509Certificate2(certPath, "", X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);
            certi.PrivateKey = GetPrivateKey(keyPath);

            // Cria a solicitação para obter o token OAuth
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://cdpj.partners.bancointer.com.br/oauth/v2/token");
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            //request.KeepAlive = false; // disable keep-alive

            // Define as credenciais da aplicação e o escopo desejado
            string postData = $"client_id={clientId}&client_secret={clientSecret}&grant_type=client_credentials&scope=boleto-cobranca.read";
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            request.ContentLength = byteArray.Length;

            // Define o certificado para autenticar a solicitação
            request.ClientCertificates.Add(certi);

            // Force the use of TLS 1.1
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            // Envia a solicitação e lê a resposta
            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(byteArray, 0, byteArray.Length);
            }

            using (WebResponse response = request.GetResponse())
            {
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream);
                    string responseText = reader.ReadToEnd();
                    return responseText;
                }
            }
        }

        private static RSA GetPrivateKey(string path)
        {
            using (StreamReader reader = new StreamReader(path))
            {
                string keyText = reader.ReadToEnd();
                return RSA.Create(keyText);
            }
        }

    }
}
