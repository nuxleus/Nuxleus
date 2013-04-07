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
using System.CodeDom;
using Nuxleus.Web.Page;
using System.IO;
using System.Xml;

namespace Nuxleus.Web.UI.Compilation
{
	public class SchematronValidatorCodeDomTreeGenerator : BaseCodeDomTreeGenerator
	{
      
		readonly SchematronParser parser;

		public Uri ValidatorUri { get; set; }
      
		public SchematronValidatorCodeDomTreeGenerator (SchematronParser parser)
		{
			this.parser = parser;
		}

		public override void BuildCodeDomTree (CodeCompileUnit compileUnit)
		{
         
			CodeThisReferenceExpression @this = new CodeThisReferenceExpression ();
			CodeBaseReferenceExpression @base = new CodeBaseReferenceExpression ();
			CodeTypeReferenceExpression thisType = new CodeTypeReferenceExpression (new CodeTypeReference (GeneratedTypeName));
			CodeTypeReference uriType = new CodeTypeReference (typeof(Uri));

			CodeMemberField executableField = new CodeMemberField {
            Name = "_Executable",
            Type = new CodeTypeReference(typeof(XsltExecutable)),
            Attributes = MemberAttributes.Private | MemberAttributes.Static
         };

			// methods

			// cctor
			CodeTypeConstructor cctor = new CodeTypeConstructor {
            CustomAttributes = { 
               new CodeAttributeDeclaration(DebuggerNonUserCodeTypeReference)
            }
         };

			CodeVariableDeclarationStatement procVar = new CodeVariableDeclarationStatement {
            Name = "proc",
            Type = new CodeTypeReference(typeof(IXsltProcessor)),
            InitExpression = new CodeIndexerExpression {
               TargetObject = new CodePropertyReferenceExpression {
                  PropertyName = "Xslt",
                  TargetObject = new CodeTypeReferenceExpression(typeof(Processors))
               },
               Indices = { 
                  new CodePrimitiveExpression(parser.ProcessorName)
               }
            } 
         };

			CodeVariableDeclarationStatement sourceVar = new CodeVariableDeclarationStatement {
            Name = "source",
            Type = new CodeTypeReference(typeof(Stream)),
            InitExpression = new CodePrimitiveExpression(null)
         };

			CodeVariableDeclarationStatement sourceUriVar = new CodeVariableDeclarationStatement {
            Name = "sourceUri",
            Type = uriType,
            InitExpression = new CodeObjectCreateExpression {
               CreateType = uriType,
               Parameters = {
                  new CodePrimitiveExpression(ValidatorUri.AbsoluteUri)
               }
            }
         };

			CodeVariableDeclarationStatement resolverVar = new CodeVariableDeclarationStatement {
            Name = "resolver",
            Type = new CodeTypeReference(typeof(XmlResolver)),
            InitExpression = new CodeObjectCreateExpression(typeof(XmlEmbeddedResourceResolver))
         };

			CodeTryCatchFinallyStatement trySt = new CodeTryCatchFinallyStatement { 
            TryStatements = {
               new CodeAssignStatement {
                  Left = new CodeVariableReferenceExpression(sourceVar.Name),
                  Right = new CodeCastExpression {
                     TargetType = new CodeTypeReference(typeof(Stream)),
                     Expression = new CodeMethodInvokeExpression {
                        Method = new CodeMethodReferenceExpression {
                           MethodName = "GetEntity",
                           TargetObject = new CodeVariableReferenceExpression(resolverVar.Name)
                        },
                        Parameters = {
                           new CodeVariableReferenceExpression(sourceUriVar.Name),
                           new CodePrimitiveExpression(null),
                           new CodeTypeOfExpression(typeof(Stream))
                        }
                     }
                  }
               }
            }
         };

			CodeVariableDeclarationStatement optionsVar = new CodeVariableDeclarationStatement {
            Name = "options",
            Type = new CodeTypeReference(typeof(XsltCompileOptions)),
         };
			optionsVar.InitExpression = new CodeObjectCreateExpression (optionsVar.Type);

			trySt.TryStatements.Add (optionsVar);
			trySt.TryStatements.Add (new CodeAssignStatement {
            Left = new CodePropertyReferenceExpression {
               PropertyName = "BaseUri",
               TargetObject = new CodeVariableReferenceExpression(optionsVar.Name)
            },
            Right = new CodeVariableReferenceExpression(sourceUriVar.Name)
         });
			trySt.TryStatements.Add (new CodeAssignStatement {
            Left = new CodeFieldReferenceExpression {
               FieldName = executableField.Name,
               TargetObject = thisType
            },
            Right = new CodeMethodInvokeExpression(
               new CodeMethodReferenceExpression {
                  MethodName = "Compile",
                  TargetObject = new CodeVariableReferenceExpression(procVar.Name)
               },
               new CodeVariableReferenceExpression(sourceVar.Name),
               new CodeVariableReferenceExpression(optionsVar.Name)
            )
         });

			CodeConditionStatement disposeIf = new CodeConditionStatement {
            Condition = new CodeBinaryOperatorExpression {
               Left = new CodeVariableReferenceExpression(sourceVar.Name),
               Operator = CodeBinaryOperatorType.IdentityInequality,
               Right = new CodePrimitiveExpression(null)
            },
            TrueStatements = {
               new CodeMethodInvokeExpression {
                  Method = new CodeMethodReferenceExpression {
                     MethodName = "Dispose",
                     TargetObject = new CodeVariableReferenceExpression(sourceVar.Name)
                  }
               }
            }
         };

			trySt.FinallyStatements.Add (disposeIf);

			cctor.Statements.AddRange (new CodeStatement[] {
                procVar,
                sourceVar,
                sourceUriVar,
                resolverVar,
                trySt
            });

			// ctor
			CodeConstructor ctor = new CodeConstructor {
            Attributes = MemberAttributes.Public,
            CustomAttributes = { 
               new CodeAttributeDeclaration(DebuggerNonUserCodeTypeReference)
            },
            BaseConstructorArgs = { 
               new CodeFieldReferenceExpression {
                  FieldName = executableField.Name,
                  TargetObject = thisType
               }
            }
         };

			// class
			CodeTypeDeclaration codeType = new CodeTypeDeclaration {
            Name = GeneratedTypeName,
            IsClass = true,
            BaseTypes = { typeof(SchematronXsltValidator) },
            Members = { cctor, ctor, executableField }
         };

			CodeNamespace codeNamespace = new CodeNamespace {
            Name = GeneratedTypeNamespace,
            Types = { codeType }
         };

			compileUnit.Namespaces.Add (codeNamespace);
		}
	}
}
