using dotnetCore_API.Common;
using dotnetCore_API.Models;
using dotnetCore_API.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace dotnetCore_API.Services
{
    public class OmiseServices : IOmiseServices
    {
        private readonly IConfiguration _config;
        private readonly IHostingEnvironment _environment;
        public OmiseServices(IConfiguration config, IHostingEnvironment environment)
        {
            _config = config;
            _environment = environment;
        }
        public async Task<string> OmiseGetSourceTrueWallet()
        {
            try
            {
                string source_id = "";
                var httpClient = new HttpClient();
                var requestSource = new HttpRequestMessage(new HttpMethod("POST"), "https://api.omise.co/sources");
                var requestCharge = new HttpRequestMessage(new HttpMethod("POST"), "https://api.omise.co/charges");
                var base64Source = Convert.ToBase64String(Encoding.ASCII.GetBytes("pkey_test_5ushw2oggr0ak290m2p:"));
                requestSource.Headers.TryAddWithoutValidation("Authorization", $"Basic {base64Source}");

                var contentList = new List<string>();
                contentList.Add("amount=400000");
                contentList.Add("currency=THB");
                contentList.Add("type=truemoney");
                contentList.Add("phone_number=0656675778");
                requestSource.Content = new StringContent(string.Join("&", contentList));
                requestSource.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");

                var responseSource = await httpClient.SendAsync(requestSource);
                var resSource = JsonConvert.DeserializeObject<ResSourceOmiseModel>(responseSource.Content.ReadAsStringAsync().Result.ToString());
                source_id = resSource.id;

                return source_id;


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<string> OmiseChargeTrueWallet(string s)
        {
            try
            {
                HttpClient client = new HttpClient();
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://api.omise.co/charges");

                request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes("skey_test_5ush3swziumpl58bpun:")));
                request.Content = new StringContent("amount=400000&currency=THB&return_uri=http://localhost:26734/&source=" + s);
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

                HttpResponseMessage response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var resCharge = JsonConvert.DeserializeObject<ResChargeModel>(response.Content.ReadAsStringAsync().Result.ToString());
                return resCharge.authorize_uri;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public async Task<string> OmiseChargeCredit(string s)
        {
            try
            {
                HttpClient client = new HttpClient();
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://api.omise.co/charges");

                request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes("skey_test_5ush3swziumpl58bpun:")));
                request.Content = new StringContent("amount=400000&currency=THB&card=" + s);
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

                HttpResponseMessage response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var resCharge = JsonConvert.DeserializeObject<ResChargeModel>(response.Content.ReadAsStringAsync().Result.ToString());
                return resCharge.status;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }
        public async Task<string> OmiseTokenCredit(ReqCreateTokenOmise s)
        {
            try
            {
                string card = "card";
                string exp_y = card+"[expiration_year]=" + s.exp_year;
                string exp_m = card + "[expiration_month]=" + s.exp_month;
                string name = card + "[name]=" + s.name;
                string number = card + "[number]=" + s.number;
                string code = card + "[security_code]=" + s.security_code;

                HttpClient client = new HttpClient();
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://vault.omise.co/tokens");
                var base64Source = Convert.ToBase64String(Encoding.ASCII.GetBytes("pkey_test_5ushw2oggr0ak290m2p:"));
                request.Headers.TryAddWithoutValidation("Authorization", $"Basic {base64Source}");
                
                var contentList = new List<string>();
                contentList.Add(exp_y);
                contentList.Add(exp_m);
                contentList.Add(name);
                contentList.Add(number);
                contentList.Add(code);
                request.Content = new StringContent(string.Join("&", contentList));
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

                HttpResponseMessage response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var resCharge = JsonConvert.DeserializeObject<ResChargeModel>(response.Content.ReadAsStringAsync().Result.ToString());
                return resCharge.id;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        private string ComputeChecksum(string data)
        {
            ushort crc = 0xFFFF;
            byte[] bytes = Encoding.ASCII.GetBytes(data);
            for (int i = 0; i < bytes.Length; ++i)
            {
                crc ^= (ushort)(bytes[i] << 8);
                for (int j = 0; j < 8; ++j)
                {
                    if ((crc & 0x8000) > 0)
                        crc = (ushort)((crc << 1) ^ 0x1021);
                    else
                        crc <<= 1;
                }
            }
            // Return the CRC code as a hexadecimal string
            return crc.ToString("X4");
        }
        private string GeneratePayloadQRCode(string amount)
        {
            string promptPayID = _config["MbPromptpay"];
            string lengthamount = (!string.IsNullOrEmpty(amount.ToString())) ? amount.ToString().Length.ToString() : "0";
            string valuramount = (!string.IsNullOrEmpty(amount.ToString())) ? amount.ToString() : "";
            if (promptPayID.StartsWith("0"))
            {
                promptPayID = "66" + promptPayID.Substring(1);
            }
            if (lengthamount.Length < 2)
            {
                lengthamount = "0" + lengthamount;
            }

            string inputData = $"00020101021229370016A000000677010111011300{promptPayID}5802TH530376454{lengthamount}{valuramount}6304";
            //string inputData = "00020101021229370016A000000677010111011300666566757785802TH53037645406100.006304";

            string crc = ComputeChecksum(inputData);
            return inputData + crc;
        }
        public QRCodeModel GenerateQRCode(QRModelReq req)
        {
            QRCodeModel qRCode = new QRCodeModel();
            try
            {
                string amount = Decimal.Parse(req.amount).ToString("0.00");
                var qr = GeneratePayloadQRCode(amount);
                QRCodeGenerator QrGenerator = new QRCodeGenerator();
                QRCodeData QrCodeInfo = QrGenerator.CreateQrCode(qr, QRCodeGenerator.ECCLevel.M);
                QRCode QrCode = new QRCode(QrCodeInfo);
                Bitmap QrBitmap = QrCode.GetGraphic(60);

                string Banner1ImagePath = Path.Combine(_environment.ContentRootPath, @"wwwroot\img\qr.png");
                var logoImage = new Bitmap(Banner1ImagePath); //"D://C#//qr.png"
                var logoSize = new Size(QrBitmap.Width / 4, QrBitmap.Height / 7);
                var logoPosition = new Point((QrBitmap.Width - logoSize.Width) / 2, (QrBitmap.Height - logoSize.Height) / 2);
                logoPosition.X += 38;
                var logoRectangle = new Rectangle(logoPosition, logoSize);
                using (var graphics = Graphics.FromImage(QrBitmap))
                {
                    graphics.DrawImage(logoImage, logoRectangle);
                }

                byte[] BitmapArray = QrBitmap.BitmapToByteArray();
                qRCode.result.Path = string.Format("data:image/png;base64,{0}", Convert.ToBase64String(BitmapArray));
                return qRCode;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
