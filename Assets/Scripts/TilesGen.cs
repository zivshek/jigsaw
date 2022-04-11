using UnityEngine;

public class TilesGen : MonoBehaviour
{
    [SerializeField]
    private string m_imageFilename;

    private Texture2D m_textureOriginal;

    void Start()
    {
        CreateBaseTexture();
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
}
