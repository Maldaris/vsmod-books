using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace VSMod.Books {

    public class InventoryFifoStack : InventoryGeneric {

        int topIndex;
        AssetLocation[] allowedCodes;

        public InventoryFifoStack(AssetLocation[] allowedCodes, int size, string invId, ICoreAPI api) : base(size, invId, api) { 
            if (allowedCodes.GetLength(0) > 0) {
                this.allowedCodes = allowedCodes;
            }
            topIndex = 0;
        }

        public InventoryFifoStack(ItemStack source, AssetLocation[] allowedCodes, int size, string invId, ICoreAPI api) : this(allowedCodes, size, invId, api) {
            this.slots[topIndex].Itemstack.SetFrom(source);
            this.MarkSlotDirty(topIndex);
        }
        
        public bool TryPop(IPlayer byPlayer, BlockSelection blockSel) {
            if (topIndex <= -1) return false;
            ItemSlot target = byPlayer.InventoryManager.ActiveHotbarSlot;
            if (target.CanHold(this.slots[topIndex])) {
                this.slots[topIndex].TryPutInto(Api.World, target);
                this.MarkSlotDirty(topIndex);
                if (this.slots[topIndex].Empty) {
                    topIndex--;
                }
                return true;
            } else {
                return false;
            }
        }

        public bool TryPush(IPlayer byPlayer, BlockSelection blockSel) {
            if (blockSel.Face != BlockFacing.UP) return false;

            if (topIndex <= -1) return false;
            ItemSlot source = byPlayer.InventoryManager.ActiveHotbarSlot;
            if(this.slots[topIndex].CanHold(byPlayer.InventoryManager.ActiveHotbarSlot)) {
                source.TryPutInto(Api.World, this.slots[topIndex]);
                this.MarkSlotDirty(topIndex);
                if (this.slots[topIndex].RemainingSlotSpace == 0 && topIndex + 1 < this.Count) {
                    topIndex++;
                }
                return true;
            } else {
                if (topIndex + 1 < this.Count && this.slots[topIndex].CanHold(this.slots[topIndex + 1])) {
                    topIndex++;
                    return TryPush(byPlayer, blockSel);
                } else return false;
            }
        }
    }
}