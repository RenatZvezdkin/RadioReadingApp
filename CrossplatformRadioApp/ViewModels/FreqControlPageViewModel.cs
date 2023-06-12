using System;
using Avalonia;
using CrossplatformRadioApp.Context;
using CrossplatformRadioApp.MainDatabase;
using CrossplatformRadioApp.Models;
using RtlSdrManager;
using RtlSdrManager.Types;
using ScottPlot.Avalonia;

namespace CrossplatformRadioApp.ViewModels;

public class FreqControlPageViewModel
{
    private string _centerFreqText="0";
    public string CenterFreqText
    {
        get =>_centerFreqText; 
        set
        {
            _centerFreqText = value;
        }
    }
    private string _sampleRate="0";
    public string SampleRate
    {
        get => _sampleRate; 
        set
        {
            _sampleRate = value;
        }
    }
    private string _fileName="New Record";
    public string FileName
    {
        get => _fileName; 
        set
        {
            _fileName = value;
        }
    }
    public bool InRecording { get; set; }
    public RelayCommand StartRecordingCommand { get; }
    public RelayCommand StopRecordingCommand { get; }
    private AvaPlot iPlot { get; set; }
    private AvaPlot qPlot { get; set; }
    public double[] iData, qData;
    public RtlSdrManagedDevice Device;
    public Record CurrentRecord;
    public FreqControlPageViewModel()
    {
        
    }
    public void SamplesReceiving(object? sender, SamplesAvailableEventArgs args)
    {
        if (!InRecording)
            return;
        
        int i = 0;
        using var db = new MyDbContext();
        foreach (var iqData in Device.GetSamplesFromAsyncBuffer(args.SampleCount))
        {
            db.RecordedIQData.Add(new Recordediqdatum
            {
                DatetimeOfRecord = DateTime.Now, 
                I = iqData.I, 
                Q = iqData.Q, 
                Record = CurrentRecord
            });
            iData[i] = iqData.I;
            qData[i++] = iqData.Q;
        }
        db.SaveChanges();
        db.Dispose();
        iPlot.RefreshRequest();
        qPlot.RefreshRequest();
        //Device.ResetDeviceBuffer();
    }
}