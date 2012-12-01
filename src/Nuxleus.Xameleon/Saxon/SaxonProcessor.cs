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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.XPath;
using Saxon.Api;

namespace myxsl.net.saxon {

   public sealed class SaxonProcessor : IXsltProcessor, IXQueryProcessor {

      readonly Processor processor;
      readonly SaxonItemFactory _ItemFactory;

      public SaxonItemFactory ItemFactory { get { return _ItemFactory; } }
      XPathItemFactory IXsltProcessor.ItemFactory { get { return this.ItemFactory; } }
      XPathItemFactory IXQueryProcessor.ItemFactory { get { return this.ItemFactory; } }

      public SaxonProcessor() {

         processor = new Processor();
         _ItemFactory = new SaxonItemFactory(processor);
         RegisterExtensionFunctions(processor, _ItemFactory);
      }

      void RegisterExtensionFunctions(Processor processor, SaxonItemFactory itemFactory) {

         IEnumerable<IEnumerable<Type>> builtInFunctions = 
            new IEnumerable<Type>[] { 
               new modules.exslt.common.Index(),
               new modules.request.Index(),
               new modules.response.Index(),
               new modules.session.Index(),
               new modules.util.Index(),
               new modules.validation.Index(),
               new modules.smtpclient.Index(),
               new modules.expath.httpclient.Index(),
            };

         IEnumerable<IEnumerable<Type>> importedFunctions =
            (from m in XPathModules.GetModuleAdaptersForProcessor(this.GetType())
             let t = m.AdapterType
             let isCollection = typeof(IEnumerable<Type>).IsAssignableFrom(t)
             select (isCollection) ? 
               (IEnumerable<Type>)Activator.CreateInstance(t)
               : new Type[] { t });

         Type itemFactoryType = itemFactory.GetType();

         foreach (var types in Enumerable.Concat(builtInFunctions, importedFunctions)) {

            var functions =
               from t in types
               let ctor = t.GetConstructors().First()
               let param = ctor.GetParameters()
               let args = param.Select(p => p.ParameterType.IsAssignableFrom(itemFactoryType) ? (object)itemFactory : null).ToArray()
               select (ExtensionFunctionDefinition)ctor.Invoke(args);

            if (functions.Select(f => f.FunctionName.Uri).Distinct().Count() > 1) 
               throw new InvalidOperationException("Functions in module must belog to the same namespace.");

            foreach (var fn in functions)
               processor.RegisterExtensionFunction(fn);
         }
      }

      XsltCompiler CreateCompiler(XsltCompileOptions options) {

         XsltCompiler compiler = this.processor.NewXsltCompiler();
         compiler.ErrorList = new ArrayList();
         compiler.BaseUri = options.BaseUri;

         if (options.XmlResolver != null)
            compiler.XmlResolver = options.XmlResolver;

         return compiler;
      }

      public XsltExecutable Compile(Stream module, XsltCompileOptions options) {

         XsltCompiler compiler = CreateCompiler(options);

         try {
            return WrapExecutable(compiler.Compile(module), options, default(Uri));
         } catch (Exception ex) {
            throw WrapCompileException(ex, compiler);
         }
      }

      public XsltExecutable Compile(TextReader module, XsltCompileOptions options) {

         XsltCompiler compiler = CreateCompiler(options);

         try {
            return WrapExecutable(compiler.Compile(module), options, default(Uri));
         } catch (Exception ex) {
            throw WrapCompileException(ex, compiler);
         }
      }

      public XsltExecutable Compile(XmlReader module, XsltCompileOptions options) {

         XsltCompiler compiler = CreateCompiler(options);

         string baseUri = module.BaseURI;

         try {
            return WrapExecutable(compiler.Compile(module), options, baseUri);
         } catch (Exception ex) {
            throw WrapCompileException(ex, compiler);
         }
      }

      public XsltExecutable Compile(IXPathNavigable module, XsltCompileOptions options) {

         XsltCompiler compiler = CreateCompiler(options);

         XPathNavigator nav = module.CreateNavigator();
         XdmNode node;

         if (SaxonExtensions.TryGetXdmNode(nav, out node)) {
            Uri baseUri = node.BaseUri;
            
            try {
               return WrapExecutable(compiler.Compile(node), options, baseUri);
            } catch (Exception ex) {
               throw WrapCompileException(ex, compiler);
            }
         } else {
            return Compile(nav.ReadSubtree(), options);
         }
      }

      ProcessorException WrapCompileException(Exception ex, XsltCompiler compiler) {
         return WrapCompileException(ex, compiler.ErrorList);
      }

      XsltExecutable WrapExecutable(Saxon.Api.XsltExecutable xsltExecutable, XsltCompileOptions options, string baseUri) {

         Uri parsedBaseUri = null;

         if (!String.IsNullOrEmpty(baseUri)) {
            try {
               parsedBaseUri = new Uri(baseUri);
            } catch (UriFormatException) { }
         }

         return WrapExecutable(xsltExecutable, options, parsedBaseUri);
      }

      XsltExecutable WrapExecutable(Saxon.Api.XsltExecutable xsltExecutable, XsltCompileOptions options, Uri baseUri) {
         return new SaxonXsltExecutable(xsltExecutable, this, baseUri ?? options.BaseUri);
      }

      XQueryCompiler CreateCompiler(XQueryCompileOptions options) {

         XQueryCompiler compiler = this.processor.NewXQueryCompiler();
         compiler.ErrorList = new ArrayList();
         compiler.BaseUri = options.BaseUri.AbsoluteUri;

         compiler.DeclareNamespace(RequestModule.Prefix, RequestModule.Namespace);
         compiler.DeclareNamespace(ResponseModule.Prefix, ResponseModule.Namespace);
         compiler.DeclareNamespace(SessionModule.Prefix, SessionModule.Namespace);
         compiler.DeclareNamespace(SecurityModule.Prefix, SecurityModule.Namespace);
         compiler.DeclareNamespace(ValidationModule.Prefix, ValidationModule.Namespace);
         compiler.DeclareNamespace(UtilModule.Prefix, UtilModule.Namespace);

         return compiler;
      }

      public XQueryExecutable Compile(Stream module, XQueryCompileOptions options) {

         XQueryCompiler compiler = CreateCompiler(options);

         try {
            return WrapExecutable(compiler.Compile(module), options);
         } catch (Exception ex) {
            throw WrapCompileException(ex, compiler);
         }
      }

      public XQueryExecutable Compile(TextReader module, XQueryCompileOptions options) {

         XQueryCompiler compiler = CreateCompiler(options);

         try {
            return WrapExecutable(compiler.Compile(module.ReadToEnd()), options);
         } catch (Exception ex) {
            throw WrapCompileException(ex, compiler);
         }
      }
      
      XQueryExecutable WrapExecutable(Saxon.Api.XQueryExecutable xqueryExecutable, XQueryCompileOptions options) {
         return new SaxonXQueryExecutable(xqueryExecutable, this, options.BaseUri);
      }

      ProcessorException WrapCompileException(Exception ex, XQueryCompiler compiler) {
         return WrapCompileException(ex, compiler.ErrorList);
      }

      ProcessorException WrapCompileException(Exception ex, IList errors) {

         StaticError err =
            errors.OfType<StaticError>().FirstOrDefault(s => !s.IsWarning)
            ?? ex as StaticError;
         
         if (err != null)
            return new SaxonException(err);

         return new SaxonException(ex.Message, ex);
      }
   }
}
