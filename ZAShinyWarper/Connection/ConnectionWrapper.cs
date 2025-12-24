using SysBot.Base;
using static SysBot.Base.SwitchButton;
using static SysBot.Base.SwitchCommand;

namespace ZAShinyWarper;

public class ConnectionWrapper(SwitchConnectionConfig Config)
{
    public readonly ISwitchConnectionAsync Connection = Config.Protocol switch
    {
        SwitchProtocol.USB => new SwitchUSBAsync(Config.Port),
        _ => new SwitchSocketAsync(Config),
    };

    public bool Connected => IsConnected;
    public bool IsWifi = Config.Protocol is SwitchProtocol.WiFi;
    private bool IsConnected { get; set; }
    private readonly bool CRLF = Config.Protocol is SwitchProtocol.WiFi;

    private readonly long[] jumpsPos = [0x47D21A0, 0x248, 0x00, 0x138, 0x90];
    private readonly long[] weatherPointer = [0x612AC30, 0xB0, 0x28, 0x00];
    private readonly long[] timePointer = [0x40F9500, 0x20, 0x40, 0x30];
    private readonly long[] basePointer = [0x40F9500, 0xB8, 0x00];
    private readonly long[] arrayStartPointer = [0x40F9500, 0xB8, 0x378, 0x00];
    private readonly long[] invalidStartPointer = [0x40F9500, 0xB8, 0x380, 0x00];

    private readonly SemaphoreSlim _connectionLock = new(1, 1);

    public async Task Connect(CancellationToken token)
    {
        if (Connected)
            return;

        try
        {
            Connection.Connect();
            IsConnected = true;
            await Connection.SendAsync(DetachController(CRLF), token).ConfigureAwait(false);
        }
        catch
        {
            IsConnected = false;
            throw;
        }
    }

    public async Task Disconnect(CancellationToken token)
    {
        if (!Connected)
            return;

        try
        {
            await Connection.SendAsync(DetachController(CRLF), token).ConfigureAwait(false);
            Connection.Disconnect();
            IsConnected = false;
        }
        catch
        {
            IsConnected = false;
        }
    }

    public async Task ToggleScreen(bool state, CancellationToken token)
    {
        await Connection.SendAsync(SetScreen(state ? ScreenState.On : ScreenState.Off), token);
    }

