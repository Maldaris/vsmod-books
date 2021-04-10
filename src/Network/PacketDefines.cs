namespace VSMod.Books {

    static class PacketDefines {
        public const int ItemBookPacketPrefix = 1000;
        public const int ItemPaperPacketPrefix = 2000;
    }
    public enum EnumItemPaperPacket {
        Open = PacketDefines.ItemPaperPacketPrefix + 1,
        Close,
        UpdateText
    }

    public enum EnumItemBookPacket {
        OpenView = PacketDefines.ItemBookPacketPrefix + 1,
        CloseView,
        SignBookOpen,
        SignBookSave
    }
}