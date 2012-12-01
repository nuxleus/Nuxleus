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
using System.Xml.XPath;
using System.Xml;
using System.Collections.Generic;

namespace Nuxleus.Web {

   public class SchematronXsltValidator {

      readonly XsltExecutable _Executable;

      public XsltExecutable Executable {
         get { return _Executable; } 
      }

      protected internal SchematronXsltValidator(XsltExecutable executable) {

         if (executable == null) throw new ArgumentNullException("executable");

         this._Executable = executable;
      }

      public IXPathNavigable Validate(IXPathNavigable source) {
         return Validate(source, null);
      }

      public IXPathNavigable Validate(IXPathNavigable source, string phase) {
         return Validate(source, null, null);
      }

      public IXPathNavigable Validate(IXPathNavigable source, string phase, IEnumerable<XPathNavigator> parameters) {

         XsltExecutable exec = this.Executable;

         if (exec == null)
            throw new InvalidOperationException("Executable cannot be null");

         XsltRuntimeOptions runtimeContext = new XsltRuntimeOptions {
            InitialContextNode = source
         };

         if (!String.IsNullOrEmpty(phase))
            runtimeContext.Parameters.Add(new XmlQualifiedName("phase"), phase);

         if (parameters != null) {
            foreach (XPathNavigator p in parameters)
               runtimeContext.Parameters.Add(new XmlQualifiedName(p.Name, p.NamespaceURI), p.TypedValue);
         }

         IXPathNavigable result = exec.Run(runtimeContext);

         return result;
      }
   }
}
