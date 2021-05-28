using Vintagestory.API.Common;

namespace VSMod.Books
{
    public class BlockPaperStack : Block
    {
        public override bool OnBlockInteractStart(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel)
        {
            BEPaperStack bepaperstack = world.BlockAccessor.GetBlockEntity(blockSel.Position) as BEPaperStack;
            if (bepaperstack != null) return bepaperstack.OnInteract(byPlayer, blockSel);


            return base.OnBlockInteractStart(world, byPlayer, blockSel);
        }
    }
}
