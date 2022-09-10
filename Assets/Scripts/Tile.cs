using System;
using System.Collections.Generic;
using UnityEngine;

namespace Puzzle
{
    public class Tile
    {
        public enum Direction
        {
            UP, RIGHT, DOWN, LEFT
        }

        public enum PosNegType
        {
            POS, NEG, NONE
        }

        public static int Padding = 15;
        public static int TileSize = 100;

        public static readonly List<Vector2> ControlPoints = new List<Vector2>()
        {
            new Vector2(0, 0),
            new Vector2(35, 15),
            new Vector2(47, 13),
            new Vector2(45, 5),
            new Vector2(48, 0),
            new Vector2(25, -5),
            new Vector2(15, -18),
            new Vector2(36, -20),
            new Vector2(64, -20),
            new Vector2(85, -18),
            new Vector2(75, -5),
            new Vector2(52, 0),
            new Vector2(55, 5),
            new Vector2(53, 13),
            new Vector2(65, 15),
            new Vector2(100, 0)
        };

        public static List<Vector2> BezCurve = BezierCurve.PointList2(ControlPoints, 0.001f);
        public static TilesSorting TilesSorting { get; private set; } = new TilesSorting();

        public Texture2D FinalCut { get; private set; }
        public int XIndex { get; private set; }
        public int YIndex { get; private set; }

        public PosNegType GetPosNegType(Direction dir)
        {
            return m_PosNeg[(int)dir];
        }

        private PosNegType[] m_PosNeg = new PosNegType[4]
        {
            PosNegType.NONE,
            PosNegType.NONE,
            PosNegType.NONE,
            PosNegType.NONE
        };

        private Texture2D m_OriginalTex;
        private bool[,] m_Visited;
        private Stack<Vector2Int> m_Stack = new Stack<Vector2Int>();
        private Dictionary<(Direction, PosNegType), LineRenderer> m_LineRenderers = new Dictionary<(Direction, PosNegType), LineRenderer>();

        public void SetPosNegType(Direction dir, PosNegType type)
        {
            m_PosNeg[(int)dir] = type;
        }

        public Tile(Texture2D tex, int xIndex, int yIndex)
        {
            int tileSizeWithPadding = 2 * Padding + TileSize;
            //if (tex.width != tileSizeWithPadding ||
            //  tex.height != tileSizeWithPadding)
            //{
            //    Debug.Log("Unsupported texture dimension for Jigsaw tile");
            //    return;
            //}

            m_OriginalTex = tex;
            XIndex = xIndex;
            YIndex = yIndex;

            // Create a new texture with width and height as Padding + TileSize + Padding.
            FinalCut = new Texture2D(
              tileSizeWithPadding,
              tileSizeWithPadding,
              TextureFormat.ARGB32,
              false);

            var transparentColor = new Color(1.0f, 1.0f, 1.0f, 0.0f);

            // Initialize the newly create texture with transparent colour.
            for (int i = 0; i < tileSizeWithPadding; ++i)
            {
                for (int j = 0; j < tileSizeWithPadding; ++j)
                {
                    FinalCut.SetPixel(i, j, transparentColor);
                }
            }
        }

        public void Apply()
        {
            FloodFillInit();
            FloodFill();
            FinalCut.Apply();
        }

        private void FloodFillInit()
        {
            int tileSizeWithPadding = 2 * Padding + TileSize;

            m_Visited = new bool[tileSizeWithPadding, tileSizeWithPadding];
            for (int i = 0; i < tileSizeWithPadding; ++i)
            {
                for (int j = 0; j < tileSizeWithPadding; ++j)
                {
                    m_Visited[i, j] = false;
                }
            }

            List<Vector2> pts = new List<Vector2>();
            for (int i = 0; i < m_PosNeg.Length; ++i)
            {
                pts.AddRange(CreateCurve((Direction)i, m_PosNeg[i]));
            }

            // Now we should have a closed curve.
            // To verify check by drawing the pts to a line renderer.
            for (int i = 0; i < pts.Count; ++i)
            {
                m_Visited[(int)pts[i].x, (int)pts[i].y] = true;
            }

            // start from center.
            Vector2Int start = new Vector2Int(tileSizeWithPadding / 2, tileSizeWithPadding / 2);

            m_Visited[start.x, start.y] = true;
            m_Stack.Push(start);
        }

