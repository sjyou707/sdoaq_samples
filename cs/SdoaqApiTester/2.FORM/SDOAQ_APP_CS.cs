using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.IO;
using System.Drawing.Imaging;
using System.Diagnostics;
using static SDOAQ.SDOAQ_API;
using static SDOWSIO.WSIO;
using static SDOWSIO.WSIO.UTIL;
using System.Security.Cryptography;

namespace SDOAQ_App_CS
{
	public partial class SDOAQ_APP_CS : Form
	{
		#region 클래스 데이터
		//----------------------------------------------------------------------------
		private SDOAQ_TEST_SET SET = new SDOAQ_TEST_SET();

		//----------------------------------------------------------------------------
		private int m_nRingBufferSize = 3;
		private int m_nContiStack = 0;
		private int m_nContiEdof = 0;
		private int m_nContiAF = 0;
		private const int m_nMaxViewer = 3;
		private IntPtr frmHandle = (IntPtr)null;
		private bool frmclosing = false;
		private IntPtr m_hwnd3D = (IntPtr)null;

		private enum eFocus { DFLT_FOCUS_STEP = 160, MAX_FOCUS_STEP = 320 };

		private List<int> m_vFocusSet = new List<int>();
		private List<int> m_vSnapFocusSet = new List<int>();
		private List<PictureBox> m_vPictBoxes = new List<PictureBox>();
		private List<ViewerData> m_vhwndIV = new List<ViewerData>();
		//----------------------------------------------------------------------------
		public static SDOAQ_LogCallback sDOAQ_LogCallback;
		public static SDOAQ_ErrorCallback sDOAQ_ErrorCallback;
		public static SDOAQ_InitDoneCallback sDOAQ_InitDoneCallback;
		public static SDOAQ_PlayCallback sDOAQ_PlayCallback;
		public static SDOAQ_PlayCallback sDOAQ_PlayEdofCallback;
		public static SDOAQ_PlayAfCallback sDOAQ_PlayAfCallback;
		public static SDOAQ_SnapCallback sDOAQ_SnapCallback;

		//----------------------------------------------------------------------------
		public delegate void EventHandler(object sender, params object[] objs);
		public static event EventHandler DllEventHandler;


		public enum EUserMessage
		{
			EUM_LOG = 0xA000,
			EUM_ERROR,
			EUM_INITDONE,
			EUM_RECEIVE_ZSTACK,
			EUM_RECEIVE_EDOF,
			EUM_RECEIVE_AF,
			EUM_RECEIVE_SNAP
		};
		#endregion

		#region 초기화
		//----------------------------------------------------------------------------
		public SDOAQ_APP_CS()
		{
			InitializeComponent();

			m_vhwndIV.Add(new ViewerData(IntPtr.Zero, pbViewer1));
			m_vhwndIV.Add(new ViewerData(IntPtr.Zero, pbViewer2));
			m_vhwndIV.Add(new ViewerData(IntPtr.Zero, pbViewer3));
			foreach (var item in m_vhwndIV)
			{
				//public static extern WSIORV WSUT_IV_CreateImageViewer(string profile_str, IntPtr parent_hwnd, out IntPtr ptr_viewer_hwnd, uint viewer_control_id, WSUTIVOPMODE operation_mode);

				if (WSIORV.WSIORV_SUCCESS <= UTIL.WSUT_IV_CreateImageViewer("Main viewer"
					, item.pBox.Handle, out item.vHwnd, 0
					, WSUTIVOPMODE.WSUTIVOPMODE_VISION | WSUTIVOPMODE.WSUTIVOPMODE_TOPTITLE | WSUTIVOPMODE.WSUTIVOPMODE_FRAMEOUTER
				))
				{
					WSUT_IV_SetColor(item.vHwnd, WSUTIVRESOURCE.WSUTIVRESOURCE_OUTERFRAME, Utils.RGB(70, 130, 180));
				}
				else
				{
					item.vHwnd = IntPtr.Zero;
					Utils.print_wsio_last_error();
				}
			}

			int nVersion = GL.WSGL_GetMajorVersion();
			if (WSIORV.WSIORV_SUCCESS <= GL.WSGL_Initialize(this.pbViewer4.Handle, out m_hwnd3D))
			{
				var attributes = GL.EDisplayAttributes.EDA_SHOW_GUIDER_OBJECTS
								| GL.EDisplayAttributes.EDA_SHOW_SCALE_OBJECTS
								| GL.EDisplayAttributes.EDA_SHOW_COLORMAPBAR_OBJECTS
								| GL.EDisplayAttributes.EDA_NOHIDE_PICKER
								;
				GL.WSGL_SetDisplayAttributes(m_hwnd3D, (int)attributes);
			}
		}
		#endregion

		#region APP 시작
		//----------------------------------------------------------------------------
		private void SDOAQ_APP_CS_Load(object sender, EventArgs e)
		{
			InitSdoAqApp();
		}

		//----------------------------------------------------------------------------
		private void InitSdoAqApp()
		{
			var strVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
			this.Text = this.Text + string.Format(" (Ver: {0}, Build: {1})", strVersion, Get_BuildDateTime(strVersion).ToString("yyyy.MM.dd HH:mm:ss"));

			sDOAQ_LogCallback = g_LogCallback;
			sDOAQ_ErrorCallback = g_ErrorCallback;
			sDOAQ_InitDoneCallback = g_InitDoneCallback;
			sDOAQ_PlayCallback = g_PlayFocusStackCallback;
			sDOAQ_PlayEdofCallback = g_PlayEdofCallback;
			sDOAQ_PlayAfCallback = g_PlayAFCallBack;
			sDOAQ_SnapCallback = g_SnapCallback;
			DllEventHandler += new EventHandler(CallbackHandler);
			LogPrintf = Utils.WriteLog;

			SET.bufHandlePtr = IntPtr.Zero;

			m_vPictBoxes.Add(pbViewer1);
			m_vPictBoxes.Add(pbViewer2);
			m_vPictBoxes.Add(pbViewer3);
			m_vPictBoxes.Add(pbViewer4);

			foreach (var item in m_vhwndIV)
			{
				WSUT_IV_ShowWindow(item.vHwnd, 1, item.pBox.Left, item.pBox.Top, item.pBox.Right, item.pBox.Bottom);
			}

			GL.WSGL_ShowWindow(m_hwnd3D, true, pbViewer4.Left, pbViewer4.Top, pbViewer4.Right, pbViewer4.Bottom);
			GL.WSGL_Display_BG(m_hwnd3D);
		}
		#endregion

		#region APP 종료
		//----------------------------------------------------------------------------
		private void SDOAQ_APP_CS_FormClosing(object sender, FormClosingEventArgs e)
		{
			frmclosing = true;

			SET.ClearBuffer();

			foreach (var item in m_vhwndIV)
			{
				WSUT_IV_DestroyImageViewer(item.vHwnd);
			}

			GL.WSGL_Finalize(m_hwnd3D);
		}
		#endregion

		#region Callback Functions [C++ DLL]
		//----------------------------------------------------------------------------
		public static void g_LogCallback(eLogSeverity severity, StringBuilder pMessage)
		{
			DllEventHandler?.Invoke(null, new object[] { EUserMessage.EUM_LOG, severity, pMessage });
		}

		//----------------------------------------------------------------------------
		public static void g_ErrorCallback(eErrorCode errorCode, StringBuilder pErrorMessage)
		{
			DllEventHandler?.Invoke(null, new object[] { EUserMessage.EUM_ERROR, errorCode, pErrorMessage });
		}

		//----------------------------------------------------------------------------
		public static void g_InitDoneCallback(eErrorCode errorCode, StringBuilder pErrorMessage)
		{
			DllEventHandler?.Invoke(null, new object[] { EUserMessage.EUM_INITDONE, errorCode, pErrorMessage });
		}

		//----------------------------------------------------------------------------
		public static void g_PlayFocusStackCallback(eErrorCode errorCode, int lastFilledRingBufferEntry)
		{
			DllEventHandler?.Invoke(null, new object[] { EUserMessage.EUM_RECEIVE_ZSTACK, errorCode, lastFilledRingBufferEntry });
		}

		//----------------------------------------------------------------------------
		public static void g_PlayEdofCallback(eErrorCode errorCode, int lastFilledRingBufferEntry)
		{
			DllEventHandler?.Invoke(null, new object[] { EUserMessage.EUM_RECEIVE_EDOF, errorCode, lastFilledRingBufferEntry });
		}

		//----------------------------------------------------------------------------
		public static void g_PlayAFCallBack(eErrorCode errorCode, int lastFilledRingBufferEntry, double dbBestFocusStep, double dbBestScore)
		{
			DllEventHandler?.Invoke(null, new object[] { EUserMessage.EUM_RECEIVE_AF, errorCode, lastFilledRingBufferEntry, dbBestFocusStep, dbBestScore });
		}

		//----------------------------------------------------------------------------
		public static void g_SnapCallback(eErrorCode errorCode, int lastFilledRingBufferEntry)
		{
			DllEventHandler?.Invoke(null, new object[] { EUserMessage.EUM_RECEIVE_SNAP, errorCode, lastFilledRingBufferEntry });
		}

