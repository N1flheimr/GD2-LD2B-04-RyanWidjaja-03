using System.Collections.Generic;
using UnityEngine;

namespace NifuDev
{
    public static class ImageManager
    {
        public enum Medal
        {
            Bronze,
            Silver,
            Gold
        }

        public static Sprite GetMedalSprite(Medal medalType)
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

