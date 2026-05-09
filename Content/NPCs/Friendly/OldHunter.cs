using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.UI;
using Terraria.Localization;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.UserInterfaces;

namespace Spooky.Content.NPCs.Friendly
{
    [AutoloadHead]
    public class OldHunter : ModNPC  
    {
        Vector2 modifier = new(-200, -75);

        Player PlayerTalkingTo = null;

        public static Mod Mod = Spooky.mod;

        private static Asset<Texture2D> UITexture;

        public static readonly SoundStyle TalkSound = new("Spooky/Content/Sounds/TalkSounds/OldHunterTalk", SoundType.Sound) { Volume = 3f, PitchVariance = 0.5f };

        public override void Load()
		{
			UITexture = ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/DialogueUIOldHunter");
		}

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 7;
            NPCID.Sets.ActsLikeTownNPC[Type] = true;
            NPCID.Sets.ShimmerTownTransform[Type] = false;
            NPCID.Sets.NoTownNPCHappiness[Type] = true;

            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
        }
        
        public override void SetDefaults()
		{
            NPC.lifeMax = 250;
            NPC.defense = 5;
            NPC.width = 38;
			NPC.height = 56;
            NPC.npcSlots = 0f;
            NPC.townNPC = true;
			NPC.friendly = true;
			NPC.immortal = true;
			NPC.dontTakeDamage = true;
			NPC.dontCountMe = true;
			TownNPCStayingHomeless = true;
			NPC.knockBackResist = 0f;
            NPC.HitSound = SoundID.NPCHit2;
			NPC.DeathSound = SoundID.NPCDeath2;
            NPC.aiStyle = 7;
        }

        public override void FindFrame(int frameHeight)
        {   
            //walking animation
            NPC.frameCounter++;
            if (NPC.frameCounter > 6)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= frameHeight * 7)
            {
                NPC.frame.Y = 1 * frameHeight;
            }

            //still frame
            if (NPC.velocity == Vector2.Zero)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
        }

		public override bool CanBeHitByNPC(NPC attacker)
		{
			return false;
		}

		public override bool? CanBeHitByProjectile(Projectile projectile)
		{
			return false;
		}

		public override bool? CanBeHitByItem(Player player, Item item)
		{
			return false;
		}

		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
		{
			return false;
		}

		public override bool CanChat() 
        {
			return true;
		}

        public override string GetChat()
		{
            if (!Flags.OldHunterDefeatDialogue)
            {
                DialogueChain chain = new();
                chain.Add(new(UITexture.Value, NPC,
                Language.GetTextValue("Mods.Spooky.Dialogue.OldHunterDialogue.Defeat1"),
                Language.GetTextValue("Mods.Spooky.Dialogue.OldHunterDialogue.PlayerDefeat1"),
                TalkSound, 2f, 0f, modifier, NPCID: NPC.type))
                .Add(new(UITexture.Value, NPC,
                Language.GetTextValue("Mods.Spooky.Dialogue.OldHunterDialogue.Defeat2"),
                Language.GetTextValue("Mods.Spooky.Dialogue.OldHunterDialogue.PlayerDefeat2"),
                TalkSound, 2f, 0f, modifier, NPCID: NPC.type))
                .Add(new(UITexture.Value, NPC,
                Language.GetTextValue("Mods.Spooky.Dialogue.OldHunterDialogue.Defeat3"),
                Language.GetTextValue("Mods.Spooky.Dialogue.OldHunterDialogue.PlayerDefeat3"),
                TalkSound, 2f, 0f, modifier, NPCID: NPC.type))
                .Add(new(UITexture.Value, NPC,
                Language.GetTextValue("Mods.Spooky.Dialogue.OldHunterDialogue.Defeat4"),
                Language.GetTextValue("Mods.Spooky.Dialogue.OldHunterDialogue.PlayerDefeat4"),
                TalkSound, 2f, 0f, modifier, NPCID: NPC.type))
                .Add(new(UITexture.Value, NPC,
                Language.GetTextValue("Mods.Spooky.Dialogue.OldHunterDialogue.Defeat5"),
                Language.GetTextValue("Mods.Spooky.Dialogue.OldHunterDialogue.PlayerDefeat5"),
                TalkSound, 2f, 0f, modifier, NPCID: NPC.type))
                .Add(new(UITexture.Value, NPC, null, null, TalkSound, 2f, 0f, modifier, true));
                chain.OnPlayerResponseTrigger += PlayerResponse;
                chain.OnEndTrigger += EndDialogue;
                DialogueUI.Visible = true;
                DialogueUI.Add(chain);
            }
            if (Flags.OldHunterDefeatDialogue && !DialogueUI.Visible)
            {
                OldHunterDialogueChoiceUI.OldHunter = NPC.whoAmI;
                OldHunterDialogueChoiceUI.UIOpen = true;
            }
            return string.Empty;
		}

		public override bool CheckActive()
		{
			return false;
		}

		public override bool NeedSaving()
		{
			return true;
		}
        
        public override void AI()
		{
			NPC.spriteDirection = NPC.direction;

            if ((NPC.velocity.X < 0f && NPC.direction == -1) || (NPC.velocity.X > 0f && NPC.direction == 1))
            {
                if (NPCGlobalHelper.IsCollidingWithFloor(NPC) && Collision.SolidTilesVersatile((int)(NPC.Center.X / 16f), (int)(NPC.Center.X + NPC.spriteDirection * 35) / 16, (int)NPC.Top.Y / 16, (int)NPC.Bottom.Y / 16 - 3))
                {
                    NPC.velocity.X = -NPC.velocity.X;
                    NPC.direction = -NPC.direction;
                    NPC.netUpdate = true;
                }
            }
        }

        public void PlayerResponse(Dialogue dialogue, string Text, int ID)
		{
			Dialogue newDialogue = new(ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/DialogueUIPlayer").Value, Main.LocalPlayer,
			Text, null, SoundID.Item1, 2f, 0f, default, NotPlayer: false);
			DialogueUI.Visible = true;
			DialogueUI.Add(newDialogue);
		}

        public void EndDialogue(Dialogue dialogue, int ID)
		{
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)SpookyMessageType.OldHunterDefeatDialogue);
                packet.Send();
            }
            else
            {
                Flags.OldHunterDefeatDialogue = true;
            }

			PlayerTalkingTo = null;
			DialogueUI.Visible = false;
		}
    }
}