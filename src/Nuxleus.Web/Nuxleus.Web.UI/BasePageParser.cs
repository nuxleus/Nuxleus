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
using System.Linq;
using System.Text;
using System.Web.Configuration;
using System.Web.UI;
using System.Web;
using System.IO;
using System.Configuration;

namespace Nuxleus.Web.Page {
   
   public abstract class BasePageParser : BaseParser {

      PagesSection _PagesSection;

      PageParameterInfoCollection _Parameters;
      IList<string> _AcceptVerbs;
      IList<string> _SourceDependencies;
      IList<ParsedValue<string>> _Namespaces;

      // input state

      public PagesSection PagesSection {
         get {
            if (_PagesSection == null) 
               _PagesSection = ConfigurationManager.GetSection("system.web/pages") as PagesSection;
            return _PagesSection;
         }
      }

      // output state

      public bool ValidateRequest { get; set; }
      public PagesEnableSessionState EnableSessionState { get; set; }
      public string ContentType { get; set; }
      public OutputCacheParameters OutputCache { get; set; }

      public PageParameterInfoCollection Parameters {
         get {
            if (_Parameters == null)
               _Parameters = new PageParameterInfoCollection();
            return _Parameters;
         }
      }

      public IList<string> AcceptVerbs { 
         get { 
            if (_AcceptVerbs == null)
               _AcceptVerbs = new List<string>();
            return _AcceptVerbs;
         } 
      }

      public IList<string> SourceDependencies {
         get {
            if (_SourceDependencies == null) {
               if (String.IsNullOrEmpty(this.VirtualPath))
                  throw new InvalidOperationException("VirtualPath cannot be null or empty.");
               _SourceDependencies = new List<string> { this.VirtualPath };
            }
            return _SourceDependencies;
         }
      }

      public IList<ParsedValue<string>> Namespaces {
         get {
            if (_Namespaces == null) {
               _Namespaces = new List<ParsedValue<string>>();

               if (PagesSection != null) {
                  foreach (ParsedValue<string> item in PagesSection.Namespaces.Cast<NamespaceInfo>().Select(n => new ParsedValue<string>(n.Namespace, n.ElementInformation.Source, n.ElementInformation.LineNumber))) 
                     _Namespaces.Add(item);
               }
            }
            return _Namespaces;
         }
      }
   }
}
