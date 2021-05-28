
using Vintagestory.API.Common;
using Vintagestory.API.Client;
using Vintagestory.API.Server;

[assembly: ModInfo( "vsmod-books",
	Description = "Books and Printing Press mod",
	Website     = "https://github.com/maldaris/vsmod-books",
	Authors     = new []{ "Maldaris" } )]

namespace VSMod.Books
{
	public class BooksMod : ModSystem
	{
		public override void Start(ICoreAPI api)
		{
            base.Start(api);

			api.RegisterBlockClass("BlockReedPresser", typeof(BlockReedPresser));
			api.RegisterBlockClass("BlockPaperStack", typeof(BlockPaperStack));
			api.RegisterBlockClass("BlockBookStack", typeof(BlockBookStack));
			api.RegisterBlockClass("BlockQuill", typeof(BlockQuill));
			api.RegisterBlockClass("BlockInkwell", typeof(BlockInkwell));

			api.RegisterBlockBehaviorClass("Inscribe", typeof(InscribeBehavior));
			api.RegisterBlockBehaviorClass("CreateReedPresser", typeof(CreateReedPresser));

			api.RegisterBlockEntityClass("BEReedPresser", typeof(BEReedPresser));
			api.RegisterBlockEntityClass("BEBookStack", typeof(BEBookStack));
			api.RegisterBlockEntityClass("BEPaperStack", typeof(BEPaperStack));

			api.RegisterItemClass("ItemPaper", typeof(ItemPaper));
			api.RegisterItemClass("ItemBook", typeof(ItemBook));
			api.RegisterItemClass("ItemBookBinding", typeof(ItemBookBinding));

		}
		
		public override void StartClientSide(ICoreClientAPI api)
		{
			base.StartClientSide(api);
		}
		
		public override void StartServerSide(ICoreServerAPI api)
		{
			base.StartServerSide(api);
		}
	}
}