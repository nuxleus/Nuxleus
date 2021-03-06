/*
// File: AtomSyndication.Feed.cs:
// Author:
//  Sylvain Hellegouarch <sh@defuze.org>
//  M. David Peterson <m.david@3rdandUrban.com>
//
// Copyright � 2007-2011 3rd&Urban, LLC
//
// The code contained in this file is licensed under The MIT License
// Please see http://www.opensource.org/licenses/mit-license.php for specific detail.
*/

using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Nuxleus.ServiceModel.Types.AtomPub;

namespace Nuxleus.ServiceModel.Types.Atom
{
    [XmlRootAttribute("feed", Namespace="http://www.w3.org/2005/Atom", IsNullable=false)]
    public class Feed {
        private static XmlSerializerNamespaces xmlns = null;
        private static XmlSerializer m_serializer = new XmlSerializer(typeof(Feed));

        [XmlAttribute("lang", Form=System.Xml.Schema.XmlSchemaForm.Qualified,
              Namespace="http://www.w3.org/XML/1998/namespace")]
        public string Lang;

        [XmlAttribute("base", Form=System.Xml.Schema.XmlSchemaForm.Qualified,
              Namespace="http://www.w3.org/XML/1998/namespace")]
        public string Base;

        [XmlElement(ElementName="id", IsNullable=false)]
        public string Id;

        [XmlElement(ElementName="title", IsNullable=false)]
        public string Title;

        [XmlElement(ElementName="subtitle")]
        public string Subtitle;

        [XmlElement(ElementName="icon")]
        public string Icon;

        [XmlElement(ElementName="logo")]
        public string Logo;

        [XmlElement(Type=typeof(DateTime), ElementName="updated")]
        public DateTime Updated = DateTime.UtcNow;

        [XmlElement(Type=typeof(Author), ElementName="author")]
        public Author[] Authors;

        [XmlElement(Type=typeof(Contributor), ElementName="contributor")]
        public Contributor[] Contributors;

        [XmlElement(Type=typeof(Category), ElementName="category")]
        public Category[] Categories;

        [XmlElement(Type=typeof(Link), ElementName="link")]
        public Link[] Links;

        [XmlElement(Type=typeof(Generator), ElementName="generator")]
        public Generator Generator;

        [XmlElement(Type=typeof(Rights), ElementName="rights")]
        public Rights Rights;

        [XmlElement(Type=typeof(Entry), ElementName="entry")]
        public Entry[] Entries;

        [XmlElement(Type=typeof(Collection), ElementName="collection",
             Namespace="http://www.w3.org/2007/app")]
        public Collection Collection;

        public static Feed Parse ( string xml ) {
            XmlReader reader = XmlReader.Create(new StringReader(xml));
            //XmlSerializer serializer = new XmlSerializer(typeof(Feed));
            return (Feed)m_serializer.Deserialize(reader);
        }

        public static Feed Parse ( Stream stream ) {
            //XmlSerializer serializer = new XmlSerializer(typeof(Feed));
            return (Feed)m_serializer.Deserialize(stream);
        }

        public override string ToString () {
            if (xmlns == null) {
                xmlns = new XmlSerializerNamespaces();
                xmlns.Add(String.Empty, "http://www.w3.org/2005/Atom");
                xmlns.Add("app", "http://www.w3.org/2007/app");
                xmlns.Add("thr", "http://purl.org/syndication/thread/1.0");
            }
            StringBuilder sb = new StringBuilder();
            StringWriter writer = new StringWriter(sb);
            //XmlSerializer serializer = new XmlSerializer(typeof(Feed));
            m_serializer.Serialize(writer, this);
            return sb.ToString();
        }
    }
}