﻿using System;

namespace BankInter.DAO.Model
{
    public class ConfiguracaoEmpresa
    {
        public String ClientId { get; set; }
        public String ClientSecret { get; set; }
        public String PathCertificado { get; set; }
        public String SenhaCertificado { get; set; }
    }
}
