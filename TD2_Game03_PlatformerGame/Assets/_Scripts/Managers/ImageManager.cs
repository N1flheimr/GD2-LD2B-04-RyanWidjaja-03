using System.Collections.Generic;
using UnityEngine;

namespace NifuDev
{
    public static class ImageManager
    {
        public enum MedalType
        {
            Bronze,
            Silver,
            Gold,
            NoMedal
        }

        public static Sprite GetMedalSprite(MedalType medalType)
        {
            foreach (GameAssets.Image image in GameAssets.Instance.imageArray)
            {
                if (image.medalType == medalType)
                {
                    return image.Sprite;
                }
            }
            return null;
        }
    }
}

