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
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Microsoft.CSharp;

namespace Nuxleus.Web.Sys {

   static class ExtensionObjectGenerator {

      internal static Type RenameMethodsIfNecessary(Type extensionObjectType) {

         if (extensionObjectType == null) throw new ArgumentNullException("extensionObjectType");

         MethodInfo[] methodsToRename = extensionObjectType.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Instance)
            .Where(m => m.Name.Contains('_'))
            .ToArray();

         if (methodsToRename.Length == 0) {
            return extensionObjectType;
         } else {
            
            AssemblyName name = new AssemblyName("myxsl.net.system");

            AssemblyBuilder asmb = AppDomain.CurrentDomain.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);
            ModuleBuilder moduleb = asmb.DefineDynamicModule(name.FullName);

            TypeBuilder typeb = moduleb.DefineType(String.Concat("Runtime", extensionObjectType.Name), TypeAttributes.Class | TypeAttributes.Public, extensionObjectType);

            for (int i = 0; i < methodsToRename.Length; i++)
               EmitMethod(typeb, methodsToRename[i]);

            return typeb.CreateType();
         }
      }

      static MethodBuilder EmitMethod(TypeBuilder typeb, MethodInfo method) {

         ParameterInfo[] parameters = method.GetParameters();

         MethodBuilder methodb = typeb.DefineMethod(method.Name.Replace('_', '-'), MethodAttributes.Public | MethodAttributes.HideBySig);
         methodb.SetReturnType(method.ReturnType);
         methodb.SetParameters(parameters.Select(p => p.ParameterType).ToArray());

         for (int j = 1; j <= parameters.Length; j++)
            methodb.DefineParameter(j, ParameterAttributes.None, parameters[j - 1].Name);

         ILGenerator ilgen = methodb.GetILGenerator();

         LocalBuilder localb = ilgen.DeclareLocal(method.ReturnType);
         Label label11 = ilgen.DefineLabel();

         ilgen.Emit(OpCodes.Nop);
         ilgen.Emit(OpCodes.Ldarg_0);

         for (int j = 1; j <= parameters.Length; j++)
            ilgen.Emit(OpCodes.Ldarg, j);

         ilgen.Emit(OpCodes.Call, method);
         ilgen.Emit(OpCodes.Stloc_0);
         ilgen.Emit(OpCodes.Br_S, label11);
         ilgen.MarkLabel(label11);
         ilgen.Emit(OpCodes.Ldloc_0);
         ilgen.Emit(OpCodes.Ret);

         return methodb;
      }
   }
}
