(*
 *
 * 单元 : chongchong
 * 网站 :
 *
 * 说明 : 阿里云错误信息
 *
 * CHANGES:
 * v1.0 <2017-04-21>
 *   + first release
 *)

unit acsError;

interface

type
  TErrorMsg = (emCode, emCnMsg);

const
  acsError_SignleSendSms: array[0..7, 0..1] of string = (
      ('InvalidDayuStatus.Malformed',               '账户短信开通状态不正确'),
      ('InvalidSignName.Malformed',                 '短信签名不正确或签名状态不正确'),
      ('InvalidTemplateCode.MalFormed',             '短信模板Code不正确或者模板状态不正确'),
      ('InvalidRecNum.Malformed',                   '目标手机号不正确，单次发送数量不能超过100'),
      ('InvalidParamString.MalFormed',              '短信模板中变量不是json格式'),
      ('InvalidParamStringTemplate.Malformed',      '短信模板中变量与模板内容不匹配'),
      ('InvalidSendSms',                            '触发业务流控'),
      ('InvalidDayu.Malformed',                     '变量不能是url，可以将变量固化在模板中')
  );

implementation

end.
