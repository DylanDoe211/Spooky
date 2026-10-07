using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.NPCs.Shipyard
{
	public class QueenConchIntro : ModNPC
	{
        public enum AnimationState
		{
			Emerge, PrepareThrow, Throw
		}

		private AnimationState CurrentAnimation
        {
			get => (AnimationState)NPC.ai[3];
			set => NPC.ai[3] = (float)value;
		}

        private static Asset<Texture2D> NPCTexture;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 10;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 5;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.width = 100;
			NPC.height = 84;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 50, 0);
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.ShipyardBiome>().Type };
        }

        public override void FindFrame(int frameHeight)
        {
			NPC.frameCounter++;
			if (NPC.frameCounter > 5)
			{
				NPC.frame.Y = NPC.frame.Y + frameHeight;
				NPC.frameCounter = 0;
			}

			if (CurrentAnimation == AnimationState.Emerge)
			{
				if (NPC.frame.Y >= frameHeight * 3)
				{
					NPC.frame.Y = 2 * frameHeight;
				}
			}
            else if (CurrentAnimation == AnimationState.PrepareThrow)
			{
				if (NPC.frame.Y >= frameHeight * 7)
				{
					NPC.frame.Y = 6 * frameHeight;
				}
			}
            else if (CurrentAnimation == AnimationState.Throw)
			{
				if (NPC.frame.Y >= frameHeight * 10)
				{
					NPC.frame.Y = 9 * frameHeight;
				}
			}
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPCTexture ??= ModContent.Request<Texture2D>(Texture);

            var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Vector2 drawOrigin = new Vector2(NPCTexture.Width() * 0.5f - 10, NPC.height * 0.5f);
            spriteBatch.Draw(NPCTexture.Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, drawOrigin, NPC.scale, effects, 0);
            
            return false;
        }

        public override void AI()
		{
            NPC.ai[0]++;
            if (NPC.ai[0] == 1)
            {
                NPC.direction = Main.rand.NextBool() ? -1 : 1;
                CurrentAnimation = AnimationState.Emerge;
            }

            if (NPC.ai[0] == 60)
            {
				SoundEngine.PlaySound(SoundID.NPCDeath19, NPC.Center);

				CurrentAnimation = AnimationState.Throw;
				NPC.velocity = new Vector2(Main.rand.Next(-3, 4), Main.rand.Next(-12, -8));
                NPC.netUpdate = true;
            }

            if (NPC.ai[0] >= 60 && NPC.ai[0] < 95)
            {
				NPC.rotation -= (Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) * 0.01f;
				NPC.velocity.Y += 0.15f;
			}

			if (NPC.ai[0] == 95)
			{
				SoundEngine.PlaySound(SoundID.DD2_GoblinBomberThrow with { Volume = 3f }, NPC.Center);

				Vector2 CrabLaunchPos = NPC.Center + new Vector2(40 * NPC.direction, 0).RotatedBy(NPC.rotation);

				Vector2 ShootSpeed = CrabLaunchPos - NPC.Center;
				ShootSpeed.Normalize();
				ShootSpeed *= 12f;

				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					int NewNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)CrabLaunchPos.X, (int)CrabLaunchPos.Y, NPCID.ZombieXmas);
					Main.npc[NewNPC].velocity = ShootSpeed;
					if (Main.netMode == NetmodeID.Server)
					{
						NetMessage.SendData(MessageID.SyncNPC, number: NewNPC);
					}
				}
			}

			if (NPC.ai[0] == 110)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int NewNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<QueenConch>());
                    Main.npc[NewNPC].velocity = NPC.velocity;
                    Main.npc[NewNPC].rotation = NPC.rotation;
                    Main.npc[NewNPC].alpha = 255;
                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendData(MessageID.SyncNPC, number: NewNPC);
                    }
                }

                NPC.active = false;
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0) 
            {
            }
        }
    }
}