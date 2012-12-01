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
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.XPath;

namespace Nuxleus.Web {

   public static class Schematron {

      public static void BuildSchematronValidatorStylesheet(this IXsltProcessor processor, IXPathNavigable schemaDoc, XmlWriter output) {

         if (processor == null) throw new ArgumentNullException("processor");
         if (schemaDoc == null) throw new ArgumentNullException("schemaDoc");
         if (output == null) throw new ArgumentNullException("output");

         XPathNavigator nav = schemaDoc.CreateNavigator();
         nav.MoveToChild(XPathNodeType.Element);

         string queryBinding = nav.GetAttribute("queryBinding", "");

         string xsltVersion = String.IsNullOrEmpty(queryBinding)
            || queryBinding.Equals("xslt2", StringComparison.OrdinalIgnoreCase)
            || queryBinding.Equals("xpath2", StringComparison.OrdinalIgnoreCase) ? 
            "xslt2" : "xslt1";

         Assembly assembly = Assembly.GetExecutingAssembly();
         
         Uri baseUri = new UriBuilder {
            Scheme = XmlEmbeddedResourceResolver.UriSchemeClires,
            Host = null,
            Path = String.Concat(assembly.GetName().Name, "/schematron/", xsltVersion, "/")
         }.Uri;

         XsltCompileOptions compileOptions = new XsltCompileOptions();
         compileOptions.BaseUri = baseUri;

         string[] stages = { "iso_dsdl_include.xsl", "iso_abstract_expand.xsl", String.Concat("iso_svrl_for_", xsltVersion, ".xsl") };

         IXPathNavigable input = schemaDoc;

         for (int i = 0; i < stages.Length; i++) {

            Uri stageUri = new Uri(baseUri, stages[i]);

            using (Stream stageDoc = (Stream)compileOptions.XmlResolver.GetEntity(stageUri, null, typeof(Stream))) {

               XsltExecutable executable = processor.Compile(stageDoc, compileOptions);

               XsltRuntimeOptions runtimeOptions = new XsltRuntimeOptions {
                  InitialContextNode = input
               };

               if (i < stages.Length - 1) {
                  // output becomes the input for the next stage
                  input = executable.Run(runtimeOptions);
               } else {
                  // last stage is output to writer
                  executable.Run(output, runtimeOptions);
               }
            }
         }
      }

      public static SchematronXsltValidator CreateSchematronValidator(this IXsltProcessor processor, IXPathNavigable schemaDoc) {

         if (processor == null) throw new ArgumentNullException("processor");
         if (schemaDoc == null) throw new ArgumentNullException("schemaDoc");

         IXPathNavigable stylesheetDoc = processor.ItemFactory.CreateNodeEditable();

         using (XmlWriter builder = stylesheetDoc.CreateNavigator().AppendChild()) 
            processor.BuildSchematronValidatorStylesheet(schemaDoc, builder);

         XsltCompileOptions compileOptions = new XsltCompileOptions();

         XPathNavigator schemaNav = schemaDoc.CreateNavigator();

         if (!String.IsNullOrEmpty(schemaNav.BaseURI))
            compileOptions.BaseUri = new Uri(schemaNav.BaseURI);

         return new SchematronXsltValidator(processor.Compile(stylesheetDoc, compileOptions));
      }
   }
}
