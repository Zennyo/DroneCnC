using System;

using Windows.System;
using Windows.Devices.Bluetooth;
using Windows.Devices.Scanners;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Enumeration;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using System.Collections.Generic;


/*
 * Service: 00008650-0000-1000-8000-00805f9b34fb
 * --- Characteristic: 00008651-0000-1000-8000-00805f9b34fb <---
 * --- Characteristic: 00008655-0000-1000-8000-00805f9b34fb
 * --- Characteristic: 0000865f-0000-1000-8000-00805f9b34fb
 */

namespace DroneCnC
{
    class Program
    {
        BluetoothLEDevice mSteering;
        const string mSteeringAdd = "C686A104C90D";
        const string mService = "00008650-0000-1000-8000-00805f9b34fb";
        const string mCharacteristic = "00008651-0000-1000-8000-00805f9b34fb";
        GattCharacteristic mRegisteredCharacteristic;
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
            if (mSteering == null)
            {
                Console.WriteLine("failed to connect");
            }
            else
            {
                var ServResult = await mSteering.GetGattServicesAsync(BluetoothCacheMode.Uncached);
                Console.WriteLine("status: " + ServResult.Status);
                if (ServResult.Status == GattCommunicationStatus.Success)
                {
                    IReadOnlyList<GattCharacteristic> characteristics = null;

                    var services = ServResult.Services;
                    foreach (var service in services)
                    {
                        var CharResult = await service.GetCharacteristicsAsync(BluetoothCacheMode.Uncached);
                        Console.WriteLine("Service: " + service.Uuid.ToString());
                        if (CharResult.Status == GattCommunicationStatus.Success)
                        {
                            characteristics = CharResult.Characteristics;
                            foreach(GattCharacteristic chara in characteristics)
                            {
                                Console.WriteLine(" --- Characteristic: " + chara.Uuid);
                                if(chara.Uuid.ToString().Equals(mCharacteristic))
                                {
                                    Console.WriteLine(" --- --- chara found!trying to subscribe to " + chara.Uuid.ToString());
                                    mRegisteredCharacteristic = chara;
                                    mRegisteredCharacteristic.ValueChanged += Characteristic_ValueChanged;
                                }
                            }
                        }
                    }
                }
            }
        }
        private void Characteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            Console.WriteLine("CHANGED!");
        }
    }
}
