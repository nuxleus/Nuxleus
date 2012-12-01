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
using System.Configuration;
using System.Text;
using System.Xml;
using Nuxleus.Web.UI.Compilation;

namespace Nuxleus.Web.Configuration{

   sealed class LibraryConfigSection : ConfigurationSection {

      static readonly ConfigurationPropertyCollection _Properties;
      static readonly ConfigurationProperty _ProcessorsProperty;
      static readonly ConfigurationProperty _ResolversProperty;
      static readonly ConfigurationProperty _ExpressionBuildersProperty;
      static readonly ConfigurationProperty _XsltProperty;
      static readonly ConfigurationProperty _XQueryProperty;

      static LibraryConfigSection _Instance;
      static readonly object padlock = new object();

      ProcessorElementCollection _Processors;
      ResolverElementCollection _Resolvers;
      ExpressionBuilderElementCollection _ExpressionBuilders;
      XsltElement _Xslt;
      XQueryElement _XQuery;

      public static LibraryConfigSection Instance {
         get {
            if (_Instance == null) {
               lock (padlock) {
                  if (_Instance == null) {
                     
                     LibraryConfigSection configSection = ConfigurationManager.GetSection("myxsl.net") as LibraryConfigSection;
                     
                     if (configSection == null) {
                        configSection = new LibraryConfigSection();
                        configSection.Init();
                        configSection.InitializeDefault();
                     }
                     _Instance = configSection;
                  }
               }
            }
            return _Instance;
         }
      }

      protected override ConfigurationPropertyCollection Properties {
         get { return _Properties; }
      }

      public ProcessorElementCollection Processors {
         get {
            if (_Processors == null)
               _Processors = (ProcessorElementCollection)base[_ProcessorsProperty];
            return _Processors;
         }
      }

      public ResolverElementCollection Resolvers {
         get {
            if (_Resolvers == null)
               _Resolvers = (ResolverElementCollection)base[_ResolversProperty];
            return _Resolvers;
         }
      }

      public ExpressionBuilderElementCollection ExpressionBuilders {
         get { 
            if (_ExpressionBuilders == null)
               _ExpressionBuilders = (ExpressionBuilderElementCollection)base[_ExpressionBuildersProperty];
            return _ExpressionBuilders;
         }
      }

      public XsltElement Xslt {
         get {
            if (_Xslt == null)
               _Xslt = (XsltElement)base[_XsltProperty];
            return _Xslt;
         }
      }

      public XQueryElement XQuery {
         get {
            if (_XQuery == null)
               _XQuery = (XQueryElement)base[_XQueryProperty];
            return _XQuery;
         }
      }

      static LibraryConfigSection() {
         
         _ProcessorsProperty = new ConfigurationProperty("processors", typeof(ProcessorElementCollection));
         _ResolversProperty = new ConfigurationProperty("resolvers", typeof(ResolverElementCollection));
         _ExpressionBuildersProperty = new ConfigurationProperty("expressionBuilders", typeof(ExpressionBuilderElementCollection));
         _XsltProperty = new ConfigurationProperty("xslt", typeof(XsltElement));
         _XQueryProperty = new ConfigurationProperty("xquery", typeof(XQueryElement));

         _Properties = new ConfigurationPropertyCollection { 
            _ProcessorsProperty, 
            _ResolversProperty, 
            _ExpressionBuildersProperty,
            _XsltProperty,
            _XQueryProperty
         };
      }

      private LibraryConfigSection() { }

      protected override void InitializeDefault() {

         ExpressionBuilderElementCollection exprBuilders = this.ExpressionBuilders;
         ResolverElementCollection resolvers = this.Resolvers;

         ProcessorElement sysProc = new ProcessorElement {
            Name = "system",
            Type = typeof(Nuxleus.Web.Sys.SystemXsltProcessor).AssemblyQualifiedName,
            LockItem = true
         };

         this.Processors.Add(sysProc);
         this.Xslt.DefaultProcessor = sysProc.Name;

         exprBuilders.Add(
            new ExpressionBuilderElement { 
               Namespace = RequestExpressionBuilder.Namespace,
               Type = typeof(RequestExpressionBuilder).AssemblyQualifiedName,
               LockItem = true
            }
         );

         exprBuilders.Add(
            new ExpressionBuilderElement {
               Namespace = SessionExpressionBuilder.Namespace,
               Type = typeof(SessionExpressionBuilder).AssemblyQualifiedName,
               LockItem = true
            }
         );

         exprBuilders.Add(
            new ExpressionBuilderElement { 
               Namespace = CodeExpressionBuilder.Namespace,
               Type = typeof(CodeExpressionBuilder).AssemblyQualifiedName,
               LockItem = true
            }
         );

         resolvers.Add(
            new ResolverElement { 
               Scheme = Uri.UriSchemeFile,
               Type = typeof(XmlVirtualPathAwareUrlResolver).AssemblyQualifiedName
            }
         );

         resolvers.Add(
             new ResolverElement { 
               Scheme = Uri.UriSchemeHttp,
               Type = typeof(XmlVirtualPathAwareUrlResolver).AssemblyQualifiedName
            }
         );

         resolvers.Add(
             new ResolverElement {
                Scheme = XmlEmbeddedResourceResolver.UriSchemeClires,
                Type = typeof(XmlEmbeddedResourceResolver).AssemblyQualifiedName
             }
         );

         base.InitializeDefault();
      }
   }
}
