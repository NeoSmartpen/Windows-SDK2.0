namespace Neosmartpen.Net
{
    public class PenControllerEx : PenController
    {
        public PenControllerEx( int protocol )
        {
            base.Protocol = protocol;
        }
    }
}