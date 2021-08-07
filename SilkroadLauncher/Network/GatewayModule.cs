using Pk2WriterAPI;
using SilkroadCommon;
using SilkroadCommon.Download;
using SilkroadSecurityAPI;
using System.Collections.Generic;

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
            CLIENT_SHARD_LIST_PING_REQUEST = 0x6106,
            CLIENT_CAPTCHA_SOLVED_REQUEST = 0x6323,

            SERVER_PATCH_RESPONSE = 0xA100,
            SERVER_SHARD_LIST_RESPONSE = 0xA101,
            SERVER_LOGIN_RESPONSE = 0xA102,
            SERVER_WEB_NOTICE_RESPONSE = 0xA104,
            SERVER_CAPTCHA_DATA = 0x2322,
            SERVER_CAPTCHA_SOLVED_RESPONSE = 0xA323,

            GLOBAL_HANDSHAKE = 0x5000,
            GLOBAL_HANDSHAKE_OK = 0x9000,
            GLOBAL_IDENTIFICATION = 0x2001,
            GLOBAL_PING = 0x2002;
        }
        public static void Server_GlobalIdentification(Packet p, Session s)
        {
            string service = p.ReadAscii();
            if (service == "GatewayServer")
            {
                // Send authentication
                Packet packet = new Packet(Opcode.CLIENT_PATCH_REQUEST, true);
                packet.WriteByte(LauncherViewModel.Instance.Locale);
                packet.WriteAscii("SR_Client"); // Module Name
                packet.WriteUInt(LauncherViewModel.Instance.Version);
                s.Send(packet);
            }
        }
        public static void Server_PatchResponse(Packet p, Session s)
        {
            System.Diagnostics.Debug.WriteLine("Server_PatchResponse");
            LauncherViewModel.Instance.IsCheckingUpdates = false;

            // Analyze the patch
            switch (p.ReadByte())
            {
                case 1:
                    LauncherViewModel.Instance.CanStartGame = true;
                    LauncherViewModel.Instance.UpdatingPercentage = 100;
                    break;
                case 2:
                    byte errorCode = p.ReadByte();
                    switch (errorCode)
                    {
                        case 2:
                            {
                                string DownloadServerIP = p.ReadAscii();
                                ushort DownloadServerPort = p.ReadUShort();
                                DownloadModule.DownloadVersion = p.ReadUInt();

                                System.Diagnostics.Debug.WriteLine("Version outdate. New version available (v" + DownloadModule.DownloadVersion + ")");
                                ulong bytesToDownload = 0;

                                while (p.ReadByte() == 1)
                                {
                                    FileEntry file = new FileEntry()
                                    {
                                        ID = p.ReadUInt(),
                                        Name = p.ReadAscii(),
                                        Path = p.ReadAscii(),
                                        Size = p.ReadUInt(),
                                        ImportToPk2 = p.ReadByte() == 1
                                    };

                                    DownloadModule.DownloadFiles.Add(file);

                                    bytesToDownload += file.Size;
                                }

                                // Set progress bar values
                                LauncherViewModel.Instance.UpdatingBytesMaxDownloading = bytesToDownload;
                                LauncherViewModel.Instance.UpdatingBytesDownloading = 0;

                                // Start downloader protocol
                                if (DownloadModule.DownloadFiles.Count > 0)
                                {
                                    // Try to load the GFXFileManager
                                    if (Pk2Writer.Initialize("GFXFileManager.dll"))
                                    {
                                        // Start downloading patch
                                        LauncherViewModel.Instance.IsUpdating = true;
                                        System.Diagnostics.Debug.WriteLine("Downloading updates...");

                                        Session downloaderSession = new Session();
                                        downloaderSession.AddHandler(Opcode.GLOBAL_IDENTIFICATION, new PacketHandler(Server_GlobalIdentification));
                                        downloaderSession.AddHandler(DownloadModule.Opcode.SERVER_READY, new PacketHandler(DownloadModule.Server_Ready));
                                        downloaderSession.AddHandler(DownloadModule.Opcode.SERVER_FILE_CHUNK, new PacketHandler(DownloadModule.Server_FileChunk));
                                        downloaderSession.AddHandler(DownloadModule.Opcode.SERVER_FILE_COMPLETED, new PacketHandler(DownloadModule.Server_FileCompleted));

                                        downloaderSession.Disconnect += new System.EventHandler((_Session, _Event) => {
                                            System.Diagnostics.Debug.WriteLine("Download: Session disconnected");
                                        });

                                        System.Threading.Tasks.Task.Run(() => downloaderSession.Start(DownloadServerIP,DownloadServerPort,10000));
                                    }
                                    else
                                    {
                                        LauncherViewModel.Instance.ShowMessage("GFXFileManager not found!");
                                    }
                                }
                            }
                            break;
                        case 4:
                            LauncherViewModel.Instance.ShowMessage(LauncherSettings.MSG_PATCH_UNABLE);
                            break;
                        case 5:
                            LauncherViewModel.Instance.ShowMessage(LauncherSettings.MSG_PATCH_TOO_OLD);
                            break;
                        case 1:
                            LauncherViewModel.Instance.ShowMessage(LauncherSettings.MSG_PATCH_TOO_NEW);
                            break;
                        default:
                            LauncherViewModel.Instance.ShowMessage("Patch Error: [" + errorCode + "]");
                            break;
                    }
                    break;
            }

            // Request shard list just for fun c:
            System.Diagnostics.Debug.WriteLine("CLIENT_SHARD_LIST_REQUEST");
            s.Send(new Packet(Opcode.CLIENT_SHARD_LIST_REQUEST, true));
            s.Send(new Packet(Opcode.CLIENT_SHARD_LIST_PING_REQUEST, true)); // Not even sure what is this..
            // Request notice
            System.Diagnostics.Debug.WriteLine("CLIENT_WEB_NOTICE_REQUEST");
            Packet packet = new Packet(Opcode.CLIENT_WEB_NOTICE_REQUEST);
            packet.WriteByte(LauncherViewModel.Instance.Locale);
            s.Send(packet);
        }
        public static void Server_WebNoticeResponse(Packet p, Session s)
        {
            System.Diagnostics.Debug.WriteLine("Server_WebNoticeResponse");

            byte noticeCount = p.ReadByte();
            // Reading notices
            List<WebNoticeViewModel> webNotices = new List<WebNoticeViewModel>(noticeCount);
            for (int i = 0; i < noticeCount; i++)
            {
                webNotices.Add(new WebNoticeViewModel(new WebNotice()
                {
                    Subject = p.ReadAscii(),
                    Article = p.ReadAscii(),
                    Year = p.ReadUShort(),
                    Month = p.ReadUShort(),
                    Day = p.ReadUShort(),
                    Hour = p.ReadUShort(),
                    Minute = p.ReadUShort(),
                    Second = p.ReadUShort(),
                    MicroSecond = p.ReadUInt()
                }));
            }
            // Set the GUI
            LauncherViewModel.Instance.WebNotices = webNotices;

            // Select the first notice found as default
            if (LauncherViewModel.Instance.WebNotices.Count > 0)
                LauncherViewModel.Instance.SelectedWebNotice = LauncherViewModel.Instance.WebNotices[0];
        }

        public static void Server_ShardListResponse(Packet p, Session s)
        {
            System.Diagnostics.Debug.WriteLine("Server_ShardListResponse");

            while (p.ReadByte() == 1)
            {
                byte farmID = p.ReadByte();
                string farmName = p.ReadAscii();
            }
            while (p.ReadByte() == 1)
            {
                ushort serverID = p.ReadUShort();
                string serverName = p.ReadAscii();
                ushort playerCounter = p.ReadUShort();
                ushort playerLimit = p.ReadUShort();
                bool isAvailable = p.ReadByte() == 1;
                byte farm_ID = p.ReadByte();

                System.Diagnostics.Debug.WriteLine($"#{serverID} {serverName} - {playerCounter}/{playerLimit} - "+(isAvailable?"Online":"Offline"));
            }
        }
    }
}
