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
using System.Text;
using Saxon.Api;

namespace myxsl.net.saxon.modules.exslt.common {
   
   sealed class NodeSet : ExtensionFunctionDefinition {

      readonly QName _FunctionName;
      readonly XdmSequenceType[] _ArgumentTypes;

      public override XdmSequenceType[] ArgumentTypes { get { return _ArgumentTypes; } }
      public override QName FunctionName { get { return _FunctionName; } }
      public override int MaximumNumberOfArguments { get { return 1; } }
      public override int MinimumNumberOfArguments { get { return 1; } }

      public NodeSet() {

         this._FunctionName = new QName("http://exslt.org/common", "node-set");
         this._ArgumentTypes = new[] { 
            new XdmSequenceType(XdmAnyNodeType.Instance, '*') 
         };
      }

      public override XdmSequenceType ResultType(XdmSequenceType[] ArgumentTypes) {
         return ArgumentTypes[0];
      }

      public override ExtensionFunctionCall MakeFunctionCall() {
         return new FunctionCall();
      }

      #region Nested Types

      class FunctionCall : ExtensionFunctionCall {
         
         public override IXdmEnumerator Call(IXdmEnumerator[] arguments, DynamicContext context) {

            IXdmEnumerator result;

            if (arguments != null || arguments.Length > 0) 
               result = arguments[0];
            else 
               result = XdmEmptySequence.INSTANCE.GetXdmEnumerator();

            return result;
         }
      }

      #endregion
   }
}
