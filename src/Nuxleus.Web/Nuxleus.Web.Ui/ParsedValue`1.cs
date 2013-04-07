

namespace Nuxleus.Web.Page
{
	public class ParsedValue<T>
	{

		public T Value { get; private set; }

		public string FileName { get; private set; }

		public int LineNumber { get; private set; }

		public ParsedValue (T value, string fileName, int lineNumber)
		{
			this.Value = value;
			this.FileName = fileName;
			this.LineNumber = lineNumber;
		}
	}
}
