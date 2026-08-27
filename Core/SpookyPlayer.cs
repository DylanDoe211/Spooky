using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.DataStructures;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Content.Biomes;
using Spooky.Content.Buffs.Debuff;
using Spooky.Content.Items.Fishing;
using Spooky.Content.Items.Fishing.Crate;
using Spooky.Content.Items.Pets;
using Spooky.Content.Items.SpookyBiome.Misc;
using Spooky.Content.Items.SpiderCave;
using Spooky.Content.Items.SpookyHell.Sentient;
using Spooky.Content.NPCs.Boss.SpookFishron;
using Spooky.Content.NPCs.SpookyHell;
using Spooky.Content.Projectiles.Minibiomes.Ocean;
using Spooky.Content.Projectiles.Sentient;
using Spooky.Content.Projectiles.SpiderCave;
using Spooky.Content.Projectiles.SpookyHell;

namespace Spooky.Core
{
    public class SpookyPlayer : ModPlayer
    {
        //pets
        public bool ColumboPet = false;
        public bool ColumbonePet = false;
        public bool ColumbooPet = false;
        public bool ColumborangePet = false;
        public bool CatPet = false;
        public bool FlyPet = false;
        public bool GhostPet = false;
        public bool InchwormPet = false;
        public bool PandoraBeanPet = false;
        public bool PetscopPet = false;
        public bool PetscopMarvinPet = false;
        public bool PetscopTiaraPet = false;
        public bool ShroomHopperPet = false;
        public bool SkullEmojiPet = false;
        public bool SkullGoopPet = false;
        public bool ValleyNautilusPet = false;
        public bool RotGourdPet = false;
        public bool SpookySpiritPet = false;
        public bool StickyEyePet = false;
        public bool MocoPet = false;
        public bool BigBonePet = false;
        public bool OrroboroPet = false;
        public bool SinisterSnailPet = false;
        public bool BeePet = false;
        public bool FuzzBatPet  = false;
        public bool PuttyPet = false;
        public bool RatPet = false;
        public bool ZombieCultistPet = false;
        public bool LongisquamaPet = false;
        public bool ChalupoPet = false;
        public bool MushroomFriendPet = false;
        public bool SludgePet = false;
        public bool FireflyPet = false;
        public bool CrabPet = false;

        //misc bools
        public bool RaveyardGuardsHostile = false;
        public bool SpiderGrottoCompass = false;
        public bool EyeValleyCompass = false;
        public bool NoseCultistDisguise1 = false;
		public bool NoseCultistDisguise2 = false;
		public bool NoseBlessingBuff = false;
        public bool DisablePlayerControls = false;
		public bool AlsoDisableEscapeKey = false;
		public bool RaveyardMonolithEquipped = false;
		public bool SporeMonolithEquipped = false;

		//misc timers
        public int SoulDrainCharge = 0;
		public int PotionSicknessCranberryTimer = 0;
		public int PotionSicknessLatteTimer = 0;
        public int SpearfishChargeCooldown = 0;

		public Vector2 MocoNoseUIPos = new Vector2(Main.screenWidth / 2 * Main.UIScale, Main.screenHeight / 1.75f * Main.UIScale);
		public Vector2 KidneyUIPos = new Vector2(Main.screenWidth / 2 * Main.UIScale, Main.screenHeight / 1.75f * Main.UIScale);
        public Vector2 ChimneyUIPos = new Vector2(Main.screenWidth / 2 * Main.UIScale, Main.screenHeight / 1.75f * Main.UIScale);

		private static Asset<Texture2D> SentientLeafBlowerBackTex;
        private static Asset<Texture2D> MiteVacuumBackTex;

		public override void SaveData(TagCompound tag)
		{
			tag[nameof(MocoNoseUIPos)] = MocoNoseUIPos;
			tag[nameof(KidneyUIPos)] = KidneyUIPos;
            tag[nameof(ChimneyUIPos)] = ChimneyUIPos;
		}

