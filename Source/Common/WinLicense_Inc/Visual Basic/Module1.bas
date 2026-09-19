Attribute VB_Name = "Module1"

Type SYSTEMTIME
    
    wYear As Integer
    wMonth As Integer
    wDayOfWeek As Integer
    wDay As Integer
    wHour As Integer
    wMinute As Integer
    wSecond As Integer
    wMilliseconds As Integer
    
End Type

Type sLicenseFeatures

     cb As Long
     NumDays As Long
     NumExec As Long
     ExpDate As SYSTEMTIME
     CountryId As Long
     Runtime As Long
     GlobalMinutes As Long
     InstallDate As SYSTEMTIME
     NetInstances As Long
     EmbedLicenseInfoInKey As Long
     EmbedCreationDate As Long

End Type
 
Public Declare Function WLGenLicenseDynSmartKey Lib "WinlicenseSDK.dll" (ByVal LicenseHash As String, ByVal UserName As String, ByVal Organization As String, ByVal CustomData As String, ByVal MachineID As String, ByRef pLicenseFeatures As sLicenseFeatures, ByVal BufferOut As String) As Integer
Public Declare Function WLGenLicenseFileKeyEx Lib "WinlicenseSDK.dll" (ByVal LicenseHash As String, ByVal UserName As String, ByVal Organization As String, ByVal CustomData As String, ByVal MachineID As String, ByRef pLicenseFeatures As sLicenseFeatures, ByVal BufferOut As String) As Integer
Public Declare Function WLGenLicenseRegistryKeyEx Lib "WinlicenseSDK.dll" (ByVal LicenseHash As String, ByVal UserName As String, ByVal Organization As String, ByVal CustomData As String, ByVal MachineID As String, ByRef pLicenseFeatures As sLicenseFeatures, ByVal BufferOut As String) As Integer
Public Declare Function WLGenLicenseTextKeyEx Lib "WinlicenseSDK.dll" (ByVal LicenseHash As String, ByVal UserName As String, ByVal Organization As String, ByVal CustomData As String, ByVal MachineID As String, ByRef pLicenseFeatures As sLicenseFeatures, ByVal BufferOut As String) As Integer


