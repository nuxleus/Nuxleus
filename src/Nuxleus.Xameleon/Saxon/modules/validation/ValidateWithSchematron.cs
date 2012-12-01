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
using System.Text;
using System.Xml.XPath;
using Saxon.Api;

namespace myxsl.net.saxon.modules.validation {

   sealed class ValidateWithSchematron : ExtensionFunctionDefinition {

      readonly SaxonItemFactory itemFactory;
      readonly XdmSequenceType resultType;
      readonly XdmSequenceType[] _ArgumentTypes;
      readonly QName _FunctionName;

      public override XdmSequenceType[] ArgumentTypes { get { return _ArgumentTypes; } }
      public override QName FunctionName { get { return _FunctionName; } }
      public override int MinimumNumberOfArguments { get { return 2; } }
      public override int MaximumNumberOfArguments { get { return 4; } }

      public ValidateWithSchematron(SaxonItemFactory itemFactory) {

         this.itemFactory = itemFactory;
         this._FunctionName = new QName(ValidationModule.Namespace, "validate-with-schematron");
         this._ArgumentTypes = new[] { 
            new XdmSequenceType(XdmAtomicType.BuiltInAtomicType(QName.XS_STRING), ' '),
            new XdmSequenceType(XdmAnyNodeType.Instance, ' '),
            new XdmSequenceType(XdmAtomicType.BuiltInAtomicType(QName.XS_STRING), '?'),
            new XdmSequenceType(XdmAnyNodeType.Instance, '*'),
         };
         this.resultType = new XdmSequenceType(XdmNodeKind.Document, ' ');
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

            string validatorUri = arguments[0].AsAtomicValues().Single().ToString();
            XPathNavigator source = arguments[1].AsNodes().Single().ToXPathNavigator();
            
            XPathNavigator report;

            if (arguments.Length == 2) {
               report = ValidationModule.ValidateWithSchematron(validatorUri, source);
            
            } else {

               string phase = arguments[2].AsAtomicValues().Select(x => x.ToString()).SingleOrDefault();

               if (arguments.Length == 3) {
                  report = ValidationModule.ValidateWithSchematron(validatorUri, source, phase);
               } else {

                  IEnumerable<XPathNavigator> parameters = arguments[3].AsNodes()
                     .Select(n => n.ToXPathNavigator());

                  report = ValidationModule.ValidateWithSchematron(validatorUri, source, phase, parameters);
               }
            }

            XdmNode result = report.ToXdmNode(this.itemFactory);

            return result.GetXdmEnumerator();
         }
      } 

      #endregion
   }
}
