using dotnetCore_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetCore_API.Services.Interfaces
{
    public interface IOmiseServices
    {
        public Task<string> OmiseGetSourceTrueWallet();
        public Task<string> OmiseChargeTrueWallet(string s);
        public Task<string> OmiseChargeCredit(string s);
        public Task<string> OmiseTokenCredit(ReqCreateTokenOmise s);
        public QRCodeModel GenerateQRCode(QRModelReq req);
    }
}
