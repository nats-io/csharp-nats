﻿using System;
using System.Collections.Generic;
using System.Text;
using NATS.Client.Internals.SimpleJSON;

namespace NATS.Client.Internals
{
    internal static class JsonUtils
    {
        internal static readonly JSONNode MinusOne = new JSONNumber(-1);

        internal static int AsIntOrMinus1(JSONNode node, String field)
        {
            return node.GetValueOrDefault(field, MinusOne);
        }

        internal static long AsLongOrMinus1(JSONNode node, String field)
        {
            return node.GetValueOrDefault(field, MinusOne);
        }

        internal static List<string> StringList(JSONNode node, String field)
        {
            List<string> list = new List<string>();
            foreach (var child in node[field].Children)
            {
                list.Add(child.Value);
            }
           
            return list;
        }

        internal static List<string> OptionalStringList(JSONNode node, String field)
        {
            List<string> list = StringList(node, field);
            return list.Count == 0 ? null : list;
        }
        
        internal static byte[] AsByteArray(JSONNode node) {
            return Encoding.ASCII.GetBytes(node.Value);
        }
        
        internal static DateTime AsDate(JSONNode node)
        {
            try
            {
                return DateTime.Parse(node.Value);
            }
            catch (Exception)
            {
                return DateTime.MinValue;
            }
        }
        
    }
}