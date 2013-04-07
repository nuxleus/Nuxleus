//
// MediaTypes.cs
//
// Author:
//       M. David Peterson <m.david@3rdandurban.com>
//
// Copyright (c) 2013 M. David Peterson
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE/ AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
namespace Nuxleus.Web.Module.EXPath.HttpClient
{
	static class MediaTypes
	{

		public const string Html = "text/html";
		public const string XHtml = "application/xhtml+xml";
		const StringComparison Comparison = StringComparison.Ordinal;

		public static bool IsXml (string mediaType)
		{
         
			if (mediaType == null)
				throw new ArgumentNullException ("mediaType");

			if (mediaType.Length < 6)
				return false;

			switch (mediaType) {
			case "text/xml":
			case "application/xml":
			case "text/xml-external-parsed-entity":
			case "application/xml-external-parsed-entity":
				return true;

			default:
				return mediaType.EndsWith ("+xml", Comparison);
			}
		}

		public static bool IsText (string mediaType)
		{
         
			if (mediaType == null)
				throw new ArgumentNullException ("mediaType");
         
			return mediaType.StartsWith ("text/", Comparison);
		}

		public static bool IsMultipart (string mediaType)
		{

			if (mediaType == null)
				throw new ArgumentNullException ("mediaType");

			return mediaType.StartsWith ("multipart/", Comparison);
		}

		public static bool Equals (string mediaType1, string mediaType2)
		{
         
			if (mediaType1 == null)
				throw new ArgumentNullException ("mediaType1");
			if (mediaType2 == null)
				throw new ArgumentNullException ("mediaType2");

			return String.Equals (mediaType1, mediaType2, Comparison);
		}
	}
}
