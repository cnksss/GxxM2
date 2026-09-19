using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace GXX.Client.HGE;

/// <summary>
/// HGE 1.81 API 面的托管抽象（HGE.pas → IHGE）：纹理/精灵/变换/输入/计时。
/// 默认实现 GdiPlusHge 用 GDI+ 双缓冲渲染；后续可插拔 Silk.NET(DirectX/OpenGL) 后端，接口面不变。
/// </summary>
public interface IHge : IDisposable
{
    bool Initialize(int screenWidth, int screenHeight, Control host);
    IntPtr Texture_Load(byte[] fileData);
    void Texture_Free(IntPtr tex);
    void Gfx_BeginScene();
    void Gfx_EndScene();
    void Gfx_Clear(uint color);
    void Gfx_RenderLine(float x1, float y1, float x2, float y2, uint color = 0xFFFFFFFF);
    void Gfx_RenderQuad(ref HgeQuad quad);
    float Timer_GetTime();
    bool Input_GetKeyState(int key);
    void System_SetWindowTitle(string title);
}

/// <summary>hgeQuad（四边形绘制单元）。</summary>
public struct HgeQuad
{
    public IntPtr Tex;
    public int Blend;          // BLEND_ALPHABLEND 等
    public float X, Y;         // 左上角屏幕坐标
    public float ScaleX, ScaleY;
    public float Rotation;     // 弧度
    public float HotX, HotY;
    public int TexX, TexY, TexW, TexH;  // 纹理区域
    public uint Color;
}

public static class HgeBlend
{
    public const int BLEND_DEFAULT = 0;
    public const int BLEND_ALPHABLEND = 2;
    public const int BLEND_NOBLEND = 1;
}

public static class HgeColor
{
    public static uint ARGB(byte a, byte r, byte g, byte b)
        => (uint)((a << 24) | (r << 16) | (g << 8) | b);
}

/// <summary>GDI+ 双缓冲实现（对应 HGE 底层 D3D8 的职责面）。</summary>
public class GdiPlusHge : IHge
{
    private Control? _host;
    private Bitmap? _backBuffer;
    private Graphics? _bufferGfx;
    private readonly System.Diagnostics.Stopwatch _timer = System.Diagnostics.Stopwatch.StartNew();

    public bool Initialize(int screenWidth, int screenHeight, Control host)
    {
        _host = host;
        _backBuffer = new Bitmap(screenWidth, screenHeight, PixelFormat.Format32bppArgb);
        _bufferGfx = Graphics.FromImage(_backBuffer);
        _bufferGfx.InterpolationMode = InterpolationMode.NearestNeighbor;
        _bufferGfx.PixelOffsetMode = PixelOffsetMode.Half;
        return true;
    }

    public void System_SetWindowTitle(string title)
    {
        if (_host != null && _host is Form f) f.Text = title;
    }

    public IntPtr Texture_Load(byte[] fileData)
    {
        try
        {
            using var ms = new System.IO.MemoryStream(fileData);
            var bmp = new Bitmap(ms);
            return GCHandle.ToIntPtr(GCHandle.Alloc(bmp));
        }
        catch
        {
            return IntPtr.Zero;
        }
    }

    public void Texture_Free(IntPtr tex)
    {
        try
        {
            var handle = GCHandle.FromIntPtr(tex);
            if (handle.Target is Bitmap bmp) bmp.Dispose();
            handle.Free();
        }
        catch { }
    }

    public void Gfx_BeginScene()
    {
        _bufferGfx?.ResetTransform();
    }

    public void Gfx_EndScene()
    {
        if (_host == null || _backBuffer == null) return;
        try
        {
            var gfx = _host.CreateGraphics();
            gfx.DrawImage(_backBuffer, 0, 0);
            gfx.Dispose();
        }
        catch { }
    }

    public void Gfx_Clear(uint color)
    {
        if (_bufferGfx == null) return;
        _bufferGfx.Clear(Color.FromArgb((int)color));
    }

    public void Gfx_RenderLine(float x1, float y1, float x2, float y2, uint color = 0xFFFFFFFF)
    {
        if (_bufferGfx == null) return;
        using var pen = new Pen(Color.FromArgb((int)color));
        _bufferGfx.DrawLine(pen, x1, y1, x2, y2);
    }

    public void Gfx_RenderQuad(ref HgeQuad quad)
    {
        if (_bufferGfx == null || quad.Tex == IntPtr.Zero) return;
        var handle = GCHandle.FromIntPtr(quad.Tex);
        if (handle.Target is not Bitmap bmp) return;

        var srcRect = new Rectangle(quad.TexX, quad.TexY, quad.TexW, quad.TexH);
        var dstRect = new RectangleF(
            quad.X - quad.HotX * quad.ScaleX,
            quad.Y - quad.HotY * quad.ScaleY,
            quad.TexW * quad.ScaleX,
            quad.TexH * quad.ScaleY);

        if (Math.Abs(quad.Rotation) > 0.001f)
        {
            var state = _bufferGfx.Save();
            float cx = dstRect.X + dstRect.Width / 2f;
            float cy = dstRect.Y + dstRect.Height / 2f;
            _bufferGfx.TranslateTransform(cx, cy);
            _bufferGfx.RotateTransform(quad.Rotation * 180f / (float)Math.PI);
            _bufferGfx.TranslateTransform(-cx, -cy);
            _bufferGfx.DrawImage(bmp, dstRect, srcRect, GraphicsUnit.Pixel);
            _bufferGfx.Restore(state);
        }
        else
        {
            _bufferGfx.DrawImage(bmp, dstRect, srcRect, GraphicsUnit.Pixel);
        }
    }

    public float Timer_GetTime() => (float)(_timer.Elapsed.TotalMilliseconds / 1000.0);

    public bool Input_GetKeyState(int key)
        => (NativeMethods.GetAsyncKeyState(key) & 0x8000) != 0;

    public void Dispose()
    {
        _bufferGfx?.Dispose(); _bufferGfx = null;
        _backBuffer?.Dispose(); _backBuffer = null;
    }
}

internal static class NativeMethods
{
    [DllImport("user32.dll")]
    public static extern short GetAsyncKeyState(int vKey);
}