		//----------------------------------------------------------------------------
		private void CallbackHandler(object sender, params object[] objs)
		{
			var idx = 0;
			var usrMsg = (EUserMessage)objs[idx++];
			string sMsg = string.Empty;

			switch (usrMsg)
			{
				case EUserMessage.EUM_LOG:
					{
						eLogSeverity eSeverity = (eLogSeverity)objs[idx++];
						StringBuilder pMessage = (StringBuilder)objs[idx++];

						sMsg = string.Format("[LogCallback : {0}] {1}{2}", eSeverity, pMessage, Environment.NewLine);
					}
					break;

				case EUserMessage.EUM_ERROR:
					{
						eErrorCode eErrorCode = (eErrorCode)objs[idx++];
						StringBuilder pMessage = (StringBuilder)objs[idx++];

						sMsg = string.Format("[ErrorCallback : ErrorCode:{0}] {1}{2}", eErrorCode, pMessage, Environment.NewLine);
					}
					break;

				case EUserMessage.EUM_INITDONE:
					{
						eErrorCode eErrorCode = (eErrorCode)objs[idx++];
						StringBuilder pMessage = (StringBuilder)objs[idx++];

						sMsg = string.Format("[InitDoneCallback : ErrorCode:{0}] {1}{2}", eErrorCode, pMessage, Environment.NewLine);

						if (eErrorCode.ecNoError == eErrorCode)
						{
							ReadySdoaqDll();

							if (btnInit.InvokeRequired == true)
							{
								this.BeginInvoke(new MethodInvoker(
									delegate ()
									{
										var Parameters = new[]
										{								
											new{ Name = "piCameraExposureTime", Value = ((int)eParameterId.piCameraExposureTime).ToString() },
											new{ Name = "piCameraFullFrameSizeX", Value = ((int)eParameterId.piCameraFullFrameSizeX).ToString() },
											new{ Name = "piCameraFullFrameSizeY", Value = ((int)eParameterId.piCameraFullFrameSizeY).ToString() },
											new{ Name = "piCameraPixelSizeX", Value = ((int)eParameterId.piCameraPixelSizeX).ToString() },
											new{ Name = "piCameraPixelSizeY", Value = ((int)eParameterId.piCameraPixelSizeY).ToString() },
											new{ Name = "piCameraBinning", Value = ((int)eParameterId.piCameraBinning).ToString() },
											new{ Name = "piCameraGain", Value = ((int)eParameterId.piCameraGain).ToString() },
											new{ Name = "piWhiteBalanceRed", Value = ((int)eParameterId.piWhiteBalanceRed).ToString() },
											new{ Name = "piWhiteBalanceGreen", Value = ((int)eParameterId.piWhiteBalanceGreen).ToString() },
											new{ Name = "piWhiteBalanceBlue", Value = ((int)eParameterId.piWhiteBalanceBlue).ToString() },
											new{ Name = "piCameraColor", Value = ((int)eParameterId.piCameraColor).ToString() },

											new{ Name = "piFocusPosition", Value = ((int)eParameterId.piFocusPosition).ToString() },
											new{ Name = "piSaveFileFormat", Value = ((int)eParameterId.piSaveFileFormat).ToString() },
											new{ Name = "piSavePixelBits - not supported yet", Value = ((int)eParameterId.piSavePixelBits).ToString() },
											new{ Name = "piFocusLeftTop", Value = ((int)eParameterId.piFocusLeftTop).ToString() },
											new{ Name = "piFocusRightBottom", Value = ((int)eParameterId.piFocusRightBottom).ToString() },
											new{ Name = "piFocusMeasureMethod", Value = ((int)eParameterId.piFocusMeasureMethod).ToString() },
											new{ Name = "piSingleFocus", Value = ((int)eParameterId.piSingleFocus).ToString() },
											new{ Name = "piObjectiveId", Value = ((int)eParameterId.piObjectiveId).ToString() },

											new{ Name = "piReflexCorrectionAlgorithm", Value = ((int)eParameterId.piReflexCorrectionAlgorithm).ToString() },
											new{ Name = "piReflexCorrectionPattern", Value = ((int)eParameterId.piReflexCorrectionPattern).ToString() },
											new{ Name = "piMaxStacksPerSecond", Value = ((int)eParameterId.piMaxStacksPerSecond).ToString() },
											new{ Name = "pi_edof_calc_resize_ratio", Value = ((int)eParameterId.pi_edof_calc_resize_ratio).ToString() },
											new{ Name = "pi_edof_calc_pixelwise_kernel_size", Value = ((int)eParameterId.pi_edof_calc_pixelwise_kernel_size).ToString() },
											new{ Name = "pi_edof_calc_pixelwise_iteration", Value = ((int)eParameterId.pi_edof_calc_pixelwise_iteration).ToString() },
											new{ Name = "pi_edof_depthwise_kernel_size", Value = ((int)eParameterId.pi_edof_depthwise_kernel_size).ToString() },
											new{ Name = "pi_edof_depth_quality_th", Value = ((int)eParameterId.pi_edof_depth_quality_th).ToString() },
											new{ Name = "pi_edof_bilateral_sigma_color", Value = ((int)eParameterId.pi_edof_bilateral_sigma_color).ToString() },
											new{ Name = "pi_edof_bilateral_sigma_space", Value = ((int)eParameterId.pi_edof_bilateral_sigma_space).ToString() },
											new{ Name = "pi_edof_num_thread", Value = ((int)eParameterId.pi_edof_num_thread).ToString() },

											new{ Name = "piInnerRingIntensity", Value = ((int)eParameterId.piInnerRingIntensity).ToString() },
											new{ Name = "piMiddleRingIntensity", Value = ((int)eParameterId.piMiddleRingIntensity).ToString() },
											new{ Name = "piOuterRingIntensity", Value = ((int)eParameterId.piOuterRingIntensity).ToString() },
											new{ Name = "piCoaxIntensity", Value = ((int)eParameterId.piCoaxIntensity).ToString() },
											new{ Name = "piIntensityGeneralChannel_1", Value = ((int)eParameterId.piIntensityGeneralChannel_1).ToString() },
											new{ Name = "piIntensityGeneralChannel_2", Value = ((int)eParameterId.piIntensityGeneralChannel_2).ToString() },
											new{ Name = "piIntensityGeneralChannel_3", Value = ((int)eParameterId.piIntensityGeneralChannel_3).ToString() },
											new{ Name = "piIntensityGeneralChannel_4", Value = ((int)eParameterId.piIntensityGeneralChannel_4).ToString() },
											new{ Name = "piIntensityGeneralChannel_5", Value = ((int)eParameterId.piIntensityGeneralChannel_5).ToString() },
											new{ Name = "piIntensityGeneralChannel_6", Value = ((int)eParameterId.piIntensityGeneralChannel_6).ToString() },
											new{ Name = "piIntensityGeneralChannel_7", Value = ((int)eParameterId.piIntensityGeneralChannel_7).ToString() },
											new{ Name = "piIntensityGeneralChannel_8", Value = ((int)eParameterId.piIntensityGeneralChannel_8).ToString() }
										};

										comboParam.DisplayMember = "Name";
										comboParam.ValueMember = "Value";
										comboParam.DataSource = Parameters;
									}));
							}

							double[] dbValue = new double[1] { 0 };
							eErrorCode rv = SDOAQ_GetDblParameterValue(eParameterId.pi_edof_calc_resize_ratio, dbValue);
							if (eErrorCode.ecNoError == rv)
							{
								if (btnInit.InvokeRequired == true)
								{
									this.BeginInvoke(new MethodInvoker(
									delegate ()
									{
										tbEdofRatio.Text = dbValue[0].ToString();
									}));
								}
								else
								{
									tbEdofRatio.Text = dbValue[0].ToString();
								}
							}

							int[] nValue = new int[1] { 0 };
							rv = SDOAQ_GetIntParameterValue(eParameterId.piCameraColor, nValue);
							if (eErrorCode.ecNoError == rv)
							{
								SET.COLORBYTES = (nValue[0] == 0) ? 3 : 1;
							}

							OnSdoaqSetROI();
							OnSdoaqSetAFROI();
							OnSdoaqSetRingBufSize();
							OnSdoaqSetFocusSet();
							OnSdoaqSetSnapFocusSet();
						}
					}
					break;

				case EUserMessage.EUM_RECEIVE_ZSTACK:
					{
						eErrorCode eErrorCode = (eErrorCode)objs[idx++];
						int lastFilledRingBufferEntry = (int)objs[idx++];

						OnReceiveZstack(eErrorCode, lastFilledRingBufferEntry);
					}
					break;

				case EUserMessage.EUM_RECEIVE_EDOF:
					{
						eErrorCode eErrorCode = (eErrorCode)objs[idx++];
						int lastFilledRingBufferEntry = (int)objs[idx++];

						OnReceiveEdof(eErrorCode, lastFilledRingBufferEntry);
					}
					break;

				case EUserMessage.EUM_RECEIVE_AF:
					{
						eErrorCode eErrorCode = (eErrorCode)objs[idx++];
						int lastFilledRingBufferEntry = (int)objs[idx++];
						double dbBestFocusStep = (double)objs[idx++];
						double dbBestScore = (double)objs[idx++];

						OnReceiveAF(eErrorCode, lastFilledRingBufferEntry, dbBestFocusStep, dbBestScore);
					}
					break;

				case EUserMessage.EUM_RECEIVE_SNAP:
					{
						eErrorCode eErrorCode = (eErrorCode)objs[idx++];
						int lastFilledRingBufferEntry = (int)objs[idx++];
						// snap is done.
					}
					break;
			}

			ShowLog(sMsg);
		}

		#region NativeMessage 처리 (PeekMessage)
		[StructLayout(LayoutKind.Sequential)]
		public struct NativeMessage
		{
			public IntPtr handle;
			public uint msg;
			public IntPtr wParam;
			public IntPtr lParam;
			public uint time;
			public System.Drawing.Point p;
		}
		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool PeekMessage(out NativeMessage lpMsg, HandleRef hWnd, uint wMsgFilterMin,
		   uint wMsgFilterMax, uint wRemoveMsg);
		#endregion

		//----------------------------------------------------------------------------
		unsafe public void OnReceiveZstack(eErrorCode eErrorCode, int lLastFilledRingBufferEntry)
		{
			if (eErrorCode.ecNoError != eErrorCode)
			{
				ApiError("SDOAQ_ContinuousAcquisitionCallback", eErrorCode);
			}
			else if (SET.rb.active)
			{
				NativeMessage message = new NativeMessage();
				while (PeekMessage(out message, new HandleRef(this, frmHandle), (uint)EUserMessage.EUM_RECEIVE_ZSTACK, (uint)EUserMessage.EUM_RECEIVE_ZSTACK, 0x0001 /*PM_REMOVE*/))
				{
					ShowLog(string.Format(">> SKIP to display {0} stacks.", lLastFilledRingBufferEntry));

					eErrorCode = (eErrorCode)message.wParam;
					lLastFilledRingBufferEntry = (int)message.lParam;
				}

				ref var AFP = ref SET.afp;
				ref var FOCUS = ref SET.focus;
				var size = SET.ImgSize();

				var nFocusNums = FOCUS.numsFocus;
				int base_order = (lLastFilledRingBufferEntry % (int)SET.rb.numsBuf) * size * (int)nFocusNums; //m_nRingBufferSize

				++m_nContiStack;

#if CSHARP_PICTURE_BOX
				for (int idx = 0; idx < m_nMaxViewer; idx++)
				{
					if (idx < nFocusNums)
					{
						ConvertByteToBitmap(SET.rb.pointerToFirst + base_order + idx * size, size, idx, string.Format("Zstack(F:{0}, {1})", FOCUS.vFocusSet[idx], m_nContiStack), AFP.cameraRoiWidth, AFP.cameraRoiHeight);
					}
				}
#else
				for (uint uid = 0; uid < m_vhwndIV.Count; uid++)
				{
					if (uid < FOCUS.numsFocus)
					{
						var pos = uid;
						if (uid == m_vhwndIV.Count - 1)
						{
							// last window -> last position
							pos = FOCUS.numsFocus - 1;
						}

						var title = string.Format("Zstack(F:{0})", FOCUS.vFocusSet[(int)pos]);
						var ptr = SET.rb.pointerToFirst + base_order + uid * size;
						ImageViewer(uid, title, m_nContiStack, SET, (byte*)ptr);
					}
					else
					{
						ImageViewer(uid);
					}
				}
#endif
			}
		}

