using UnityEngine;
using UnityEngine.UI;

public class PanelView : MonoBehaviour
{
	[Header("Elements")]
	[SerializeField] private Text panelTitle;
	[SerializeField] private GameObject contentRoot;

	[Header("Prefabs")]
	[SerializeField] private GameObject columnPrefab;
	[SerializeField] private Text columnTitlePrefab;
	[SerializeField] private Text columnContentPrefab;

	public void CleanPanel()
	{
		panelTitle.text = string.Empty;

		foreach (Transform childTransform in contentRoot.transform)
		{
			Destroy(childTransform.gameObject);
		}
	}

	public void UpdatePanelContent(PanelData panelData)
	{
		CleanPanel();

		panelTitle.text = panelData.Title;

		foreach (var panelDataColumnHeader in panelData.ColumnHeaders)
		{
			var column = Instantiate(columnPrefab, contentRoot.transform, false);

			var columnTitle = Instantiate(columnTitlePrefab, column.transform, false);
			columnTitle.text = panelDataColumnHeader;

			foreach (var contentData in panelData.ContentsData)
			{
				var columnContent = Instantiate(columnContentPrefab, column.transform, false);
				columnContent.text = contentData[panelDataColumnHeader];
			}
		}
	}
}