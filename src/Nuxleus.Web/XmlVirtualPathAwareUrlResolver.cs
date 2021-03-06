﻿// Copyright 2009 Max Toro Q.
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
using System.Text;
using System.Xml;
using System.Web.Hosting;
using System.Text.RegularExpressions;
using System.Web;

namespace Nuxleus.Web {
   
   public class XmlVirtualPathAwareUrlResolver : XmlUrlResolver {

      static readonly Uri ApplicationBaseUri;

      static XmlVirtualPathAwareUrlResolver() {

         if (HostingEnvironment.IsHosted) 
            ApplicationBaseUri = new Uri(HostingEnvironment.ApplicationPhysicalPath, UriKind.Absolute);
      }

      public override Uri ResolveUri(Uri baseUri, string relativeUri) {

         if (HostingEnvironment.IsHosted) {

            bool baseUriIsInApp;

            if (baseUri == null || !baseUri.IsAbsoluteUri) {
               baseUri = ApplicationBaseUri;
               baseUriIsInApp = true;
            } else {
               Uri baseDiff = ApplicationBaseUri.MakeRelativeUri(baseUri);
               baseUriIsInApp = !baseDiff.IsAbsoluteUri;
            }

            Uri relUri = (!String.IsNullOrEmpty(relativeUri)) ?
               new Uri(relativeUri, UriKind.RelativeOrAbsolute) :
               null;

            if (relUri != null && !relUri.IsAbsoluteUri && baseUriIsInApp) {

               if (VirtualPathUtility.IsAbsolute(relUri.OriginalString) || VirtualPathUtility.IsAppRelative(relUri.OriginalString)) 
                  return new Uri(HostingEnvironment.MapPath(relUri.OriginalString), UriKind.Absolute);
            }
         }

         return base.ResolveUri(baseUri, relativeUri);
      }
   }
}
