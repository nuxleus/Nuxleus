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
using System.Web;

namespace Nuxleus.Web {
   
   [XPathModule(Namespace)]
   public static class ResponseModule {
      
      public const string Prefix = "response";
      public const string Namespace = "http://nuxleus.net/ns/response";

      static HttpContext Context {
         get { return HttpContext.Current; }
      }

      [XPathFunction("redirect", "empty-sequence()", "xs:string")]
      public static void Redirect(string url) {
         Context.Response.Redirect(url, false);
      }

      [XPathFunction("set-header", "empty-sequence()", "xs:string", "xs:string")]
      public static void SetHeader(string name, string value) {
         Context.Response.Headers.Set(name, value);
      }

      [XPathFunction("set-content-type", "empty-sequence()", "xs:string")]
      public static void SetContentType(string value) {
         Context.Response.ContentType = value;
      }

      [XPathFunction("set-status", "empty-sequence()", "xs:integer", "xs:string")]
      public static void SetStatus(long code, string description) {

         HttpResponse response = Context.Response;

         response.StatusCode = (int)code;
         response.StatusDescription = description;
      }

      [XPathFunction("set-cookie", "empty-sequence()", "xs:string", "xs:string")]
      public static void SetCookie(string name, string value) {
         Context.Response.Cookies.Set(new HttpCookie(name, value));
      }

      [XPathFunction("remove-cookie", "empty-sequence()", "xs:string")]
      public static void RemoveCookie(string name) {
         Context.Response.Cookies.Remove(name);
      }
   }
}
