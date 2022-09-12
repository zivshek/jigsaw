using UnityEngine;
using UnityEngine.UI;

public class FlexibleGridLayout : LayoutGroup
{
    public int Rows;
    public int Cols;
    public Vector2 CellSize;
    public Vector2 Spacing;

    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();

        Rows = Cols = Mathf.CeilToInt(Mathf.Sqrt(transform.childCount));

        CellSize = new Vector2(rectTransform.rect.width / Cols - ((Spacing.x / Cols) * (Cols - 1)),
            rectTransform.rect.height / Rows - ((Spacing.y / Rows) * (Rows - 1)));

        for (int i = 0; i < rectChildren.Count; ++i)
        {
            int rowIndex = i / Cols;
            int colIndex = i % Cols;
            var element = rectChildren[i];

            var xPos = (CellSize.x * colIndex) + (Spacing.x * colIndex);
            var yPos = (CellSize.y * rowIndex) + (Spacing.y * rowIndex);

            SetChildAlongAxis(element, 0, xPos, CellSize.x);
            SetChildAlongAxis(element, 1, yPos, CellSize.y);
        }
    }

    public override void CalculateLayoutInputVertical()
    {
    }

    public override void SetLayoutHorizontal()
    {
    }

    public override void SetLayoutVertical()
    {
    }
}
