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
using Nuxleus.Web.Module;

namespace Nuxleus.Web.Sys.Modules  {

   public class RequestModuleExtensionObject {

      public string url() {
         return RequestModule.Url();
      }

      protected string url_app_relative_path() {
         return RequestModule.UrlAppRelativePath();
      }

      protected string url_app_relative_file_path() {
         return RequestModule.UrlAppRelativeFilePath();
      }

      protected string url_path_info() {
         return RequestModule.UrlPathInfo();
      }

      protected string url_components(string components) {
         return RequestModule.UrlComponents(components);
      }

      protected string url_components(string components, string format) {
         return RequestModule.UrlComponents(components, format);
      }

      protected object referrer_url() {
         return ExtensionObjectConvert.ToInputOrEmpty(RequestModule.ReferrerUrl());
      }

      protected object query_string(string name) {
         return ExtensionObjectConvert.ToInputOrEmpty(RequestModule.QueryString(name));
      }

      public object form(string name) {
         return ExtensionObjectConvert.ToInputOrEmpty(RequestModule.Form(name));
      }

      protected string http_method() {
         return RequestModule.HttpMethod();
      }

      public object header(string name) {
         return ExtensionObjectConvert.ToInputOrEmpty(RequestModule.Header(name));
      }

      protected object content_type() {
         return ExtensionObjectConvert.ToInputOrEmpty(RequestModule.ContentType());
      }

      protected bool is_local() {
         return RequestModule.IsLocal();
      }
   }
}