		//----------------------------------------------------------------------------
		unsafe public void OnReceiveEdof(eErrorCode eErrorCode, int lLastFilledRingBufferEntry)
		{
			if (eErrorCode.ecNoError != eErrorCode)
			{
				ApiError("SDOAQ_ContinuousEdofCallback", eErrorCode);
			}
			else if (SET.rb.active)
			{
				NativeMessage message = new NativeMessage();
				while (PeekMessage(out message, new HandleRef(this, frmHandle), (uint)EUserMessage.EUM_RECEIVE_EDOF, (uint)EUserMessage.EUM_RECEIVE_EDOF, 0x0001 /*PM_REMOVE*/))
				{
					ShowLog(string.Format(">> SKIP to display {0} stacks.", lLastFilledRingBufferEntry));

					eErrorCode = (eErrorCode)message.wParam;
					lLastFilledRingBufferEntry = (int)message.lParam;
				}

				ref var AFP = ref SET.afp;

				int base_order = (lLastFilledRingBufferEntry % (int)SET.rb.numsBuf) * SET.EdofSize(); //m_nRingBufferSize

				++m_nContiEdof;

				try
				{
					if (frmclosing)
						return;

#if CSHARP_PICTURE_BOX
					var image_size = SET.ImgSize();

					if (SET.rb.pSizes[base_order + 0] > 0 && SET.rb.ppBuf[base_order + 0] != IntPtr.Zero)
					{
						ConvertByteToBitmap((byte*)SET.rb.ppBuf[base_order + 0], image_size, 0, string.Format("EDoF({0})", m_nContiEdof), AFP.cameraRoiWidth, AFP.cameraRoiHeight);
					}

					var pixel_size = SET.PixelSize();

					if (SET.rb.pSizes[base_order + 1] > 0 && SET.rb.ppBuf[base_order + 1] != IntPtr.Zero)
					{
						ConvertFloatPointerToBitmap((float*)SET.rb.ppBuf[base_order + 1], pixel_size, 1, string.Format("StepMAP({0})", m_nContiEdof), AFP.cameraRoiWidth, AFP.cameraRoiHeight);
					}
#if false
					if (SET.rb.pSizes[base_order + 2] > 0 && SET.rb.ppBuf[base_order + 2] != IntPtr.Zero)
					{
						ConvertFloatPointerToBitmap((float*)SET.rb.ppBuf[base_order + 2], pixel_size, 2, string.Format("QualityMAP({0})", m_nContiEdof), AFP.cameraRoiWidth, AFP.cameraRoiHeight);
					}
#endif
					if (SET.rb.pSizes[base_order + 3] > 0 && SET.rb.ppBuf[base_order + 3] != IntPtr.Zero)
					{
						ConvertFloatPointerToBitmap((float*)SET.rb.ppBuf[base_order + 3], pixel_size, 2, string.Format("HeightMAP({0})", m_nContiEdof), AFP.cameraRoiWidth, AFP.cameraRoiHeight);
					}

					if (SET.rb.pSizes[base_order + 4] > 0 && SET.rb.ppBuf[base_order + 4] != IntPtr.Zero)
					{
						PointerTo3DViewer(ref SET, (float*)SET.rb.ppBuf[base_order + 4], (byte*)SET.rb.ppBuf[base_order + 0]);
					}
#else
					ImageViewer(0, "EDoF", m_nContiEdof, SET, (byte*)SET.rb.ppBuf[base_order + 0]);
					FloatViewer(true, 1, "StepMAP", m_nContiEdof, SET, (void*)SET.rb.ppBuf[base_order + 1]);
					//FloatViewer(true, 2, "QualityMAP", m_nContiEdof, SET, (void *)SET.rb.ppBuf[base_order + 2]);
					FloatViewer(true, 2, "HeightMAP", m_nContiEdof, SET, (void*)SET.rb.ppBuf[base_order + 3]);

					if (SET.rb.pSizes[base_order + 4] > 0 && SET.rb.ppBuf[base_order + 4] != IntPtr.Zero)
					{
						PointerTo3DViewer(ref SET, (float*)SET.rb.ppBuf[base_order + 4], (byte*)SET.rb.ppBuf[base_order + 0]);
					}
#endif
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.ToString(), "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
				finally
				{
				}
			}
		}

		//----------------------------------------------------------------------------
		unsafe public void OnReceiveAF(eErrorCode eErrorCode, int lLastFilledRingBufferEntry, double dbBestFocusStep, double dbBestScore)
		{
			if (eErrorCode.ecNoError != eErrorCode)
			{
				ApiError("SDOAQ_ContinuousAfCallback", eErrorCode);
			}
			else if (SET.rb.active)
			{
				NativeMessage message = new NativeMessage();
				while (PeekMessage(out message, new HandleRef(this, frmHandle), (uint)EUserMessage.EUM_RECEIVE_AF, (uint)EUserMessage.EUM_RECEIVE_AF, 0x0001 /*PM_REMOVE*/))
				{
					ShowLog(string.Format(">> SKIP to display {0} stacks.", lLastFilledRingBufferEntry));

					eErrorCode = (eErrorCode)message.wParam;
					lLastFilledRingBufferEntry = (int)message.lParam;
				}

				ref var AFP = ref SET.afp;
				ref var FOCUS = ref SET.focus;
				var size = SET.ImgSize();

				int base_order = (lLastFilledRingBufferEntry % (int)SET.rb.numsBuf) * size;
				++m_nContiAF;

#if CSHARP_PICTURE_BOX
				ConvertByteToBitmap(SET.rb.pointerToFirst + base_order, size, 0, string.Format("AF({0})", m_nContiAF), AFP.cameraRoiWidth, AFP.cameraRoiHeight);
#else
				var ptr = SET.rb.pointerToFirst + base_order;
				ImageViewer(0, "AF", m_nContiAF, SET, (byte*)ptr);
#endif
			}
		}

		#endregion

		#region 로그 표시
		public delegate void OnLogPrintf(string log);
		public OnLogPrintf LogPrintf;

		//----------------------------------------------------------------------------
		private void ShowLog(string sMsg)
		{
			if (sMsg == string.Empty)
				return;

			if (frmclosing)
				return;

			sMsg = string.Format("[{0}] {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), sMsg);

			if (rtbLOG.InvokeRequired == true)
			{
				this.BeginInvoke(new MethodInvoker(
					delegate ()
					{
						rtbLOG.AppendText(sMsg);
						rtbLOG.ScrollToCaret();
					}));
			}
			else
			{
				rtbLOG.AppendText(sMsg);
				rtbLOG.ScrollToCaret();
			}

			LogPrintf(sMsg);
		}

		//----------------------------------------------------------------------------
		private void ApiError(string sMsg, eErrorCode eCode)
		{
			ShowLog(string.Format("[API Error] >> {0}() return error {1} (={2}).", sMsg, (int)eCode, eCode) + Environment.NewLine);
		}
		#endregion

		#region 버튼 핸들러

		//----------------------------------------------------------------------------
		private void btnInit_Click(object sender, EventArgs e)
		{
			OnSdoaqInitialize();
		}

		private void OnSdoaqInitialize()
		{
			SET.rb.active = false;

			eErrorCode rv = SDOAQ_Initialize(sDOAQ_LogCallback, sDOAQ_ErrorCallback, sDOAQ_InitDoneCallback);
		}

		//----------------------------------------------------------------------------
		private void ReadySdoaqDll()
		{
			ShowLog(">> ============================================================" + Environment.NewLine);

			int nMajorVersion = SDOAQ_GetMajorVersion();
			int nMinorVersion = SDOAQ_GetMinorVersion();
			int nPatchVersion = SDOAQ_GetPatchVersion();

			ShowLog(string.Format(">> DLL Version = {0}.{1}.{2}{3}", nMajorVersion, nMinorVersion, nPatchVersion, Environment.NewLine));
		}

		//----------------------------------------------------------------------------
		private void btnFinal_Click(object sender, EventArgs e)
		{
			OnSdoaqFinalize();
		}

		private void OnSdoaqFinalize()
		{
			eErrorCode rv = SDOAQ_Finalize();

			SET.rb.active = false;
			SET.ClearBuffer();
		}

		//----------------------------------------------------------------------------
		private void btnSelectedParamIDChanged(object sender, System.EventArgs e)
		{
			//eParameterId paraID = (eParameterId)comboParam.SelectedIndex;
			string sParaId = comboParam.SelectedValue.ToString();
			eParameterId paraID = (eParameterId)Convert.ToInt32(sParaId);

			int[] nflagAvailable = new int[1] { 0 };
			eErrorCode rv = SDOAQ_IsParameterAvailable(paraID, nflagAvailable);
			if (eErrorCode.ecNoError == rv)
			{
				bool flagAvailable = (nflagAvailable[0] != 0) ? true : false;
				if (flagAvailable)
				{
					eParameterType[] eType = new eParameterType[1];
					SDOAQ_GetParameterType(paraID, eType);

					if (eType[0] == eParameterType.ptInt)
					{
						int[] nValue = new int[1] { 0 };
						rv = SDOAQ_GetIntParameterValue(paraID, nValue);

						if (eErrorCode.ecNoError == rv)
						{
							if (paraID == eParameterId.piReflexCorrectionPattern)
							{
								this.tbParamValue.Text = string.Format("0x{0:x2}", nValue[0]);
							}
							else
							{
								this.tbParamValue.Text = nValue[0].ToString();
							}
						}
						else
						{
							ApiError(string.Format("[ParamID-{0}] has error!", paraID), rv);
						}
					}
					else if (eType[0] == eParameterType.ptDouble)
					{
						double[] dbValue = new double[1] { 0 };
						rv = SDOAQ_GetDblParameterValue(paraID, dbValue);
						if (eErrorCode.ecNoError == rv)
							this.tbParamValue.Text = dbValue[0].ToString();
						else
							ApiError(string.Format("[ParamID-{0}] has error!", paraID), rv);
					}
					else // ptString
					{
						int[] nSize = new int[1] { 1024 };
						StringBuilder sGetValue = new StringBuilder();
						rv = SDOAQ_GetStringParameterValue(paraID, sGetValue, nSize);
						if (eErrorCode.ecNoError == rv)
							this.tbParamValue.Text = sGetValue.ToString();
						else
							ApiError(string.Format("[ParamID-{0}] has error!", paraID), rv);
					}

					int[] nflagWritable = new int[1] { 0 };
					rv = SDOAQ_IsParameterWritable(paraID, nflagWritable);
					bool flagWritable = (nflagWritable[0] != 0) ? true : false;

					this.tbParamValue.Enabled = flagWritable;
					this.btnSetParam.Enabled = flagWritable;
				}
				else
				{
					ApiError(string.Format("[ParamID-{0}] is not available for this product!", paraID), rv);
				}
			}
		}

		//----------------------------------------------------------------------------
		private void btnSetParam_Click(object sender, EventArgs e)
		{
			OnSdoaqSetParameter();
		}

		//----------------------------------------------------------------------------
		private void OnSdoaqSetParameter()
		{
			if (comboParam.SelectedIndex == -1)
			{
				ApiError(string.Format("OnSdoaqSetParameter: Parameter Not Selected!"), eErrorCode.ecInvalidParameter);
				return;
			}

			// 콤보박스에서 선택한 파라미터 아이디
			eParameterId paraID = (eParameterId)comboParam.SelectedIndex;

			int[] nflagAvailable = new int[1] { 0 };
			eErrorCode rv = SDOAQ_IsParameterAvailable(paraID, nflagAvailable);
			if (eErrorCode.ecNoError == rv)
			{
				int[] nflagWritable = new int[1] { 0 };
				rv = SDOAQ_IsParameterWritable(paraID, nflagWritable);
				if (eErrorCode.ecNoError == rv)
				{
					bool flagAvailable = (nflagAvailable[0] != 0) ? true : false;
					bool flagWritable = (nflagWritable[0] != 0) ? true : false;

					if (flagAvailable && flagWritable)
					{
						eParameterType[] eType = new eParameterType[1];
						SDOAQ_GetParameterType(paraID, eType);

						string sParameters = tbParamValue.Text;

						if (eType[0] == eParameterType.ptInt)
						{
							sParameters.ToLower();

							int nValue = 0;

							if (Int32.TryParse(sParameters, out nValue) == false)
							{
								nValue = Convert.ToInt32(sParameters, 16);
								//nValue = Int32.Parse(sParameters, NumberStyles.HexNumber);
							}

							int[] nMin = new int[1] { 1 };
							int[] nMax = new int[1] { 1 };
							rv = SDOAQ_GetIntParameterRange(paraID, nMin, nMax);

							if (eErrorCode.ecNoError == rv)
							{
								if (nValue >= nMin[0] && nValue <= nMax[0])
								{
									SDOAQ_SetIntParameterValue(paraID, nValue);
								}
								else
								{
									ApiError(string.Format("SDOAQ_SetIntParameterValue[ParamID-{0}] : value is out of range[{1} ~ {2}]", paraID, nMin[0], nMax[0]), eErrorCode.ecUnknownError);
								}
							}
							else
							{
								ApiError(string.Format("SDOAQ_GetIntParameterRange[ParamID-{0}]", paraID), rv);
							}
						}
						else if (eType[0] == eParameterType.ptDouble)
						{
							double dbValue = 0;
							double[] dbMin = new double[1] { 1 };
							double[] dbMax = new double[1] { 1 };

							if (double.TryParse(sParameters, out dbValue))
							{
								rv = SDOAQ_GetDblParameterRange(paraID, dbMin, dbMax);
								if (eErrorCode.ecNoError == rv)
								{
									if (dbValue >= dbMin[0] && dbValue <= dbMax[0])
									{
										SDOAQ_SetDblParameterValue(paraID, dbValue);
									}
									else
									{
										ApiError(string.Format("SDOAQ_SetDblParameterValue[ParamID-{0}] : value is out of range[{1:N1} ~ {2:N1}]", paraID, dbMin[0], dbMax[0]), eErrorCode.ecUnknownError);
									}
								}
								else
								{
									ApiError(string.Format("SDOAQ_GetDblParameterRange[ParamID-{0}]", paraID), rv);
								}
							}
							else
							{
								ApiError(string.Format("OnSdoaqSetParameter: Data should be of double type!"), eErrorCode.ecInvalidParameter);
							}
						}
						else // ptString
						{
							rv = SDOAQ_SetStringParameterValue(paraID, sParameters);
						}
					}
					else
					{
						if (!flagAvailable)
							ApiError(string.Format("OnSdoaqSetParameter: Not Available[ParamID-{0}]", paraID), eErrorCode.ecInvalidParameter);

						if (!flagWritable)
							ApiError(string.Format("OnSdoaqSetParameter: Not Writable[ParamID-{0}]", paraID), eErrorCode.ecInvalidParameter);
					}
				}
				else
				{
					ApiError(string.Format("SDOAQ_IsParameterWritable[ParamID-{0}]", paraID), rv);
				}
			}
			else
			{
				ApiError(string.Format("SDOAQ_IsParameterAvailable[ParamID-{0}]", paraID), rv);
			}
		}

		//----------------------------------------------------------------------------
		private void btnSetROI_Click(object sender, EventArgs e)
		{
			OnSdoaqSetROI();
		}

		private void OnSdoaqSetROI()
		{
			string sParameters = tbROI.Text;
			string[] saParaData = sParameters.Split(',');
			int[] nData = new int[4] { 0, 0, 0, 0 };

			for (int idx = 0; idx < saParaData.Length; idx++)
			{
				int num = 0;
				if (int.TryParse(saParaData[idx].Trim(), out num))
					nData[idx] = num;
			}

			AcquisitionFixedParameters AFP;
			AFP.cameraRoiTop = nData[0];
			AFP.cameraRoiLeft = nData[1];
			AFP.cameraRoiWidth = (nData[2] / 4) * 4;
			AFP.cameraRoiHeight = nData[3];
			AFP.cameraBinning = 1;

			int[] nMin = new int[1] { 1 };
			int[] nMax = new int[1] { 1 };
			var rv = SDOAQ_GetIntParameterRange(eParameterId.piCameraFullFrameSizeX, nMin, nMax);
			if (eErrorCode.ecNoError == rv)
			{
				if (AFP.cameraRoiLeft < 0 || AFP.cameraRoiLeft > nMax[0])
				{
					ApiError(string.Format("Set cameraRoiLeft : value is out of range[{0} ~ {1}]", nMin[0], nMax[0]), eErrorCode.ecUnknownError);
					return;
				}
				if (AFP.cameraRoiWidth < 1 || AFP.cameraRoiWidth > nMax[0])
				{
					ApiError(string.Format("Set cameraRoiWidth : value is out of range[{0} ~ {1}]", nMin[0], nMax[0]), eErrorCode.ecUnknownError);
					return;
				}
			}

			rv = SDOAQ_GetIntParameterRange(eParameterId.piCameraFullFrameSizeY, nMin, nMax);
			if (eErrorCode.ecNoError == rv)
			{
				if (AFP.cameraRoiTop < 0 || AFP.cameraRoiTop > nMax[0])
				{
					ApiError(string.Format("Set cameraRoiTop : value is out of range[{0} ~ {1}]", nMin[0], nMax[0]), eErrorCode.ecUnknownError);
					return;
				}
				if (AFP.cameraRoiHeight < 1 || AFP.cameraRoiHeight > nMax[0])
				{
					ApiError(string.Format("Set cameraRoiHeight : value is out of range[{0} ~ {1}]", nMin[0], nMax[0]), eErrorCode.ecUnknownError);
					return;
				}
			}

			if (!SET.rb.active)
			{
				SET.afp = AFP;
			}
		}

		//----------------------------------------------------------------------------
		private void btnSetAFROI_Click(object sender, EventArgs e)
		{
			OnSdoaqSetAFROI();
		}

		private void OnSdoaqSetAFROI()
		{
			string sParameters = tbAFROI.Text;
			string[] saParaData = sParameters.Split(',');
			int[] nData = new int[4] { 0, 0, 0, 0 };

			for (int idx = 0; idx < saParaData.Length; idx++)
			{
				int num = 0;
				if (int.TryParse(saParaData[idx].Trim(), out num))
					nData[idx] = num;
			}

			int[] nMin = new int[1] { 1 };
			int[] nMax = new int[1] { 1 };
			var rv = SDOAQ_GetIntParameterRange(eParameterId.piCameraFullFrameSizeX, nMin, nMax);
			if (eErrorCode.ecNoError == rv)
			{
				if (nData[0] < 0 || nData[0] > nMax[0])
				{
					ApiError(string.Format("Set cameraRoiLeft : value is out of range[{0} ~ {1}]", nMin[0], nMax[0]), eErrorCode.ecUnknownError);
					return;
				}
				if (nData[2] < 1 || nData[2] > nMax[0])
				{
					ApiError(string.Format("Set cameraRoiWidth : value is out of range[{0} ~ {1}]", nMin[0], nMax[0]), eErrorCode.ecUnknownError);
					return;
				}
			}

			rv = SDOAQ_GetIntParameterRange(eParameterId.piCameraFullFrameSizeY, nMin, nMax);
			if (eErrorCode.ecNoError == rv)
			{
				if (nData[1] < 0 || nData[1] > nMax[0])
				{
					ApiError(string.Format("Set cameraRoiTop : value is out of range[{0} ~ {1}]", nMin[0], nMax[0]), eErrorCode.ecUnknownError);
					return;
				}
				if (nData[3] < 1 || nData[3] > nMax[0])
				{
					ApiError(string.Format("Set cameraRoiHeight : value is out of range[{0} ~ {1}]", nMin[0], nMax[0]), eErrorCode.ecUnknownError);
					return;
				}
			}

			rv = SDOAQ_SetIntParameterValue(eParameterId.piFocusLeftTop, ((nData[0] & 0x0000FFFF) << 16) | (nData[1] & 0x0000FFFF) << 0);
			if (eErrorCode.ecNoError != rv)
			{
				ApiError("set eParameterId.piFocusLeftTop", rv);
			}
			rv = SDOAQ_SetIntParameterValue(eParameterId.piFocusRightBottom, (((nData[0] + nData[2]) & 0x0000FFFF) << 16) | ((nData[1] + nData[3]) & 0x0000FFFF) << 0);
			if (eErrorCode.ecNoError != rv)
			{
				ApiError("set eParameterId.piFocusRightBottom", rv);
			}
		}

		//----------------------------------------------------------------------------
		private void btnSetRingBSize_Click(object sender, EventArgs e)
		{
			OnSdoaqSetRingBufSize();
		}

		private void OnSdoaqSetRingBufSize()
		{
			string sSize = tbRingBSize.Text;
			int nValue = 0;

			if (int.TryParse(sSize, out nValue))
				m_nRingBufferSize = Math.Max(1, nValue);
		}

		//----------------------------------------------------------------------------
		private void btnSetFocus_Click(object sender, EventArgs e)
		{
			OnSdoaqSetFocusSet();
		}

		//----------------------------------------------------------------------------
		private void OnSdoaqSetFocusSet()
		{
			string sFocusSet = tbFocusSet.Text;

			m_vFocusSet.Clear();

			if (sFocusSet.Contains("-"))
			{
				string[] saFocusData = sFocusSet.Split('-');
				if (3 <= saFocusData.Length)
				{
					int[] nParam = new int[3] { 0, 0, 0 };
					for (int idx = 0; idx < 3; idx++)
					{
						int nValue = 0;
						if (int.TryParse(saFocusData[idx].Trim(), out nValue))
							nParam[idx] = nValue;
					}

					for (int nFocus = nParam[0]; nFocus <= nParam[1]; nFocus += nParam[2])
					{
						m_vFocusSet.Add(nFocus);
					}
				}
			}
			else
			{
				string[] numbers = Regex.Split(sFocusSet, @"\D+");
				foreach (string value in numbers)
				{
					if (!string.IsNullOrEmpty(value))
					{
						int nValue = 0;
						if (int.TryParse(value.Trim(), out nValue) && nValue <= (int)eFocus.MAX_FOCUS_STEP)
							m_vFocusSet.Add(nValue);
					}
				}
			}

			if (m_vFocusSet.Count == 0)
			{
				m_vFocusSet.Add((int)eFocus.DFLT_FOCUS_STEP);
			}
		}

		//----------------------------------------------------------------------------
		private void btnSetSnapFocus_Click(object sender, EventArgs e)
		{
			OnSdoaqSetSnapFocusSet();
		}

		private void OnSdoaqSetSnapFocusSet()
		{
			string sFocusSet = tbSnapFocusSet.Text;

			m_vSnapFocusSet.Clear();

			if (sFocusSet.Contains("-"))
			{
				string[] saFocusData = sFocusSet.Split('-');
				if (3 <= saFocusData.Length)
				{
					int[] nParam = new int[3] { 0, 0, 0 };
					for (int idx = 0; idx < 3; idx++)
					{
						int nValue = 0;
						if (int.TryParse(saFocusData[idx].Trim(), out nValue))
							nParam[idx] = nValue;
					}

					for (int nFocus = nParam[0]; nFocus <= nParam[1]; nFocus += nParam[2])
					{
						m_vSnapFocusSet.Add(nFocus);
					}
				}
			}
			else
			{
				string[] numbers = Regex.Split(sFocusSet, @"\D+");
				foreach (string value in numbers)
				{
					if (!string.IsNullOrEmpty(value))
					{
						int nValue = 0;
						if (int.TryParse(value.Trim(), out nValue) && nValue <= (int)eFocus.MAX_FOCUS_STEP)
							m_vSnapFocusSet.Add(nValue);
					}
				}
			}

			if (m_vSnapFocusSet.Count == 0)
			{
				m_vSnapFocusSet.Add((int)eFocus.DFLT_FOCUS_STEP);
			}
		}

		//----------------------------------------------------------------------------
		private void btnEdofRatio_Click(object sender, EventArgs e)
		{
			OnSdoaqSetEdofResize();
		}

		private void OnSdoaqSetEdofResize()
		{
			string sEdofResize = tbEdofRatio.Text;

			double resize_ratio = 0.25;
			if (double.TryParse(sEdofResize, out resize_ratio))
			{
				double[] dbMin = new double[1];
				double[] dbMax = new double[1];
				var rv = SDOAQ_GetDblParameterRange(eParameterId.pi_edof_calc_resize_ratio, dbMin, dbMax);
				if (eErrorCode.ecNoError == rv)
				{
					if (resize_ratio >= dbMin[0] && resize_ratio <= dbMax[0])
					{
						//SET.efp.resize_ratio = resize_ratio;
						SDOAQ_SetDblParameterValue(eParameterId.pi_edof_calc_resize_ratio, resize_ratio);
					}
					else
					{
						ApiError(string.Format("Set EDoF resize ratio : value is out of range[{0:N2} ~ {1:N2}]", dbMin, dbMax), eErrorCode.ecUnknownError);
					}
				}
				else
				{
					ApiError("SDOAQ_GetDblParameterRange [pi_edof_calc_resize_ratio]", rv);
				}
			}
		}
		#endregion

		#region ACQ STACK

		//----------------------------------------------------------------------------
		private void btnAcqStack_Click(object sender, EventArgs e)
		{
			OnSdoaqAcqStack();
		}

		//----------------------------------------------------------------------------
		unsafe private void OnSdoaqAcqStack()
		{
			if (SET.rb.active)
			{
				return;
			}

			ClearPictureBoxes();

			AcquisitionFixedParameters[] AFP = new AcquisitionFixedParameters[1] { SET.afp };
			cFocus FOCUS = SET.focus;

			FOCUS.numsFocus = (uint)m_vFocusSet.Count;
			FOCUS.vFocusSet = m_vFocusSet.ToList();

			int[] pPositions = new int[FOCUS.numsFocus];
			byte*[] ppFocusImages = new byte*[FOCUS.numsFocus];
			ulong[] pFocusImageBufferSizes = new ulong[FOCUS.numsFocus];

			var size = SET.ImgSize();
			byte[] bData = new byte[size * FOCUS.numsFocus];
			//bData = Enumerable.Repeat((byte)255, size * (int)FOCUS.numsFocus).ToArray();

			try
			{
				// CLR (Common Language Runtime) can change the memory address of bData (byte array)
				// "fixed" keyword tells CLR not to change the memory address of bData..
				// Without "fixed", exception can happen due to the change of memory address..

				fixed (byte* pointerToFirst = bData)
				{
					for (int pos = 0; pos < FOCUS.numsFocus; pos++)
					{
						pPositions[pos] = FOCUS.vFocusSet[pos];
						ppFocusImages[pos] = (byte*)(pointerToFirst + size * pos);
						pFocusImageBufferSizes[pos] = (ulong)size;
					}

					eErrorCode rv = SDOAQ_SingleShotFocusStack(
					AFP,
					pPositions, (int)FOCUS.numsFocus,
					ppFocusImages, pFocusImageBufferSizes
					);

					if (eErrorCode.ecNoError == rv)
					{
						++m_nContiStack;

#if CSHARP_PICTURE_BOX
#if false
						for (int idx = 0; idx < FOCUS.numsFocus; idx++)
						{							
							ConvertByteToBitmap(ppFocusImages[idx], size, idx, string.Format("Zstack(F:{0}, {1})", pPositions[idx], m_nContiStack), AFP[0].cameraRoiWidth, AFP[0].cameraRoiHeight);
						}
#else
						for (int idx = 0; idx < m_nMaxViewer; idx++)
						{
							if (idx < FOCUS.numsFocus)
								ConvertByteToBitmap(ppFocusImages[idx], size, idx, string.Format("Zstack(F:{0}, {1})", pPositions[idx], m_nContiStack), AFP[0].cameraRoiWidth, AFP[0].cameraRoiHeight);
						}
#endif
#else
						for (uint uid = 0; uid < m_vhwndIV.Count; uid++)
						{
							if (uid < FOCUS.numsFocus)
							{
								var pos = uid;
								if (uid == m_vhwndIV.Count - 1)
								{
									// last window -> last position
									pos = FOCUS.numsFocus - 1;
								}

								var title = string.Format("Zstack(%0)", pPositions[pos]);
								ImageViewer(uid, title, m_nContiStack, SET, ppFocusImages[pos]);
							}
							else
							{
								ImageViewer(uid);
							}
						}
#endif
					}
					else
					{
						ApiError("SDOAQ_SingleShotFocusStack", rv);
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString(), "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		#region Image Viewer
		//----------------------------------------------------------------------------
		unsafe private void ImageViewer(uint uViewer, string title = null, int title_no = 0, SDOAQ_TEST_SET SET = null, byte* data = null)
		{
			ImageViewer(uViewer, title, title_no, SET.afp.cameraRoiWidth, SET.afp.cameraRoiHeight, SET.COLORBYTES, data);
		}

		unsafe private void ImageViewer(uint uViewer, string title, int title_no, int width, int height, int colorbytes, void* data)
		{
			string full_title = string.Empty;
			if (title != null)
			{
				full_title = string.Format("{0} {1}", title, title_no);
			}

			uint size = (data != null) ? (uint)(width * height * colorbytes) : (uint)0;
			if (WSIORV.WSIORV_SUCCESS > WSUT_IV_AttachRawImgData_V2(m_vhwndIV[(int)uViewer].vHwnd, (uint)width, (uint)height, (uint)(width * colorbytes), (uint)colorbytes, data, size, full_title))
			{
				Utils.print_wsio_last_error();
			}
		}
		#endregion

		//----------------------------------------------------------------------------
		unsafe private void ConvertByteToBitmap(byte* data, int size, int idx, string title, int width, int height)
		{
			try
			{
				if (frmclosing)
					return;

				Stopwatch watch = Stopwatch.StartNew();

				byte[] byteArray;

				if (!SET.IsColor())
				{
					size = size * 3;
					byteArray = new byte[size];

					Parallel.For(0, size / 3, index
					   =>
					{
						byte gray = (byte)data[index];

						byteArray[index * 3] = gray;
						byteArray[index * 3 + 1] = gray;
						byteArray[index * 3 + 2] = gray;
					}
					);
				}
				else
				{
					byteArray = new byte[size];
					Marshal.Copy((IntPtr)data, byteArray, 0, size);
				}

				Bitmap bmp = new Bitmap(width, height);
				BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
				{
					// Get the address of the first line.
					IntPtr ptr = bmpData.Scan0;
					Marshal.Copy(byteArray, 0, ptr, size);
				}
				bmp.UnlockBits(bmpData);

				watch.Stop();
				//ShowLog(string.Format("{0}: Elapsed {1} ms{2}", MethodBase.GetCurrentMethod().Name, watch.ElapsedMilliseconds, Environment.NewLine));

				var viewerIdx = idx % m_nMaxViewer;
				ShowBitmapToViewer(viewerIdx, bmp, title);

				bmp.Dispose();
				GC.Collect();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString(), "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
			}
		}

		//----------------------------------------------------------------------------
		unsafe private void ConvertByteArrayToBitmap(byte[] data, int size, int idx, string title, int width, int height)
		{
			try
			{
				if (frmclosing)
					return;

				Stopwatch watch = Stopwatch.StartNew();

				Bitmap bmp = new Bitmap(width, height);

				if (!SET.IsColor())
				{
					size = size * 3;
					byte[] byteArray = new byte[size];

					Parallel.For(0, size / 3, index
					   =>
					{
						byte gray = (byte)data[index];

						byteArray[index * 3] = gray;
						byteArray[index * 3 + 1] = gray;
						byteArray[index * 3 + 2] = gray;
					}
					);

					BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
					{
						// Get the address of the first line.
						IntPtr ptr = bmpData.Scan0;
						Marshal.Copy(byteArray, 0, ptr, size);
					}
					bmp.UnlockBits(bmpData);
				}
				else
				{
					BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
					{
						// Get the address of the first line.
						IntPtr ptr = bmpData.Scan0;
						Marshal.Copy(data, 0, ptr, size);
					}
					bmp.UnlockBits(bmpData);
				}

				watch.Stop();
				//ShowLog(string.Format("{0}: Elapsed {1} ms{2}", MethodBase.GetCurrentMethod().Name, watch.ElapsedMilliseconds, Environment.NewLine));

				var viewerIdx = idx % m_nMaxViewer;
				ShowBitmapToViewer(viewerIdx, bmp, title);

				bmp.Dispose();
				GC.Collect();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString(), "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
			}
		}

		unsafe private void ConvertFloatArrayToBitmap(float[] data, int size, int idx, string title, int width, int height)
		{
			try
			{
				if (frmclosing)
					return;

				Stopwatch watch = Stopwatch.StartNew();

				byte[] byteArray = new byte[width * height * 3];
				float arrMax = data.Max();
				float arrMin = data.Min();

				Parallel.For(0, size, index
				   =>
				{
					float normX = (data[index] - arrMin) / (arrMax - arrMin) * 255;
					byte red = (byte)Math.Min(normX, 255);
					byte green = red; // (byte)Math.Min((int)((normX - red) * 256), 255); // red;
					byte blue = red; // (byte)Math.Min((int)(((normX - red) * 256 - green) * 256), 255); // red;

					byteArray[index * 3] = red;
					byteArray[index * 3 + 1] = green;
					byteArray[index * 3 + 2] = blue;
				}
				);

				//var byteArray = data.Select(f => (byte)(Math.Round(f))).ToArray();

				Bitmap bmp = new Bitmap(width, height);
				BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
				{
					// Get the address of the first line.
					IntPtr ptr = bmpData.Scan0;
					Marshal.Copy(byteArray, 0, ptr, byteArray.Length);
				}
				bmp.UnlockBits(bmpData);

				watch.Stop();
				//ShowLog(string.Format("{0}: Elapsed {1} ms{2}", MethodBase.GetCurrentMethod().Name, watch.ElapsedMilliseconds, Environment.NewLine));

				var viewerIdx = idx % m_nMaxViewer;
				ShowBitmapToViewer(viewerIdx, bmp, title);

				bmp.Dispose();
				GC.Collect();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString(), "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
			}
		}

		unsafe private void ConvertFloatPointerToBitmap(float* data, int size, int idx, string title, int width, int height)
		{
			try
			{
				if (frmclosing)
					return;

				Stopwatch watch = Stopwatch.StartNew();

				byte[] byteArray = new byte[width * height * 3];

				float arrMax = 0;
				float arrMin = float.MaxValue;
				for (int cnt = 0; cnt < size; cnt++)
				{
					if (data[cnt] < arrMin)
						arrMin = data[cnt];
					if (data[cnt] > arrMax)
						arrMax = data[cnt];
				}
				Parallel.For(0, size, index
					=>
				{
					float normX = (data[index] - arrMin) / (arrMax - arrMin) * 255;
					byte red = (byte)Math.Min(normX, 255);
					byte green = red; // (byte)Math.Min((int)((normX - red) * 256), 255); // red;
					byte blue = red; // (byte)Math.Min((int)(((normX - red) * 256 - green) * 256), 255); // red;

					byteArray[index * 3] = red;
					byteArray[index * 3 + 1] = green;
					byteArray[index * 3 + 2] = blue;
				}

				);

				//var byteArray = data.Select(f => (byte)(Math.Round(f))).ToArray();

				watch.Stop();
				//ShowLog(string.Format("{0}: Elapsed {1} ms{2}", MethodBase.GetCurrentMethod().Name, watch.ElapsedMilliseconds, Environment.NewLine));

				Bitmap bmp = new Bitmap(width, height);
				BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
				{
					// Get the address of the first line.
					IntPtr ptr = bmpData.Scan0;
					Marshal.Copy(byteArray, 0, ptr, byteArray.Length);
				}
				bmp.UnlockBits(bmpData);

				var viewerIdx = idx % m_nMaxViewer;
				ShowBitmapToViewer(viewerIdx, bmp, title);

				bmp.Dispose();
				GC.Collect();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString(), "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
			}
		}

		//----------------------------------------------------------------------------
		private void ShowBitmapToViewer(int viewerIdx, Bitmap bmp, string title)
		{
			var destPictBoxName = string.Format("pbViewer{0}", viewerIdx + 1);
			var destLabel = string.Format("lbViewer{0}", viewerIdx + 1);

			Control[] ctn = this.Controls.Find(destPictBoxName, true);
			PictureBox cPictBox = (PictureBox)ctn[0];
			ctn = this.Controls.Find(destLabel, true);
			Label cLabel = (Label)ctn[0];
			if (cPictBox != null)
			{
				if (cPictBox.InvokeRequired == true)
				{
					this.Invoke(new MethodInvoker(
						delegate ()
						{
							try
							{
								cPictBox.Image = (Image)bmp.Clone();
								//cPictBox.SizeMode = PictureBoxSizeMode.Zoom;

								if (cLabel != null)
									cLabel.Text = string.Format("Viewer #{0} : [{1}]", viewerIdx + 1, title);
							}
							catch (Exception ex)
							{
								ShowLog(ex.ToString() + " " + title);
								//MessageBox.Show(ex.ToString(), "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
							}
						}));
				}
				else
				{
					cPictBox.Image = (Image)bmp.Clone();
					//cPictBox.SizeMode = PictureBoxSizeMode.Zoom;

					if (cLabel != null)
						cLabel.Text = string.Format("Viewer #{0} : [{1}]", viewerIdx + 1, title);
				}
			}
		}

		//----------------------------------------------------------------------------
		unsafe private void PointerTo3DViewer(ref SDOAQ_TEST_SET SET, float* p_pointcloud, byte* p_edof)
		{
			if (p_pointcloud != null && p_edof != null)
			{
				var pc_size = SET.PixelSize() * SDOAQ_TEST_SET.XYZNUMS;
				float[] pcArray = new float[pc_size];
				var img_size = SET.ImgSize();
				byte[] imgArray = new byte[img_size];

				Marshal.Copy((IntPtr)p_pointcloud, pcArray, 0, pcArray.Length);
				Marshal.Copy((IntPtr)p_edof, imgArray, 0, imgArray.Length);

				ArrayTo3DViewer(SET, pcArray, imgArray);
				//GC.Collect();
			}
			else
			{
				GL.WSGL_Display_BG(m_hwnd3D);
			}
		}

		//----------------------------------------------------------------------------
		unsafe private void ArrayTo3DViewer(SDOAQ_TEST_SET SET, float[] p_pointcloud, byte[] p_edof)
		{
			if (p_pointcloud != null && p_edof != null)
			{
				GL.tPara_Display25D[] Para = new GL.tPara_Display25D[1];
				Para[0].width = (uint)SET.PixelWidth();
				Para[0].height = (uint)SET.PixelHeight();
				Para[0].z_offset1 = 0;
				Para[0].z_offset2 = 0;
				Para[0].z_slices = (uint)m_vFocusSet.Count;
				Para[0].scx1 = 0;
				Para[0].scx2 = 0;
				Para[0].scy1 = 0;
				Para[0].scy2 = 0;
				Para[0].scz1 = 0;
				Para[0].scz2 = 0;

				var display_mode = (GL.EDisplayMode.EDM_BGR_BYTE | GL.EDisplayMode.EDM_DIMENSION_CALXY_25D | GL.EDisplayMode.EDM_NDC_XY_ONLY);

				if (pbViewer4.InvokeRequired)
				{
					this.Invoke(new MethodInvoker(
					delegate ()
					{
						GL.WSGL_Display_25D(m_hwnd3D, GL.GL_MG_ONSTAGE, "main", p_pointcloud, (uint)SET.PixelSize() * SDOAQ_TEST_SET.XYZNUMS, p_edof, (uint)SET.ImgSize(),
							(int)display_mode, 1.0f, Para);
					}));
				}
				else
				{
					GL.WSGL_Display_25D(m_hwnd3D, GL.GL_MG_ONSTAGE, "main", p_pointcloud, (uint)SET.PixelSize() * SDOAQ_TEST_SET.XYZNUMS, p_edof, (uint)SET.ImgSize(),
						(int)display_mode, 1.0f, Para);
				}
			}
			else
			{
				GL.WSGL_Display_BG(m_hwnd3D);
			}
		}

		//----------------------------------------------------------------------------
		private void btnContiStack_Click(object sender, EventArgs e)
		{
			OnSdoaqContiStack();
		}

		//----------------------------------------------------------------------------
		unsafe private void OnSdoaqContiStack()
		{
			if (SET.rb.active)
			{
				return;
			}

			ClearPictureBoxes();

			AcquisitionFixedParameters[] AFP = new AcquisitionFixedParameters[1] { SET.afp };
			ref cFocus FOCUS = ref SET.focus;

			FOCUS.numsFocus = (uint)m_vFocusSet.Count;
			FOCUS.vFocusSet = m_vFocusSet.ToList();

			int[] pPositions = new int[FOCUS.numsFocus];
			for (int pos = 0; pos < FOCUS.numsFocus; pos++)
			{
				pPositions[pos] = FOCUS.vFocusSet[pos];
			}

			SET.rb.numsBuf = FOCUS.numsFocus * (uint)m_nRingBufferSize;
			SET.rb.pSizes = new ulong[SET.rb.numsBuf];

			var size = SET.ImgSize();
			var bufSize = size * (int)SET.rb.numsBuf;
			byte*[] ppFocusImages = new byte*[SET.rb.numsBuf];
			ulong[] pFocusImageBufferSizes = new ulong[SET.rb.numsBuf];

			SET.ClearBuffer();
			SET.AllocBuffer(bufSize);

			eErrorCode rv = eErrorCode.ecNoError;

			try
			{
				/// memory allocation in unmanaged area
				/// fixed is not needed..

				byte* pointerToFirst = (byte*)SET.bufHandlePtr;
				//fixed (byte* pointerToFirst = bData)
				{
					for (int pos = 0; pos < SET.rb.numsBuf; pos++)
					{
						ppFocusImages[pos] = (byte*)(pointerToFirst + size * pos);
						pFocusImageBufferSizes[pos] = (ulong)size;
					}
					SET.rb.pointerToFirst = pointerToFirst;

					rv = SDOAQ_PlayFocusStack(
					AFP,
					sDOAQ_PlayCallback,
					pPositions, (int)FOCUS.numsFocus,
					m_nRingBufferSize, ppFocusImages, pFocusImageBufferSizes);

					if (eErrorCode.ecNoError == rv)
					{
						SET.rb.active = true;
					}
					else
					{
						ApiError("SDOAQ_PlayFocusStack", rv);
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString(), "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

		}

		//----------------------------------------------------------------------------
		private void OnSdoaqStopStack(object sender, EventArgs e)
		{
			OnSdoaqStopStack();
		}

		void OnSdoaqStopStack()
		{
			if (SET.rb.active)
			{
				SET.rb.active = false;

				/// Note on Problem..
				/// If SDOAQ_StopFocusStack() is called directly (from UI Thread),
				///     timeout error occurs randomly and application crashes.
				/// So, the function is moved to ThreadPool (non UI Thread).
				/// The issue is cleared..
				ThreadPool.QueueUserWorkItem(StopStackThread, new object { });
			}

			/// Sometimes, System.AccessViolationException occurs.
			/// ClearBuffer() is called before allocation and on form closing.
			//SET.ClearBuffer();
		}

		static void StopStackThread(object data)
		{
			SDOAQ_StopFocusStack();
		}

		#endregion

		#region ACQ EDOF
		//----------------------------------------------------------------------------
		private void btnAcqEdof_Click(object sender, EventArgs e)
		{
			OnSdoaqAcqEdof();
		}

		unsafe private void OnSdoaqAcqEdof()
		{
			if (SET.rb.active)
			{
				return;
			}

			ClearPictureBoxes();

			AcquisitionFixedParameters[] AFP = new AcquisitionFixedParameters[1] { SET.afp };
			ref var FOCUS = ref SET.focus;

			FOCUS.numsFocus = (uint)m_vFocusSet.Count;
			FOCUS.vFocusSet = m_vFocusSet.ToList();

			int[] pPositions = new int[FOCUS.numsFocus];
			for (int pos = 0; pos < FOCUS.numsFocus; pos++)
			{
				pPositions[pos] = FOCUS.vFocusSet[pos];
			}

			float[] pStepMapImageBuffer = new float[SET.PixelSize()];   // 각 pixel 별 focus step number (0~319)
			byte[] pEdofImageBuffer = new byte[SET.ImgSize()];          // all-in-focus image
			float[] pQualityMapBuffer = new float[SET.PixelSize()];     // 높이맵의 각 픽셀높이에 대한 점수(얼마나 신뢰할만한지)
			float[] pHeightMapBuffer = new float[SET.PixelSize()];      // 각 pixel 별 높이 정보
			float[] pPointCloudBuffer = new float[SET.PixelSize() * SDOAQ_TEST_SET.XYZNUMS]; // 각 pixel의 (x,y,z) 좌표데이타

			uint stepMapBufferSize = (uint)SET.DataSize();
			uint edofImageBufferSize = (uint)SET.ImgSize();
			uint qualityMapBufferSize = (uint)SET.DataSize();
			uint heightMapBufferSize = (uint)SET.DataSize();
			uint pointCloudBufferSize = (uint)SET.DataSize() * SDOAQ_TEST_SET.XYZNUMS;

			if (!checkStepMap.Checked)
			{
				stepMapBufferSize = 0;
			}
			if (!checkEdof.Checked)
			{
				edofImageBufferSize = 0;
			}
			if (!checkQualityMap.Checked)
			{
				qualityMapBufferSize = 0;
			}
			if (!checkHeightMap.Checked)
			{
				heightMapBufferSize = 0;
			}
			if (!checkPointCloud.Checked)
			{
				pointCloudBufferSize = 0;
			}

			eErrorCode rv = SDOAQ_SingleShotEdof(
				AFP,
				pPositions, (int)FOCUS.numsFocus,
				pStepMapImageBuffer, stepMapBufferSize,
				pEdofImageBuffer, edofImageBufferSize,
				pQualityMapBuffer, qualityMapBufferSize,
				pHeightMapBuffer, heightMapBufferSize,
				pPointCloudBuffer, pointCloudBufferSize
			);

			if (eErrorCode.ecNoError == rv)
			{
				++m_nContiEdof;

#if CSHARP_PICTURE_BOX
                var size = SET.ImgSize();

				if (edofImageBufferSize > 0)
					ConvertByteArrayToBitmap(pEdofImageBuffer, size, 0, string.Format("EDoF({0})", m_nContiEdof), AFP[0].cameraRoiWidth, AFP[0].cameraRoiHeight);

				size = SET.PixelSize();
				if (stepMapBufferSize > 0)
					ConvertFloatArrayToBitmap(pStepMapImageBuffer, size, 1, string.Format("StepMAP({0})", m_nContiEdof), AFP[0].cameraRoiWidth, AFP[0].cameraRoiHeight);
#if false
				if (qualityMapBufferSize > 0)
					ConvertFloatArrayToBitmap(pQualityMapBuffer, size, 2, string.Format("QualityMAP({0})", m_nContiEdof), AFP[0].cameraRoiWidth, AFP[0].cameraRoiHeight);
#endif
				if (heightMapBufferSize > 0)
					ConvertFloatArrayToBitmap(pHeightMapBuffer, size, 2, string.Format("HeightMAP({0})", m_nContiEdof), AFP[0].cameraRoiWidth, AFP[0].cameraRoiHeight);

				if (pointCloudBufferSize > 0 && edofImageBufferSize > 0)
				{
					ArrayTo3DViewer(SET, pPointCloudBuffer, pEdofImageBuffer);
				}
#else
				if (pEdofImageBuffer != null && edofImageBufferSize > 0)
				{
					fixed (byte* ptr = pEdofImageBuffer)
					{
						ImageViewer(0, "EDoF", m_nContiEdof, SET, ptr);
					}
				}
				else
				{
					ImageViewer(0, "EDoF", m_nContiEdof);
				}

				fixed (void* ptr = pStepMapImageBuffer)
				{
					FloatViewer(pStepMapImageBuffer != null && stepMapBufferSize > 0, 1, "StepMAP", m_nContiEdof, SET, ptr);
				}
				//fixed (void* ptr = pQualityMapBuffer)
				//{
				//	FloatViewer(pQualityMapBuffer != null && qualityMapBufferSize > 0, 2, "QualityMAP", m_nContiEdof, SET, ptr);
				//}
				fixed (void* ptr = pHeightMapBuffer)
				{
					FloatViewer(pHeightMapBuffer != null && heightMapBufferSize > 0, 2, "HeightMAP", m_nContiEdof, SET, ptr);
				}

				if (pointCloudBufferSize > 0 && edofImageBufferSize > 0)
				{
					ArrayTo3DViewer(SET, pPointCloudBuffer, pEdofImageBuffer);
				}
#endif
			}
			else
			{
				ApiError("SDOAQ_SingleShotEdof", rv);
			}
		}

		#region Float Viewer
		//----------------------------------------------------------------------------
		unsafe private void FloatViewer(bool onoff, uint uViewer, string title, int title_no, SDOAQ_TEST_SET SET, void* data)
		{
			if (onoff && data != null)
			{
				var pSeek = (float*)data;
				var low = *pSeek;
				var high = *pSeek;

				uint pixels = (uint)SET.PixelSize();
				for (uint uSampling = 0; uSampling < pixels; uSampling += 100)
				{
					float v = *(pSeek + uSampling);
					if (v < low)
					{
						low = v;
					}
					if (high < v)
					{
						high = v;
					}
				}
				float inc = ((float)256.0 / (high - low));

				var pImg = new byte[SET.PixelSize()];
				var pDest = pImg;
				var pSrc = (float*)data;
				for (uint loop = 0; loop < pixels; loop++)
				{
					pImg[loop] = (byte)((*pSrc++ - low) * inc);
				}

				fixed (void* ptr = pImg)
				{
					ImageViewer(uViewer, title, title_no, SET.afp.cameraRoiWidth, SET.afp.cameraRoiHeight, SET.MONOBYTES, ptr);
				}
			}
			else
			{
				ImageViewer(uViewer, title, title_no);
			}
		}
		#endregion

		//----------------------------------------------------------------------------
		private void btnContiEdof_Click(object sender, EventArgs e)
		{
			OnSdoaqContiEdof();
		}

		unsafe private void OnSdoaqContiEdof()
		{
			if (SET.rb.active)
			{
				return;
			}

			ClearPictureBoxes();

			AcquisitionFixedParameters[] AFP = new AcquisitionFixedParameters[1] { SET.afp };
			ref var FOCUS = ref SET.focus;

			FOCUS.numsFocus = (uint)m_vFocusSet.Count;
			FOCUS.vFocusSet = m_vFocusSet.ToList();

			int[] pPositions = new int[FOCUS.numsFocus];
			for (int pos = 0; pos < FOCUS.numsFocus; pos++)
			{
				pPositions[pos] = FOCUS.vFocusSet[pos];
			}

			ulong stepMapBufferSize = (ulong)SET.DataSize();
			ulong edofImageBufferSize = (ulong)SET.ImgSize();
			ulong qualityMapBufferSize = (ulong)SET.DataSize();
			ulong heightMapBufferSize = (ulong)SET.DataSize();
			ulong pointCloudBufferSize = (ulong)SET.DataSize() * SDOAQ_TEST_SET.XYZNUMS;

			if (!checkStepMap.Checked)
			{
				stepMapBufferSize = 0;
			}
			if (!checkEdof.Checked)
			{
				edofImageBufferSize = 0;
			}
			if (!checkQualityMap.Checked)
			{
				qualityMapBufferSize = 0;
			}
			if (!checkHeightMap.Checked)
			{
				heightMapBufferSize = 0;
			}
			if (!checkPointCloud.Checked)
			{
				pointCloudBufferSize = 0;
			}

			SET.ClearBuffer();

			SET.rb.numsBuf = Convert.ToUInt32(SET.EdofSize() * m_nRingBufferSize);
			SET.rb.pSizes = new ulong[SET.rb.numsBuf];
			SET.rb.ppBuf = new IntPtr[SET.rb.numsBuf];

			for (uint uidx = 0; uidx + SET.EdofSize() - 1 < SET.rb.numsBuf;)
			{
				SET.rb.ppBuf[uidx] = (edofImageBufferSize > 0) ? Marshal.AllocHGlobal((int)edofImageBufferSize) : IntPtr.Zero;
				SET.rb.pSizes[uidx] = edofImageBufferSize;
				uidx++; // EDOF: all-in-focus image

				SET.rb.ppBuf[uidx] = (stepMapBufferSize > 0) ? Marshal.AllocHGlobal((int)stepMapBufferSize) : IntPtr.Zero;
				SET.rb.pSizes[uidx] = stepMapBufferSize;
				uidx++; // StepMap: 각 pixel 별 focus step number (0~319)

				SET.rb.ppBuf[uidx] = (qualityMapBufferSize > 0) ? Marshal.AllocHGlobal((int)qualityMapBufferSize) : IntPtr.Zero;
				SET.rb.pSizes[uidx] = qualityMapBufferSize;
				uidx++; // QualityMap: 높이맵의 각 픽셀높이에 대한 점수(얼마나 신뢰할만한지)

				SET.rb.ppBuf[uidx] = (heightMapBufferSize > 0) ? Marshal.AllocHGlobal((int)heightMapBufferSize) : IntPtr.Zero;
				SET.rb.pSizes[uidx] = heightMapBufferSize;
				uidx++; // HeightMap: 각 pixel 별 높이 정보

				SET.rb.ppBuf[uidx] = (pointCloudBufferSize > 0) ? Marshal.AllocHGlobal((int)pointCloudBufferSize) : IntPtr.Zero;
				SET.rb.pSizes[uidx] = pointCloudBufferSize;
				uidx++; // PointCloud: 각 pixel의 (x,y,z) 좌표데이타
			}

			eErrorCode rv = SDOAQ_PlayEdof(
				AFP,
				sDOAQ_PlayEdofCallback,
				pPositions, (int)FOCUS.numsFocus,
				m_nRingBufferSize,
				SET.rb.ppBuf,
				SET.rb.pSizes
			);
			if (eErrorCode.ecNoError == rv)
			{
				SET.rb.active = true;
			}
			else
			{
				ApiError("SDOAQ_PlayEdof", rv);
			}

		}

		//----------------------------------------------------------------------------
		private void btnStopEdof_Click(object sender, EventArgs e)
		{
			OnSdoaqStopEdof();
		}

		private void OnSdoaqStopEdof()
		{
			if (SET.rb.active)
			{
				SET.rb.active = false;

				/// Note on Problem..
				/// If SDOAQ_StopEdof() is called directly (from UI Thread),
				///     timeout error occurs randomly and application crashes.
				/// So, the function is moved to ThreadPool (non UI Thread).
				/// The issue is cleared..

				ThreadPool.QueueUserWorkItem(StopEdofThread, new object { });
			}
		}

		static void StopEdofThread(object data)
		{
			SDOAQ_StopEdof();
		}
		#endregion

		#region ACQ AF
		//----------------------------------------------------------------------------
		private void btnAcqAF_Click(object sender, EventArgs e)
		{
			OnSdoaqAcqAF();
		}

		unsafe private void OnSdoaqAcqAF()
		{
			if (SET.rb.active)
			{
				return;
			}

			ClearPictureBoxes();

			AcquisitionFixedParameters[] AFP = new AcquisitionFixedParameters[1] { SET.afp };
			ref var FOCUS = ref SET.focus;

			FOCUS.numsFocus = (uint)m_vFocusSet.Count;
			FOCUS.vFocusSet = m_vFocusSet.ToList();

			int[] pPositions = new int[FOCUS.numsFocus];
			for (int pos = 0; pos < FOCUS.numsFocus; pos++)
			{
				pPositions[pos] = m_vFocusSet[pos];
			}

			uint AFImageBufferSize = (uint)SET.ImgSize();
			byte[] pAFImageBuffer = new byte[AFImageBufferSize];
			double[] dbBestFocusStep = new double[1];
			double[] dbScore = new double[1];

			eErrorCode rv = SDOAQ_SingleShotAF(
				AFP,
				pPositions, (int)FOCUS.numsFocus,
				pAFImageBuffer,
				AFImageBufferSize,
				dbBestFocusStep,
				dbScore
			);
			if (eErrorCode.ecNoError == rv)
			{
				++m_nContiAF;

#if CSHARP_PICTURE_BOX
				if (AFImageBufferSize > 0)
				{
					ConvertByteArrayToBitmap(pAFImageBuffer, Convert.ToInt32(AFImageBufferSize), 0, string.Format("AF({0})", m_nContiAF), AFP[0].cameraRoiWidth, AFP[0].cameraRoiHeight);
					ShowLog(string.Format("\t>> BEST FOCUS STEP : {0}, SCORE : {1:N4}", dbBestFocusStep[0], dbScore[0]));
				}
				else
				{
					//ImageViewer(0, "AF", m_nContiAF);
				}
#else
				if (pAFImageBuffer != null && AFImageBufferSize > 0)
				{
					fixed (byte* ptr = pAFImageBuffer)
					{
						ImageViewer(0, "AF", m_nContiAF, SET, ptr);
					}
					ShowLog(string.Format("\t>> BEST FOCUS STEP : {0}, SCORE : {1:N4}", dbBestFocusStep[0], dbScore[0]));
				}
				else
				{
					ImageViewer(0, "AF", m_nContiAF);
				}
#endif
			}
			else
			{
				ApiError("SDOAQ_SingleShotAF", rv);
			}

		}
		//----------------------------------------------------------------------------
		private void btnContiAF_Click(object sender, EventArgs e)
		{
			OnSdoaqContiAF();
		}

		//----------------------------------------------------------------------------
		unsafe private void OnSdoaqContiAF()
		{
			if (SET.rb.active)
			{
				return;
			}

			ClearPictureBoxes();

			AcquisitionFixedParameters[] AFP = new AcquisitionFixedParameters[1] { SET.afp };
			ref cFocus FOCUS = ref SET.focus;

			FOCUS.numsFocus = (uint)m_vFocusSet.Count;
			FOCUS.vFocusSet = m_vFocusSet.ToList();

			int[] pPositions = new int[FOCUS.numsFocus];
			for (int pos = 0; pos < FOCUS.numsFocus; pos++)
			{
				pPositions[pos] = FOCUS.vFocusSet[pos];
			}

			SET.rb.numsBuf = (uint)m_nRingBufferSize;
			SET.rb.pSizes = new ulong[SET.rb.numsBuf];

			var size = SET.ImgSize();
			var bufSize = size * (int)SET.rb.numsBuf;
			byte*[] ppFocusImages = new byte*[SET.rb.numsBuf];
			ulong[] pFocusImageBufferSizes = new ulong[SET.rb.numsBuf];

			SET.ClearBuffer();
			SET.AllocBuffer(bufSize);

			try
			{
				byte* pointerToFirst = (byte*)SET.bufHandlePtr;

				for (int pos = 0; pos < SET.rb.numsBuf; pos++)
				{
					ppFocusImages[pos] = (byte*)(pointerToFirst + size * pos);
					pFocusImageBufferSizes[pos] = (uint)size;
				}
				SET.rb.pointerToFirst = pointerToFirst;

				eErrorCode rv = SDOAQ_PlayAF(
					AFP,
					sDOAQ_PlayAfCallback,
					pPositions, (int)FOCUS.numsFocus,
					m_nRingBufferSize,
					ppFocusImages,
					pFocusImageBufferSizes
				);
				if (eErrorCode.ecNoError == rv)
				{
					SET.rb.active = true;
				}
				else
				{
					ApiError("SDOAQ_PlayAF", rv);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString(), "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

		}

		//----------------------------------------------------------------------------
		private void btnStopAF_Click(object sender, EventArgs e)
		{
			OnSdoaqStopAF();
		}

		private void OnSdoaqStopAF()
		{
			if (SET.rb.active)
			{
				SET.rb.active = false;
				ThreadPool.QueueUserWorkItem(StopAFThread, new object { });
			}

			/// Sometimes, System.AccessViolationException occurs.
			/// ClearBuffer() is called before allocation and on form closing.
			//SET.ClearBuffer();
		}

		static void StopAFThread(object data)
		{
			SDOAQ_StopAF();
		}

		#endregion

		#region SNAP
		//----------------------------------------------------------------------------
		unsafe private void btnSnap_Click(object sender, EventArgs e)
		{
			if (SET.rb.active)
			{
				var numsFocus = (uint)m_vSnapFocusSet.Count;
				int[] pPositions = new int[numsFocus];
				for (int pos = 0; pos < numsFocus; pos++)
				{
					pPositions[pos] = m_vSnapFocusSet[pos];
				}

				string currentDir = Directory.GetCurrentDirectory();
				const string Format = "yyyy.MMM.dd.HHmmss";
				string sSnapPath = Path.Combine(currentDir, "Snap", DateTime.Now.ToString(Format));

				SDOAQ_PlaySnap(sDOAQ_SnapCallback, pPositions, (int)numsFocus, sSnapPath);
			}
		}
		#endregion

		#region UI 함수
		private void ClearPictureBoxes()
		{
			GL.WSGL_Display_BG(m_hwnd3D);

			foreach (var pb in m_vPictBoxes)
			{
				pb.Image = null;
			}
		}
		#endregion

		#region 버전 및 빌드 정보
		/// <summary>
		/// 버전 정보를 넣으면 빌드 시간을 반환.
		/// </summary>
		/// <param name="version"></param>
		/// <returns></returns>
		public System.DateTime Get_BuildDateTime(System.Version version = null)
		{
			// 주.부.빌드.수정
			// 주 버전    Major Number
			// 부 버전    Minor Number
			// 빌드 번호  Build Number
			// 수정 버전  Revision NUmber

			//매개 변수가 존재할 경우
			if (version == null)
				version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

			//세번째 값(Build Number)은 2000년 1월 1일부터
			//Build된 날짜까지의 총 일(Days) 수 이다.
			int day = version.Build;
			System.DateTime dtBuild = (new System.DateTime(2000, 1, 1)).AddDays(day);

			//네번째 값(Revision NUmber)은 자정으로부터 Build된
			//시간까지의 지나간 초(Second) 값 이다.
			int intSeconds = version.Revision;
			intSeconds = intSeconds * 2;
			dtBuild = dtBuild.AddSeconds(intSeconds);


			//시차 보정
			System.Globalization.DaylightTime daylingTime = System.TimeZone.CurrentTimeZone
					.GetDaylightChanges(dtBuild.Year);
			if (System.TimeZone.IsDaylightSavingTime(dtBuild, daylingTime))
				dtBuild = dtBuild.Add(daylingTime.Delta);

			return dtBuild;
		}
		#endregion

		//----------------------------------------------------------------------------
		private void SDOAQ_APP_CS_Resize(object sender, EventArgs e)
		{
			foreach (var item in m_vhwndIV)
			{
				WSUT_IV_ShowWindow(item.vHwnd, 1, item.pBox.Left, item.pBox.Top, item.pBox.Right, item.pBox.Bottom);
			}
			GL.WSGL_ShowWindow(m_hwnd3D, true, pbViewer4.Left, pbViewer4.Top, pbViewer4.Right, pbViewer4.Bottom);
		}

		//----------------------------------------------------------------------------
		private void KeyDownEventHandler(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Delete:
					if (GL.WSGL_IsOnRun(m_hwnd3D))
					{
						GL.WSGL_ClearAllMeasureData(m_hwnd3D, true);
					}
					break;

				case Keys.Home:
					if (GL.WSGL_IsOnRun(m_hwnd3D))
					{
						GL.WSGL_ResetDisplay(m_hwnd3D);
					}
					break;

				case Keys.Back:
					if (GL.WSGL_IsOnRun(m_hwnd3D))
					{
						GL.WSGL_ClearLastMeasureData(m_hwnd3D, true);
					}
					break;
			}
		}

		private void btnSelectedCalibObject(object sender, EventArgs e)
		{
			// read calibration file and set calibaration data to sdoaq dll through SDOAQ_SetExternalCalibrationTable() api
		}
	}
}
