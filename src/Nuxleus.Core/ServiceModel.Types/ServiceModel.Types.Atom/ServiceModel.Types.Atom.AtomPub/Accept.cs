/*
// File: AtomPub.Accept.cs:
// Author(s):
//  Sylvain Hellegouarch <sh@defuze.org>
//  M. David Peterson <m.david@3rdandUrban.com>
//
// Copyright � 2007-2011 3rd&Urban, LLC
//
// The code contained in this file is licensed under The MIT License
// Please see http://www.opensource.org/licenses/mit-license.php for specific detail.
*/

using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace Nuxleus.ServiceModel.Types.AtomPub
{
    [DataContract(Name = "accept", Namespace = "http://www.w3.org/2007/app")]
    public class Accept
    {
        [XmlText]
        [DataMember]
        public string Value;
    }
}