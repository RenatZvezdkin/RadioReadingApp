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
        //IGraph = new ();
        //QGraph = new();
        ComboboxMessageBox.ShowDialog(
            Manager.Instance.MainWindow,
            RtlSdrDeviceManager.Instance.Devices.Select(pair => new DeviceWithId(pair.Key, pair.Value)).ToList() ,
            "DeviceName",
            "Выберите устройство для чтения", InitDevice);
    }

    private void InitDevice(object o)
    {
        var deviceInfo = o as DeviceWithId;
        RtlSdrDeviceManager.Instance.OpenManagedDevice(deviceInfo.Id, "rtlsdr");
        RealDataContext.Device = RtlSdrDeviceManager.Instance["rtlsdr"];
        Device.TunerGainMode = TunerGainModes.AGC;
        Device.AGCMode = AGCModes.Enabled;
        Device.MaxAsyncBufferSize = RealDataContext.SamplesAmount;
        Device.DropSamplesOnFullBuffer = true;
        Device.SamplesAvailable += RealDataContext.SamplesReceiving;
        
        /*IGraph.Plot.Add.Signal(RealDataContext.iData = new double[samplesAmount]);
        RealDataContext.iPlot = IGraph;
        QGraph.Plot.Add.Signal(RealDataContext.qData = new double[samplesAmount]);
        RealDataContext.qPlot = QGraph;*/
        
        Device.ResetDeviceBuffer();
        RealDataContext.UpdateButtons();
    }
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void SelectingItemsControl_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        RealDataContext.UpdateButtons();
    }

    private void TextBox_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        RealDataContext.UpdateButtons();
    }
}

public record DeviceWithId(uint Id, DeviceInfo Device)
{
    public string DeviceName => Device.Name;
}