using System;
using Blue.Cosacs.Shared.Extensions;
using Microsoft.PointOfService;
using STL.PL;

namespace Blue.Cosacs.Client
{
    public class StoreCardMagStripeReader
    {
        private Msr msr = null;
        //"MagTek Msr"

        public StoreCardMagStripeReader(string MsrServiceObjectName, Action<SwipeData> callback, Action<DeviceErrorEventArgs> errorcallback)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            try
            {
                var explorer = new PosExplorer();
                var devices = explorer.GetDevices(DeviceType.Msr);

                foreach (DeviceInfo device in explorer.GetDevices(DeviceType.Msr))
                    if (device.ServiceObjectName == MsrServiceObjectName)
                        msr = (Msr)explorer.CreateInstance(device);


                msr.Open();
                msr.Claim(1000);
                msr.DeviceEnabled = true;
                msr.DataEventEnabled = true;

                msr.DataEvent += (sender, args) =>
                    {
                        try
                        {
                            callback(new SwipeData { Track1 = msr.Track1Data, Track2 = msr.Track2Data, state = msr.State });
                        }
                        finally
                        {
                            //msr.ClearInput();
                            // re-enable the data event for subsequent scans
                            msr.DataEventEnabled = true;
                        }
                    };
                msr.ErrorEvent += (sender, args) =>
                    {
                        errorcallback(args);
                        args.ErrorResponse = ErrorResponse.Clear;
                        msr.DataEventEnabled = true;
                    };
            }
            catch (Exception ex)
            {
                MainForm.Current.ShowStatus("Magnetic Stripe Reader failed to initialize. Please try closing all Cash & Go and Payment screens.\n" + ex.Message);
            }
        }

        public void Close()
        {
            if (msr != null)
            {
                msr.DataEventEnabled = false;
                msr.DeviceEnabled = false;
                msr.Close();
            }
            msr = null;
        }

        public bool Ready()
        {
            return msr.State == ControlState.Idle;
        }

        public class SwipeData
        {
            public byte[] Track1 { get; set; }
            public byte[] Track2 { get; set; }
            public ControlState state;
        }


    }

    public static class DecodeMsrTrack
    {
        public static string Decode(byte[] track)
        {
            return System.Text.ASCIIEncoding.ASCII.GetString(track).StripNonNumeric();
        }
    }
}
