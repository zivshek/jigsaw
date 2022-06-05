using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Puzzle
{
    public class BoardGen : MonoBehaviour
    {
        [SerializeField]
        private string m_imageFilename;

        private Sprite m_baseSpriteOpaque;
        private Sprite m_baseSpriteTransparent;

        private GameObject m_gameObjOpaque;
        private GameObject m_gameObjTransparent;

        private Tile[,] m_tiles;
        private GameObject[,] m_tileObjs;

        public int NumTilesX { get; private set; }
        public int NumTilesY { get; private set; }
        public bool LoadingFinished { get; private set; }

        void Start()
        {
            CreateBoard();
        }

        public void CreateBoard()
        {
            m_baseSpriteOpaque = LoadBaseTexture();

            m_gameObjOpaque = new GameObject();
            m_gameObjOpaque.name = m_imageFilename + "_Opaque";
            m_gameObjOpaque.AddComponent<SpriteRenderer>().sprite = m_baseSpriteOpaque;
            m_gameObjOpaque.GetComponent<SpriteRenderer>().sortingLayerName = "Opaque";

            m_baseSpriteTransparent = CreateTransparentView();
            m_gameObjTransparent = new GameObject();
            m_gameObjTransparent.name = m_imageFilename + "_Transparent";
            m_gameObjTransparent.AddComponent<SpriteRenderer>().sprite = m_baseSpriteTransparent;
            m_gameObjTransparent.GetComponent<SpriteRenderer>().sortingLayerName = "Transparent";

            StartCoroutine(CreateTilesCoroutine());

            m_gameObjOpaque.gameObject.SetActive(false);
        }

        IEnumerator CreateBoardCoroutine()
        {
            yield return StartCoroutine(CreateTilesCoroutine());

            // Hide the m_gameObjOpaque game object.
            m_gameObjOpaque.gameObject.SetActive(false);

            LoadingFinished = true;
        }

        Sprite LoadBaseTexture()
        {
            Texture2D tex = SpriteUtils.LoadTexture(m_imageFilename);
            if (!tex.isReadable)
            {
                Debug.Log("Error: Texture is not readable");
                return null;
            }

            if (tex.width % Tile.TileSize != 0 || tex.height % Tile.TileSize != 0)
            {
                Debug.Log("Error: Image must be of size that is multiple of <" + Tile.TileSize + ">");
                return null;
            }

            // Add padding to the image.
            Texture2D newTex = new Texture2D(
                tex.width + Tile.Padding * 2,
                tex.height + Tile.Padding * 2,
                TextureFormat.ARGB32,
                false);

            // Set the default colour as white
            for (int x = 0; x < newTex.width; ++x)
            {
                for (int y = 0; y < newTex.height; ++y)
                {
                    newTex.SetPixel(x, y, Color.white);
                }
            }

            // Copy the colours.
            for (int x = 0; x < tex.width; ++x)
            {
                for (int y = 0; y < tex.height; ++y)
                {
                    Color color = tex.GetPixel(x, y);
                    color.a = 1.0f;
                    newTex.SetPixel(x + Tile.Padding, y + Tile.Padding, color);
                }
            }
            newTex.Apply();

            Sprite sprite = SpriteUtils.CreateSpriteFromTexture2D(
                newTex,
                0,
                0,
                newTex.width,
                newTex.height);
            return sprite;
        }

        Sprite CreateTransparentView()
        {
            Texture2D tex = m_baseSpriteOpaque.texture;

            // Add padding to the image.
            Texture2D newTex = new Texture2D(
                tex.width,
                tex.height,
                TextureFormat.ARGB32,
                false);

            //for (int x = Tile.Padding; x < Tile.Padding + Tile.TileSize; ++x)
            for (int x = 0; x < newTex.width; ++x)
            {
                //for (int y = Tile.Padding; y < Tile.Padding + Tile.TileSize; ++y)
                for (int y = 0; y < newTex.height; ++y)
                {
                    Color c = tex.GetPixel(x, y);
                    if (x > Tile.Padding && x < (newTex.width - Tile.Padding) &&
                        y > Tile.Padding && y < newTex.height - Tile.Padding)
                    {
                        c.a = 0.2f;
                    }
                    newTex.SetPixel(x, y, c);
                }
            }

            newTex.Apply();

            Sprite sprite = SpriteUtils.CreateSpriteFromTexture2D(
                newTex,
                0,
                0,
                newTex.width,
                newTex.height);
            return sprite;
        }

        IEnumerator CreateTilesCoroutine()
        {
            Texture2D baseTexture = m_baseSpriteOpaque.texture;
            NumTilesX = baseTexture.width / Tile.TileSize;
            NumTilesY = baseTexture.height / Tile.TileSize;

            m_tiles = new Tile[NumTilesX, NumTilesY];
            m_tileObjs = new GameObject[NumTilesX, NumTilesY];

            for (int i = 0; i < NumTilesX; ++i)
            {
                for (int j = 0; j < NumTilesY; ++j)
                {
                    Tile tile = new Tile(baseTexture, i, j);

                    // Left side tiles
                    if (i == 0)
                    {
                        tile.SetPosNegType(Tile.Direction.LEFT, Tile.PosNegType.NONE);
                    }
                    else
                    {
                        // We have to create a tile that has LEFT direction opposite operation 
                        // of the tile on the left's RIGHT direction operation.
                        Tile leftTile = m_tiles[i - 1, j];
                        Tile.PosNegType rightOp = leftTile.GetPosNegType(Tile.Direction.RIGHT);
                        tile.SetPosNegType(Tile.Direction.LEFT, rightOp ==
                          Tile.PosNegType.NEG ?
                          Tile.PosNegType.POS : Tile.PosNegType.NEG);
                    }

                    // Bottom side tiles
                    if (j == 0)
                    {
                        tile.SetPosNegType(Tile.Direction.DOWN, Tile.PosNegType.NONE);
                    }
                    else
                    {
                        // We have to create a tile that has LEFT direction opposite operation 
                        // of the tile on the left's RIGHT direction operation.
                        Tile downTile = m_tiles[i, j - 1];
                        Tile.PosNegType rightOp = downTile.GetPosNegType(Tile.Direction.UP);
                        tile.SetPosNegType(Tile.Direction.DOWN, rightOp ==
                          Tile.PosNegType.NEG ?
                          Tile.PosNegType.POS : Tile.PosNegType.NEG);
                    }

                    // Right side tiles
                    if (i == NumTilesX - 1)
                    {
                        tile.SetPosNegType(Tile.Direction.RIGHT, Tile.PosNegType.NONE);
                    }
                    else
                    {
                        float toss = Random.Range(0.0f, 1.0f);
                        if (toss < 0.5f)
                        {
                            tile.SetPosNegType(Tile.Direction.RIGHT, Tile.PosNegType.POS);
                        }
                        else
                        {
                            tile.SetPosNegType(Tile.Direction.RIGHT, Tile.PosNegType.NEG);
                        }
                    }

                    // Up side tiles
                    if (j == NumTilesY - 1)
                    {
                        tile.SetPosNegType(Tile.Direction.UP, Tile.PosNegType.NONE);
                    }
                    else
                    {
                        float toss = Random.Range(0.0f, 1.0f);
                        if (toss < 0.5f)
                        {
                            tile.SetPosNegType(Tile.Direction.UP, Tile.PosNegType.POS);
                        }
                        else
                        {
                            tile.SetPosNegType(Tile.Direction.UP, Tile.PosNegType.NEG);
                        }
                    }

                    tile.Apply();

                    m_tiles[i, j] = tile;

                    // Create a game object for the tile.
                    m_tileObjs[i, j] = Tile.CreateGameObjectFromTile(tile);

                    if (m_tilesParent != null)
                    {
                        m_tileObjs[i, j].transform.SetParent(m_tilesParent);
                    }
                }
                yield return null;
            }
        }
    }
}
