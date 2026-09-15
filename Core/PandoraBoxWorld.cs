using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Collections.Generic;

using Spooky.Content.Biomes;

namespace Spooky.Core
{
	public class PandoraBoxWorld : ModSystem
	{
		public static int Wave = 0;
		public static bool PandoraEventActive;

		public override void OnWorldLoad()
		{
			Wave = 0;
			PandoraEventActive = false;
		}

		public override void NetSend(BinaryWriter writer)
		{
			writer.Write(Wave);
			writer.WriteFlags(PandoraEventActive);
		}

		public override void NetReceive(BinaryReader reader)
		{
			Wave = reader.ReadInt32();
			reader.ReadFlags(out PandoraEventActive);
		}

		public override void PostUpdateEverything()
		{
			if (!PandoraEventActive)
			{
				Wave = 0;
			}

			if (PandoraEventActive && !AnyPlayersInBiome())
			{
				Wave = 0;
				PandoraEventActive = false;

				if (Main.netMode == NetmodeID.Server)
				{
					NetMessage.SendData(MessageID.WorldData);
				}
			}
		}

		public bool AnyPlayersInBiome()
		{
			foreach (Player player in Main.ActivePlayers)
			{
				int playerInBiomeCount = 0;

				if (!player.dead && player.InModBiome(ModContent.GetInstance<CatacombBiome2>()))
				{
					playerInBiomeCount++;
				}

				if (playerInBiomeCount >= 1)
				{
					return true;
				}
			}

			return false;
		}
	}
}