using UnityEngine;

namespace Puzzle
{
    public class TilesGen : MonoBehaviour
    {
        [SerializeField]
        private string m_imageFilename;

        private Texture2D m_textureOriginal;

        public int NumTilesX { get; private set; }
        public int NumTilesY { get; private set; }

        void Start()
        {
            CreateBaseTexture();
            TestTileFloodFill();
        }

        void CreateBaseTexture()
        {
            // Load the main image.
            m_textureOriginal = SpriteUtils.LoadTexture(m_imageFilename);

            SpriteRenderer spriteRenderer =
              gameObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = SpriteUtils.CreateSpriteFromTexture2D(
                m_textureOriginal,
                0,
                0,
                m_textureOriginal.width,
                m_textureOriginal.height);
        }

        void TestTileCurves()
        {
            Tile tile = new Tile(m_textureOriginal, 0, 0);
            tile.DrawCurve(Tile.Direction.UP, Tile.PosNegType.POS, Color.red);
            tile.DrawCurve(Tile.Direction.UP, Tile.PosNegType.NEG, Color.green);
            tile.DrawCurve(Tile.Direction.UP, Tile.PosNegType.NONE, Color.white);

            tile.DrawCurve(Tile.Direction.RIGHT, Tile.PosNegType.POS, Color.red);
            tile.DrawCurve(Tile.Direction.RIGHT, Tile.PosNegType.NEG, Color.green);
            tile.DrawCurve(Tile.Direction.RIGHT, Tile.PosNegType.NONE, Color.white);

            tile.DrawCurve(Tile.Direction.DOWN, Tile.PosNegType.POS, Color.red);
            tile.DrawCurve(Tile.Direction.DOWN, Tile.PosNegType.NEG, Color.green);
            tile.DrawCurve(Tile.Direction.DOWN, Tile.PosNegType.NONE, Color.white);

            tile.DrawCurve(Tile.Direction.LEFT, Tile.PosNegType.POS, Color.red);
            tile.DrawCurve(Tile.Direction.LEFT, Tile.PosNegType.NEG, Color.green);
            tile.DrawCurve(Tile.Direction.LEFT, Tile.PosNegType.NONE, Color.white);
        }

        void TestTileFloodFill()
        {
            Tile tile = new Tile(m_textureOriginal, 0, 0);

            tile.SetPosNegType(Tile.Direction.UP, Tile.PosNegType.POS);
            tile.SetPosNegType(Tile.Direction.RIGHT, Tile.PosNegType.POS);
            tile.SetPosNegType(Tile.Direction.DOWN, Tile.PosNegType.POS);
            tile.SetPosNegType(Tile.Direction.LEFT, Tile.PosNegType.POS);

            tile.DrawCurve(Tile.Direction.UP, Tile.PosNegType.POS, Color.white);
            tile.DrawCurve(Tile.Direction.RIGHT, Tile.PosNegType.POS, Color.white);
            tile.DrawCurve(Tile.Direction.DOWN, Tile.PosNegType.POS, Color.white);
            tile.DrawCurve(Tile.Direction.LEFT, Tile.PosNegType.POS, Color.white);

            tile.Apply();

            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

            Sprite sprite = SpriteUtils.CreateSpriteFromTexture2D(
                tile.FinalCut,
                0,
                0,
                tile.FinalCut.width,
                tile.FinalCut.height);
            spriteRenderer.sprite = sprite;
        }
    }
}
