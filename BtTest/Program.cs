﻿using System.Diagnostics;
using Microsoft.SPOT;
using Nrf8001Lib;
using Nrf8001Lib.Commands;
using Nrf8001Lib.Events;
using SecretLabs.NETMF.Hardware.NetduinoPlus;

namespace BtTest
{
    public class Program
    {
        private static readonly byte[][] SetupData = new byte[][] {
            new byte[] { 0x00,0x00,0x02,0x02,0x41,0xfe },
            new byte[] { 0x10,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x01,0x01,0x00,0x00,0x06,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00 },
            new byte[] { 0x10,0x1c,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x03,0x90,0x00 },
            new byte[] { 0x20,0x00,0x04,0x04,0x02,0x02,0x00,0x01,0x28,0x00,0x01,0x00,0x18,0x04,0x04,0x05,0x05,0x00,0x02,0x28,0x03,0x01,0x02,0x03,0x00,0x00,0x2a,0x04,0x04,0x14 },
            new byte[] { 0x20,0x1c,0x0e,0x00,0x03,0x2a,0x00,0x01,0x4c,0x61,0x73,0x65,0x72,0x54,0x61,0x67,0x20,0x47,0x75,0x6e,0x20,0x31,0x00,0x00,0x00,0x00,0x00,0x00,0x04,0x04 },
            new byte[] { 0x20,0x38,0x05,0x05,0x00,0x04,0x28,0x03,0x01,0x02,0x05,0x00,0x01,0x2a,0x06,0x04,0x03,0x02,0x00,0x05,0x2a,0x01,0x01,0x36,0x15,0x04,0x04,0x05,0x05,0x00 },
            new byte[] { 0x20,0x54,0x06,0x28,0x03,0x01,0x02,0x07,0x00,0x04,0x2a,0x06,0x04,0x09,0x08,0x00,0x07,0x2a,0x04,0x01,0xff,0xff,0xff,0xff,0x00,0x00,0xff,0xff,0x04,0x04 },
            new byte[] { 0x20,0x70,0x02,0x02,0x00,0x08,0x28,0x00,0x01,0x01,0x18,0x00 },
            new byte[] { 0xf0,0x00,0x02,0x5b,0x91 }
        };
        private static int SetupIndex = 0;

        public static void Main()
        {
            var nrf = new Nrf8001(Pins.GPIO_PIN_D4, Pins.GPIO_PIN_D2, Pins.GPIO_PIN_D3, SPI_Devices.SPI1);

            while (true)
            {
                var nrfEvent = nrf.HandleEvent();

                if (nrfEvent == null)
                    continue;

                if (nrf.State == Nrf8001State.Setup)
                    break;
            }

            nrf.Setup(SetupData[SetupIndex++]);

            while (true)
            {
                var nrfEvent = nrf.HandleEvent();

                if (nrfEvent == null)
                    continue;

                if (nrfEvent.EventType == Nrf8001EventType.CommandResponse && nrfEvent.Data[1] == (byte)Nrf8001OpCode.Setup)
                {
                    if (nrfEvent.Data[2] == (byte)Nrf8001AciStatusCode.TransactionContinue)
                    {
                        nrf.Setup(SetupData[SetupIndex++]);
                    }
                    else if (nrfEvent.Data[2] == (byte)Nrf8001AciStatusCode.TransactionComplete)
                    {
                        break;
                    }
                    else
                        Debugger.Break();
                }
            }

            while (true)
            {
                var nrfEvent = nrf.HandleEvent();

                if (nrfEvent == null)
                    continue;

                if (nrfEvent.EventType == Nrf8001EventType.DeviceStarted)
                {
                    nrf.Bond(60, 32);
                }
                else
                    Debug.Print("Event: " + nrfEvent.EventType);
            }
        }
    }
}
