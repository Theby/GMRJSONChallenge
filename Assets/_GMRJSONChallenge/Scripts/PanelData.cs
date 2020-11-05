using System;
using System.Collections.Generic;
using System.Linq;
using SimpleJSON;

namespace GMRJSONChallenge
{
	public class PanelData
	{
		public string Title { get; }
		public IEnumerable<string> ColumnHeaders { get; }
		public List<Dictionary<string, string>> ContentsData { get; }

		private const string JsonTitleKey = "Title";
		private const string JsonColumnHeaderKey = "ColumnHeaders";
		private const string JsonDataKey = "Data";

		public PanelData(string title, IEnumerable<string> columnHeaders)
		{
			Title = title;
			ColumnHeaders = columnHeaders;
			ContentsData = new List<Dictionary<string, string>>();
		}

		private void AddContent(Dictionary<string, string> content)
		{
			ContentsData.Add(content);
		}

		public static PanelData ParseJSON(string jsonText)
		{
			var json = JSON.Parse(jsonText);

			if (!json.HasKey(JsonTitleKey))
				throw new Exception("Wrong Title Key");

			var panelTitle = json[JsonTitleKey];

			if (!json.HasKey(JsonColumnHeaderKey))
				throw new Exception("Wrong Column Header Key");

			var panelColumnHeaders = json[JsonColumnHeaderKey].Childs.Select(node => node.Value);
			var panelData = new PanelData(panelTitle, panelColumnHeaders);

			if (!json.HasKey(JsonDataKey))
				throw new Exception("Wrong Data Key");

			foreach (JSONNode dataNode in json[JsonDataKey].AsArray)
			{
				var content = new Dictionary<string, string>();

				foreach (var contentHeader in panelData.ColumnHeaders)
				{
					if (dataNode.HasKey(contentHeader))
					{
						content.Add(contentHeader, dataNode[contentHeader]);
					}
					else
					{
						content.Add(contentHeader, "N/A");
					}
				}

				panelData.AddContent(content);
			}

			return panelData;
		}
	}
}