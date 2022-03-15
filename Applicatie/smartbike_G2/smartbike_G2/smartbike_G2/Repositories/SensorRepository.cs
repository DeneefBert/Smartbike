using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.Exceptions;
using System;
using System.Diagnostics;

namespace smartbike_G2.Repositories
{
    public class SensorRepository
    {
        // GUID OF SENSOR SPEED MODE: 00000000-0000-0000-0000-c7fdd50da51d

        public Guid SensorId = Guid.Parse("00000000-0000-0000-0000-c7fdd50da51d");
        private bool WaitingForData = true;

        public float RecentRevolution = 0;
        public float RecentTimeLastEvent = 0;

        public float[][] SensorData = new float[2][]; // matrix which keeps revolutions, time

        public static float Speed = 0;
        public float Radius = 0.622F; // F means convert double to float
        // 3/25 * pi * radius * rpm

        public float Alpha;
        public bool Connected = false;

        public IBluetoothLE Ble { get; set; }
        public IAdapter Adapter { get; set; }
        
        public IDevice Device { get; set; }

        public void Init()
        {
            try
            {
                Ble = CrossBluetoothLE.Current;
                Adapter = CrossBluetoothLE.Current.Adapter;
                Alpha = Convert.ToSingle(3F/25F * Math.PI * Radius);
                Debug.WriteLine("alpha: " + Alpha);

                for (int i = 0; i < SensorData.Length; i++)
                {
                    SensorData[i] = new float[] { 0, 0 };
                }
                foreach (var item in SensorData)
                {
                    Debug.WriteLine("Revs: " + item[0] + " Time: " + item[1]);
                }

                Ble.StateChanged += (s, e) =>
                {
                    if (e.NewState == BluetoothState.On) { ConnectDevice(); Connected = true; }
                    if (e.NewState == BluetoothState.Off) { Connected = false; }
                    Debug.WriteLine($"The bluetooth state changed to {e.NewState}");
                    Debug.WriteLine($"If bluetooth has been turned off, this application will not function properly");
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Notice", ex.Message.ToString(), "Error !");
            }

        }

        // ObservableCollection<IDevice> DeviceList { get; set; }

        //public async void scanDevices()
        //{
        //    try
        //    {
        //        Debug.WriteLine("DeviceData:");
        //        DeviceList.Clear();
        //        adapter.ScanMode = ScanMode.LowLatency;
        //        adapter.ScanTimeout = 5000;
        //        adapter.DeviceDiscovered += (s, a) =>
        //        {
        //            DeviceList.Add(a.Device);
        //            Debug.WriteLine("Discovered device:" + a.Device.Id);

        //        };
        //        await adapter.StartScanningForDevicesAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine("Notice", ex.Message.ToString(), "Error !");
        //    }
        //}

        public async void ConnectDevice()
        {
            try
            {
                {
                    Adapter.DeviceConnected += (s, a) =>
                    {
                        Connected = true;
                        Device = a.Device;
                        Debug.WriteLine("Connected to device:" + a.Device.Id);
                        RetrieveServices();
                    };

                    Adapter.DeviceDisconnected += (s, a) =>
                    {
                        Connected = false;
                        Debug.WriteLine("Device disconnected:" + a.Device.Id);
                    };

                    await Adapter.ConnectToKnownDeviceAsync(SensorId);
                }
            }
            catch (DeviceConnectionException e)
            {
                // ... could not connect to device
                Debug.WriteLine("Could not connect to device");
                Debug.WriteLine(e);
                Connected = false;
            }
        }

        public async void RetrieveServices()
        {
            var services = await Device.GetServicesAsync();
            IService reqService = services[5];
            Debug.WriteLine("Got services.");
            GetCharacteristics(reqService);
        }

        public async void GetCharacteristics(IService service)
        {
            var characteristics = await service.GetCharacteristicsAsync();
            ICharacteristic characteristic = characteristics[0];
            characteristic.ValueUpdated += (o, args) =>
            {
                var bytes = characteristic.Value;
                UpdateSpeedByCadence(bytes);
                //updateSpeed(bytes);
            };

            await characteristic.StartUpdatesAsync();
        }

        public void UpdateSpeedByCadence(byte[] bytes) {
            if (bytes[0] != 2)
            {
                Debug.WriteLine("You're not in the right mode");
                return;
            }
            float revolutions = (bytes[2] << 8 | bytes[1]);
            float timeLastEvent = (bytes[4] << 8 | bytes[3]);

            if (WaitingForData)
            {
                RecentRevolution = revolutions;
                RecentTimeLastEvent = timeLastEvent;
                WaitingForData = false;
            }
            ConvertCadence(revolutions, RecentRevolution, timeLastEvent, RecentTimeLastEvent);
            Debug.WriteLine("\tSpeed: " + Speed);
            UserRepository.UpdateScore(Speed, timeLastEvent, RecentTimeLastEvent);

            RecentRevolution = revolutions;
            RecentTimeLastEvent = timeLastEvent;
        }

        public void UpdateSpeed(byte[] bytes)
        {
            if (bytes[0] != 1)
            {
                Debug.WriteLine("You're not in the right mode");
                return;
            }

            float Revolutions = (bytes[4] << 24) | (bytes[3] << 16) | (bytes[2] << 8) | bytes[1];
            float TimeLastEvent = (bytes[6] << 8) | bytes[5];

            if (WaitingForData)
            {
                RecentRevolution = Revolutions;
                RecentTimeLastEvent = TimeLastEvent;
                WaitingForData = false;
            }

            //Debug.WriteLine("Values since last event:\n");
            ConvertSpeed(Revolutions, RecentRevolution, TimeLastEvent, RecentTimeLastEvent);
            Debug.WriteLine("\tSpeed: " + Speed);
            UserRepository.UpdateScore(Speed,TimeLastEvent,RecentTimeLastEvent);

            RecentRevolution = Revolutions;
            RecentTimeLastEvent = TimeLastEvent;
        }


        public void ConvertCadence(float currev, float recrev, float curtime, float rectime)
        {
            float rev = currev - recrev;
            float time = curtime - rectime;

            if (rev < 0)
            {
                rev += 65536;
                Debug.WriteLine("\tWarning: Integer overflow on 'rev',\n\trecalculated to:" + rev);
            }

            if (time < 0)
            {
                time += 65536;
                Debug.WriteLine("\tWarning: Integer overflow on 'time',\n\trecalculated to:" + time);
            }

            float CumRev = 0;
            float CumTime = 0;

            for (int i = 0; i < SensorData.Length - 1; i++)
            {
                SensorData[i] = SensorData[i + 1];
                CumRev += SensorData[i][0];
                CumTime += SensorData[i][1];
            }

            SensorData[SensorData.Length - 1] = new float[] { rev, time };
            CumRev += SensorData[SensorData.Length - 1][0];
            CumTime += SensorData[SensorData.Length - 1][1];

            if (CumTime == 0)
            {
                //Debug.WriteLine("\tNo data changes since last event.");  
                if (Speed > 2)
                {
                    Speed = 2;
                    return;
                }
                Speed = 0;
                return;
            }

            //Debug.WriteLine("\tRev: " + rev);
            //Debug.WriteLine("\tTime: " + time);

            float revMultiplier = CumTime / 1024F / 60F; // 1000 ms / cumulative time
            float RPM = CumRev / revMultiplier;
            Debug.WriteLine("\tRPM: " + RPM);
            Speed = RPM * Alpha; // alpha value for 1 RPM given the radius of generic bycicle wheel.
            //Speed = Math.Max(Speed, 0);
        }

        public void ConvertSpeed(float currev, float recrev, float curtime, float rectime)
        {
            float rev = currev - recrev;
            float time = curtime - rectime;

            if (rectime > curtime)
            {
                time = 65535 - rectime + curtime;
                Debug.WriteLine("\tWarning: Integer overflow on 'time',\n\trecalculated to:" + time);
            }

            float CumRev = 0;
            float CumTime = 0;

            for (int i = 0; i < SensorData.Length - 1; i++)
            {
                SensorData[i] = SensorData[i + 1];
                CumRev += SensorData[i][0];
                CumTime += SensorData[i][1];
            }

            SensorData[SensorData.Length - 1] = new float[] { rev, time };
            CumRev += SensorData[SensorData.Length - 1][0];
            CumTime += SensorData[SensorData.Length - 1][1];

            if (CumTime == 0)
            {
                //Debug.WriteLine("\tNo data changes since last event.");  
                if (Speed > 2)
                {
                    Speed /= 2;
                    return;
                }
                Speed = 0;
                return;
            }

            //Debug.WriteLine("\tRev: " + rev);
            //Debug.WriteLine("\tTime: " + time);

            float RevMultiplier = 1000F / CumTime; // 1000 ms / cumulative time
            float RevPerSeconds = CumRev * RevMultiplier;
            float RPM = RevPerSeconds * 60;
            Speed = RPM * Alpha; // alpha value for 1 RPM given the radius of generic bycicle wheel.
        }

    }
}