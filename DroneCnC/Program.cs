using System;

using Windows.System;
using Windows.Devices.Bluetooth;
using Windows.Devices.Scanners;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Enumeration;

namespace DroneCnC
{
    class Program
    {
        BluetoothLEDevice mSteering;
        const string mSteeringAdd = "C686A104C90D";
        bool mFound = false;

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!\n-very first BTProject");

            var scanner = new Program();

            Console.ReadLine();
        }

        public Program()
        {
            var w = new BluetoothLEAdvertisementWatcher();

            w.ScanningMode = BluetoothLEScanningMode.Active;
            w.SignalStrengthFilter.InRangeThresholdInDBm = -80;
            w.SignalStrengthFilter.OutOfRangeThresholdInDBm = -90;

            w.Received += OnAdvertiseentRec;


            w.Start();
        }

        private void OnAdvertiseentRec(BluetoothLEAdvertisementWatcher watcher, BluetoothLEAdvertisementReceivedEventArgs args)
        {
            //Console.WriteLine(String.Format("BTADD::0x{0:X}, BTNAME::{1}", args.BluetoothAddress, args.Advertisement.LocalName));
            string foundAdd = String.Format("{0:X}", args.BluetoothAddress);
            if((foundAdd == mSteeringAdd) && (!mFound))
            {
                mFound = true;
                Console.WriteLine("Steering found!");
                ConnectDevice(args.BluetoothAddress);
            }
        }

        private async void ConnectDevice(ulong deviceAdd)
        {
            Console.WriteLine("Connecting...");
            mSteering = await BluetoothLEDevice.FromBluetoothAddressAsync(deviceAdd);
            if(mSteering == null)
            {
                Console.WriteLine("failed to connect");
            }
            else
            {
                var result = await mSteering.DeviceInformation.Pairing.PairAsync();
                Console.WriteLine("status: " + result.Status);
            }
        }
    }
}
