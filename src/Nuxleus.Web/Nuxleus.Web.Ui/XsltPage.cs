// Copyright 2009 Max Toro Q.
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
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace Nuxleus.Web.UI {
   
   public abstract class XsltPage : BasePage, IHasXPathItemFactory {

      readonly XsltRuntimeOptions runtimeOptions;

      public abstract XsltExecutable Executable { get; }

      XPathItemFactory IHasXPathItemFactory.ItemFactory {
         get { return this.Executable.Processor.ItemFactory; }
      }

      public XsltPage() {
         this.runtimeOptions = new XsltRuntimeOptions();
      }

      protected override void ProcessRequest() {

         try {
            InitializeRuntimeOptions(this.runtimeOptions);
         } catch (ProcessorException ex) {
            throw new HttpException((int)HttpStatusCode.BadRequest, ex.Message, ex);
         }

         base.ProcessRequest();
      }

      public virtual void InitializeRuntimeOptions(XsltRuntimeOptions options) {
         SetBoundParameters(options.Parameters);
      }

      public override void Render(TextWriter writer) {
         Render(writer, this.runtimeOptions);
      }
      
      public void Render(TextWriter writer, XsltRuntimeOptions options) {
         
         try {
            this.Executable.Run(writer, options);
         
         } catch (ProcessorException ex) {
            throw CreateRenderException(ex);
         }
      }

      protected string AsString<T>(T value) {
         return (value != null) ? value.ToString() : default(String);
      }

      protected string AsString<T>(IEnumerable<T> values) {

         T first = values.FirstOrDefault();

         return AsString(first);
      }
   }
}
