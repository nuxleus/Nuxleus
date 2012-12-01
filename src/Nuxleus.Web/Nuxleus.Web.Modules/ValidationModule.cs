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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.XPath;

namespace Nuxleus.Web.Module {
   
   [XPathModule(Namespace)]
   public static class ValidationModule {

      public const string Prefix = "validation";
      public const string Namespace = "http://nuxleus.net/ns/validation";

      [XPathFunction("validate-with-schematron", "document-node()", "xs:string", "node()")]
      public static XPathNavigator ValidateWithSchematron(string validatorUri, XPathNavigator source) {

         return GetValidator(validatorUri)
            .Validate(source)
            .CreateNavigator();
      }

      [XPathFunction("validate-with-schematron", "document-node()", "xs:string", "node()", "xs:string?")]
      public static XPathNavigator ValidateWithSchematron(string validatorUri, XPathNavigator source, string phase) {

         return GetValidator(validatorUri)
            .Validate(source, phase)
            .CreateNavigator();
      }

      [XPathFunction("validate-with-schematron", "document-node()", "xs:string", "node()", "xs:string?", "node()*")]
      public static XPathNavigator ValidateWithSchematron(string validatorUri, XPathNavigator source, string phase, IEnumerable<XPathNavigator> parameters) {

         return GetValidator(validatorUri)
            .Validate(source, phase, parameters)
            .CreateNavigator();
      }

      static SchematronXsltValidator GetValidator(string validatorUri) {

         Type type = TypeResolver.ResolveUri(new Uri(validatorUri));

         SchematronXsltValidator validator = (SchematronXsltValidator)Activator.CreateInstance(type);

         return validator;
      }
   }
}
