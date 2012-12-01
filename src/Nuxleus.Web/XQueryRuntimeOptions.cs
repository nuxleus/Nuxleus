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
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.XPath;

namespace Nuxleus.Web {

   public class XQueryRuntimeOptions {

      XmlSerializationOptions _Serialization;

      public object ContextItem { get; set; }
      public IDictionary<XmlQualifiedName, object> ExternalVariables { get; private set; }

      public XmlResolver InputXmlResolver { get; set; }

      public XmlSerializationOptions Serialization {
         get {
            if (_Serialization == null)
               _Serialization = new XmlSerializationOptions();
            return _Serialization;
         }
         set { _Serialization = value; }
      }

      public XQueryRuntimeOptions() {

         this.InputXmlResolver = new XmlDynamicResolver(Assembly.GetCallingAssembly());
         this.ExternalVariables = new Dictionary<XmlQualifiedName, object>();
      }
   }
}
