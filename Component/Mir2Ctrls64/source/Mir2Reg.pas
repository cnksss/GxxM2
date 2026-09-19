unit Mir2Reg;

interface

uses
  Classes, ColorIndexEdit, SpinEditEx, PictureButton, BmpProgressBar, PaintPanel,
  ButtonEdit, EditEx, StyleForm,
  {$IFNDEF  CPUX64}
  FrmWizard,
   DesignIntf,
   ExptIntf,
    DesignEditors,
  {$ENDIF}

  Mir2Common, HtmlLabel, RealTimeMarquee, ThreadTimer;

{$IFNDEF  CPUX64}
type
  TStyleFormProperty = class(TClassProperty)
  public
    function GetAttributes: TPropertyAttributes; override;
    function GetValue: string; override;
    procedure GetValues(Proc: TGetStrProc); override;
    procedure SetValue(const Value: string); override;
  end;
  {$ENDIF}

  procedure Register;

implementation

{$IFNDEF  CPUX64}
{ TStyleFormProperty }

function TStyleFormProperty.GetAttributes: TPropertyAttributes;
begin
  Result := inherited GetAttributes
end;

function TStyleFormProperty.GetValue: string;
begin
  Result := inherited GetValue;
end;

procedure TStyleFormProperty.GetValues(Proc: TGetStrProc);
begin
  inherited;

end;

procedure TStyleFormProperty.SetValue(const Value: string);
begin
  inherited;

end;
{$ENDIF}

procedure Register;
begin
  RegisterComponents('Mir2Ctrls', [TColorIndexEdit, TSpinEditEx, TSpinEditLongWord,
    TPictureButton, TBmpProgressBar, TPaintPanel, TEditEx, TButtonEdit, THtmlLabel,
    TRealTimeMarquee, TThreadTimer]);

  {$IFNDEF  CPUX64}
  RegisterCustomModule(TCustomStyleForm, TCustomModule);
  RegisterLibraryExpert(TFrmCustomTitleExpert.Create);    // RegisterPackageWizard(TCustomDropDownFormEhWizard.Create);

  UnlistPublishedProperty(TStyleForm, 'BorderColor');
  UnlistPublishedProperty(TStyleForm, 'EnabledEraseBkgnd');
  {$ENDIF}


  {
  //RegisterSelectionEditor(TStyleForm, TSelectionEditor);
  //
  UnlistPublishedProperty(TStyleForm, 'StyleImage');
  UnlistPublishedProperty(TStyleForm, 'HorzStretchPoint');
  UnlistPublishedProperty(TStyleForm, 'VertStretchPoint');
  UnlistPublishedProperty(TStyleForm, 'HorzStretchType');
  UnlistPublishedProperty(TStyleForm, 'VertStretchType');
  }
  {
  RegisterPropertyEditor(TypeInfo(Integer), TStyleForm, 'HorzStretchPoint', TIntegerProperty);
  RegisterPropertyEditor(TypeInfo(Integer), TStyleForm, 'VertStretchPoint', TIntegerProperty);
  RegisterPropertyEditor(TypeInfo(THorzStretchType), TStyleForm, 'HorzStretchType', TEnumProperty);
  RegisterPropertyEditor(TypeInfo(TVertStretchType), TStyleForm, 'VertStretchType', TEnumProperty);
  }

end;


end.
