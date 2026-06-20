using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.Chat;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Core;
using Spooky.Content.Dusts;

namespace Spooky.Content.NPCs.Boss.Daffodil
{
	public class DaffodilBody : ModNPC
	{
        private static Asset<Texture2D> VineLeftTexture;
        private static Asset<Texture2D> VineRightTexture;
        private static Asset<Texture2D> FlowerTexture;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
        }

		public override void SetDefaults()
		{
            NPC.lifeMax = 5;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.width = 550;
            NPC.height = 302;
            NPC.npcSlots = 0f;
            NPC.knockBackResist = 0f;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.behindTiles = true;
            NPC.dontCountMe = true;
            NPC.aiStyle = -1;
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            VineLeftTexture ??= ModContent.Request<Texture2D>(Texture + "VineLeft");
            VineRightTexture ??= ModContent.Request<Texture2D>(Texture + "VineRight");
            FlowerTexture ??= ModContent.Request<Texture2D>(Texture + "Flower");

            Color color1 = Lighting.GetColor((int)NPC.Center.X / 16, (int)NPC.Center.Y / 16);
            Color color2 = Lighting.GetColor((int)(NPC.Center.X - 150) / 16, (int)(NPC.Center.Y - 60) / 16);
            Color color3 = Lighting.GetColor((int)(NPC.Center.X + 150) / 16, (int)(NPC.Center.Y - 60) / 16);

            Main.EntitySpriteDraw(ModContent.Request<Texture2D>(Texture).Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(color1), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(VineLeftTexture.Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(color2), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(VineRightTexture.Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(color3), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(FlowerTexture.Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(color1), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0);

            return false;
        }
        
        public override bool CheckActive()
        {
            return false;
        }

        public override void AI()
        {
            Flags.DaffodilSpawnX = (int)NPC.Center.X;
            Flags.DaffodilSpawnY = (int)NPC.Center.Y + 30;
            Flags.DaffodilParent = NPC.whoAmI;

            //sleepy particles
            if (!NPC.AnyNPCs(ModContent.NPCType<DaffodilEye>()))
            {
                if (!Main.gamePaused)
                {
                    if (Main.rand.NextBool(75))
                    {
                        Dust.NewDust(new Vector2(NPC.Center.X + Main.rand.Next(-50, 50), NPC.Center.Y + Main.rand.Next(-25, 25)), 5, 5, ModContent.DustType<DaffodilSleepyDust>());
                    }
                }
            }
        }
    }
}