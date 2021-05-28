using System;
using Vintagestory.API.Common;
using Vintagestory.API.Client;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace VSMod.Books
{
    public class BindBookBehavior : BlockBehavior
    {

        public BindBookBehavior(Block block) : base(block) { }

        public override bool OnBlockInteractStart(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel, ref EnumHandling handling)
        {

            handling = EnumHandling.PassThrough;

            if (!block.Code.Path.Contains("paperstack"))
            {
                world.Logger.Debug(String.Format("{0} failed the check for item type", block.Code.Path));
                return false;
            }

            bool sneaking = byPlayer.WorldData.EntityControls.Sneak;
            ItemSlot slot = byPlayer.InventoryManager.ActiveHotbarSlot;

            if (slot.Itemstack == null) return false;

            if (sneaking && slot.Itemstack.Class == EnumItemClass.Item && slot.Itemstack.Item.Code.Path.Contains("bookbinding"))
            {
                ItemStack stk = slot.Itemstack;
                


                return true;
            }
            if (slot.Itemstack.Class == EnumItemClass.Item) {
                world.Logger.Debug(String.Format("Tried to use wrong item for book bindings {0}", slot.Itemstack.Item.Code.Path));
            }
            
            return false;
        }
    }
}