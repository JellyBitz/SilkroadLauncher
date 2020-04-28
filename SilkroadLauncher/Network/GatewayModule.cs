using SilkroadSecurityAPI;
namespace SilkroadLauncher.Network
{
    public static class GatewayModule
    {
        public static class Opcode
        {
            public const ushort
            CLIENT_PATCH_REQUEST = 0x6100,
            CLIENT_SHARD_LIST_REQUEST = 0x6101,
            CLIENT_LOGIN_REQUEST = 0x6102,
            CLIENT_WEB_NOTICE_REQUEST = 0x6104,
            CLIENT_CAPTCHA_SOLVED_REQUEST = 0x6323,

            SERVER_PATCH_RESPONSE = 0xA100,
            SERVER_SHARD_LIST_RESPONSE = 0xA101,
            SERVER_WEB_NOTICE_RESPONSE = 0xA104,
            SERVER_LOGIN_RESPONSE = 0xA102,
            SERVER_CAPTCHA_DATA = 0x2322,
            SERVER_CAPTCHA_SOLVED_RESPONSE = 0xA323,

            GLOBAL_HANDSHAKE = 0x5000,
            GLOBAL_HANDSHAKE_OK = 0x9000,
            GLOBAL_IDENTIFICATION = 0x2001,
            GLOBAL_PING = 0x2002;
        }
        public static void Server_GlobalIdentification(Packet p, Security s)
        {
            string service = p.ReadAscii();
            if (service == "GatewayServer")
            {
                // Send authentication
                Packet packet = new Packet(Opcode.CLIENT_PATCH_REQUEST, true);
                packet.WriteByte(Globals.LauncherViewModel.Locale);
                packet.WriteAscii("SR_Client"); // Module Name
                packet.WriteUInt(Globals.LauncherViewModel.Version);
                s.Send(packet);
            }
        }
        public static void Server_PatchResponse(Packet p, Security s)
        {
            switch (p.ReadByte())
            {
                case 1:
                    // Packet packet = new Packet(Opcode.CLIENT_SHARD_LIST_REQUEST, true);
                    // s.Send(packet);
                    Globals.LauncherViewModel.CanStartGame = true;

                    Packet packet = new Packet(Opcode.CLIENT_WEB_NOTICE_REQUEST);
                    packet.WriteByte(Globals.LauncherViewModel.Locale);
                    s.Send(packet);


                    break;
                case 2:
                    byte errorCode = p.ReadByte();
                    if (errorCode == 2)
                    {
                        Globals.LauncherViewModel.IsUpdating = true;

                        string DownloadServerIP = p.ReadAscii();
                        ushort DownloadServerPort = p.ReadUShort();
                        uint DownloadServerCurVersion = p.ReadUInt();

                        System.Diagnostics.Debug.WriteLine("Version outdate. New version available (v" + DownloadServerCurVersion + ")");

                        while (p.ReadByte() == 1)
                        {
                            uint fileId = p.ReadUInt();
                            string fileName = p.ReadAscii();
                            string filePath = p.ReadAscii();
                            uint fileLength = p.ReadUInt();
                            byte doPack = p.ReadByte();
                        }

                        Globals.LauncherViewModel.IsUpdating = false;
                        Globals.LauncherViewModel.CanStartGame = true;
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Patch error: [" + errorCode + "]");
                    }
                    break;
            }
        }
        public static void Server_WebNoticeResponse(Packet p, Security s)
        {
            byte noticeCount = p.ReadByte();
            for (int i = 0; i < noticeCount; i++)
            {
               // 2   ushort notice.Subject.Length
               // *   string  notice.Subject
               // 2   ushort notice.Article.Length
               // *   string  notice.Article
               // 2   ushort notice.EditDate.Year
               // 2   ushort notice.EditDate.Month
               // 2   ushort notice.EditDate.Day
               // 2   ushort notice.EditDate.Hour
               // 2   ushort notice.EditDate.Minute
               // 2   ushort notice.EditDate.Second
               // 4   uint notice.EditDate.Microsecond
            }                            
        }
    }
}
