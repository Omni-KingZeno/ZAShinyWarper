using System;
using System.Collections.Generic;
using System.Threading;
using LibUsbDotNet;
using LibUsbDotNet.Main;

namespace NHSE.Injection
{
    public class USBBot : IRAMReadWriter
    {
        private UsbDevice? SwDevice;
        private UsbEndpointReader? reader;
        private UsbEndpointWriter? writer;

        public bool Connected { get; private set; }
        public int MaximumTransferSize { get { return 468; } }

        private readonly object _sync = new();

        public bool Connect()
        {
            lock (_sync)
            {
                // Find and open the usb device.
                // SwDevice = UsbDevice.OpenUsbDevice(SwFinder);
                foreach (UsbRegistry ur in UsbDevice.AllDevices)
                {
                    if (ur.Vid == 1406 && ur.Pid == 12288)
                        SwDevice = ur.Device;
                }
                // SwDevice = UsbDevice.OpenUsbDevice(MyUsbFinder);

                // If the device is open and ready
                if (SwDevice == null)
                {
                    throw new Exception("Device Not Found.");
                }

                if (SwDevice.IsOpen)
                    SwDevice.Close();
                SwDevice.Open();

                if (SwDevice is IUsbDevice wholeUsbDevice)
                {
                    // This is a "whole" USB device. Before it can be used, 
                    // the desired configuration and interface must be selected.

                    // Select config #1
                    wholeUsbDevice.SetConfiguration(1);

                    // Claim interface #0.
                    bool resagain = wholeUsbDevice.ClaimInterface(0);
                    if (!resagain)
                    {
                        wholeUsbDevice.ReleaseInterface(0);
                        wholeUsbDevice.ClaimInterface(0);
                    }
                }
                else
                {
                    Disconnect();
                    throw new Exception("Device is using WinUSB driver. Use libusbK and create a filter");
                }

                // open read write endpoints 1.
                reader = SwDevice.OpenEndpointReader(ReadEndpointID.Ep01);
                writer = SwDevice.OpenEndpointWriter(WriteEndpointID.Ep01);

                Connected = true;
                return true;
            }
        }

        public void Disconnect()
        {
            lock (_sync)
            {
                if (SwDevice != null)
                {
                    if (SwDevice.IsOpen)
                    {
                        if (SwDevice is IUsbDevice wholeUsbDevice)
                            wholeUsbDevice.ReleaseInterface(0);
                        SwDevice.Close();
                    }
                }

                reader?.Dispose();
                writer?.Dispose();
                Connected = false;
            }
        }

        private int ReadInternal(byte[] buffer)
        {
            byte[] sizeOfReturn = new byte[4];

            // read size, no error checking as of yet, should be the required 368 bytes
            if (reader == null)
                throw new Exception("USB writer is null, you may have disconnected the device during previous function");

            reader.Read(sizeOfReturn, 5000, out _);

            // read stack
            reader.Read(buffer, 5000, out var lenVal);
            return lenVal;
        }

        private int SendInternal(byte[] buffer)
        {
            if (writer == null)
                throw new Exception("USB writer is null, you may have disconnected the device during previous function");

            uint pack = (uint)buffer.Length + 2;
            var ec = writer.Write(BitConverter.GetBytes(pack), 2000, out _);
            if (ec != ErrorCode.None)
            {
                Disconnect();
                throw new Exception(UsbDevice.LastErrorString);
            }
            ec = writer.Write(buffer, 2000, out var l);
            if (ec != ErrorCode.None)
            {
                Disconnect();
                throw new Exception(UsbDevice.LastErrorString);
            }
            return l;
        }

        public int Read(byte[] buffer)
        {
            lock (_sync)
            {
                return ReadInternal(buffer);
            }
        }

