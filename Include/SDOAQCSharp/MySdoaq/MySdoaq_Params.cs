﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using SDOAQ;
using SDOAQCSharp.Tool;

namespace SDOAQCSharp
{
    partial class MySdoaq
    {
        private const string REGEX_PATTERN_COMMA_LIST_4 = @"^(\d+),(\d+),(\d+),(\d+)$";
        private const string REGEX_PATTEN_FOCUS_LHU = @"^(\d+)-(\d+)-(\d+)$";

        public bool IsExist(int idxCam)
        {
            return s_sdoaqObjList.ContainsKey(idxCam);
        }

        public SDOAQ_API.eErrorCode GetIntParamRange(SDOAQ_API.eParameterId param, out int min, out int max)
        {
            min = 0;
            max = 0;
            var tmp_min = new int[1] { 0 };
            var tmp_max = new int[1] { 0 };

            SelectMultiWS(CamIndex);

            var rv = SDOAQ_API.SDOAQ_GetIntParameterRange(param, tmp_min, tmp_max);

            min = tmp_min[0];
            max = tmp_max[0];

            if (rv != SDOAQ_API.eErrorCode.ecNoError)
            {
                WriteLog(Logger.emLogLevel.API, $"###SDOAQ_GetIntParameterRange(), ErrorCode = {rv}");
            }
            return rv;
        }

        public SDOAQ_API.eErrorCode GetDblParamRange(SDOAQ_API.eParameterId param, out double min, out double max)
        {
            min = 0;
            max = 0;
            var tmp_min = new double[1] { 0 };
            var tmp_max = new double[1] { 0 };

            SelectMultiWS(CamIndex);

            var rv = SDOAQ_API.SDOAQ_GetDblParameterRange(param, tmp_min, tmp_max);

            min = tmp_min[0];
            max = tmp_max[0];

            if (rv != SDOAQ_API.eErrorCode.ecNoError)
            {
                WriteLog(Logger.emLogLevel.API, $"###SDOAQ_GetDblParameterRange(), ErrorCode = {rv}");
            }
            return rv;
        }

        #region Roi
        public bool SetRoi(string roi)
        {
            if (ConvertStrParam_CommaList_4(roi, out var params_roi) == false)
            {
                WriteLog(Logger.emLogLevel.Error, $"roi Param Invalid Pattern({roi})... Roi Parms = int,int,int,int (Left,Top,Width,Height)");
            }
            
            return SetRoi(params_roi);
        }

        public bool SetRoi(int[] params_roi)
        {
            var acqParams = new SDOAQ_API.AcquisitionFixedParameters();
            var rv = GetAcqParamByRoi(params_roi, ref acqParams);

            if (rv && IsRunPlayer == false)
            {
                CamInfo.AcqParam = acqParams;
            }

            return rv;
        }
        public bool SetRoi_AF(string roi)
        {
            if (ConvertStrParam_CommaList_4(roi, out var params_roi) == false)
            {
                WriteLog(Logger.emLogLevel.Error, $"AF roi Param Invalid Pattern({roi})... AF Roi Parms = int,int,int,int (Left,Top,Width,Height)");
            }

            return SetRoi_AF(params_roi);
        }

