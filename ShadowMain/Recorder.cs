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
        public ScapCapture Cap;
        public bool isRecording,isDecompressing,isEncoding,isFileReady = false;
        public void Initialize(int Xpos, int Ypos)
        {
            Cap = new ScapCapture(false, 2, Global.RecordFPS, ScapVideoFormats.MPEG4, ScapImageFormats.Jpeg, Xpos, Ypos, Global.ScreenWidth, Global.ScreenHeight,"CaptureTemp");
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

        public double GetDecProg()
        {
            return ScapCore.GetDecompressionProgress();
        }

        public double GetEncProg()
        {
            return ScapCore.GetEncodeProgress();
        }

        public bool IsDecompressFinished()
        {
            if (GetDecProg() == 1d)
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
            if (GetEncProg() == 1d)
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
