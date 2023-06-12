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
    public int CenterFreqType { get; set; } = 2;
    public int SampleFreqType { get; set;} = 2;
    private string _centerFreqText="100";
    public string CenterFreqText
    {
        get =>_centerFreqText; 
        set
        {
            _centerFreqText = value;
            UpdateButtons();
        }
    }
    private string _sampleRateText="1";
    public string SampleRateText
    {
        get => _sampleRateText; 
        set
        {
            _sampleRateText = value;
            UpdateButtons();
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
    public AvaPlot iPlot { get; set; }
    public AvaPlot qPlot { get; set; }
    public double[] iData, qData;
    public RtlSdrManagedDevice Device;
    public Record CurrentRecord;
    private MyDbContext db;
    public uint SamplesAmount = 1024 * 8;
    public FreqControlPageViewModel()
    {
        StartRecordingCommand = new RelayCommand(o =>
        {
            InRecording = true;
            UpdateButtons();
            CurrentRecord = new Record { FileName = FileName, DateOfRecord = DateTime.Now };
            db = new MyDbContext();
            db.Add(CurrentRecord);
            Device.StartReadSamplesAsync(SamplesAmount);
        }, CheckFreqConditions);
        StopRecordingCommand = new RelayCommand(o =>
        {
            InRecording = false;
            UpdateButtons();
            Device.StopReadSamplesAsync();
            Device.ResetDeviceBuffer();
            db.Dispose();
            db = null;
        }, o => InRecording);
    }
    public void SamplesReceiving(object? sender, SamplesAvailableEventArgs args)
    {
        if (!InRecording)
            return;
        
        int i = 0;
        foreach (var iqData in Device.GetSamplesFromAsyncBuffer(args.SampleCount))
        {
            CurrentRecord.Recordediqdata.Add(
                new Recordediqdatum
            {
                DatetimeOfRecord = DateTime.Now, 
                I = iqData.I, 
                Q = iqData.Q
            });
            /*iData[i] = iqData.I;
            qData[i++] = iqData.Q;*/
        }
        db.SaveChanges();
        /*if (iPlot!=null)
        {
            iPlot.Refresh();
        }
        if (qPlot!=null)
        {
            qPlot.Refresh();
        }*/
    }

    public bool CheckFreqConditions(object o)
    {
        {
            if (InRecording || !(int.TryParse(_centerFreqText, out int CenterFreq) &&
                                 int.TryParse(_sampleRateText, out int SampRate)) || Device==null)
            {
                return false;
            }
            bool centerFreqFlag = false;
            var centerFreq = new Frequency();
            var sampFreq = new Frequency();
            switch (CenterFreqType)
            {
                case 0:
                    centerFreq.Hz = (uint) CenterFreq;
                    break;
                case 1:
                    centerFreq.KHz = (uint) CenterFreq;
                    break;
                case 2:
                    centerFreq.MHz = (uint) CenterFreq;
                    break;
                case 3:
                    centerFreq.GHz = (uint) CenterFreq;
                    break;
            }
            switch (SampleFreqType)
            {
                case 0:
                    sampFreq.Hz = (uint) SampRate;
                    break;
                case 1:
                    sampFreq.KHz = (uint) SampRate;
                    break;
                case 2:
                    sampFreq.MHz = (uint) SampRate;
                    break;
                case 3:
                    sampFreq.GHz = (uint) SampRate;
                    break;
            }
            switch (Device.TunerType)
            {
                case TunerTypes.E4000:
                    if (centerFreq.MHz is < 52.0 or >= 1100.0 && centerFreq.MHz is <= 1250.0 or > 2200.0)
                    {
                        centerFreqFlag = true;
                        break;
                    }

                    break;
                case TunerTypes.FC0012:
                    if (centerFreq.MHz is < 22.0 or > 948.6)
                    {
                        centerFreqFlag = true;
                        break;
                    }

                    break;
                case TunerTypes.FC0013:
                    if (centerFreq.MHz is < 22.0 or > 1100.0)
                    {
                        centerFreqFlag = true;
                        break;
                    }

                    break;
                case TunerTypes.FC2580:
                    if (centerFreq.MHz is < 146.0 or > 308.0 && centerFreq.MHz is < 438.0 or > 924.0)
                    {
                        centerFreqFlag = true;
                        break;
                    }

                    break;
                case TunerTypes.R820T:
                case TunerTypes.R828D:
                    if (centerFreq.MHz is < 24.0 or > 1766.0)
                    {
                        centerFreqFlag = true;
                        break;
                    }

                    break;
                default:
                    centerFreqFlag = true;
                    break;
            }
            return !centerFreqFlag || !(sampFreq.Hz is < 225001U or > 300000U && sampFreq.Hz is < 900001U or > 3200000U);
        }
    }
    public void UpdateButtons()
    {
        StartRecordingCommand.RaiseCanExecuteChanged();
        StopRecordingCommand.RaiseCanExecuteChanged();
    }
}