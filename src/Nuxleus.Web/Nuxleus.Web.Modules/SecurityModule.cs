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

namespace Nuxleus.Web.Module {
   
   [XPathModule(Namespace)]
   public static class SecurityModule {

      public const string Prefix = "security";
      public const string Namespace = "http://nuxleus.net/ns/security";

      static HttpContext Context {
         get { return HttpContext.Current; }
      }

      [XPathFunction("user-name", "xs:string")]
      public static string UserName() {
         return Context.User.Identity.Name;
      }

      [XPathFunction("user-is-authenticated", "xs:boolean")]
      public static bool UserIsAuthenticated() {
         return Context.User.Identity.IsAuthenticated;
      }

      [XPathFunction("user-is-in-role", "xs:boolean", "xs:string")]
      public static bool UserIsInRole(string role) {
         return Context.User.IsInRole(role);
      }
   }
}
