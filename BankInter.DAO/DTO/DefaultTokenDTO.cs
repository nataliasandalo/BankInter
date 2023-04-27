using BankInter.DAO.Response;
using System;

namespace BankInter.DAO.DTO
{
    public class DefaultTokenDTO : ResponseBase
    {
        public String access_token { get; set; }
        public String refresh_token { get; set; }
        public String token_type { get; set; }
    }
}
