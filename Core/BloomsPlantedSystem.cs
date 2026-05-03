using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Microsoft.Xna.Framework;
using System.IO;
using System.Collections.Generic;

using Spooky.Content.Achievements;

namespace Spooky.Core
{
    public class BloomsPlantedSystem : ModSystem
    {
        public static bool CemeterySeed = false;
        public static bool DandelionSeed = false;
        public static bool DragonfruitSeed = false;
        public static bool FallSeed = false;
		public static bool FossilSeed = false;
        public static bool FungusSeed = false;
        public static bool SeaSeed = false;
        public static bool SpringSeed = false;
		public static bool SummerSeed = false;
        public static bool TomatoSeed = false;
        public static bool VegetableSeed = false;
		public static bool WinterSeed = false;

        public override void ClearWorld()
        {
            CemeterySeed = false;
            DandelionSeed = false;
            DragonfruitSeed = false;
            FallSeed = false;
            FossilSeed = false;
            FungusSeed = false;
            SeaSeed = false;
            SpringSeed = false;
            SummerSeed = false;
            TomatoSeed = false;
            VegetableSeed = false;
            WinterSeed = false;
        }

        public override void SaveWorldData(TagCompound tag)
        {
			tag[nameof(CemeterySeed)] = CemeterySeed;
			tag[nameof(DandelionSeed)] = DandelionSeed;
			tag[nameof(DragonfruitSeed)] = DragonfruitSeed;
			tag[nameof(FallSeed)] = FallSeed;
			tag[nameof(FossilSeed)] = FossilSeed;
			tag[nameof(FungusSeed)] = FungusSeed;
			tag[nameof(SeaSeed)] = SeaSeed;
			tag[nameof(SpringSeed)] = SpringSeed;
			tag[nameof(SummerSeed)] = SummerSeed;
			tag[nameof(TomatoSeed)] = TomatoSeed;
			tag[nameof(VegetableSeed)] = VegetableSeed;
			tag[nameof(WinterSeed)] = WinterSeed;
        }

        public override void LoadWorldData(TagCompound tag) 
        {
			CemeterySeed = tag.GetBool(nameof(CemeterySeed));
			DandelionSeed = tag.GetBool(nameof(DandelionSeed));
			DragonfruitSeed = tag.GetBool(nameof(DragonfruitSeed));
			FallSeed = tag.GetBool(nameof(FallSeed));
			FossilSeed = tag.GetBool(nameof(FossilSeed));
			FungusSeed = tag.GetBool(nameof(FungusSeed));
			SeaSeed = tag.GetBool(nameof(SeaSeed));
			SpringSeed = tag.GetBool(nameof(SpringSeed));
			SummerSeed = tag.GetBool(nameof(SummerSeed));
			TomatoSeed = tag.GetBool(nameof(TomatoSeed));
			VegetableSeed = tag.GetBool(nameof(VegetableSeed));
			WinterSeed = tag.GetBool(nameof(WinterSeed));
        }
        
        public override void NetSend(BinaryWriter writer)
        {
            writer.WriteFlags(CemeterySeed, DandelionSeed, DragonfruitSeed, FallSeed, FossilSeed, FungusSeed, SeaSeed, SpringSeed);
            writer.WriteFlags(SummerSeed, TomatoSeed, VegetableSeed, WinterSeed);
        }

        public override void NetReceive(BinaryReader reader)
        {
            reader.ReadFlags(out CemeterySeed, out DandelionSeed, out DragonfruitSeed, out FallSeed, out FossilSeed, out FungusSeed, out SeaSeed, out SpringSeed);
			reader.ReadFlags(out SummerSeed, out TomatoSeed, out VegetableSeed, out WinterSeed);
        }

        public override void PostUpdateEverything()
        {
            if (CemeterySeed && DandelionSeed && DragonfruitSeed && FallSeed && FossilSeed && FungusSeed && SeaSeed && SpringSeed && SummerSeed && TomatoSeed && VegetableSeed && WinterSeed)
            {
                ModContent.GetInstance<MiscAchievementBloomSeeds>().BloomSeedsCondition.Complete();
            }
        }
    }
}