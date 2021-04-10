
using System.IO;
using Vintagestory.API.Common;
using Vintagestory.API.Client;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;

namespace VSMod.Books
{
    public class BookSigningDialog : GuiDialogGeneric
    {
        public BookSigningDialog(string title, ICoreClientAPI api) : base(title, api) {}   
    }
}