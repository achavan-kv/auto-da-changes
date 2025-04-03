using System;
using System.Collections.Generic;
using System.Text;

namespace Blue.Cosacs.Shared
{
    public class StoreCardCardReader
    {
        public bool capture = false;
        private StringBuilder capturedText = new StringBuilder();
        private DateTime cardTime;

        public string ReadKey(char key)
        {
            if (key == (char)37)
            {
                capture = true;
                capturedText = new StringBuilder();
                cardTime = DateTime.Now;
            }

            if (capture)
            {
                capturedText.Append(key);
                if (key == (char)63)
                {
                    capture = false;
                    return capturedText.ToString();
                }

                if (cardTime.AddSeconds(3) < DateTime.Now)
                    capture = false;
            }
            return null;
        }
    }
}
