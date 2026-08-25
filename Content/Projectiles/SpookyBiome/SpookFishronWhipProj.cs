using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

using Spooky.Content.Buffs.WhipDebuff;

namespace Spooky.Content.Projectiles.SpookyBiome
{
	public class SpookFishronWhipProj : ModProjectile
	{
        private static Asset<Texture2D> ProjTexture;
		private static Asset<Texture2D> GlowTexture;

        public override void SetStaticDefaults() 
		{
			ProjectileID.Sets.IsAWhip[Type] = true;
		}

		public override void SetDefaults() 
		{
			Projectile.DefaultToWhip();

			Projectile.WhipSettings.Segments = 65;
			Projectile.WhipSettings.RangeMultiplier = 2f;
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) 
		{
			Player owner = Main.player[Projectile.owner];

			owner.MinionAttackTargetNPC = target.whoAmI;
			Projectile.damage = (int)(damageDone * 0.8f);

			target.AddBuff(ModContent.BuffType<SpookFishronWhipDebuff>(), 300);
        }

		public override void PostDraw(Color lightColor)
        {
			ProjTexture ??= ModContent.Request<Texture2D>(Texture);

			List<Vector2> list = new();
			Projectile.FillWhipControlPoints(Projectile, list);

			Main.instance.LoadProjectile(Type);

			Vector2 pos = list[0];

			for (int i = 0; i < list.Count - 1; i++) 
            {
				Rectangle frame = new Rectangle(0, 0, 22, 18);
				Vector2 origin = new Vector2(11, 11);
				float scale = 1;

				//tip of the whip
				if (i == list.Count - 2) 
				{
					frame.Y = 64;
					frame.Height = 28;
				}
				//middle segments
				else if (i == list.Count - 3) 
				{
					frame.Y = 50;
					frame.Height = 12;
				}
				else if (i == 1) 
				{
					frame.Y = 22;
					frame.Height = 12;
				}
				else if (i > 0) 
				{
					frame.Y = 36;
					frame.Height = 12;
				}

				Vector2 element = list[i];
				Vector2 diff = list[i + 1] - element;

				float rotation = diff.ToRotation() - MathHelper.PiOver2;

				var effects = Projectile.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

				//draw the whip itself
				Color color = Lighting.GetColor(element.ToTileCoordinates());
				Main.EntitySpriteDraw(ProjTexture.Value, pos - Main.screenPosition, frame, color, rotation, origin, scale, effects, 0);

				pos += diff;
			}
		}

        public override bool PreDraw(ref Color lightColor) 
        {
			GlowTexture ??= ModContent.Request<Texture2D>(Texture + "Glow");

            List<Vector2> list = new();
			Projectile.FillWhipControlPoints(Projectile, list);

			Main.instance.LoadProjectile(Type);

			Vector2 pos = list[0];

			for (int i = 0; i < list.Count - 1; i++) 
            {
				Rectangle frame = new Rectangle(0, 0, 22, 18);
				Vector2 origin = new Vector2(11, 11);
				float scale = 1;

				//tip of the whip
				if (i == list.Count - 2) 
				{
					frame.Y = 64;
					frame.Height = 28;
				}
				//middle segments
				else if (i == list.Count - 3) 
				{
					frame.Y = 50;
					frame.Height = 12;
				}
				else if (i == 1) 
				{
					frame.Y = 22;
					frame.Height = 12;
				}
				else if (i > 0) 
				{
					frame.Y = 36;
					frame.Height = 12;
				}

				Vector2 element = list[i];
				Vector2 diff = list[i + 1] - element;

				float rotation = diff.ToRotation() - MathHelper.PiOver2;

				//draw the whip glow outline
				Color glowColor = new Color(125, 125, 125, 0).MultiplyRGBA(Color.OrangeRed);
				var effects = Projectile.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

				for (int circle = 0; circle < 360; circle += 90)
            	{
					Vector2 circular = new Vector2(Main.rand.NextFloat(1f, 5f), Main.rand.NextFloat(1f, 5f)).RotatedBy(MathHelper.ToRadians(circle));

					Main.EntitySpriteDraw(GlowTexture.Value, pos - Main.screenPosition + circular, frame, glowColor, rotation, origin, scale, effects, 0);
				}

				pos += diff;
			}

			return false;
		}
	}
}