
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

			api.RegisterBlockBehaviorClass("CreateReedPresser", typeof(CreateReedPresser));

			api.RegisterBlockEntityClass("BEReedPresser", typeof(BEReedPresser));

		}
		
		public override void StartClientSide(ICoreClientAPI api)
		{
			
		}
		
		public override void StartServerSide(ICoreServerAPI api)
		{
			
		}
	}
}