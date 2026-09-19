                  //----------------------------------------------------
                  MF.Memory.Allow := _TMemory_Alloc;
                  MF.Memory.Free := _TMemory_Free;
                  MF.Memory.Realloc := _TMemory_Realloc;

                  //----------------------------------------------------

                  MF.List.Create := _TList_Create;
                  MF.List.Free := _TList_Free;
                  MF.List.Count := _TList_Count;
                  MF.List.Clear := _TList_Clear;
                  MF.List.Add := _TList_Add;
                  MF.List.Insert := _TList_Insert;
                  MF.List.Remove := _TList_Remove;
                  MF.List.Delete := _TList_Delete;
                  MF.List.GetItem := _TList_GetItem;
                  MF.List.SetItem := _TList_SetItem;
                  MF.List.IndexOf := _TList_IndexOf;
                  MF.List.Exchange := _TList_Exchange;
                  MF.List.CopyTo := _TList_CopyTo;

                  //----------------------------------------------------

                  MF.StringList.Create := _TStrList_Create;
                  MF.StringList.Free := _TStrList_Free;
                  MF.StringList.GetCaseSensitive := _TStrList_GetCaseSensitive;
                  MF.StringList.SetCaseSensitive := _TStrList_SetCaseSensitive;
                  MF.StringList.GetSorted := _TStrList_GetSorted;
                  MF.StringList.SetSorted := _TStrList_SetSorted;
                  MF.StringList.GetDuplicates := _TStrList_GetDuplicates;
                  MF.StringList.SetDuplicates := _TStrList_SetDuplicates;
                  MF.StringList.Count := _TStrList_Count;
                  MF.StringList.GetText := _TStrList_GetText;
                  MF.StringList.SetText := _TStrList_SetText;
                  MF.StringList.Add := _TStrList_Add;
                  MF.StringList.AddObject := _TStrList_AddObject;
                  MF.StringList.Insert := _TStrList_Insert;
                  MF.StringList.InsertObject := _TStrList_InsertObject;
                  MF.StringList.Remove := _TStrList_Remove;
                  MF.StringList.Delete := _TStrList_Delete;
                  MF.StringList.GetItem := _TStrList_GetItem;
                  MF.StringList.SetItem := _TStrList_SetItem;
                  MF.StringList.GetObject := _TStrList_GetObject;
                  MF.StringList.SetObject := _TStrList_SetObject;
                  MF.StringList.IndexOf := _TStrList_IndexOf;
                  MF.StringList.IndexOfObject := _TStrList_IndexOfObject;
                  MF.StringList.Find := _TStrList_Find;
                  MF.StringList.Exchange := _TStrList_Exchange;
                  MF.StringList.LoadFromFile := _TStrList_LoadFromFile;
                  MF.StringList.SaveToFile := _TStrList_SaveToFile;
                  MF.StringList.CopyTo := _TStrList_CopyTo;


                  //----------------------------------------------------

                  MF.MemStream.Create := _TMemStream_Create;
                  MF.MemStream.Free := _TMemStream_Free;
                  MF.MemStream.GetSize := _TMemStream_GetSize;
                  MF.MemStream.SetSize := _TMemStream_SetSize;
                  MF.MemStream.Clear := _TMemStream_Clear;
                  MF.MemStream.Read := _TMemStream_Read;
                  MF.MemStream.Write := _TMemStream_Write;
                  MF.MemStream.Seek := _TMemStream_Seek;
                  MF.MemStream.Memory := _TMemStream_Memory;
                  MF.MemStream.GetPosition := _TMemStream_GetPosition;
                  MF.MemStream.SetPosition := _TMemStream_SetPosition;
                  MF.MemStream.LoadFromFile := _TMemStream_LoadFromFile;
                  MF.MemStream.SaveToFile := _TMemStream_SaveToFile;

                  //----------------------------------------------------

                  MF.Menu.GetMainMenu := _TMenu_GetMainMenu;
                  MF.Menu.GetControlMenu := _TMenu_GetControlMenu;
                  MF.Menu.GetViewMenu := _TMenu_GetViewMenu;
                  MF.Menu.GetOptionMenu := _TMenu_GetOptionMenu;
                  MF.Menu.GetManagerMenu := _TMenu_GetManagerMenu;
                  MF.Menu.GetToolsMenu := _TMenu_GetToolsMenu;
                  MF.Menu.GetHelpMenu := _TMenu_GetHelpMenu;
                  MF.Menu.GetPluginMenu := _TMenu_GetPluginMenu;
                  MF.Menu.Count := _TMenu_Count;
                  MF.Menu.GetItems := _TMenu_GetItems;
                  MF.Menu.Add := _TMenu_Add;
                  MF.Menu.Insert := _TMenu_Insert;
                  MF.Menu.GetCaption := _TMenu_GetCaption;
                  MF.Menu.SetCaption := _TMenu_SetCaption;
                  MF.Menu.GetEnabled := _TMenu_GetEnabled;
                  MF.Menu.SetEnabled := _TMenu_SetEnabled;
                  MF.Menu.GetVisable := _TMenu_GetVisable;
                  MF.Menu.SetVisable := _TMenu_SetVisable;

                  MF.Menu.GetChecked := _TMenu_GetChecked;
                  MF.Menu.SetChecked := _TMenu_SetChecked;
                  MF.Menu.GetRadioItem := _TMenu_GetRadioItem;
                  MF.Menu.SetRadioItem := _TMenu_SetRadioItem;
                  MF.Menu.GetGroupIndex := _TMenu_GetGroupIndex;
                  MF.Menu.SetGroupIndex := _TMenu_SetGroupIndex;

                  MF.Menu.GetTag := _TMenu_GetTag;
                  MF.Menu.SetTag := _TMenu_SetTag;
                  
                  //----------------------------------------------------
                  MF.IniFile.Create := _TIniFile_Create;
                  MF.IniFile.Free := _TIniFile_Free;
                  MF.IniFile.SectionExists := _TIniFile_SectionExists;
                  MF.IniFile.ValueExists := _TIniFile_ValueExists;
                  MF.IniFile.ReadString := _TIniFile_ReadString;
                  MF.IniFile.WriteString := _TIniFile_WriteString;
                  MF.IniFile.ReadInteger := _TIniFile_ReadInteger;
                  MF.IniFile.WriteInteger := _TIniFile_WriteInteger;
                  MF.IniFile.ReadBool := _TIniFile_ReadBool;
                  MF.IniFile.WriteBool := _TIniFile_WriteBool;
                  
                  //----------------------------------------------------

                  MF.MagicACList.Count := _TMagicACList_Count;
                  MF.MagicACList.GetItem := _TMagicACList_GetItem;
                  MF.MagicACList.FindByMagIdx := _TMagicACList_FindByMagIdx;

                  //----------------------------------------------------

                  MF.MapManager.FindMap := _TMapManager_FindMap;
                  MF.MapManager.GetMapList := _TMapManager_GetMapList;

                  //----------------------------------------------------

                  MF.Envir.GetMapName := _TEnvir_GetMapName;
                  MF.Envir.GetMapDesc := _TEnvir_GetMapDesc;
                  MF.Envir.GetWidth := _TEnvir_GetWidth;
                  MF.Envir.GetHeight := _TEnvir_GetHeight;
                  MF.Envir.GetMinMap := _TEnvir_GetMinMap;
                  MF.Envir.IsMainMap := _TEnvir_IsMainMap;
                  MF.Envir.GetMainMapName := _TEnvir_GetMainMapName;

                  MF.Envir.IsMirrMap := _TEnvir_IsMirrMap;
                  MF.Envir.GetMirrMapCreateTick := _TEnvir_GetMirrMapCreateTick;
                  MF.Envir.GetMirrMapSurvivalTime := _TEnvir_GetMirrMapSurvivalTime;
                  MF.Envir.GetMirrMapExitToMap := _TEnvir_GetMirrMapExitToMap;
                  MF.Envir.GetMirrMapMinMap := _TEnvir_GetMirrMapMinMap;
                  MF.Envir.GetAlwaysShowTime := _TEnvir_GetAlwaysShowTime;

                  MF.Envir.IsFBMap := _TEnvir_IsFBMap;
                  MF.Envir.GetFBMapName := _TEnvir_GetFBMapName;
                  MF.Envir.GetFBEnterLimit := _TEnvir_GetFBEnterLimit;
                  MF.Envir.GetFBCreated := _TEnvir_GetFBCreated;
                  MF.Envir.GetFBCreateTime := _TEnvir_GetFBCreateTime;

                  MF.Envir.GetMapParam := _TEnvir_GetMapParam;
                  MF.Envir.GetMapParamValue := _TEnvir_GetMapParamValue;

                  MF.Envir.CheckCanMove := _TEnvir_CheckCanMove;

                  MF.Envir.IsValidObject := _TEnvir_IsValidObject;
                  MF.Envir.GetItemObjects := _TEnvir_GetItemObjects;
                  MF.Envir.GetBaseObjects := _TEnvir_GetBaseObjects;
                  MF.Envir.GetPlayObjects := _TEnvir_GetPlayObjects;

                  //----------------------------------------------------

                  MF.M2Engine.GetVersion := _TM2Engine_GetVersion;
                  MF.M2Engine.GetVersionInt := _TM2Engine_GetVersionInt;
                  MF.M2Engine.GetMainFormHandle := _TM2Engine_GetMainFormHandle;
                  MF.M2Engine.SetMainFormCaption := _TM2Engine_SetMainFormCaption;
                  MF.M2Engine.GetAppDir := _TM2Engine_GetAppDir;
                  MF.M2Engine.GetGlobalIniFile := _TM2Engine_GetGlobalIniFile;
                  MF.M2Engine.GetOtherFileDir := _TM2Engine_GetOtherFileDir;
                  MF.M2Engine.MainOutMessage := _TM2Engine_MainOutMessage;
                  MF.M2Engine.GetGlobalVarI := _TM2Engine_GetGlobalVarI;
                  MF.M2Engine.SetGlobalVarI := _TM2Engine_SetGlobalVarI;
                  MF.M2Engine.GetGlobalVarG := _TM2Engine_GetGlobalVarG;
                  MF.M2Engine.SetGlobalVarG := _TM2Engine_SetGlobalVarG;
                  MF.M2Engine.GetGlobalVarA := _TM2Engine_GetGlobalVarA;
                  MF.M2Engine.SetGlobalVarA := _TM2Engine_SetGlobalVarA;
                  MF.M2Engine.EncodeBuffer := _TM2Engine_EncodeBuffer;
                  MF.M2Engine.DecodeBuffer := _TM2Engine_DecodeBuffer;
                  MF.M2Engine.ZLibEncodeBuffer := _TM2Engine_ZLibEncodeBuffer;
                  MF.M2Engine.ZLibDecodeBuffer := _TM2Engine_ZLibDecodeBuffer;
                  MF.M2Engine.EncryptBuffer := _TM2Engine_EncryptBuffer;
                  MF.M2Engine.DecryptBuffer := _TM2Engine_DecryptBuffer;
                  MF.M2Engine.EncryptPassword := _TM2Engine_EncryptPassword;
                  MF.M2Engine.DecryptPassword := _TM2Engine_DecryptPassword;
                  MF.M2Engine.GetTakeOnPosition := _TM2Engine_GetTakeOnPosition;
                  MF.M2Engine.CheckBindType := _TM2Engine_CheckBindType;
                  MF.M2Engine.SetBindValue := _TM2Engine_SetBindValue;
                  MF.M2Engine.GetRGB := _TM2Engine_GetRGB;

                  //----------------------------------------------------

                  MF.BaseObject.GetChrName := _TBaseObject_GetChrName;
                  MF.BaseObject.SetChrName := _TBaseObject_SetChrName;
                  MF.BaseObject.RefShowName := _TBaseObject_RefShowName;
                  MF.BaseObject.RefNameColor := _TBaseObject_RefNameColor;

                  MF.BaseObject.GetGender := _TBaseObject_GetGender;
                  MF.BaseObject.SetGender := _TBaseObject_SetGender;

                  MF.BaseObject.GetJob := _TBaseObject_GetJob;
                  MF.BaseObject.SetJob := _TBaseObject_SetJob;

                  MF.BaseObject.GetHair := _TBaseObject_GetHair;
                  MF.BaseObject.SetHair := _TBaseObject_SetHair;

                  MF.BaseObject.GetEnvir := _TBaseObject_GetEnvir;
                  MF.BaseObject.GetMapName := _TBaseObject_GetMapName;
                  MF.BaseObject.GetCurrX := _TBaseObject_GetCurrX;
                  MF.BaseObject.GetCurrY := _TBaseObject_GetCurrY;
                  MF.BaseObject.GetDirection := _TBaseObject_GetDirection;

                  MF.BaseObject.GetHomeMap := _TBaseObject_GetHomeMap;
                  MF.BaseObject.GetHomeX := _TBaseObject_GetHomeX;
                  MF.BaseObject.GetHomeY := _TBaseObject_GetHomeY;
                  MF.BaseObject.GetPermission := _TBaseObject_GetPermission;
                  MF.BaseObject.SetPermission := _TBaseObject_SetPermission;
                  MF.BaseObject.GetDeath := _TBaseObject_GetDeath;
                  MF.BaseObject.GetDeathTick := _TBaseObject_GetDeathTick;
                  MF.BaseObject.GetGhost := _TBaseObject_GetGhost;
                  MF.BaseObject.GetGhostTick := _TBaseObject_GetGhostTick;
                  MF.BaseObject.MakeGhost := _TBaseObject_MakeGhost;
                  MF.BaseObject.ReAlive := _TBaseObject_ReAlive;


                  MF.BaseObject.GetRaceServer := _TBaseObject_GetRaceServer;
                  MF.BaseObject.GetAppr := _TBaseObject_GetAppr;
                  MF.BaseObject.GetRaceImg := _TBaseObject_GetRaceImg;

                  MF.BaseObject.GetCharStatus := _TBaseObject_GetCharStatus;
                  MF.BaseObject.SetCharStatus := _TBaseObject_SetCharStatus;
                  MF.BaseObject.StatusChanged := _TBaseObject_StatusChanged;

                  MF.BaseObject.GetHungerPoint := _TBaseObject_GetHungerPoint;
                  MF.BaseObject.SetHungerPoint := _TBaseObject_SetHungerPoint;

                  MF.BaseObject.IsNGMonster := _TBaseobject_IsNGMonster;
                  MF.BaseObject.IsDummyObject := _TBaseObject_IsDummyObject;

                  MF.BaseObject.GetViewRange := _TBaseObject_GetViewRange;
                  MF.BaseObject.SetViewRange := _TBaseObject_SetViewRange;
                  MF.BaseObject.GetAbility := _TBaseObject_GetAbility;

                  MF.BaseObject.GetWAbility := _TBaseObject_GetWAbility;
                  MF.BaseObject.SetWAbility := _TBaseObject_SetWAbility;

                  MF.BaseObject.GetSlaveList := _TBaseObject_GetSlaveList;
                  MF.BaseObject.GetMaster := _TBaseObject_GetMaster;
                  MF.BaseObject.GetMasterEx := _TBaseObject_GetMasterEx;

                  MF.BaseObject.GetSuperManMode := _TBaseObject_GetSuperManMode;
                  MF.BaseObject.SetSuperManMode := _TBaseObject_SetSuperManMode;

                  MF.BaseObject.GetAdminMode := _TBaseObject_GetAdminMode;
                  MF.BaseObject.SetAdminMode := _TBaseObject_SetAdminMode;

                  MF.BaseObject.GetTransparent := _TBaseObject_GetTransparent;
                  MF.BaseObject.SetTransparent := _TBaseObject_SetTransparent;

                  MF.BaseObject.GetObMode := _TBaseObject_GetObMode;
                  MF.BaseObject.SetObMode := _TBaseObject_SetObMode;

                  MF.BaseObject.GetStoneMode := _TBaseObject_GetStoneMode;
                  MF.BaseObject.SetStoneMode := _TBaseObject_SetStoneMode;

                  MF.BaseObject.GetStickMode := _TBaseObject_GetStickMode;
                  MF.BaseObject.SetStickMode := _TBaseObject_SetStickMode;

                  MF.BaseObject.GetIsAnimal := _TBaseObject_GetIsAnimal;
                  MF.BaseObject.SetIsAnimal := _TBaseObject_SetIsAnimal;

                  MF.BaseObject.GetIsNoItem := _TBaseObject_GetIsNoItem;
                  MF.BaseObject.SetIsNoItem := _TBaseObject_SetIsNoItem;

                  MF.BaseObject.GetCoolEye := _TBaseObject_GetCoolEye;
                  MF.BaseObject.SetCoolEye := _TBaseObject_SetCoolEye;

                  MF.BaseObject.GetHitPoint := _TBaseObject_GetHitPoint;
                  MF.BaseObject.SetHitPoint := _TBaseObject_SetHitPoint;

                  MF.BaseObject.GetSpeedPoint := _TBaseObject_GetSpeedPoint;
                  MF.BaseObject.SetSpeedPoint := _TBaseObject_SetSpeedPoint;

                  MF.BaseObject.GetHitSpeed := _TBaseObject_GetHitSpeed;
                  MF.BaseObject.SetHitSpeed := _TBaseObject_SetHitSpeed;

                  MF.BaseObject.GetWalkSpeed := _TBaseObject_GetWalkSpeed;
                  MF.BaseObject.SetWalkSpeed := _TBaseObject_SetWalkSpeed;

                  MF.BaseObject.GetHPRecover := _TBaseObject_GetHPRecover;
                  MF.BaseObject.SetHPRecover := _TBaseObject_SetHPRecover;

                  MF.BaseObject.GetMPRecover := _TBaseObject_GetMPRecover;
                  MF.BaseObject.SetMPRecover := _TBaseObject_SetMPRecover;

                  MF.BaseObject.GetPoisonRecover := _TBaseObject_GetPoisonRecover;
                  MF.BaseObject.SetPoisonRecover := _TBaseObject_SetPoisonRecover;

                  MF.BaseObject.GetAntiPoison := _TBaseObject_GetAntiPoison;
                  MF.BaseObject.SetAntiPoison := _TBaseObject_SetAntiPoison;

                  MF.BaseObject.GetAntiMagic := _TBaseObject_GetAntiMagic;
                  MF.BaseObject.SetAntiMagic := _TBaseObject_SetAntiMagic;

                  MF.BaseObject.GetLuck := _TBaseObject_GetLuck;
                  MF.BaseObject.SetLuck := _TBaseObject_SetLuck;

                  MF.BaseObject.GetAttatckMode := _TBaseObject_GetAttatckMode;
                  MF.BaseObject.SetAttatckMode := _TBaseObject_SetAttatckMode;

                  MF.BaseObject.GetNation := _TBaseObject_GetNation;
                  MF.BaseObject.SetNation := _TBaseObject_SetNation;

                  MF.BaseObject.GetNationaName := _TBaseObject_GetNationaName;

                  MF.BaseObject.GetGuild := _TBaseObject_GetGuild;
                  MF.BaseObject.GetGuildRankNo := _TBaseobject_GetGuildRankNo;
                  MF.BaseObject.GetGuildRankName := _TBaseobject_GetGuildRankName;
                  MF.BaseObject.IsGuildMaster := _TBaseObject_IsGuildMaster;

                  MF.BaseObject.GetHideMode := _TBaseObject_GetHideMode;
                  MF.BaseObject.SetHideMode := _TBaseObject_SetHideMode;
  
                  MF.BaseObject.GetIsParalysis := _TBaseObject_GetIsParalysis;
                  MF.BaseObject.SetIsParalysis := _TBaseObject_SetIsParalysis;
                  MF.BaseObject.GetParalysisRate := _TBaseObject_GetParalysisRate;
                  MF.BaseObject.SetParalysisRate := _TBaseObject_SetParalysisRate;

                  MF.BaseObject.GetIsMDParalysis := _TBaseObject_GetIsMDParalysis;
                  MF.BaseObject.SetIsMDParalysis := _TBaseObject_SetIsMDParalysis;
                  MF.BaseObject.GetMDParalysisRate := _TBaseObject_GetMDParalysisRate;
                  MF.BaseObject.SetMDParalysisRate := _TBaseObject_SetMDParalysisRate;

                  MF.BaseObject.GetIsFrozen := _TBaseObject_GetIsFrozen;
                  MF.BaseObject.SetIsFrozen := _TBaseObject_SetIsFrozen;
                  MF.BaseObject.GetFrozenRate := _TBaseObject_GetFrozenRate;
                  MF.BaseObject.SetFrozenRate := _TBaseObject_SetFrozenRate;

                  MF.BaseObject.GetIsCobwebWinding := _TBaseObject_GetIsCobwebWinding;
                  MF.BaseObject.SetIsCobwebWinding := _TBaseObject_SetIsCobwebWinding;
                  MF.BaseObject.GetCobwebWindingRate := _TBaseObject_GetCobwebWindingRate;
                  MF.BaseObject.SetCobwebWindingRate := _TBaseObject_SetCobwebWindingRate;

                  MF.BaseObject.GetUnParalysisValue := _TBaseObject_GetUnParalysisValue;
                  MF.BaseObject.SetUnParalysisValue := _TBaseObject_SetUnParalysisValue;
                  MF.BaseObject.GetIsUnParalysis := _TBaseObject_GetIsUnParalysis;

                  MF.BaseObject.GetUnMagicShieldValue := _TBaseObject_GetUnMagicShieldValue;
                  MF.BaseObject.SetUnMagicShieldValue := _TBaseObject_SetUnMagicShieldValue;
                  MF.BaseObject.GetIsUnMagicShield := _TBaseObject_GetIsUnMagicShield;

                  MF.BaseObject.GetUnRevivalValue := _TBaseObject_GetUnRevivalValue;
                  MF.BaseObject.SetUnRevivalValue := _TBaseObject_SetUnRevivalValue;
                  MF.BaseObject.GetIsUnRevival := _TBaseObject_GetIsUnRevival;

                  MF.BaseObject.GetUnPosionValue := _TBaseObject_GetUnPosionValue;
                  MF.BaseObject.SetUnPosionValue := _TBaseObject_SetUnPosionValue;
                  MF.BaseObject.GetIsUnPosion := _TBaseObject_GetIsUnPosion;

                  MF.BaseObject.GetUnTammingValue := _TBaseObject_GetUnTammingValue;
                  MF.BaseObject.SetUnTammingValue := _TBaseObject_SetUnTammingValue;
                  MF.BaseObject.GetIsUnTamming := _TBaseObject_GetIsUnTamming;

                  MF.BaseObject.GetUnFireCrossValue := _TBaseObject_GetUnFireCrossValue;
                  MF.BaseObject.SetUnFireCrossValue := _TBaseObject_SetUnFireCrossValue;
                  MF.BaseObject.GetIsUnFireCross := _TBaseObject_GetIsUnFireCross;

                  MF.BaseObject.GetUnFrozenValue := _TBaseObject_GetUnFrozenValue;
                  MF.BaseObject.SetUnFrozenValue := _TBaseObject_SetUnFrozenValue;
                  MF.BaseObject.GetIsUnFrozen := _TBaseObject_GetIsUnFrozen;

                  MF.BaseObject.GetUnCobwebWindingValue := _TBaseObject_GetUnCobwebWindingValue;
                  MF.BaseObject.SetUnCobwebWindingValue := _TBaseObject_SetUnCobwebWindingValue;
                  MF.BaseObject.GetIsUnCobwebWinding := _TBaseObject_GetIsUnCobwebWinding;


                  MF.BaseObject.GetTargetCret := _TBaseObject_GetTargetCret;
                  MF.BaseObject.SetTargetCret := _TBaseObject_SetTargetCret;
                  MF.BaseObject.DelTargetCreat := _TBaseObject_DelTargetCreat;
                  MF.BaseObject.GetLastHiter := _TBaseObject_GetLastHiter;
                  MF.BaseObject.GetExpHitter := _TBaseObject_GetExpHitter;
                  MF.BaseObject.GetPoisonHitter := _TBaseObject_GetPoisonHitter;
                  MF.BaseObject.GetPoseCreate := _TBaseObject_GetPoseCreate;

                  MF.BaseObject.IsProperTarget := _TBaseObject_IsProperTarget;
                  MF.BaseObject.IsProperFriend := _TBaseObject_IsProperFriend;

                  MF.BaseObject.TargetInRange := _TBaseObject_TargetInRange;

                  MF.BaseObject.SendMsg := _TBaseObject_SendMsg;
                  MF.BaseObject.SendDelayMsg := _TBaseObject_SendDelayMsg;
                  MF.BaseObject.SendRefMsg := _TBaseObject_SendRefMsg;
                  MF.BaseObject.SendUpdateMsg := _TBaseObject_SendUpdateMsg;
                  MF.BaseObject.SysMsg := _TBaseObject_SysMsg;

                  MF.BaseObject.GetBagItemList := _TBaseObject_GetBagItemList;
                  MF.BaseObject.IsEnoughBag := _TBaseObject_IsEnoughBag;
                  MF.BaseObject.IsEnoughBagEx := _TBaseObject_IsEnoughBagEx;
                  MF.BaseObject.AddItemToBag := _TBaseObject_AddItemToBag;
                  MF.BaseObject.DelBagItemByIndex := _TBaseObject_DelBagItemByIndex;
                  MF.BaseObject.DelBagItemByMakeIdx := _TBaseObject_DelBagItemByMakeIdx;
                  MF.BaseObject.DelBagItemByUserItem := _TBaseObject_DelBagItemByUserItem;

                  MF.BaseObject.IsInSafeZone := _TBaseObject_IsInSafeZone;
                  MF.BaseObject.IsPtInSafeZone := _TBaseObject_IsPtInSafeZone;

                  MF.BaseObject.RecalcLevelAbil := _TBaseObject_RecalcLevelAbil;
                  MF.BaseObject.RecalcAbil := _TBaseObject_RecalcAbil;
                  MF.BaseObject.RecalcBagWeight := _TBaseObject_RecalcBagWeight;

                  MF.BaseObject.GetLevelExp := _TBaseObject_GetLevelExp;

                  MF.BaseObject.HasLevelUp := _TBaseObject_HasLevelUp;
                  MF.BaseObject.TrainSkill := _TBaseObject_TrainSkill;

                  MF.BaseObject.CheckMagicLevelup := _TBaseObject_CheckMagicLevelup;
                  MF.BaseObject.MagicTranPointChanged := _TBaseObject_MagicTranPointChanged;

                  MF.BaseObject.DamageHealth := _TBaseObject_DamageHealth;
                  MF.BaseObject.DamageSpell := _TBaseObject_DamageSpell;
                  MF.BaseObject.IncHealthSpell := _TBaseObject_IncHealthSpell;
                  MF.BaseObject.HealthSpellChanged := _TBaseObject_HealthSpellChanged;

                  MF.BaseObject.FeatureChanged := _TBaseObject_FeatureChanged;
                  MF.BaseObject.WeightChanged := _TBaseObject_WeightChanged;

                  MF.BaseObject.GetHitStruckDamage := _TBaseObject_GetHitStruckDamage;
                  MF.BaseObject.GetMagStruckDamage := _TBaseObject_GetMagStruckDamage;

                  MF.BaseObject.GetActorIcon := _TBaseObject_GetActorIcon;
                  MF.BaseObject.SetActorIcon := _TBaseObject_SetActorIcon;

                  MF.BaseObject.RefUseIcons := _TBaseObject_RefUseIcons;
                  MF.BaseObject.RefUseEffects := _TBaseObject_RefUseEffects;

                  MF.BaseObject.SpaceMove := _TBaseObject_SpaceMove;
                  MF.BaseObject.MapRandomMove := _TBaseObject_MapRandomMove;

                  MF.BaseObject.CanMove := _TBaseObject_CanMove;
                  MF.BaseObject.CanRun := _TBaseObject_CanRun;
                  MF.BaseObject.TurnTo := _TBaseObject_TurnTo;
                  MF.BaseObject.WalkTo := _TBaseObject_WalkTo;
                  MF.BaseObject.RunTo := _TBaseObject_RunTo;

                  MF.BaseObject.PluginList := _TBaseObject_PluginList;

                  //----------------------------------------------------

                  MF.Smarter.GetMagicList := _TSmartObject_GetMagicList;

                  MF.Smarter.GetUseItem := _TSmartObject_GetUseItem;

                  MF.Smarter.GetJewelryBoxStatus := _TSmartObject_GetJewelryBoxStatus;
                  MF.Smarter.SetJewelryBoxStatus := _TSmartObject_SetJewelryBoxStatus;
                  MF.Smarter.GetJewelryItem := _TSmartObject_GetJewelryItem;

                  MF.Smarter.GetIsShowGodBless := _TSmartObject_GetIsShowGodBless;
                  MF.Smarter.SetIsShowGodBless := _TSmartObject_SetIsShowGodBless;

                  MF.Smarter.GetGodBlessItemsState := _TSmartObject_GetGodBlessItemsState;
                  MF.Smarter.SetGodBlessItemsState := _TSmartObject_SetGodBlessItemsState;

                  MF.Smarter.GetGodBlessItem := _TSmartObject_GetGodBlessItem;

                  MF.Smarter.GetFengHaoItems := _TSmartObject_GetFengHaoItems;
                  MF.Smarter.GetActiveFengHao := _TSmartObject_GetActiveFengHao;
                  MF.Smarter.SetActiveFengHao := _TSmartObject_SetActiveFengHao;
                  MF.Smarter.ActiveFengHaoChanged := _TSmartObject_ActiveFengHaoChanged;
                  MF.Smarter.DeleteFengHao := _TSmartObject_DeleteFengHao;
                  MF.Smarter.ClearFengHao := _TSmartObject_ClearFengHao;

                  MF.Smarter.GetMoveSpeed := _TSmartObject_GetMoveSpeed;
                  MF.Smarter.SetMoveSpeed := _TSmartObject_SetMoveSpeed;
                  MF.Smarter.GetAttackSpeed := _TSmartObject_GetAttackSpeed;
                  MF.Smarter.SetAttackSpeed := _TSmartObject_SetAttackSpeed;
                  MF.Smarter.GetSpellSpeed := _TSmartObject_GetSpellSpeed;
                  MF.Smarter.SetSpellSpeed := _TSmartObject_SetSpellSpeed;
                  MF.Smarter.RefGameSpeed := _TSmartObject_RefGameSpeed;
                  MF.Smarter.GetIsButch := _TSmartObject_GetIsButch;
                  MF.Smarter.SetIsButch := _TSmartObject_SetIsButch;

                  MF.Smarter.GetIsTrainingNG := _TSmartObject_GetIsTrainingNG;
                  MF.Smarter.SetIsTrainingNG := _TSmartObject_SetIsTrainingNG;

                  MF.Smarter.GetIsTrainingXF := _TSmartObject_GetIsTrainingXF;
                  MF.Smarter.SetIsTrainingXF := _TSmartObject_SetIsTrainingXF;

                  MF.Smarter.GetIsOpenLastContinuous := _TSmartObject_GetIsOpenLastContinuous;
                  MF.Smarter.SetIsOpenLastContinuous := _TSmartObject_SetIsOpenLastContinuous;

                  MF.Smarter.GetContinuousMagicOrder := _TSmartObject_GetContinuousMagicOrder;
                  MF.Smarter.SetContinuousMagicOrder := _TSmartObject_SetContinuousMagicOrder;

                  MF.Smarter.GetPKDieLostExp := _TSmartObject_GetPKDieLostExp;
                  MF.Smarter.SetPKDieLostExp := _TSmartObject_SetPKDieLostExp;

                  MF.Smarter.GetPKDieLostLevel := _TSmartObject_GetPKDieLostLevel;
                  MF.Smarter.SetPKDieLostLevel := _TSmartObject_SetPKDieLostLevel;

                  MF.Smarter.GetPKPoint := _TSmartObject_GetPKPoint;
                  MF.Smarter.SetPKPoint := _TSmartObject_SetPKPoint;
                  MF.Smarter.IncPKPoint := _TSmartObject_IncPKPoint;
                  MF.Smarter.DecPKPoint := _TSmartObject_DecPKPoint;

                  MF.Smarter.GetPKLevel := _TSmartObject_GetPKLevel;
                  MF.Smarter.SetPKLevel := _TSmartObject_SetPKLevel;

                  MF.Smarter.GetIsTeleport := _TSmartObject_GetIsTeleport;
                  MF.Smarter.SetIsTeleport := _TSmartObject_SetIsTeleport;

                  MF.Smarter.GetIsRevival := _TSmartObject_GetIsRevival;
                  MF.Smarter.SetIsRevival := _TSmartObject_SetIsRevival;

                  MF.Smarter.GetRevivalTime := _TSmartObject_GetRevivalTime;
                  MF.Smarter.SetRevivalTime := _TSmartObject_SetRevivalTime;

                  MF.Smarter.GetIsFlameRing := _TSmartObject_GetIsFlameRing;
                  MF.Smarter.SetIsFlameRing := _TSmartObject_SetIsFlameRing;

                  MF.Smarter.GetIsRecoveryRing := _TSmartObject_GetIsRecoveryRing;
                  MF.Smarter.SetIsRecoveryRing := _TSmartObject_SetIsRecoveryRing;

                  MF.Smarter.GetIsMagicShield := _TSmartObject_GetIsMagicShield;
                  MF.Smarter.SetIsMagicShield := _TSmartObject_SetIsMagicShield;

                  MF.Smarter.GetIsMuscleRing := _TSmartObject_GetIsMuscleRing;
                  MF.Smarter.SetIsMuscleRing := _TSmartObject_SetIsMuscleRing;

                  MF.Smarter.GetIsFastTrain := _TSmartObject_GetIsFastTrain;
                  MF.Smarter.SetIsFastTrain := _TSmartObject_SetIsFastTrain;

                  MF.Smarter.GetIsProbeNecklace := _TSmartObject_GetIsProbeNecklace;
                  MF.Smarter.SetIsProbeNecklace := _TSmartObject_SetIsProbeNecklace;

                  MF.Smarter.GetIsRecallSuite := _TSmartObject_GetIsRecallSuite;
                  MF.Smarter.SetIsRecallSuite := _TSmartObject_SetIsRecallSuite;

                  MF.Smarter.GetIsPirit := _TSmartObject_GetIsPirit;
                  MF.Smarter.SetIsPirit := _TSmartObject_SetIsPirit;

                  MF.Smarter.GetIsSupermanItem := _TSmartObject_GetIsSupermanItem;
                  MF.Smarter.SetIsSupermanItem := _TSmartObject_SetIsSupermanItem;

                  MF.Smarter.GetIsExpItem := _TSmartObject_GetIsExpItem;
                  MF.Smarter.SetIsExpItem := _TSmartObject_SetIsExpItem;

                  MF.Smarter.GetExpItemValue := _TSmartObject_GetExpItemValue;
                  MF.Smarter.SetExpItemValue := _TSmartObject_SetExpItemValue;

                  MF.Smarter.GetExpItemRate := _TSmartObject_GetExpItemRate;

                  MF.Smarter.GetIsPowerItem := _TSmartObject_GetIsPowerItem;
                  MF.Smarter.SetIsPowerItem := _TSmartObject_SetIsPowerItem;

                  MF.Smarter.GetPowerItemValue := _TSmartObject_GetPowerItemValue;
                  MF.Smarter.SetPowerItemValue := _TSmartObject_SetPowerItemValue;
                  
                  MF.Smarter.GetPowerItemRate := _TSmartObject_GetPowerItemRate;

                  MF.Smarter.GetIsGuildMove := _TSmartObject_GetIsGuildMove;
                  MF.Smarter.SetIsGuildMove := _TSmartObject_SetIsGuildMove;

                  MF.Smarter.GetIsAngryRing := _TSmartObject_GetIsAngryRing;
                  MF.Smarter.SetIsAngryRing := _TSmartObject_SetIsAngryRing;

                  MF.Smarter.GetIsStarRing := _TSmartObject_GetIsStarRing;
                  MF.Smarter.SetIsStarRing := _TSmartObject_SetIsStarRing;

                  MF.Smarter.GetIsACItem := _TSmartObject_GetIsACItem;
                  MF.Smarter.SetIsACItem := _TSmartObject_SetIsACItem;

                  MF.Smarter.GetACItemValue := _TSmartObject_GetACItemValue;
                  MF.Smarter.SetACItemValue := _TSmartObject_SetACItemValue;

                  MF.Smarter.GetIsMACItem := _TSmartObject_GetIsMACItem;
                  MF.Smarter.SetIsMACItem := _TSmartObject_SetIsMACItem;

                  MF.Smarter.GetMACItemValue := _TSmartObject_GetMACItemValue;
                  MF.Smarter.SetMACItemValue := _TSmartObject_SetMACItemValue;

                  MF.Smarter.GetIsNoDropItem := _TSmartObject_GetIsNoDropItem;
                  MF.Smarter.SetIsNoDropItem := _TSmartObject_SetIsNoDropItem;

                  MF.Smarter.GetIsNoDropUseItem := _TSmartObject_GetIsNoDropUseItem;
                  MF.Smarter.SetIsNoDropUseItem := _TSmartObject_SetIsNoDropUseItem;

                  MF.Smarter.GetNGAbility := _TSmartObject_GetNGAbility;
                  MF.Smarter.SetNGAbility := _TSmartObject_SetNGAbility;

                  MF.Smarter.GetAlcohol := _TSmartObject_GetAlcohol;
                  MF.Smarter.SetAlcohol := _TSmartObject_SetAlcohol;

                  MF.Smarter.RepairAllItem := _TSmartObject_RepairAllItem;
                  MF.Smarter.IsAllowUseMagic := _TSmartObject_IsAllowUseMagic;

                  MF.Smarter.SelectMagic := _TSmartObject_SelectMagic;

                  MF.Smarter.AttackTarget := _TSmartObject_AttackTarget;

                  //----------------------------------------------------

                  MF.Player.GetUserID := _TPlayObject_GetUserID;
                  MF.Player.GetIPAddr := _TPlayObject_GetIPAddr;
                  MF.Player.GetIPLocal := _TPlayObject_GetIPLocal;
                  MF.Player.GetMachineID := _TPlayObject_GetMachineID;
                  MF.Player.GetIsReadyRun := _TPlayObject_GetIsReadyRun;
                  MF.Player.GetLogonTime := _TPlayObject_GetLogonTime;

                  MF.Player.GetSoftVerDate := _TPlayObject_GetSoftVerDate;
                  MF.Player.GetClientType := _TPlayObject_GetClientType;

                  MF.Player.IsOldClient := _TPlayObject_IsOldClient;

                  MF.Player.GetScreenWidth := _TPlayObject_GetScreenWidth;
                  MF.Player.GetScreenHeight := _TPlayObject_GetScreenHeight;

                  MF.Player.GetClientViewRange := _TPlayObject_GetClientViewRange;

                  MF.Player.GetRelevel := _TPlayObject_GetRelevel;
                  MF.Player.SetRelevel := _TPlayObject_SetRelevel;

                  MF.Player.GetBonusPoint := _TPlayObject_GetBonusPoint;
                  MF.Player.SetBonusPoint := _TPlayObject_SetBonusPoint;
                  MF.Player.SendAdjustBonus := _TPlayObject_SendAdjustBonus;

                  MF.Player.GetHeroName := _TPlayObject_GetHeroName;
                  MF.Player.GetDeputyHeroName := _TPlayObject_GetDeputyHeroName;
                  MF.Player.GetDeputyHeroJob := _TPlayObject_GetDeputyHeroJob;
                  MF.Player.GetMyHero := _TPlayObject_GetMyHero;

                  MF.Player.GetFixedHero := _TPlayObject_GetFixedHero;

                  MF.Player.ClientHeroLogOn := _TPlayObject_ClientHeroLogOn;

                  MF.Player.GetStorageHero := _TPlayObject_GetStorageHero;
                  MF.Player.GetStorageDeputyHero := _TPlayObject_GetStorageDeputyHero;

                  MF.Player.GetIsStorageOpen := _TPlayObject_GetIsStorageOpen;
                  MF.Player.SetIsStorageOpen := _TPlayObject_SetIsStorageOpen;

                  MF.Player.GetGold := _TPlayObject_GetGold;
                  MF.Player.SetGold := _TPlayObject_SetGold;
                  MF.Player.GetGoldMax := _TPlayObject_GetGoldMax;

                  MF.Player.IncGold := _TPlayObject_IncGold;
                  MF.Player.DecGold := _TPlayObject_DecGold;
                  MF.Player.GoldChanged := _TPlayObject_GoldChanged;

                  MF.Player.GetGameGold := _TPlayObject_GetGameGold;
                  MF.Player.SetGameGold := _TPlayObject_SetGameGold;
                  MF.Player.IncGameGold := _TPlayObject_IncGameGold;
                  MF.Player.DecGameGold := _TPlayObject_DecGameGold;
                  MF.Player.GameGoldChanged := _TPlayObject_GameGoldChanged;

                  MF.Player.GetGamePoint := _TPlayObject_GetGamePoint;
                  MF.Player.SetGamePoint := _TPlayObject_SetGamePoint;
                  MF.Player.IncGamePoint := _TPlayObject_IncGamePoint;
                  MF.Player.DecGamePoint := _TPlayObject_DecGamePoint;

                  MF.Player.GetGameDiamond := _TPlayObject_GetGameDiamond;
                  MF.Player.SetGameDiamond := _TPlayObject_SetGameDiamond;
                  MF.Player.IncGameDiamond := _TPlayObject_IncGameDiamond;
                  MF.Player.DecGameDiamond := _TPlayObject_DecGameDiamond;
                  MF.Player.NewGamePointChanged := _TPlayObject_NewGamePointChanged;

                  MF.Player.GetGameGird := _TPlayObject_GetGameGird;
                  MF.Player.SetGameGird := _TPlayObject_SetGameGird;
                  MF.Player.IncGameGird := _TPlayObject_IncGameGird;
                  MF.Player.DecGameGird := _TPlayObject_DecGameGird;

                  MF.Player.GetGameGoldEx := _TPlayObject_GetGameGoldEx;
                  MF.Player.SetGameGoldEx := _TPlayObject_SetGameGoldEx;

                  MF.Player.GetGameGlory := _TPlayObject_GetGameGlory;
                  MF.Player.SetGameGlory := _TPlayObject_SetGameGlory;
                  MF.Player.IncGameGlory := _TPlayObject_IncGameGlory;
                  MF.Player.DecGameGlory := _TPlayObject_DecGameGlory;
                  MF.Player.GameGloryChanged := _TPlayObject_GameGloryChanged;

                  MF.Player.GetPayMentPoint := _TPlayObject_GetPayMentPoint;
                  MF.Player.SetPayMentPoint := _TPlayObject_SetPayMentPoint;

                  MF.Player.GetMemberType := _TPlayObject_GetMemberType;
                  MF.Player.SetMemberType := _TPlayObject_SetMemberType;
                  MF.Player.GetMemberLevel := _TPlayObject_GetMemberLevel;
                  MF.Player.SetMemberLevel := _TPlayObject_SetMemberLevel;

                  MF.Player.GetContribution := _TPlayObject_GetContribution;
                  MF.Player.SetContribution := _TPlayObject_SetContribution;

                  MF.Player.IncExp := _TPlayObejct_IncExp;
                  MF.Player.SendExpChanged := _TPlayObject_SendExpChanged;

                  MF.Player.IncExpNG := _TPlayObject_IncExpNG;
                  MF.Player.SendExpNGChanged := _TPlayObject_SendExpNGChanged;

                  MF.Player.IncBeadExp := _TPlayObject_IncBeadExp;

                  MF.Player.GetVarP := _TPlayObject_GetVarP;
                  MF.Player.SetVarP := _TPlayObject_SetVarP;

                  MF.Player.GetVarM := _TPlayObject_GetVarM;
                  MF.Player.SetVarM := _TPlayObject_SetVarM;

                  MF.Player.GetVarD := _TPlayObject_GetVarD;
                  MF.Player.SetVarD := _TPlayObject_SetVarD;

                  MF.Player.GetVarU := _TPlayObject_GetVarU;
                  MF.Player.SetVarU := _TPlayObject_SetVarU;

                  MF.Player.GetVarT := _TPlayObject_GetVarT;
                  MF.Player.SetVarT := _TPlayObject_SetVarT;

                  MF.Player.GetVarN := _TPlayObject_GetVarN;
                  MF.Player.SetVarN := _TPlayObject_SetVarN;

                  MF.Player.GetVarS := _TPlayObject_GetVarS;
                  MF.Player.SetVarS := _TPlayObject_SetVarS;
    
                  MF.Player.GetDynamicVarList := _TPlayObject_GetDynamicVarList;

                  MF.Player.GetQuestFlagStatus := _TPlayObject_GetQuestFlagStatus;
                  MF.Player.SetQuestFlagStatus := _TPlayObject_SetQuestFlagStatus;

                  MF.Player.IsOffLine := _TPlayObject_IsOffLine;

                  MF.Player.IsMaster := _TPlayObject_IsMaster;
                  MF.Player.GetMasterName := _TPlayObject_GetMasterName;
                  MF.Player.GetMasterHuman := _TPlayObject_GetMasterHuman;

                  MF.Player.GetApprenticeNO := _TPlayObject_GetApprenticeNO;
                  MF.Player.GetOnlineApprenticeList := _TPlayObject_GetOnlineApprenticeList;
                  MF.Player.GetAllApprenticeList := _TPlayObject_GetAllApprenticeList;

                  MF.Player.GetDearName := _TPlayObject_GetDearName;
                  MF.Player.GetDearHuman := _TPlayObject_GetDearHuman;
                  MF.Player.GetMarryCount := _TPlayObject_GetMarryCount;

                  MF.Player.GetGroupOwner := _TPlayObject_GetGroupOwner;
                  MF.Player.GetGroupMembers := _TPlayObject_GetGroupMembers;

                  MF.Player.GetIsLockLogin := _TPlayObject_GetIsLockLogin;
                  MF.Player.SetIsLockLogin := _TPlayObject_SetIsLockLogin;

                  MF.Player.GetIsAllowGroup := _TPlayObject_GetIsAllowGroup;
                  MF.Player.SetIsAllowGroup := _TPlayObject_SetIsAllowGroup;

                  MF.Player.GetIsAllowGroupReCall := _TPlayObject_GetIsAllowGroupReCall;
                  MF.Player.SetIsAllowGroupReCall := _TPlayObject_SetIsAllowGroupReCall;

                  MF.Player.GetIsAllowGuildReCall := _TPlayObject_GetIsAllowGuildReCall;
                  MF.Player.SetIsAllowGuildReCall := _TPlayObject_SetIsAllowGuildReCall;

                  MF.Player.GetIsAllowTrading := _TPlayObject_GetIsAllowTrading;
                  MF.Player.SetIsAllowTrading := _TPlayObject_SetIsAllowTrading;

                  MF.Player.GetIsDisableInviteHorseRiding := _TPlayObject_GetIsDisableInviteHorseRiding;
                  MF.Player.SetIsDisableInviteHorseRiding := _TPlayObject_SetIsDisableInviteHorseRiding;

                  MF.Player.GetIsGameGoldTrading := _TPlayObject_GetIsGameGoldTrading;
                  MF.Player.SetIsGameGoldTrading := _TPlayObject_SetIsGameGoldTrading;

                  MF.Player.GetIsNewServer := _TPlayObject_GetIsNewServer;

                  MF.Player.GetIsFilterGlobalDropItemMsg := _TPlayObject_GetIsFilterGlobalDropItemMsg;
                  MF.Player.SetIsFilterGlobalDropItemMsg := _TPlayObject_SetIsFilterGlobalDropItemMsg;

                  MF.Player.GetIsFilterGlobalCenterMsg := _TPlayObject_GetIsFilterGlobalCenterMsg;
                  MF.Player.SetIsFilterGlobalCenterMsg := _TPlayObject_SetIsFilterGlobalCenterMsg;

                  MF.Player.GetIsFilterGolbalSendMsg := _TPlayObject_GetIsFilterGolbalSendMsg;
                  MF.Player.SetIsFilterGolbalSendMsg := _TPlayObject_SetIsFilterGolbalSendMsg;

                  MF.Player.GetIsPleaseDrink := _TPlayObject_GetIsPleaseDrink;

                  MF.Player.GetIsDrinkWineQuality := _TPlayObject_GetIsDrinkWineQuality;
                  MF.Player.SetIsDrinkWineQuality := _TPlayObject_SetIsDrinkWineQuality;

                  MF.Player.GetIsDrinkWineAlcohol := _TPlayObject_GetIsDrinkWineAlcohol;
                  MF.Player.SetIsDrinkWineAlcohol := _TPlayObject_SetIsDrinkWineAlcohol;

                  MF.Player.GetIsDrinkWineDrunk := _TPlayObject_GetIsDrinkWineDrunk;
                  MF.Player.SetIsDrinkWineDrunk := _TPlayObject_SetIsDrinkWineDrunk;

                  MF.Player.MoveToHome := _TPlayObject_MoveToHome;
                  MF.Player.MoveRandomToHome := _TPlayObject_MoveRandomToHome;

                  MF.Player.SendSocket := _TPlayObject_SendSocket;
                  MF.Player.SendDefMessage := _TPlayObject_SendDefMessage;
                  MF.Player.SendMoveMsg := _TPlayObject_SendMoveMsg;
                  MF.Player.SendCenterMsg := _TPlayObject_SendCenterMsg;
                  MF.Player.SendTopBroadCastMsg := _TPlayObject_SendTopBroadCastMsg;

                  MF.Player.CheckTakeOnItems := _TPlayObject_CheckTakeOnItems;
                  MF.Player.ProcessUseItemSkill := _TPlayObject_ProcessUseItemSkill;

                  MF.Player.SendUseItems := _TPlayObject_SendUseItems;
                  MF.Player.SendAddItem := _TPlayObject_SendAddItem;
                  MF.Player.SendDelItemList := _TPlayObject_SendDelItemList;
                  MF.Player.SendDelItem := _TPlayObject_SendDelItem;
                  MF.Player.SendUpdateItem := _TPlayObject_SendUpdateItem;
                  MF.Player.SendItemDuraChange := _TPlayObject_SendItemDuraChange;

                  MF.Player.SendBagItems := _TPlayObject_SendBagItems;

                  MF.Player.SendJewelryBoxItems := _TPlayObject_SendJewelryBoxItems;

                  MF.Player.SendGodBlessItems := _TPlayObject_SendGodBlessItems;
                  MF.Player.SendOpenGodBlessItem := _TPlayObject_SendOpenGodBlessItem;
                  MF.Player.SendCloseGodBlessItem := _TPlayObject_SendCloseGodBlessItem;

                  MF.Player.SendUseMagics := _TPlayObject_SendUseMagics;
                  MF.Player.SendAddMagic := _TPlayObject_SendAddMagic;
                  MF.Player.SendDelMagic := _TPlayObject_SendDelMagic;

                  MF.Player.SendFengHaoItems := _TPlayObject_SendFengHaoItems;
                  MF.Player.SendAddFengHaoItem := _TPlayObject_SendAddFengHaoItem;
                  MF.Player.SendDelFengHaoItem := _TPlayObject_SendDelFengHaoItem;

                  MF.Player.SendSocketStatusFail := _TPlayObject_SendSocketStatusFail;

                  MF.Player.PlayEffect := _TPlayObject_PlayEffect;
                  MF.Player.IsAutoPlayGame := _TPlayObject_IsAutoPlayGame;
                  MF.Player.StartAutoPlayGame := _TPlayObject_StartAutoPlayGame;
                  MF.Player.StopAutoPlayGame := _TPlayObject_StopAutoPlayGame;

                  //----------------------------------------------------

                  MF.Dummy.IsStart := _TDummyObject_IsStart;
                  MF.Dummy.Start := _TDummyObject_Start;
                  MF.Dummy.Stop := _TDummyObject_Stop;


                  //----------------------------------------------------

                  MF.Hero.GetAttackMode := _THeroObject_GetAttackMode;
                  MF.Hero.SetAttackMode := _THeroObject_SetAttackMode;
                  MF.Hero.SetNextAttackMode := _THeroObject_SetNextAttackMode;

                  MF.Hero.GetBagCount := _THeroObject_GetBagCount;
                  MF.Hero.GetAngryValue := _THeroObject_GetAngryValue;

                  MF.Hero.GetLoyalPoint := _THeroObject_GetLoyalPoint;
                  MF.Hero.SetLoyalPoint := _THeroObject_SetLoyalPoint;
                  MF.Hero.SendLoyalPointChanged := _THeroObject_SendLoyalPointChanged;

                  MF.Hero.IsDeputy := _THeroObject_IsDeputy;
                  MF.Hero.GetMasterName := _THeroObject_GetMasterName;

                  MF.Hero.GetQuestFlagStatus := _THeroObject_GetQuestFlagStatus;
                  MF.Hero.SetQuestFlagStatus := _THeroObject_SetQuestFlagStatus;

                  MF.Hero.SendUseItems := _THeroObject_SendUseItems;
                  MF.Hero.SendBagItems := _THeroObject_SendBagItems;

                  MF.Hero.SendJewelryBoxItems := _THeroObject_SendJewelryBoxItems;

                  MF.Hero.SendGodBlessItems := _THeroObject_SendGodBlessItems;
                  MF.Hero.SendOpenGodBlessItem := _THeroObject_SendOpenGodBlessItem;
                  MF.Hero.SendCloseGodBlessItem := _THeroObject_SendCloseGodBlessItem;

                  MF.Hero.SendAddItem := _THeroObject_SendAddItem;
                  MF.Hero.SendDelItem := _THeroObject_SendDelItem;
                  MF.Hero.SendUpdateItem := _THeroObject_SendUpdateItem;
                  MF.Hero.SendItemDuraChange := _THeroObject_SendItemDuraChange;


                  MF.Hero.SendUseMagics := _THeroObject_SendUseMagics;
                  MF.Hero.SendAddMagic := _THeroObject_SendAddMagic;
                  MF.Hero.SendDelMagic := _THeroObject_SendDelMagic;

                  MF.Hero.FindGroupMagic := _THeroObject_FindGroupMagic;
                  MF.Hero.GetGroupMagicId := _THeroObject_GetGroupMagicId;

                  MF.Hero.SendFengHaoItems := _THeroObject_SendFengHaoItems;
                  MF.Hero.SendAddFengHaoItem := _THeroObject_SendAddFengHaoItem;
                  MF.Hero.SendDelFengHaoItem := _THeroObject_SendDelFengHaoItem;

                  MF.Hero.IncExp := _THeroObject_IncExp;
                  MF.Hero.IncExpNG := _THeroObject_IncExpNG;

                  MF.Hero.IsOldClient := _THeroObject_IsOldClient;

                  //----------------------------------------------------

                  MF.Npc.Create := _TNormNpc_Create;

                  MF.Npc.LoadNpcScript := _TNormNpc_LoadNpcScript;
                  MF.Npc.ClearScript := _TNormNpc_ClearScript;

                  MF.Npc.GetFilePath := _TNormNpc_GetFilePath;
                  MF.Npc.SetFilePath := _TNormNpc_SetFilePath;

                  MF.Npc.GetPath := _TNormNpc_GetPath;
                  MF.Npc.SetPath := _TNormNpc_SetPath;

                  MF.Npc.GetIsHide := _TNormNpc_GetIsHide;
                  MF.Npc.SetIsHide := _TNormNpc_SetIsHide;

                  MF.Npc.GetIsQuest := _TNormNpc_GetIsQuest;

                  MF.Npc.GetLineVariableText := _TNormNpc_GetLineVariableText;

                  MF.Npc.GotoLable := _TNormNpc_GotoLable;
                  MF.Npc.SendMsgToUser := _TNormNpc_SendMsgToUser;
                  MF.Npc.MessageBox := _TNormNpc_MessageBox;

                  MF.Npc.GetVarValue := _TNormNpc_GetVarValue;
                  MF.Npc.SetVarValue := _TNormNpc_SetVarValue;
                  MF.Npc.GetDynamicVarValue := _TNormNpc_GetDynamicVarValue;
                  MF.Npc.SetDynamicVarValue := _TNormNpc_SetDynamicVarValue;

                  //----------------------------------------------------

                  MF.UserEngine.GetPlayerList := _TUserEngine_GetPlayerList;
                  MF.UserEngine.GetPlayerByName := _TUserEngine_GetPlayerByName;
                  MF.UserEngine.GetPlayerByUserID := _TUserEngine_GetPlayerByUserID;
                  MF.UserEngine.GetPlayerByObject := _TUserEngine_GetPlayerByObject;

                  MF.UserEngine.GetOfflinePlayer := _TUserEngine_GetOfflinePlayer;
                  MF.UserEngine.KickPlayer := _TUserEngine_KickPlayer;

                  MF.UserEngine.GetHeroList := _TUserEngine_GetHeroList;
                  MF.UserEngine.GetHeroByName := _TUserEngine_GetHeroByName;
                  MF.UserEngine.KickHero := _TUserEngine_KickHero;

                  MF.UserEngine.GetMerchantList := _TUserEngine_GetMerchantList;
                  MF.UserEngine.GetCustomNpcConfigList := _TUserEngine_GetCustomNpcConfigList;

                  MF.UserEngine.GetQuestNPCList := _TUserEngine_GetQuestNPCList;
                  MF.UserEngine.GetManageNPC := _TUserEngine_GetManageNPC;
                  MF.UserEngine.GetFunctionNPC := _TUserEngine_GetFunctionNPC;
                  MF.UserEngine.GetRobotNPC := _TUserEngine_GetRobotNPC;
                  MF.UserEngine.MissionNPC := _TUserEngine_MissionNPC;

                  MF.UserEngine.FindMerchant := _TUserEngine_FindMerchant;
                  MF.UserEngine.FindMerchantByPos := _TUserEngine_FindMerchantByPos;

                  MF.UserEngine.FindQuestNPC := _TUserEngine_FindQuestNPC;

                  MF.UserEngine.GetMagicList := _TUserEngine_GetMagicList;
                  MF.UserEngine.GetCustomMagicConfigList := _TUserEngine_GetCustomMagicConfigList;
                  MF.UserEngine.GetMagicACList := _TUserEngine_GetMagicACList;

                  MF.UserEngine.FindMagicByName := _TUserEngine_FindMagicByName;
                  MF.UserEngine.FindMagicByIndex := _TUserEngine_FindMagicByIndex;
                  MF.UserEngine.FindMagicByNameEx := _TUserEngine_FindMagicByNameEx;
                  MF.UserEngine.FindMagicByIndexEx := _TUserEngine_FindMagicByIndexEx;

                  MF.UserEngine.FindHeroMagicByName := _TUserEngine_FindHeroMagicByName;
                  MF.UserEngine.FindHeroMagicByIndex := _TUserEngine_FindHeroMagicByIndex;
                  MF.UserEngine.FindHeroMagicByNameEx := _TUserEngine_FindHeroMagicByNameEx;
                  MF.UserEngine.FindHeroMagicByIndexEx := _TUserEngine_FindHeroMagicByIndexEx;


                  MF.UserEngine.GetStdItemList := _TUserEngine_GetStdItemList;
                  MF.UserEngine.GetStdItemByName := _TUserEngine_GetStdItemByName;
                  MF.UserEngine.GetStdItemByIndex := _TUserEngine_GetStdItemByIndex;
                  MF.UserEngine.GetStdItemName := _TUserEngine_GetStdItemName;
                  MF.UserEngine.GetStdItemIndex := _TUserEngine_GetStdItemIndex;

                  MF.UserEngine.MonsterList := _TUserEngine_MonsterList;

                  MF.UserEngine.SendBroadCastMsg := _TUserEngine_SendBroadCastMsg;
                  MF.UserEngine.SendBroadCastMsgExt := _TUserEngine_SendBroadCastMsgExt;
                  MF.UserEngine.SendTopBroadCastMsg := _TUserEngine_SendTopBroadCastMsg;
                  MF.UserEngine.SendMoveMsg := _TUserEngine_SendMoveMsg;
                  MF.UserEngine.SendCenterMsg := _TUserEngine_SendCenterMsg;
                  MF.UserEngine.SendNewLineMsg := _TUserEngine_SendNewLineMsg;
                  MF.UserEngine.SendSuperMoveMsg := _TUserEngine_SendSuperMoveMsg;
                  MF.UserEngine.SendSceneShake := _TUserEngine_SendSceneShake;

                  MF.UserEngine.CopyToUserItemFromName := _TUserEngine_CopyToUserItemFromName;
                  MF.UserEngine.CopyToUserItemFromItem := _TUserEngine_CopyToUserItemFromItem;

                  MF.UserEngine.RandomUpgradeItem := _TUserEngine_RandomUpgradeItem;
                  MF.UserEngine.RandomItemNewAbil := _TUserEngine_RandomItemNewAbil;
                  MF.UserEngine.GetUnknowItemValue := _TUserEngine_GetUnknowItemValue;

                  MF.UserEngine.GetAllDummyCount := _TUserEngine_GetAllDummyCount;
                  MF.UserEngine.GetMapDummyCount := _TUserEngine_GetMapDummyCount;
                  MF.UserEngine.GetOfflineCount := _TUserEngine_GetOfflineCount;
                  MF.UserEngine.GetRealPlayerCount := _TUserEngine_GetRealPlayerCount;

                  //----------------------------------------------------

                  MF.Guild.GetGuildName := _TGuild_GetGuildName;

                  MF.Guild.GetJoinJob := _TGuild_GetJoinJob;
                  MF.Guild.GetJoinLevel := _TGuild_GetJoinLevel;
                  MF.Guild.GetJoinMsg := _TGuild_GetJoinMsg;

                  MF.Guild.GetBuildPoint := _TGuild_GetBuildPoint;
                  MF.Guild.GetAurae := _TGuild_GetAurae;
                  MF.Guild.GetStability := _TGuild_GetStability;
                  MF.Guild.GetFlourishing := _TGuild_GetFlourishing;
                  MF.Guild.GetChiefItemCount := _TGuild_GetChiefItemCount;

                  MF.Guild.GetMemberCount := _TGuild_GetMemberCount;
                  MF.Guild.GetOnlineMemeberCount := _TGuild_GetOnlineMemeberCount;
                  MF.Guild.GetMasterCount := _TGuild_GetMasterCount;

                  MF.Guild.GetMaster := _TGuild_GetMaster;
                  MF.Guild.GetMasterName := _TGuild_GetMasterName;

                  MF.Guild.CheckMemberIsFull := _TGuild_CheckMemberIsFull;
                  MF.Guild.IsMemeber := _TGuild_IsMemeber;

                  MF.Guild.AddMember := _TGuild_AddMember;
                  MF.Guild.AddMemberEx := _TGuild_AddMemberEx;

                  MF.Guild.DelMemeber := _TGuild_DelMemeber;
                  MF.Guild.DelMemeberEx := _TGuild_DelMemeberEx;

                  MF.Guild.IsAllianceGuild := _TGuild_IsAllianceGuild;
                  MF.Guild.IsWarGuild := _TGuild_IsWarGuild;
                  MF.Guild.IsAttentionGuild := _TGuild_IsAttentionGuild;


                  MF.Guild.AddAlliance := _TGuild_AddAlliance;
                  MF.Guild.AddWarGuild := _TGuild_AddWarGuild;
                  MF.Guild.AddAttentionGuild := _TGuild_AddAttentionGuild;

                  MF.Guild.DelAllianceGuild := _TGuild_DelAllianceGuild;
                  MF.Guild.DelAttentionGuild := _TGuild_DelAttentionGuild;

                  MF.Guild.GetRandNameByName := _TGuild_GetRandNameByName;
                  MF.Guild.GetRandNameByPlayer := _TGuild_GetRandNameByPlayer;

                  MF.Guild.SendGuildMsg := _TGuild_SendGuildMsg;

                  //----------------------------------------------------

                  MF.GuildManager.FindGuild := _TGuildManager_FindGuild;
                  MF.GuildManager.GetPlayerGuild := _TGuildManager_GetPlayerGuild;
                  MF.GuildManager.AddGuild := _TGuildManager_AddGuild;
                  MF.GuildManager.DelGuild := _TGuildManager_DelGuild;

                  //----------------------------------------------------