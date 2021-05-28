using System;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Util;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace VSMod.Books
{
    /// <summary>
    /// Contains a stack of paper sheet blocks
    /// </summary>
    public class BEPaperStack : BlockEntityDisplay
    {
        InventoryPaperStack inv;

        public override InventoryBase Inventory => inv;
        public override string InventoryClassName => "paperstack";

        Matrixf mat = new Matrixf();
        Random rand = new Random();

        Block block;


        public BEPaperStack()
        {
            meshes = new MeshData[8];
        }

        public override void Initialize(ICoreAPI api)
        {
            block = api.World.BlockAccessor.GetBlock(Pos);

            int size = block.Attributes["maxstacksize"].AsInt();
            inv = new InventoryPaperStack(size, "paperstack-0", api);

            base.Initialize(api);
        }

        internal bool OnInteract(IPlayer byPlayer, BlockSelection blockSel)
        {
            ItemSlot slot = byPlayer.InventoryManager.ActiveHotbarSlot;

            if (slot.Empty)
            {
                if (TryTake(byPlayer, blockSel))
                {
                    return true;
                }
                return false;
            }
            else
            {
                CollectibleObject colObj = slot.Itemstack.Collectible;

                if (!colObj.FirstCodePart().Contains("paper")) return false;

                if (TryPut(slot, blockSel))
                {
                    Api.World.PlaySoundAt(new AssetLocation("game:sounds/player/build"), byPlayer.Entity, byPlayer, true, 16);
                    updateMeshes();
                    return true;
                }
            }


            return false;
        }

        private bool TryPut(ItemSlot slot, BlockSelection blockSel)
        {
            return false;
        }

        private bool TryTake(IPlayer byPlayer, BlockSelection blockSel)
        {
            return false;
        }

        public override bool OnTesselation(ITerrainMeshPool mesher, ITesselatorAPI tessThreadTesselator)
        {
            mat.Identity();
            mat.RotateYDeg(block.Shape.rotateY);

            return base.OnTesselation(mesher, tessThreadTesselator);
        }

        protected override void updateMeshes()
        {
            mat.Identity();
            mat.RotateYDeg(block.Shape.rotateY);

            base.updateMeshes();
        }

        protected override MeshData genMesh(ItemStack stack, int index)
        {
            MeshData mesh;
            ICoreClientAPI capi = Api as ICoreClientAPI;
            nowTesselatingItem = stack.Item;
            nowTesselatingShape = capi.TesselatorManager.GetCachedShape(stack.Item.Shape.Base);
            capi.Tesselator.TesselateItem(stack.Item, out mesh, this);

            mesh.RenderPasses.Fill((short)EnumChunkRenderPass.BlendNoCull);

            Vec4f offset = mat.TransformVector(new Vec4f(
                (rand.NextDouble() >= 0.33 ? rand.NextDouble() >= 0.66 ? 0 : -0.5f : 0.5f),
                (index + 1) / 10,
                (rand.NextDouble() >= 0.33 ? rand.NextDouble() >= 0.66 ? 0 : -0.5f : 0.5f),
                0
            ));
            mesh.Translate(offset.XYZ);

            return mesh;
        }
    }

    public class InventoryPaperStack : InventoryGeneric
    {
        public InventoryPaperStack(int size, string invId, ICoreAPI api) : base(size, invId, api) { }
    }
}