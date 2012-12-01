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
using System.Web;
using Saxon.Api;

namespace myxsl.net.saxon.modules.request {

   sealed class IsLocal : ExtensionFunctionDefinition {

      readonly XdmSequenceType resultType;
      readonly QName _FunctionName;
      readonly XdmSequenceType[] _ArgumentTypes;

      public override XdmSequenceType[] ArgumentTypes { get { return _ArgumentTypes; } }
      public override QName FunctionName { get { return _FunctionName; } }
      public override int MaximumNumberOfArguments { get { return 0; } }
      public override int MinimumNumberOfArguments { get { return 0; } }

      public IsLocal() {

         this._FunctionName = new QName(RequestModule.Namespace, "is-local");
         this._ArgumentTypes = new XdmSequenceType[0];
         this.resultType = new XdmSequenceType(XdmAtomicType.BuiltInAtomicType(QName.XS_BOOLEAN), ' ');
      }

      public override ExtensionFunctionCall MakeFunctionCall() {
         return new FunctionCall();
      }

      public override XdmSequenceType ResultType(XdmSequenceType[] ArgumentTypes) {
         return this.resultType;
      }

      #region Nested Types

      class FunctionCall : ExtensionFunctionCall {

         public override IXdmEnumerator Call(IXdmEnumerator[] arguments, DynamicContext context) {

            XdmValue result = RequestModule.IsLocal().ToXdmItem();

            return result.GetXdmEnumerator();
         }
      }

      #endregion
   }
}