        private void FloodFill()
        {
            int width_height = Padding * 2 + TileSize;
            while (m_Stack.Count > 0)
            {
                //FloodFillStep();
                Vector2Int v = m_Stack.Pop();

                int xx = v.x;
                int yy = v.y;
                Fill(v.x, v.y);

                // check right.
                int x = xx + 1;
                int y = yy;
                if (x < width_height)
                {
                    if (!m_Visited[x, y])
                    {
                        m_Visited[x, y] = true;
                        m_Stack.Push(new Vector2Int(x, y));
                    }
                }

                // check left.
                x = xx - 1;
                y = yy;
                if (x >= 0)
                {
                    if (!m_Visited[x, y])
                    {
                        m_Visited[x, y] = true;
                        m_Stack.Push(new Vector2Int(x, y));
                    }
                }

                // check up.
                x = xx;
                y = yy + 1;
                if (y < width_height)
                {
                    if (!m_Visited[x, y])
                    {
                        m_Visited[x, y] = true;
                        m_Stack.Push(new Vector2Int(x, y));
                    }
                }

                // check down.
                x = xx;
                y = yy - 1;
                if (y >= 0)
                {
                    if (!m_Visited[x, y])
                    {
                        m_Visited[x, y] = true;
                        m_Stack.Push(new Vector2Int(x, y));
                    }
                }
            }
        }

        private void Fill(int x, int y)
        {
            Color c = m_OriginalTex.GetPixel(x + XIndex * TileSize, y + YIndex * TileSize);
            c.a = 1.0f;
            FinalCut.SetPixel(x, y, c);
        }

        private static List<Vector2> CreateCurve(Direction dir, PosNegType type)
        {
            int padding_x = Padding;
            int padding_y = Padding;
            int sw = TileSize;
            int sh = TileSize;

            List<Vector2> pts = new List<Vector2>(Tile.BezCurve);
            switch (dir)
            {
                case Direction.UP:
                    if (type == PosNegType.POS)
                    {
                        TranslatePoints(pts, new Vector3(padding_x, padding_y + sh, 0));
                    }
                    else if (type == PosNegType.NEG)
                    {
                        InvertY(pts);
                        TranslatePoints(pts, new Vector3(padding_x, padding_y + sh, 0));
                    }
                    else if (type == PosNegType.NONE)
                    {
                        pts.Clear();
                        for (int i = 0; i < 100; ++i)
                        {
                            pts.Add(new Vector2(i + padding_x, padding_y + sh));
                        }
                    }
                    break;
                case Direction.RIGHT:
                    if (type == PosNegType.POS)
                    {
                        SwapXY(pts);
                        TranslatePoints(pts, new Vector3(padding_x + sw, padding_y, 0));
                    }
                    else if (type == PosNegType.NEG)
                    {
                        InvertY(pts);
                        SwapXY(pts);
                        TranslatePoints(pts, new Vector3(padding_x + sw, padding_y, 0));
                    }
                    else if (type == PosNegType.NONE)
                    {
                        pts.Clear();
                        for (int i = 0; i < 100; ++i)
                        {
                            pts.Add(new Vector2(padding_x + sw, i + padding_y));
                        }
                    }
                    break;
                case Direction.DOWN:
                    if (type == PosNegType.POS)
                    {
                        InvertY(pts);
                        TranslatePoints(pts, new Vector3(padding_x, padding_y, 0));
                    }
                    else if (type == PosNegType.NEG)
                    {
                        TranslatePoints(pts, new Vector3(padding_x, padding_y, 0));
                    }
                    else if (type == PosNegType.NONE)
                    {
                        pts.Clear();
                        for (int i = 0; i < 100; ++i)
                        {
                            pts.Add(new Vector2(i + padding_x, padding_y));
                        }
                    }
                    break;
                case Direction.LEFT:
                    if (type == PosNegType.POS)
                    {
                        InvertY(pts);
                        SwapXY(pts);
                        TranslatePoints(pts, new Vector3(padding_x, padding_y, 0));
                    }
                    else if (type == PosNegType.NEG)
                    {
                        SwapXY(pts);
                        TranslatePoints(pts, new Vector3(padding_x, padding_y, 0));
                    }
                    else if (type == PosNegType.NONE)
                    {
                        pts.Clear();
                        for (int i = 0; i < 100; ++i)
                        {
                            pts.Add(new Vector2(padding_x, i + padding_y));
                        }
                    }
                    break;
            }
            return pts;
        }

