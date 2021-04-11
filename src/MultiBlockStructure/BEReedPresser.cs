using System;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Server;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.GameContent;

namespace VSMod.Books
{

    /// <summary>
    /// Presses reeds into sheets of paper.
    /// </summary>
    public class BEReedPresser : BlockEntityContainer
    {
        MultiblockStructure structure;

        double progress = 0.0;
        double processingStartTime = 0.0;
        bool structureComplete = false;

        bool processing = false;
        bool processComplete = false;

        long tickListener = -1;

        ICoreClientAPI capi;
        InventoryReedPresser inv;

        public override InventoryBase Inventory => inv;
        public override string InventoryClassName => "reedpresser";
        public BEReedPresser()
        {
            inv = new InventoryReedPresser(1, null, null);
        }

        public override void Initialize(ICoreAPI api)
        {
            base.Initialize(api);

            inv.LateInitialize(InventoryClassName + "-" + Pos, api);

            capi = api as ICoreClientAPI;

            structure = Block.Attributes["multiBlockStructure"].AsObject<MultiblockStructure>();
            structure.InitForUse(0);

            if (processing && !processComplete && api.Side == EnumAppSide.Server)
            {
                tickListener = RegisterGameTickListener(onServerTick3s, 3000);
            }
        }

        public bool Interact(IPlayer player)
        {
            bool sneaking = player.WorldData.EntityControls.Sneak;

            if (sneaking)
            {
                int ic = structure.InCompleteBlockCount(Api.World, Pos);

                if (ic == 0)
                {
                    structure.ClearHighlights(Api.World, player);
                    return true;
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

        private bool checkStructure()
        {
            int ic = structure.InCompleteBlockCount(Api.World, Pos);

            if (ic > 0) return false;

            Block bottom = Api.World.BlockAccessor.GetBlock(Pos);
            Block top = Api.World.BlockAccessor.GetBlock(Pos.UpCopy(1));

            String bottomPath = bottom.Code.Path;
            String rockType = bottomPath.Contains("andesite") ? "andesite" : bottomPath.Contains("granite") ? "granite" : null;
            if (top.BlockId == 0 || rockType == null || !top.Code.Path.Contains(rockType))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void onServerTick3s(float dt)
        {
            structureComplete = checkStructure();

            if (structureComplete && processing)
            {
                double hoursPassed = Api.World.Calendar.TotalHours - processingStartTime;
                progress = hoursPassed / 20f;
                MarkDirty();

                if (progress >= 1.0)
                {
                    processComplete = true;
                    UnregisterGameTickListener(tickListener);
                }
            }
            else if (!structureComplete || (structureComplete && processComplete))
            {
                UnregisterGameTickListener(tickListener);
            }
        }

        public override void OnBlockRemoved()
        {
            base.OnBlockRemoved();

            if (Api.Side == EnumAppSide.Client)
            {
                structure?.ClearHighlights(Api.World, (Api as ICoreClientAPI).World.Player);
            }
        }

        public override void OnBlockUnloaded()
        {
            base.OnBlockUnloaded();

            if (Api?.Side == EnumAppSide.Client)
            {
                structure?.ClearHighlights(Api.World, (Api as ICoreClientAPI).World.Player);
            }
        }

        public override void GetBlockInfo(IPlayer forPlayer, StringBuilder dsc)
        {
            if (!structureComplete && structure.InCompleteBlockCount(Api.World, Pos) > 1)
            {
                dsc.AppendLine("Area around the pressing block must be clear.");
                return;
            }

            if (!structureComplete)
            {
                dsc.AppendLine("Add the cap block to the top and finish the presser.");
                return;
            }

            if (structureComplete && processing && !processComplete)
            {
                dsc.AppendLine(String.Format("{0}% percent complete.", progress));
                return;
            }

            if (processComplete)
            {
                dsc.AppendLine("Drying complete! Break the bottom block to collect the paper.");
                return;
            }
        }

        public override void OnBlockBroken()
        {
            if (Api.World is IServerWorldAccessor)
            {
                inv.DropAll(Pos.ToVec3d().Add(0.5, 0.5, 0.5), processComplete);
            }
        }

        public override void FromTreeAttributes(ITreeAttribute tree, IWorldAccessor worldAccessForResolve)
        {
            base.FromTreeAttributes(tree, worldAccessForResolve);

            structureComplete = tree.GetBool("structureComplete");
            processComplete = tree.GetBool("processComplete");
            processing = tree.GetBool("processing");
            processingStartTime = tree.GetDouble("processingStartTime");
            progress = tree.GetDouble("progress");
        }

        public override void ToTreeAttributes(ITreeAttribute tree)
        {
            base.ToTreeAttributes(tree);

            tree.SetBool("structureComplete", structureComplete);
            tree.SetBool("processComplete", processComplete);
            tree.SetBool("processing", processing);
            tree.SetDouble("progress", progress);
            tree.SetDouble("processingStartTime", processingStartTime);

        }

        public bool HandleNeighborUpdate(IWorldAccessor world, BlockPos pos, BlockPos neibpos)
        {
            if (Api.Side == EnumAppSide.Client) return false;

            if (neibpos.Y - 1 == pos.Y)
            {
                Block it = world.BlockAccessor.GetBlock(neibpos);
                Block src = world.BlockAccessor.GetBlock(pos);

                if (it.BlockId == 0 && tickListener != -1)
                {
                    UnregisterGameTickListener(tickListener);
                    processing = false;
                    progress = 0.0;
                    return true;
                }

                String srcRockType = src.Code.Path.Contains("andesite") ? "andesite" : src.Code.Path.Contains("granite") ? "granite" : null;

                if (it.Code.Path.Contains(String.Format("rock-{0}", srcRockType)) && tickListener == -1)
                {
                    RegisterGameTickListener(onServerTick3s, 3000);
                    processingStartTime = world.Calendar.TotalHours;
                    processing = true;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }

    public class InventoryReedPresser : InventoryGeneric
    {
        public InventoryReedPresser(int size, string invId, ICoreAPI api) : base(size, invId, api) { }

        public void DropAll(Vec3d pos, bool processComplete)
        {
            if (!processComplete)
            {
                base.DropAll(pos);
            }
            else
            {
                foreach (var slot in this)
                {
                    if (slot.Itemstack == null) continue;

                    int count = slot.Itemstack.StackSize;
                    if (count == 0) continue;

                    if (slot.Itemstack.Class != EnumItemClass.Item)
                        continue;

                    if (slot.Itemstack.Item.FirstCodePart() != "soakedreeds")
                        continue;

                    if (count > 10)
                    {
                        do
                        {
                            if (count >= 10)
                            {
                                ItemStack newStack = new ItemStack(Api.World.GetItem(new AssetLocation("books:paper-blank")), 4);
                                Api.World.SpawnItemEntity(newStack, pos);
                                count -= 10;
                            }
                            else if (count >= 4)
                            {
                                ItemStack newStack = new ItemStack(Api.World.GetItem(new AssetLocation("books:paper-blank")), 2);
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
                        ItemStack newStack = new ItemStack(Api.World.GetItem(new AssetLocation("books:paper-blank")), 2);
                        Api.World.SpawnItemEntity(newStack, pos);
                    }
                    else if (count == 10)
                    {
                        ItemStack newStack = new ItemStack(Api.World.GetItem(new AssetLocation("books:paper-blank")), 4);
                        Api.World.SpawnItemEntity(newStack, pos);
                    }

                    slot.Itemstack = null;
                    slot.MarkDirty();
                }
            }
        }
    }
}