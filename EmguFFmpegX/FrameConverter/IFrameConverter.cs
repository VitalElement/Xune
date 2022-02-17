using System;
using System.Collections.Generic;

namespace EmguFFmpeg
{
    public interface IFrameConverter
    {
        IEnumerable<MediaFrame> Convert(MediaFrame frame);
    }

}
