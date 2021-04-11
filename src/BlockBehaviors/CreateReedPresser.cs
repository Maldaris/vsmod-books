using System;
using Vintagestory.API.Common;
using Vintagestory.API.Client;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace VSMod.Books
{
    public class CreateReedPresser : BlockBehavior
    {

        public CreateReedPresser(Block block) : base(block) { }

        public override bool OnBlockInteractStart(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel, ref EnumHandling handling)
        {

            handling = EnumHandling.PassThrough;

            if (!(block.Code.Path.Contains("andesite") || block.Code.Path.Contains("granite")))
            {
                world.Logger.Debug(String.Format("{0} failed the check for rock type", block.Code.Path));
                return false;
            }

            if (blockSel.Face != BlockFacing.UP)
            {
                world.Logger.Debug(String.Format("{0} was not  the correct face, expected {1}", blockSel.Face, BlockFacing.UP));
                return false;
            }

            bool sneaking = byPlayer.WorldData.EntityControls.Sneak;
            ItemSlot slot = byPlayer.InventoryManager.ActiveHotbarSlot;

            if (slot.Itemstack == null) return false;

            if (sneaking && slot.Itemstack.Class == EnumItemClass.Item && slot.Itemstack.Item.Code.Path.Contains("soakedreeds"))
            {
                ItemStack stk = slot.Itemstack;
                if (stk.StackSize >= 10)
                {
                    String rockType = block.Code.Path.Contains("andesite") ? "andesite" : block.Code.Path.Contains("granite") ? "granite" : null;

                    if (rockType == null) {
                        world.Logger.Error("how'd we get here?");
                        return false;
                    }

                    Block target = world.GetBlock(new AssetLocation(String.Format("books:reedpresser-{0}", rockType)));
                    
                    if (target == null)
                    {
                        world.Logger.Error("Failed failed to locate block type for reedpresser");
                        return false;
                    }

                    world.BlockAccessor.SetBlock(target.BlockId, blockSel.Position);
                    
                    // we're doing major overhauls to this block, don't let weirdness happen by subsequent events firing on the wrong block type.
                    handling = EnumHandling.PreventSubsequent;
                    
                    Block inst = world.BlockAccessor.GetBlock(blockSel.Position);
                    if (inst.Id == 0 || inst.BlockId != target.BlockId)
                    {
                        world.Logger.Error("Failed to place ReedPresser block in world", target, blockSel.Position);
                        return false;
                    }

                    BEReedPresser be = world.BlockAccessor.GetBlockEntity(blockSel.Position) as BEReedPresser;

                    if (be == null)
                    {
                        world.Logger.Error("Converted block at position to ReedPresser, but no BEReedPresser created in tandem", target, blockSel.Position);
                        return false;
                    }

                    be.Inventory[0].Itemstack = stk.Clone();
                    slot.Itemstack = null;

                    return true;
                }
                else
                {
                    (world.Api as ICoreClientAPI)?.TriggerIngameError(
                        this,
                        "invalid-item-count",
                        Lang.Get("Not enough reeds to create a sheet of paper for pressing.")
                    );
                    return false;
                }
            }
            if (slot.Itemstack.Class == EnumItemClass.Item) {
                world.Logger.Debug(String.Format("Tried to use the wrong item type for creating a presser {0}", slot.Itemstack.Item.Code.Path));
            }
            
            return false;
        }
    }
}