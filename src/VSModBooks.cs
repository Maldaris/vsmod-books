
using Vintagestory.API.Common;
using Vintagestory.API.Client;
using Vintagestory.API.Server;

[assembly: ModInfo( "vsmod-books",
	Description = "Books and Printing Press mod",
	Website     = "https://github.com/maldaris/vsmod-books",
	Authors     = new []{ "Maldaris" } )]

namespace VSMod.Books
{
	public class Books : ModSystem
	{
		public override void Start(ICoreAPI api)
		{
            base.Start(api);

			api.RegisterItemClass("soakedreeds", typeof(ItemSoakedReeds));

			api.RegisterBlockEntityClass("reedpresser", typeof(ReedPresser));

		}
		
		public override void StartClientSide(ICoreClientAPI api)
		{
			
		}
		
		public override void StartServerSide(ICoreServerAPI api)
		{
			
		}
	}
}