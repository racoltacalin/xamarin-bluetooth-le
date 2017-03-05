using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plugin.BLE.Abstractions.Contracts;

namespace Plugin.BLE.Abstractions
{
    public abstract class DeviceBase : IDevice
    {
        protected readonly IAdapter Adapter;

        protected DeviceBase(IAdapter adapter)
        {
            Adapter = adapter;
        }

        protected readonly List<IService> KnownServices = new List<IService>();

        public Guid Id { get; protected set; }
        public string Name { get; protected set; }
        public int Rssi { get; protected set; }
        public DeviceState State => GetState();
        public IList<AdvertisementRecord> AdvertisementRecords { get; protected set; }

        public abstract object NativeDevice { get; }

        public async Task<IList<IService>> GetServicesAsync()
        {
            if (!KnownServices.Any())
            {
                KnownServices.AddRange(await GetServicesNativeAsync());
            }

            return KnownServices;
        }

        public async Task<IService> GetServiceAsync(Guid id)
        {
            var services = await GetServicesAsync();
            return services.FirstOrDefault(x => x.Id == id);
        }

        public abstract Task<bool> UpdateRssiAsync();

        protected abstract DeviceState GetState();
        protected abstract Task<IEnumerable<IService>> GetServicesNativeAsync();

        public override string ToString()
        {
            return Name;
        }

        public void Dispose()
        {
            Adapter.DisconnectDeviceAsync(this);
        }

        public void ClearServices()
        {
            KnownServices.Clear();
        }

        #region IEquatable implementation

        public override bool Equals(object other)
        {
            if (other == null)
            {
                return false;
            }

            if (other.GetType() != GetType())
            {
                return false;
            }

            var otherDeviceBase = (DeviceBase)other;
            return Id == otherDeviceBase.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        #endregion
    }
}