        public byte[] ReadBytes(ulong offset, int length, RWMethod method = RWMethod.Heap)
        {
            if (length > MaximumTransferSize)
                return ReadBytesLarge(offset, length, method);
            lock (_sync)
            {
                var cmd = SwitchCommandMethodHelper.GetPeekCommand(offset, length, method, true);
                SendInternal(cmd);

                // give it time to push data back
                Thread.Sleep((length / 256));

                var buffer = new byte[length];
                var _ = ReadInternal(buffer);
                // return Decoder.ConvertHexByteStringToBytes(buffer);
                return buffer;
            }
        }

        public void WriteBytes(byte[] data, ulong offset, RWMethod method = RWMethod.Heap)
        {
            if (data.Length > MaximumTransferSize)
                WriteBytesLarge(data, offset, method);
            lock (_sync)
            {
                SendInternal(SwitchCommandMethodHelper.GetPokeCommand(offset, data, method, true));

                // give it time to push data back
                Thread.Sleep((data.Length / 256));
            }
        }

        public void SendBytes(byte[] encodeData)
        {
            lock (_sync)
            {
                SendInternal(encodeData);
            }
        }

        public byte[] GetVersion()
        {
            lock (_sync)
            {
                var cmd = SwitchCommand.Version();
                SendInternal(cmd);

                // give it time to push data back
                Thread.Sleep(1);
                var buffer = new byte[9];
                var _ = ReadInternal(buffer);
                return buffer;
            }
        }

        public void Configure(string name, string value)
        {
            lock (_sync)
            {
                SendInternal(SwitchCommand.Configure(name, value));

                // give it time to push data back
                Thread.Sleep(1);
            }
        }

        private void WriteBytesLarge(byte[] data, ulong offset, RWMethod method)
        {
            int byteCount = data.Length;
            for (int i = 0; i < byteCount; i += MaximumTransferSize)
                WriteBytes(SubArray(data, i, MaximumTransferSize), offset + (uint)i, method);
        }

        private byte[] ReadBytesLarge(ulong offset, int length, RWMethod method)
        {
            List<byte> read = new();
            for (int i = 0; i < length; i += MaximumTransferSize)
                read.AddRange(ReadBytes(offset + (uint)i, Math.Min(MaximumTransferSize, length - i), method));
            return read.ToArray();
        }

        private static T[] SubArray<T>(T[] data, int index, int length)
        {
            if (index + length > data.Length)
                length = data.Length - index;
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

        void IRAMReadWriter.SendBytes(byte[] encodeData)
        {
            lock (_sync)
            {
                SendInternal(encodeData);
            }
        }

        byte[] IRAMReadWriter.GetVersion()
        {
            throw new NotImplementedException();
        }

        byte[] IRAMReadWriter.GetBattery()
        {
            throw new NotImplementedException();
        }

        ulong IRAMReadWriter.FollowMainPointer(long[] jumps)
        {
            lock (_sync)
            {
                var cmd = SwitchCommand.FollowMainPointer(jumps);
                SendInternal(cmd);

                // give it time to push data back
                Thread.Sleep(1);
                var buffer = new byte[17];
                var _ = ReadInternal(buffer);
                return BitConverter.ToUInt64(buffer, 0);
            }
        }

        byte[] IRAMReadWriter.PeekMainPointer(long[] jumps, int length)
        {
            throw new NotImplementedException();
        }

        void IRAMReadWriter.FreezeBytes(byte[] data, uint offset)
        {
            throw new NotImplementedException();
        }

        void IRAMReadWriter.UnFreezeBytes(uint offset)
        {
            throw new NotImplementedException();
        }

        byte IRAMReadWriter.GetFreezeCount()
        {
            throw new NotImplementedException();
        }

        void IRAMReadWriter.UnfreezeAll()
        {
            throw new NotImplementedException();
        }

        void IRAMReadWriter.FreezePause()
        {
            throw new NotImplementedException();
        }

        void IRAMReadWriter.FreezeUnpause()
        {
            throw new NotImplementedException();
        }
    }
}