		public override void LoadData(TagCompound tag)
		{
			if (tag.ContainsKey(nameof(MocoNoseUIPos)))
			{
				MocoNoseUIPos = tag.Get<Vector2>(nameof(MocoNoseUIPos));
			}
            
			if (tag.ContainsKey(nameof(KidneyUIPos)))
			{
				KidneyUIPos = tag.Get<Vector2>(nameof(KidneyUIPos));
			}

            if (tag.ContainsKey(nameof(ChimneyUIPos)))
			{
				ChimneyUIPos = tag.Get<Vector2>(nameof(ChimneyUIPos));
			}
		}

		public override void OnEnterWorld()
        {
            //un-hide the sun if you enter the world with the spooky mod menu enabled since it hides the sun offscreen
            if (ModContent.GetInstance<SpookyMenu>().IsSelected)
            {
                Main.sunModY = 0;
            }
        }

        public override void ResetEffects()
        {
            //pets
            ColumboPet = false;
            ColumbonePet = false;
            ColumbooPet = false;
            ColumborangePet = false;
            CatPet = false;
            FlyPet = false;
            GhostPet = false;
            InchwormPet = false;
            PandoraBeanPet = false;
            PetscopPet = false;
            PetscopMarvinPet = false;
            PetscopTiaraPet = false;
            ShroomHopperPet = false;
            SkullEmojiPet = false;
            SkullGoopPet = false;
            ValleyNautilusPet = false;
            RotGourdPet = false;
            SpookySpiritPet = false;
            StickyEyePet = false;
            MocoPet = false;
            BigBonePet = false;
            OrroboroPet = false;
            SinisterSnailPet = false;
            BeePet = false;
            FuzzBatPet  = false;
            PuttyPet = false;
            RatPet = false;
            ZombieCultistPet = false;
            LongisquamaPet = false;
            ChalupoPet = false;
            MushroomFriendPet = false;
            SludgePet = false;
            FireflyPet = false;
            CrabPet = false;

            //misc bools
            SpiderGrottoCompass = false;
            EyeValleyCompass = false;
            NoseCultistDisguise1 = false;
			NoseCultistDisguise2 = false;
			NoseBlessingBuff = false;
            DisablePlayerControls = false;
			AlsoDisableEscapeKey = false;
			RaveyardMonolithEquipped = false;
			SporeMonolithEquipped = false;

			//prevent player from building in boss arenas, needs to be done in reset effects so the creative shock applies properly
			if (!SpookyWorld.IsInSubworld())
			{
				Rectangle DaffodilRect = new Rectangle((int)(Flags.DaffodilPosition.X - 750), (int)(Flags.DaffodilPosition.Y - 275), 1490, 600);
				Rectangle BigBoneRect = new Rectangle((int)(Flags.FlowerPotPosition.X - 835), (int)(Flags.FlowerPotPosition.Y - 500), 1650, 1050);
				Rectangle OldHunterRect = new Rectangle((int)(Flags.OldHunterPosition.X - 600), (int)(Flags.OldHunterPosition.Y - 400), 1200, 415);

				foreach (Player player in Main.ActivePlayers)
				{
					if (!player.dead && !player.ghost)
					{
						if ((Flags.DaffodilPosition != Vector2.Zero && player.Hitbox.Intersects(DaffodilRect)) ||
						(Flags.FlowerPotPosition != Vector2.Zero && player.Hitbox.Intersects(BigBoneRect)) ||
						(Flags.OldHunterPosition != Vector2.Zero && player.Hitbox.Intersects(OldHunterRect)))
						{
							player.AddBuff(BuffID.NoBuilding, 2);
							player.noBuilding = true;
						}
					}
				}
			}
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.active && target.CanBeChasedBy(this) && !target.friendly && !target.dontTakeDamage && !NPCID.Sets.CountsAsCritter[target.type])
            {
                //if the player has the nose blessing buff and hits an npc with the nose blessing debuff
                if (NoseBlessingBuff && Main.rand.NextBool(10) && Player.ownedProjectileCounts[ModContent.ProjectileType<SnotBlessingOrbiter>()] < 10 && !target.HasBuff(ModContent.BuffType<NoseBlessingDebuffCooldown>()))
                {
                    if (!target.HasBuff(ModContent.BuffType<NoseBlessingDebuff>()))
                    {
                        target.AddBuff(ModContent.BuffType<NoseBlessingDebuff>(), 360);
                    }
                    
                    if (target.HasBuff(ModContent.BuffType<NoseBlessingDebuff>()))
                    {
                        int distance = Main.rand.Next(0, 360);

                        Projectile.NewProjectile(target.GetSource_OnHit(target), target.Center, Vector2.Zero, ModContent.ProjectileType<SnotBlessingOrbiter>(), damageDone * 2, 3, Player.whoAmI, target.whoAmI, distance);
                    }
                }

                //spawn blades when hitting enemies with whips for each possessed crown you have
                if (Player.ownedProjectileCounts[ModContent.ProjectileType<PossessedCrownProj>()] > 0 && hit.DamageType == DamageClass.SummonMeleeSpeed)
                {
                    int MaxBlades = Player.ownedProjectileCounts[ModContent.ProjectileType<PossessedCrownProj>()];
                    for (int numBlades = 0; numBlades < MaxBlades; numBlades++)
                    {
                        Vector2 SpawnPosition = target.Center + new Vector2(0, Main.rand.Next(260, 301)).RotatedByRandom(360);

                        Projectile.NewProjectile(target.GetSource_OnHit(target), SpawnPosition, Vector2.Zero, 
                        ModContent.ProjectileType<PossessedDagger>(), damageDone / 3, hit.Knockback, Player.whoAmI, 0, target.whoAmI);
                    }
                }
            }
        }

