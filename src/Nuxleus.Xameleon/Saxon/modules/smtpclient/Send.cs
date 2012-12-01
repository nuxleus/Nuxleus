// Copyright 2011 Max Toro Q.
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
using System.Xml.XPath;
using Saxon.Api;
using myxsl.net.smtpclient;

namespace myxsl.net.saxon.modules.smtpclient {

   sealed class Send : ExtensionFunctionDefinition {

      readonly SaxonItemFactory itemFactory;
      readonly XdmSequenceType resultType;
      readonly QName _FunctionName;
      readonly XdmSequenceType[] _ArgumentTypes;

      public override XdmSequenceType[] ArgumentTypes { get { return _ArgumentTypes; } }
      public override QName FunctionName { get { return _FunctionName; } }
      public override int MaximumNumberOfArguments { get { return 1; } }
      public override int MinimumNumberOfArguments { get { return 1; } }
      public override bool HasSideEffects { get { return true; } }

      public Send(SaxonItemFactory itemFactory) {

         this.itemFactory = itemFactory;
         this._FunctionName = new QName(XPathSmtpClient.Namespace, "send");
         this._ArgumentTypes = new[] {
            new XdmSequenceType(XdmNodeKind.Element, ' ')
         };
         this.resultType = new XdmSequenceType(XdmNodeKind.Element, ' ');
      }

      public override ExtensionFunctionCall MakeFunctionCall() {
         return new FunctionCall(this.itemFactory);
      }

      public override XdmSequenceType ResultType(XdmSequenceType[] ArgumentTypes) {
         return this.resultType;
      }

      #region Nested Types

      class FunctionCall : ExtensionFunctionCall {

         readonly SaxonItemFactory itemFactory;

         public FunctionCall(SaxonItemFactory itemFactory) {
            this.itemFactory = itemFactory;
         }

         public override IXdmEnumerator Call(IXdmEnumerator[] arguments, DynamicContext context) {

            XPathSmtpClient client = new XPathSmtpClient { 
               ItemFactory = this.itemFactory
            };

            XPathNavigator message = arguments[0].AsNodes().Select(x => x.ToXPathNavigator()).Single();

            XPathNavigator result = client.Send(message);

            return result.ToXdmValue(this.itemFactory).GetXdmEnumerator();
         }
      }

      #endregion
   }
}
