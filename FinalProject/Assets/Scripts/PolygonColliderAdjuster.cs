using System.Collections.Generic;
using UnityEngine;

public class PolygonColliderAdjuster : MonoBehaviour
{
    private PolygonCollider2D polyCollider;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        polyCollider = GetComponent<PolygonCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            FitColliderToSprite();
        }
    }

    void FitColliderToSprite()
    {
        if (polyCollider == null || spriteRenderer == null)
            return;

        // Xóa đường viền cũ
        polyCollider.pathCount = 0;

        // Lấy đường viền từ sprite
        Texture2D texture = spriteRenderer.sprite.texture;
        Vector2[] newPoints = GenerateColliderFromTexture(texture, spriteRenderer.sprite);

        if (newPoints.Length > 0)
        {
            polyCollider.pathCount = 1;
            polyCollider.SetPath(0, newPoints);
        }
    }

    Vector2[] GenerateColliderFromTexture(Texture2D texture, Sprite sprite)
    {
        // Lấy dữ liệu pixel của sprite
        Color[] pixels = texture.GetPixels();
        int width = texture.width;
        int height = texture.height;

        // Danh sách các điểm cạnh của nhân vật
        List<Vector2> colliderPoints = new List<Vector2>();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (pixels[x + y * width].a > 0.1f) // Lấy những pixel có alpha > 10% (không trong suốt)
                {
                    Vector2 worldPoint = new Vector2(
                        (x - width / 2) / sprite.pixelsPerUnit,
                        (y - height / 2) / sprite.pixelsPerUnit
                    );
                    colliderPoints.Add(worldPoint);
                }
            }
        }

        return colliderPoints.ToArray();
    }
}
