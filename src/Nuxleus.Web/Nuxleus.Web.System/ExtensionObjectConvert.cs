// Copyright 2010 Max Toro Q.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.XPath;
//using System.Xml.Xsl.Runtime;

namespace Nuxleus.Web.Sys {
   
   public static class ExtensionObjectConvert {

      public static readonly XPathNodeIterator EmptyIterator = new EmptyXPathNodeIterator();

      public static bool IsEmpty(object value) {
         return value == null || Object.ReferenceEquals(EmptyIterator, value);
      }

      public static object ToInput(object value) {

         if (value == null) throw new ArgumentNullException("value");

         Type type = value.GetType();

         switch (Type.GetTypeCode(type)) {
            case TypeCode.Boolean:
            case TypeCode.Byte:
            case TypeCode.Char:
            case TypeCode.DateTime:
            case TypeCode.Decimal:
            case TypeCode.Double:
            case TypeCode.Int16:
            case TypeCode.Int32:
            case TypeCode.Int64:
            case TypeCode.SByte:
            case TypeCode.Single:
            case TypeCode.UInt16:
            case TypeCode.UInt32:
            case TypeCode.UInt64:
            case TypeCode.String:
               return value;
               
            case TypeCode.DBNull:
            case TypeCode.Empty:
               throw new ArgumentException("value cannot be a null type.", "value");

            case TypeCode.Object:
            default:

               XPathItem item = value as XPathItem;

               if (item != null)
                  return ToInput(item);

               IXPathNavigable navigable = value as IXPathNavigable;

               if (navigable != null)
                  return ToInput(navigable);

               IEnumerable enumerable = value as IEnumerable;

               if (enumerable != null)
                  return ToInput(enumerable);

               return ToNode(value);
         }
      }

      public static string ToInput(string value) {
         return value;
      }

      public static object ToInput(XPathItem value) {

         if (value == null) throw new ArgumentNullException("value");

         if (value.IsNode)
            return value;
         
         return value.TypedValue;
      }

      public static XPathNavigator ToInput(IXPathNavigable value) {

         if (value == null) throw new ArgumentNullException("value");

         return value.CreateNavigator();
      }

      public static XPathNavigator[] ToInput(IEnumerable value) {

         if (value == null) throw new ArgumentNullException("value");

         return ToInput(
            ((IEnumerable)value).Cast<object>()
               .Where(o => !IsEmpty(o))
               .Select(o => ToItem(ToInput(o)))
         );
      }

      public static XPathNavigator[] ToInput(IEnumerable<XPathItem> value) {

         if (value == null) throw new ArgumentNullException("value");

         return value
            .Where(i => !IsEmpty(i))
            .Select(i => ToNode(i))
            .ToArray();
      }

      public static object ToInputOrEmpty(object value) {

         if (IsEmpty(value))
            return EmptyIterator;

         return ToInput(value);
      }

      public static object ToInputOrEmpty(string value) {
         return value ?? (object)EmptyIterator;
      }

      public static object ToInputOrEmpty<T>(Nullable<T> value) where T : struct {
         return (value.HasValue) ? ToInput(value.Value) : (object)EmptyIterator;
      }

      public static object ToInputOrEmpty(XPathItem item) {

         if (item == null)
            return EmptyIterator;

         return ToInput(item);
      }

      public static object ToInputOrEmpty(IXPathNavigable value) {
         return (value == null) ? EmptyIterator : (object)ToInput(value);
      }

      public static object ToInputOrEmpty(IEnumerable values) {

         IEnumerable<object> objects = values.Cast<object>();

         int count = (values == null) ? 0 : objects.Count();

         if (count == 0)
            return EmptyIterator;
         else if (count == 1)
            return ToInputOrEmpty(objects.First());
         else
            return ToInputOrEmpty(objects.Where(o => !IsEmpty(o)).Select(o => ToItem(o)));
      }

      public static object ToInputOrEmpty(IEnumerable<XPathItem> items) {

         int count = (items == null) ? 0 : items.Count();

         if (count == 0)
            return EmptyIterator;
         else if (count == 1)
            return ToInputOrEmpty(items.First());
         else
            return ToInput(items);
      }

