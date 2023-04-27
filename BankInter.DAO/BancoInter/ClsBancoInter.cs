//using BankInter.Shared.Model;
//using System;
//using System.Data.SqlClient;
//using System.IO;
//using System.Net;
//using System.Text;
//using System.Web.Script.Serialization;

//namespace BankInter.DAO.BancoInter
//{
    
//    public class clsBancoInter
//    {

//        public bool sandbox;
//        //verifique se o TOKEN gerado no JS está apontando para qual ambiente para não haver incoerência

//        public string clientID = "";
//        public string clientSecret = "";
//        public string token_javascript = "";
//        public string url_endpoint = "";
//        string encodedAuth = "";

//        public clsBancoInter(bool prod, string _clientID, string _clientSecret, string _token_javascript)
//        {
//            sandbox = prod;
//            clientID = _clientID;
//            clientSecret = _clientSecret;
//            token_javascript = _token_javascript;
//            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
//            if (sandbox)
//                url_endpoint = "https://cdpj.partners.bancointer.com.br/oauth/v2";
//            else
//                url_endpoint = "https://cdpj.partners.bancointer.com.br/oauth/v2";
//        }

//        public Token pegarToken()
//        {
//            byte[] arrByte = Encoding.UTF8.GetBytes("{\"grant_type\":\"client_credentials\"}");
//            string retorno = "";

//            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;

//            WebRequest wr = WebRequest.Create(url_endpoint + "/token");
//            wr.Method = "POST";
//            wr.ContentLength = arrByte.Length;
//            wr.ContentType = "application/json";
//            wr.Headers.Add("authorization", "Basic " + encodedAuth);
//            wr.Headers.Add("api-sdk", "NOG");

//            Token token = new Token();
//            try
//            {
//                Stream dataStream = wr.GetRequestStream();
//                dataStream.Write(arrByte, 0, arrByte.Length);
//                dataStream.Close();

//                WebResponse response = wr.GetResponse();
//                using (dataStream = response.GetResponseStream())
//                {
//                    StreamReader reader = new StreamReader(dataStream);
//                    string responseFromServer = reader.ReadToEnd();
//                    retorno = responseFromServer.ToString();
//                }
//                response.Close();
//                token.erro = "";
//                retorno = retorno.Replace("}", "").Replace("{", "").Replace("\"", "");
//                string[] arr = retorno.Split(',');

//                token.access_token = arr[0].Split(':')[1].Trim();
//                token.refresh_token = arr[1].Split(':')[1].Trim();
//                token.expires_in = arr[2].Split(':')[1].Trim();
//                token.expire_at = arr[3].Split(':')[1].Trim();
//                token.token_type = arr[4].Split(':')[1].Trim();

//                //logGN("GN.pegarToken <<", "", retorno);
//            }
//            catch (Exception ex)
//            {
//                token.erro = "Servidor GerenciaNet não está acessível";
//                //logGN("GN.pegarToken <<", "", ex.ToString());
//            }

//            return token;
//        }

//        public Retorno conectar(string method, string rota, string json)
//        {
//            Retorno r = new Retorno();

//            encodedAuth = Convert.ToBase64String(Encoding.GetEncoding("UTF-8").GetBytes(clientID + ":" + clientSecret));

//            Token token = pegarToken();//esse token serve para o servidor aceitar qualquer solicitação a ele próprio
//            r.token = token.access_token;

//            if (!token.erro.Equals(""))
//            {
//                r.msg_erro = token.erro;
//                r.sucesso = false;
//            }
//            else
//            {
//                byte[] arrByte = Encoding.UTF8.GetBytes(json);
//                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;

//                string endpoint = url_endpoint + "/" + rota;
//                if (method.Equals("GET") && !json.Equals(""))
//                    endpoint = endpoint + "?" + json;

//                WebRequest wr = WebRequest.Create(endpoint);
//                wr.Method = method;
//                wr.Headers.Add("authorization", "Bearer " + token.access_token);
//                wr.Headers.Add("api-sdk", "NOG");
//                string conteudo = "endpoint: " + endpoint + " >> token Bearer: " + token.access_token;
//                //logGN("GN.conectar", HttpContext.Current.Session["id_pedido"] + "", conteudo);

//                if (method.Equals("POST"))
//                {
//                    wr.ContentType = "application/json";
//                    wr.ContentLength = arrByte.Length;
//                    Stream dataStream = wr.GetRequestStream();
//                    dataStream.Write(arrByte, 0, arrByte.Length);
//                    dataStream.Close();
//                    try
//                    {
//                        WebResponse response = wr.GetResponse();
//                        using (dataStream = response.GetResponseStream())
//                        {
//                            StreamReader reader = new StreamReader(dataStream);
//                            r.json = reader.ReadToEnd().ToString();
//                        }
//                        r.sucesso = true;
//                    }
//                    catch (WebException ex)
//                    {
//                        r.sucesso = false;
//                        r.code = "" + ((HttpWebResponse)ex.Response).StatusCode;
//                        var reader = new StreamReader(ex.Response.GetResponseStream());
//                        r.msg_erro = reader.ReadToEnd();
//                    }
//                }
//                else if (method.Equals("GET"))
//                {
//                    try
//                    {
//                        using (WebResponse response = wr.GetResponse())
//                        {
//                            StreamReader reader = new StreamReader(response.GetResponseStream());
//                            r.json = reader.ReadToEnd().ToString();
//                        }
//                        r.sucesso = true;
//                    }
//                    catch (WebException ex)
//                    {
//                        //reader.ReadToEnd() +"<hr>" + statusCode + "<hr>" + ex.ToString()
//                        r.sucesso = false;
//                        r.code = "" + ((HttpWebResponse)ex.Response).StatusCode;
//                        var reader = new StreamReader(ex.Response.GetResponseStream());
//                        r.msg_erro = reader.ReadToEnd();
//                    }
//                }
//            }

