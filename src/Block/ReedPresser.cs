using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Client;
using Vintagestory.API.MathTools;

namespace VSMod.Books
{
    public class ReedPresser : Block
    {
        public override bool OnBlockInteractStart(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel)
        {
            BEReedPresser besc = world.BlockAccessor.GetBlockEntity(blockSel.Position) as BEReedPresser;
            if (besc != null)
            {
                bool placed = besc.Interact(byPlayer);
                (byPlayer as IClientPlayer)?.TriggerFpAnimation(EnumHandInteract.HeldItemInteract);
                return true;
            }

            return base.OnBlockInteractStart(world, byPlayer, blockSel);
        }

        public override void OnNeighbourBlockChange(IWorldAccessor world, BlockPos pos, BlockPos neibpos)
        {
            BEReedPresser besc = world.BlockAccessor.GetBlockEntity(pos) as BEReedPresser;
            if (besc != null)
            {
                besc.HandleNeighborUpdate(world, pos, neibpos);
            }
            else
            {
                base.OnNeighbourBlockChange(world, pos, neibpos);
            }
        }
    }
}