using System;
using FFmpeg.AutoGen;

namespace EmguFFmpeg
{
    /// <summary>
    /// must call <see cref="FFmpegHelper.RegisterDevice"/>
    /// </summary>
    public unsafe class MediaDevice
    {
        public static MediaDictionary ListDevicesOptions
        {
            get
            {
                MediaDictionary dict = new MediaDictionary();
                dict.Add("list_devices", "true");
                return dict;
            }
        }

        /// <summary>
        /// NOTE: ffmpeg cannot get device information through code, only print the display.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="parame"></param>
        /// <param name="options">options for <see cref=" ffmpeg.avformat_open_input"/></param>
        /// <returns></returns>
        public static void PrintDeviceInfos(MediaFormat device, string parame, MediaDictionary options = null)
        {
            AVFormatContext* pFmtCtx = ffmpeg.avformat_alloc_context();
            ffmpeg.av_log(null, (int)LogLevel.Verbose, $"--------------------------{Environment.NewLine}");
            if (device is InFormat iformat)
                ffmpeg.avformat_open_input(&pFmtCtx, parame, iformat, options);
            else if (device is OutFormat oformat)
                ffmpeg.avformat_alloc_output_context2(&pFmtCtx, oformat, null, parame);
            ffmpeg.av_log(null, (int)LogLevel.Verbose, $"--------------------------{Environment.NewLine}");
            ffmpeg.avformat_free_context(pFmtCtx);
        }

        /* ffmpeg not implemented
        public bool IsDefaultDevice { get; private set; }
        public string DeviceName { get; private set; }
        public string DeviceDescription { get; private set; }
        private static IReadOnlyList<IReadOnlyList<MediaDevice>> CopyAndFree(AVDeviceInfoList** ppDeviceInfoList, int deviceInfoListLength)
        {
            List<List<MediaDevice>> result = new List<List<MediaDevice>>();
            if (deviceInfoListLength > 0 && ppDeviceInfoList != null)
            {
                for (int i = 0; i < deviceInfoListLength; i++)
                {
                    List<MediaDevice> infos = new List<MediaDevice>();
                    for (int j = 0; j < ppDeviceInfoList[i]->nb_devices; j++)
                    {
                        AVDeviceInfo* deviceInfo = ppDeviceInfoList[i]->devices[j];
                        MediaDevice info = new MediaDevice()
                        {
                            DeviceName = ((System.IntPtr)deviceInfo->device_name).PtrToStringUTF8(),
                            DeviceDescription =  ((System.IntPtr)deviceInfo->device_description).PtrToStringUTF8(),
                            IsDefaultDevice = j == ppDeviceInfoList[i]->default_device,
                        };
                        infos.Add(info);
                    }
                    result.Add(infos);
                }
                ffmpeg.avdevice_free_list_devices(ppDeviceInfoList);
            }
            return result;
        }

        public static IReadOnlyList<IReadOnlyList<MediaDevice>> GetDeviceInfos(MediaFormat device, MediaDictionary options = null)
        {
            int deviceInfoListLength = 0;
            AVDeviceInfoList* pDeviceInfoList = null;
            if (device is InFormat iformat)
                deviceInfoListLength = ffmpeg.avdevice_list_input_sources(iformat, null, options, &pDeviceInfoList);
            else if (device is OutFormat oformat)
                deviceInfoListLength = ffmpeg.avdevice_list_output_sinks(oformat, null, options, &pDeviceInfoList);
            deviceInfoListLength.ThrowExceptionIfError();
            return CopyAndFree(&pDeviceInfoList, deviceInfoListLength);
        }

        /// <summary>
        /// get output device infos
        /// </summary>
        /// <param name="deviceName"></param>
        /// <param name="options"></param>
        /// <returns></returns>

        public static IReadOnlyList<IReadOnlyList<MediaDevice>> GetOutputDeviceInfos(string deviceName, MediaDictionary options = null)
        {
            AVDeviceInfoList* pDeviceInfoList = null;
            int deviceInfoListLength = ffmpeg.avdevice_list_output_sinks(null, deviceName, options, &pDeviceInfoList);
            return CopyAndFree(&pDeviceInfoList, deviceInfoListLength);
        }

        /// <summary>
        /// get input device infos
        /// </summary>
        /// <param name="deviceName"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IReadOnlyList<IReadOnlyList<MediaDevice>> GetInputDeviceInfos(string deviceName, MediaDictionary options = null)
        {
            AVDeviceInfoList* pDeviceInfoList = null;
            int deviceInfoListLength = ffmpeg.avdevice_list_input_sources(null, deviceName, options, &pDeviceInfoList);
            return CopyAndFree(&pDeviceInfoList, deviceInfoListLength);
        }
        */
    }
}
