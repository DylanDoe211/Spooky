using Terraria;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using Terraria.GameContent.Generation;
using ReLogic.Utilities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Spooky.Content.Generation
{
	public class VanillaGenModifications : ModSystem
	{
		public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
		{
			//re-locate the jungle temple so it doesnt get generated over by the catacombs
			//almost all of this is copied from vanilla jungle temple generation, but its modified to clamp the X-value so it doesnt generate too close to where the catacombs is
			//sorry im too lazy to change all the decompiler variable names
			int JungleTempleIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Jungle Temple"));
			tasks[JungleTempleIndex] = new PassLegacy("Jungle Temple", (progress, config) =>
			{
				//middle of the where the cemetery/catacombs is
				int XMiddle = Catacombs.PositionX;

				//attempt to find a valid position for the jungle temple to place in, just in case it generates far away from the jungle
				bool foundValidPosition = false;
				int attempts = 0;

				//keep moving towards the center of the world until a valid position in the jungle is found
				while (!foundValidPosition && attempts++ < 100000)
				{
					while (Catacombs.NoJungleNearby(XMiddle, (int)Main.rockLayer))
					{
						XMiddle += (XMiddle > (Main.maxTilesX / 2) ? -100 : 100);
					}
					if (!Catacombs.NoJungleNearby(XMiddle, (int)Main.rockLayer))
					{
						foundValidPosition = true;
					}
				}

				int num648 = 0;
				long num649 = 0L;
				double num650 = 0.25;
				bool flag38 = false;
				while (true)
				{
					int num651 = (int)Main.rockLayer;
					int num652 = Main.maxTilesY - 500;
					if (num651 > num652 - 1)
					{
						num651 = num652 - 1;
					}
					int num653 = WorldGen.genRand.Next(num651, num652);
					int num654 = (int)(((WorldGen.genRand.NextDouble() * num650 + 0.1) * (double)(-GenVars.dungeonSide) + 0.5) * (double)Main.maxTilesX);
					if (WorldGen.remixWorldGen)
					{
						if (WorldGen.notTheBees)
						{
							num654 = ((GenVars.dungeonSide <= 0) ? WorldGen.genRand.Next((int)((double)Main.maxTilesX * 0.6), (int)((double)Main.maxTilesX * 0.8)) : WorldGen.genRand.Next((int)((double)Main.maxTilesX * 0.2), (int)((double)Main.maxTilesX * 0.4)));
						}
						else
						{
							num654 = WorldGen.genRand.Next((int)((double)Main.maxTilesX * 0.2), (int)((double)Main.maxTilesX * 0.8));
							while ((double)num654 > (double)Main.maxTilesX * 0.4 && (double)num654 < (double)Main.maxTilesX * 0.6)
							{
								num654 = WorldGen.genRand.Next((int)((double)Main.maxTilesX * 0.2), (int)((double)Main.maxTilesX * 0.8));
							}
						}
						while (Main.tile[num654, num653].HasTile || Main.tile[num654, num653].WallType > 0 || (double)num653 > Main.worldSurface - 5.0)
						{
							num653--;
						}
						num653++;
						if (Main.tile[num654, num653].HasTile && (Main.tile[num654, num653].TileType == 60 || Main.tile[num654, num653].TileType == 59))
						{
							int num655 = 10;
							bool flag39 = false;
							for (int num656 = num654 - num655; num656 <= num656 + num655; num656++)
							{
								for (int num657 = num653 - num655; num657 < num655; num657++)
								{
									if (Main.tile[num656, num657].TileType == 191 || Main.tileDungeon[Main.tile[num656, num657].TileType])
									{
										flag39 = true;
									}
								}
							}
							if (!flag39)
							{
								flag38 = true;
								num653 -= 10 + WorldGen.genRand.Next(10);
								int Start = XMiddle < (Main.maxTilesX / 2) ? XMiddle + 550 : 0;
								int End = XMiddle < (Main.maxTilesX / 2) ? Main.maxTilesX : XMiddle - 550;
								int newTempleX = (int)MathHelper.Clamp(num654, Start, End);
								WorldGen.makeTemple(newTempleX, num653);
								break;
							}
						}
					}
					else if (Main.tile[num654, num653].HasTile && Main.tile[num654, num653].TileType == 60)
					{
						flag38 = true;
						int Start = XMiddle < (Main.maxTilesX / 2) ? XMiddle + 550 : 0;
						int End = XMiddle < (Main.maxTilesX / 2) ? Main.maxTilesX : XMiddle - 550;
						int newTempleX = (int)MathHelper.Clamp(num654, Start, End);
						WorldGen.makeTemple(newTempleX, num653);
						break;
					}
					if (num649++ > 2000000)
					{
						if (num650 == 0.35)
						{
							num648++;
							if (num648 > 10)
							{
								break;
							}
						}
						num650 = Math.Min(0.35, num650 + 0.05);
						num649 = 0L;
					}
				}
				if (!flag38)
				{
					int x14 = Main.maxTilesX - GenVars.dungeonX;
					int y14 = (int)Main.rockLayer + 100;
					if (WorldGen.remixWorldGen)
					{
						x14 = ((!WorldGen.notTheBees) ? ((GenVars.dungeonSide > 0) ? ((int)((double)Main.maxTilesX * 0.4)) : ((int)((double)Main.maxTilesX * 0.6))) : ((GenVars.dungeonSide > 0) ? ((int)((double)Main.maxTilesX * 0.3)) : ((int)((double)Main.maxTilesX * 0.7))));
					}

					int Start = XMiddle < (Main.maxTilesX / 2) ? XMiddle + 550 : 0;
					int End = XMiddle < (Main.maxTilesX / 2) ? Main.maxTilesX : XMiddle - 550;
					int newTempleX = (int)MathHelper.Clamp(x14, Start, End);
					WorldGen.makeTemple(newTempleX, y14);
				}
			});

			//re-locate the shimmer to be closer to the edge of the world so it also never gets generated over by the catacombs or rotten depths when necessary
			//copy-pasted and slightly modified shimmer generation code from terraria itself
			int shimmerIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Shimmer"));
			tasks[shimmerIndex] = new PassLegacy("Shimmer", (progress, config) =>
			{
				int ShimmerYMin = (int)(Main.worldSurface + Main.rockLayer) / 2 + 100;
				int ShimmerYMax = (int)(((Main.maxTilesY - 250) * 2) + Main.rockLayer) / 3;

				bool RottenDepthsJungleSide = (GenVars.JungleX < (Main.maxTilesX / 2) && ZombieOcean.StartPositionX < (Main.maxTilesX / 2)) ||
				(GenVars.JungleX > (Main.maxTilesX / 2) && ZombieOcean.StartPositionX > (Main.maxTilesX / 2));

				//if the rotten depths is on the jungle side, lower the shimmer more
				if (RottenDepthsJungleSide)
				{
					ShimmerYMin = Main.maxTilesY < 1800 ? Main.maxTilesY - 250 : (Main.maxTilesY / 2) + 30;
				}

				if (ShimmerYMax > Main.maxTilesY - 200)
				{
					ShimmerYMax = Main.maxTilesY - 200;
				}
				if (ShimmerYMax <= ShimmerYMin)
				{
					ShimmerYMax = ShimmerYMin + 50;
				}

				int ShimmerX = GenVars.dungeonSide < 0 ? Main.maxTilesX - 100 : 100;
				int ShimmerY = WorldGen.genRand.Next(ShimmerYMin, ShimmerYMax);

				int ShimmerYAnniversaryMin = (int)Main.worldSurface + 150;
				int ShimmerYAnniversaryMax = (int)(Main.rockLayer + Main.worldSurface + 200) / 2;

				if (ShimmerYAnniversaryMax <= ShimmerYAnniversaryMin)
				{
					ShimmerYAnniversaryMax = ShimmerYAnniversaryMin + 50;
				}

				if (WorldGen.tenthAnniversaryWorldGen)
				{
					ShimmerY = WorldGen.genRand.Next(ShimmerYAnniversaryMin, ShimmerYAnniversaryMax);
				}

				while (!WorldGen.ShimmerMakeBiome(ShimmerX, ShimmerY))
				{
					//this changes the shimmer position to be closer to the edge of the world
					ShimmerX = (GenVars.dungeonSide < 0) ? (int)(Main.maxTilesX * 0.95f) : (int)(Main.maxTilesX * 0.05f);
					ShimmerY = WorldGen.genRand.Next((int)(Main.worldSurface + Main.rockLayer) / 2 + 22, ShimmerYMax);
				}

				GenVars.shimmerPosition = new Vector2D(ShimmerX, ShimmerY);

				//add the shimmer as a protected structure so nothing attempts to generate over it
				int ProtectionRectSize = 200;
				GenVars.structures.AddProtectedStructure(new Rectangle(ShimmerX - ProtectionRectSize / 2, ShimmerY - ProtectionRectSize / 2, ProtectionRectSize, ProtectionRectSize));
			});
		}
	}
}