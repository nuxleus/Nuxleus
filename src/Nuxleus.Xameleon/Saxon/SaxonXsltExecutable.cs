﻿// Copyright 2009 Max Toro Q.
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
using System.IO;
using System.Xml;
using System.Xml.XPath;
using Saxon.Api;

namespace myxsl.net.saxon {

   public sealed class SaxonXsltExecutable : XsltExecutable {

      readonly Saxon.Api.XsltExecutable executable;
      readonly SaxonProcessor _Processor;
      readonly Uri _StaticBaseUri;

      public new SaxonProcessor Processor { get { return this._Processor; } }
      protected override IXsltProcessor XsltProcessor { get { return this.Processor; } }

      public override Uri StaticBaseUri { get { return _StaticBaseUri; } }

      internal SaxonXsltExecutable(Saxon.Api.XsltExecutable executable, SaxonProcessor processor, Uri staticBaseUri) {
         
         this.executable = executable;
         this._Processor = processor;
         this._StaticBaseUri = staticBaseUri;
      }

      public override void Run(Stream output, XsltRuntimeOptions options) {

         if (options == null) throw new ArgumentNullException("options");

         Serializer serializer = this.Processor.ItemFactory.CreateSerializer(options.Serialization);
         serializer.SetOutputStream(output);

         Run(serializer, options);
      }

      public override void Run(TextWriter output, XsltRuntimeOptions options) {

         if (options == null) throw new ArgumentNullException("options");

         Serializer serializer = this.Processor.ItemFactory.CreateSerializer(options.Serialization);
         serializer.SetOutputWriter(output);

         Run(serializer, options);
      }

      public override void Run(XmlWriter output, XsltRuntimeOptions options) {

         XmlDestination builder = new TextWriterDestination(output);

         Run(builder, options);
      }

      public override IXPathNavigable Run(XsltRuntimeOptions options) {

         XdmDestination xdmDest = new XdmDestination();
         
         Run(xdmDest, options);

         return xdmDest.XdmNode.ToXPathNavigable();
      }

      void Run(XmlDestination destination, XsltRuntimeOptions options) {

         if (options == null) throw new ArgumentNullException("options");

         XsltTransformer transformer = executable.Load();
         transformer.RecoveryPolicy = RecoveryPolicy.DoNotRecover;

         if (options.InputXmlResolver != null)
            transformer.InputXmlResolver = options.InputXmlResolver;

         // XsltTransformer.BaseOutputUri doesn't accept null
         if (options.BaseOutputUri != null)
            transformer.BaseOutputUri = options.BaseOutputUri;

         // TODO: Bug in Saxon 9.3
         //else if (this.StaticBaseUri != null && this.StaticBaseUri.IsFile)
         //   transformer.BaseOutputUri = new Uri(Path.GetDirectoryName(this.StaticBaseUri.LocalPath), UriKind.Absolute);

         try {
            if (options.InitialTemplate != null)
               transformer.InitialTemplate = new QName(options.InitialTemplate);

         } catch (DynamicError err) {
            throw new SaxonException(err);
         }

         if (options.InitialMode != null)
            transformer.InitialMode = new QName(options.InitialMode);

         if (options.InitialContextNode != null) 
            transformer.InitialContextNode = options.InitialContextNode.ToXdmNode(this.Processor.ItemFactory);

         transformer.MessageListener = new SaxonMessageListener();

         foreach (var pair in options.Parameters) {
            
            QName qname = new QName(pair.Key);
            XdmValue xdmValue = pair.Value.ToXdmValue(this.Processor.ItemFactory);
         
            transformer.SetParameter(qname, xdmValue);
         }

         try {
            transformer.Run(destination);

         } catch (DynamicError ex) {
            throw new SaxonException(ex);

         } catch (Exception ex) {
            throw new SaxonException(ex.Message, ex);
         }
      }
   }
}
