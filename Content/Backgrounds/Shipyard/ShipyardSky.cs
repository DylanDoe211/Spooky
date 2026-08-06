using Terraria;
using Terraria.ModLoader;
using Terraria.Graphics.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;

namespace Spooky.Content.Backgrounds.Shipyard
{
    public class ShipyardSky : CustomSky
    {
        public bool skyActive;
        public float opacity;

        public override void Update(GameTime gameTime)
        {
            if (skyActive && opacity < 1f)
            {
                opacity += 0.01f;
            }
            else if (!skyActive && opacity > 0f)
            {
                opacity -= 0.025f;
            }
        }

		//sky color code from: https://github.com/GabeHasWon/SpiritReforged/blob/master/Content/Savanna/Biome/SavannaSky.cs
        public static float TimeProgress()
        {
            if (Main.dayTime)
            {
                return (float)Math.Sin(Math.PI * Main.time / Main.dayLength);
            }
            else
            {
                return (float)Math.Sin(Math.PI * Main.time / Main.nightLength);
            }
        }

		private static Color SkyColor()
		{
			float sunRiseSetFactor = 1 - TimeProgress();
			float midDayFactor = Main.dayTime ? TimeProgress() : 0;

			sunRiseSetFactor = Main.dayTime ? EaseFunction.EaseQuadOut.Ease(sunRiseSetFactor) : EaseFunction.EaseCircularIn.Ease(sunRiseSetFactor);

			var midDayColor = new Color(0, 255, 238, 200);
			var sunRiseSetColor = new Color(113, 96, 237);

			var finalColor = Color.Lerp(sunRiseSetColor, midDayColor, EaseFunction.EaseQuadOut.Ease(midDayFactor));

			//Make it slightly dimmer during the sunrise
			float sunRiseFactor = EaseFunction.EaseCircularOut.Ease((float)(Main.time / Main.dayLength));
			if (!Main.dayTime)
			{
				sunRiseFactor = 1 - EaseFunction.EaseCircularOut.Ease((float)(Main.time / Main.nightLength));
			}

			sunRiseFactor = MathHelper.Lerp(sunRiseFactor, 1, 0.7f);

			return finalColor * Math.Min(midDayFactor + sunRiseSetFactor, 1) * sunRiseFactor;
		}

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {    
            if (maxDepth >= 3E+38f && minDepth < 3E+38f && !Main.gameMenu)
            {
				Texture2D SkyTexture = ModContent.Request<Texture2D>("Spooky/Content/Backgrounds/Shipyard/ShipyardSky").Value;

				float dayProgress = Main.dayTime ? TimeProgress() : 0;
				dayProgress = EaseFunction.EaseQuadOut.Ease(dayProgress);
				Color skyColor = SkyColor() * opacity;

				float invertedDayProgress(float minValue) => Math.Max((1 - dayProgress), minValue);
				Color gradientColor = Color.Lerp(skyColor, Color.White * opacity * dayProgress * invertedDayProgress(0.25f), 0.1f);
				int verticalOffset = (int)MathHelper.Lerp(0, -100, dayProgress);

				spriteBatch.Draw(SkyTexture,
				new Rectangle(0, verticalOffset, Main.screenWidth, Main.screenHeight),
				null, Color.Lerp(skyColor, Color.White * opacity * dayProgress * invertedDayProgress(0.25f), 0.1f));

				spriteBatch.Draw(SkyTexture,
				new Rectangle(0, Main.screenHeight + verticalOffset, Main.screenWidth, -verticalOffset),
				new Rectangle(0, SkyTexture.Height - 1, SkyTexture.Width, 1), gradientColor);
			}

            //deactivate the sky if in the menu
            if (Main.gameMenu || !Main.LocalPlayer.active)  
            {
                skyActive = false;
            }
        }

        public override void Activate(Vector2 position, params object[] args)
        {
            opacity = 0.002f;
            skyActive = true;
        }

        public override void Deactivate(params object[] args)
        {
            skyActive = false;
        }

        public override void Reset()
        {
            skyActive = false;
        }

        public override bool IsActive()
        {
            return skyActive || opacity > 0.001f;
        }
    }
}
