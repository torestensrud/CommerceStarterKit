/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System;
using System.Collections;
using System.Collections.Generic;

namespace OxxCommerceStarterKit.Core.PaymentProviders
{
    public class HmacCalculator : EncryptionCalculator
    {
        private readonly string _key;

        public HmacCalculator()
        {
            
        }

        public HmacCalculator(string key)
        {
            _key = key;
        }

        public string GetHex(OrderInfo message)
        {
            return base.HashHMACHex(_key, message.ToString());
        }

        public string GetHex(Dictionary<string, string> message)
        {
            List<string> strings = new List<string>();

            foreach (KeyValuePair<string, string> orderParam in message)
            {
                strings.Add(string.Format("{0}={1}", orderParam.Key, orderParam.Value));
            }

            return base.HashHMACHex(_key, String.Join("&", strings));
        }     
    }
}
