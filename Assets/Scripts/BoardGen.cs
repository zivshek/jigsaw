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

        void Start()
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
    }
}
