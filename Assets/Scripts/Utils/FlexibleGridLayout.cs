using UnityEngine;
using UnityEngine.UI;

public enum FitType
{
    Uniform, Width, Height, FixedRows, FixedColumns
}

public class FlexibleGridLayout : LayoutGroup
{
    public FitType FitType;
    public int Rows;
    public int Cols;
    public Vector2 CellSize;
    public Vector2 Spacing;

    [SerializeField][ReadOnly]
    private bool fitX;
    [SerializeField][ReadOnly]
    private bool fitY;

    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();

        if (FitType == FitType.Uniform || FitType == FitType.Width || FitType == FitType.Height)
        {
            fitX = true;
            fitY = true;
            Rows = Cols = Mathf.CeilToInt(Mathf.Sqrt(transform.childCount));
        }

        if (FitType == FitType.Width || FitType == FitType.FixedColumns)
        {
            Rows = Mathf.CeilToInt(transform.childCount / Cols);
        }
        else if (FitType == FitType.Height || FitType == FitType.FixedRows)
        {
            Cols = Mathf.CeilToInt(transform.childCount / Rows);
        }

        float cellWidth = rectTransform.rect.width / Cols - ((Spacing.x / Cols) * (Cols - 1)) - padding.left / Cols - padding.right / Cols;
        float cellHeight = rectTransform.rect.height / Rows - ((Spacing.y / Rows) * (Rows - 1)) - padding.top / Rows - padding.bottom / Rows;
        
        CellSize.x = fitX ? cellWidth : CellSize.x;
        CellSize.y = fitY ? cellHeight : CellSize.y;

        for (int i = 0; i < rectChildren.Count; ++i)
        {
            int rowIndex = i / Cols;
            int colIndex = i % Cols;
            var element = rectChildren[i];

            var xPos = (CellSize.x * colIndex) + (Spacing.x * colIndex) + padding.left;
            var yPos = (CellSize.y * rowIndex) + (Spacing.y * rowIndex) + padding.top;

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
