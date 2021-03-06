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
using System.Configuration;

namespace Nuxleus.Web.Configuration {
   
   sealed class ProcessorElement : ConfigurationElement {

      static readonly ConfigurationPropertyCollection _Properties;
      static readonly ConfigurationProperty _NameProperty;
      static readonly ConfigurationProperty _TypeProperty;

      Type _TypeInternal;

      protected override ConfigurationPropertyCollection Properties {
         get { return _Properties; }
      }

      public string Name { 
         get { return (string)this[_NameProperty]; }
         internal set { this[_NameProperty] = value; }
      }
      
      public string Type { 
         get { return (string)this[_TypeProperty]; }
         internal set { this[_TypeProperty] = value; }
      }

      internal Type TypeInternal {
         get {
            if (_TypeInternal == null) {
               lock (this) {
                  if (_TypeInternal == null) {
                     _TypeInternal = TypeLoader.LoadType(this.Type, typeof(Object), this, "type");
                  }
               }
            }
            return _TypeInternal;
         }
      }
 
      static ProcessorElement() {

         _NameProperty = new ConfigurationProperty("name", typeof(String), null, ConfigurationPropertyOptions.IsKey | ConfigurationPropertyOptions.IsRequired);
         _TypeProperty = new ConfigurationProperty("type", typeof(String), null, ConfigurationPropertyOptions.IsRequired);

         _Properties = new ConfigurationPropertyCollection { 
            _NameProperty, _TypeProperty
         };
      }
   }
}