		public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
		{
			RaveyardGuardsHostile = false;
		}

        public override void PreUpdate()
        {
            if (SpearfishChargeCooldown > 0)
            {
                SpearfishChargeCooldown--;
            }

            //set skeleton bouncer hositility to false if no raveyard is happening
            if (!Flags.RaveyardHappening)
            {
                RaveyardGuardsHostile = false;
            }

            //make player immune to the sandstorm debuff since it still applies it when you're in spooky mod biomes and theres a desert with a sandstorm happening nearby
            //because spooky mod biomes take higher priority that vanilla ones, this should not cause any issues
            if (Player.InModBiome(ModContent.GetInstance<SpookyBiome>()) || Player.InModBiome(ModContent.GetInstance<CemeteryBiome>()))
            {
                Player.buffImmune[BuffID.WindPushed] = true;
            }
        }

		public override void SetControls()
		{
			if (DisablePlayerControls)
			{
                Main.playerInventory = false;
				Player.controlLeft = false;
				Player.controlRight = false;
				Player.controlUp = false;
				Player.controlDown = false;
				Player.controlJump = false;
				Player.controlHook = false;
				Player.controlUseItem = false;
				Player.controlUseTile = false;
				Player.controlMap = false;
				Player.controlMount = false;
				Player.immuneNoBlink = true;
                Player.immuneTime = 30;

				if (AlsoDisableEscapeKey)
				{
					Player.controlInv = false;
				}
			}
		}

		public override void PostUpdateMiscEffects()
		{
			if (PotionSicknessCranberryTimer > 0)
			{
				PotionSicknessCranberryTimer--;
			}
			if (PotionSicknessCranberryTimer == 1)
			{
				int Duration = Player.pStone ? (int)(1800 * 0.75) : 1800;
				Player.AddBuff(BuffID.PotionSickness, Duration);
			}

			if (PotionSicknessLatteTimer > 0)
			{
				PotionSicknessLatteTimer--;
			}
			if (PotionSicknessLatteTimer == 1)
			{
				int Duration = Player.pStone ? (int)(3600 * 0.75) : 3600;
				Player.AddBuff(BuffID.PotionSickness, Duration);
			}

            if (Player.ownedProjectileCounts[ModContent.ProjectileType<SpearfishLanceSlashProj>()] > 0 || Player.ownedProjectileCounts[ModContent.ProjectileType<SpearfishLanceMetalSlashProj>()] > 0)
            {
                Player.noFallDmg = true;
                Player.maxFallSpeed = 80f;
            }
		}

