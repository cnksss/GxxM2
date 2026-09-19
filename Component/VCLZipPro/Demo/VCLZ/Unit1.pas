unit Unit1;

interface

uses
  Winapi.Windows, Winapi.Messages, System.SysUtils, System.Variants, System.Classes, Vcl.Graphics,
  Vcl.Controls, Vcl.Forms, Vcl.Dialogs, Vcl.StdCtrls, VCLUnZip, VCLZip;

type
  TForm1 = class(TForm)
    VCLZip1: TVCLZip;
    Button1: TButton;
    Button2: TButton;
    VCLUnZip1: TVCLUnZip;
    Label1: TLabel;
    procedure Button1Click(Sender: TObject);
    procedure Button2Click(Sender: TObject);
  private
    { Private declarations }
  public
    { Public declarations }
  end;

var
  Form1: TForm1;

implementation

{$R *.dfm}

procedure TForm1.Button1Click(Sender: TObject);
begin
     VCLZip1.ZipName := 'Examples.zip'; // Задайте имя архива
      VCLZip1.FilesList.Add('pic\*.*'); // Папку для архивации
       VCLZip1.Zip;                     // Архивировать файлы

        if VCLZip1.Zip = 0 then
           Label1.Caption := 'Создан';
end;

procedure TForm1.Button2Click(Sender: TObject);
begin
    VCLUnZip1.ZipName := 'Examples.zip';       // Задайте имя архива
     VCLUnZip1.ReadZip;                        // открыть и прочитать его информацию
      VCLUnZip1.FilesList.Add('*');            // распаковать все файлов
       VCLUnZip1.DestDir:= 'Examples';         // Установить папку назначения
        VCLUnZip1.RetainAttributes := true;    // Установите атрибуты оригинального после распаковки
         VCLUnZip1.UnZip;                      // Извлечь файлы

         if VCLUnZip1.UnZip = 0 then
           Label1.Caption := 'Распакован';
end;

end.
