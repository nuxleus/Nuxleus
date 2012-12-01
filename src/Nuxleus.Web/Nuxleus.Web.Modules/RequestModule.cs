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
using System.IO;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;

namespace Nuxleus.Web.Module {

   [XPathModule(Namespace)]
   public static class RequestModule {

      public const string Prefix = "request";
      public const string Namespace = "http://nuxleus.net/ns/request";

      static HttpContext Context {
         get { return HttpContext.Current; }
      }

      [XPathFunction("application-path", "xs:string")]
      public static string ApplicationPath() {
         return VirtualPathUtility.AppendTrailingSlash(Context.Request.ApplicationPath);
      }

      [XPathFunction("url", "xs:string", Description = "The absolute URL of the current HTTP request.")]
      public static string Url() {
         return Context.Request.Url.AbsoluteUri;
      }

      [XPathFunction("url-app-relative-path", "xs:string", Description = "The application relative path of the current HTTP request URL.")]
      public static string UrlAppRelativePath() {
         return Context.Request.Url.GetComponents(System.UriComponents.Path | System.UriComponents.KeepDelimiter, UriFormat.UriEscaped).Remove(0, ApplicationPath().Length);
      }

      [XPathFunction("url-app-relative-file-path", "xs:string", Description = "The application relative path of the current HTTP request URL, without the pathinfo part.")]
      public static string UrlAppRelativeFilePath() {

         string appRelativePathWithoutPathInfo = UrlAppRelativePath().Remove(0, UrlPathInfo().Length);

         string appRelativeFilePath = Context.Request.AppRelativeCurrentExecutionFilePath.Substring(2);

         if (appRelativeFilePath.Length < appRelativePathWithoutPathInfo.Length)
            return appRelativePathWithoutPathInfo;
         else
            return appRelativeFilePath;
      }

      [XPathFunction("url-path-info", "xs:string", Description = "The pathinfo part of the current HTTP request URL.")]
      public static string UrlPathInfo() {

         if (Context.Request.Path.EndsWith("/"))
            return "";

         return Context.Request.PathInfo;
      }

      [XPathFunction("url-components", "xs:string", "xs:string")]
      public static string UrlComponents(string components) {
         return UrlComponents(components, null);
      }

      [XPathFunction("url-components", "xs:string", "xs:string", "xs:string")]
      public static string UrlComponents(string components, string format) {

         System.UriComponents componentsEnum = (System.UriComponents)Enum.Parse(typeof(System.UriComponents), components, true);
         UriFormat formatEnum = (format == null) ? UriFormat.UriEscaped : (UriFormat)Enum.Parse(typeof(UriFormat), format, true);

         return Context.Request.Url.GetComponents(componentsEnum, formatEnum);
      }

      [XPathFunction("referrer-url", "xs:string?", Description = "The referrer URL of the current HTTP request URL.")]
      public static string ReferrerUrl() {

         Uri url = null;

         try {
            url = Context.Request.UrlReferrer;

         } catch (UriFormatException) { }

         return (url != null) ? url.AbsoluteUri : null;
      }

      [XPathFunction("referrer-url-components", "xs:string?", "xs:string")]
      public static string ReferrerUrlComponents(string components) {
         return ReferrerUrlComponents(components, null);
      }

      [XPathFunction("referrer-url-components", "xs:string?", "xs:string", "xs:string")]
      public static string ReferrerUrlComponents(string components, string format) {

         Uri referrerUrl = null;

         try {
            referrerUrl = Context.Request.UrlReferrer;

         } catch (UriFormatException) { }

         if (referrerUrl != null) {

            System.UriComponents componentsEnum = (System.UriComponents)Enum.Parse(typeof(System.UriComponents), components, true);
            UriFormat formatEnum = (format == null) ? UriFormat.UriEscaped : (UriFormat)Enum.Parse(typeof(UriFormat), format, true);

            return referrerUrl.GetComponents(componentsEnum, formatEnum);
         } else {
            return null;
         }
      }

      [XPathFunction("query-string", "xs:string*", "xs:string", Description = "The values of the querystring parameters of the current HTTP request URL, that matches the provided name.")]
      public static string[] QueryString(string name) {
         return Context.Request.QueryString.GetValues(name) ?? new string[0];
      }

      [XPathFunction("form", "xs:string*", "xs:string", Description = "The values of the form parameters of the current HTTP request, that matches the provided name.")]
      public static string[] Form(string name) {
         return Context.Request.Form.GetValues(name) ?? new string[0];
      }

      [XPathFunction("http-method", "xs:string", Description = "The HTTP method the current request.")]
      public static string HttpMethod() {
         return Context.Request.HttpMethod;
      }

      [XPathFunction("header", "xs:string?", "xs:string", Description = "The value of the HTTP header of the current request, that matches the provided name.")]
      public static string Header(string name) {
         return Context.Request.Headers.Get(name);
      }

      [XPathFunction("content-type", "xs:string?", Description = "The value of the Content-Type header of the current HTTP request.")]
      public static string ContentType() {
         return Context.Request.ContentType;
      }

      [XPathFunction("is-local", "xs:boolean")]
      public static bool IsLocal() {
         return Context.Request.IsLocal;
      }

      [XPathFunction("get-cookie", "xs:string?", "xs:string")]
      public static string GetCookie(string name) {

         HttpCookie cookie = Context.Request.Cookies.Get(name);

         if (cookie != null)
            return cookie.Value;
         else
            return null;
      }

      [XPathFunction("get-and-remove-cookie", "xs:string?", "xs:string")]
      public static string GetAndRemoveCookie(string name) {

         string val = GetCookie(name);
         ResponseModule.RemoveCookie(name);

         return val;
      }

      [XPathFunction("user-languages", "xs:string*")]
      public static string[] UserLanguages() {
         return Context.Request.UserLanguages ?? new string[0];
      }

      [XPathFunction("user-host-address", "xs:string")]
      public static string UserHostAddress() {
         return Context.Request.UserHostAddress;
      }

      [XPathFunction("user-host-name", "xs:string")]
      public static string UserHostName() {
         return Context.Request.UserHostName;
      }

      [XPathFunction("map-path", "xs:string", "xs:string")]
      public static string MapPath(string virtualPath) {
         return Context.Request.MapPath(virtualPath);
      }
   }
}
