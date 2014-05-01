using System;
using System.Collections;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Nrf8001Lib.Commands;
using Nrf8001Lib.Events;
using Nrf8001Lib.Extensions;

namespace Nrf8001Lib
{
    public class Nrf8001
    {
        private static readonly byte[] ReadEventLengthBuffer = new byte[2];

        private OutputPort _req, _rst;
        private InterruptPort _rdy;
        private SPI _spi;
        private Queue _eventQueue;

        /// <summary>
        /// Gets the device state of the nRF8001.
        /// </summary>
        public Nrf8001State State { get; protected set; }
        /// <summary>
        /// Gets the amount of data credits available.
        /// </summary>
        public byte DataCreditsAvailable { get; protected set; }

        public ulong OpenPipesBitmap { get; protected set; }
        public ulong ClosedPipesBitmap { get; protected set; }

        /// <summary>
        /// Creates a new nRF8001 device interface.
        /// </summary>
        /// <param name="rstPin">The application controller pin that the nRF8001's RST pin is connected to.</param>
        /// <param name="reqPin">The application controller pin that the nRF8001's REQn pin is connected to.</param>
        /// <param name="rdyPin">The application controller pin that the nRF8001's RDYn pin is connected to.</param>
        /// <param name="spiModule">The SPI module to use for communication with the nRF8001.</param>
        public Nrf8001(Cpu.Pin rstPin, Cpu.Pin reqPin, Cpu.Pin rdyPin, SPI.SPI_module spiModule)
        {
            _rst = new OutputPort(rstPin, true);
            _req = new OutputPort(reqPin, true);
            _rdy = new InterruptPort(rdyPin, false, Port.ResistorMode.PullUp, Port.InterruptMode.InterruptEdgeLow);

            _spi = new SPI(new SPI.Configuration(Cpu.Pin.GPIO_NONE, false, 0, 0, false, true, 100, spiModule));

            State = Nrf8001State.Unknown;
            _eventQueue = new Queue();

            // Reset the nRF8001
            Reset();

            _rdy.OnInterrupt += new NativeEventHandler(OnRdyInterrupt);
        }

        /// <summary>
        /// Resets the nRF8001 by toggling its RST pin.
        /// </summary>
        public void Reset()
        {
            State = Nrf8001State.Resetting;

            _rst.Write(false);
            Thread.Sleep(10);

            _eventQueue.Clear();
            _rst.Write(true);
        }

        /// <summary>
        /// Handles the first event in the nRF8001's event queue. Call this method as often as possible from the application controller.
        /// </summary>
        /// <returns>The first event in the event queue.</returns>
        public AciEvent HandleEvent()
        {
            if (_eventQueue.Count == 0)
                return null;

            var nrfEvent = (AciEvent)_eventQueue.Dequeue();

            // Device events
            switch (nrfEvent.EventType)
            {
                case Nrf8001EventType.DeviceStarted:
                    State = (Nrf8001State)nrfEvent.Data[1];
                    DataCreditsAvailable = nrfEvent.Data[3];
                    break;

                case Nrf8001EventType.Connected:
                    State = Nrf8001State.Connected;
                    break;

                case Nrf8001EventType.PipeStatus:
                    OpenPipesBitmap = nrfEvent.Data.ToUnsignedLong(1);
                    ClosedPipesBitmap = nrfEvent.Data.ToUnsignedLong(9);
                    break;

                case Nrf8001EventType.Disconnected:
                    State = Nrf8001State.Standby;
                    break;

                case Nrf8001EventType.DataCredit:
                    DataCreditsAvailable = nrfEvent.Data[1];
                    break;
            }

#if DEBUG
            Debug.Print("Event: " + nrfEvent.EventType);
#endif

            return nrfEvent;
        }

        #region ACI Commands
        /// <summary>
        /// Enables or disables Test mode on the nRF8001.
        /// </summary>
        /// <remarks>Section 24.1</remarks>
        /// <param name="testFeature">The test feature to activate, or 0xFF to disable Test mode.</param>
        public void Test(byte testFeature)
        {
            AciSend(AciOpCode.Test, testFeature);
        }

        /// <summary>
        /// Requests an EchoEvent from the nRF8001 containing the specified data.
        /// </summary>
        /// <remarks>Section 24.2</remarks>
        /// <param name="data">The data to be contained in the EchoEvent.</param>
        public void Echo(params byte[] data)
        {
            if (State != Nrf8001State.Test)
                throw new InvalidOperationException("The device is not in Test mode.");

            AciSend(AciOpCode.Echo, data);
        }

        /// <summary>
        /// Activates Sleep mode.
        /// </summary>
        /// <remarks>Section 24.4</remarks>
        public void Sleep()
        {
            if (State != Nrf8001State.Standby)
                throw new InvalidOperationException("nRF8001 is not in Standby mode.");

            AciSend(AciOpCode.Sleep);

            // Sleep does not return a CommandResponse event.
            State = Nrf8001State.Sleep;
        }

        /// <summary>
        /// Wakes up from Sleep mode.
        /// </summary>
        /// <remarks>Section 24.5</remarks>
        public void Wakeup()
        {
            if (State != Nrf8001State.Sleep)
                throw new InvalidOperationException("nRF8001 is not in Sleep mode.");

            AciSend(AciOpCode.Wakeup);
        }

