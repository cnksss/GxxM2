unit kbmMemResChs;

interface

const
  kbmMasterlinkErr = '主字段列表masterfields与索引字段列表indexfields数目不匹配。';
  kbmSelfRef = '不允许自引用主从关系';
  kbmFindNearestErr = '不可以在未排序的数据上使用FindNearest。';
  kbminternalOpen1Err = '字段定义 ';
  kbminternalOpen2Err = ' 数据类型 %d 不被支持。';
  kbmReadOnlyErr = '字段 %s 为只读';
  kbmVarArrayErr = '数值变体数组维数非法';
  kbmVarReason1Err = '字段个数比值个数多';
  kbmVarReason2Err = '至少需要一个字段';
  kbmBookmErr = '书签 %d 找不到。';
  kbmUnknownFieldErr1 = '位置字段类型(%s)';
  kbmUnknownFieldErr2 = ' 在 CSV 文件中。(%s)';
  kbmIndexErr = '不能在字段 %s 上索引';
  kbmEditModeErr = '数据集不在编辑模式。';
  kbmDatasetRemoveLockedErr = '数据集锁定后被移除。';
  kbmSetDatasetLockErr = '数据集锁定中不能被修改。';
  kbmOutOfBookmarks = '书签计数器越界，请关闭并重新打开表。';
  kbmIndexNotExist = '索引 %s 不存在';
  kbmKeyFieldsChanged = '不可进行此操作因主键字段已修改。';
  kbmDupIndex = '重复的索引值，操作终止。';
  kbmMissingNames = '索引定义IndexDef中缺少Name/FieldNames!';
  kbmInvalidRecord = '非法记录 ';
  kbmTransactionVersioning = '交易操作需要多版本模式支持。';
  kbmNoCurrentRecord = '无当前记录。';
  kbmCantAttachToSelf = '不能将自身附加到内存表。';
  kbmCantAttachToSelf2 = '不能附加到其他表因为当前表已经附加到其他表。';
  kbmUnknownOperator = '未知操作 (%d)';
  kbmUnknownFieldType = '位置字段类型 (%d)';
  kbmOperatorNotSupported = '操作不被支持(%d)。';
  kbmSavingDeltasBinary = '保存变动数据只能在二进制格式中使用。';
  kbmCantCheckpointAttached = '不能检查已附加表。';
  kbmDeltaHandlerAssign = '变动数据处理器未应用到任何内存表中。';
  kbmOutOfRange = '数值越界 (%d)';
  kbmInvArgument = '非法参数。';
  kbmInvOptions = '非法选项。';
  kbmTableMustBeClosed = '必须关闭表才能进行此操作。';
  kbmChildrenAttached = '已有子表附加到此表。';
  kbmIsAttached = '该表已被附加到其他表。';
  kbmInvalidLocale = '非法语言。';
  kbmInvFunction = '非法函数 %s';
  kbmInvMissParam = '函数 %s 缺少或非法参数';
  kbmNoFormat = '未指定格式。';
  kbmTooManyFieldDefs = '太多字段定义，请加大 KBM_MAX_FIELDS 值。';
  kbmCannotMixAppendStructure = '不能append的同时进行复制结构。';
implementation


end.

