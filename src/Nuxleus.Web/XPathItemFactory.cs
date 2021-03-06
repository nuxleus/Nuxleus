﻿// Copyright 2010 Max Toro Q.
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
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace Nuxleus.Web {
   
   public abstract class XPathItemFactory {

      static readonly IDictionary<Type, XmlSerializer> serializerCache;
      static readonly object padlock;

      static XPathItemFactory() {

         serializerCache = new Dictionary<Type, XmlSerializer>();
         padlock = new object();
      }

      public XPathItem CreateAtomicValue(object value, XmlTypeCode typeCode) {
         return CreateAtomicValue(value, XmlSchemaType.GetBuiltInSimpleType(typeCode).QualifiedName);
      }

      public abstract XPathItem CreateAtomicValue(object value, XmlQualifiedName qualifiedName);

      public IEnumerable<XPathItem> CreateAtomicValueSequence(object value, XmlQualifiedName qualifiedName) {

         if (value == null)
            yield break;

         yield return CreateAtomicValue(value, qualifiedName);
      }

      public IEnumerable<XPathItem> CreateAtomicValueSequence(string value, XmlQualifiedName qualifiedName) {
         return CreateAtomicValueSequence(value as object, qualifiedName);
      }

      public IEnumerable<XPathItem> CreateAtomicValueSequence(IEnumerable value, XmlQualifiedName qualifiedName) {

         if (value == null)
            yield break;

         IEnumerator en = value.GetEnumerator();

         while (en.MoveNext()) {
            if (en.Current != null)
               yield return CreateAtomicValue(en.Current, qualifiedName);
         }
      }

      public IXPathNavigable CreateDocument(object value) {

         if (value == null)
            return null;

         IXPathNavigable inputNode = value as IXPathNavigable;

         if (inputNode == null) {

            XNode xNode = value as XNode;

            if (xNode != null) {
               inputNode = xNode.CreateNavigator();
            } else {

               IXmlSerializable xmlSer = value as IXmlSerializable;

               if (xmlSer != null) {
                  inputNode = CreateDocument(xmlSer);
               } else {

                  inputNode = CreateNodeEditable();

                  using (XmlWriter xmlWriter = inputNode.CreateNavigator().AppendChild()) {
                     XmlSerializer serializer = GetSerializer(value.GetType());
                     serializer.Serialize(xmlWriter, value);
                  }
               }
            }
         }

         return inputNode;
      }

      public IXPathNavigable CreateDocument(IXmlSerializable value) {

         if (value == null)
            return null;

         IXPathNavigable doc = CreateNodeEditable();

         using (XmlWriter xmlWriter = doc.CreateNavigator().AppendChild()) 
            value.WriteXml(xmlWriter);

         return doc;
      }

      public XPathNavigator CreateElement(object value) {

         IXPathNavigable doc = CreateDocument(value);

         if (doc == null)
            return null;

         return MoveToDocumentElement(doc);
      }

      public XPathNavigator CreateElement(IXmlSerializable value) {

         IXPathNavigable doc = CreateDocument(value);

         if (doc == null)
            return null;

         return MoveToDocumentElement(doc);
      }

      XPathNavigator MoveToDocumentElement(IXPathNavigable doc) {

         XPathNavigator nav = doc.CreateNavigator();
         nav.MoveToChild(XPathNodeType.Element);

         return nav;
      }

      XmlSerializer GetSerializer(Type type) {

         if (!serializerCache.ContainsKey(type)) {
            lock (padlock) {
               if (!serializerCache.ContainsKey(type)) {
                  serializerCache[type] = new XmlSerializer(type);
               }               
            }
         }

         return serializerCache[type];
      }

      public virtual IXPathNavigable CreateNodeReadOnly() {
         return CreateNodeReadOnly(XmlReader.Create(new StringReader(""), new XmlReaderSettings { ConformanceLevel = ConformanceLevel.Fragment }));      
      }

      public IXPathNavigable CreateNodeReadOnly(Stream input) {
         return CreateNodeReadOnly(input, null);
      }
      
      public abstract IXPathNavigable CreateNodeReadOnly(Stream input, XmlParsingOptions options);

      public IXPathNavigable CreateNodeReadOnly(TextReader input) {
         return CreateNodeReadOnly(input, null);
      }

      public abstract IXPathNavigable CreateNodeReadOnly(TextReader input, XmlParsingOptions options);

      public abstract IXPathNavigable CreateNodeReadOnly(XmlReader reader);

      public virtual IXPathNavigable CreateNodeEditable() {
         return new XmlDocument();
      }
      
      public IXPathNavigable CreateNodeEditable(Stream input) {
         return CreateNodeEditable(input, null);
      }

      public virtual IXPathNavigable CreateNodeEditable(Stream input, XmlParsingOptions options) {

         options = options ?? new XmlParsingOptions();

         XmlReaderSettings settings = (XmlReaderSettings)options;

         XmlReader reader;

         if (options.BaseUri != null)
            reader = XmlReader.Create(input, settings, options.BaseUri.AbsoluteUri);
         else
            reader = XmlReader.Create(input, settings);

         return CreateNodeEditable(reader);
      }

      public IXPathNavigable CreateNodeEditable(TextReader input) {
         return CreateNodeEditable(input, null);
      }

      public virtual IXPathNavigable CreateNodeEditable(TextReader input, XmlParsingOptions options) {
         
         options = options ?? new XmlParsingOptions();

         XmlReaderSettings settings = (XmlReaderSettings)options;

         XmlReader reader;

         if (options.BaseUri != null)
            reader = XmlReader.Create(input, settings, options.BaseUri.AbsoluteUri);
         else
            reader = XmlReader.Create(input, settings);

         return CreateNodeEditable(reader);
      }

      public virtual IXPathNavigable CreateNodeEditable(XmlReader reader) {
         
         XmlDocument document = new XmlDocument();
         document.Load(reader);

         return document;
      }
      
      public void Serialize(XPathItem item, Stream output) {
         Serialize(item, output, null);
      }

      public virtual void Serialize(XPathItem item, Stream output, XmlSerializationOptions options) {

         options = options ?? new XmlSerializationOptions();

         XmlWriter writer = XmlWriter.Create(output, (XmlWriterSettings)options);

         if (options.Method == XmlSerializationOptions.Methods.XHtml)
            writer = new XHtmlWriter(writer);

         Serialize(item, writer);

         writer.Flush();
      }

      public void Serialize(XPathItem item, TextWriter output) {
         Serialize(item, output, null);
      }

      public virtual void Serialize(XPathItem item, TextWriter output, XmlSerializationOptions options) {

         options = options ?? new XmlSerializationOptions();

         XmlWriter writer = XmlWriter.Create(output, (XmlWriterSettings)options);

         if (options.Method == XmlSerializationOptions.Methods.XHtml)
            writer = new XHtmlWriter(writer);

         Serialize(item, writer);

         writer.Flush();
      }

      public void Serialize(XPathItem item, XmlWriter output) {

         if (item == null) throw new ArgumentNullException("item");

         XPathNavigator node;

         if (item.IsNode) {
            node = (XPathNavigator)item;
         } else {
            node = CreateNodeReadOnly(new StringReader(item.Value), new XmlParsingOptions { ConformanceLevel = ConformanceLevel.Fragment })
               .CreateNavigator();
         }

         node.WriteSubtree(output);
      }
   }
}
