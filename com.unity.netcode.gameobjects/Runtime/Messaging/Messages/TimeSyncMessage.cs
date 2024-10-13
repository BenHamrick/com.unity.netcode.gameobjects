using System;

namespace Unity.Netcode
{
    internal struct TimeSyncMessage : INetworkMessage, INetworkSerializeByMemcpy
    {
        public int Version => 0;

        public double Time;

        public void Serialize(FastBufferWriter writer, int targetVersion)
        {
            BytePacker.WriteValuePacked(writer, Time);
        }

        public bool Deserialize(FastBufferReader reader, ref NetworkContext context, int receivedMessageVersion)
        {
            var networkManager = (NetworkManager)context.SystemOwner;
            if (!networkManager.IsClient)
            {
                return false;
            }
            ByteUnpacker.ReadValuePacked(reader, out Time);
            return true;
        }

        public void Handle(ref NetworkContext context)
        {
            var networkManager = (NetworkManager)context.SystemOwner;
            networkManager.NetworkTimeSystem.Sync(Time, networkManager.NetworkConfig.NetworkTransport.GetCurrentRtt(context.SenderId) / 1000d);
        }
    }
}
