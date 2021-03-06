using UnityEngine;
using System.Collections;
using NyARUnityUtils;
using jp.nyatla.nyartoolkit.cs.markersystem;
using jp.nyatla.nyartoolkit.cs.core;

namespace NyARUnityUtils
{
	/// <summary>
	/// This class provides WebCamTexture wrapper derived  from NyARMarkerSystemSensor.
	/// </summary>
	/// <exception cref='NyARException'>
	/// Is thrown when the ny AR exception.
	/// </exception>
	public class NyARUnityWebCam :NyARSensor
	{
		/// <summary>
		/// This function creates WebCamTexture wraped Sensor.
		/// </summary>
		/// <param name="i_wtx">
		/// A <see cref="WebCamTexture"/>
		/// </param>
		/// <returns>
		/// A <see cref="NyARUnityWebCam"/>
		/// </returns>
		public static NyARUnityWebCam createInstance(WebCamTexture i_wtx)
		{
			if(i_wtx.isPlaying)
			{
				//起動中
				return new NyARUnityWebCam(i_wtx);
			}else{
				//一時的にON
				NyARUnityWebCam ret;
				i_wtx.Play();
				ret=new NyARUnityWebCam(i_wtx);
				i_wtx.Stop();
				return ret;
			}
		}

		public int width{
			get{return this._raster.getWidth();}
		}
		public int height{
			get{return this._raster.getHeight();}
		}
		private WebCamTexture _wtx;
	    private NyARUnityRaster _raster;
		/**
		 * WebcamTextureを元にインスタンスを生成します.
		 * 画像サイズを自分で設定できます.
		 * @param i_wtx
		 * Webカメラは開始されている必要があります.
		 * 
		 */
		protected NyARUnityWebCam(WebCamTexture i_wtx): base(new NyARIntSize(i_wtx.width,i_wtx.height))
		{
			//念のためチェック
			if(!i_wtx.isPlaying){
				throw new NyARException("WebCamTexture must be startings.");
			}
	        //RGBラスタの生成(Webtextureは上下反転必要)
	        this._raster = new NyARUnityRaster(i_wtx.width,i_wtx.height,true);
	        //ラスタのセット
	        base.update(this._raster);
			this._wtx=i_wtx;
		}
	    /**
	     * この関数は、JMFの非同期更新を停止します。
	     */
	    public void stop()
	    {
	        this._wtx.Stop();
	    }
	    /**
	     * この関数は、JMFの非同期更新を開始します。
	     */
	    public void start()
	    {
	        this._wtx.Play();
	    }
		/**
		 * Call this function on update!
		 */
		public void update()
		{
			if(!this._wtx.didUpdateThisFrame){
				return;
			}
			//テクスチャがアップデートされていたら、ラスタを更新
			this._raster.updateByWebCamTexture(this._wtx);
			//センサのタイムスタンプを更新
			base.updateTimeStamp();
			return;
		}
		public override void update(INyARRgbRaster i_input)
		{
			throw new NyARException();
		}
		public void dGetGsTex(Texture2D tx)
		{
			int[] s=(int[])this._gs_raster.getBuffer();
			Color32[] c=new Color32[320*240];
			for(int i=0;i<240;i++){
				for(int i2=0;i2<320;i2++){
					c[i*320+i2].r=c[i*320+i2].g=c[i*320+i2].b=(byte)s[i*320+i2];
					c[i*320+i2].a=0xff;
				}
			}
			tx.SetPixels32(c);
			tx.Apply( false );
		}
	}

}

