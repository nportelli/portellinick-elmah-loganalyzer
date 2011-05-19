﻿namespace Crepido.ElmahOfflineViewer.Core.Common
{
	public class NameValuePair
	{
		public NameValuePair(string name, string value)
		{
			Name = name;
			Value = value;
		}

		public string Name { get; private set; }

		public string Value { get; private set; }

		public override string ToString()
		{
			return Name;
		}
	}
}
