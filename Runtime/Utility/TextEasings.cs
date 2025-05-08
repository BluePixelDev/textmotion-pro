using UnityEngine;

namespace BP.TextMotion
{
    public static class TextEasings
    {
        public static float EaseOutBack(float x)
        {
            const float c1 = 1.70158f;
            const float c3 = c1 + 1;
            return 1 + c3 * Mathf.Pow(x - 1, 3) + c1 * Mathf.Pow(x - 1, 2);
        }

        public static float EaseInBack(float x)
        {
            const float c1 = 1.70158f;
            const float c3 = c1 + 1;
            return c3 * x * x * x - c1 * x * x;
        }
    }
}
