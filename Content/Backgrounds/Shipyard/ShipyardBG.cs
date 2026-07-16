using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Reflection;

using Spooky.Core;

namespace Spooky.Content.Backgrounds.Shipyard
{
	public class ShipyardBG : ModSurfaceBackgroundStyle
	{
		public override int ChooseCloseTexture(ref float scale, ref double parallax, ref float a, ref float b) => BackgroundTextureLoader.GetBackgroundSlot("Spooky/Content/Backgrounds/Shipyard/ShipyardBG1");

		public override int ChooseMiddleTexture() => BackgroundTextureLoader.GetBackgroundSlot("Spooky/Content/Backgrounds/Shipyard/ShipyardBG2");

		public override int ChooseFarTexture() => BackgroundTextureLoader.GetBackgroundSlot("Spooky/Content/Backgrounds/Shipyard/ShipyardBG3");

		public override void ModifyFarFades(float[] fades, float transitionSpeed)
		{
			for (int i = 0; i < fades.Length; i++)
			{
				if (i == Slot)
				{
					fades[i] += transitionSpeed;
					if (fades[i] > 1f)
					{
						fades[i] = 1f;
					}
				}
				else
				{
					fades[i] -= transitionSpeed;
					if (fades[i] < 0f)
					{
						fades[i] = 0f;
					}
				}
			}
		}
    }
}