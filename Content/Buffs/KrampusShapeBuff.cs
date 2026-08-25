using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Core;
using Spooky.Content.Items.Minibiomes.Christmas;

namespace Spooky.Content.Buffs
{
	public class KrampusShapeBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.buffNoSave[Type] = true;
		}

		public override void PostDraw(SpriteBatch spriteBatch, int buffIndex, BuffDrawParams drawParams)
        {
			Player player = Main.LocalPlayer;

			if (player.GetModPlayer<KrampusShapeBoxPlayer>().KrampusShapeBuffStacks >= 1)
			{
				spriteBatch.Draw(ModContent.Request<Texture2D>("Spooky/Content/Buffs/KrampusShapeBuff1").Value, 
				drawParams.Position, null, drawParams.DrawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
			}
			if (player.GetModPlayer<KrampusShapeBoxPlayer>().KrampusShapeBuffStacks >= 2)
			{
				spriteBatch.Draw(ModContent.Request<Texture2D>("Spooky/Content/Buffs/KrampusShapeBuff2").Value, 
				drawParams.Position, null, drawParams.DrawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
			}
			if (player.GetModPlayer<KrampusShapeBoxPlayer>().KrampusShapeBuffStacks >= 3)
			{
				spriteBatch.Draw(ModContent.Request<Texture2D>("Spooky/Content/Buffs/KrampusShapeBuff3").Value, 
				drawParams.Position, null, drawParams.DrawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
			}
			if (player.GetModPlayer<KrampusShapeBoxPlayer>().KrampusShapeBuffStacks >= 4)
			{	
				spriteBatch.Draw(ModContent.Request<Texture2D>("Spooky/Content/Buffs/KrampusShapeBuff4").Value, 
				drawParams.Position, null, drawParams.DrawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
			}
			if (player.GetModPlayer<KrampusShapeBoxPlayer>().KrampusShapeBuffStacks >= 5)
			{
				spriteBatch.Draw(ModContent.Request<Texture2D>("Spooky/Content/Buffs/KrampusShapeBuff5").Value, 
				drawParams.Position, null, drawParams.DrawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
			}
		}

		public override void Update(Player player, ref int buffIndex)
		{
			int numStacks = player.GetModPlayer<KrampusShapeBoxPlayer>().KrampusShapeBuffStacks;

			int StatDefense = 2 * numStacks;
			float StatDamage = 0.03f * numStacks;
			float StatAttackSpeed = 0.07f * numStacks;

			player.statDefense += StatDefense;
			player.GetDamage(DamageClass.Generic) += StatDamage;
			player.GetAttackSpeed(DamageClass.Generic) += StatAttackSpeed;
		}
	}
}
