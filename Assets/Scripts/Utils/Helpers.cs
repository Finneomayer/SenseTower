using UnityEngine;

namespace Assets.Scripts.Utils
{
    public class Helpers
    {
        public static Texture2D ResizeTexture(Texture2D originalTexture, Vector2 newSize)
        {
            // Проверяем, что оригинальный спрайт не равен null
            if (originalTexture == null)
            {
                Debug.LogError("Original sprite is null!");
                return null;
            }

            // Получаем текстуру из оригинального спрайта

            // Создаем новую текстуру
            Texture2D resizedTexture = new Texture2D((int)newSize.x, (int)newSize.y, originalTexture.format, false);
        
            // Копируем пиксели из оригинальной текстуры в новую с использованием метода Resize
            Color[] originalPixels = originalTexture.GetPixels();
            Color[] resizedPixels = new Color[resizedTexture.width * resizedTexture.height];

            for (int y = 0; y < resizedTexture.height; y++)
            {
                for (int x = 0; x < resizedTexture.width; x++)
                {
                    // Находим соответствующий пиксель в оригинальной текстуре
                    int originalX = Mathf.FloorToInt((float)x / resizedTexture.width * originalTexture.width);
                    int originalY = Mathf.FloorToInt((float)y / resizedTexture.height * originalTexture.height);

                    // Проверяем границы
                    originalX = Mathf.Clamp(originalX, 0, originalTexture.width - 1);
                    originalY = Mathf.Clamp(originalY, 0, originalTexture.height - 1);

                    // Копируем пиксель
                    resizedPixels[y * resizedTexture.width + x] = originalPixels[originalY * originalTexture.width + originalX];
                }
            }

            // Применяем новые пиксели к текстуре
            resizedTexture.SetPixels(resizedPixels);
            resizedTexture.Apply();

            return resizedTexture;
        }
    }
}