#ifndef __CUSTOMWINLICENSDK__
#define __CUSTOMWINLICENSDK__


// ***********************************************
// WinLicense typedef definition
// ***********************************************

typedef struct _sLicenseFeatures 
{ 
	unsigned	cb;						// size of struct
	unsigned 	NumDays;				// expiration days
	unsigned	NumExec;				// expiration executions
	SYSTEMTIME  ExpDate;				// expiration date 
	unsigned	CountryId;				// country ID
	unsigned	Runtime;				// expiration runtime
	unsigned	GlobalMinutes;  		// global time expiration
	SYSTEMTIME	InstallDate;			// Date to install the license since it was created
	unsigned	NetInstances;			// Network instances via shared file
	unsigned	EmbedLicenseInfoInKey;	// for Dynamic SmartKeys, it embeds Name+Company+Custom inside generated SmartKey
	unsigned    EmbedCreationDate;  	// Embed the date that the key was created
} sLicenseFeatures;


// ***********************************************
// WinLicense functions prototype
// ***********************************************

 #ifdef __BORLANDC__

 extern "C" _stdcall __declspec(dllimport) int WLCustomGenTrialExtensionFileKey(int Level,\
                                                int NumDays, int NumExec, SYSTEMTIME* NewDate, int NumMinutes,\
                                                int TimeRuntime, char* BufferOut);

 extern "C" _stdcall __declspec(dllimport) int WLCustomGenTrialExtensionRegistryKey(int Level,\
                                                int NumDays, int NumExec, SYSTEMTIME* NewDate, int NumMinutes,\
                                                int TimeRuntime, char* pKeyName, char* pKeyValueName, char* BufferOut);

 extern "C" _stdcall __declspec(dllimport) int WLCustomGenPassword(char* Name, char* BufferOut);

 extern "C" _stdcall __declspec(dllimport) int WLCustomGenLicenseFileKey(char* UserName, char* Organization,\
                                                char* CustomData, char* MachineID, int NumDays, int NumExec, SYSTEMTIME* NewDate, \
                                                int CountryId, int Runtime, int GlobalMinutes, char* BufferOut);

 extern "C" _stdcall __declspec(dllimport) int WLCustomGenLicenseFileKeyW(const wchar_t* UserName, const wchar_t* Organization,\
                                                const wchar_t* CustomData, const wchar_t* MachineID, int NumDays, int NumExec, SYSTEMTIME* NewDate, \
                                                int CountryId, int Runtime, int GlobalMinutes, char* BufferOut);

 extern "C" _stdcall __declspec(dllimport) int WLCustomGenLicenseFileKeyEx(const char* UserName, const char* Organization,\
                                                const char* CustomData, const char* MachineID, sLicenseFeatures* LicenseFeatures, char* BufferOut);

 extern "C" _stdcall __declspec(dllimport) int WLCustomGenLicenseFileKeyExW(const wchar_t* UserName, const wchar_t* Organization,\
                                                const wchar_t* CustomData, const wchar_t* MachineID, sLicenseFeatures* LicenseFeatures, char* BufferOut);

 extern "C" _stdcall __declspec(dllimport) int WLCustomGenLicenseRegistryKey(char* UserName, char* Organization,\
                                                char* CustomData, char* MachineID, int NumDays, int NumExec, SYSTEMTIME* NewDate, \
                                                int CountryId, int Runtime, int GlobalMinutes, char* KeyName, char* KeyValueName, char* BufferOut);

 extern "C" _stdcall __declspec(dllimport) int WLCustomGenLicenseRegistryKeyW(const wchar_t* UserName, const wchar_t* Organization,\
                                                const wchar_t* CustomData, const wchar_t* MachineID, int NumDays, int NumExec, SYSTEMTIME* NewDate, \
                                                int CountryId, int Runtime, int GlobalMinutes, const wchar_t* KeyName, const wchar_t* KeyValueName, char* BufferOut);

 extern "C" _stdcall __declspec(dllimport) int WLCustomGenLicenseRegistryKeyEx(char* UserName, char* Organization,\
                                                char* CustomData, char* MachineID, sLicenseFeatures* LicenseFeatures, char* KeyName, char* KeyValueName, char* BufferOut);

 extern "C" _stdcall __declspec(dllimport) int WLCustomGenLicenseRegistryKeyExW(const wchar_t* UserName, const wchar_t* Organization,\
                                                const wchar_t* CustomData, const wchar_t* MachineID, sLicenseFeatures* LicenseFeatures, 
                                                const char* KeyName, const wchar_t* KeyValueName, wchar_t* BufferOut);

 extern "C" _stdcall __declspec(dllimport) int WLCustomGenLicenseTextKey(char* UserName, char* Organization,\
                                                char* CustomData, char* MachineID, int NumDays, int NumExec, SYSTEMTIME* NewDate, \
                                                int CountryId, int Runtime, int GlobalMinutes, char* BufferOut);

 extern "C" _stdcall __declspec(dllimport) int WLCustomGenLicenseTextKeyW(const wchar_t* UserName, const wchar_t* Organization,\
                                                const wchar_t* CustomData, char* MachineID, int NumDays, int NumExec, SYSTEMTIME* NewDate, \
                                                int CountryId, int Runtime, int GlobalMinutes, wchar_t* BufferOut);

 extern "C" _stdcall __declspec(dllimport) int WLCustomGenLicenseTextKeyEx(const char* UserName, const char* Organization,\
                                                const char* CustomData, const char* MachineID, sLicenseFeatures* LicenseFeatures, char* BufferOut);

 extern "C" _stdcall __declspec(dllimport) int WLCustomGenLicenseTextKeyExW(const wchar_t* UserName, const wchar_t* Organization,\
                                                const wchar_t* CustomData, const wchar_t* MachineID, sLicenseFeatures* LicenseFeatures, wchar_t* BufferOut);

 extern "C" _stdcall __declspec(dllimport) int WLCustomGenLicenseSmartKey(char* UserName, char* Organization,\
                                                char* CustomData, char* MachineID, int NumDays, int NumExec, SYSTEMTIME* NewDate, \
                                                char* BufferOut);

 extern "C" _stdcall __declspec(dllimport) int WLCustomGenLicenseSmartKeyW(const wchar_t* UserName, const wchar_t* Organization,\
                                                const wchar_t* CustomData, const wchar_t* MachineID, int NumDays, int NumExec, SYSTEMTIME* NewDate, \
                                                char* BufferOut);

 extern "C" _stdcall __declspec(dllimport) int WLCustomGenLicenseDynSmartKey(const char* UserName, const char* Organization,\
                                                const char* CustomData, const char* MachineID, sLicenseFeatures* LicenseFeatures, char* BufferOut);

 extern "C" _stdcall __declspec(dllimport) int WLCustomGenLicenseDynSmartKeyW(const wchar_t* UserName, const wchar_t* Organization,\
                                                const wchar_t* CustomData, const wchar_t* MachineID, sLicenseFeatures* LicenseFeatures, wchar_t* BufferOut);

 
 #else

 extern "C" int _stdcall GenerateTrialExtensionKey(int Level, int NumDays, int NumExec,\
                                                SYSTEMTIME* NewDate, int NumMinutes, int TimeRuntime,\
                                                char* BufferOut);

 extern "C" int _stdcall WLCustomGenTrialExtensionFileKey(int Level,\
                                            int NumDays, int NumExec, SYSTEMTIME* NewDate, int NumMinutes,\
                                            int TimeRuntime, char* BufferOut);

 extern "C" int _stdcall WLCustomGenTrialExtensionRegistryKey(int Level,\
                          int NumDays, int NumExec, SYSTEMTIME* NewDate, int NumMinutes,\
                          int TimeRuntime, char* pKeyName, char* pKeyValueName, char* BufferOut);

 extern "C" int _stdcall WLCustomGenPassword(char* Name, char* BufferOut);

 extern "C" int _stdcall WLCustomGenLicenseFileKey(char* UserName, char* Organization,\
                          char* CustomData, char* MachineID, int NumDays, int NumExec, SYSTEMTIME* NewDate, \
                          int CountryId, int Runtime, int GlobalMinutes, char* BufferOut);

 extern "C" int _stdcall WLCustomGenLicenseFileKeyW(const wchar_t* UserName, const wchar_t* Organization,\
                          const wchar_t* CustomData, const wchar_t* MachineID, int NumDays, int NumExec, SYSTEMTIME* NewDate, \
                          int CountryId, int Runtime, int GlobalMinutes, char* BufferOut);

 extern "C" int _stdcall WLCustomGenLicenseFileKeyEx(const char* UserName, const char* Organization,\
                                               const char* CustomData, const char* MachineID, sLicenseFeatures* LicenseFeatures, char* BufferOut);

 extern "C" int _stdcall WLCustomGenLicenseFileKeyExW(const wchar_t* UserName, const wchar_t* Organization,\
                                                const wchar_t* CustomData, const wchar_t* MachineID, sLicenseFeatures* LicenseFeatures, char* BufferOut);

 extern "C" int _stdcall WLCustomGenLicenseRegistryKey(char* UserName, char* Organization,\
                          char* CustomData, char* MachineID, int NumDays, int NumExec, SYSTEMTIME* NewDate, \
                          int CountryId, int Runtime, int GlobalMinutes, char* KeyName, char* KeyValueName, char* BufferOut);

 extern "C" int _stdcall WLCustomGenLicenseRegistryKeyW(const wchar_t* UserName, const wchar_t* Organization,\
                                                const wchar_t* CustomData, const wchar_t* MachineID, int NumDays, int NumExec, SYSTEMTIME* NewDate, \
                                                int CountryId, int Runtime, int GlobalMinutes, const wchar_t* KeyName, const wchar_t* KeyValueName, char* BufferOut);

 extern "C" int _stdcall WLCustomGenLicenseRegistryKeyEx(const char* UserName, const char* Organization,\
                          const char* CustomData, const char* MachineID, sLicenseFeatures* LicenseFeatures, char* KeyName, char* KeyValueName, char* BufferOut);

 extern "C" int _stdcall WLCustomGenLicenseRegistryKeyExW(const wchar_t* UserName, const wchar_t* Organization,\
                          const wchar_t* CustomData, const wchar_t* MachineID, sLicenseFeatures* LicenseFeatures, const wchar_t* KeyName, const wchar_t* KeyValueName, char* BufferOut);

 extern "C" int _stdcall WLCustomGenLicenseTextKey(char* UserName, char* Organization,\
                          char* CustomData, char* MachineID, int NumDays, int NumExec, SYSTEMTIME* NewDate, \
                          int CountryId, int Runtime, int GlobalMinutes, char* BufferOut);

 extern "C" int _stdcall WLCustomGenLicenseTextKeyW(const wchar_t* UserName, const wchar_t* Organization,\
                          const wchar_t* CustomData, const wchar_t* MachineID, int NumDays, int NumExec, SYSTEMTIME* NewDate, \
                          int CountryId, int Runtime, int GlobalMinutes, wchar_t* BufferOut);

 extern "C" int _stdcall WLCustomGenLicenseTextKeyEx(const char* UserName, const char* Organization,\
                          const char* CustomData, const char* MachineID, sLicenseFeatures* LicenseFeatures, char* BufferOut);

 extern "C" int _stdcall WLCustomGenLicenseTextKeyExW(const wchar_t* UserName, const wchar_t* Organization,\
                          const wchar_t* CustomData, const wchar_t* MachineID, sLicenseFeatures* LicenseFeatures, wchar_t* BufferOut);

 extern "C" int _stdcall WLCustomGenLicenseSmartKey(char* UserName, char* Organization,\
                          char* CustomData, char* MachineID, int NumDays, int NumExec, SYSTEMTIME* NewDate, \
                          char* BufferOut);

 extern "C" int _stdcall WLCustomGenLicenseSmartKeyW(wchar_t* UserName, wchar_t* Organization,\
                          wchar_t* CustomData, wchar_t* MachineID, int NumDays, int NumExec, SYSTEMTIME* NewDate, \
                          wchar_t* BufferOut);

 extern "C" int _stdcall WLCustomGenLicenseDynSmartKey(const char* UserName, const char* Organization,\
                                                const char* CustomData, const char* MachineID, sLicenseFeatures* LicenseFeatures, char* BufferOut);

 extern "C" int _stdcall WLCustomGenLicenseDynSmartKeyW(const wchar_t* UserName, const wchar_t* Organization,\
                                                const wchar_t* CustomData, const wchar_t* MachineID, sLicenseFeatures* LicenseFeatures, wchar_t* BufferOut);

 #endif

#endif