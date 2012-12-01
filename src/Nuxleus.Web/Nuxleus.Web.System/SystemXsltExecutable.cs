// Copyright 2009 Max Toro Q.
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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using Nuxleus.Web.Module;

namespace Nuxleus.Web.Sys {

   using MonoTransform = Action<XslCompiledTransform, XPathNavigator, XsltArgumentList, XmlWriter, XmlResolver>;
   using Net20Transform = Action<object, IXPathNavigable, XmlResolver, XsltArgumentList, XmlWriter>;
   
   public sealed class SystemXsltExecutable : XsltExecutable {

      static readonly MonoTransform monoTransform;
      static readonly Net20Transform net20Transform;
      static readonly FieldInfo commandField;
      static readonly object padlock = new object();
      static IDictionary<string, Type> _ExtensionObjects;
      readonly object command;

      readonly XslCompiledTransform transform;
      readonly SystemXsltProcessor _Processor;
      readonly Uri _StaticBaseUri;
      readonly bool possiblyXhtmlMethod;

      static IDictionary<string, Type> ExtensionObjects {
			get {
				if (_ExtensionObjects == null) {
					lock (padlock) {
						if (_ExtensionObjects == null) {

							_ExtensionObjects =
                        (from pair in
                            (new[] { 
                           new { 
                              Type = ExtensionObjectGenerator.RenameMethodsIfNecessary (typeof(Modules.RequestModuleExtensionObject)),
                              Namespace = RequestModule.Namespace
                           },
                           new { 
                              Type = ExtensionObjectGenerator.RenameMethodsIfNecessary (typeof(Modules.ResponseModuleExtensionObject)),
                              Namespace = ResponseModule.Namespace
                           },
                           new { 
                              Type = ExtensionObjectGenerator.RenameMethodsIfNecessary (typeof(Modules.SessionModuleExtensionObject)),
                              Namespace = SessionModule.Namespace
                           },
                           new { 
                              Type = ExtensionObjectGenerator.RenameMethodsIfNecessary (typeof(Modules.UtilModuleExtensionObject)),
                              Namespace = UtilModule.Namespace
                           },
                           new { 
                              Type = ExtensionObjectGenerator.RenameMethodsIfNecessary (typeof(Modules.ValidationModuleExtensionObject)),
                              Namespace = ValidationModule.Namespace
                           },
                           new { 
                              Type = ExtensionObjectGenerator.RenameMethodsIfNecessary (typeof(Modules.HttpClientExtensionObject)),
                              Namespace = Nuxleus.Web.Module.EXPath.HttpClient.XPathHttpClient.Namespace
                           },
                           new { 
                              Type = ExtensionObjectGenerator.RenameMethodsIfNecessary (typeof(Modules.SmtpClientExtensionObject)),
                              Namespace = Nuxleus.Web.Module.SmtpClient.XPathSmtpClient.Namespace
                           },
                           new {
                              Type = ExtensionObjectGenerator.RenameMethodsIfNecessary (typeof(Modules.XPathFunctionsExtensionObject)),
                              Namespace = "http://www.w3.org/2005/xpath-functions"
                           }
                         }).Concat(
                               from a in XPathModules.GetModuleAdaptersForProcessor(typeof(SystemXsltProcessor))
                               where a.Module != null
                               select new { Type = ExtensionObjectGenerator.RenameMethodsIfNecessary(a.AdapterType), Namespace = a.Module.Namespace }
                               )
                         select pair).ToDictionary(p => p.Namespace, p => p.Type);
                  }
               }
            }

            return _ExtensionObjects;
         }
      }

      public new SystemXsltProcessor Processor { get { return _Processor; } }
      protected override IXsltProcessor XsltProcessor { get { return this.Processor; } }

      public override Uri StaticBaseUri { get { return _StaticBaseUri; } }

