using System;
using System.ComponentModel;

namespace PPTCombiner
{
    public static class Helpers
    {
        public static void Raise(this PropertyChangedEventHandler sourceEvent, object sender, string propertyName)
        {
            if (sourceEvent != null)
                sourceEvent(sender, new PropertyChangedEventArgs(propertyName));
        }

        public static void Raise(this EventHandler sourceEvent, object sender, EventArgs e)
        {
            if (sourceEvent != null)
                sourceEvent(sender, e);
        }

        public static bool ClientHasPowerpointInstalled()
        {
            Type pptType = Type.GetTypeFromProgID("Powerpoint.Application");

            return pptType != null;
        }
    }
}