//            return r;
//        }

//        public Parcelamento verParcelamento(string valor, string bandeira)
//        {
//            string json = "";
//            Parcelamento parcelamento = new Parcelamento();
//            if (!valor.Equals(""))
//            {
//                string dados = "brand=" + bandeira + "&total=" + valor;
//                json = conectar("GET", "installments", dados).json;
//                parcelamento = new JavaScriptSerializer().Deserialize<Parcelamento>(json);
//            }

//            return parcelamento;
//        }


//        public Cobranca_ret criarCobranca(string id_pedido, Cobranca cob)
//        {
//            Cobranca_ret ret = new Cobranca_ret();
//            string dados = new JavaScriptSerializer().Serialize(cob);
//            //Util.criarSqlLog(id_pedido, "Cobranca", "GN", dados, "0");
//            Retorno r = conectar("POST", "charge", dados);
//            if (r.sucesso)
//            {
//                ret = new JavaScriptSerializer().Deserialize<Cobranca_ret>(r.json);
//                ret.json = r.json;
//            }
//            else
//            {
//                //Util.criarSqlLog(id_pedido, "Cobranca_ret", "GN", r.msg_erro, "0");
//                ret.erro = r.msg_erro;
//            }
//            return ret;
//        }

//        public Pagamento_ret pagarCobranca(string id_pedido, Pagamento pg, int charge_id)
//        {
//            Pagamento_ret ret = new Pagamento_ret();
//            string dados = new JavaScriptSerializer().Serialize(pg);
//            dados = dados.Replace("\"credit_card\":null,", "").Replace(",\"banking_billet\":null", "").Replace(",\"juridical_person\":null", "");
//            //Util.criarSqlLog(id_pedido, "Pagamento", "GN", dados, "0");
//            Retorno r = conectar("POST", "charge/" + charge_id + "/pay", dados);
//            if (r.sucesso)
//            {
//                ret = new JavaScriptSerializer().Deserialize<Pagamento_ret>(r.json);
//                //Util.criarSqlLog(id_pedido, "Pagamento_ret", "GN", r.json, "0");
//                ret.json = r.json;
//            }
//            else
//            {
//                //Util.criarSqlLog(id_pedido, "Pagamento_ret", "GN", r.msg_erro, "0");
//                ret.erro = r.msg_erro;
//            }
//            return ret;
//        }

//        public Retorno checarStatus(string id) {
//            string charge_id = "";
//            Retorno status = new Retorno();

//            Conexao c = new Conexao();
//            c.add("@id", id);
//            SqlDataReader sdr = c.oDR("select * from vwPedido where id_pedido = @id");
//            if (sdr.Read())
//            {
//                charge_id = "" + sdr["charge_id"];
//                status.code = "" + sdr["fl_gn"];
//                status.msg_usuario = "" + sdr["chr_usuario"];
//            }
//            c.encerrarConexao();

//            if (!charge_id.Equals(""))
//            {
//                Retorno r = conectar("GET", "charge/" + charge_id, "");
//                Transacao_ret ret = new JavaScriptSerializer().Deserialize<Transacao_ret>(r.json);

//                //Util.criarSqlLog(id, "Consulta", "GN", r.json, "0");

//                string fl_gn = ret.data.status;
//                status.code += ">" + fl_gn;
//                TransactSQL ped = new TransactSQL("");
//                ped.Add("fl_gn", fl_gn);
//                if (fl_gn.Equals("paid"))
//                    ped.Add("dt_pagamento", "getDate()", false);
//                else
//                    ped.Add("dt_pagamento", "null", false);
//                ped.Where("id_pedido");
//                ped.Update("tbPedido");
//                ped.Exec();
//            }
//            return status;
//        }
//    }

//    public class Token
//    {
//        public string erro = "";
//        public string access_token = "";
//        public string refresh_token = "";
//        public string expires_in = "";
//        public string expire_at = "";
//        public string token_type = "";
//    }

//    public class Retorno
//    {
//        public string json = "";
//        public string msg_usuario = "";
//        public string token = "";
//        public string msg_erro = "";
//        public bool sucesso = true;
//        public string code = "";
//    }

//    public class Browser
//    {
//        public string token = "";
//        public string link = "";
//        public string versao = "";
//        public bool prod = false;
//    }
    
//}