        public bool SetRoi_AF(int[] params_roi)
        {
            var acqParams = new SDOAQ_API.AcquisitionFixedParameters();
            var rv = GetAcqParamByRoi(params_roi, ref acqParams);

            if (rv && IsRunPlayer == false)
            {
                int paramFocus_LeftTop = ((acqParams.cameraRoiLeft & 0x0000FFFF) << 16)
                                        | ((acqParams.cameraRoiTop & 0x0000FFFF) << 0);

                int paramFocus_RithgBtm = (((acqParams.cameraRoiLeft + acqParams.cameraRoiWidth) & 0x0000FFFF) << 16)
                                        | (((acqParams.cameraRoiTop + acqParams.cameraRoiHeight) & 0x0000FFFF) << 0);

                SDOAQ_API.SDOAQ_SetIntParameterValue(SDOAQ_API.eParameterId.piFocusLeftTop, paramFocus_LeftTop);
                SDOAQ_API.SDOAQ_SetIntParameterValue(SDOAQ_API.eParameterId.piFocusRightBottom, paramFocus_RithgBtm);
            }
            return rv;
        }
        public bool GetRoi_AF(out int[] params_roi)
        {
            params_roi = new int[4];

            int[] leftTop = new int[] { 0 };
            int[] rightBottom = new int[] { 0 };

            SDOAQ_API.SDOAQ_GetIntParameterValue(SDOAQ_API.eParameterId.piFocusLeftTop, leftTop);
            SDOAQ_API.SDOAQ_GetIntParameterValue(SDOAQ_API.eParameterId.piFocusRightBottom, rightBottom);

            int i = 0;

            params_roi[i++] = (leftTop[0] >> 16) & 0x0000FFFF; // left;
            params_roi[i++] = (leftTop[0] >> 0) & 0x0000FFFF; // top;
            params_roi[i++] = (rightBottom[0] >> 16) & 0x0000FFFF; // right;
            params_roi[i++] = (rightBottom[0] >> 0) & 0x0000FFFF; // bottom;

            return true;
        }

        private bool GetAcqParamByRoi(int[] params_roi, ref SDOAQ_API.AcquisitionFixedParameters acqParams)
        {
            if (params_roi == null || params_roi.Length < 4)
            {
                return false;
            }

            acqParams.cameraRoiLeft   = params_roi[0];
            acqParams.cameraRoiTop    = params_roi[1];
            acqParams.cameraRoiWidth  = params_roi[2];
            acqParams.cameraRoiHeight = params_roi[3];
            
            if (GetIntParamRange(SDOAQ_API.eParameterId.piCameraFullFrameSizeX, out var frameSize_x_min, out var frameSize_x_max) != SDOAQ_API.eErrorCode.ecNoError)
            {
                if (acqParams.cameraRoiLeft < 0 || acqParams.cameraRoiLeft > frameSize_x_max)
                {
                    WriteLog(Logger.emLogLevel.API, "Set cameraRoiLeft : value is out of range[{0} ~ {1}]", frameSize_x_min, frameSize_x_max);

                    acqParams.cameraRoiLeft = Math.Min(frameSize_x_max, Math.Max(0, acqParams.cameraRoiLeft));
                }

                int width = acqParams.cameraRoiLeft + acqParams.cameraRoiWidth;

                if (width < 1 || width > frameSize_x_max)
                {
                    WriteLog(Logger.emLogLevel.API, "Set cameraRoiWidth : value is out of range[{0} ~ {1}]", frameSize_x_min, frameSize_x_max);

                    acqParams.cameraRoiWidth = Math.Min(frameSize_x_max - acqParams.cameraRoiLeft, Math.Max(1, width));
                }
            }


            if (GetIntParamRange(SDOAQ_API.eParameterId.piCameraFullFrameSizeY, out var frameSize_y_min, out var frameSize_y_max) != SDOAQ_API.eErrorCode.ecNoError)
            {
                if (acqParams.cameraRoiTop < 0 || acqParams.cameraRoiTop > frameSize_y_max)
                {
                    WriteLog(Logger.emLogLevel.API, "Set cameraRoiTop : value is out of range[{0} ~ {1}]", frameSize_y_min, frameSize_y_max);
                    acqParams.cameraRoiTop = Math.Min(frameSize_y_max, Math.Max(0, acqParams.cameraRoiTop));
                }

                int height = acqParams.cameraRoiTop + acqParams.cameraRoiHeight;

                if (height < 1 || height > frameSize_y_max)
                {
                    WriteLog(Logger.emLogLevel.API, "Set cameraRoiHeight : value is out of range[{0} ~ {1}]", frameSize_y_min, frameSize_y_max);
                    acqParams.cameraRoiHeight = Math.Min(frameSize_y_max - acqParams.cameraRoiTop, Math.Max(1, height));
                }
            }
            
            return true;
        }
        #endregion

        #region Focus
        public bool SetFocus(string focus)
        {
            if (ConvertStrPara_FocusList(focus, out int low, out int high, out int unit))
            {
                FocusList.SetFocusList(low, high, unit);
                return true;
            }
            return false;
        }
        

