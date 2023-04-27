using BankInter.DAO.Model;

namespace BancoInterDotNet.DAO.Service
{
    public static class ConfiguracaoEmpresaService
    {
        public static ConfiguracaoEmpresa ObterConfiguracao()
        {
            ConfiguracaoEmpresa empresaConfiguracao = new ConfiguracaoEmpresa();
            empresaConfiguracao.ClientId = "416f2e18-9da5-4589-8b45-6cf5c1409b40";
            empresaConfiguracao.ClientSecret = "9f7f2c06-7b85-4c68-8a3a-2f9341429b72";
            empresaConfiguracao.PathCertificado = @"C:\Users\natal\Desktop\Sapiens\Eduardo\Bank Inter\Inter_API-Chave_e_Certificado\Inter API_Certificado.crt";
            empresaConfiguracao.SenhaCertificado = "123456";

            return empresaConfiguracao;
        }
    }
}