Public Declare Function WLGenTrialExtensionFileKey Lib "WinlicenseSDK.dll" (ByVal TrialHash As String, ByVal Level As Integer, ByVal NumDays As Integer, ByVal NumExec As Integer, NewDate As Any, ByVal NumMinutes As Integer, ByVal TimeRuntime As Integer, ByVal BufferOut As String) As Integer
Public Declare Function WLGenTrialExtensionRegistryKey Lib "WinlicenseSDK.dll" (ByVal TrialHash As String, ByVal Level As Integer, ByVal NumDays As Integer, ByVal NumExec As Integer, NewDate As Any, ByVal NumMinutes As Integer, ByVal TimeRuntime As Integer, ByVal pKeyName As String, ByVal pKeyValueName As String, ByVal BufferOut As String) As Integer
Public Declare Function WLGenPassword Lib "WinlicenseSDK.dll" (ByVal PasswordHash As String, ByVal UserName As String, ByVal BufferOut As String) As Integer
Public Declare Function WLGenLicenseTextKey Lib "WinlicenseSDK.dll" (ByVal LicenseHash As String, ByVal UserName As String, ByVal Organization As String, ByVal CustomData As String, ByVal MachineID As String, ByVal NumDays As Integer, ByVal NumExec As Integer, ByVal NewDate As Any, ByVal CountryId As Integer, ByVal Runtime As Integer, ByVal GlobalMinutes As Integer, ByVal BufferOut As String) As Integer
Public Declare Function WLGenLicenseTextKeyW Lib "WinlicenseSDK.dll" (ByVal LicenseHash As String, ByVal UserName As String, ByVal Organization As String, ByVal CustomData As String, ByVal MachineID As String, ByVal NumDays As Integer, ByVal NumExec As Integer, ByVal NewDate As Any, ByVal CountryId As Integer, ByVal Runtime As Integer, ByVal GlobalMinutes As Integer, ByVal BufferOut As String) As Integer
Public Declare Function WLGenLicenseSmartKey Lib "WinlicenseSDK.dll" (ByVal LicenseHash As String, ByVal UserName As String, ByVal Organization As String, ByVal CustomData As String, ByVal MachineID As String, ByVal NumDays As Integer, ByVal NumExec As Integer, ByVal NewDate As Any, ByVal BufferOut As String) As Integer
Public Declare Function WLGenLicenseSmartKeyW Lib "WinlicenseSDK.dll" (ByVal LicenseHash As String, ByVal UserName As String, ByVal Organization As String, ByVal CustomData As String, ByVal MachineID As String, ByVal NumDays As Integer, ByVal NumExec As Integer, ByVal NewDate As Any, ByVal BufferOut As String) As Integer
Public Declare Function WLGenLicenseFileKey Lib "WinlicenseSDK.dll" (ByVal LicenseHash As String, ByVal UserName As String, ByVal Organization As String, ByVal CustomData As String, ByVal MachineID As String, ByVal NumDays As Integer, ByVal NumExec As Integer, ByVal NewDate As Any, ByVal CountryId As Integer, ByVal Runtime As Integer, ByVal GlobalMinutes As Integer, ByVal BufferOut As String) As Integer
Public Declare Function WLGenLicenseFileKeyW Lib "WinlicenseSDK.dll" (ByVal LicenseHash As String, ByVal UserName As String, ByVal Organization As String, ByVal CustomData As String, ByVal MachineID As String, ByVal NumDays As Integer, ByVal NumExec As Integer, ByVal NewDate As Any, ByVal CountryId As Integer, ByVal Runtime As Integer, ByVal GlobalMinutes As Integer, ByVal BufferOut As String) As Integer
Public Declare Function WLGenLicenseRegistryKey Lib "WinlicenseSDK.dll" (ByVal LicenseHash As String, ByVal UserName As String, ByVal Organization As String, ByVal CustomData As String, ByVal MachineID As String, ByVal NumDays As Integer, ByVal NumExec As Integer, ByVal NewDate As Any, ByVal CountryId As Integer, ByVal Runtime As Integer, ByVal GlobalMinutes As Integer, ByVal RegName As String, ByVal RegValueName As String, ByVal BufferOut As String) As Integer
Public Declare Function WLGenLicenseRegistryKeyW Lib "WinlicenseSDK.dll" (ByVal LicenseHash As String, ByVal UserName As String, ByVal Organization As String, ByVal CustomData As String, ByVal MachineID As String, ByVal NumDays As Integer, ByVal NumExec As Integer, ByVal NewDate As Any, ByVal CountryId As Integer, ByVal Runtime As Integer, ByVal GlobalMinutes As Integer, ByVal RegName As String, ByVal RegValueName As String, ByVal BufferOut As String) As Integer
Public Declare Function WLRegGetStatus Lib "WinlicenseSDK.dll" (ByVal ExtendedInfo As Integer) As Integer
Public Declare Function WLTrialGetStatus Lib "WinlicenseSDK.dll" (ByVal ExtendedInfo As Integer) As Integer
Public Declare Function WLTrialExtGetStatus Lib "WinlicenseSDK.dll" () As Integer
Public Declare Function WLRegGetLicenseInfo Lib "WinlicenseSDK.dll" (ByVal UserName As String, ByVal Organization As String, ByVal CustomData As String) As Boolean
Public Declare Function WLRegGetLicenseInfoW Lib "WinlicenseSDK.dll" (ByVal UserName As String, ByVal Organization As String, ByVal CustomData As String) As Boolean
Public Declare Function WLTrialTotalDays Lib "WinlicenseSDK.dll" () As Integer
Public Declare Function WLTrialTotalExecutions Lib "WinlicenseSDK.dll" () As Integer
Public Declare Function WLTrialDaysLeft Lib "WinlicenseSDK.dll" () As Integer
Public Declare Function WLTrialExecutionsLeft Lib "WinlicenseSDK.dll" () As Integer
Public Declare Function WLTrialExpirationDate Lib "WinlicenseSDK.dll" (ExpDate As Any) As Integer
Public Declare Function WLTrialGlobalTimeLeft Lib "WinlicenseSDK.dll" () As Integer
Public Declare Function WLTrialRuntimeLeft Lib "WinlicenseSDK.dll" () As Integer
Public Declare Function WLTrialLockedCountry Lib "WinlicenseSDK.dll" () As Integer
Public Declare Function WLRegDaysLeft Lib "WinlicenseSDK.dll" () As Integer
Public Declare Function WLRegExecutionsLeft Lib "WinlicenseSDK.dll" () As Integer
Public Declare Function WLRegExpirationDate Lib "WinlicenseSDK.dll" (ExpDate As Any) As Integer
Public Declare Function WLRegTotalExecutions Lib "WinlicenseSDK.dll" () As Integer
Public Declare Function WLRegTotalDays Lib "WinlicenseSDK.dll" () As Integer
Public Declare Function WLHardwareGetID Lib "WinlicenseSDK.dll" (ByVal HardId As String) As Boolean
Public Declare Function WLRegGetLicenseHardwareID Lib "WinlicenseSDK.dll" (ByVal HardId As String) As Boolean
Public Declare Function WLHardwareCheckID Lib "WinlicenseSDK.dll" (ByVal HardId As String) As Boolean
Public Declare Function WLRegSmartKeyCheck Lib "WinlicenseSDK.dll" (ByVal UserName As String, ByVal Organization As String, ByVal CustomData As String, ByVal AsciiKey As String) As Boolean
Public Declare Function WLRegSmartKeyCheckW Lib "WinlicenseSDK.dll" (ByVal UserName As String, ByVal Organization As String, ByVal CustomData As String, ByVal AsciiKey As String) As Boolean
Public Declare Function WLRegNormalKeyCheck Lib "WinlicenseSDK.dll" (ByVal AsciiKey As String) As Boolean
Public Declare Function WLRegNormalKeyInstallToFile Lib "WinlicenseSDK.dll" (ByVal AsciiKey As String) As Boolean
Public Declare Function WLRegNormalKeyInstallToRegistry Lib "WinlicenseSDK.dll" (ByVal AsciiKey As String) As Boolean
Public Declare Function WLRegSmartKeyInstallToFile Lib "WinlicenseSDK.dll" (ByVal UserName As String, ByVal Organization As String, ByVal CustomData As String, ByVal AsciiKey As String) As Boolean
Public Declare Function WLRegSmartKeyInstallToFileW Lib "WinlicenseSDK.dll" (ByVal UserName As String, ByVal Organization As String, ByVal CustomData As String, ByVal AsciiKey As String) As Boolean
Public Declare Function WLRegSmartKeyInstallToFileInFolder Lib "WinlicenseSDK.dll" (ByVal UserName As String, ByVal Organization As String, ByVal CustomData As String, ByVal AsciiKey As String, ByVal FilePath As String) As Boolean
Public Declare Function WLRegSmartKeyInstallToRegistry Lib "WinlicenseSDK.dll" (ByVal UserName As String, ByVal Organization As String, ByVal CustomData As String, ByVal AsciiKey As String) As Boolean
Public Declare Function WLRegSmartKeyInstallToRegistryW Lib "WinlicenseSDK.dll" (ByVal UserName As String, ByVal Organization As String, ByVal CustomData As String, ByVal AsciiKey As String) As Boolean
Public Declare Function WLLoadWinlicenseDll Lib "WinlicenseSDK.dll" () As Integer
Public Declare Function WLTrialCustomCounterInc Lib "WinlicenseSDK.dll" (ByVal Value As Integer, ByVal CounterID As Integer) As Integer
Public Declare Function WLTrialCustomCounterDec Lib "WinlicenseSDK.dll" (ByVal Value As Integer, ByVal CounterID As Integer) As Integer
Public Declare Function WLTrialCustomCounter Lib "WinlicenseSDK.dll" (ByVal CounterID As Integer) As Integer
Public Declare Function WLTrialCustomCounterSet Lib "WinlicenseSDK.dll" (ByVal Value As Integer, ByVal CounterID As Integer) As Integer
Public Declare Function WLRestartApplication Lib "WinlicenseSDK.dll" () As Boolean
Public Declare Function WLRegLockedCountry Lib "WinlicenseSDK.dll" () As Integer
Public Declare Function WLRegRuntimeLeft Lib "WinlicenseSDK.dll" () As Integer
Public Declare Function WLRegGlobalTimeLeft Lib "WinlicenseSDK.dll" () As Integer
Public Declare Function WLRegDisableCurrentKey Lib "WinlicenseSDK.dll" (ByVal DisableFlags As Integer) As Boolean
Public Declare Function WLRegRemoveCurrentKey Lib "WinlicenseSDK.dll" () As Boolean
Public Declare Function WLHardwareGetFormattedID Lib "WinlicenseSDK.dll" (ByVal NumCharBlock As Integer, ByVal Uppercase As Integer, ByVal Buffer As String) As Boolean
Public Declare Function WLPasswordCheck Lib "WinlicenseSDK.dll" (ByVal UserName As String, ByVal Password As String) As Boolean
Public Declare Function WLTrialExpireTrial Lib "WinlicenseSDK.dll" () As Boolean
Public Declare Function WLRegLicenseName Lib "WinlicenseSDK.dll" (ByVal FileKeyName As String, ByVal RegKeyName As String, ByVal RegKeyValueName As String) As Boolean
Public Declare Function WLRestartApplicationArgs Lib "WinlicenseSDK.dll" (ByVal pArgs As String) As Boolean
Public Declare Function WLActGetInfo Lib "WinlicenseSDK.dll" (ByRef Custom1 As Integer, ByRef Custom2 As Integer, ByRef Custom3 As Integer) As Boolean
Public Declare Function WLActCheck Lib "WinlicenseSDK.dll" (ByVal ActivationCode As String) As Boolean
Public Declare Function WLActInstall Lib "WinlicenseSDK.dll" (ByVal ActivationCode As String) As Boolean
Public Declare Function WLActExpirationDate Lib "WinlicenseSDK.dll" (ExpDate As Any) As Boolean
Public Declare Function WLActDaysToActivate Lib "WinlicenseSDK.dll" () As Integer
Public Declare Function WLActUninstall Lib "WinlicenseSDK.dll" () As Boolean
Public Declare Function WLGetCurrentCountry Lib "WinlicenseSDK.dll" () As Integer
Public Declare Function WLTrialExtGetLevel Lib "WinlicenseSDK.dll" () As Integer
Public Declare Function WLProtectCheckDebugger Lib "WinlicenseSDK.dll" () As Boolean
Public Declare Function WLTrialExtendExpiration Lib "WinlicenseSDK.dll" (ByVal NumDays As Integer, ByVal NumExec As Integer, NewDate As Any, ByVal NumMinutes As Integer, ByVal TimeRuntime As Integer) As Boolean
Public Declare Function WLTrialFirstRun Lib "WinlicenseSDK.dll" () As Boolean
Public Declare Function WLRegFirstRun Lib "WinlicenseSDK.dll" () As Boolean
Public Declare Function WLRegCheckMachineLocked Lib "WinlicenseSDK.dll" () As Boolean
Public Declare Function WLSplashHide Lib "WinlicenseSDK.dll" () As Boolean
Public Declare Function WLBufferCrypt Lib "WinlicenseSDK.dll" (Buffer As Any, BufferLength As Integer, ByVal Password As String) As Boolean
Public Declare Function WLBufferDecrypt Lib "WinlicenseSDK.dll" (Buffer As Any, BufferLength As Integer, ByVal Password As String) As Boolean
Public Declare Function WLTrialDateDaysLeft Lib "WinlicenseSDK.dll" () As Integer
Public Declare Function WLRegDateDaysLeft Lib "WinlicenseSDK.dll" () As Integer
Public Declare Function WLRegGetDynSmartKey Lib "WinlicenseSDK.dll" (ByVal SmartKey As String) As Boolean
Public Declare Function WLRegDisableKeyCurrentInstance Lib "WinlicenseSDK.dll" () As Boolean
Public Declare Function WLHardwareRuntimeCheckU3 Lib "WinlicenseSDK.dll" () As Boolean
Public Declare Function WLGetVersion Lib "WinlicenseSDK.dll" (ByVal Buffer As String) As Boolean
Public Declare Function WLRegNetInstancesGet Lib "WinlicenseSDK.dll" () As Integer
Public Declare Function WLRegNetInstancesMax Lib "WinlicenseSDK.dll" () As Integer
Public Declare Function WLGetProtectionDate Lib "WinlicenseSDK.dll" (pProtectionDate As Any) As Integer
Public Declare Function WLProtectCheckCodeIntegrity Lib "WinlicenseSDK.dll" () As Boolean
Public Declare Function WLHardwareGetIdType Lib "WinlicenseSDK.dll" () As Integer
Public Declare Function WLTrialStringRead Lib "WinlicenseSDK.dll" (ByVal StringName As String, ByVal StringValue As String) As Boolean
Public Declare Function WLTrialStringWrite Lib "WinlicenseSDK.dll" (ByVal StringName As String, ByVal StringValue As String) As Boolean
Public Declare Function WLTrialDebugCheck Lib "WinlicenseSDK.dll" () As Boolean
Public Declare Function WLRegGetLicenseRestrictions Lib "WinlicenseSDK.dll" () As Integer
Public Declare Function WLRegGetLicenseType Lib "WinlicenseSDK.dll" () As Integer
Public Declare Function WLCheckVirtualPC Lib "WinlicenseSDK.dll" () As Boolean
Public Declare Function WLTrialGetTrialRestrictions Lib "WinlicenseSDK.dll" () As Integer
Public Declare Function WLRegActivateSoftware Lib "WinlicenseSDK.dll" (ByVal pActivationKey As String, OutWinsockErrorCode As Variant, ByVal OutServerOutput As String, ByVal OutWinsockErrorCode As Integer) As Integer
Public Declare Function WLRegDeactivateSoftware Lib "WinlicenseSDK.dll" (ByVal pActivationKey As String, OutWinsockErrorCode As Variant, ByVal OutServerOutput As String, ByVal OutWinsockErrorCode As Integer) As Integer


