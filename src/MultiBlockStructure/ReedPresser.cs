using System;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.API.Config;
using Vintagestory.GameContent;

namespace VSMod.Books
{

    /// <summary>
    /// Presses reeds into sheets of paper.
    /// </summary>
    public class ReedPresser : BlockEntityContainer
    {
        MultiblockStructure structure;

        double progress;
        int tickCounter;
        bool completed;
        bool processComplete;

        ICoreClientAPI capi;
        InventoryReedPresser inv;

        public override InventoryBase Inventory => inv;
        public override string InventoryClassName => "reedpresser";
        public ReedPresser()
        {
            inv = new InventoryReedPresser(1, null, null);
        }

        public override void Initialize(ICoreAPI api)
        {
            base.Initialize(api);

            inv.LateInitialize(InventoryClassName + "-" + Pos, api);

            capi = api as ICoreClientAPI;

            if (api.Side == EnumAppSide.Server)
            {
                RegisterGameTickListener(onServerTick1s, 1000);
            }

            structure = Block.Attributes["multiBlockStructure"].AsObject<MultiblockStructure>();
            structure.InitForUse(0);

        }

        public bool Interact(IPlayer player, bool preferThis)
        {
            bool sneaking = player.WorldData.EntityControls.Sneak;

            if (sneaking)
            {
                int ic = structure.InCompleteBlockCount(Api.World, Pos);

                if (ic == 0)
                {
                    structure.ClearHighlights(Api.World, player);

                    // check to see if the item in the player's active hand is the same rock type as the presser, if so, complete it, start processing.

                    ItemSlot slot = player.InventoryManager.ActiveHotbarSlot;

                    if (slot.Itemstack.Class == EnumItemClass.Block && Array.IndexOf(new string[2] { "andesite", "granite" }, slot.Itemstack.Item.FirstCodePart()) > -1)
                    {
                        Block it = slot.Itemstack.Block;

                        if (!Block.Code.Path.Contains(it.FirstCodePart()))
                        {
                            capi?.TriggerIngameError(this, "invalid-cap", Lang.Get("Use a rock of the same type as the base to begin pressing the reeds."));
                            return false;
                        }

                        slot.Itemstack.StackSize--;

                        if (slot.Itemstack.StackSize == 0) {
                            slot.Itemstack = null;
                        }

                        slot.MarkDirty();
                    
                        Api.World.BlockAccessor.SetBlock(it.BlockId, Pos.UpCopy(1));
                        completed = true;
                        return true;
                    }
                    else
                    {
                        capi?.TriggerIngameError(this, "invalid-cap", Lang.Get("Use a rock of the same type as the base to begin pressing the reeds."));
                        return false;
                    }

                }
                else
                {
                    structure.HighlightIncompleteParts(Api.World, player, Pos);
                    capi?.TriggerIngameError(this, "incomplete", Lang.Get("Structure is not complete, make sure the immediate area around the block is clear."));
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private void onServerTick1s(float dt)
        {

        }
    }

    public class InventoryReedPresser : InventoryGeneric
    {
        public InventoryReedPresser(int size, string invId, ICoreAPI api) : base(size, invId, api) { }

        public override void DropAll(Vec3d pos)
        {
            foreach (var slot in this)
            {
                if (slot.Itemstack == null) continue;

                int count = slot.Itemstack.StackSize;
                if (count == 0) continue;

                if (slot.Itemstack.Class != EnumItemClass.Item)
                    continue;

                if (slot.Itemstack.Item.FirstCodePart() != "cattailtops")
                    continue;

                if (count > 10)
                {
                    do
                    {
                        if (count >= 10)
                        {
                            ItemStack newStack = new ItemStack(Api.World.GetItem(new AssetLocation("paper-papyrus-blank")), 4);
                            Api.World.SpawnItemEntity(newStack, pos);
                            count -= 10;
                        }
                        else if (count >= 4)
                        {
                            ItemStack newStack = new ItemStack(Api.World.GetItem(new AssetLocation("paper-papyrus-blank")), 2);
                            Api.World.SpawnItemEntity(newStack, pos);
                            break;
                        }
                        else
                        {
                            break;
                        }
                    } while (count > 0);
                }
                else if (count < 10 && count > 4)
                {
                    ItemStack newStack = new ItemStack(Api.World.GetItem(new AssetLocation("paper-papyrus-blank")), 2);
                    Api.World.SpawnItemEntity(newStack, pos);
                }
                else if (count == 10)
                {
                    ItemStack newStack = new ItemStack(Api.World.GetItem(new AssetLocation("paper-papyrus-blank")), 4);
                    Api.World.SpawnItemEntity(newStack, pos);
                }

                slot.Itemstack = null;
                slot.MarkDirty();
            }
        }
    }
}