      static SystemXsltExecutable() {

         if (CLR.IsMono) {
            monoTransform = (MonoTransform)Delegate.CreateDelegate(typeof(MonoTransform), typeof(XslCompiledTransform).GetMethod("Transform", BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[] { typeof(XPathNavigator), typeof(XsltArgumentList), typeof(XmlWriter), typeof(XmlResolver) }, null));
         } else {

            commandField = typeof(XslCompiledTransform).GetField("command", BindingFlags.Instance | BindingFlags.NonPublic);

            ParameterExpression commandParam = Expression.Parameter(typeof(object), "instance");
            ParameterExpression inputParam = Expression.Parameter(typeof(IXPathNavigable), "input");
            ParameterExpression resolverParam = Expression.Parameter(typeof(XmlResolver), "resolver");
            ParameterExpression argumentsParam = Expression.Parameter(typeof(XsltArgumentList), "arguments");
            ParameterExpression writerParam = Expression.Parameter(typeof(XmlWriter), "writer");

            MethodInfo executeMethod = commandField.FieldType.GetMethod("Execute", BindingFlags.Instance | BindingFlags.Public, null, new Type[] { typeof(IXPathNavigable), typeof(XmlResolver), typeof(XsltArgumentList), typeof(XmlWriter) }, null);
            MethodCallExpression executeMethodExpr = Expression.Call(Expression.Convert(commandParam, commandField.FieldType), executeMethod, inputParam, resolverParam, argumentsParam, writerParam);
            net20Transform = Expression.Lambda<Net20Transform>(executeMethodExpr, commandParam, inputParam, resolverParam, argumentsParam, writerParam).Compile();
         }
      }

      internal SystemXsltExecutable(XslCompiledTransform transform, SystemXsltProcessor processor, Uri staticBaseUri) {

         this.transform = transform;
         this._Processor = processor;
         this._StaticBaseUri = staticBaseUri;

         this.possiblyXhtmlMethod = this.transform.OutputSettings != null &&
            this.transform.OutputSettings.OutputMethod == XmlOutputMethod.AutoDetect;

         if (!CLR.IsMono)
            this.command = commandField.GetValue(this.transform);
      }

      public override void Run(Stream output, XsltRuntimeOptions options) {

         if (output == null) throw new ArgumentNullException("output");
         if (options == null) throw new ArgumentNullException("options");

         XmlWriterSettings settings = this.transform.OutputSettings;
         options.Serialization.CopyTo(settings);

         XmlWriter writer = XmlWriter.Create(output, settings);

         Run(writer, options);
      }

      public override void Run(TextWriter output, XsltRuntimeOptions options) {

         if (output == null) throw new ArgumentNullException("output");
         if (options == null) throw new ArgumentNullException("options");

         XmlWriterSettings settings = this.transform.OutputSettings;
         options.Serialization.CopyTo(settings);

         XmlWriter writer = XmlWriter.Create(output, settings);

         Run(writer, options);
      }

      public override void Run(XmlWriter output, XsltRuntimeOptions options) {

         if (output == null) throw new ArgumentNullException("output");
         if (options == null) throw new ArgumentNullException("options");

         if (this.possiblyXhtmlMethod || options.Serialization.Method == XmlSerializationOptions.Methods.XHtml)
            output = new XHtmlWriter(output);

         IXPathNavigable input;

         if (options.InitialContextNode != null)
            input = options.InitialContextNode;

         else
            // this processor doesn't support initial template,
            // a node must be provided
            input = this.Processor.ItemFactory.CreateNodeReadOnly();

         XsltArgumentList args = GetArguments(options);
         XmlResolver resolver = options.InputXmlResolver;

         try {
            if (CLR.IsMono) 
               monoTransform(this.transform, ((input != null) ? input.CreateNavigator() : null), args, output, resolver);
            else 
               net20Transform(this.command, input, resolver, args, output);

         } catch (XsltException ex) {
            throw new SystemXsltException(ex);
         }
      }

      public override IXPathNavigable Run(XsltRuntimeOptions options) {

         IXPathNavigable doc = this.Processor.ItemFactory.CreateNodeEditable();

         using (XmlWriter writer = doc.CreateNavigator().AppendChild())
            Run(writer, options);

         return doc;
      }

      XsltArgumentList GetArguments(XsltRuntimeOptions options) {

         XsltArgumentList list = new XsltArgumentList();

         foreach (var item in options.Parameters) {
            object val = ExtensionObjectConvert.ToInputOrEmpty(item.Value);

            if (!ExtensionObjectConvert.IsEmpty(val))
               list.AddParam(item.Key.Name, item.Key.Namespace, val);
         }

         foreach (var pair in ExtensionObjects)
            list.AddExtensionObject(pair.Key, Activator.CreateInstance(pair.Value));

         list.XsltMessageEncountered += new XsltMessageEncounteredEventHandler(args_XsltMessageEncountered);

         return list;
      }

      void args_XsltMessageEncountered(object sender, XsltMessageEncounteredEventArgs e) {
         Trace.WriteLine(e.Message);
      }
   }
}
