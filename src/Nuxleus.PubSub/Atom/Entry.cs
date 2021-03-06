//
// entry.cs: 
//
// Author:
//   Sylvain Hellegouarch (sh@defuze.org)
//
// Copyright (C) 2007, 3rd&Urban, LLC
// 

using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Nuxleus.Atom
{
    [XmlRootAttribute("entry", Namespace = "http://www.w3.org/2005/Atom", IsNullable = false)]
    public class Entry
    {
        private static XmlSerializerNamespaces xmlns = null;

        [XmlAttribute("lang", Form = System.Xml.Schema.XmlSchemaForm.Qualified,
              Namespace = "http://www.w3.org/XML/1998/namespace")]
        public string Lang;

        [XmlAttribute("base", Form = System.Xml.Schema.XmlSchemaForm.Qualified,
              Namespace = "http://www.w3.org/XML/1998/namespace")]
        public string Base;

        [XmlElementAttribute(ElementName = "id", IsNullable = false)]
        public string Id;

        [XmlElementAttribute(ElementName = "title", IsNullable = false)]
        public string Title;

        [XmlElementAttribute(ElementName = "icon")]
        public string Icon;

        [XmlElementAttribute(ElementName = "logo")]
        public string Logo;

        [XmlElementAttribute(Type = typeof(DateTime), ElementName = "published")]
        public DateTime? Published = DateTime.UtcNow;

        [XmlElementAttribute(Type = typeof(DateTime), ElementName = "updated")]
        public DateTime? Updated = DateTime.UtcNow;

        [XmlElementAttribute(Type = typeof(DateTime), ElementName = "edited",
             Namespace = "http://www.w3.org/2007/app")]
        public DateTime? Edited = DateTime.UtcNow;

        [XmlElementAttribute(Type = typeof(Author), ElementName = "author")]
        public Author[] Authors;

        [XmlElementAttribute(Type = typeof(Contributor), ElementName = "contributor")]
        public Contributor[] Contributors;

        [XmlElementAttribute(Type = typeof(Category), ElementName = "category")]
        public Category[] Categories;

        [XmlElementAttribute(Type = typeof(Link), ElementName = "link")]
        public List<Link> Links;

        [XmlElementAttribute(Type = typeof(Nuxleus.Atom.Summary), ElementName = "summary")]
        public Nuxleus.Atom.Summary Summary;

        [XmlElementAttribute(Type = typeof(Nuxleus.Atom.Rights), ElementName = "rights")]
        public Nuxleus.Atom.Rights Rights;

        [XmlElementAttribute(Type = typeof(Nuxleus.Atom.Content), ElementName = "content")]
        public Nuxleus.Atom.Content Content;

        [XmlElementAttribute(DataType = "int", ElementName = "total",
             Namespace = "http://purl.org/syndication/thread/1.0")]
        public int? TotalResponses;

        [XmlElementAttribute(Type = typeof(Nuxleus.Atom.InReplyTo), ElementName = "in-reply-to",
             Namespace = "http://purl.org/syndication/thread/1.0")]
        public Nuxleus.Atom.InReplyTo InReplyTo;


        public static Entry Parse(string xml)
        {
            XmlReader reader = XmlReader.Create(new StringReader(xml));
            XmlSerializer serializer = new XmlSerializer(typeof(Entry));
            return (Entry)serializer.Deserialize(reader);
        }

        public static Entry Parse(Stream stream)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Entry));
            return (Entry)serializer.Deserialize(stream);
        }

        public override string ToString()
        {
            if (xmlns == null)
            {
                xmlns = new XmlSerializerNamespaces();
                xmlns.Add(String.Empty, "http://www.w3.org/2005/Atom");
                xmlns.Add("app", "http://www.w3.org/2007/app");
                xmlns.Add("thr", "http://purl.org/syndication/thread/1.0");
            }
            StringBuilder sb = new StringBuilder();
            StringWriter writer = new StringWriter(sb);
            XmlSerializer serializer = new XmlSerializer(typeof(Entry));
            serializer.Serialize(writer, this);
            return sb.ToString();
        }
    }

}