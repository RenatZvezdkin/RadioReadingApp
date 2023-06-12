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
    private RtlSdrManagedDevice Device => RealDataContext.Device;
    public FreqControlPageViewModel RealDataContext => (DataContext as FreqControlPageViewModel);
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
        RealDataContext.Device = RtlSdrDeviceManager.Instance["rtlsdr"];
        Device.TunerGainMode = TunerGainModes.AGC;
        Device.AGCMode = AGCModes.Enabled;
        uint samplesAmount = 1024 * 8;
        Device.MaxAsyncBufferSize = samplesAmount*2;
        Device.DropSamplesOnFullBuffer = true;
        Device.SamplesAvailable += RealDataContext.SamplesReceiving;
        
        IGraph.Plot.Add.Signal(RealDataContext.iData = new double[samplesAmount]);
        RealDataContext.iPlot = IGraph;
        QGraph.Plot.Add.Signal(RealDataContext.qData = new double[samplesAmount]);
        RealDataContext.qPlot = QGraph;
        
        Device.ResetDeviceBuffer();
        Device.StartReadSamplesAsync(samplesAmount);
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