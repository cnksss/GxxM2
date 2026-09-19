// CodeGear C++Builder
// Copyright (c) 1995, 2015 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'kbmMemResEng.pas' rev: 30.00 (Windows)

#ifndef KbmmemresengHPP
#define KbmmemresengHPP

#pragma delphiheader begin
#pragma option push
#pragma option -w-      // All warnings off
#pragma option -Vx      // Zero-length empty class member 
#pragma pack(push,8)
#include <System.hpp>
#include <SysInit.hpp>

//-- user supplied -----------------------------------------------------------

namespace Kbmmemreseng
{
//-- forward type declarations -----------------------------------------------
//-- type declarations -------------------------------------------------------
//-- var, const, procedure ---------------------------------------------------
#define kbmMasterlinkErr L"Number of masterfields doesn't correspond to number of ind"\
	L"exfields."
#define kbmSelfRef L"Selfreferencing master/detail relations not allowed."
#define kbmFindNearestErr L"Can't do FindNearest on non sorted data."
#define kbminternalOpen1Err L"Fielddef "
#define kbminternalOpen2Err L" Datatype %d not supported."
#define kbmReadOnlyErr L"Field %s is read only"
#define kbmVarArrayErr L"Values variant array has invalid dimension count"
#define kbmVarReason1Err L"More fields than values"
#define kbmVarReason2Err L"There must be at least one field"
#define kbmBookmErr L"Bookmark %d not found."
#define kbmUnknownFieldErr1 L"Unknown field type (%s)"
#define kbmUnknownFieldErr2 L" in CSV file. (%s)"
#define kbmIndexErr L"Can't index on field %s"
#define kbmEditModeErr L"Dataset is not in edit mode."
#define kbmDatasetRemoveLockedErr L"Dataset being removed while locked."
#define kbmSetDatasetLockErr L"Dataset is locked and cant be changed."
#define kbmOutOfBookmarks L"Bookmark counter is out of range. Please close and reopen "\
	L"table."
#define kbmIndexNotExist L"Index %s does not exist"
#define kbmKeyFieldsChanged L"Could'nt perform operation since key fields changed."
#define kbmDupIndex L"Duplicate index value. Operation aborted."
#define kbmMissingNames L"Missing Name or FieldNames in IndexDef!"
#define kbmInvalidRecord L"Invalid record "
#define kbmTransactionVersioning L"Transactioning requires multiversion versioning."
#define kbmNoCurrentRecord L"No current record."
#define kbmCantAttachToSelf L"Cant attach memorytable to it self."
#define kbmCantAttachToSelf2 L"Cant attach to another table which itself is an attachment"\
	L"."
#define kbmUnknownOperator L"Unknown operator (%d)"
#define kbmUnknownFieldType L"Unknown fieldtype (%d)"
#define kbmOperatorNotSupported L"Operator not supported (%d)."
#define kbmSavingDeltasBinary L"Saving deltas is supported only in binary format."
#define kbmCantCheckpointAttached L"Cannot checkpoint attached table."
#define kbmDeltaHandlerAssign L"Delta handler is not assigned to any memorytables."
#define kbmOutOfRange L"Out of range (%d)"
#define kbmInvArgument L"Invalid argument."
#define kbmInvOptions L"Invalid options."
#define kbmTableMustBeClosed L"Table must be closed for this operation."
#define kbmChildrenAttached L"Children are attached to this table."
#define kbmIsAttached L"Table is attached to another table."
#define kbmInvalidLocale L"Invalid locale."
#define kbmInvFunction L"Invalid function name %s"
#define kbmInvMissParam L"Invalid or missing parameter for function %s"
#define kbmNoFormat L"No format specified."
#define kbmTooManyFieldDefs L"Too many fielddefs. Please raise KBM_MAX_FIELDS value."
#define kbmCannotMixAppendStructure L"Cannot both append and copy structure."
}	/* namespace Kbmmemreseng */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_KBMMEMRESENG)
using namespace Kbmmemreseng;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// KbmmemresengHPP
