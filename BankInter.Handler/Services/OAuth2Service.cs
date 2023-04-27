using BankInter.Shared.Model;
using System.IO;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

public class OAuth2Services
{
    private OAuth2 _oauth2Handler;

    public OAuth2Services(bool sandbox, string clientID, string clientSecret)
    {
        _oauth2Handler = new OAuth2(sandbox, clientID, clientSecret);
    }

    public async Task<Token> GetTokenAsync(string scopes)
    {
        return await _oauth2Handler.GetTokenAsync(scopes);
    }

    public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return await _oauth2Handler.SendAsyncWrapper(request, cancellationToken);
    }
}