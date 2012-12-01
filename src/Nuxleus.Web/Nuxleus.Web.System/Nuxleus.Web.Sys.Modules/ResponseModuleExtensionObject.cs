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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.XPath;
using Nuxleus.Web.Module;

namespace Nuxleus.Web.Sys.Modules  {
   
   public class ResponseModuleExtensionObject {

      public XPathNodeIterator redirect(string url) {

         ResponseModule.Redirect(url);
         return ExtensionObjectConvert.EmptyIterator;
      }

      protected XPathNodeIterator set_response_header(string name, string value) {

         ResponseModule.SetHeader(name, value);
         return ExtensionObjectConvert.EmptyIterator;
      }

      protected XPathNodeIterator set_response_content_type(string value) {

         ResponseModule.SetContentType(value);
         return ExtensionObjectConvert.EmptyIterator;
      }

      protected XPathNodeIterator set_response_status(long code, string description) {

         ResponseModule.SetStatus(code, description);
         return ExtensionObjectConvert.EmptyIterator;
      }
   }
}
