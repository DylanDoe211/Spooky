using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using System.Collections.Generic;

namespace Spooky.Core
{
    internal static class DrawUtils
    {
        private static readonly Dictionary<Texture2D, Color[]> ColorCache = [];
	    private static readonly Dictionary<Texture2D, Texture2D> SolidTextureCache = [];

        public static Color Additive(this Color color, byte newAlpha = 0)
        {
            var temp = color;
            temp.A = (byte)(temp.A * newAlpha / byte.MaxValue);
            return temp;
        }

        public static Color[] GetColors(Texture2D texture)
        {
            if (ColorCache.TryGetValue(texture, out Color[] value))
                return value;

            var data = new Color[texture.Width * texture.Height];
            texture.GetData(data);

            //Orders colors from darkest to brightest, and excludes fully transparent pixels
            data = [.. data.OrderBy(x => x.ToVector3().Length()).Where(x => x != Color.Transparent)];

            if (data.Length != 0)
                ColorCache.Add(texture, data);
            //Fallback
            else
                data = [Color.Black];

            return data;
        }

        public static Texture2D ColorSolid(Texture2D texture, Color color)
        {
            if (SolidTextureCache.TryGetValue(texture, out var textureFromCache))
                return textureFromCache;

            var data = new Color[texture.Width * texture.Height];
            texture.GetData(data);

            for (int i = data.Length - 1; i >= 0; i--)
            {
                if (data[i] != Color.Transparent)
                {
                    byte alpha = data[i].A;
                    data[i] = color.Additive(alpha);
                }
            }

            var textureToCache = new Texture2D(Main.graphics.GraphicsDevice, texture.Width, texture.Height);
            textureToCache.SetData(data);

            SolidTextureCache.Add(texture, textureToCache);
            return textureToCache;
        }
    }
}