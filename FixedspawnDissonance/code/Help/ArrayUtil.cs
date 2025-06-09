using System.Linq;

//Thank you RedGesture mod and AI Blacklister mod
namespace VanillaArtifactsPlus
{
    public static class Util
    {
        public static T[] Add<T>(this T[] array, params T[] items)
        {
            return (array ?? Enumerable.Empty<T>()).Concat(items).ToArray();
        }

        public static T[] Remove<T>(this T[] array, params T[] items)
        {
            return (array ?? Enumerable.Empty<T>()).Except(items).ToArray();
        }
    }
}
