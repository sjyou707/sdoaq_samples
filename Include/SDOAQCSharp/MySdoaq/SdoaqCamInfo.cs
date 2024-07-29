using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDOAQ_App_CS
{
    public class SdoaqCamInfo
    {
        public bool IsColor => ColorByte != 1;

        public int ColorByte { get; set; } = 1;
        public int PixelSize => AcqParam.cameraRoiWidth * AcqParam.cameraRoiHeight;
        public int ImgSize => PixelSize * ColorByte;
        public int DataSize => PixelSize * sizeof(float);
        public int PixelWidth => AcqParam.cameraRoiWidth;
        public int PixelHeight => AcqParam.cameraRoiHeight;
        public int Edofrecsize => 5;

        public SDOAQ.SDOAQ_API.AcquisitionFixedParameters AcqParam { get; set; }
        public SdoaqCamInfo()
        {
            AcqParam = new SDOAQ.SDOAQ_API.AcquisitionFixedParameters();
        }

        public string GetCamRoi()
        {
            return $"{AcqParam.cameraRoiLeft},{AcqParam.cameraRoiTop},{AcqParam.cameraRoiWidth},{AcqParam.cameraRoiHeight}";
        }
    }
}
