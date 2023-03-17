using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetCore_API.Models
{
    public class QRCodeModel
    {
        public QRModelReq input { get; set; }
        public QRModelRes result { get; set; }
        public QRCodeModel()
        {
            this.input = new QRModelReq();
            this.result = new QRModelRes();
        }
    }
    public class QRModelReq
    {
        public string amount { get; set; }
    }
    public class QRModelRes
    {
        public string Path { get; set; }
    }
}
