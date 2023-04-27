using BankInter.Shared.Model;
using System.IO;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

public class OAuth2 : DelegatingHandler
{
    private bool _sandbox;
    private string _clientID;
    private string _clientSecret;
    private string _urlEndpoint;
    private string _encodedAuth;
    private Token _token;

    public OAuth2(bool sandbox, string clientID, string clientSecret)
    {
        _sandbox = sandbox;
        _clientID = clientID;
        _clientSecret = clientSecret;

        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

        _urlEndpoint = "https://cdpj.partners.bancointer.com.br/oauth/v2/token";

        _encodedAuth = Convert.ToBase64String(Encoding.GetEncoding("UTF-8").GetBytes(_clientID + ":" + _clientSecret));
    }

    public async Task<HttpResponseMessage> SendAsyncWrapper(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return await SendAsync(request, cancellationToken);
    }
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (_token == null || _token.IsExpired())
        {
            _token = await GetTokenAsync("boleto-cobranca.read");
        }

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token.AccessToken);

        return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }

    public async Task<Token> GetTokenAsync(string scopes)
    {
        try
        {
            var postData = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("client_id", _clientID),
                new KeyValuePair<string, string>("client_secret", _clientSecret),
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("scope", scopes)
            };

            string certFilePath = @"C:\Users\natal\Desktop\Sapiens\Eduardo\Bank Inter\Inter_API-Chave_e_Certificado\Inter API_Certificado.crt";
            string certPassword = "123456";

            string keyFilePath = @"C:\Users\natal\Desktop\Sapiens\Eduardo\Bank Inter\Inter_API-Chave_e_Certificado\Inter API_Chave.key";
            string keyPassword = "123456";

            X509Certificate2 cert = new X509Certificate2(certFilePath, certPassword);
            cert.PrivateKey = new X509Certificate2(keyFilePath).PrivateKey;

            var content = new FormUrlEncodedContent(postData);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

            var handler = new WebRequestHandler();
            handler.ClientCertificates.Add(cert);

            // Adicione o seguinte trecho:
            handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            handler.UseDefaultCredentials = true;

            var client = new HttpClient(handler);

            var response = await client.PostAsync(_urlEndpoint, content);

            if (!response.IsSuccessStatusCode)
            {
                var responseString2 = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to retrieve access token. Status code: {response.StatusCode}, Reason: {response.ReasonPhrase}. Response content: {responseString2}");
            }

            var responseString = await response.Content.ReadAsStringAsync();
            var token = JsonConvert.DeserializeObject<Token>(responseString);

            return token;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error while retrieving access token: {ex.Message}", ex);
        }
    }
}