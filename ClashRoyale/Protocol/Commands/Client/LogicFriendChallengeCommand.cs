using System;
using System.Threading.Tasks;
using ClashRoyale.Logic;
using ClashRoyale.Logic.Home.StreamEntry.Entries;
using ClashRoyale.Protocol.Commands.Client;
using ClashRoyale.Protocol.Messages.Server;
using ClashRoyale.Utilities.Netty;
using DotNetty.Buffers;
using SharpRaven.Data;
using SharpRaven.Data;

namespace ClashRoyale.Protocol.Commands.Client
{
    public class LogicFriendChallengeCommand : LogicCommand
    {
        public LogicFriendChallengeCommand(Device device, IByteBuffer buffer) : base(device, buffer)
        {
        }

        public string Message { get; set; }
        public int Arena { get; set; }
        public int GameMode { get; set; }

        public override void Decode()
        {
            try
            {
                Message = Reader.ReadScString();

                if (Reader.ReadableBytes < 1) return;
                Reader.ReadBoolean();

                if (Reader.ReadableBytes < 1) return;
                Reader.ReadVInt(); // ClassId

                if (Reader.ReadableBytes < 1) return;
                GameMode = Reader.ReadVInt(); // InstanceId

                if (Reader.ReadableBytes < 1) return;
                Reader.ReadVInt();

                if (Reader.ReadableBytes < 1) return;
                Reader.ReadVInt();

                if (Reader.ReadableBytes < 1) return;
                Reader.ReadVInt();

                if (Reader.ReadableBytes < 1) return;
                Reader.ReadVInt();

                if (Reader.ReadableBytes < 1) return;
                Reader.ReadVInt();

                if (Reader.ReadableBytes < 1) return;
                Arena = Reader.ReadVInt();
            }
            catch (Exception ex)
            {
                Logger.Log(ex, GetType(), ErrorLevel.Error);
                // Paquete corrupto o truncado; se ignora la decodificación.
            }
        }

        public override async void Process()
        {
            Logger.Log($"Friend Gamemode: {GameMode}", null);

            if (GameMode == 1) // 1 = friend friendly battle
            {
                var home = Device.Player.Home;
                var player = await Resources.Players.GetPlayerAsync(home.Id);

                if (player == null) return;

                var entry = new FriendChallengeStreamEntry
                {
                    Message = Message,
                    Arena = Arena + 1
                };

                entry.SetSender(Device.Player);
                player.AddEntry(entry);
            }
        }
    }
}