using System;
using GXX.Client.DxComponent;
using Xunit;
using Xunit.Abstractions;

namespace GXX.Client.Tests;

public sealed class DxDebugTests
{
    private readonly ITestOutputHelper _o;
    public DxDebugTests(ITestOutputHelper o) { _o = o; }

    [Fact]
    public void Probe()
    {
        _o.WriteLine("=== SetPosition 探针 ===");
        var c = new TDXTrackBar { Left = 0, Top = 0, Width = 100, Height = 20, Designing = false };
        c.Min = 0;
        c.Max = 10;
        c.SetPosition(0);
        _o.WriteLine($"before: Min={c.Min} Max={c.Max} Position={c.Position}");
        c.SetPosition(8);
        _o.WriteLine($"after SetPosition(8): Position={c.Position}");

        _o.WriteLine("=== Hot 探针 ===");
        var b = new TDXTrackBar { Left = 0, Top = 0, Width = 100, Height = 20, Designing = false };
        b.SliderIndex.Image = new TDxImageLibraryStub().Add(new TDxTextureStub(10, 10));
        b.SliderIndex.Up = 0;
        b.Min = 0;
        b.Max = 10;
        b.Position = 5;
        _o.WriteLine($"Position={b.Position} Min={b.Min} Max={b.Max}");
        b.MouseMoveTo(TDxShiftState.None, 50, 10);
        _o.WriteLine($"IsHotSlider={b.IsHotSlider}");

        Assert.True(true);
    }
}
