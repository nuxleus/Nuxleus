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
using System.Xml.XPath;
using System.Linq;
using Nuxleus.Web.Sys;
using Nuxleus.Web.Module.EXPath.HttpClient;

namespace Nuxleus.Web.Sys.Modules  {

   public class HttpClientExtensionObject {

      readonly XPathHttpClient client = new XPathHttpClient();

      protected XPathNavigator[] send_request(XPathNavigator request) {
         return ExtensionObjectConvert.ToInput(this.client.SendRequest(request));
      }

      protected XPathNavigator[] send_request(XPathNavigator request, string href) {
         return ExtensionObjectConvert.ToInput(this.client.SendRequest(request, href));
      }

      protected XPathNavigator[] send_request(XPathNavigator request, string href, XPathNodeIterator bodies) {
         return ExtensionObjectConvert.ToInput(this.client.SendRequest(request, href, ExtensionObjectConvert.ToItems(bodies)));
      }
   }
}
