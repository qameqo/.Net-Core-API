using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetCore_API.Models
{
    public class ChargeOmiseModel
    {
        public int amount { get; set; }
        public string source_id { get; set; }
        public string currency { get; set; }
        public string return_uri { get; set; }
        public string url { get; set; }
    }
    public class ResSourceOmiseModel
    {
        public string Object { get; set; }
        public string id { get; set; }
        public string livemodefalse { get; set; }
        public string location { get; set; }
        public string amount { get; set; }
        public string barcode { get; set; }
        public string created_at { get; set; }
        public string currency { get; set; }
        public string email{ get; set; }
        public string flow { get; set; }
        public string installment_term{ get; set; }
        public string name{ get; set; }
        public string mobile_number  { get; set; }
        public string phone_number  { get; set; }
        public string scannable_code{ get; set; }
        public string references{ get; set; }
        public string store_id{ get; set; }
        public string store_name{ get; set; }
        public string terminal_id{ get; set; }
        public string type { get; set; }
        public string zero_interest_installments{ get; set; }
        public string charge_status { get; set; }
    }
    public class ResChargeModel
    {
        public string Object  { get; set; }
        public string id { get; set; }
        public string location { get; set; }
        public string amount { get; set; }
        public string net { get; set; }
        public string fee { get; set; }
        public string fee_vat { get; set; }
        public string interest { get; set; }
        public string interest_vat { get; set; }
        public string funding_amount { get; set; }
        public string refunded_amount { get; set; }
        public string authorized { get; set; }
        public string capturable { get; set; }
        public string capture { get; set; }
        public string disputable { get; set; }
        public string livemode { get; set; }
        public string refundable { get; set; }
        public string reversed { get; set; }
        public string reversible { get; set; }
        public string voided { get; set; }
        public string paid { get; set; }
        public string expired { get; set; }
        public string currency { get; set; }
        public string funding_currency { get; set; }
        public string ip { get; set; }
        public string link { get; set; }
        public string description { get; set; }
        public card source { get; set; }
        public string schedule { get; set; }
        public string customer { get; set; }
        public string dispute { get; set; }
        public string transaction { get; set; }
        public string failure_code { get; set; }
        public string failure_message { get; set; }
        public string status { get; set; }
        public string authorize_uri { get; set; }
        public string return_uri { get; set; }
        public string created_at { get; set; }
        public string paid_at { get; set; }
        public string expires_at { get; set; }
        public string expired_at { get; set; }
        public string reversed_at { get; set; }
        public string zero_interest_installments { get; set; }
        public string branch { get; set; }
        public string terminal { get; set; }
        public string device { get; set; }
        public platform_fee platform_fee { get; set; }
        public refunds refunds { get; set; }
        public metadata metadata { get; set; }
        public card card { get; set; }
    }
    public class platform_fee
    {
        public string Fixed {get; set; }
        public string amount { get; set; }
        public string percentage { get; set; }

    }
    public class refunds 
    {
        public string  Object { get; set; }
        public string limit { get; set; }
        public string offset { get; set; }
        public string total { get; set; }
        public string location { get; set; }
        public string order { get; set; }
        public string from { get; set; }
        public string to { get; set; }
        public object data { get; set; }
    }
    public class metadata 
    {
        public string order_id { get; set; }
        public string color { get; set; }
    }
    public class card { 
        public string Object { get; set; }
        public string id { get; set; }
        public string livemode { get; set; }
        public string location { get; set; }
        public string deleted { get; set; }
        public string street1 { get; set; }
        public string street2 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string phone_number { get; set; }
        public string postal_code { get; set; }
        public string country { get; set; }
        public string financing { get; set; }
        public string bank { get; set; }
        public string brand { get; set; }
        public string fingerprint { get; set; }
        public string first_digits { get; set; }
        public string last_digits { get; set; }
        public string name { get; set; }
        public string expiration_month { get; set; }
        public string expiration_year { get; set; }
        public string security_code_check { get; set; }
        public string created_at { get; set; }
    }

    public class ReqCreateTokenOmise : ChargeOmiseModel
    {
        public string exp_month { get; set; }
        public string exp_year { get; set; }
        public string name { get; set; }
        public string number { get; set; }
        public string security_code { get; set; }
    }
}
