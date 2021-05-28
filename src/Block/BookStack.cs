using Vintagestory.API.Common;

namespace VSMod.Books
{
    public class BlockBookStack : Block
    {
        public override bool OnBlockInteractStart(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel)
        {
            BEBookStack bebookstack = world.BlockAccessor.GetBlockEntity(blockSel.Position) as BEBookStack;
            if (bebookstack != null) return bebookstack.OnInteract(byPlayer, blockSel);


            return base.OnBlockInteractStart(world, byPlayer, blockSel);
        }
    }
}
