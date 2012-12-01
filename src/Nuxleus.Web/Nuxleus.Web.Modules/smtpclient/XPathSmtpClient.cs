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
using System.Net;
using System.Reflection;
using System.Xml;
using System.Xml.XPath;
using System.Web;
using System.Net.Mail;
using System.Xml.Schema;

namespace Nuxleus.Web.Module.SmtpClient {

   [XPathModule(Namespace, NamespaceBindings = new[] { Prefix, Namespace })]
   public sealed class XPathSmtpClient {
      
      public const string Namespace = "http://nuxleus.net/ns/smtp-client";
      internal const string Prefix = "smtp";

      public XPathItemFactory ItemFactory { get; set; }

      [XPathFunction("send", "item()", "element(smtp:message)")]
      public XPathNavigator Send(XPathNavigator message) {

         if (message == null) throw new ArgumentNullException("message");

         XPathItemFactory itemFactory = this.ItemFactory ?? HttpContext.Current.GetCurrentItemFactory();

         if (itemFactory == null)
            throw new InvalidOperationException("ItemFactory cannot be null.");

         MailMessage mailMessage = GetMailMessage(message, itemFactory);

         var smtp = new System.Net.Mail.SmtpClient();

         try {
            smtp.Send(mailMessage);

         } catch (SmtpException ex) {
            return itemFactory.CreateElement(GetError(ex));
         }

         return itemFactory.CreateElement(new XPathSmtpSuccess());
      }

      MailMessage GetMailMessage(XPathNavigator message, XPathItemFactory itemFactory) {

         var xpathMessage = new XPathMailMessage();
         xpathMessage.ReadXml(message);

         return xpathMessage.ToMailMessage(itemFactory);
      }

      XPathSmtpError GetError(SmtpException exception) {

         return new XPathSmtpError { 
            Status = exception.StatusCode,
            Message = exception.Message
         };
      }
   }
}