        public bool SetSnapFocus(string focus)
        {
            if (ConvertStrPara_FocusList(focus, out int low, out int high, out int unit))
            {
                SnapFocusList.SetFocusList(low, high, unit);
            }
            return false;
        }
        
        #endregion

        public bool SetCalibrationFile(string fileName)
        {
            if (System.IO.File.Exists(fileName))
            {
                SelectMultiWS(CamIndex);
                SDOAQ_API.SDOAQ_SetCalibrationFile(fileName);

                WriteLog(Logger.emLogLevel.Info, ">> calibration data of file [{0}] is set.", fileName);
                return true;
            }
            return false;
        }

        public bool SetParam(SDOAQ_API.eParameterId paramID, string paramValue)
        {
            SelectMultiWS(CamIndex);
            var availables = new int[1] { 0 };
            var rv = SDOAQ_API.SDOAQ_IsParameterAvailable(paramID, availables);

            if (rv != SDOAQ_API.eErrorCode.ecNoError)
            {
                WriteLog(Logger.emLogLevel.API, $"###SDOAQ_IsParameterAvailable(), ParamID = {paramID}, Value = {paramValue}, Error Code = {rv}");
                return false;
            }

            int[] writables = new int[1] { 0 };
            rv = SDOAQ_API.SDOAQ_IsParameterWritable(paramID, writables);

            if (rv != SDOAQ_API.eErrorCode.ecNoError)
            {
                WriteLog(Logger.emLogLevel.API, $"###SDOAQ_IsParameterWritable(), ParamID = {paramID}, Value = {paramValue}, Error Code = {rv}");
                return false;
            }

            if (availables[0] == 0)
            {
                WriteLog(Logger.emLogLevel.API, $"SetParam(), param state Unavailable. ParamID = {paramID}, Value = {paramValue}");
                return false;
            }
            
            if (writables[0] == 0)
            {
                WriteLog(Logger.emLogLevel.API, $"SetParam(), param state Non-writable. ParamID = {paramID}, Value = {paramValue}");
                return false;
            }

            var paramTypes = new SDOAQ_API.eParameterType[1];
            SDOAQ_API.SDOAQ_GetParameterType(paramID, paramTypes);

            switch (paramTypes[0])
            {
                case SDOAQ_API.eParameterType.ptInt:
                    {
                        if (int.TryParse(paramValue, out int val) == false)
                        {
                            WriteLog(Logger.emLogLevel.API, $"SetParam(), int param parse false. ParamID = {paramID}, Value = {paramValue}");
                            return false;
                        }
                        
                        GetIntParamRange(paramID, out int min, out int max);

                        val = Math.Min(max, Math.Max(min, val));

                        rv = SDOAQ_API.SDOAQ_SetIntParameterValue(paramID, val);

                        WriteLog(Logger.emLogLevel.API, $"SDOAQ_SetIntParameterValue(), rv = {rv}, Pararm = {paramID}, Value = {val}");
                    }
                    break;
                case SDOAQ_API.eParameterType.ptDouble:
                    {
                        if (double.TryParse(paramValue, out double val) == false)
                        {
                            WriteLog(Logger.emLogLevel.API, $"SetParam(), double param parse false. ParamID = {paramID}, Value = {paramValue}");
                            return false;
                        }

                        rv = GetDblParamRange(paramID, out double min, out double max);

                        val = Math.Min(max, Math.Max(min, val));

                        SDOAQ_API.SDOAQ_SetDblParameterValue(paramID, val);

                        WriteLog(Logger.emLogLevel.API, $"SDOAQ_SetDblParameterValue(), rv = {rv}, Pararm = {paramID}, Value = {val}");
                    }
                    break;
                case SDOAQ_API.eParameterType.ptString:
                    {
                        if (string.IsNullOrEmpty(paramValue))
                        {
                            WriteLog(Logger.emLogLevel.API, $"SetParam(), string param null or empty. ParamID = {paramID}, Value = {paramValue}");
                            return false;
                        }

                        rv = SDOAQ_API.SDOAQ_SetStringParameterValue(paramID, paramValue);

                        WriteLog(Logger.emLogLevel.API, $"SDOAQ_SetStringParameterValue(), rv = {rv}, Pararm = {paramID}, Value = {paramValue}");
                    }
                    break;
                default:
                    WriteLog(Logger.emLogLevel.API, $"SetParam(), Invalid Param Type({paramTypes[0]}). ParamID = {paramID}, Value = {paramValue}");
                    return false;
            }
            return true;
        }

