﻿using System.Diagnostics;
using Microsoft.SPOT;
using Nrf8001Lib;
using Nrf8001Lib.Commands;
using Nrf8001Lib.Events;
using SecretLabs.NETMF.Hardware.NetduinoPlus;
using Microsoft.SPOT.Hardware;
using System.Threading;
using System;

namespace BtTest
{
    public class Program
    {
        const byte TriggerStatePipeId = 1;
        const byte ReloadStatePipeId = 2;

        const byte Timeout = 15;
        const byte Interval = 32;

        private readonly byte[][] SetupData = new byte[][] {
            new byte[] {0x00,0x00,0x02,0x02,0x42,0x07,},
            new byte[] {0x10,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x02,0x00,0x02,0x01,0x01,0x00,0x00,0x06,0x00,0x01,0xc1,0x10,0xff,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,},
            new byte[] {0x10,0x1c,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x11,0x00,0x00,0x00,0x11,0x03,0x90,0x00,},
            new byte[] {0x20,0x00,0x04,0x04,0x02,0x02,0x00,0x01,0x28,0x00,0x01,0x00,0x18,0x04,0x04,0x05,0x05,0x00,0x02,0x28,0x03,0x01,0x02,0x03,0x00,0x00,0x2a,0x04,0x04,0x14,},
            new byte[] {0x20,0x1c,0x0e,0x00,0x03,0x2a,0x00,0x01,0x4c,0x61,0x73,0x65,0x72,0x54,0x61,0x67,0x20,0x47,0x75,0x6e,0x20,0x31,0x00,0x00,0x00,0x00,0x00,0x00,0x04,0x04,},
            new byte[] {0x20,0x38,0x05,0x05,0x00,0x04,0x28,0x03,0x01,0x02,0x05,0x00,0x01,0x2a,0x06,0x04,0x03,0x02,0x00,0x05,0x2a,0x01,0x01,0x20,0xff,0x04,0x04,0x05,0x05,0x00,},
            new byte[] {0x20,0x54,0x06,0x28,0x03,0x01,0x02,0x07,0x00,0x04,0x2a,0x06,0x04,0x09,0x08,0x00,0x07,0x2a,0x04,0x01,0xff,0xff,0xff,0xff,0x00,0x00,0xff,0xff,0x04,0x04,},
            new byte[] {0x20,0x70,0x02,0x02,0x00,0x08,0x28,0x00,0x01,0x01,0x18,0x04,0x04,0x02,0x02,0x00,0x09,0x28,0x00,0x01,0x10,0xff,0x04,0x04,0x05,0x05,0x00,0x0a,0x28,0x03,},
            new byte[] {0x20,0x8c,0x01,0x12,0x0b,0x00,0x11,0xff,0x16,0x04,0x02,0x01,0x00,0x0b,0xff,0x11,0x01,0x00,0x46,0x14,0x03,0x02,0x00,0x0c,0x29,0x02,0x01,0x00,0x00,0x04,},
            new byte[] {0x20,0xa8,0x04,0x05,0x05,0x00,0x0d,0x28,0x03,0x01,0x12,0x0e,0x00,0x12,0xff,0x16,0x04,0x02,0x01,0x00,0x0e,0xff,0x12,0x01,0x00,0x46,0x14,0x03,0x02,0x00,},
            new byte[] {0x20,0xc4,0x0f,0x29,0x02,0x01,0x00,0x00,0x00,},
            new byte[] {0x40,0x00,0xff,0x11,0x01,0x00,0x02,0x04,0x00,0x0b,0x00,0x0c,0xff,0x12,0x01,0x00,0x02,0x04,0x00,0x0e,0x00,0x0f,},
            new byte[] {0xf0,0x00,0x02,0x3b,0xf6,},
        };


        private Nrf8001 _nrf;
        private OutputPort _led = new OutputPort(Pins.ONBOARD_LED, false);

        private InputPort _trigger, _reload;

        public Program()
        {
            _trigger = new InputPort(Pins.GPIO_PIN_D4, true, ResistorModes.PullUp);
            _reload = new InputPort(Pins.GPIO_PIN_D5, true, ResistorModes.PullUp);

            _nrf = new Nrf8001(Pins.GPIO_PIN_D8, Pins.GPIO_PIN_D9, Pins.GPIO_PIN_D7, SPI_Devices.SPI1);
            _nrf.AciEventReceived += OnAciEventReceived;

            _nrf.Setup(SetupData);
        }


        public void Loop()
        {
            bool triggerState = _trigger.Read();
            bool reloadState = _reload.Read();

            //_nrf.AwaitConnection(Timeout, Interval);
            _nrf.AwaitBonding(Timeout, Interval);

            while (true)
            {
                _nrf.ProcessEvents();

                // Notify peer device of trigger state change
                NotifyButtonState(_trigger, TriggerStatePipeId, ref triggerState);

                // Notify peer device of reload state change
                NotifyButtonState(_reload, ReloadStatePipeId, ref reloadState);
            }
        }


        private void NotifyButtonState(InputPort button, byte servicePipeId, ref bool state)
        {
            var currentState = button.Read();

            if (currentState != state)
            {
                _nrf.SendData(servicePipeId, (byte)(currentState ? 0x00 : 0x01));

                state = currentState;
            }
        }

        private void OnAciEventReceived(AciEvent aciEvent)
        {
            if (aciEvent.EventType == AciEventType.Disconnected)
            {
                _led.Write(false);

                // Auto reconnect
                _nrf.AwaitConnection(Timeout, Interval);
            }

            if (aciEvent.EventType == AciEventType.PipeStatus && _nrf.OpenPipesBitmap > 1)
                _led.Write(true);
        }


        public static void Main()
        {
            new Program().Loop();
        }
    }
}