		public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
		{
			if (drawInfo.shadow != 0f)
			{
				return;
			}

			if (!drawInfo.drawPlayer.frozen && !drawInfo.drawPlayer.dead && !drawInfo.drawPlayer.wet)
			{
                SpriteEffects spriteEffects = drawInfo.drawPlayer.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

                if (ItemGlobal.ActiveItem(drawInfo.drawPlayer).type == ModContent.ItemType<SentientLeafBlower>() && drawInfo.drawPlayer.ownedProjectileCounts[ModContent.ProjectileType<SentientLeafBlowerProj>()] > 0)
                {
                    SentientLeafBlowerBackTex ??= ModContent.Request<Texture2D>("Spooky/Content/Items/SpookyHell/Sentient/SentientLeafBlowerBack");
                    
                    int xOffset = 6;

                    DrawData PlayerBack = new DrawData(SentientLeafBlowerBackTex.Value,
					new Vector2((int)(drawInfo.drawPlayer.MountedCenter.X - Main.screenPosition.X - (xOffset * drawInfo.drawPlayer.direction)) - 4f * drawInfo.drawPlayer.direction, (int)(drawInfo.drawPlayer.MountedCenter.Y - Main.screenPosition.Y + 2f * drawInfo.drawPlayer.gravDir - 8f * drawInfo.drawPlayer.gravDir + drawInfo.drawPlayer.gfxOffY)),
					new Rectangle(0, 0, SentientLeafBlowerBackTex.Width(), SentientLeafBlowerBackTex.Height()),
                    drawInfo.colorArmorBody,
                    drawInfo.drawPlayer.bodyRotation,
                    new Vector2(SentientLeafBlowerBackTex.Width() / 2, SentientLeafBlowerBackTex.Height() / 2),
                    1f, 
                    spriteEffects, 
                    0);

                    PlayerBack.shader = 0;
                    drawInfo.DrawDataCache.Add(PlayerBack);
                }

                if (ItemGlobal.ActiveItem(drawInfo.drawPlayer).type == ModContent.ItemType<MiteVacuum>() && drawInfo.drawPlayer.ownedProjectileCounts[ModContent.ProjectileType<MiteVacuumProj>()] > 0)
                {
                    MiteVacuumBackTex ??= ModContent.Request<Texture2D>("Spooky/Content/Items/SpiderCave/MiteVacuumBack");

                    int xOffset = 10;

                    DrawData PlayerBack = new DrawData(MiteVacuumBackTex.Value,
					new Vector2((int)(drawInfo.drawPlayer.MountedCenter.X - Main.screenPosition.X - (xOffset * drawInfo.drawPlayer.direction)) - 4f * drawInfo.drawPlayer.direction, (int)(drawInfo.drawPlayer.MountedCenter.Y - Main.screenPosition.Y + 2f * drawInfo.drawPlayer.gravDir - 8f * drawInfo.drawPlayer.gravDir + drawInfo.drawPlayer.gfxOffY)),
					new Rectangle(0, 0, MiteVacuumBackTex.Width(), MiteVacuumBackTex.Height()),
                    drawInfo.colorArmorBody,
                    drawInfo.drawPlayer.bodyRotation,
                    new Vector2(MiteVacuumBackTex.Width() / 2, MiteVacuumBackTex.Height() / 2),
                    1f, 
                    spriteEffects, 
                    0);

                    PlayerBack.shader = 0;
                    drawInfo.DrawDataCache.Add(PlayerBack);
                }
			}
		}