        public bool GetParam(SDOAQ_API.eParameterId paramID, 
            out bool isWritable,
            out string paramValue)
        {
            SelectMultiWS(CamIndex);

            isWritable = false;
            paramValue = string.Empty;

            var availables = new int[1] { 0 };
            var rv = SDOAQ_API.SDOAQ_IsParameterAvailable(paramID, availables);

            if (rv != SDOAQ_API.eErrorCode.ecNoError)
            {
                WriteLog(Logger.emLogLevel.API, $"###SDOAQ_IsParameterAvailable(), ParamID = {paramID}, Error Code = {rv}");
                return false;
            }

            int[] writables = new int[1] { 0 };
            rv = SDOAQ_API.SDOAQ_IsParameterWritable(paramID, writables);

            if (rv != SDOAQ_API.eErrorCode.ecNoError)
            {
                WriteLog(Logger.emLogLevel.API, $"###SDOAQ_IsParameterWritable(), ParamID = {paramID}, Error Code = {rv}");
                return false;
            }

            isWritable = Convert.ToBoolean(writables[0]);

            if (availables[0] == 0)
            {
                WriteLog(Logger.emLogLevel.API, $"SetParam(), param state Unavailable. ParamID = {paramID}");
                return false;
            }

            var paramTypes = new SDOAQ_API.eParameterType[1];
            SDOAQ_API.SDOAQ_GetParameterType(paramID, paramTypes);

            switch(paramTypes[0])
            {
                case SDOAQ_API.eParameterType.ptInt:
                    {
                        var values = new int[] { 0 };
                        SDOAQ_API.SDOAQ_GetIntParameterValue(paramID, values);

                        if (paramID == SDOAQ_API.eParameterId.piReflexCorrectionPattern)
                        {
                            paramValue = $"0x{values[0]:x2}";
                        }
                        else
                        {
                            paramValue = values[0].ToString();
                        }
                    }
                    break;
                case SDOAQ_API.eParameterType.ptDouble:
                    {
                        var values = new double[] { 0 };
                        SDOAQ_API.SDOAQ_GetDblParameterValue(paramID, values);
                        paramValue = values[0].ToString();
                    }
                    break;
                case SDOAQ_API.eParameterType.ptString:
                    {
                        var size = new int[] { 1024 };
                        var value = new StringBuilder(size[0]);
                        SDOAQ_API.SDOAQ_GetStringParameterValue(paramID, value, size);
                        paramValue = value.ToString();
                    }
                    break;
                default:
                    WriteLog(Logger.emLogLevel.API, $"GetParam(), Invalid Param Type({paramTypes[0]}). ParamID = {paramID}");
                    return false;
            }

            return true;
        }

        private bool ConvertStrParam_CommaList_4(string param, 
            out int[] roi)
        {
            roi = new int[4] { 0, 0, 0, 0};
            var match = Regex.Match(param, REGEX_PATTERN_COMMA_LIST_4);
            if (match.Success)
            {
                for (int i = 0; i < roi.Length; i++)
                {
                    roi[i] = int.Parse(match.Groups[i + 1].Value);
                }

                return true;
            }
            return false;
        }

        private bool ConvertStrPara_FocusList(string param, 
            out int low,
            out int high,
            out int unit)
        {
            low = 0;
            high = 0;
            unit = 0;

            var match = Regex.Match(param, REGEX_PATTEN_FOCUS_LHU);

            if (match.Success)
            {
                low     = int.Parse(match.Groups[1].Value);
                high    = Math.Min(MAX_FOCUS_STEP, int.Parse(match.Groups[2].Value));
                unit    = int.Parse(match.Groups[3].Value);
                
                return true;
            }
            return false;
        }
    }
}
