using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Kinect;

namespace ShadowMain
{
    class Player
    {
        //Kinect
        KinectSensor kinect;
        Texture2D colorVideo, depthVideo, jointTexture, headTexture;
        Texture2D[] skin = new Texture2D[5];
        Skeleton[] skeletonData;
        Skeleton skeleton;
        public string status = "";
        Boolean debugging = false;

        public void Initialize(GraphicsDeviceManager graphics)
        {
            try
            {
                kinect = KinectSensor.KinectSensors[0];
                //kinect.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
                kinect.DepthStream.Enable(DepthImageFormat.Resolution320x240Fps30);
                kinect.SkeletonStream.Enable();
                kinect.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(kinect_AllFramesReady);

                kinect.Start();
                colorVideo = new Texture2D(graphics.GraphicsDevice, kinect.ColorStream.FrameWidth, kinect.ColorStream.FrameHeight);
                depthVideo = new Texture2D(graphics.GraphicsDevice, kinect.DepthStream.FrameWidth, kinect.DepthStream.FrameHeight);
                Debug.WriteLineIf(debugging, kinect.Status);
                status = "ok";
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                status = "error";
            }
        }

        public void LoadContent(ContentManager content)
        {
            jointTexture = content.Load<Texture2D>("Kinect\\joint");
            skin[0] = content.Load<Texture2D>("Kinect\\head");
            skin[1] = content.Load<Texture2D>("Kinect\\upper torso");
            skin[2] = content.Load<Texture2D>("Kinect\\pelvis");
            skin[3] = content.Load<Texture2D>("Kinect\\leftarm-upper");
            skin[4] = content.Load<Texture2D>("Kinect\\rightarm-upper");

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            DrawSkeleton(spriteBatch, new Vector2(Global.ScreenWidth,Global.ScreenHeight),jointTexture);
        }

        void kinect_AllFramesReady(object sender, AllFramesReadyEventArgs imageFrames)
        {
            //
            // Color Frame 
            //
            /*
            //Get raw image
            ColorImageFrame colorVideoFrame = imageFrames.OpenColorImageFrame();

            if (colorVideoFrame != null)
            {
                //Create array for pixel data and copy it from the image frame
                Byte[] pixelData = new Byte[colorVideoFrame.PixelDataLength];
                colorVideoFrame.CopyPixelDataTo(pixelData);

                //Convert RGBA to BGRA
                Byte[] bgraPixelData = new Byte[colorVideoFrame.PixelDataLength];
                for (int i = 0; i < pixelData.Length; i += 4)
                {
                    bgraPixelData[i] = pixelData[i + 2];
                    bgraPixelData[i + 1] = pixelData[i + 1];
                    bgraPixelData[i + 2] = pixelData[i];
                    bgraPixelData[i + 3] = (Byte)255; //The video comes with 0 alpha so it is transparent
                }

                // Create a texture and assign the realigned pixels
                colorVideo = new Texture2D(graphics.GraphicsDevice, colorVideoFrame.Width, colorVideoFrame.Height);
                colorVideo.SetData(bgraPixelData);
            }

            //
            // Depth Frame
            //
            DepthImageFrame depthVideoFrame = imageFrames.OpenDepthImageFrame();

            if (depthVideoFrame != null)
            {
                Debug.WriteLineIf(debugging, "Frame");
                //Create array for pixel data and copy it from the image frame
                short[] pixelData = new short[depthVideoFrame.PixelDataLength];
                depthVideoFrame.CopyPixelDataTo(pixelData);

                for (int i = 0; i < 10; i++)
                { Debug.WriteLineIf(debugging, pixelData[i]); }
                
                // Convert the Depth Frame
                // Create a texture and assign the realigned pixels
                //
                depthVideo = new Texture2D(graphics.GraphicsDevice, depthVideoFrame.Width, depthVideoFrame.Height);
                depthVideo.SetData(ConvertDepthFrame(pixelData, kinect.DepthStream));

            }*/

            //
            // Skeleton Frame
            //
            using (SkeletonFrame skeletonFrame = imageFrames.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    if ((skeletonData == null) || (this.skeletonData.Length != skeletonFrame.SkeletonArrayLength))
                    {
                        this.skeletonData = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    }

                    //Copy the skeleton data to our array
                    skeletonFrame.CopySkeletonDataTo(this.skeletonData);
                }
            }