Public Const wlNoTrialExt = 0
Public Const wlAppExtended = 1
Public Const wlInvalidTrialExt = 2
Public Const wlNoMoreExt = 3
Public Const wlTrialOk = 0
Public Const wlTrialDaysExpired = 1
Public Const wlTrialExecExpired = 2
Public Const wlTrialDateExpired = 3
Public Const wlTrialRuntimExpired = 4
Public Const wlTrialGlobalExpired = 5
Public Const wlTrialInvalidCountry = 6
Public Const wlTrialManipulated = 7
Public Const wlMarkStolenKey = 0
Public Const wlMarkInvalidKey = 1
Public Const wlLicenseDaysExpired = 1
Public Const wlLicenseExecExpired = 2
Public Const wlLicenseDateExpired = 3
Public Const wlLicenseGlobalExpired = 4
Public Const wlLicenseRuntimeExpired = 5
Public Const wlLicenseActivationExpired = 6
Public Const wlIsTrial = 0
Public Const wlIsRegistered = 1
Public Const wlInvalidLicense = 2
Public Const wlInvalidHardwareLicense = 3
Public Const wlNoMoreHwdChanges = 4
Public Const wlLicenseExpired = 5
Public Const wlInvalidCountryLicense = 6
Public Const wlLicenseStolen = 7
Public Const wlWrongLicenseExp = 8
Public Const wlWrongLicenseHardware = 9
Public Const wlIsRegisteredAndActivated = wlIsRegistered
Public Const wlIsRegisteredNotActivated = 10
Public Const wlNoMoreInstancesAllowed = 12
Public Const wlNetworkNoServerRunning = 13
Public Const wlInstallLicenseDateExpired = 14
Public Const wlLicenseDisabledInInstance = 15
Public Const wlTrialModeNotSupported = 16


