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
   
   public class UtilModuleExtensionObject {

      protected object app_settings(string name) {
         return ExtensionObjectConvert.ToInputOrEmpty(UtilModule.AppSettings(name));
      }

      protected string absolute_path(string applicationRelativePath) {
         return UtilModule.AbsolutePath(applicationRelativePath);
      }
   }
}