		public override void CatchFish(FishingAttempt attempt, ref int itemDrop, ref int npcSpawn, ref AdvancedPopupRequest sonar, ref Vector2 sonarPosition)
        {
            if (!attempt.inLava && !attempt.inHoney)
            {
                //spook fishron
				if (Player.ZoneBeach && (Main.pumpkinMoon || Main.snowMoon) && attempt.playerFishingConditions.BaitItemType == ModContent.ItemType<SinisterSnailItem>())
				{
					npcSpawn = ModContent.NPCType<SpookFishron>();
					return;
				}

                //spooky forest
                if (Player.InModBiome<SpookyBiome>() || Player.InModBiome<SpookyBiomeUg>())
                {
                    //quest fishes
                    if (attempt.questFish == ModContent.ItemType<GourdFish>() && attempt.uncommon)
					{
                        itemDrop = ModContent.ItemType<GourdFish>();
                    }
                    if (attempt.questFish == ModContent.ItemType<ZomboidFish>() && attempt.uncommon)
					{
                        itemDrop = ModContent.ItemType<ZomboidFish>();
                    }
					if (attempt.questFish == ModContent.ItemType<DumboOctopoid>() && attempt.uncommon && Player.InModBiome<SpookyBiomeUg>())
					{
						itemDrop = ModContent.ItemType<DumboOctopoid>();
					}

                    //crab pet
                    if (attempt.legendary)
                    {
                        itemDrop = ModContent.ItemType<CrabClaw>();
                    }

					//crate
					if (attempt.rare && attempt.crate)
					{
						itemDrop = Main.hardMode ? ModContent.ItemType<SpookyCrate2>() : ModContent.ItemType<SpookyCrate>();
                    }
                }

                //cemetery
                if (Player.InModBiome<CemeteryBiome>())
                {
                    //quest fishes
                    if (attempt.questFish == ModContent.ItemType<SpookySpiritFish>() && attempt.uncommon)
					{
                        itemDrop = ModContent.ItemType<SpookySpiritFish>();
                    }

					//crate
					if (attempt.rare && attempt.crate && Flags.downedSpookySpirit)
					{
						itemDrop = Main.hardMode ? ModContent.ItemType<CatacombCrate2>() : ModContent.ItemType<CatacombCrate>();
					}
				}

                //catacomb
                if (Player.InModBiome<CatacombBiome>() || Player.InModBiome<CatacombBiome2>())
                {
                    //quest fishes
                    if (attempt.questFish == ModContent.ItemType<HibiscusFish>() && attempt.uncommon)
					{
                        itemDrop = ModContent.ItemType<HibiscusFish>();
                    }

                    //crate
                    if (attempt.rare && attempt.crate)
                    {
                        itemDrop = Main.hardMode ? ModContent.ItemType<CatacombCrate2>() : ModContent.ItemType<CatacombCrate>();
                    }
                }

                //spider cave
                if (Player.InModBiome<SpiderCaveBiome>())
                {
                    //quest fishes
                    if (attempt.questFish == ModContent.ItemType<SphiderFish>() && attempt.uncommon)
					{
                        itemDrop = ModContent.ItemType<SphiderFish>();
                    }

					//crate
					if (attempt.rare && attempt.crate)
					{
						itemDrop = Main.hardMode ? ModContent.ItemType<GrottoCrate2>() : ModContent.ItemType<GrottoCrate>();
                    }
                }

                //tar pits
				if (Player.InModBiome<TarPitsBiome>())
				{
					//quest fishes
					if (attempt.questFish == ModContent.ItemType<Tarpon>() && attempt.uncommon)
					{
						itemDrop = ModContent.ItemType<Tarpon>();
					}

					if (Main.rand.NextBool(3) && attempt.common)
					{
						itemDrop = ModContent.ItemType<TarGar>();
					}
				}

                //fetid farms
				if (Player.InModBiome<VegetableBiome>())
				{
					if (Main.rand.NextBool(7) && attempt.common)
					{
						itemDrop = ModContent.ItemType<CarrotFish>();
					}
				}

                //shipyard
				if (Player.InModBiome<ShipyardBiome>())
				{
					if (Main.rand.NextBool(3) && attempt.common)
					{
						itemDrop = ModContent.ItemType<GhostFish>();
					}

					//crate
					if (attempt.rare && attempt.crate)
					{
						itemDrop = Main.hardMode ? ModContent.ItemType<ShipyardCrate2>() : ModContent.ItemType<ShipyardCrate>();
                    }
				}

                //eye valley
                if (Player.InModBiome<SpookyHellBiome>())
                {
                    itemDrop = ModContent.ItemType<FleshSac>();

                    //do not allow any other npcs to be caught in the eye valley besides the enemies below
                    //this is specifically to prevent any regular blood moon fishing enemies from being caught in the blood lake if a blood moon is happening
                    npcSpawn = NPCID.None;

                    //quest fishes
                    if (attempt.questFish == ModContent.ItemType<BoogerFish>() && attempt.uncommon)
                    {
                        itemDrop = ModContent.ItemType<BoogerFish>();

                        return;
                    }
                    if (attempt.questFish == ModContent.ItemType<OrroEel>() && attempt.uncommon)
                    {
                        itemDrop = ModContent.ItemType<OrroEel>();

                        return;
                    }

                    //crate
                    if (attempt.rare && attempt.crate)
                    {
                        itemDrop = Main.hardMode ? ModContent.ItemType<SpookyHellCrate2>() : ModContent.ItemType<SpookyHellCrate>();
                    }

                    //the sludge
                    if (attempt.legendary)
                    {
                        itemDrop = ModContent.ItemType<TheSludge>();
                    }

                    //do not allow blood lake enemy catches if any of the enemies already exist in the world
                    bool BloodFishingEnemiesExist = NPC.AnyNPCs(ModContent.NPCType<ValleyFish>()) || NPC.AnyNPCs(ModContent.NPCType<ValleyMerman>()) || 
                    NPC.AnyNPCs(ModContent.NPCType<ValleySquid>()) || NPC.AnyNPCs(ModContent.NPCType<ValleyNautilus>()) || 
                    NPC.AnyNPCs(ModContent.NPCType<ValleyEelHead>()) || NPC.AnyNPCs(ModContent.NPCType<ValleyShark>());

                    if (!BloodFishingEnemiesExist && ItemGlobal.ActiveItem(Player).type == ModContent.ItemType<SentientChumCaster>())
                    {
                        //claret cephalopod
                        if (Flags.downedOrroboro && Main.rand.NextBool(25))
                        {
                            npcSpawn = ModContent.NPCType<ValleyNautilus>();

                            return;
                        }

                        //aortic eel and hemostasis beast
                        if (Main.hardMode && Main.rand.NextBool(20))
                        {
                            npcSpawn = Main.rand.NextBool() ? ModContent.NPCType<ValleyEelHead>() : ModContent.NPCType<ValleyShark>();

                            return;
                        }

                        //clot squid
                        if (Main.rand.NextBool(18))
                        {
                            npcSpawn = ModContent.NPCType<ValleySquid>();

                            return;
                        }

                        //peeper fish and flesh merfolk
                        if (Main.rand.NextBool(15))
                        {
                            npcSpawn = Main.rand.NextBool() ? ModContent.NPCType<ValleyFish>() : ModContent.NPCType<ValleyMerman>();

                            return;
                        }
                    }
                }
            }
        }

