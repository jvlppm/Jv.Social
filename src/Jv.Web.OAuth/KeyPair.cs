﻿using Jv.Web.OAuth.Json;
using System;
using System.Collections.Generic;

namespace Jv.Web.OAuth
{
    public class KeyPair
    {
        #region Properties
        public string Key { get; private set; }
        public string Secret { get; private set; }
        #endregion

        #region Constructors
        public KeyPair(string key, string secret)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");

            if (string.IsNullOrEmpty(secret))
                throw new ArgumentNullException("secret");

            Key = key;
            Secret = secret;
        }
        #endregion

        #region Serialization
        public override string ToString()
        {
            return new { key = Key, secret = Secret }.ToJson();
        }

        public static KeyPair Parse(string keyPair)
        {
            var pair = keyPair.AsJson() as IDictionary<string, object>;
            return new KeyPair(
                pair["key"] as string,
                pair["secret"] as string
            );
        }
        #endregion

        public override bool Equals(object obj)
        {
            if (obj is KeyPair)
            {
                var kp = (KeyPair)obj;
                return kp.Key == Key && kp.Secret == Secret;
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Secret.GetHashCode();
        }
    }
}