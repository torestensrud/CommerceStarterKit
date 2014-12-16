/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System;
using System.Net;
using System.Text;
using System.Web;
using EPiServer;
using EPiServer.Core;
using EPiServer.Logging;
using Mediachase.Commerce.Core;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Orders.Dto;
using Mediachase.Commerce.Orders.Managers;
using Mediachase.Commerce.Plugins.Payment;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace OxxCommerceStarterKit.Core.PaymentProviders.DIBS
{
    public class DIBSPaymentGateway : AbstractPaymentGateway
    {
        public const string UserParameter = "MerchantID";        
        public const string ProcessingUrl = "ProcessingUrl";
        public const string KeyParameter = "Key";        

        public const string Capture = "CaptureTransaction";
        public const string Refund = "RefundTransaction";
        public const string Cancel = "CancelTransaction";

        public const string PaymentCompleted = "DIBS payment completed";

        private string _merchant;
        private PaymentMethodDto _payment;
        private static string _key;        


        private ILogger _log = LogManager.GetLogger();

        /// <summary>
        /// Processes the payment.
        /// </summary>
        /// <param name="payment">The payment.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public override bool ProcessPayment(Mediachase.Commerce.Orders.Payment payment, ref string message)
        {

            // We need this for some of the properties on this class that loads
            // payment method parameters (like MDS keys)
            _payment = PaymentManager.GetPaymentMethod(payment.PaymentMethodId); // .GetPaymentMethodBySystemName("DIBS", SiteContext.Current.LanguageName);

            if (payment.Parent.Parent is PurchaseOrder)
            {
                if (payment.TransactionType == TransactionType.Capture.ToString())
                {                    
                    bool isCaptured= CaptureTransaction(payment);
                    
                    if (!isCaptured)
                    {
                        message = "There was an error while capturing payment with DIBS";
                        return false;
                    }
                    return true;
                }

                if (payment.TransactionType == TransactionType.Credit.ToString())
                {
                    var transactionID = payment.TransactionID;
                    if (string.IsNullOrEmpty(transactionID) || transactionID.Equals("0"))
                    {
                        message = "TransactionID is not valid or the current payment method does not support this order type.";
                        return false;
                    }
                    //The transact must be captured before refunding
                    bool isRefunded = RefundTransaction(payment);
                    if (!isRefunded)
                    {
                        message = "There was an error while refunding with DIBS";
                        return false;
                    }

                    return true;
                }
                //right now we do not support processing the order which is created by Commerce Manager
                message = "The current payment method does not support this order type.";
                return false;
            }

            Cart cart = payment.Parent.Parent as Cart;
            if (cart != null && cart.Status == PaymentCompleted)
            {
                //return true because this shopping cart has been paid already on DIBS
                return true;
            }

            if (HttpContext.Current != null)
            {
                var pageRef = DataFactory.Instance.GetPage(PageReference.StartPage)["DIBSPaymentPage"] as PageReference;
                PageData page = DataFactory.Instance.GetPage(pageRef);
                HttpContext.Current.Response.Redirect(page.LinkURL);
            }
            else
            {
                throw new NullReferenceException("Cannot redirect to payment page without Http Context");
            }

            return true;
        }

 
        private string GetAmount(Mediachase.Commerce.Orders.Payment payment)
        {
            var amountInCents = (payment.Amount*100).ToString("0");
            return amountInCents;
        }

        /// <summary>
        /// Gets the payment.
        /// </summary>
        /// <value>The payment.</value>
        public PaymentMethodDto Payment
        {
            get
            {
                if (_payment == null)
                {
                    _payment = PaymentManager.GetPaymentMethodBySystemName("DIBS", SiteContext.Current.LanguageName);
                }
                return _payment;
            }
        }

        /// <summary>
        /// Gets the merchant.
        /// </summary>
        /// <value>The merchant.</value>
        public string Merchant
        {
            get
            {
                if (String.IsNullOrEmpty(_merchant))
                {
                    _merchant = GetParameterByName(Payment, DIBSPaymentGateway.UserParameter).Value;
                }
                return _merchant;
            }
        }
      

        public string Key
        {
            get
            {
                if (string.IsNullOrEmpty(_key))
                {
                    _key = GetParameterByName(Payment, KeyParameter).Value;
                }
                return _key;
            }
        }


        /// <summary>
        /// Gets the parameter by name.
        /// </summary>
        /// <param name="paymentMethodDto">The payment method dto.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static PaymentMethodDto.PaymentMethodParameterRow GetParameterByName(PaymentMethodDto paymentMethodDto, string name)
        {
            PaymentMethodDto.PaymentMethodParameterRow[] rowArray = (PaymentMethodDto.PaymentMethodParameterRow[])paymentMethodDto.PaymentMethodParameter.Select(string.Format("Parameter = '{0}'", name));
            if ((rowArray != null) && (rowArray.Length > 0))
            {
                return rowArray[0];
            }
            throw new ArgumentNullException("Parameter named " + name + " for DIBS payment cannot be null");
        }
        

        /**
* CaptureTransaction
* Captures a previously authorized transaction using the CaptureTransaction JSON service
* @param amount The amount of the capture in smallest unit
* @param merchantId DIBS Merchant ID / customer number
* @param transactionId The ticket number on which the authorization should be done
* @param K The secret HMAC key from DIBS Admin
*/

        public bool CaptureTransaction(Mediachase.Commerce.Orders.Payment payment)
        {
            var message = GetTransactionMessage(payment);            
            Dictionary<string, string> res = postToDIBS(Capture, message);
            return GetTransactionResult(res);
        }


        public bool RefundTransaction(Mediachase.Commerce.Orders.Payment payment)
        {
            var message = GetTransactionMessage(payment);
            Dictionary<string, string> res = postToDIBS(Refund, message);
            return GetTransactionResult(res);
        }

        public bool CancelTransaction(Mediachase.Commerce.Orders.Payment payment)
        {
            var message = GetTransactionMessage(payment);
            Dictionary<string, string> res = postToDIBS(Cancel, message);
            return GetTransactionResult(res);
        }

        private bool GetTransactionResult(Dictionary<string, string> res)
        {
            _log.Debug("CaptureTransaction done.");
            _log.Debug("Response:");
            foreach (KeyValuePair<string, string> r in res)
            {
                _log.Debug("{0} = {1}", r.Key, r.Value);
            }

            if (res["status"] != "ACCEPT")
                return false;

            return true;
        }

        private Dictionary<string, string> GetTransactionMessage(Mediachase.Commerce.Orders.Payment payment)
        {
            var macCalculator = new HmacCalculator(Key);
            var merchantId = Merchant;

            //Create Dictionary<string, string> object with used values. Can be modified to contain additional parameters.
            Dictionary<string, string> message = new Dictionary<string, string>
            {
                {"amount", GetAmount(payment)},
                {"merchantId", merchantId},
                {"transactionId", payment.TransactionID}
            };

            //Calculate mac and add it
            string mac = macCalculator.GetHex(message);
            message.Add("MAC", mac);
            return message;
        }

        /**
* postToDIBS
* Sends a set of parameters to a DIBS API function
* @param paymentFunction The name of the target payment function, e.g. AuthorizeCard
* @param data A set of parameters to be posted in Dictionary<string, string> format
* @return Dictionary<string, string>
*/
        static Dictionary<string, string> postToDIBS(string paymentFunction, Dictionary<string, string> data)
        {
            //Set correct POST URL corresponding to the payment function requested
            string postUrl = "https://api.dibspayment.com/merchant/v1/JSON/Transaction/";
            switch (paymentFunction)
            {
                case "AuthorizeCard":
                    postUrl += "AuthorizeCard";
                    break;
                case "AuthorizeTicket":
                    postUrl += "AuthorizeTicket";
                    break;
                case Cancel:
                    postUrl += Cancel;
                    break;
                case Capture:
                    postUrl += Capture;
                    break;
                case "CreateTicket":
                    postUrl += "CreateTicket";
                    break;
                case Refund:
                    postUrl += Refund;
                    break;
                case "Ping":
                    postUrl += "Ping";
                    break;
                default:
                    System.Console.WriteLine("Wrong input paymentFunctions to postToDIBS");
                    postUrl = null;
                    break;
            }

            //Create JSON string from Dictionary<string, string>
            string json_data = JsonConvert.SerializeObject(data);
            json_data = "request=" + json_data;

            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] json_data_encoded = encoding.GetBytes(json_data);

            //Using HttpWebRequest for posting and receiving response
            HttpWebRequest con = (HttpWebRequest)WebRequest.Create(postUrl);

            con.Method = "POST";
            con.ContentType = "application/x-www-form-urlencoded";
            con.ContentLength = json_data_encoded.Length;
            con.Timeout = 15000; //15 seconds timeout

            //Send the POST request
            using (Stream stream = con.GetRequestStream())
            {
                stream.Write(json_data_encoded, 0, json_data_encoded.Length);
            }

            //Receive response
            Dictionary<string, string> res_dict = new Dictionary<string, string> { };
            try
            {
                HttpWebResponse response = (HttpWebResponse)con.GetResponse();
                string responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                //Create Dictionary<string,string> hashmap from response JSON data
                res_dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseString);
            }
            catch (System.Net.WebException)
            {
                Console.WriteLine("Timeout occured...");
            }
            return res_dict;
        }


    }
}
