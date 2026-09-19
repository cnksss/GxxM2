  // 查找插件接口
  Plugin.HookGetIPLocal := BTMemoryGetProcAddress(MemMoudle, 'HookGetIPLocal');

  Plugin.HookEngineReadyToStart := BTMemoryGetProcAddress(MemMoudle, 'HookEngineReadyToStart');
  Plugin.HookEngineStartComplete := BTMemoryGetProcAddress(MemMoudle, 'HookEngineStartComplete');

  Plugin.HookEngineReloadComplete := BTMemoryGetProcAddress(MemMoudle, 'HookEngineReloadComplete');

  Plugin.HookLoadScriptFile := BTMemoryGetProcAddress(MemMoudle, 'HookLoadScriptFile');

  Plugin.HookDecryptScriptFile := BTMemoryGetProcAddress(MemMoudle, 'HookDecryptScriptFile');
  Plugin.HookDecryptScriptLine := BTMemoryGetProcAddress(MemMoudle, 'HookDecryptScriptLine');

  Plugin.HookNpcLoadConditionCmd := BTMemoryGetProcAddress(MemMoudle, 'HookNpcLoadConditionCmd');
  Plugin.HookNpcConditionProcess := BTMemoryGetProcAddress(MemMoudle, 'HookNpcConditionProcess');

  Plugin.HookNpcLoadActionCmd := BTMemoryGetProcAddress(MemMoudle, 'HookNpcLoadActionCmd');
  Plugin.HookNpcActionProcess := BTMemoryGetProcAddress(MemMoudle, 'HookNpcActionProcess');

  Plugin.HookUserSelect := BTMemoryGetProcAddress(MemMoudle, 'HookUserSelect');
  Plugin.HookUserCommand := BTMemoryGetProcAddress(MemMoudle, 'HookUserCommand');
  Plugin.HookGetVariableText := BTMemoryGetProcAddress(MemMoudle, 'HookGetVariableText');

  Plugin.HookBaseObjectCreate := BTMemoryGetProcAddress(MemMoudle, 'HookBaseObjectCreate');

  Plugin.HookBaseObjectRecalAbilBegin := BTMemoryGetProcAddress(MemMoudle, 'HookBaseObjectRecalAbilBegin');
  Plugin.HookBaseObjectRecalAbilEnd := BTMemoryGetProcAddress(MemMoudle, 'HookBaseObjectRecalAbilEnd');
  
  Plugin.HookBaseObjectRun := BTMemoryGetProcAddress(MemMoudle, 'HookBaseObjectRun');
  Plugin.HookBaseObjectProcessMsg := BTMemoryGetProcAddress(MemMoudle, 'HookBaseObjectProcessMsg');
  Plugin.HookBaseObjectStruck := BTMemoryGetProcAddress(MemMoudle, 'HookBaseObjectStruck');
  Plugin.HookBaseObjectMagicStruck := BTMemoryGetProcAddress(MemMoudle, 'HookBaseObjectMagicStruck');
  Plugin.HookBaseObjectAttack := BTMemoryGetProcAddress(MemMoudle, 'HookBaseObjectAttack');
  Plugin.HookBaseObjectMagicAttack := BTMemoryGetProcAddress(MemMoudle, 'HookBaseObjectMagicAttack');
  Plugin.HookBaseObjectDie := BTMemoryGetProcAddress(MemMoudle, 'HookBaseObjectDie');
  Plugin.HookBaseObjectMakeGhost := BTMemoryGetProcAddress(MemMoudle, 'HookBaseObjectMakeGhost');
  Plugin.HookBaseObjectFree := BTMemoryGetProcAddress(MemMoudle, 'HookBaseObjectFree');

  Plugin.HookPlayerCreate := BTMemoryGetProcAddress(MemMoudle, 'HookPlayerCreate');
  Plugin.HookPlayerLogin1 := BTMemoryGetProcAddress(MemMoudle, 'HookPlayerLogin1');
  Plugin.HookPlayerLogin2 := BTMemoryGetProcAddress(MemMoudle, 'HookPlayerLogin2');
  Plugin.HookPlayerLogin3 := BTMemoryGetProcAddress(MemMoudle, 'HookPlayerLogin3');
  Plugin.HookPlayerLogin4 := BTMemoryGetProcAddress(MemMoudle, 'HookPlayerLogin4');
  Plugin.HookPlayerRun := BTMemoryGetProcAddress(MemMoudle, 'HookPlayerRun');
  Plugin.HookPlayerViewRangeNewObject := BTMemoryGetProcAddress(MemMoudle, 'HookPlayerViewRangeNewObject');

  Plugin.HookPlayerProcessMsgBegin := BTMemoryGetProcAddress(MemMoudle, 'HookPlayerProcessMsgBegin');
  Plugin.HookPlayerProcessMsgEnd := BTMemoryGetProcAddress(MemMoudle, 'HookPlayerProcessMsgEnd');

  Plugin.HookPlayerFree := BTMemoryGetProcAddress(MemMoudle, 'HookPlayerFree');

  Plugin.HookDummyObjectRunBegin := BTMemoryGetProcAddress(MemMoudle, 'HookDummyObjectRunBegin');
  Plugin.HookDummyObjectRunEnd := BTMemoryGetProcAddress(MemMoudle, 'HookDummyObjectRunEnd');

  Plugin.HookHeroObjectCreate := BTMemoryGetProcAddress(MemMoudle, 'HookHeroObjectCreate');
  Plugin.HookHeroObjectFree := BTMemoryGetProcAddress(MemMoudle, 'HookHeroObjectFree');