        public static LineRenderer CreateLineRenderer(Color color, float lineWidth = 1.0f)
        {
            GameObject obj = new GameObject();

            LineRenderer lr = obj.AddComponent<LineRenderer>();

            lr.startColor = color;
            lr.endColor = color;
            lr.startWidth = lineWidth;
            lr.endWidth = lineWidth;
            lr.material = new Material(Shader.Find("Sprites/Default"));
            return lr;
        }

        public void DrawCurve(Direction dir, PosNegType type, Color color)
        {
            if (!m_LineRenderers.ContainsKey((dir, type)))
            {
                m_LineRenderers.Add((dir, type), CreateLineRenderer(color));
            }

            LineRenderer lr = m_LineRenderers[(dir, type)];
            lr.startColor = color;
            lr.endColor = color;
            lr.gameObject.name = "LineRenderer_" + dir.ToString() + "_" + type.ToString();
            List<Vector2> pts = CreateCurve(dir, type);

            lr.positionCount = pts.Count;
            for (int i = 0; i < pts.Count; ++i)
            {
                lr.SetPosition(i, pts[i]);
            }
        }

        public static GameObject CreateGameObjectFromTile(Tile tile)
        {
            // Create a game object for the tile.
            GameObject obj = new GameObject();

            // Give a name that is recognizable for the GameObject.
            obj.name = "TileGameObj_" + tile.XIndex + "_" + tile.YIndex;

            // Set the position of this GameObject.
            // We will use the xIndex and yIndex to find the actual 
            // position of the tile. We can get this position by multiplying
            // xIndex by TileSize and yIndex by TileSize.
            obj.transform.position = new Vector3(tile.XIndex * TileSize, tile.YIndex * TileSize, 0.0f);

            // Create a SpriteRenderer.
            SpriteRenderer spriteRenderer = obj.AddComponent<SpriteRenderer>();
            spriteRenderer.sortingLayerName = "Tiles";

            // Set the sprite created with the FinalCut 
            // texture of the tile to the SpriteRenderer
            spriteRenderer.sprite = SpriteUtils.CreateSpriteFromTexture2D(
                tile.FinalCut,
                0,
                0,
                Padding * 2 + TileSize,
                Padding * 2 + TileSize);

            // Add a box colliders so that we can handle 
            // picking/selection of the Tiles.
            BoxCollider2D box = obj.AddComponent<BoxCollider2D>();
            int boxSize = Math.Max(TileSize - Padding, Padding);
            box.size = new Vector2(boxSize, boxSize);

            // add the TileMovement script component.
            TileMovement tm = obj.AddComponent<TileMovement>();
            tm.Tile = tile;

            TilesSorting.Add(spriteRenderer);

            return obj;
        }

        static void TranslatePoints(List<Vector2> iList, Vector2 offset)
        {
            for (int i = 0; i < iList.Count; ++i)
            {
                iList[i] += offset;
            }
        }

        static void InvertY(List<Vector2> iList)
        {
            for (int i = 0; i < iList.Count; ++i)
            {
                iList[i] = new Vector2(iList[i].x, -iList[i].y);
            }
        }

        static void SwapXY(List<Vector2> iList)
        {
            for (int i = 0; i < iList.Count; ++i)
            {
                iList[i] = new Vector2(iList[i].y, iList[i].x);
            }
        }
    }
}