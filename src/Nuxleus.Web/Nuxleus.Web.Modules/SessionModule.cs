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
   public static class SessionModule {

      public const string Prefix = "session";
      public const string Namespace = "http://nuxleus.net/ns/session";

      static HttpContext Context {
         get { return HttpContext.Current; }
      }

      [XPathFunction("get", "item()?", "xs:string")]
      public static object Get(string name) {
         return Context.Session[name];
      }

      [XPathFunction("get-and-remove", "item()?", "xs:string")]
      public static object GetAndRemove(string name) {

         object val = Get(name);
         Remove(name);

         return val;
      }

      [XPathFunction("set", "empty-sequence()", "xs:string", "item()")]
      public static void Set(string name, object value) {
         Context.Session[name] = value;
      }

      [XPathFunction("remove", "empty-sequence()", "xs:string")]
      public static void Remove(string name) {
         Context.Session.Remove(name);
      }

      [XPathFunction("remove-all", "empty-sequence()")]
      public static void RemoveAll() {
         Context.Session.RemoveAll();
      }
   }
}
