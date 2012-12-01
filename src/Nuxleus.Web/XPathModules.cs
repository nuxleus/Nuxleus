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
using System.Reflection;
using System.Web.Compilation;
using System.Web.Hosting;
using System.Collections.ObjectModel;

namespace Nuxleus.Web {

   public static class XPathModules {

      static IDictionary<Type, ReadOnlyCollection<XPathModuleAdapterInfo>> _Cache;
      static readonly object padlock = new object();

      static IDictionary<Type, ReadOnlyCollection<XPathModuleAdapterInfo>> Cache {
         get {
            if (_Cache == null) {
               lock (padlock) {
                  if (_Cache == null) {

                     IList<Assembly> assemblies = (HostingEnvironment.IsHosted) ?
                        BuildManager.GetReferencedAssemblies().Cast<Assembly>().ToArray() :
                        AppDomain.CurrentDomain.GetAssemblies();

                     var adap = (from assembly in assemblies
                                 where Attribute.IsDefined(assembly, typeof(XPathModuleAdapterExportAttribute))
                                 from adapterAttr in Attribute.GetCustomAttributes(assembly, typeof(XPathModuleAdapterExportAttribute)).Cast<XPathModuleAdapterExportAttribute>()
                                 where adapterAttr.AdapterType != null && adapterAttr.ProcessorType != null
                                 select adapterAttr).ToList();

                     var mod = (from a in adap
                                where a.ModuleType != null
                                group a by a.ModuleType into g
                                select GetModule(g.Key)).ToList();

                     _Cache = (from a in adap
                               group a by a.ProcessorType)
                              .ToDictionary(g => g.Key, g => new ReadOnlyCollection<XPathModuleAdapterInfo>(
                                 g.Select(a => new XPathModuleAdapterInfo { 
                                    AdapterType = a.AdapterType, 
                                    Module = (a.ModuleType != null ? mod.Find(m => m.Type == a.ModuleType) : null) }).ToList()
                              ));
                  } 
               }
            }
            return _Cache;
         }
      }

      static XPathModuleInfo GetModule(Type moduleType) {

         if (moduleType == null) throw new ArgumentNullException("moduleType");

         XPathModuleAttribute attr = 
            Attribute.GetCustomAttribute(moduleType, typeof(XPathModuleAttribute)) as XPathModuleAttribute;

         string ns = (attr != null) ? attr.Namespace : "clitype:" + moduleType.FullName;

         return new XPathModuleInfo { 
            Namespace = ns,
            Type = moduleType
         };
      }

      public static IEnumerable<XPathModuleAdapterInfo> GetModuleAdaptersForProcessor(Type processorType) {

         if (processorType == null) throw new ArgumentNullException("processorType");

         var cache = Cache;

         if (!cache.ContainsKey(processorType))
            return Enumerable.Empty<XPathModuleAdapterInfo>();

         return cache[processorType].AsEnumerable();
      }
   }
}
