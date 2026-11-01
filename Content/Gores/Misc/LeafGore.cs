using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Gores.Misc
{
	public class LeafGreen : ModGore
	{
		public override void SetStaticDefaults()
		{
			ChildSafety.SafeGore[Type] = true;
			GoreID.Sets.SpecialAI[Type] = 3;
			GoreID.Sets.PaintedFallingLeaf[Type] = true;
		}
	}

	public class LeafOrange : LeafGreen
	{
	}

	public class LeafRed : LeafGreen
	{
	}

	public class LeafTeal : LeafGreen
	{
	}

	public class LeafGrottoRed : LeafGreen
	{
	}

	public class LeafGrottoOrange : LeafGreen
	{
	}

	public class LeafGrottoYellow : LeafGreen
	{
	}

	public class LeafMangroveBlue : LeafGreen
	{
	}

	public class LeafMangroveOrange : LeafGreen
	{
	}

	public class LeafMangroveYellow : LeafGreen
	{
	}
}