using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Content.Projectiles.SpookyBiome;

namespace Spooky.Content.Items.SpookyBiome
{
    public class CreepyCandelabra : ModItem
    {
        private static Asset<Texture2D> GlowTexture;

        public override void SetStaticDefaults()
        {
            GlowTexture = ModContent.Request<Texture2D>(Texture + "Glow");

            Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 40;
            Item.mana = 15;
			Item.DamageType = DamageClass.Magic;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.width = 56;
            Item.height = 58;
            Item.useTime = 60;
			Item.useAnimation = 60;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 0;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.buyPrice(gold: 15);
            Item.UseSound = SoundID.Item43 with { Pitch = -1f };
            Item.shoot = ModContent.ProjectileType<LingeringCandle>();
            Item.shootSpeed = 10f;
        }

		public override bool ModifyItemDraw(ref PlayerDrawSet drawInfo, ref DrawData drawData, ref DrawData? coloredDrawData, ref DrawData? glowMaskDrawData)
		{
			drawInfo.DrawDataCache.Add(drawData);

			for (int i = 0; i < 360; i += 90)
			{
				Vector2 circular = new Vector2(Main.rand.NextFloat(0.5f, 1.5f), Main.rand.NextFloat(0.5f, 1.5f)).RotatedBy(MathHelper.ToRadians(i));
				drawInfo.DrawDataCache.Add(drawData with { texture = GlowTexture.Value, color = new Color(255, 255, 255, 0), position = drawData.position + circular });
			}

			return false;
		}
    }
}