        //converts the players speed to miles per hour, uses vanillas own calculations for the stopwatch
        public static float PlayerSpeedToMPH(Player Player)
        {
            Vector2 SpeedVector = Player.velocity + Player.instantMovementAccumulatedThisFrame;

            if (Player.mount.Active && Player.mount.IsConsideredASlimeMount && Player.velocity != Vector2.Zero && !Player.SlimeDontHyperJump)
            {
                SpeedVector += Player.velocity;
            }

            Player.speedSlice[0] = SpeedVector.Length();

            int num15 = (int)(1f + SpeedVector.Length() * 6f);
            if (num15 > Player.speedSlice.Length)
            {
                num15 = Player.speedSlice.Length;
            }

            float num16 = 0f;
            for (int num17 = num15 - 1; num17 > 0; num17--)
            {
                Player.speedSlice[num17] = Player.speedSlice[num17 - 1];
            }

            Player.speedSlice[0] = SpeedVector.Length();
            for (int m = 0; m < Player.speedSlice.Length; m++)
            {
                if (m < num15)
                {
                    num16 += Player.speedSlice[m];
                }
                else
                {
                    Player.speedSlice[m] = num16 / (float)num15;
                }
            }

            num16 /= num15;
            int num18 = 42240;
            int num19 = 216000;
            float num20 = num16 * (float)num19 / (float)num18;

            return num20;
        }
	}
}