      public static XPathItem ToItem(object value) {

         if (value == null) throw new ArgumentNullException("value");

         Type type = value.GetType();

         switch (Type.GetTypeCode(type)) {
            case TypeCode.Boolean:
               return ToItem((Boolean)value);
               
            case TypeCode.Byte:
               return ToItem((Byte)value);
               
            case TypeCode.Char:
               return ToItem((Char)value);
               
            case TypeCode.DateTime:
               return ToItem((DateTime)value);
               
            case TypeCode.Decimal:
               return ToItem((Decimal)value);
               
            case TypeCode.Double:
               return ToItem((Double)value);
               
            case TypeCode.Int16:
               return ToItem((Int16)value);
               
            case TypeCode.Int32:
               return ToItem((Int32)value);
               
            case TypeCode.Int64:
               return ToItem((Int64)value);
            
            case TypeCode.SByte:
               return ToItem((SByte)value);
               
            case TypeCode.Single:
               return ToItem((Single)value);
               
            case TypeCode.String:
               return ToItem((String)value);

            case TypeCode.UInt16:
               return ToItem((UInt16)value);
               
            case TypeCode.UInt32:
               return ToItem((UInt32)value);
               
            case TypeCode.UInt64:
               return ToItem((UInt64)value);

            case TypeCode.DBNull:
            case TypeCode.Empty:
               throw new ArgumentException("value cannot be a null type.", "value");

            case TypeCode.Object:
            default:

               XPathItem item = value as XPathItem;

               if (item != null)
                  return item;

               IXPathNavigable navigable = value as IXPathNavigable;

               if (navigable != null)
                  return navigable.CreateNavigator();

               return ToNode(value);
         }
      }

      public static XPathItem ToItem(Boolean value) {
         return SystemItemFactory.CreateBoolean(value);
      }

      public static XPathItem ToItem(Int16 value) {
         return SystemItemFactory.CreateDouble((Double)value);
      }

      public static XPathItem ToItem(Int32 value) {
         return SystemItemFactory.CreateDouble((Double)value);
      }

      public static XPathItem ToItem(Int64 value) {
         return SystemItemFactory.CreateDouble((Double)value);
      }

      public static XPathItem ToItem(Byte value) {
         return SystemItemFactory.CreateDouble((Double)value);
      }

      public static XPathItem ToItem(SByte value) {
         return SystemItemFactory.CreateDouble((Double)value);
      }

      public static XPathItem ToItem(UInt16 value) {
         return SystemItemFactory.CreateDouble((Double)value);
      }

      public static XPathItem ToItem(UInt32 value) {
         return SystemItemFactory.CreateDouble((Double)value);
      }

      public static XPathItem ToItem(UInt64 value) {
         return SystemItemFactory.CreateDouble((Double)value);
      }

      public static XPathItem ToItem(Decimal value) {
         return SystemItemFactory.CreateDouble((Double)value);
      }

      public static XPathItem ToItem(Single value) {
         return SystemItemFactory.CreateDouble((Double)value);
      }

      public static XPathItem ToItem(Double value) {
         return SystemItemFactory.CreateDouble(value);
      }

      public static XPathItem ToItem(DateTime value) {
         return SystemItemFactory.CreateDateTime(value);
      }

      public static XPathItem ToItem(Char value) {
         return SystemItemFactory.CreateString(value.ToString());
      }

      public static XPathItem ToItem(String value) {

         if (value == null) throw new ArgumentNullException("value");

         return SystemItemFactory.CreateString(value);
      }

      public static XPathNavigator ToNode(object value) {

         if (value == null) throw new ArgumentNullException("value");

         XPathItem item = value as XPathItem;

         if (item != null)
            return ToNode(item);

         var itemFactory = new SystemItemFactory();

         return itemFactory.CreateDocument(value).CreateNavigator();
      }

      public static XPathNavigator ToNode (XPathItem value)
		{

			if (value == null)
				throw new ArgumentNullException ("value");

			if (value.IsNode)
				return (XPathNavigator) value;

			return null;
         //return XsltConvert.ToNode(value);
      }

      public static IEnumerable<XPathItem> ToItems(XPathNodeIterator iterator) {
         return iterator.Cast<XPathItem>();
      }

      public static IEnumerable<XPathNavigator> ToNodes(XPathNodeIterator iterator) {
         return iterator.Cast<XPathNavigator>();
      }
   }
}