            if (skeletonData != null)
            {
                foreach (Skeleton skel in skeletonData)
                {
                    if (skel.TrackingState == SkeletonTrackingState.Tracked)
                    {
                        skeleton = skel;
                    }
                }
            }

        }

        private void DrawSkeleton(SpriteBatch spriteBatch, Vector2 resolution, Texture2D img)
        {
            if (skeleton != null)
            {
                foreach (Joint joint in skeleton.Joints)
                    
                {
                    DrawJoint(spriteBatch, skeleton.Joints[JointType.Head], resolution, skin[0], skin[0].Bounds.Center.X,skin[0].Bounds.Center.Y);
                    DrawJoint(spriteBatch, skeleton.Joints[JointType.ShoulderCenter], resolution, skin[1],0,0);
                    DrawJoint(spriteBatch, skeleton.Joints[JointType.HipCenter], resolution, skin[2], 0, 0);
                    DrawJoint(spriteBatch, skeleton.Joints[JointType.ShoulderLeft], resolution, skin[3], 0, 0);
                    DrawJoint(spriteBatch, skeleton.Joints[JointType.ShoulderRight], resolution, skin[4], 0, 0);
                    Vector2 position = new Vector2((((0.5f * joint.Position.X) + 0.5f) * (resolution.X)), (((-0.5f * joint.Position.Y) + 0.5f) * (resolution.Y)));
                    //Vector2 position = new Vector2(0.5f * joint.Position.X * resolution.X, -0.5f * joint.Position.Y * resolution.Y);
                    spriteBatch.Draw(img, new Rectangle(Convert.ToInt32(position.X), Convert.ToInt32(position.Y), 10, 10), Color.Red);
                }
                
                
            }

        }

        private void DrawJoint(SpriteBatch spriteBatch, Joint joint, Vector2 resolution,Texture2D img, int offsetX, int offsetY)
        {
            //int offsetX, offsetY = 0;
            //float offsetX = img.Width / 2;
            //float offsetY = img.Height / 2;
            Vector2 position = new Vector2((((0.5f * joint.Position.X) + 0.5f) * (resolution.X)) + offsetX, (((-0.5f * joint.Position.Y) + 0.5f) * (resolution.Y)) + offsetY);
            //Vector2 position = new Vector2(joint.Position.X * resolution.X, joint.Position.Y * resolution.Y);
            spriteBatch.Draw(img, position, Color.White);

        }

        private byte[] ConvertDepthFrame(short[] depthFrame, DepthImageStream depthStream)
        {
            int RedIndex = 0, GreenIndex = 1, BlueIndex = 2, AlphaIndex = 3;

            byte[] depthFrame32 = new byte[depthStream.FrameWidth * depthStream.FrameHeight * 4];

            for (int i16 = 0, i32 = 0; i16 < depthFrame.Length && i32 < depthFrame32.Length; i16++, i32 += 4)
            {
                int player = depthFrame[i16] & DepthImageFrame.PlayerIndexBitmask;
                int realDepth = depthFrame[i16] >> DepthImageFrame.PlayerIndexBitmaskWidth;

                // transform 13-bit depth information into an 8-bit intensity appropriate
                // for display (we disregard information in most significant bit)
                byte intensity = (byte)(~(realDepth >> 4));

                depthFrame32[i32 + RedIndex] = (byte)(intensity);
                depthFrame32[i32 + GreenIndex] = (byte)(intensity);
                depthFrame32[i32 + BlueIndex] = (byte)(intensity);
                depthFrame32[i32 + AlphaIndex] = 255;
            }

            return depthFrame32;
        }
    }
}