        /// <summary>
        /// Uploads setup data generated by nRFgo Studio to the nRF8001.
        /// </summary>
        /// <remarks>Section 24.6</remarks>
        /// <param name="data">The setup data to upload.</param>
        public void Setup(byte[] data)
        {
            AciSend(AciOpCode.Setup, data);
        }

        /// <summary>
        /// Starts advertising and establishes a connection to a peer device.
        /// </summary>
        /// <remarks>Section 24.14</remarks>
        /// <param name="timeout">Advertisement timeout, in seconds.</param>
        /// <param name="interval">Advertisement interval, in periods of 0.625 milliseconds.</param>
        public void Connect(ushort timeout, ushort interval)
        {
            if (State != Nrf8001State.Standby)
                throw new InvalidOperationException("nRF8001 is not in Standby mode.");

            if (timeout < 0x0001 || timeout > 0x3FFF)
                throw new ArgumentOutOfRangeException("timeout", "Timeout must be between 0x0000 and 0x4000.");

            if (interval < 0x0020 || interval > 0x4000)
                throw new ArgumentOutOfRangeException("interval", "Interval must be between 0x001F and 0x4001.");

            AciSend(AciOpCode.Connect, (byte)(timeout & 0xFF), (byte)(timeout >> 8 & 0xFF), // Timeout
                                           (byte)(interval & 0xFF), (byte)(interval >> 8 & 0xFF)); // Interval
        }

        /// <summary>
        /// Starts advertising to setup a trusted relationship with a peer device.
        /// </summary>
        /// <remarks>Section 24.15</remarks>
        /// <param name="timeout">Advertisement timeout, in seconds.</param>
        /// <param name="interval">Advertisement interval, in periods of 0.625 milliseconds.</param>
        public void Bond(ushort timeout, ushort interval)
        {
            if (State != Nrf8001State.Standby)
                throw new InvalidOperationException("nRF8001 is not in Standby mode.");

            if (timeout < 0x0001 || timeout > 0x001E)
                throw new ArgumentOutOfRangeException("timeout", "Timeout must be between 0x0000 and 0x001F.");

            if (interval < 0x0020 || interval > 0x4000)
                throw new ArgumentOutOfRangeException("interval", "Interval must be between 0x001F and 0x4001.");

            AciSend(AciOpCode.Bond, 0x1E, 0x00, //(byte)(timeout & 0xFF), (byte)(timeout >> 8 & 0xFF), // Timeout
                                        (byte)(interval & 0xFF), (byte)(interval >> 8 & 0xFF)); // Interval
        }

        /// <summary>
        /// Sends data to a peer device through a transmit service pipe.
        /// </summary>
        /// <remarks>Section 25.2</remarks>
        /// <param name="servicePipeId">The ID of the service pipe to send data through.</param>
        /// <param name="data">The data to send.</param>
        public void SendData(byte servicePipeId, params byte[] data)
        {
            if (servicePipeId < 1 || servicePipeId > 62)
                throw new ArgumentOutOfRangeException("pipe", "Service pipe ID must be between 0 and 63.");

            if (data.Length < 1 || data.Length > 20)
                throw new ArgumentOutOfRangeException("data", "Data length must be between 0 and 21.");

            if (State != Nrf8001State.Connected)
                throw new InvalidOperationException("nRF8001 is not connected.");

            if (DataCreditsAvailable <= 0)
                throw new InvalidOperationException("There are no data credits available.");

            AciSend(AciOpCode.SendData, servicePipeId, data);
        }
        #endregion

        #region ACI Interface
        protected void AciSend(AciOpCode opCode, params byte[] data)
        {
            if (data.Length > 30)
                throw new ArgumentOutOfRangeException("data", "The maximum amount of data bytes is 30.");

            // Create ACI packet
            var packet = new byte[data.Length + 2];
            packet[0] = (byte)(data.Length + 1);
            packet[1] = (byte)opCode;
            Array.Copy(data, 0, packet, 2, data.Length);

            // Request transfer
            _rdy.DisableInterrupt();
            _req.Write(false);

            // Wait for RDY to go low
            while (_rdy.Read()) ;

            _spi.WriteLsb(packet);

            _req.Write(true);

            // Wait for RDY to go high
            while (!_rdy.Read()) ;
            _rdy.EnableInterrupt();
        }

        protected void AciSend(AciOpCode opCode, byte arg0, params byte[] data)
        {
            var buffer = new byte[data.Length + 1];

            buffer[0] = arg0;
            Array.Copy(data, 0, buffer, 1, data.Length);

            AciSend(opCode, buffer);
        }

        protected byte[] AciReceive()
        {
            // Create a new read buffer
            var readBuffer = new byte[2];

            // Start SPI communication
            _req.Write(false);

            _spi.WriteReadLsb(ReadEventLengthBuffer, readBuffer);

            // Check event packet length
            if (readBuffer[1] > 0)
            {
                readBuffer = new byte[readBuffer[1]];
                _spi.WriteReadLsb(new byte[readBuffer.Length], readBuffer);
            }
            
            // SPI communication done
            _req.Write(true);

            return readBuffer;
        }
        #endregion

        private void OnRdyInterrupt(uint data1, uint data2, DateTime time)
        {
            _eventQueue.Enqueue(new AciEvent(AciReceive()));
        }
    }
}