    public async Task<ulong> GetArrayStartOffset(CancellationToken token)
    {
        await _connectionLock.WaitAsync(token);
        try
        {
            return await Connection.PointerAll(arrayStartPointer, token);
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    public async Task<ulong> GetInvalidStartOffset(CancellationToken token)
    {
        await _connectionLock.WaitAsync(token);
        try
        {
            return await Connection.PointerAll(invalidStartPointer, token);
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    public async Task<ulong> GetMetaBaseOffset(CancellationToken token)
    {
        await _connectionLock.WaitAsync(token);
        try
        {
            return await Connection.PointerAll(basePointer, token);
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    public async Task<byte[]> ReadBytesAbsolute(ulong offset, int length, CancellationToken token)
    {
        await _connectionLock.WaitAsync(token);
        try
        {
            return await Connection.ReadBytesAbsoluteAsync(offset, length, token);
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    public async Task WriteBytesAbsolute(byte[] data, ulong offset, CancellationToken token)
    {
        await _connectionLock.WaitAsync(token);
        try
        {
            await Connection.WriteBytesAbsoluteAsync(data, offset, token);
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    public async Task MovePlayer(float x, float y, float z, int distance, CancellationToken token)
    {
        await _connectionLock.WaitAsync(token);
        try
        {
            var offset = await Connection.PointerAll(jumpsPos, token);
            var bytes = await Connection.ReadBytesAbsoluteAsync(offset, 12, token);

            float xn = BitConverter.ToSingle(bytes, 0);
            float zn = BitConverter.ToSingle(bytes, 4);
            float yn = BitConverter.ToSingle(bytes, 8);

            xn += x * distance;
            yn += y * distance;
            zn += z * distance;

            await Connection.WriteBytesAbsoluteAsync(BitConverter.GetBytes(xn), offset, token).ConfigureAwait(false);
            await Connection.WriteBytesAbsoluteAsync(BitConverter.GetBytes(zn), offset + 4, token).ConfigureAwait(false);
            await Connection.WriteBytesAbsoluteAsync(BitConverter.GetBytes(yn), offset + 8, token).ConfigureAwait(false);
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    public async Task<Vector3> GetPlayerPositionAsync(CancellationToken token)
    {
        await _connectionLock.WaitAsync(token);
        try
        {
            var offset = await Connection.PointerAll(jumpsPos, token);
            var bytes = await Connection.ReadBytesAbsoluteAsync(offset, 12, token);

            float xn = BitConverter.ToSingle(bytes, 0);
            float yn = BitConverter.ToSingle(bytes, 4);
            float zn = BitConverter.ToSingle(bytes, 8);

            return new Vector3 { X = xn, Y = yn, Z = zn };
        }
        catch
        {
            return new Vector3();
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    public async Task SetPlayerPosition(float x, float y, float z, CancellationToken token)
    {
        await _connectionLock.WaitAsync(token);
        try
        {
            byte[] xb = BitConverter.GetBytes(x);
            byte[] yb = BitConverter.GetBytes(y);
            byte[] zb = BitConverter.GetBytes(z);

            var offset = await Connection.PointerAll(jumpsPos, token);
            var bytes = xb.Concat(yb).Concat(zb).ToArray();

            await Connection.WriteBytesAbsoluteAsync(bytes, offset, token);
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    public async Task<byte[]> GetCurrentWeather(CancellationToken token)
    {
        await _connectionLock.WaitAsync(token);
        try
        {
            var offset = await Connection.PointerAll(weatherPointer, token).ConfigureAwait(false);
            var weather = await Connection.ReadBytesAbsoluteAsync(offset, 4, token).ConfigureAwait(false);
            return weather;
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    public async Task SetCurrentWeather(byte[] weather, CancellationToken token)
    {
        await _connectionLock.WaitAsync(token);
        try
        {
            var offset = await Connection.PointerAll(weatherPointer, token).ConfigureAwait(false);
            await Connection.WriteBytesAbsoluteAsync(weather, offset, token).ConfigureAwait(false);
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    public async Task<byte[]> GetCurrentTime(CancellationToken token)
    {
        await _connectionLock.WaitAsync(token);
        try
        {
            var offset = await Connection.PointerAll(timePointer, token).ConfigureAwait(false);
            var time = await Connection.ReadBytesAbsoluteAsync(offset, 4, token).ConfigureAwait(false);
            return time;
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    public async Task SetCurrentTime(byte[] time, CancellationToken token)
    {
        await _connectionLock.WaitAsync(token);
        try
        {
            var offset = await Connection.PointerAll(timePointer, token).ConfigureAwait(false);
            await Connection.WriteBytesAbsoluteAsync(time, offset, token).ConfigureAwait(false);
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    public async Task SetRotation(int speed, CancellationToken token)
    {
        await _connectionLock.WaitAsync(token);
        try
        {
            await Connection.SendAsync(SetStick(SwitchStick.RIGHT, (short)speed, 0, IsWifi), token).ConfigureAwait(false);
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    public async Task OpenMenu(CancellationToken token)
    {
        await _connectionLock.WaitAsync(token);
        try
        {
            await Connection.SendAsync(Click(X, IsWifi), token).ConfigureAwait(false);
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    public async Task MarkSpawn(CancellationToken token)
    {
        await _connectionLock.WaitAsync(token);
        try
        {
            await Connection.SendAsync(Click(PLUS, IsWifi), token).ConfigureAwait(false);
            await Task.Delay(450, token).ConfigureAwait(false);
            await Connection.SendAsync(Click(LSTICK, IsWifi), token).ConfigureAwait(false);
            await Task.Delay(350, token).ConfigureAwait(false);
            await Connection.SendAsync(Click(A, IsWifi), token).ConfigureAwait(false);
            await Task.Delay(350, token).ConfigureAwait(false);
            await Connection.SendAsync(Click(PLUS, IsWifi), token).ConfigureAwait(false);
            await Task.Delay(350, token).ConfigureAwait(false);
            await Connection.SendAsync(Hold(A, IsWifi), token).ConfigureAwait(false);
            await Task.Delay(350, token).ConfigureAwait(false);
            await Connection.SendAsync(Release(A, IsWifi), token).ConfigureAwait(false);
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    public async Task OnClose(CancellationToken token)
    {
        await _connectionLock.WaitAsync(token);
        try
        {
            await Connection.SendAsync(ResetStick(SwitchStick.LEFT, IsWifi), token).ConfigureAwait(false);
            await Connection.SendAsync(DetachController(IsWifi), token).ConfigureAwait(false);
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    public async Task SaveGame(CancellationToken token)
    {
        await _connectionLock.WaitAsync(token);
        try
        {
            await Connection.SendAsync(Click(X, CRLF), token).ConfigureAwait(false);
            await Task.Delay(1_000, token).ConfigureAwait(false);
            await Connection.SendAsync(Click(R, CRLF), token).ConfigureAwait(false);
            await Task.Delay(1_000, token).ConfigureAwait(false);
            await Connection.SendAsync(Click(A, CRLF), token).ConfigureAwait(false);
            await Task.Delay(5_000, token).ConfigureAwait(false);
            await Connection.SendAsync(Click(B, CRLF), token).ConfigureAwait(false);
            await Task.Delay(800, token).ConfigureAwait(false);
            await Connection.SendAsync(Click(B, CRLF), token).ConfigureAwait(false);
            await Task.Delay(800, token).ConfigureAwait(false);
        }
        finally
        {
            _connectionLock.Release();
        }
    }
}