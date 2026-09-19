  // 查找插件接口
  Plugin.HookGetIPLocal := GetProcAddress(Moudle, 'HookGetIPLocal');

  Plugin.HookEngineReadyToStart := GetProcAddress(Moudle, 'HookEngineReadyToStart');
  Plugin.HookEngineStartComplete := GetProcAddress(Moudle, 'HookEngineStartComplete');

  Plugin.HookEngineReloadComplete := GetProcAddress(Moudle, 'HookEngineReloadComplete');

  Plugin.HookLoadScriptFile := GetProcAddress(Moudle, 'HookLoadScriptFile');

  Plugin.HookDecryptScriptFile := GetProcAddress(Moudle, 'HookDecryptScriptFile');
  Plugin.HookDecryptScriptLine := GetProcAddress(Moudle, 'HookDecryptScriptLine');

  Plugin.HookNpcLoadConditionCmd := GetProcAddress(Moudle, 'HookNpcLoadConditionCmd');
  Plugin.HookNpcConditionProcess := GetProcAddress(Moudle, 'HookNpcConditionProcess');

  Plugin.HookNpcLoadActionCmd := GetProcAddress(Moudle, 'HookNpcLoadActionCmd');
  Plugin.HookNpcActionProcess := GetProcAddress(Moudle, 'HookNpcActionProcess');

  Plugin.HookUserSelect := GetProcAddress(Moudle, 'HookUserSelect');
  Plugin.HookUserCommand := GetProcAddress(Moudle, 'HookUserCommand');
  Plugin.HookGetVariableText := GetProcAddress(Moudle, 'HookGetVariableText');

  Plugin.HookBaseObjectCreate := GetProcAddress(Moudle, 'HookBaseObjectCreate');

  Plugin.HookBaseObjectRecalAbilBegin := GetProcAddress(Moudle, 'HookBaseObjectRecalAbilBegin');
  Plugin.HookBaseObjectRecalAbilEnd := GetProcAddress(Moudle, 'HookBaseObjectRecalAbilEnd');
  
  Plugin.HookBaseObjectRun := GetProcAddress(Moudle, 'HookBaseObjectRun');
  Plugin.HookBaseObjectProcessMsg := GetProcAddress(Moudle, 'HookBaseObjectProcessMsg');
  Plugin.HookBaseObjectStruck := GetProcAddress(Moudle, 'HookBaseObjectStruck');
  Plugin.HookBaseObjectMagicStruck := GetProcAddress(Moudle, 'HookBaseObjectMagicStruck');
  Plugin.HookBaseObjectAttack := GetProcAddress(Moudle, 'HookBaseObjectAttack');
  Plugin.HookBaseObjectMagicAttack := GetProcAddress(Moudle, 'HookBaseObjectMagicAttack');
  Plugin.HookBaseObjectDie := GetProcAddress(Moudle, 'HookBaseObjectDie');
  Plugin.HookBaseObjectMakeGhost := GetProcAddress(Moudle, 'HookBaseObjectMakeGhost');
  Plugin.HookBaseObjectFree := GetProcAddress(Moudle, 'HookBaseObjectFree');

  Plugin.HookPlayerCreate := GetProcAddress(Moudle, 'HookPlayerCreate');
  Plugin.HookPlayerLogin1 := GetProcAddress(Moudle, 'HookPlayerLogin1');
  Plugin.HookPlayerLogin2 := GetProcAddress(Moudle, 'HookPlayerLogin2');
  Plugin.HookPlayerLogin3 := GetProcAddress(Moudle, 'HookPlayerLogin3');
  Plugin.HookPlayerLogin4 := GetProcAddress(Moudle, 'HookPlayerLogin4');
  Plugin.HookPlayerRun := GetProcAddress(Moudle, 'HookPlayerRun');
  Plugin.HookPlayerViewRangeNewObject := GetProcAddress(Moudle, 'HookPlayerViewRangeNewObject');

  Plugin.HookPlayerProcessMsgBegin := GetProcAddress(Moudle, 'HookPlayerProcessMsgBegin');
  Plugin.HookPlayerProcessMsgEnd := GetProcAddress(Moudle, 'HookPlayerProcessMsgEnd');
  
  Plugin.HookPlayerFree := GetProcAddress(Moudle, 'HookPlayerFree');

  Plugin.HookDummyObjectRunBegin := GetProcAddress(Moudle, 'HookDummyObjectRunBegin');
  Plugin.HookDummyObjectRunEnd := GetProcAddress(Moudle, 'HookDummyObjectRunEnd');

  Plugin.HookHeroObjectCreate := GetProcAddress(Moudle, 'HookHeroObjectCreate');
  Plugin.HookHeroObjectFree := GetProcAddress(Moudle, 'HookHeroObjectFree');