Public Const wlPermKey = -1
Public Const wlNoKey = -2
Public Const wlNoTrialDate = -1
Public Const wlInvalidCounter = -1

Public Const wlRegRestrictionDays = 2 ^ 0
Public Const wlRegRestrictionExec = 2 ^ 1
Public Const wlRegRestrictionDate = 2 ^ 2
Public Const wlRegRestrictionRuntime = 2 ^ 3
Public Const wlRegRestrictionGlobalTime = 2 ^ 4
Public Const wlRegRestrictionCountry = 2 ^ 5
Public Const wlRegRestrictionHardwareId = 2 ^ 6
Public Const wlRegRestrictionNetwork = 2 ^ 7
Public Const wlRegRestrictionInstallDate = 2 ^ 8
Public Const wlRegRestrictionCreationDate = 2 ^ 9
Public Const wlRegRestrictionEmbedUserInfo = 2 ^ 10


Public Const wlTrialRestrictionUnlimited = 0
Public Const wlTrialRestrictionDays = 2 ^ 0
Public Const wlTrialRestrictionExec = 2 ^ 1
Public Const wlTrialRestrictionDate = 2 ^ 2
Public Const wlTrialRestrictionRuntime = 2 ^ 3
Public Const wlTrialRestrictionGlobalTime = 2 ^ 4
Public Const wlTrialRestrictionCountry = 2 ^ 5


Global GlobalBuffer As String * 4000
Global RegName As String * 200
Global RegCompany As String * 200
Global RegCustom As String * 200

Public Const wlActivationOk = 0
Public Const wlActivationErrorKeyNotFound = 1
Public Const wlActivationErrorDeviceRevoked = 2
Public Const wlActivationErrorMaxDevicesReached = 3
Public Const wlActivationErrorDeviceReactivationExcedeed = 4
Public Const wlActivationErrorNoMoreRevokeAllowed = 5
Public Const wlActivationErrorDeviceNotFound = 6
Public Const wlActivationErrorWrongServerDataReceived = 7
Public Const wlActivationErrorKeyDisabledBySeller = 8
Public Const wlActivationErrorKeyExpired = 9
Public Const wlActivationErrorCannotInstallLicense = 50
Public Const wlActivationErrorWinsockError = 100


