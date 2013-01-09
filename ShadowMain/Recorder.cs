using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScapLIB;
using AForge.Video;
using AForge.Video.FFMPEG;

namespace ShadowMain
{
    class Recorder
    {
        ScapCapture Cap;
        public void Initialize(int Xpos, int Ypos)
        {
            Cap = new ScapCapture(false, 5, Global.RecordFPS, ScapVideoFormats.MPEG4, ScapImageFormats.Jpeg, Xpos, Ypos, Global.ScreenWidth, Global.ScreenHeight);
            ScapBackendConfig.ScapBackendSetup(Cap);
        }

        public void Record()
        {
            ScapCore.StartCapture();
        }

        public void StopAndDecompress()
        {
            ScapCore.StopCapture();
            ScapCore.DecompressCapture(false);
        }

        public void Encode()
        {
            ScapCore.EncodeCapture(false);
        }

        public bool IsDecompressFinished()
        {
            if (ScapCore.GetDecompressionProgress() == 1d)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsEncodeFinished()
        {
            if (ScapCore.GetEncodeProgress() == 1d)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
