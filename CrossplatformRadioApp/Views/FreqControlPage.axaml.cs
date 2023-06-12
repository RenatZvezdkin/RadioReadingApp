using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using CrossplatformRadioApp.ViewModels;
using NAudio.Wave;
using RtlSdrManager;
using RtlSdrManager.Types;


namespace CrossplatformRadioApp.Views;

public partial class FreqControlPage : UserControl
{
    private RtlSdrManagedDevice Device => (DataContext as FreqControlPageViewModel).Device;
    public FreqControlPage()
    {
        InitializeComponent();
        var cbmb = new ComboboxMessageBox(
            RtlSdrDeviceManager.Instance.Devices.Select(pair => new DeviceWithId(pair.Key, pair.Value)).ToList(),
            "DeviceName",
            "Выберите устройство для чтения", InitDevice
            );
        cbmb.ShowDialog(Manager.Instance.MainWindow);
    }

    private void InitDevice(object o)
    {
        var deviceInfo = o as DeviceWithId;
        RtlSdrDeviceManager.Instance.OpenManagedDevice(deviceInfo.Id, "rtlsdr");
        (DataContext as FreqControlPageViewModel).Device = RtlSdrDeviceManager.Instance["rtlsdr"];
        Device.TunerGainMode = TunerGainModes.AGC;
        Device.AGCMode = AGCModes.Enabled;
        Device.MaxAsyncBufferSize = 1024*8*2;
        Device.DropSamplesOnFullBuffer = true;
        Device.SamplesAvailable += (DataContext as FreqControlPageViewModel).SamplesReceiving;
        Device.StartReadSamplesAsync(1024*8);
    }
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}

public record DeviceWithId(uint Id, DeviceInfo Device)
{
    public string DeviceName => Device.Name;
}