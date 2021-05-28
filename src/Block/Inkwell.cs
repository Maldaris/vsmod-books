using Vintagestory.API.Common;
using Vintagestory.GameContent;

namespace VSMod.Books
{
    public class BlockInkwell : BlockLiquidContainerBase
    {
        public override float CapacityLitres => 1f;

        public override int ContainerSlotId => 1;
    }
}
