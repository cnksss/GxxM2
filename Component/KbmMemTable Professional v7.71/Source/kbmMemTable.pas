unit kbmMemTable;

interface

{$include kbmMemTable.inc}

// kbmMemTable v. 7.70.00
// =========================================================================
// An inmemory temporary table.
//
// Copyright 1999-2014 Kim Bo Madsen/Components4Developers
// All rights reserved.
//
// PLEASE NOTE THAT THE LICENSE AGREEMENT HAS CHANGED!!! 26. Oct. 2011
//
{$REGION 'License'}
// LICENSE AGREEMENT
// -----------------
// Before using or evaluating kbmMemTable/kbmSQL you are required to read this license agreement in full,
// and agree to its full contents. By starting to use or evaluate kbmMemTable/kbmSQL you have automatically
// agreed on the full contents of this license.
//
// All files part of the package, except the files
// CocoAncestor.pas, COPYING.txt, COPYING-LIB.txt, LICENSE-2.0.txt and NOTICE.txt
// are the property of Kim Bo Madsen, Components4Developers and ideas and conceptuals coming from them
// may not be copied in part or in full.
// The file CocoAncestor.pas that is included in this package, is covered by one of the licenses
// GPL, LGPL or Apache License 2.0. Components4Developers have chosen to adhere to the Apache License 2.0 for
// the file CocoAncestor.pas.
// Check the folder (externallicenses) for the Apache License.
// All files (except CocoAncestor.pas and the text files in externallicenses)
// are covered by the kbmMemTable/kbmSQL license described in this document.
//
// To use the files within this product, you must have a developer license.
//
// Developer licenses fall in 4 categories:
//
// 1) Application specific freeware license. An application specific
//    freeware license can be obtained for strictly freeware noncommercial
//    applications, and in some situations for commercial charity purposes, but
//    only after _written_ agreement with Components4Developers.
//    Components4Developers retain the full right to decide if a specific
//    project/application require a commerical license or not.
//
//    A freeware license is restricted to a specific application/project and
//    to one specific developer only, unless other agreement has been made in
//    writing with Components4Developers.
//
// 2) Commercial licenses. Commercial licenses _must_ be obtained to use
//    these components or any part of kbmMemTable/kbmSQL.
//
//    Unless you have gotten a _written_ agreement with Components4Developers
//    about a freeware license, you _must_ purchase one or more commercial licenses.
//
//    A commercial licence is restricted to one and only one specific developer.
//    Thus if more or other developers use kbmMemTable/kbmSQL, additional developer
//    licenses must be obtained.
//
// 3) Free commercial licenses.
//    A free commercial license means that kbmMemTable/kbmSQL can be used for free in
//    non open source applications regardless if they are for commercial use or not,
//    provided that the applications do not compete against any C4D products at any time,
//    and that a notification is sent to sales@components4developers.com about what the
//    license is used for.
//    Components4Developers retain the right to use the name of the
//    companies/entities using the free commercial license for PR/reference purposes.
//    Free commercial licenses are only valid when they have been issued by
//    Components4Developers in writing for a specific project.
//
// 4) Free "CodeGear" licences.
//    A free "CodeGear" license means that kbmMemTable/kbmSQL or both were made available
//    for developers as a CodeGear Edition. The CodeGear Edition do not contain source for kbmMemTable/kbmSQL.
//    It is allowed to produce compiled end user products (commercial and non commercial) not competing with
//    Components4Developers products, for free and without runtime license requirements for the products that
//    are part of the CodeGear Edition package.
//    Components4Developers retain the right to use the name of the
//    companies/entities using the free "CodeGear" license for PR/reference purposes.
//
// A license is only valid for the specific SKU (eg. Standard Edition, Professional Edition, CodeGear Edition)
// for which the license originally was obtained. Its not legal to 'borrow' parts
// from other licensed SKU levels's to enhance a licensed SKU.
//
// You are not allowed to create developer libraries based on these components
// regardless of license choise without prior _written_ agreement with Components4Developers.
//
// You are not allowed to copy source code in parts or full into other
// libraries or applications or to distribute source in part or in full.
//
// You are not allowed to copy binary library code in parts or full to other developers.
// Other developers must obtain their own license from Components4Developers.
//
// You are not allowed to create competing products using ideas or source from
// the components/files which are part of kbmMemTable/kbmSQL or any knowledge gained from
// using/browsing the files/components which are part of the kbmMemTable/kbmSQL package.
//
// You are not allowed to discuss in public the algorithms used within the kbmMemTable/kbmSQL
// products/files.
//
// Copies of the kbmMemTable and/or kbmSQL package, not obtained _directly_ from Components4Developers
// or TurboMiddleware are illegal and therefore not permitted. kbmMemTable and/or kbmSQL must _not_ be
// distributed via any 3rdparty channels, except via Components4Developers or TurboMiddleware.
//
// No part of the kbmMemTable and/or kbmSQL package may be published in parts or in full unless
// a specific permit is granted by Components4Developers.
//
// When, and only when, a valid commercial or freeware license has been obtained,
// you are free to distribute your compiled and linked application without
// royalty requirements for kbmMemTable/kbmSQL. Freeware licenses can contain restrictions to
// the number of royalty free copies that can be distributed, unless
// specifically agreed in writing with Components4Developers.
//
// At the time of receiving the license for kbmMemTable/kbmSQL, a specified length of Service and Update
// subscription is included with the license. While the subscription is active,
// new updates can be fetched, after registration and requesting the license, from the
// MyC4D (www.myc4d.com) portal.
// The moment the subscription runs out, the right to newer free updates is automatically revoked.
// Additional periods of SAU can be purchased from Components4Developers.
// Extensions for SAU are always calculated from the end date of the previous SAU period.
//
// The kbmMemTable/kbmSQL package and files can never be subject to any GPL or LGPL or similar license
// except where specifically stated in this license agreement.
// Thus regardless of if you use this library in combination with GPL'ed or LGPL'ed or similar
// licensed code, the kbmMemTable/kbmSQL package including all its files, unless Components4Developers has
// specifically stated otherwise for specific files, remain being subject _only_ to the license
// stated in this document.
// Any 3rdparty licenses, regardless if they are viral or not, cannot overrule this license.
// In case kbmMemTable/kbmSQL is used in an application in combination with libraries which are subject
// to the GPL/LGPL or other viral license the developer/publisher is required to include a text
// file specifically stating that kbmMemTable/kbmSQL and all parts theirof are not subject to the
// GPL/LGPL or similar viral license. Further no parts of kbmMemTable/kbmSQL sourcecode may be
// published or included unless specifically prior written agreement with Components4Developers.
//
// If you find bugs (and optionally fix them) or alter the components, you must make
// that change/fix/report available to Components4Developers.
// Not doing so violates any obtained license.
//
// License violations, may void your licenses. This is determined case by case
// solely by Components4Developers.
//
// Obtaining a license by fraud, or other improper action or by giving misleading
// information will automatically void your license.
//
// Using kbmMemTable/kbmSQL for illegal purposes automatically voids your license.
//
// If a license becomes void, all licenses from Components4Developers for that
// specific entity (company/individual) are automatically void.
//
// When a license becomes void, you are _required_ to remove _ALL_ copies of
// the licenced product including all compiled and noncompiled files.
// You also MUST remove the previously licenced product from your applications immediately.
//
// Expiration of a SAU (Service and Update) subscription do not in it self void your license(s).
//
// Any ruling by Components4Developers to void a license is final
// and cannot be appealed to courts or other authorities.
//
{$ENDREGION}
{$REGION 'Disclaimer'}
// DISCLAIMER
// ----------
// By using these components/files or parts theirof you are accepting the full
// responsibility of the use. You are understanding that the author or
// Components4Developers can not be made responsible in any way for any
// problems or damage occuring using these components/files.
// You also recognize the author as the creator and owner of these components/files
// and agrees not to claim otherwise!
//
// Please forward corrected versions (source code ONLY!), comments,
// and emails saying you are using it for this or that project to:
//            kbm@components4developers.com
//
// Latest version can be found at:
//            http://www.components4developers.com
//
// Please see the comments in the start of the component sources for what’s
// new in this release.
{$ENDREGION}
{$REGION 'History'}
//
// History:
//
//1.00:	The first release. Was created due to a need for a component like this.
//                                                                    (15. Jan. 99)
//1.01:	The first update. Release 1.00 contained some bugs related to the ordering
//	    of records inserted and to bookmarks. Problems fixed.         (21. Jan. 99)

//1.02:	Fixed handling of NULL values. Added SaveToStream, SaveToFile,
//	    LoadFromStream and LoadFromFile. SaveToStream and SaveToFile is controlled
//	    by a flag telling if to save data, contents of calculated fields,
//	    contents of lookupfields and contents of non visible fields.
//	    Added an example application with Delphi 3 source code.       (26. Jan. 99)
//
//1.03: Claude Rieth from Computer Team sarl (clrieth@team.lu) came up with an
//      implementation of CommaText and made a validation check in _InternalInsert.
//      Because I allready have implemented the saveto.... functions, I decided
//      to implement Claude's idea using my own saveto.... functions. (27. Jan. 99)
//      I have decided to rename the component, because Claude let me know that
//      the RX library have a component with the same name as this.
//      Thus in the future the component will be named TkbmMemTable.
//      SaveToStream and LoadFromStream now set up date and decimal separator
//      temporary to make sure that the data saved can be loaded on another
//      installation with different date and decimal separator setups.
//      Added EmptyTable method to clear the contents of the memory table.
//
//1.04: Wagner ADP (wagner@cads-informatica.com.br) found a bug in the _internalinsert
//      procedure which he came up with a fix for.                     (4. Feb. 99)
//      Added support for the TDataset protected function findrecord.
//      Added support for CreateTable, DeleteTable.
//
//1.05: Charlie McKeegan from Task Software Limited (charlie@task.co.uk) found
//      a minor bug (and came up with a fix) in SetRecNo which mostly is
//      noticeable in a grid where the grid wont refresh when the record slider
//      is moved to the end of the recordset.                          (5. Feb. 99)
//      Changed SaveToStream to use the displayname of the field as the header
//      instead of the fieldname.
//
//1.06: Introduced a persistence switch and a reference to a file. If the
//      persistence switch is set, the file will be read aut. on table open,
//      and the contents of the table will be saved in the table on table close.
//
//1.07: Changed calculation of fieldofsets in InternalOpen to follow the fielddefs
//      instead of fields. It has importance when rearranging fields.
//      Because of this change the calculation of fieldoffsets into the buffer
//      has been changed too. These corrections was found to be needed to
//      support the new components tkbmPooledQuery and tkbmPooledStoredProc
//      (found in the package tkbmPooledConn)
//      which in turn handles pooled connections to a database. Very usefull
//      in f.ex a WWW application as they also makes the BDE function threadsafe,
//      and limits the concurrent connections to a database.
//
//1.08: Changed buffer algorithm in GetLine since the old one was faulty.
//      Problem was pointed out by: Markus Roessler@gmx.de
//
//1.09: Added LoadFromDataset, SaveToDataset, CreateTableAs,
//      BCD and BLOB support by me.
//      James Baile (James@orchiddata.demon.co.uk) pointed out a bug in GetRecNo
//      which could lead to an endless loop in cases with filtering. He also
//      provided code for sorting, which I have been rearranging a bit and
//      implemented.
//      Travis Diamond (tdiamond@airmail.net) pointed out a bug in GetWord where
//      null fields would be skipped.
//      Claudio Driussi (c.driussi@popmail.iol.it) send me version including
//      a sort routine, and Ive used some of he's ideas to implement a sorting
//      mechanism. Further he's code contained MoveRecords and MoveCurRec which
//      I decided to include in a modified form for Drag and Drop record
//      rearrangements.
//
//1.10: Support for Locate.                                             (17. May. 99)
//      Claudio Driussi (c.driussi@popmail.iol.it) came up with a fix for
//      GetRecNo. MoveRecord is now public.
//      Andrius Adamonis (andrius@prototechnika.lt) came up with fix for
//      call to validation routines in SetFieldData and support for
//      OldValue and NewValue in GetActiveRecordBuffer.
//
//1.11: Pascalis Bochoridis from SATO S.A Greece (pbohor@sato.gr)
//      Corrected bookmark behavior (substituted RecordNo from TRecInfo with unused Bookmark)
//      Corrected GetRecNo & SetRecNo  (Scrollbars now work ok at First and Last record)
//      Added LocateRecord, Locate, Lookup (I decided to use his Locate instead of my own).
//
//1.12: Added CloseBlob. Corrected destructor.                          (26. May. 99)
//      Corrected GetFieldData and SetFieldData. Could Result in overwritten
//      memory elsewhere because of use of DataSize instead of Size.
//      Pascalis Bochoridis from SATO S.A Greece (phohor@sato.gr) send me a corrected
//      LocateRecord which solves problems regarding multiple keyfields.
//      Thomas Bogenrieder (tbogenrieder@wuerzburg.netsurf.de) have suggested a faster
//      LoadFromStream version which is much faster, but loads the full stream contents into
//      memory before processing. I fixed a few bugs in it, rearranged it and decided
//      to leave it up to you what you want to use.
//      I suggest that if you need speed for smaller streams, use his method, else use mine.
//      You c an activate his method by uncommenting the define statement FAST_LOADFROMSTREAM
//      a few lines down.
//
//1.13  Corrected SetRecNo and MoveRecord.                               (31. May. 99)
//      By Pascalis Bochoridis from SATO S.A Greece (phohor@sato.gr)
//
//1.14  Respecting DefaultFields differently.                            (3. Jun. 99)
//      LoadFromDataset now includes a CopyStructure flag.
//      Supporting Master/Detail relations.
//      Now the Sort uses a new property for determining the fields to sort on,
//      which means its possible to sort on other fields than is used for
//      master/detail. Added SortOn(FieldNames:string) which is used for quick
//      addhoc sorting by just supplying the fieldnames as parameters to the
//      SortOn command.
//      Fixed memory leak in LoadFromDataset (forgot to free bookmark).
//      Added CopyFieldProperties parameter to LoadFromDataset and CreateTableAs which
//      if set to TRUE will copy f.ex. DisplayName, DisplayFormat, EditMask etc. from
//      the source.
//      Corrected OnValidate in SetFieldData. Was placed to late to have any impact.
//      Now checking for read only fields in SetFieldData.
//
//1.15  Totally rearranged LocateRecord to handle binary search when     (23. Jun. 99)
//      the data searched is sorted.
//      I was inspired by Paulo Conzon (paolo.conzon@smc.it) who send me a version with the Locate method
//      hacked for binary search. New methods PopulateRecord and PopulateField is used to create
//      a key record to search for.
//      Changed Sort and SortOn to accept sortoptions by a parameter instead of having a property.
//      The following sort options exists: mtsoCaseInsensitive and mtsoDescending.
//      mtsoPartialKey is used internally from LocateRecord where the TLocateOptions are remapped to
//      TkbmMemTableCompareOptions.
//
//1.16  Bug fixed in SaveToDataset. Close called where it shouldnt.      (19. Jul. 99)
//      Bug reported by Reinhard Kalinke (R_Kalinke@compuserve.com)
//      Fixed a few bugs + introduced Blob saving/loading in LoadFromStream/SaveToStream and thus
//      also LoadFromFile/SaveToFile. To save blob fields specify mtfSaveBlobs in save flags.
//      Full AutoInc implementation (max. 1 autoinc field/table).
//      These fixes and enhancements was contributed by Arséne von Wys (arsene@vonwyss.ch)
//      SetFieldData triggered the OnChange event to often. This is now fixed. Bug discovered by
//      Andrius Adamonis (andrius@prototechnika.lt).
//      Added mtfSkipRest save flag which if set will ONLY write out the fields specified by the rest of
//      the flags, while default operation is to put a marker to indicate a field skipped.
//      Usually mtfSkipRest shouldnt be specified if the stream should be reloaded by LoadFromStream later on.
//      But for generating Excel CSV files or other stuff which doesnt need to be reloaded,
//      mtfSkipRest can be valuable.
//      Greatly speeded up LoadFromStream (GetLine and GetWord).
//
//1.17  Supporting fieldtypes ftFixedChar,ftWideString.
//      Added runtime fieldtype checking for catching unsupported fields.
//      Raymond J. Schappe (rschappe@isthmus-ts.com) found a bug in CompareFields which he send a fix for.
//      Added a read only Version property.
//      The faster LoadFromStream added in 1.12 has now been removed due to the optimization of
//      the original LoadFromStream in 1.16. Tests shows no noticably performance difference anymore.
//      Inspired by Bruno Depero (bdepero@usa.net) which send me some methods for saving and
//      loading table definitions, I added mtfSaveDef to TkbmMemTableSaveFlag. If used the table
//      definition will be saved in a file. To copy another datasets definition, first do a CreateTableAs,
//      then SaveToFile/SaveToStream with mtfSaveDef.
//      Renamed TkbmSupportedFieldTypes to kbmSupportedFieldTypes and TkbmBlobTypes to kbmBlobTypes.
//      Generalized Delphi/BCB definitions.
//      Denis Tsyplakov (den@vrn.sterling.ru) suggested two new events: OnLoadField and OnLoadRecord
//      which have been implemented.
//      Added OnCompressSave and OnDecompressLoad for user defined (de)compression of SaveToStream,
//      LoadFromStream, SaveToFile and LoadFromFile.
//      Bruno Depero (bdepero@usa.net) inspired me to do this, since he send me a version including
//      Zip compression. But I like to generalize things and not to depend on any other specific
//      3. part library. Now its up to you which compression to use.
//      Added OnCompressBlobStream and OnDecompressBlobStream for (de)compression of inmemory blobs.
//      Added LZH compression to the Demo project by Bruno Depero (bdepero@usa.net).
//
//1.18  Changed SaveToStream and LoadFromStream to code special characters in string fields
//      (ftString,ftWideString,ftFixedChar,ftMemo,ftFmtMemo).
//      You may change this behaviour to the old way by setting kbmStringTypes to an empty set.
//      Fixed severe blob null bug. Blobs fields was sometimes reported IsNull=true even if
//      they had data in them.
//      Fixed a few minor bugs.
//
//1.19  Fixed a bug in CodedStringToString where SetLength was to long.             (10. Aug. 1999)
//      Bug reported by Del Piero (tomli@mail.tycoon.com.tw).
//      Fixed bug in LoadFromStream where DuringTableDef was wrongly initialized
//      to true. Should be false. Showed when a CSV file without definition was loaded.
//      Bug reported by Mr. Schmidt (ISAT.Schmidt@t-online.de)
//
//1.20  Marcelo Roberto Jimenez (mroberto@jaca.cetuc.puc-rio.br) reported a few bugs(23. Aug. 1999)
//      to do with empty dataset, which he also corrected (GetRecNo, FilterMasterDetail).
//      Furthermore he suggested and provided code for a Capacity property, which
//      can be used to preallocate room in the internal recordlist for a specific
//      minimum number of records.
//      Explicitly typecasted @ to PChar in several places to avoid problem about
//      people checking 'Typed @' in compile options.
//
//1.21  Corrected Locate on filtered recordssets.                                   (24. Aug. 1999)
//      Problem observed by Keith Blows (keithblo@woollyware.com)
//
//1.22  Corrected GetActiveRecord and added overridden SetBlockReadSize to be       (30. Aug. 1999)
//      compatible with D4/BCB4's TProvider functionality. The information and
//      code has been generously supplied by Jason Wharton (jwharton@ibobjects.com).
//      Paul Moorcroft (pmoor@netspace.net.au) added SaveToBinaryFile, LoadFromBinaryFile,
//      SaveToBinaryStream and LoadFromBinaryStream. They save/load the contents incl.
//      structure to/from the stream/file in a binary form which saves space and is
//      faster.
//      Added support for ftLargeInt (D4/D5/BCB4).
//
//1.23  Forgot to add defines regarding Delphi 5. I havnt got D5 yet, and thus      (12. Sep. 1999)
//      not tested this myself, but have relied on another person telling me that it
//      do work in D5. Let me know if it doesnt.
//      Added save flag mtfSaveFiltered to allow saving all records regardless if they are
//      filtered or not. Suggestion posed by Jose Mauro Teixeira Marinho (marinho@aquarius.ime.eb.br)
//      Added automatic resort when records are inserted/edited into a sorted table by
//      Jir?Hostinsk?(tes@pce.cz). The autosort is controlled by AutoSort flag which is
//      disabled by default. Furthermore at least one SortField must be defined for the auto
//      sort to be active.
//
//1.24  The D5 story is continuing. Removed the override keyword on BCDToCurr        (7. Oct. 1999)
//      and CurrToBCD for D5 only. Changed type of PersistentFile from String to TFileName.
//      Support for SetKey, GotoKey, FindKey inspired by sourcecode from Azuer (blue@nexmil.net) for
//      TkbmMemTable v. 1.09, but now utilizing the new internal search features.
//      Fixed bug with master/detail introduced in 1.21 or so.
//      Fixed old bug in GetRecNo which didnt know how to end a count on a filtered recordset.
//      Support for SetRange, SetRangeStart, SetRangeEnd, ApplyRange, CancelRange,
//      EditRangeStart, EditRangeEnd, FindNearest, EditKey.
//      Fixed several bugs to do with date, time and datetime fields. Now the internal
//      storage format is a TDateTimeRec.
//      Fixed problems when saving a CSV file on one machine and loading it on another
//      with different national setup. Now the following layout is allways used on
//      save and load (unless the flag mtfSaveInLocalFormat is specified on the SaveToStream/SaveToFile method,
//      in which case the local national setup is used for save. Beware that the fileformat will not be
//      portable between machines, but can be used for simply creating a Comma separated report for other
//      use):  DateSeparator:='/'  TimeSeparator:=':'  ThousandSeparator:=','  DecimalSeparator:='.'
//      ShortDateFormat:='dd/mm/yyyy' CurrencyString:='' CurrencyFormat:=0 NegCurrFormat:=1
//      Date problems reported by Craig Murphy (craig@isleofjura.demon.co.uk).
//      We are getting close to have a very complete component now !!!
//
//1.25  Added CompareBookmarks, BookmarkValid, InternalBookmarkValid by             (11. Oct. 1999)
//      Lars Søndergaard (ls@lunatronic.dk)
//
//1.26  In 1.24 I introduced a new keybuffer principle for performance and easyness.(14. Oct. 1999)
//      Unfortunately I forgot a few things to do with Master/Detail. They have now
//      been fixed. Problem reported by Dirk Carstensen (D.Carstensen@FH-Wolfenbuettel.DE)
//      He also translated all string ressources to German.
//      Simply define the localization type wanted further down and recompile TkbmMemTable.
//      Fixed AutoSort problem when AutoSort=true on first record insert.
//      Further more setting AutoSort=true on a nonsorted dataset will Result in an
//      automatic full sort on table open.
//      Problem reported by Carl (petrolia@inforamp.net)
//      Added events OnSave and OnLoad which are called by SaveToDataSet,
//      SaveToStream, SaveToBinaryStream, SaveToFile, SaveToBinaryFile,
//      LoadFromDataSet, LoadFromStream, LoadFromBinaryStream,
//      LoadFromFile, LoadFromBinaryFile. StorageType defines what type of save/load
//      is going on: mtstDataSet, mtstStream, mtstBinaryStream, mtstFile and mtstBinaryFile.
//      Stream specifies the stream used to save/load. Will be nil if StorageType is mtstDataSet.
//
//1.27  In 1.26 I unfortunately made 2 errors... Forgot to rename the german ressource file's (19. Oct. 1999)
//      unitname and made another autosort problem. Things are going a bit fast at the moment,
//      thus it is up to you all to test my changes :)
//      Well..well... the german ressourcefile's unitname is now correct.
//      AutoSort is now working as it should. Been checking it :)
//      And its pretty fast too. Tried with 100.000 records, almost immediately
//      on a PII 500Mhz.
//      Added autosort facilities to the demo project and posibility to change
//      number of records in sample data. Tried with 1 million records... and it works :)
//      although quicksort is not the optimal algorithm to use on a very unbalanced
//      large recordset. It seems to be fast enough for around 10.000 records.
//      (almost immediately on a PII 500Mhz.)
//      Published SortOptions, and added SortDefault to do a sort using the published
//      sortfields and options. Mind you that Sort(...) sets up new sortoptions.
//
//1.28  Added PersistentSaveOptions.                                                 (21. Oct. 1999)
//      Added PersistentSaveFormat either mtsfBinary or mtsfCSV.
//      Fixed some flaws regarding persistense in designmode which could lead to loss of
//      data in the persistent file and loss of field definitions.
//
//1.29  I. M. M. VATOPEDIOU (monh@vatopedi.thessal.singular.gr) found a bug in GetRecNo (22. Oct. 1999)
//      which he send a fix for.
//
//1.30  Fernando (tolentino@atalaia.com.br) send OldValue enhancements and thus introduced
//      InternalInsert, InternalEdit, InternalCancel procedures.
//      Furthermore he suggested to rename TkbmMemTable to TkbmCustomMemTable and descend TkbmMemTable from it.
//      I decided to follow his suggestion as to make it easier to design own memory table children.
//      Kanca (kanca@ibm.net) send me example on runtime creation of TkbmMemTable. The example
//      has been put in the demo project as a comment.
//      Holger Dors (dors@kittelberger.de) suggested a version of CompareBookmarks which guarantiees
//      values -1, 0 or 1 as Result. This has now been implemented.
//      Furthermore he retranslated an incorrect German translation for FindNearest.
//
//1.31  SetRecNo and GetRecNo has been analyzed carefully and rewritten to      (26. Oct. 1999)
//      reflect normal behaviour. Locate was broken in 1.29 because of the prev.
//      GetRecNo fix. Reported by Carl (petrolia@inforamp.net).
//      There have been significant speedups in insert record and delete record.
//      Now TkbmMemTable contains a componenteditor for D5. Source has been donated by
//      Albert Research (albertrs@redestb.es) and partly changed by me.
//      Ressourcestrings has been translated to French by John Knipper (knipjo@altavista.net)
//      Removed InternalInsert for D3. Problem reported by John Knipper.
//
//1.32  Fernando (tolentino@atalaia.com.br) sent Portuguese/Brasillian translation. (5. Nov. 1999)
//      Vasil (vasils@ru.ru) sent Russian translation.
//      Javier Tari Agullo (jtari@cyber.es) sent Spanish translation.
//      Tero Tilus (terotil@cc.jyu.fi) suggested to save the visibility flag of a field too
//      along with all the other fielddefinitions in the SaveToStream/LoadFromStream etc. methods.
//      I changed CreateTableAs format to:
//        procedure CreateTableAs(Source:TDataSet; CopyOptions:TkbmMemTableCopyTableOptions);
//      where copyoptions can be:
//        mtcpoStructure         - Copy structure from the source datasource.
//        mtcpoOnlyActiveFields  - Only copy structure of active fields in the source datasource.
//        mtcpoProperties        - Also copy field info like DisplayName etc. from the source datasource.
//        mtcpoLookup            - Also copy lookup definitions from the source datasource.
//        mtcpoCalculated        - Also copy calculated fielddefinitions from the source datasource.
//      or a combination of those values in square brackets [...].
//      Further LoadFromDataSet is now following the same syntax.
//      A new method CreateFieldAs has been appended used by CreateTableAs.
//      Lookup fields defined in the memorytable now works as expected.
//      Now the designer will show all types of database tables, not only STANDARD.
//      Changed CopyRecords to allow copying of calculated data, and not clearing out
//      lookup fields on destination.
//      Fixed autosort error reported by Walter Yu (walteryu@21cn.com).
//
//1.33  Fixed error in CopyRecords which would lead to wrongly clearing calculated fields  (23. Nov. 1999)
//      after they have correcly been set.
//      Fixed problem with autosort when inserting record before first.
//      Fixed exception problem with masterfields property during load. Problem
//      reported by Jose Luis Tirado (jltirado@jazzfree.com).
//      Fixed a few errors in the demo application.
//      Added Italian translation by Bruno Depero (bdepero@usa.net).
//
//1.34  Fixed Resort problem not setting FSorted=true. Problem reported by     (3. Dec. 1999)
//      Tim_Daborn@Compuserve.com.
//      Added Slovakian translation by Roman Olexa (systech@ba.telecom.sk)
//      Added Romanian translation by Sorin Pohontu (spohontu@assist.cccis.ro)
//      Fixed problem about FSortFieldList not being updated when new sortfields are defined.
//      The fix solves the AutoSort problem reported by Sorin Pohontu (spohontu@assist.cccis.ro)
//      Javier Tari Agullo (jtari@cyber.es) send a fix for Spanish translation.
//      Added threaded dataset controller (TkbmThreadDataSet).
//      Put it on a form, link the dataset property to a dataset.
//      When you need to use a dataset, do:
//
//      ds:=ThreadDataset.Lock;
//      try
//         ...
//      finally
//         ThreadDataset.Unlock;
//      end;
//
//1.35  LargeInt type handling changed. Now it will be read and saved          21. Dec. 1999
//      as a float, not as an integer. General fixes to do with LargeInt field
//      types. Bug reported by Fernando P. Nájera Cano (j.najera@cgac.es).
//      Fixed bug reported by Urs Wagner (Urs_Wagner%NEUE_BANKENSOFTWARE_AG@raiffeisen.ch)
//      where a field could be edited even if no Edit or Insert statement has been issued.
//      Jozef Bielik (jozef@gates96.com) suggested to reset FRecNo and FSorted when
//      last record is deleted in _InternalDelete. This has been implemented.
//
//1.36  Edison Mera Menéndez (edmera@yahoo.com) send fix for bug in autoupdate. 23. Dec. 1999
//      Further he send code for a faster resort based on binary search for
//      insertionpoint instead of the rather sequential one in the previous version.
//      In case of problems, the old code can be activated by defining ORIGINAL_RESORT
//      He also suggested and send code for an unified _InternalSearch which does the
//      job of selecting either sequential or binary search.
//      Brad - RightClick (brade@rightclick.com.au) suggested that autosort should be
//      disengaged during load operations. I agree and thus have implemented it.
//      Implemented that EmptyTable implecitely does a cancel in case the table
//      was in Edit mode. Suggested by Albert Research (albertrs@redestb.es).
//
//1.37  Claude Rieth from Computer Team sarl (clrieth@team.lu) suggested to be able to 3. Jan. 2000
//      specify save options for CommaText. Thus CommaTextOptions has been added.
//      Several people have been having trouble installing in BCB4. The reason is
//      that some unused 3.rd party libraries sneeked into the TkbmMemTable
//      BCB4 project file. It has been fixed.
//      Bookmark handling has been corrected. Problem reported by Dick Boogaers (d.boogaers@css.nl).
//      InternalLoadFromBinaryStream has been fixed with regards to loading a NULL date.
//      A date is considered NULL when the value of the date is 0.0 (1/1/1899).
//      Problem reported by Paul Moorcroft (pmoor@netspace.net.au)
//      Ohh.. and HAPPY NEWYEAR everybody! The world didnt vanish because of 2 digits.
//      Isnt that NICE !! :)
//
//2.00a BETA First beta release.
//      CompareBookmark corrected to handle nil bookmark pointers by             5. Jan. 2000
//      jozef bielik (jozef@gates96.com)
//      Indexes introduced. AddIndex,DeleteIndex,IndexDefs supported.
//      AutoSort removed, Resort removed, Sort and SortOn now emulated via an internal index
//      named __MT__DEFSORT__.
//      Indexes introduced as a way into the internal indexes. Not really needed by most.
//      EnableIndexes can be set to false to avoid updating the indexes during a heavy load
//      operation f.ex. The indexes will be invalid until next UpdateIndexes is issued.
//      mtfSaveIndexDef added to possible save flags.
//      Save formats changed for both CSV files and binary files. For reading v.1.xx
//      files, either use CSV format or for binary files, set the compatibility
//      definition further down. V. 1.xx will NOT be compatible with files written
//      in v.2.xx format unless no table definitions are written.
//      _InternalSearch, _InternalBinarySearch and _InternalSequentialSearch removed.
//      ftBytes fieldtype corrected. Usage is shown in the demo project.
//
//2.00b BETA Fixed D4 installation problem reported by Edison Mera Menéndez (edmera@yahoo.com).
//      Published IndexDefs.
//      Added support for ixUnique.
//      Thomas Everth (everth@wave.co.nz) fixed two minor issues in LoadFromDataSet and
//      SaveToDataSet. Further he provided the protected method UpdateRecords, and the
//      public method UpdateToDataset which can be used to sync. another dataset with
//      the contents of the memorytable.
//      Fixed lookup to correcly handle null or empty keyvalues.
//
//2.00c BETA Renamed IndexFields to IndexFieldNames. Supporting IndexName.
//      Fixed designtime indexdefinitions wouldnt be activated at runtime.
//      Fixed Sort/SortOn would generate exception. Fixed Sort/SortOn on blob
//      would generate exception.
//
//2.00d BETA Fixed Master/Detail.
//      Changed the internal FRecords TList to a double linked list to make
//      deletes alot easier from the list. FRecords have been superseeded by
//      FFirstRecord and FFLastRecord. Changed the recordstructure. Introduced
//      TkbmRecord and PkbmRecord which is the definition of a record item.
//      Fixed SwitchToIndex to maintain current record the same after a switch.
//      Added notification handling to reflect removal of other components.
//      Added support for borrowing structures from another TDataset in the
//      table designer.
//
//2.00e BETA Changed bookmark functionality.
//      Fixed D4 installation.
//      Fixed index updating.
//      Optimized indexing and bookmark performance.
//      Added record validity check.
//      Still some bookmark problems.
//
//2.00f BETA Removed the double linked list idea from 2.00d. Back to a TList.
//      Reason is the bookmark handling is not easy to get to work with pointers,
//      plus its needed to be able to delete a record without actually removing
//      it from the 'physical' database. Thus PackTable has been introduced.
//      Deleting a record actually frees the recordcontents, but the spot in
//      the TList will not be deleted, just marked as deleted (nil). PackTable removes
//      all those empty spots, but at the same time invalidates bookmarks.
//      EmptyTable does what the name says, empty it including empty spots and
//      records as allways. Result should be that bookmarks are working as they
//      should, and GotoBookmark is very fast now.
//      Empty spots will automatically be reused when inserting new records
//      by the use of a list of deleted recordID's, FDeletedRecords.
//      Protected function LocateRecord changed.
//      Locate changed, Lookup behaviour improved.
//      CancelRange behaviour improved.
//
//2.00g BETA Fixed edit error when only 1 record was left and indexes defined.
//      Problem reported by Dick Boogaers (d.boogaers@css.nl).
//      Fixed error occuring when inserting records in an empty memtable with
//      a unique index defined.
//      Fixed error when altering a field which is indexed to a value bigger than
//      biggest value for that field in the table.
//
//2.00h BETA Added IndexFields property as suggested by Dick Boogaers (d.boogaers@css.nl).
//
//2.00i BETA Added AttachedTo property for having two or more views on the
//      physical same data. Updating one table will show immediately in the others.
//      Fielddefinitions will be inherited from the primary table holding the data.
//      There can only be one level of attachments. Eg. t2 can be attached to t1,
//      but then t3 can't be attached at t2, but must be directly attached to t1.
//      Its possible to have different indexes on the tables sharing same data.
//      Fixed SearchRecordID problem which sometimes didnt find the record even
//      if it definitely existed. The reason and solution is explained in the
//      SearchRecordID method.
//      Made TkbmCustomMemTable threadsafe.
//      ftArray and ftADT support added by Stas Antonov (hcat@hypersoft.ru).
//
//2.00j BETA Changed to not check for disabled controls on DisableControls/EnableControls
//      pairs. Fixed wrong call to _InternalEmpty in InternalClose when an attached
//      table close. Thread locking changed and fixed.
//      New property AttachedAutoRefresh. Set to false to disallow the cascading refreshes
//      on dataaware controls connected to all the attached memorytables.
//
//2.00k BETA Made public properties on TkbmIndex:
//      IsOrdered:boolead It can be used to determine if the index is up to date.
//      Or it can be set to true to force that the index must be percieved as up to date,
//      even if it hasnt been fully resorted since creation. Usefull f.ex. when loading
//      data from a presorted file. Eg.:
//      mt.EnableIndexes:=false;
//      mt.LoadFromFile(...);
//      mt.EnableIndexes:=true;
//
//      // Since the data was saved using the index named 'iSomeIndex' and thus loaded
//      // in the right sortorder, dont reupdate the iSomeIndex index.
//      mt.Indexes.Get('iSomeIndex').IsOrdered:=true;
//
//      // At some point, update all nonordered indexes.
//      mt.UpdateIndexes;
//
//      IndexType:TkbmIndexType Should only be used to determine the indextype. Dont set.
//      CompareOptions:TkbmMemTableCompareOptions Should only be used to determine the compare options.
//      Dont set.
//      IndexOptions:TIndexOptions Should only be used to determine the indexoptions. Dont set.
//      IndexFields:string Should only be used to determine the fields in the index. Dont set.
//      IndexFieldList:TList Should only be used to gain access to a list of TField objects
//      of fields participating in the index. Dont alter or set.
//      Dataset:TkbmCustomMemTable Should only be used to determine the dataset on which the
//      index is created. Dont set.
//      Name:string Should only be used to determine the name used during creation of the index.
//      Dont set.
//      References:TList References to the records. The references are sorted in the way the index defines.
//      IsRowOrder:boolean Used to determine if this index is a row order index. (the order the records
//      are inserted).
//      IsInternal:boolean Used to determine if this index is an internal index (used by SortOn f.ex.)
//      IndexOfs:integer Used to determine what position this index have in the TkbmIndexes list.
//      Fixed ftBytes bug reported by Gianluca Bonfatti (gbonfatti@libero.it).
//
//2.01  FINAL Cosmin (monh@vatopedi.thessal.singular.gr) reported some problems
//      and came up with some fixes. I have loosely followed them to solve the problems.
//      a) Fixed loading data into readonly fields using LoadFrom...
//      Developers can use new 'IgnoreReadOnly' public property if they want to
//      update fields which are actually defined as readonly. Remember to
//      set it to false again to avoid users updating readonly fields.
//      b) Fixed readonly on table level.
//      c) New read only property: RangeActive:boolean. True if a range is set.
//      d) Fixed autoinc. fields.
//      Fixed bogus warning about undefined return from SearchRecordID.
//
//2.01a Fixed wrong use of FBufferSize in PrepareKeyRecord. Should be FRecordSize;
//
//2.10  Now complete support for Filter property!
//
//2.11  The filter property is only supported for D5. IFDEF's inserted to maintain
//      backwards compatibility.
//      Support for a UniqueRecordID which allways will increase on each insert
//      in contrast to RecordID which is not unique through the lifetime of
//      a memorytable (eg. reusing deleted record spots).
//      Support for RecordTag (longint) which can be used by any application to fill
//      extra info into the record for own use without having to create a real
//      field for that information. Remember that the application is responsible
//      for freeing memory pointed to by this tag (if thats the way its used).
//      Added mtfSaveIgnoreRange and mtfSaveIgnoreMasterDetail to solve problem reported
//      by monh@vatopedi.thessal.singular.gr that saving data during master/detail
//      or ranges will only save 'visible' records.
//
//2.20  NOTE!!!!!   !!!!   !!!!!  NEW LICENSE AGREEMENT  !!!! PLEASE READ !!!!!
//      Fixed quite serious bug in PrepareKeyRecord which would overwrite    16. Feb. 2000
//      memory. Problem occured after using one of the SetKey, EditKey, FindKey,
//      GotoKey, FindNearest methods.
//      Enhanced record validation scheme. Can now check for memory overruns.
//      Changed the inner workings of FindKey and FindNearest a little bit.
//      Fixed that if an IndexDef was specified without any fields, an exception will
//      be raised.
//      Added CopyBlob boolean flag to _InternalCopyRecord which if set, also
//      duplicates all blobs in the copy. Remember to _InternalFreeRecord with
//      the FreeBlob boolean flag set for these copies.
//      Added journaling functionality through the new properties
//      Journal:TkbmJournal, EnableJournal:boolean, IsJournaling:boolean and
//      IsJournalAvailable:boolean. The journaling features are inspired by
//      CARIOTOGLOU MIKE (Mike@singular.gr).
//
//2.21  Fixed backward compatibility with Delphi 3.
//
//2.22  Fixed Sort and SortOn problem with CancelRange.
//      Fixed compile problem when defining BINARY_FILE_1_COMPATIBILITY.
//
//2.23  Fixed several bugs in TkbmCustomMemTable.UpdateIndexes which would
//      lead to duplicate indexdefinitions on every update, which again would
//      lead to the infamous idelete<0 bug.
//      Inserted several checks for table being active in situations where
//      indexes are modified.
//
//2.24  Fixed bug reported by Jean-Pierre LIENARD (jplienard@compuserve.com)
//      where Sort/SortOn called 2 times around a close/open pair would lead
//      to AV. InternalClose now deletes a sortindex if one where created.
//      Thus Sort/SortOn are only temporary (as they were supposed to be in
//      the first place anyway :)
//      Fixed small filtering bug.
//
//2.30a ALPHA Fixed bug in BinarySearch. Before it correctly found a matching record
//      but it was not guranteed the first available record matching. Now it
//      backtracks to make sure its the first one in the sortorder which matchs.
//      Fixed old problem when persistent tables are not written on destruction
//      of the component.
//      Fixed GetByFieldNames (use to find an index based on fieldnames) to be
//      case insensitive.
//      Journal is now freed on close of table, not destruction.
//      Fixed bug in _InternalCompareRecords which would compare with one field
//      to little if maxfields were specified (noticed in some cases in M/D
//      setups).
//      Changed internals of _InternalSaveToStream and _InternalSaveToBinaryStream.
//      Support for versioning implemented. Support for saving deltas using
//      SaveToBinary... supported using flag mtfSaveDeltas.
//      Resolver class (TkbmCustomDeltaHandler) for descending from to
//      make delta resolvers.
//      Added StartTransaction, Commit, Rollback as virtual methods for local
//      transaction handling. Override these to handle middleware transactions.
//      Added readonly TransactionLevel which shows the level of the current
//      transaction.
//      CARIOTOGLOU MIKE (Mike@singular.gr) came up with a big part of the versioning code
//      and had several suggestions to how to handle deltas and versioning.
//
//2.30b BETA Added checking for empty record in list of records in several places (lst.items[n]=nil)
//      Added new save flag mtfSaveDontFilterDeltas which if not specified, filters out
//      all records which has been inserted and then deleted again within the same session.
//      (A session understood like from load of records to save of them).
//      Added new property AllData which can be read to get a variant containing all data
//      and be set from a variant to load all data from.
//      Added new property AllDataOptions which defines which data will be saved using the
//      AllData property. Suggested by CARIOTOGLOU MIKE (Mike@singular.gr).
//      Added designtime persistense of data on form by the new property StoreDataOnForm.
//      If a designtime memtable contains data and thus is active, setting StoreDataOnForm:=true
//      and saving the form will save the data on the form. Thus the data will be available
//      when the form is loaded next time aslong the memtable is active (opened).
//      Added Czech translation by Roman Krejci (info@rksolution.cz)
//      Added property IsFieldModified(i:integer):boolean for checking if a field has been modified.
//      The status is only available until Cancel or Post. Suggested by Alexandre DANVY (alex-dan@ebp.fr)
//      Tabledesigner layout fixed.
//
//2.30c BETA Added two generaly available procedures:                        3. Mar. 2000
//      StreamToVariant and VariantToStream which handles putting a stream into
//      a variant and extracting it again. Means its possible to store f.ex a complete
//      file in a variant by opening the file with a TFileStream and pass the stream
//      through StreamToVariant.
//      Added example of transactioning to the demo project.
//      Added support for TField.DefaultExpression (default value for each field).
//      Define DO_CHECKRECORD to call _InternalCheckRecord. Was default before.
//      Normally only to be used in debug situations.
//      Applied some performance optimizations.
//
//2.30  FINAL Fixed before close bug which would clear a persistent file
//      if the programmer called close before destroy. Problem reported by
//      Frans Bouwmans (fbouwmans@spiditel.nl)
//      Fixed clear index bug not resetting FSortIndex. Problem reported by
//      Ronald Eckersberger (TeamDM) (ron@input.at)
//      Published AutoCalcFields.
//      Added property: RecalcOnFetch (default true) which regulates if
//      calculated fields should be recalced on each fetch of a record or not.
//      Fixed resetting AttachedTo when parent table is destroyed.
//
//2.31  Fixed D3 compatibility.
//      Fixed Resolver. OrigValues as record at checkpoint, Values as
//      record in current version.
//      Fixed missing resync in FindNearest reported by
//      Alexander V. Miloserdov (tatco@cherkiz.spb.su).
//
//2.32  Fixed TkbmCustomMemTable.DeleteIndex A/V bug in D4. Problem reported
//      by Alexander V. Miloserdov (tatco@cherkiz.spb.su).
//
//2.33  Refixed again FindNearest. This time it works ;)
//
//2.34  Fixed missing filter in SaveTo.... Problem reported by Alexei Smirnov (alexeisu@kupol.ru)
//      Fixed missing use of binary search. Problem reported by Tim Daborn (Tim_Daborn@Compuserve.com)
//      Data persistency on destruction of component fixed. Fix by Cosmin (monh@vatopedi.thessal.singular.gr)
//      UpdateRecord fixed with regards to only one key field specified.
//      Fix by Marcello Tavares (TAVARES@emicol.com.br)
//      Brazilian ressource file changed by Marcello Tavares (TAVARES@emicol.com.br).
//      Added KBM_MAX_FIELDS which can be changed to set max. number of fields
//      to handle in a table. Default 256.
//      Fixed bug in error message when unsupported field added to fielddef.
//      Problem reported by Alexandre Danvy (alex-dan@ebp.fr)
//      Fixed some bugs regarding SetRangeStart/End EditRangeStart/End and ApplyRange.
//      Remember to set IndexFieldNames to an index to use including the fields used for the range.
//      Better is to use a filter or similar.
//
//2.35  Fixed bug not clearing out indexes on table close.
//
//2.36  Fixed bug in DeleteIndex which would not reset FCurIndex correctly.   30. mar. 2000
//      Problem seen when SortOn or Sort would be called many times.
//      Problem reported by Michail Haralampos (Space Systems) (spacesys@otenet.gr)
//      Changed CreateTableAs to not to update fielddefs while source table is allready
//      active for compatibility with a bug in Direct Oracle Access. Problem
//      reported by Holger Dors (dors@kittelberger.de)
//      Changed handling of oldrecord and current record during an edit of a record to
//      correctly cancel changes to a blob. Problem reported by Ludek Horcicka (ludek.horcicka@bcbrno.cz)
//
//2.37  Fixed A/V bug versioning blobfields. Problem reported by Jerzy Labocha (jurekl@geocities.com).
//      Optimized indexing when record edited which does not affect index.
//      Suggested by Lou Fernandez (lfernandez@horizongt.com).
//      Fixed bug in InternalLoadFromBinary where check for ixNonMaintained was in wrong order
//      compared to savetobinary.
//      Fixed bug in InternalLoadFromBinary where indexes was created and marked
//      as updated prematurely. Problem reported by Jerzy Labocha (jurekl@geocities.com).
//      Changed LoadFromDataSet to allow copy of properties from default fields.
//      Problem reported by Tim Evans (time@cix.compulink.co.uk).
//
//2.38  Fixed bug not correctly determining autoinc value on loading binary file.
//      Problem and fix reported by Jerzy Labocha (jurekl@geocities.com).
//      Fixed small bug in SaveToStream where nil records would risc being saved.
//      Fixed bug in SetRecNo reported by Mike Cariotoglou (Mike@singular.gr).
//      Added RespectFilter on TkbmIndex.Search and TkbmIndexes.Search for Locate etc. to
//      respect filters set. Problem reported by Andrew Leiper (Andy@ietgroup.com).
//      Added to index search routines to make them threadsafe.
//      Fixed bug updating indexes of attached tables on edit of master table.
//      Problem reported by Lou Fernandez (lfernandez@horizongt.com).
//      Added CSV delimiter properties: CSVQuote, CSVFieldDelimiter, CSVRecordDelimiter
//      which are all char to define how CSV output or input should be handled.
//      CSVRecordDelimiter can be #0 to not insert a recorddelimiter. Note that
//      #13+#10 will be inserted at all times anyway to seperate records.
//      Fixed bug in _InternalClearRecord and added new protected method
//      UnmodifiedRecord in TkbmCustomDeltaHandler.
//      Changed algorithm of dsOldRecord to return first version of record.
//      Added 3 new public medium level functions:
//        function GetVersionFieldData(Field:TField; Version:integer):variant;
//        function GetVersionStatus(Version:integer):TUpdateStatus;
//        function GetVersionCount:integer;
//      which can be used to obtain info about previous versions of current record.
//      GetVersionCount get number of versions of current record. Min 1.
//      GetVersionFieldData gets a variant of data of a specific version. Current
//      record version (newest) is 0.
//      GetVersionStatus returns the TUpdateStatus info of a specific version. Current
//      record version (newest) is 0.
//      Inspiration and fixes by Mike Cariotoglou (Mike@singular.gr).
//
//2.39  Fixed bug setting Filtered while Filter is empty.
//      Fixed autoinc bug on attached tables reported by Jerzy Labocha (jurekl@geocities.com).
//      Added GetRows function for getting a specified number of rows at a starting point
//      and return them as a variant. Code contributed by Reinhard Kalinke (R_Kalinke@compuserve.com)
//      Added integer property LoadLimit which will limit the number of records loaded using LoadFrom....
//      methods. Suggested by Roman Olexa (systech@ba.telecom.sk). if LoadLimit<=0 then
//      no limit is imposed.
//      Added read only integer property LoadCount which specifies how many records
//      was affected in last load operation.
//      Added read only boolean property LoadedCompletely which is true if all data was loaded,
//      false if the load was interrupted because of LoadLimit.
//      Fixed LoadFrom.... to not load into non data fields. Fix by cosmin@lycosmail.com.
//      Fixed persistency on destruction of component.
//      Added partial Dutch ressourcefile by M.H. Avegaart (avegaart@mccomm.nl).
//      Added method Reset to clear out data, fields, indexes and more by kanca@ibm.net.
//      LoadFromBinaryStream/File now tries to guess approx. how many records will be loaded
//      and thus adjust capacity accordingly.
//      Improved persistent save to not delete original file before finished writing new.
//      Suggested by Paul Bailey (paul@cirrlus.co.za).
//      Fixed minor bug in GetRecordCount when table not active by Csehi Andras (acsehi@qsoft.hu)
//
//2.40  Fixed problem with SetRange specifying fewer fields than number of index fields, giving   12. May. 2000
//      wrong number of Resulting records. Problem reported by Jay Herrmann (Jayh@adamsbusinessforms.com)
//      Added new AddIndex2 method to TkbmCustomMemTable which allows to define some additional indexsetups:
//        mtcoIgnoreLocale   which use standard CompareStr instead of AnsiCompareStr.
//        mtcoIgnoreNullKey  which specifies that a null key field value will be ignored in record comparison.
//      Except for the ExtraOptions parameter its equal in functionality to AddIndex.
//      Added new property: OnCompareFields which can be used to handle specialized sortings and searches.
//      Made the following functions publicly available:
//           function CompareFields(KeyField,AField:pointer; FieldType: TFieldType; Partial, CaseInsensitive,IgnoreLocale:boolean):Integer;
//           function StringToCodedString(const Source:string):string;
//           function CodedStringToString(const Source:string):string;
//           function StringToBase64(const Source:string):string;
//           function Base64ToString(const Source:string):string;
//      Added new property: AutoIncMinValue which can be used to set startvalue for autoinc. field.
//      Added new property: AutoIncValue which can be used to obtain current next value for an autoinc field.
//
//2.41  Fixed problem regarding calculated fields on attached table not updating. Problem
//      reported by aZaZel (azazel@planningsrl.it).
//
//2.42  Added PersistentBackup:boolean and PersistentBackupExt:string which controls if to make  25. May. 2000
//      a backup of the previous persistent file and which extension the file should have.
//      Code provided by cosmin@lycosmail.com.
//      Made ResetAutoInc public. Suggested by cosmin@lycosmail.com.
//      Fixed missing copy of RecordTag in InternalCopyR*. Reported by Alexey Trizno (xpg@mail.spbnit.ru).
//      Added property groups by Chris G. Royle (cgr@dialabed.co.za).
//      Fixed missing reset of UpdateStatus in _InternalClearRecord. Reported by CARIOTOGLOU MIKE (Mike@singular.gr)
//      Fixed Search bug on empty table. Problem seen inserting into empty table with ixunique index defined.
//      Problem reported by George Tasker (gtasker@informedsources.com.au)
//      Published BeforeRefresh and AfterRefresh.
//      Fixed deactivation of designed active table during runtime. Reason was missing inherited
//      in Loaded method. Problem reported by John McLaine (johnmclaine@hotmail.com)
//
//2.43  Added OnProgress event which will fire on long operations notifying how
//      many percent has been accomplished. The operation performed can be
//      found in the Code parameter.
//      Added new FastQuickSort procedure to TkbmIndex. Enhances searchspeed by
//      somewhere around 50-75%. Sorting 100.000 records on a field on a PII 450Mhz
//      now takes approx 5 secs. FastQuickSort (combination of a modified Quicksort and
//      Insertion sort) is now the default sorting mechanism.
//      To reenable the previous standard Quicksort mechanism, put a comment on the
//      USE_FAST_QUICKSORT definition further down.
//      Danish translation of ressource strings added.
//      Added public low level function GetDeletedRecordsCount:integer.
//      Changed master/detail behaviour to allow more indexfields than masterfields.
//      Change proposed seperately by Thomas Everth (everth@wave.co.nz) and
//      IMB Tameio (vatt@internet.gr)
//      Updated Brasilian translation by Eduardo Costa e Silva (SoftAplic) (eduardo@softaplic.com.br)
//      Added protected procedure PopulateBlob by Fernando (tolentino@atalaia.com.br)
//      Fixed issues compiling in D3 and BCB4.
//      Commented out not copying nondatafields in LoadFromDataset as implemented in v.2.39
//      Problem reported by Roman Olexa (systech@ba.telecom.sk).
//
//2.44  Removed stupid bug I implemented in 2.43. Forgot to remove some code.
//      Fixed the demo project handling range. The demo of the SetRange function
//      forgot that no index named 'Period' was defined, thus the range was set
//      on the currently active index instead.
//      Added support for using SortOn('',....) for selecting the roworder index.
//      Defined some consts for internal indexnames and internal journal field names.
//        kbmRowOrderIndex = '__MT__ROWORDER'
//        kbmDefSortIndex  = '__MT__DEFSORT'
//        kbmJournalOperationField  = '__MT__Journal_Operation'
//        kbmJournalRecordTypeField = '__MT__Journal_RecordType'
//
//2.45  Fixed Master/detail problem setting masterfields while table not active.
//      Problem reported by CARIOTOGLOU MIKE (Mike@singular.gr).
//      Fixed Filter expression problem when reordering fields in runtime.
//      Problem reported by houyuguo@21cn.com.
//      Added several more progress functions.
//      Added TableState which can be polled to decide whats going on in the table at
//      the moment.
//      Added AutoReposition property (default false) which determines if automatically
//      to reposition to new place for record on record post, or to stay at current pos.
//      Fixed dupplicate fieldname problem with attached tables as reported by
//      Roman Olexa (systech@ba.telecom.sk). If fieldnames conflict between the
//      current table and the table attached to, the original current table field
//      is removed from the table, and the attached to table field used instead.
//
//2.45b Fixed missing FOrdered:=true on FastQuicksort. Problem reported
//      by Tim  Daborn (Tim_Daborn@Compuserve.com)
//
//2.46  Fixed SetFilterText bug. Problem reported by Anders Thomsen (thomsenjunk@hotmail.com)
//      Added BCB 5 support by Lester Caine (lester@lsces.globalnet.co.uk)
//
//2.47  Added copy flag mtcpoAppend for appending data using LoadFromDataset.
//      Added Master/Detail designer. Corrected master/detail functionality.
//      Added Hungarian translation by Csehi Andras (acsehi@qsoft.hu)
//
//2.48  Fixed InternalSaveToStream to save in current indexorder. Problem reported
//      by Cosmin (vatt@internet.gr) and Christoph Ansermot (info@illuminati.ch)
//      Fixed InternalAddRecord to respect the Append flag. Problem reported by
//      Milleder Markus (QI/LSR-Wi) (Markus.Milleder@tenovis.com)
//      Fixed filter bug < which was considered the same as <=. Bug reported by
//      Milleder Markus (QI/LSR-Wi) (Markus.Milleder@tenovis.com)
//
//2.49  Fixed TkbmIndexes.Clear leaving an invalid FSortIndex.      16. July 2000
//      Problem fixed by Jason Mills (jmills@sync-link.com).
//      Fixed D3 bugs which wouldnt allow to compile. Problem fixed by
//      Speets, RCJ (ramon.speets@corusgroup.com)
//      Changed ftVarBytes to work similar to ftBytes. Problem reported by
//      mike cariotoglou (Mike@singular.gr)
//      Fixed filtering of strings through the Filter property. Problem reported
//      by several.
//      Modified demoproject with FindKey functionality and string field.
//      For the time being, removed support for the WideString fieldtype.
//      Changed SaveToBinaryxxxxx to save in the order of the current index.
//      Beware that if the current index is not up to date, it could mean
//      saving less records than there actually is in the table.
//      Changed binary file format to include null value info. LoadFromBinaryxxx
//      is backwards compatible, but files saved with SaveToBinaryxxxx can only
//      be read by software incoorporating TkbmMemTable v. 2.49 or newer.
//      If needed, one of the BINARY_FILE_XXX_COMPATIBILITY defines can be
//      specified for saving in a format compatible with older versions of
//      TkbmMemTable.
//
//2.50  Fixed bug on SortOn after UpdateIndexes. Problem reported by Gate (x_gate@hotmail.com)
// a-d  Added IndexByName(IndexName:string) property to obtain a TkbmIndex object for
// Beta the specified indexname.
//      Added Enabled property to TkbmIndex which can be set to false to disable
//      updating that index or true to allow updating again. An automatic rebuild
//      is issued if needed.
//      Fixed incorrect definition of properties EnableVersioning and VersioningMode
//      in TkbmMemTable. Problem reported by U. Classen (uc@dsa-ac.de)
//      Made CopyRecords public and changed it to copy from current pos in source.
//      Fixed counter bug in CopyRecords which would copy one record more than limit.
//      Fix suggested by Wilfried Mestdagh (wilfried_sonal@compuserve.com).
//      Added saveflag mtfSaveAppend which will append the current dataset to
//      the data previously saved in the file or stream. Suggested
//      by Denis Tsyplakov (den@icsv.ru)
//      Fixed AutoInc problem using InsertRecord/AppendRecord as reported by
//      Jerzy Labocha (jurekl@ramzes.szczecin.pl)
//      Fixed D3 inst. by replacing TField.FullName for TField.FieldName for
//      level 3 installations only. Problem reported by Marcel Langr (mlangr@ivar.cz)
//      Fixed cancel/Blob bug. Pretty tuff to fix. Several routines heavily
//      rewritten to solve problem. Those changes also allows for better strings
//      optimization.
//      Fixed bug reported by jacky@acroprise.com.tw in SwitchToIndex on empty table.
//      Fixed autoreposition bug by CARIOTOGLOU MIKE (Mike@singular.gr).
//      Fixed attachedto bug during destroy by Vladimir Piven (grumbler@ekonomik.com.ua).
//      Optimized per record memory usage by compiling out the debug values startident and endident
//      + made record allocation one call to getmem instead of two. Suggested by
//      Lluís Oll?(mailto:llob@menta.net)
//      Added Performance property which can hold mtpfFast, mtpfBalanced or mtpfSmall.
//      Meaning:
//        mtpfFast=One GetMem/Rec. No recordcompression.
//        mtpfBalanced=One or more GetMem/Rec. Null varlength fields are compressed.
//        mtpfSmall=Like mtpfBalanced except varlength field level compression is made.
//      Use mtpfFast if all string values will have a value which are close to the
//      max size of the string fields or raw speed is important.
//      Use mtpfBalanced if most string fields will be null.
//      Use mtpfSmall in other cases.
//      Added OnCompressField and OnDecompressField which can be used to
//      create ones own field level compression and decompression for all nonblob
//      varlength fields. For blobfields checkout OnCompressBlob and OnDecompressBlob.
//      Added OnSetupField which can be used to overwrite indirection of
//      specific fields when Performance is mtpfBalanced or mtpfSmall.
//      Fixed locate on date or time fields giving variant conversion error.
//      Problem reported by Tim Daborn (Tim_Daborn@Compuserve.com).
//      Updated Russian ressources and added Ukrainian ressources by
//      Vladimir (grumbler@ekonomik.com.ua)
//
//2.50e Fixed delete on master when attached to it.
// Beta Fixed delete on client table without versioning enabled
//      attached to a master with versioning enabled.
//      The bug fix makes sure the client is using same versioning
//      method as the master table.
//      Bugs reported by Davy Anest (davy-ane@ebp.fr)
//      Vladimir (grumbler@ekonomik.com.ua) suggested a bit different way of
//      copying field properties from during attaching to a master table.
//      Since I cannot completely grasp the implications of the changed scheme,
//      I have included it to leave it up to you to test it. The old scheme
//      is commented out in SetAttachedTo.
//      Alex Wijoyo (alex_wijoyo@telkom.net) suggested changing CreateTableAs.
//      Instead of using FieldDefs.Assign.... then a routine of our own is used.
//      This to avoid complications he had using TkbmMemTable in a DLL.
//      ProviderFlags are now copied in CopyFieldProperties. Bug reported by
//      Csehi Andras (acsehi@qsoft.hu).
//      Hungarian ressource file updated by Csehi Andras (acsehi@qsoft.hu).
//      InternalSave.... fixed when not specifying mtfSaveBlobs.
//      Fixed InternalLoadFrom..... A/V when stream size = 0.
//      Bugs reported by Arsène von Wyss (arsene@vonwyss.ch).
//      Changed InternalLoadFromStream to not call progress on each line,
//      but rather on each 100 lines.
//      Added MarkAllDirty in TkbmIndexes to make sure UpdateIndexes will
//      make a full update of all indexes regardless of previous state.
//      Solves bug when sequence Open table, Close table, LoadFrom.... didnt
//      update indexes correctly.
//      Added properties CSVTrueString and CSVFalseString for setting
//      stringrepresentation of true and false. Default value 'True' and 'False'.
//      Notice they are caxe sensitive.
//      Updated demo project to show OnProgress event.
//      Added new class TkbmSharedData for datasharing between memtables.
//      Modifed TkbmCustomMemtable to use this new class.
//      Added Standalone property which can be set to true for true standalone
//      memorytables that is tables that are not attaching to other table and other
//      tables dont attach to. The table is not threadsafe if Standalone=true.
//      It can gain a few percentage of speed.
//      Reintroduced Capacity property for prespecifying expected number of records.
//      Today i've tested inserting 1 million records of 1 string field with field
//      length 10 chars. Its able to insert 100.000 recs/sec.
//      Method used:
//        kbmMemTable1.Open;
//        kbmMemTable1.DisableControls;
//        kbmMemTable1.EnableIndexes:=false;
//        kbmMemTable1.Performance:=mtpfFast;
//        kbmMemTable1.Standalone:=true;
//        kbmMemTable1.Capacity:=1000000;
//        for i:=0 to 999999 do
//            kbmMemTable1.AppendRecord(['ABC']);
//        kbmMemTable1.EnableIndexes:=true;
//        kbmMemTable1.UpdateIndexes;
//        kbmMemTable1.EnableControls;
//      Added RecNo and UniqueRecID properties to TCustomDeltahandler to obtain
//      those informations from the current record during resolve.
//      Changed LoadFromDataset to avoid closing and opening the table
//      unless needed. Suggested by Stefano Monterisi (info@sesamoweb.it)
//      Setting RecordTag fixed. Problem reported by CARIOTOGLOU MIKE (Mike@singular.gr)
//      Several properties now must be set when table is inactive:
//        EnableVersioning, VersioningMode, AutoIncMin, Performance, Standalone,
//
//2.50f Fixed missing scroll event on SetRecNo. Problem reported by jcyr@jeancoutu.com.
// Beta Fixed A/V on setting or getting recno when table closed. Problem
//      reported by ycloutier@jeancoutu.com.
//      Removed active checks for Performance, EnableVersioning, VersioningMode,
//      AutoIncMin.
//      Fixed wrong default value for VersioningMode. Now its mtvm1SinceCheckPoint again.
//      Altered SetRecordTag to use RecordID instead of RecordNo.
//      Fixed A/V when versioning on records with blobs. Bug reported by
//      Davy Anest (davy-ane@ebp.fr)
//      Added Modified property to table to determine if the data in the table has been
//      modified. Set Modified:=false to clear the flag.
//      Modified tabledesigner to allow borrowing structure from datamodules too.
//      Submitted by Andrew Leiper (Andy@ietgroup.com)
//      Fixed bug closing a table where children are attached to. Now the children are
//      closed too. Bug reported by Davy Anest (davy-ane@ebp.fr).
//      Fixed bug opening a table which is attached to a closed table. Now the
//      'master' table will be opened too.
//      Changed the use of TCriticalSection to TRTLCriticalSection for non Level 5
//      compilers. Should make D3 compilation possible.
//      Changed SetEnabled of TkbmIndex to use a FEnabled field instead of ixNonMaintained.
//      Should make D3 compile. Reported by peter nouvakis (nouvakis@freemail.gr).
//      CreateTableAs fielddefs assignments changed for Level 3 compilers.
//      Reported by peter nouvakis (nouvakis@freemail.gr).
//
//2.50g Fixed two stupid bugs introduced in 2.50f:
// Beta  Indexes being disabled by default, and a blob destruction when no blob
//       existed. Sorry folks. Thats what happens when time for open source
//       development is in the late evening :)
//      Added default statements to properties. Suggested by CARIOTOGLOU MIKE (Mike@singular.gr)
//      Updated Ukrainian and Russian ressource files by Vladimir Piven (grumbler@ekonomik.com.ua)
//      Removed extra unneeded CopyFieldProperties line in SetAttachedTo.
//      Reported by Vladimir Piven (grumbler@ekonomik.com.ua)
//      All binary searching functions changed from recursive to non recursive.
//      Suggested by CARIOTOGLOU MIKE (Mike@singular.gr)
//      Fixed 'out of range' bug in SetFieldData and GetFieldData on calculated fields.
//
//2.50h Changed name of Modified property introduced in 2.50g Beta to IsDataModified to
// Beta avoid pretty serious conflict with TDataSet.Modified which only works on record
//      level.
//      Fixed setting DeletedCount to 0 in EmptyTable. Bugs reported
//      by CARIOTOGLOU MIKE (Mike@singular.gr).
//      Fixed bug not clearing the deletedlist after internalpack. Bug reported by
//      Edison Mera Menéndez (edmera@yahoo.com).
//      Fixed bug setting masterlink=nil during save of data. Now a flag FMasterLinkUsed
//      will be set to false if mtfSaveIgnoreMasterDetail is specified in saveflags, and
//      reset to true after the save finished. Reported by GIANNAKOPOULOS KOSTAS
//      (kyan@singular.gr)
//      Added mtcpoFieldIndex for specifying copying 'Index' position of field
//      during CreateTableAs to make sure the order of the field columns are
//      the same as the source. Problem reported by IMB Tameio (vatt@internet.gr)
//      Changed bookmarks from storing RecordNo to storing a PkbmRecord pointer.
//      This will make bookmarks behave correct. Many thanks to several people
//      which came with suggestions. The used suggestion was from
//      CARIOTOGLOU MIKE (Mike@singular.gr).
//
//2.50i Major changes in shareddatarecord system. Rewritten to use TkbmCommon class. 4. Jan. 2001
// Beta Rewritten parts of VarLength and blob handling to fix several bugs
//      and leaks.
//      Changed so each attaced table has its own bookmark in the record.
//      Thus total recordsize is dependent on the number of tables attached
//      to it.
//      Implemented lots of fixes and memory optimizations.
//      Fixed and updated designer for loading of data from another TDataset
//      + several other bugs which could cause D5 to hang without posibility
//      to exit.
//      Implemented fix in GetLine by Primoz Gabrijelcic (gabr@17slon.com)
//      Fixed missing quoting of boolean field on save. Reported by Juergen (jschroth@gmx.de)
//      Added saveflag mtfSaveFieldKind. If specified together with mtfSaveDef,
//      the field kind will be restored when data is loaded again.
//      Thus fileformat is upgraded to 250, but should be backwards read
//      compatible.
//      Fixed autoreposition when no keys changed. Bug reported by Davy Anest (davy-ane@ebp.fr)
//      Renamed GetDeletedRecordsCount function to a property DeletedRecordsCount.
//      Journaling removed! Let me hear if that is a problem.
//      Implemented different BinarySearch algorithm long time ago suggested by
//      CARIOTOGLOU MIKE (Mike@singular.gr).
//      Fixed comparebookmarks to also compare on recordid.
//      Fixed bug trying to copy data to autoinc field in destination table using
//      SaveToDataset. Reported by Bohuslav Švancara (boh.svancara@quick.cz).
//      Fixed test for Field.Readonly in SetFieldData. Should use take dsSetKey
//      dataset state into account. Reported by Bohuslav Švancara (boh.svancara@quick.cz)
//      ftWideString and ftGUID support during LoadFromDataset and CreateTableAs
//      as a normal ftString.
//      Fixed so autoinc is not populated during InternalLoadFromStream if the
//      field is in the data to load.
//      Changed so InternalLoadFromBinaryStream can load v. 1.xx files.
//      Solution suggested by Paul Moorcroft (pmoor@netspace.net.au)
//      Fixed SetRecNo to use GetRecNo instead of relying on FRecNo.
//      Fix suggested by Stefan Knecht (StefanKnecht@gmx.de)
//      Fixed bookmark error string in german ressource file by Stefan Knecht.
//      Added _REAL_ localization support through LocaleID, LanguageID, SubLanguageID
//      and SortID. In D5 LocaleID also presents a human readable list of
//      available locales. Setting LocaleID will autoupdate LanguageID, SubLanguageID
//      and SortID, and setting any of those will update LocaleID.
//      Made sure it could compile in D3! :)
//      Time to release and get on with my real work!                      7. Jan. 2001
//
//2.50j Fixed TkbmCommon._InternalEmpty wrong call to Indexes.Rebuild.
// Beta Fixed EmptyTable not clearing buffers. Bugs reported by
//      Michael Gillen (Michael@gillenconsultinggroup.com)
//      Added OnSetupFieldProperties event where field properties can
//      be changed during load of structures.
//      Added ClearBuffers to Commit, Rollback, CheckPoint, PackTable to
//      make sure the internal TDataSet buffers are refreshed.
//
//2.50k Added AttachMaxCount which can be set to indicate how many tables   18. Jan. 2001
// Beta should be allowed to be attached to the base table without record
//      structure must change. Note that this takes up approx 8 bytes/attachment
//      /record. There is allways minimum one attachment (the base table itself).
//      As long there are free entries, they will be used. When no more free
//      entries are available and some table is open, an exception will be raised.
//      Changes in InternalOpen, CheckActive,CheckInActive,InternalClose,SetAttachedTo.
//      Persistent files layout must since v. 2.50i be defined in the table at
//      designtime to avoid exceptions. This will be corrected in v.3.00
//      Altered InternalEmptyTable to preserve state. Suggestion by (Alex@ig.co.uk).
//      Fixed 1.st record varlengths shown incorrect when Performance<>mtpfFast.
//      Bug reported by Szakály Balázs (szakalyb@freemail.hu).
//      Fixed setting indexname using different case than created index.
//      Bug reported by Joseph Gordon (pdsphx@parishdatainc.com)
//      Changed .Value to .AsString in CopyRecords and UpdateRecords to fix
//      TLargeIntField not supporting variants.
//
//2.50l Fixed 'Invalid record' error in GetData when varlength field is null.
// Beta Added procedure AssignRecord(Source,Destination:TDataSet);
//      to move the reoord of the active source record to the active dest record.
//      Suggested by Christian Weber (christian.weber@nextra.at)
//      Added DetailFields for master/detail relations. Use DetailFields instead
//      of the TTable standard IndexFieldNames for specifying detail fields in
//      a master/detaíl relation.
//      Fixed design and runtime bugs with master/detail.
//      Fixed Search routine to correctly track record.
//      Bug reported by Bohuslav Švancara (boh.svancara@quick.cz)
//      Fixed SetRecNo delta count and removed resync. Bug reported by paulp@eriksonpro.com.
//      Fixed GetRecNo return based on 0 should be 1. Bug reported by ut@tario.net
//
//2.50m Fixed TDateTime filter bug as suggested by Yuri Tolsky (ut@tario.net)
// Beta Fixed 'List index out of bounds (-1)' in BinarySearch by Yuri Tolsky (ut@tario.net).
//      Fixed bug in InternalSaveToStream/InternalSaveToBinaryStream where field was
//      saved even if it was not matching the saveflags (mtfSaveData,mtfSaveCalculated,mtfSaveLookup).
//      Bug reported by Les Pawelczyk (lpawelczyk@pixelpointpos.com)
//      Fixed bug tracking record in BinarySearch. Bug reported by Yuri Tolsky (ut@tario.net).
//
//2.50m2 Fixed Locate on first record in index. Bug reported by Yuri Tolsky (ut@tario.net).
// Beta
//
//2.50m3 Fixed Insert of records using index. Bug reported by scs.inf@mail.telepac.pt.
// Beta  Fixed so delete of record will update indexes regardless of EnableIndexes
//       to avoid pointer prob. on f.ex. grid refresh. Problem reported by
//       paulp@eriksonpro.com.
//
//2.50m4 Fixed GotoKey/FindKey/FindNearest not to return true if record not found.
// Beta  Bug reported by Ove Bjerregaard (dev_dude2001@yahoo.com).
//
//2.50n  Added Exists function to evaluate if the persistant file allready exists.
// Beta  Suggested by Tom Deprez (tom.deprez@village.uunet.be).
//       Added support for wildcard comparisons in filters by Yuri Tolsky (ut@tario.net)
//       Added support for BCD in filters for D5/BCB5 only by CARIOTOGLOU MIKE (Mike@singular.gr)
//       Removed BCDtoCurr and CurrToBCD from TkbmCustomMemTable since they were incorrect and
//       not used.
//
//2.50o  Fixed varlength bug in comparerecords. Bug reported by Jerzy Labocha (jurekl@ramzes.szczecin.pl)
//  Beta Fixed transaction handling. Bug reported by Radovan Antloga (radovan.antloga@siol.net)
//       Changed some Resync's to Refresh to better keep current record position.
//
//2.51   Fixed SetBlockReadSize which should only do next in D4 for fixing
// Beta  duplicate record bug in Midas for Delphi 4.00-4.03.
//       Added mtfSaveNoHeader to saveflags. If specified will not save field name header
//       line. Remember that LoadFromFile/Stream will _allways_ skip the first record in the
//       file/stream regardless of this flag. Thus mainly use mtfSaveNoHeader for appending
//       to an existing file or for other external use.
//       Added posibility to set CSVQuote to #0 to not use quotes for both save and load.
//       Be carefull not to have , in your field data and empty fields will allways
//       be interpreted as being null.
//       Now creating file if its not existing using mtfSaveAppend. Suggested by
//       Wilfried Mestdagh (wilfried_sonal@compuserve.com)
//       Fixed D4 compatibility to do with ftGUID.
//       Added support for saving/loading defaultexpression when saving field defs.
//       Fieldkind is now allways saved in the file if definitions are saved.
//       If mtfSaveFieldKind is not specified, it is saved as 'data' as fieldkind.
//       Binary file version is now 251, CSV file version is now 251.
//
//2.51b  Fixed allowing TDateTime and strings for PopulateField of date/time/datetime fields.
// Beta  Bug reported by Radovan Antloga (radovan.antloga@siol.net).
//       Fixed bug on persistent save when backup file exists by Naji Mouawad (naji@home.com)
//       Fixed GetRecNo when table is emptied.
//       Fixed missing index update when record is deleted and versioning is enabled.
//       Added filtered indexes. You can decide using a filter which records should be
//       part of an index when inserting and editing records. Check AddFilteredIndex function.
//       The filter doesnt have to be on the field(s) which are indexed.
//       Added OnFilterIndex event functioning like the OnFilterRecord except for indexes.
//       Fixed bug attaching on open table even if AttachMaxCount is set. Bug reported by
//       Andrew Leiper (Andy@ietgroup.com).
//       Refixed D4 ftGUID compile bug which should be fixed in 2.51 Beta.
//       Moved DisableControls in front of a try block as suggested by IMB (vatt@internet.gr)
//       Fixed missing reset of Result in StringToBase64 when len=0 as reported by
//       delphi5 (delphi5@freemail.hu)
//
//2.52a  Fixed allowing also Double, Integer, Single to be used for argument for populatefield of
// Beta  date/time/datetime fields. Problem reported by Radovan Antloga (radovan.antloga@siol.net).
//       Changed InternalLoadFromStream to handle autoskipping fields which is not
//       present in either source file or table field definition.
//       Added posibility to specify index/sort/key options per field by using an enhanced
//       FieldNames syntax. The normal syntax is: fldname1;fldname2;fldname3...
//       The enhanced syntax allows adding a colon and some options to each of the fieldnames.
//       Eg: fldname1:C;fldname2:DC;fldname3
//       Which means fld specified with fldname1 should be case insensitive,
//       field specified with fldname2 should be descending and case insensitive,
//       and fldname3 should be ascending and case sensitive.
//       The following options are recognized:
//          C for Case insensitive,
//          D for Descending,
//          P for Partial,
//          N for ignore Nulls,
//          L for ignore Locale.
//       The changes was inspired by a descfields contribution from
//       Michael Bonner (michaelbonner@earthlink.net).
//       If SortOptions etc are specified, they overrule field specific settings.
//       Added backwards compability to IndexFieldNames in Master/Detail.
//       If DetailFields are not specified, but IndexFieldNames are, IndexFieldNames will be
//       used.
//       Fixed a few more compile bugs for pre LEVEL5 compilers.
//
//2.52b  Fixed bug using LoadFromStream to load a stream containing fewer fields than
// Beta  in memory table.
//
//2.52c  Removed overriding statefield in EmptyTable. Bug reported by Alex.Gromyko@ig.co.uk.
// Beta  Fixed AppendRecord using varlength fields. Was not copying varlengths
//       in InternalCopyRecord. Bug reported by Darren Wade (waded@dameaurora.f2s.com)
//       Added missing DisableControls in EmptyTable. Bug reported by
//       IMB T (vatt@internet.gr)
//
//2.52d  Added check for table open in SaveToxxxx. Bug reported
// Beta  by Chris G. Royle (chris.royle@valmet.com)
//       Moved Precision setting of TBCDFields from CopyFieldProperties to
//       CreateFieldAs. Bug reported by Harry Holzhauser (harryh@pobox.com).
//       Fixed setting active record during indexfilter. Bug reported
//       by Timo Salmi (tjs@iki.fi).
//       Changed EmptyTable to correctly update attached tables.
//       Suggestion by IMB T (vatt@internet.gr).
//       Fixed A/V bug when rearranging order of fields (f.ex. in a grid) and
//       stringfield is present. Bug reported by Anders Obermueller (obermueller@lop.de)
//       Fixed bug Fíltering according to expression even if Filtered:=false.
//       Cleaned up use of Refresh.
//
//2.52e  Refixed Index filtering bug.
// Beta
//
//2.52f  Enhanced CopyRecords to fix both LargeInt and Date/Time fields.
// Beta  If source and dest is same type, but not largeint, value will be used,
//       else AsString.
//       Bug reported by Steven Kamradt (steven@resource-dynamics.com).
//       Fixed bug in InternalInitFieldDefs where also predefined non data fields
//       was used as a template for a fielddef.
//       Removed wrong Validate call in GetFieldData.
//       Fixed setting field modified flag for blobfields whn operation is ReadWrite.
//       Now allowing creating an index without name. Not recommended though!
//       Bugs rep. by Mogens B. Nielsen (mobak@teliamail.dk)
//       Fixed missing calculate of fields during save/binarysave.
//       Bug rep. by Jerzy Labocha (jurekl@ramzes.szczecin.pl).
//       Fixed Standalone setting bug.
//       Bug rep. by Jerzy Labocha (jurekl@ramzes.szczecin.pl) and others.
//       Filter functions enhanced. Contribution by Markus Landwehr
//       (leisys.entwicklung@leisys.de). Now following operators and functions
//       are available in a filter: =,<>,>,>=,<,<=,+,-,*,/,(,),
//       ISNULL,ISNOTNULL,NOT,IN,LIKE,YEAR,MONTH,DAY,HOUR,MINUTE,SECOND,GETDATE,
//       DATE,TIME.
//
// 2.52g Removed leftover validate in GetFieldData. Bug reported by         4. Apr. 2001
// Beta  Mogens B. Nielsen - Danish Software Development A/S (mobak@teliamail.dk)
//       Fixed bug table sending A/V after attaching to another table and exception
//       raised. Bug reported by Michael Bonner (michael.bonner@ci.fresno.ca.us)
//       Fixed Index.Rebuild not validating recordnumber bug reported by
//       Timo Salmi (tjs@iki.fi).
//       Added AttachCount readonly property which returns the current number
//       of attached tables.
//       Fixed bug in CopyRecord as reported by IMB T (vatt@internet.gr).
//       Added support for Boolean fields in PopulateField by
//       Jeffrey Jones - Focus Land & Legal Technologies (jonesjeffrey@home.com).
//       Updated Dutch ressource file by avegaart@mccomm.nl.
//       Fixed SetRange to allow specifying null or less than indexfieldnames.count
//       entries. Bug reported by Ove Bjerregaard (dev_dude2001@yahoo.com).
//
// 2.52h Fixed FindNearest. Bug reported by florian@radiotel.ro (florian@radiotel.ro).
// Beta
//
// 2.52i Fixed record inserted after cur record instead of before using roworder index.
//       Bug reported by N.K. MacEwan B.E. E&E (neven@mwk.co.nz)
//       Fixed locate bug when fieldmodifiers given and on current index.
//       Bug reported by florian@radiotel.ro and others.
//
// 2.53  Fixed compare multiple string fields. Bug reported by Everix Peter (peter.everix@wkb.be)
//       Fixed compilation bug when BINARY_FILE_1XX_COMPATIBILITY was defined.
//       Fixed AutoInc field not correctly handled in some situations.
//       Problem reported by Martin Kreidenweis (80171@gmx.de)
//       Added protected method SetLoadedCompletely(AValue:boolean).
//       Improved threadsafeness. Remember that the VCL and dataaware
//       controls are _not_ threadsafe. To remedy this, in a thread
//       before any updates or access to the table do
//       mt.Lock;
//       try
//        ....
//       finally
//         mt.Unlock;
//       end;
//       This also makes dataaware controls behave relatively nicely.
//       Fixed loading CSV data where last field is only 1 char and not quoted.
//       Bug reported by Mark Voevodin (marner@bigpond.net.au)
//       Added support for FindFirst,FindNext,FindPrior,FindLast using filter
//       expreesion or onFilterrecord as search criteria, even if Filtered:=false.
//       Enhancement (and making kbmMemTable more TTable compatible) suggested by
//       Mikael Willberg (mig@vip.fi).
//       Added support for dest. fields of type ftWideString in
//       SaveToDataset and UpdateToDataset.
//       Returning nil on TkbmIndexes.Get instead of raising an exception.
//       Bug reported by Chris Michael (nojunkmail@computeralchemist.com)
//       Altered CopyRecords not to call RecordCount unless an OnProgress
//       eventhandler is assigned. Suggested by Dan (programinator@yahoo.com)
//       Fixed A/V when EmptyTable called during table state dsEdit/dsInsert.
//       Bug reported by chetta19@yahoo.com.
//       Fixed so EmptyTable resets index to row order index.
//       Added support for Delphi 6.
//
// 2.53a Re-removed wrong RecordCount reference :)
//       Fixed pretty hard to find indexing bug reported
//       by Sergei Safar (hissa59@ig.com.br)
//
// 2.53b Fixed wrong insertion order in the row order index.
//       Bug reported by Primoz Gabrijelcic (gabr@17slon.com)
//       Fixed Range bug in BinarySearch.
//       Fixed setting filter not refreshing correctly. Bug reported by karsten.pester@locom.de.
//       Added flags mtufAppend, mtufEdit to UpdateToDataset.
//       Level5 compilers also keep backwards compatible old version of UpdateToDataset.
//       Contribution by Thomas Everth (everth@wave.co.nz)
//       Added TkbmIndexes.GetIndex(Ordinal:integer):TkbmIndex for getting an index
//       by ordinal number.
//       Fixed switching to a filtered index when current record is not accepted by index filter.
//       Bug reported by Maurizio Lotauro (Lotauro.Maurizio@dnet.it).
//       Added DesignActivation property (default true) which decides if
//       the table should be automatically activated at runtime if it was active
//       during save in designtime. Suggestion from Maurizio Lotauro (Lotauro.Maurizio@dnet.it).
//       Fixed so SaveToxxxx and UpdateToxxxx makes calls CheckBrowseMode to
//       ensure that the table is in browse mode, and not any edit/insert modes.
//       Bug reported by Maurizio Lotauro (Lotauro.Maurizio@dnet.it).
//
// 3.00a Moved FieldTypeNames and FieldKindNames to interface section for
// alpha compability with kbmMW.                                         22. July 2001
//       Removed wrong setting of FRecalcOnIndex in BuildFieldList.
//       Made BuildFieldList, FindFieldInList, IsFieldListsEqual and
//       SetFieldListOptions public.
//       Fixed A/V bug setting Deltahandler to nil.
//       Rearranged I/O scheme. Thus SaveToBinaryxxxx, LoadFromBinaryxxxx and
//       SaveToxxxx and LoadFromxxxx along with all the CSV save flags are
//       not available anymore. Instead two new components TkbmBinaryStreamFormat,
//       and TkbmCSVStreamFormat has been introduced. New formats for save and
//       load can be build by inheriting from TkbmCustomStreamFormat or
//       TkbmStreamFormat.
//       OnCompressBlobStream and OnDecompressBlobStream has an extra ADataset parameter.
//       This will require you to make a small change in your code for your event handler.
//       (De)Compression of load/save operations has now been moved to the TkbmCustomStreamFormat
//       components. Thus code for these events need to be moved.
//       Improved bookmarkvalid check by adding TkbmUserBookmark.
//
// 3.00b Added OnBeforeLoad, OnAfterLoad, OnBeforeSave and OnAfterSave to TkbmCustomStreamFormat.
// alpha Fixed GetRecord(grCurrent) when filter not matching.
//       Fixed missing Refresh in SwitchToIndex when FRecNo=-1;
//       Bugs reported by Kuznetsov A. V. (kuaw26@mail.ru)
//       Added protected SetTableState(AValue:TkbmState) by request of
//       Kuznetsov A. V. (kuaw26@mail.ru)
//       Removed DesignActivation from D3 since D3 doesnt have SetActive.
//       Fixed Lookup field type bug reported by Giovanni Premuda (gpremuda@softwerk.it).
//       Fixed setting IsDataModified when versioning is enabled and record deleted.
//       Bug reported by Radek Zhasil (radek.zhasil@vitkovice.cz).
//       Altered CopyRecords to copy autoinc value from source if destination table is empty.
//       If not, a new unique value will be generated.
//       Fixed FindKey/GotoKey returning True on second run even if key not found.
//       Bug reported by Sergei Safar (hissa59@ig.com.br).
//       Reordered property entries of TkbmCustomMemTable to make sure
//       AttachedTo is set before all other properties. Note this require that
//       the form is saved again to update the dfm file with the changed order.
//       Bug reported by Maurizio Lotauro (Lotauro.Maurizio@dnet.it).
//       Split the kbmMemStreamFormat.pas file into kbmMemCSVStreamFormat.pas and
//       kbmMemBinaryStreamFormat.pas since its the most logical thing to do.
//       All changes specific to those formats will be written in a history
//       in the start of each of the files.
//       Updated Slovakian ressource file by Roman Olexa (systech@ba.telecom.sk)
//
// 3.00c Fixed SetRange on multiple fields. In previous versions, the combined
// alpha set of fields was compared instead of field by field as Borland describes.
//       Bug reported by Dave (rave154@yahoo.co.uk).
//
// 3.00d Added Assign method to TkbmCustomStreamFormat.
// alpha Fixed Comparebookmarks to return -1,0 and 1 specifically.
//       Fixed not raising keyfields has changed exception when editing keyvalue
//       on a filtered index on an attached table. Bug reported by Timo Salmi (tjs@iki.fi).
//       Fixed AV when freeing base memtable to which others are attached.
//
// 3.00e Fixed _InternalCompareRecords when comparing two null fields.
// alpha Bug reported by Radovan Antloga (radovan.antloga@siol.net).
//
// 3.00f Added protected CheckpointRecord to TkbmCustomMemTable.
//       Added missing published OnCompress and OnDecompress properties to
//       TkbmStreamFormat.
//       Added public Stream property to TkbmStreamFormat.
//       Fixed bug reported by Andreas Obermüller <obermueller@lop.de> in D6
//       when requiredfields was not checked.
//       Fixed copying null fields in CopyRecords, UpdateRecords and AssignRecord.
//
// Entering BETA state!
// 3.00f1 Fixed A/V when dataset to which deltahandler connected is removed.
//       Added support for fkInternalCalc.
//
// 3.00f2 Added missing AllDataFormat property.
//       Virtualized LoadFromStreamViaFormat and SaveToStreamViaFormat.
//       Fixed bug in InternalCompareFields as reported by Radovan Antloga
//       (radovan.antloga@siol.net).
//       Added TestFilter method for applying a filter to the current
//       record to see if its matching the filter. Contributed by vatt@internet.gr.
//       Added sfSaveInsert flag to TkbmCustomStreamFormat which will save to a
//       stream at the current position. Other values are sfSaveAppend which will
//       append to the stream and none which will overwrite contents of stream.
//       sfAppend overrules sfInsert.
//       Added check for number of field defs in several places.
//       Updated some language ressource files.
//       Fixed designer to save in binary when the binary checkbox was checked.
//       Bug reported by Francois Haasbroek (fransh@argo.net.au).
//       Fixed borrowing from TDataset in designer in Delphi 6. Solution
//       provided by Jorge Ducano (jorgeduc@portoweb.com.br)
//       Added sfSaveUsingIndex to TkbmCustomBinaryFormat. Its default true to
//       keep backwards compability. Set it to false if to save according to
//       the internal record list which also maintains deleted records.
//       v. 3.00 now again supports Delphi 3.
//
// 3.00f3 Fixed floating point bug introduced in 3.00f2 in binary stream format.
//       Bug reported by Fred Schetterer (yahoogroups@shaw.ca).
//
// 3.00f4 Fixed MasterFields designer for inherited components.
//       Virtualized MasterChanged, MasterDisabled, CopyRecords, Progress,
//       Lock and Unlock.
//
// 3.00f5 Fixed deltahandler Value and OrigValue returning empty string instead of Null
//        when field is null.
//
// 3.00f6 Fixed massive leak in TkbmBinaryStreamFormat Resulting from
//        missing to indicate records were part of table.
//        Changed so LoadFromDataset only issues First if source table was not on
//        first record. Will satisfy forward only sources.
//        Suggestion by Marco Dissel (mdissel@home.nl).
//        Added compiler directive for enabling short circuit evaluation.
//        Suggested by Bill Lee (mrbill@qualcomm.com)
//        Fixed Locate (and all other searching methods) on descending index.
//        Bug reported by Walter Yu (walter@163.net).
//        Fixed raising exception if no indexfieldnames given for FindKey.
//        Suggested by Sergei Safar (sergei@gold.com.br).
//        Fixed AutoReposition Index out of range when delete, close table, open table.
//        Bug reported by Federico Corso (federico.corso@eudata.it)
//
// 3.00f7 Fixed bug not copying autoinc value in CopyRec when destination table empty.
//        Was wrongly testing for source table empty.
//        Fixed bug negating a negate when comparing descending field indexes.
//        Fixed problem comparing longint and int64 fields in comparefields.
//        Fixed sorting bugs introduced in 3.00f6.
//        Fixed searching using FindKey/FindNearest on descending indexes.
//        Fixed CompareBookmark function to better test for invalid bookmarks.
//        Fixed Persistent save during destruction which could lead to A/V.
//
// 3.00f8 Fixed loading CSV files containing blobfields.
//
// 3.00f9 Added OnFormatLoadField and OnFormatSaveField event for reformatting of
//        data before they are actually loaded or saved.
//        Added sfQuoteOnlyStrings (CSV) flag for selecting to only quote string/binary fields during save.
//        Published sfNoHeader and completed support for it (CSV). It controls if a header should be saved or loaded.
//        Added raising an exception if Save... is called when table is closed.
//        Changed the Result of OldValue for a field to return the original unchanged value
//        if versioning is enabled. This is to make kbmMemTable be more compatible with TClientDataset.
//
// 3.00g  Beta Fixed Commit/Rollback as suggested by (peter.bossier@sintandriestielt.be)
//        Made small changes for better support of fetch on demand in kbmMW.
//        Improved filtering to check for variant empty/null.
//        Added support for BCB6.
//        Added support for Kylix1. Please notice that the memtable designer although
//        ported for Linux, is not available simply because I cannot find a
//        decent way to invoke the default field editor programatically.
//        Thus all custom component editors have been disabled for Kylix 1.
//        RecordTag use while filtering bug fixed. Bug reported by Aart Molenaar (almo@xs4all.nl)
//        Fixed comparing very large values in CompareField which could raise OutOfRange error.
//
// 3.00 FINAL                                                             14. June 2002
//        Fixed minor problem not resetting internal TDataset flags correctly
//        after a binary load.
//        Rolled the rollback and commit change in 3.00g Beta back due to serious
//        problems. Sorry about that. Bug reported by hans@hoogstraat.ca
//        Added support for indexing Lookup fields. Suggested by hans@hoogstraat.ca.
//        Fixed C++ Builder 6 project which mistakenly referred to kbmMW file.
//        Added BufferSize property to binary stream format.
//        Suggested by Ken Schafer (prez@write-brain.com)
//        Fixed some Kylix compability situations by (Hans-Dieter Karl) hdk@hdkarl.com
//        Removed TkbmTreadDataset from Kylix distribution.
//        Added ClearModified method to TkbmMemTable to clear out modification flags
//        of both fields and table. Suggested by hans@hoogstraat.ca
//        Changed so IsDataModified flag is not set if Edit/Post do not change anything.
//        Suggested by hans@hoogstraat.ca
//
// 3.01   30. June. 2002
//        Changed TkbmIndex constructor to accept TkbmMemTableCompareOptions instead of
//        TIndexOptions.
//        Added global public functions:
//        IndexOptions2CompareOptions and CompareOptions2IndexOptions for
//        easy conversion between the two sets.
//        This allows for Sort/SortOn/SortDefault to utilize all special memtable
//        compare options. Further it enables manually created indexes via
//        TkbmIndex to take advantage of those features too.
//        Problem indicated by mariusz@nizinski.net.
//        Added new property AutoUpdateFieldVariables which is false by default.
//        Setting it to true automatically updates/adds field variables to the
//        owner of the memtable (f.ex. a form). Contributed by Ken Schafer (prez@write-brain.com)
//        Added support for ftFmtBCD.
//        Fixed InternalOpen problem which recreated fields/fielddefs for attached tables.
//        This Resulted in severe problems when field order is different between base table
//        and attached table. Bug reported by michael.bonner@ci.fresno.ca.us.
//        Added support for ftTimestamp for LEVEL6 compilers (D6/BCB6).
//
// 3.02   15. July 2002
//        Internal version reserved for Delphi 7 and Kylix 3.
//        Fixed compilation problems in D4 and Kylix by adding missing IFDEF's.
//        Problem reported by several.
//        Fixed not equal filter bug reported by several.
//
// 3.03   7. August 2002
//        Kylix 3 and Delphi 7 support officially added.
//        Kylix 2 project files added.
//        Added several error classes:
//          EMemTableFatalError -> EMemTableInvalidRecord
//          EMemTableIndexError -> EMemTableDupKey
//          EMemTableFilterError
//          EMemTableLocaleError -> EMemTableInvalidLocale
//        Updated Italian ressource file by Alberto Menghini (alberto@asoft.it).
//        Updated Czech ressource file.
//        Updated Romanian ressource file by Sorin Pohontu (spohontu@assist.ro).
//        Made SavePersistent and LoadPersistent public.
//        Added public property PersistentSaved which indicates if persistent file
//        was saved. If true, SavePersistent will not be called again.
//        Added LookupByIndex by Prokopec M. (prokopec@algo-hk.cz)
//        Fixed 'List index out of range' while defining fielddefs with attribute hidden.
//        Bug reported by Adi Miller (dontspam@il.quest.com-adi)
//        Fixed so its ok to modify a field during OnCalcFields without putting
//        the dataset in edit mode.
//        Fixed problem with array and ADT fields. Bug reported by (huqd@mail.csoft.com.cn)
//
// 3.04   Fixed locking problem with resolver on table with attached tables and datacontrols
//        in threaded environment.
//        Modified InsertRecord, ModifyRecord, DeleteRecord, UnmodifiedRecord to
//        allow for Retry and State variable arguments.
//
// 3.05   28. Aug. 2002
//        Fixed autoinc population when using attached tables.
//
// 3.06   26. Sep. 2002
//        Added OnGetValue event to TkbmCustomResolver which is called when
//        the resolver requests the new value for a field.
//        Fixed endless loop in TkbmIndexes.DeleteIndex. Reported by
//        Markus Dütting (duetting@cosymed.de)
//        Removed ClearRange in SwitchToIndex to support keeping range while
//        switching index.
//        Made CheckPointRecord public.
//        Added support for ftOraBlob and ftOraClob field types for level 5+ compilers.
//
// 3.07   8. Nov. 2002
//        Fixed case bug confusing BCB. Declared RollBack implemented Rollback.
//        This lead to A/V's of adjacent functions in BCB.
//
// 3.08   11. Nov. 2002
//        Fixed compile bug in kbmMemTableReg.pas for LEVEL4 compilers.
//        Fixed small inconsistency in SequentialSearchRecord reported
//        by Markus Dütting (duetting@cosymed.de)
//        Fixed bug where OnGetValue was called in GetOrigValues instead
//        of GetValuesByName. Problem reported by WangWH (wangwh66@yahoo.com).
//        Fixed so setting filtered to same value as it already has is ignored.
//
// 3.09   15. Maj. 2003
//        Fixed missing AfterScroll event in GotoKey/FindKey/FindNearest.
//        Bug reported by Bill Miller (wcmiller@marimyc.com)
//        Fixed occational A/V on rollback of inserted values.
//        Bug reported by Gert Kello (gert@gaiasoft.ee)
//        Added Swedish resource file by Henrick Hellström (henrick@streamsec.se)
//        Optimized binary search DoRespectFilter and fixed bug in some cases
//        Resulting in finding a filtered records.
//        Bug reported by Artem Volk (artvolk@softhome.net)
//        Changed to disable filter when calling Reset.
//        Reported by Marco Kregar (mk_delphi@yahoo.com)
//        Fixed SetRange, SetKey etc. throwing exception when dataset readonly.
//        Bug reported by Ping Kam (pkam@quikcard.com)
//        Updated Spanish resource file by Francisco Armando Dueñas Rodríguez
//        (fduenas@flashmail.com)
//        Updated Italian resource file by Alberto Menghini (alberto@asoft.it).
//        Changed not to move DefaultExpression to LargeInt fields due to
//        Borland not fully having implemented largeint support in variants.
//        Changed OnCompareFields to include a AFld:TField value.
//        Notice that this breaks existing code which need to add that extra
//        argument to their code.
//        Fixed bug which could Result in incorrect sorting of entries when
//        adding records.
//        Fixed bug in SaveToDataset which would cause non graceful exception
//        in case destination table could not be opened.
//        Fixed bug not loading last field in certain cases in CSV format.
//        Fix by Wilfried Mestdagh.
//        Published AllDataFormat.
//        Altered table designer to better list tables based on BDE alias.
//        Disabled platform warnings and hints.
//        Removed Application/Forms/Dialogs dependencies.
//        Extended AddFilteredIndex to accept an optional FilterFunc argument.
//        The filter function is of format:
//            TkbmOnFilterIndex = procedure(DataSet:TDataSet; Index:TkbmIndex; var Accept:boolean) of object;
//        Enhanced AddFiltered to allow for empty filterstring.
//        Fixed Search which would not correctly search on current descending index unless
//        complete correct field specifiers was given. Now use field specifiers for
//        current index if fields matching.
//        Added better error message for when trying to do LoadFromDataset with options for append and structure.
//
// 3.10   16. May 2003
//        Reimplemented InternalHandleException. Altered how it behaves under Kylix.
//        Fixed Notification which would give problems if the same streamformat was
//        attached to two StreamFormat properties at the same time.
//        Fixed persistency problems.
//
// 3.11   29. June 2003
//        Added MergeOptionsTo to TkbmFieldList.
//        Changed to TkbmIndexes.Search combine given field options with index field options.
//
// 3.12   30. June 2003
//        Made TkbmCustomDeltaHandler.Dataset writable.
//        Added mtufDontClear to TkbmMemTableUpdateFlags which if set, avoids clearing
//        out fields which are not to be set by UpdateRecords.
//        Changed to guarantee that AfterLoad is called in InternalLoadFromStreamViaFormat.
//        Bug reported by Vladimir Ulchenko (zlojvavan@bigfoot.com).
//        Fixed bug checking for number of fielddefs in CreateTable. Will now
//        for sure complain if more than KBM_MAX_FIELDS have been defined.
//        Fixed range error in CodedString2String. Bug reported
//        by Karl Thompson (karlt@pine-grove.com).
//        Changed several methods incl. CopyRecords from virtual to dynamic
//        for BCB only to solve BCB compile bugs.
//        Fixed counting when not accepting a record in CopyRecords and UpdateRecords.
//        Bug reported by Nick (nring@smf.com.au)
//        Fixed bug in SequentialSearchRecord with a missing pair of paranthesis.
//        Bug reported by winsano (llob@menta.net).
//        Modified TkbmIndexes.Search to always try to use matching index even if
//        its not current. Contributed by Markus Dütting (duetting@cosymed.de)
//        Added AutoAddIndex property (default false) which if set to true
//        will automatically add an index during a search if none are available.
//        Use cautiously since you could end up with lots of indexes that have to
//        be maintained. Contributed by Markus Dütting (duetting@cosymed.de)
//
// 3.13   Fixed 'List out of bounds' bug in TkbmFieldList.MergeOptionsTo
//        when the two lists was not of same length.
//
// 3.14   Fixed locate/search operations on fields which are not indexed.
//        Bug reported by Ole Willy Tuv (owtuv@online.no).
//
// 4.00   Added DoOnFilterRecord virtual method. Suggested by Neven MacEwan (neven@mwk.co.nz).
//        Fixed enforcing unique index checking in AppendRecord. Bug reported by
//        Thomas Wegner (thomas@wegner24.de)
//        Performance optimized kbmMemTable. Now much faster for many operations.
//        Fixed several indexing bugs.
//
// 4.01   Fixed compiler warning in ParseNode.
//        Fixed A/V in Commit. Reported by Willi (nonn@inwind.it)
//        Added new Pro features for benefit of valid kbmMW commercial license holders.
//        These features significantly speed up memorytable operations, specially
//        for large amounts of data.
//        Holders of valid kbmMW commercial developer licenses, please remember to
//        define HAVE_COMMERCIAL_KBMMW_LICENSE in your projects conditionals/define
//        to benefit from the Pro speed. Also remember to download the Pro features
//        seperately from C4D.
//        Updated Spanish ressource file by Francisco Armando Dueñas Rodríguez (fduenas@flashmail.com)
//        Added support for ftBCD fields in PopulateField by Arming (armintan@263.net)
//        Fixed ftGUID support.
//        Fixed locating on partial indexes, using index.
//
// 4.02   Fixed a serious string compare bug.
//        Fixed grid repositioning issue when deleting record. Reported by Rado Antloga (radovan.antloga@siol.net)
//        Added the darned BCB virtual/dynamic fix in all places in TkbmCustomMemTable.
//        Fixed USE_FAST_MOVE definition which were misspelled several places.
//
// 4.03   Fixed null comparison bug in TkbmCommon._InternalCompareRecords reported by
//          Emmanuel TRIBALLIER (emmanuel.triballier@ebp.com).
//        Added call to clearbuffers when setting filter or enablefilter to force
//        a complete refetch of all TDataset buffers.
//        Fixed installation in Level 4 compilers (ftGUID field type not supported).
//
// 4.04   Fixed cancel resets cursor to first row. Reported by Rado Antloga.
//        Removed ClearBuffers from SetRecNo. Reported by Rado Antloga.
//        Altered AddIndex and AddFilteredIndex to be functions returning the
//        added index.
//        Fixed enabling filtering before table opening. Reported by peter.andersen@dsd.as
//        Fixed CompareFields ftTime comparison.
//        Fixed nasty memory leaks in PrepareKeyRecord, LocateRecord and LookupByIndex.
//        Leak reported by Wilfied Mestdagh.
//        Fixed coNOT operator not correctly returning Null instead of boolean value
//        when arguments is null. Bug reported by Peter Andersen (peter.andersen@dsd.as)
//        Fixed bug when calling Sort or SortOn while in Edit mode. Problem reported by
//        Francesco Beccari (fbeccari@interplanet.it).
//        Fixed compilation problems in Kylix.
//        Fixed Commit and Rollback transaction handling.
//
// 4.05   Added support for sfLoadIndexDef in sfIndexDef in binary and csv streamformats.
//        Thus its possible to not load indexes.
//        Added new boolean property RangeIgnoreNullKeyValues which controls if
//        Null values are to be ignored or not in the key values for ranges.
//        Default true.
//        Fixed bug with reposition on deleted record. Fix by Wilfried Mestdagh.
//        Fixed bug when Filtered:=true and Filter<>'' at designtime. Then
//        opening table do not correctly set Filter. Reported by Rado Antloga.
//        Fixed bug clearing main filterexpression when a filtered index is added.
//        Added IgnoreErrors argument to CopyRecords. Will ignore errors in assigning
//        field values and posting the record and continue to operate.
//        Added mtcpoIgnoreErrors and mtcpoDontDisableIndexes to TkbmMemTableCopyTableOptions.
//        Updated LoadFromDataset to respect mtcpoIgnoreErrors and mtcpoDontDisableIndexes.
//        Added CopyOptions argument to SaveToDataset. Only mtcpoIgnoreErrors is currently supported.
//        Added true ftWideString support (Unicode) also for csv and binary streamformats (LEVEL6+ compilers only).
//        Pre LEVEL6 compilers will convert the widestring to a normal string using the AsString field method.
//        Added AddIndex2 and AddFilteredIndex2 (pre LEVEL5 compilers) and overloaded AddIndex/AddFilteredIndex
//        (LEVEL5+ compilers) which includes a TUpdateStatusSet argument.
//        This allows for the index to show or filter specific updatestatus settings incl. deleted.
//        Added Undo to revert to older record version.
//        Disabled warnings in ParseNode due to unremovable warning. By Wilfried Mestdagh.
//        Fixed _InternalCompareRecords to handle comparing null values correctly on descending indexes.
//        By Rado AntLoga.
//        Added TkbmMWIndex.CreateByIndexDef (Pre LEVEL5 compilers) and TkbmMWIndex.Create overloaded
//        (LEVEL5+ compilers) which allow for specifying TIndexDef. By Rado Antloga.
//        Changed Refresh to Resync in SwitchToIndex and UpdateIndexes. By Rado Antloga.
//        Changed so fields are default visible when attaching to another table. By Julian Mesa.
//        Added conditional CreateFields in InternalInitFieldDefs
//
// 4.06   Fixed not to default HAVE_COMMERCIAL_KBMMW_LICENSE definition.
//        Fixed problem with indexes not setup to include inserted, modified and unmodified records by default
//        using one of the TkbmIndex constructors.
//        Updated demo to fix compilation problem regarding AddFilteredIndex.
//        Fixed the dreaded varlength getting null when calculated fields before varlength fields.
//
// 4.07   Fixed version string :|
//        Rewrote WideString support as the other way broke lots of other fieldtypes... Sorry folks!
//        Now done the 'right' way via the DataConvert method which of some strange reason
//        do not natively support WideString. Fortunately its possible to override it and fix it.
//        Added functions for setting fielddata and status for any version of a record. The old value is returned.
//        function SetVersionFieldData(Field:TField; AVersion:integer; AValue:variant):variant;
//        function SetVersionStatus(AVersion:integer; AUpdateStatus:TUpdateStatus):TUpdateStatus;
//        Fixed D5 problems regarding overloaded AddFilteredIndex.
//
// 4.08   Added support for mtcpoStringAsWideString copy flag which if set means that all
//        string and memo fields will be created as widestring instead of string.
//        Added support for mtcpoWideStringUTF8 which means that all loadfromdataset/
//        savetodataset operations automaticaly includes UTF8 conversion.
//        Fixed bug with regards to loading widestring data in CSV streamformat.
//        Added sfDataTypeHeader to binary stream format which will allow storing
//        datatypes in the header. The information can be used to skip fields later on.
//        Notice that if this is used, it will break backward compatibility with older
//        binary files and with older versions of kbmMemTable.
//        Fixed repositioning problem after UpdateIndexes was run.
//        Fixed Kylix 3 compatibility.
//
// 4.08b  Fixed D4 and D5 compatibility.
//
// 5.00 beta 2   Oct. 21 2004
//        Fixed resetting IsDataModified correctly after a Checkpoint.
//        Altered CheckpointRecord to a function returning new current updatestatus.
//        Fixed setting calculated fields on readonly table.
//        Added support for D8.Net (fully safe and managed code!)
//        Added support for Delphi 2005 Win32 and dotNet.
//
// 5.00 beta 3  Dec. 1. 2004
//        Fixed several bugs related with dotNet port.
//        Added Delphi for dotNet demo app. Notice that there seems to be bugs
//        in some parts of the BDE for dotNet port and possibly some Delphi 2005.Net related
//        event bugs which can surface when copying from another dataset
//        (enablecontrols in LoadFromDataset). The exact circumstances are still being
//        investigated.
//        Further the icons still dont show up as expected. Will also be investigated further.
//        Finally WideString fields do not work as expected at the moment.
//
// 5.00 beta 4  Apr. 8. 2005
//        Fixed Locate when using a null value due to bug in PopulateField.
//        Fix by Carsten Schuette (carsten.schuette@ewetel.net)
//
// 5.00 RC1 Apr. 19. 2005
//        Upgrading to release candidate 1.
//
// 5.00 May 28. 2005
//        Modified AssignRecord not to use AsString but instead .Value.
//        Promoted CopyFieldProperties and CopyFieldsProperties to be public methods.
//
// 5.01 Aug 22. 2005
//        Added Cancel to TkbmCustomMemTable.Rollback. Bug discovered by
//        Vladimir Ulchenko <zlojvavan@bigfoot.com>
//        Added BeforeRecord and AfterRecord virtual methods to TkbmCustomDeltaHandler.
//        Fixed A/V in CheckpointRecord as reported by Henri Gourvest.
//        Fixed A/V after using EmptyTable as reported by several and fix suggested by
//           Vladimir Ulchenko.
//        Added GotoCurrent method.
//        Fixed not saving field invisible status in CSV format.
//        Fixed ftDate and ftTime fields size to be 4 bytes (longint) instead of 8
//           to solve A/V and data corruption problems. Borland use a partial
//           TDateTimeRec (half of it only) to store ftDate and ftTime values in
//           and since calculated fields use Borlands field size calculation,
//           we had to adjust kbmMemTable to adhere to same size.
//
// 5.02 Oct. 3. 2005
//        Fixed A/V in PopulateField for WideString field types. Fixes for example
//           filtering and locating on widestring fields. Problem reported by several.
//        Fixed Widestring fields in .Net.
//        Added GotoNearest method.
//        Fixed 'List out of bounds' when Rollback leaves no records.
//        Fixed implementing check for invalid values in calls to CurrToBCD.
//           Now the precision will at least be decimals after comma + 1 digit.
//        Fixed using ixCaseInsensitive and ixDescending in CreateIndex. Before
//           The wrong field lists where referenced. Now the current indexed field
//           list is used. Report by Carsten Schuette.
//
// 5.02b Oct. 7 2005
//        Fixed bug introduced in 5.02 regarding PopulateField and ftString which
//           mysteriously didnt show up in internal tests but which produced all
//           sorts of A/V's in true world apps. Reported by several.
//
// 5.50 Dec. 14 2005
//        Added support for BDS 2006 and 4 new fieldtypes:
//           ftFixedWideChar,ftWideMemo,ftOraTimeStamp,ftOraInterval
//        Fixed Notification when freeing instance that was created
//           as owned by another TComponent or container.
//        Fixed bug related to assignment of values of fields of type ftLargeInt.
//
// 5.51 Dec. 30 2006
//        Fixed bug in CSV streamformat when used in D2006/Win32 due to
//           several TField properties changed to WideString.
//        Removed SetLength(s,0) as that could cause invalid pointer operation.
//        Fixed support for ftFixedWideChar and ftWideMemo for BDS2006.
//        Changed PopulateField for ftLargeInt to improve precision.
//
// 5.52 Feb. 23 2007
//        Fixed WideString support for BDS2006.
//        Fixed filter expressions for .Net (may still be issues in special cases. If so report them.)
//        Fixed respecting case when looking for appropriate index on searches.
//        Fixed using given search options when doing sequential search.
//        Fixed filtering on smallint field.
//        Changed commit not to drop older versions as that interfered with deltahandling.
//        Fixed not setting UpdateStatus on a record when its inserted for .Net.
//        Fixed marking fielddefs as internalcalc when they are based on an internalcalc field.
//        Fixed VariantToStream in .Net which truncated one byte affecting AllData.
//
// 5.53 Mar. 23 2007
//        Added support for D2007.
//        Added support for FreePascal (FPC) (2.1.3 or newer required).
//
// 5.54 Jun. 05 2007
//        Fixed bug when editing and table was locked resulted in missing
//        versioning of record and no IsDataModified/Modified flags set.
//        Fixed bug in kbmMemBinaryStreamFormat when compiled with FPC.
//        Fixed automatically setting Delphi mode when compiling with FPC.
//        Fixed RecordCount resetting Modified when filtered and called from an
//        event during Edit resulting in dataloss.
//
// 5.55 July 24 2007
//        Updated CSV component to v. 3.10. It contains the following:
//        Added support for ignoring commented lines starting with CommentChar.
//        Added support for changing escape character % to something else via EscapeChar.
//        Added OnGetLine event.
//        Added support for skipping empty lines instead of simply stopping import.
//        Added support for loading CSV files without knowing structure.
//          Fields will be defined either based on names in the first line, or as Field_x.
//          Each field will be defined as a 255 (default) character stringfield why small performance mode
//          is an advantage. Set sfDef to sfLoadAutoLayout
//        Added DefaultFieldStringSize (default 255).
//        Fixed issues with automatic index addition where locateoptions could differ.
//
// 5.60 Aug 16 2007
//        Added support for hooking into the TMasterDataLink used by kbmMemTable for M/D scenarios.
//        Added support for not refreshing detail table in case edit is done on master record,
//          unless the keyfields are edited.
//        Changed .Net handling of filterexpression.
//
// 5.70 May 11 2008
//        Added support for copying calculated and lookup fields from another dataset as regular
//        datafields. Use copy options: mtcpoLookupAsData, mtcpoCalculatedAsData.
//        Fixed bug in WideStringToBuffer when WideString is an empty string on
//          LEVEL10 compilers and up.
//        Fixed potential leak in CloseBlob.
//        Fixed 'object is not an instance' bug in .Net in relation to deleting
//          a record and afterwards reloading indexes.
//        Fixed expression filters for D2007.Net.
//
// 5.71 May 12 2008
//        Fixed repositioning issue when AutoReposition=true. The bug were showing
//          up after inserting new records and then editing one of them. Position
//          after Post were incorrect.
//
// 5.72 May 18 2008
//        Fixed respecting IgnoreReadOnly when adding/deleting/updatingnrecords.
//        Changed way Pro Additions is linked in to remove requirement for
//        Pro Additions definitions.
//        If Pro Additions is licensed, its required to use Pro Additions v.5.72!
//
// 5.80 Oct 17 2008
//        Added support for D2009
//        Split up projects to remove requirement for seperate Pro Additions package.
//        Now use relevant Standard Edition or Professional Edition installer.
//
// 6.00 Dec 22 2008
//        Fixed project files for several SKUs.
//        Added check for AddIndex actually being passed some field names.
//
// 6.01 Mar 25 2009
//        Provided some more flexible facilities for controlling streamformatters upon extension.
//
// 6.10 July 1 2009
//        Fixed Unicode filter for D2009.
//
// 6.20 July 20 2009
//        Modified the fielddef initialization code to be more flexible.
//
// 6.20b Aug 12 2009
//        Fixed bug reg. fielddefs introduced in 6.20, that made kbmMW fail.
//        Added support for D2010.
//
// 6.30  Dec 20 2009
//        Added CopyRecord method that copies one record to another place in the table.
//         Require the roworder index to be selected. If any indexes is defined as unique
//         and enabled then the copy will fail.
//        Fixed destination problem in MoveRecord.
//
// 6.31  Jan 18 2010
//        Fixed bug specifying filter expression before open didnt correctly filter
//          upon open.
//
// 6.40  June 16 2010
//        Added pure C++Builder 2010 support.
//        Added prerequisite methods for kbmMW v. 3.40.
//
// 6.50  Aug 5. 2010
//        Removed call to fielddefs.update and indexdefs.update in Reset.
//        Added CopyFieldDefs procedure to copy fielddefs from one
//          fielddefs collection to another (as a real copy, not only a reassignment)
//
// 6.50.01 Aug 22. 2010
//        Changed frOraClob and ftOraBlob handling for environments that supports
//          asBytes method.
//        Added support for automaticaly adjusting displaywidth on string fields
//          when loading data via a streamformatter. Check sfDisplayWidth property.
//        For newer Delphi/C++ versions, added internal FormatSettings property
//          to use for controling various date/time/number to string and back conversions.
//        Added Delphi XE/C++Builder XE support.
//
// 6.51.00 Oct 16. 2010
//        Added support for Oracle like null values in indexes (null is perceived as very large value)
//        Added support for allowing multiple null values in unique indexes.
//        Dropped original Quicksort source in favor of default Fast Quicksort algorithm.
//        Not supporting mtcoIgnoreNullKey and field modified N in indexes.
//           Only to be optionally used in searches and then only with care.
//        Fixed various things.
//
// 7.00.00 beta 1 Dec 8. 2010
//        Enhanced index search facilities and derived indexes.
//        New support for index optimized ranges (SetRange...)
//        New support for index optimized master/detail.
//        New index optimized GroupBy method. Provide with grouping fieldname list
//          and aggregate fieldname list. Use fieldmodifiers MAX, MIN, AVG, SUM,
//          COUNT, USR1, USR2 og USR3 to describe aggregation function.
//          If USR1,USR2 or USR3, the OnUserAggregate method of the destination
//          table will be called when needed.
//          If an empty groupfields value was provided, the total dataset (according to current index)
//          will be used as one group.
//
// 7.01.00 Jan 28. 2011
//        Fixed A/V introduced in 7.00.00
//        Improved derived index handling including keeping track of indexes being
//          destroyed. If a base index is destroyed, so is its child indexes.
//        Added support for calling Sort/SortOn while master/detail is active.
//          Notice that when the masterrecord is changed, the Sort/SortOn index will
//          be destroyed, and a new one must manually be created to have the sort
//          active again.
//        Fixed unicode bug in CSV streamformat for LEVEL13 compilers (D2009 and up).
//        Changed GroupBy so grouped fields are not automatically included in
//          resulting table. Instead specify the grouped fields you want to include
//          in the aggregate field list part of the call to GroupBy.
//          Eg. GroupBy('Color','Color:COUNT'); only shows a result column Color_COUNT.
//              GroupBy('Color','Color;Color:COUNT'); displays both Color column and a count column.
//        Fixed serious GroupBy bug and performance optimized it.
//        Added new sfDef flag sfLoadByFieldNo, which will load data from CSV into
//          predefined fields without requiring field name match. Number of fields
//          indicates how many fields from the CSV file (counted from left) that
//          will be loaded.
//
// 7.02.00 Feb 2 2011
//        Fixed codegen bug where inlined method resulted in critical sections
//          not being released correctly. Recommended update if you are using v. 7!
//        Fixed master/detail when DetailFields not specified.
//        Fixed FindKey/GotoKey/FindNearest
//
// 7.03.00 Mar 23 2011
//        Added compare options (fieldmodifier): mtifoIgnoreNonSpace (S),
//                                               mtifoIgnoreKanatype (K),
//                                               mtifoIgnoreSymbols  (I),
//                                               mtifoIgnoreWidth    (W)
//        Fixed master/detail bugs shown in testcases by Brandon Rock.
//        Fixed bookmark bugs shown in testcase by Saulius Vabalas.
//        Fixed issue that kbmMemTable was registered as non ObjectView, which
//          essentially means that a seperate flat field list needs to be
//          updated. However due to the list not being flexible enough to
//          update based on an exiting fielddefs list (without reupdating that
//          fielddefs list which will cause issues in kbmMW), the solution
//          is to identify kbmMemTable as being object aware, although it
//          probably doesnt support it all the way (long time no testing in
//          that area).
//        Fixed attached to issues.
//        Fixed autocreating index when no index define upon switching index.
//        Added sfLoadAsASCII to CSV load flags for property sfLoadLocal (D2009+).
//          Set to true if to load pre D2009 CSV files containing non 7 bit ASCII data.
//        Other minor fixes.
//
// 7.05.00 Oct. 21 2011
//        Added support for Delphi XE2 Win32/Win64/OSX32 and C++Builder XE2 Win32
//        Dropped official support for Kylix and pre Delphi 7.
//        Removed all .Net related code.
//        Improved support for FPC.
//        Dropped TkbmThreadDataSet. It has not practical use for a long time.
//        Fixed bug when setting detail fields in master/detail situation and
//          master not active.
//        Improved range handling by new range index.
//        Added support for Sort on a detail table in a master/detail relation.
//           It require that DetailFields is used instead of IndexFieldNames for
//           the master/detail setup.
//        Improved dependant and base index handling.
//        Fixed sfIgnoreRange and sfIgnoreMasterDetail save flags.
//        Fixed FieldDefList updating before field binding. It undos the
//           previous requirement to have ObjectView = true.
//        Added support for Delphi XE2, Win32/Win64 and OSX32.
//
// 7.10.00 Oct. 26 2011
//        Added kbmSQL support to kbmMemtable package.
//        kbmSQL will no longer be its own package, and will now
//        take part of the kbmMemTable version number.
//
// 7.11.00 Nov. 26 2011
//        Fixed sequential search bug resulting in not always finding relevant data.
//        Fixed closeblob bug when table has been destroyed before application blob field.
//        Fixed possible A/V in some situations where indexes were cleared 2nd time.
//        Fixed pre D2009 compilers loading ASCII CSV data.
//        Added AffectedRows to TkbmSQLTable. It can be queried via the TkbmMemSQL.Tables property
//          to figure out how many rows was affected by the insert/update/delete operation.
//        Updated FPC support to also support SQL features.
//
// 7.12.00 Dec 3. 2011
//        Fixed search after setting mastersource to nil on detail table.
//
// 7.12.10 Dec 5. 2011
//        Fixed kbmSQLMem bug where it didnt parse floating point constants with decimal digits correctly.
//
// 7.13.00 Feb 12. 2012
//        Fixed kbmSQLMem memory leak.
//        Added IF(condition,true_result,false_result) function to kbmSQL.
//          Eg. SELECT if(fld2 > 40,fld3+3,fld4) FROM ATable
//          Outputs one field which contents depends on fld2.
//        Added CASTTONUMBER(val), CASTTODATETIME(val) and CASTTOSTRING(val)
//          SQL standard functions.
//
// 7.14.00 Feb 26. 2012
//        Added handling of NULL fields in SQL unary, binary, like and in situations.
//          If an operand is null, the result of the substatement will be null.
//          If its a criteria for SELECT, then the record will be skipped.
//        Fixed issue loading last character in CSV streamformat.
//        Added support for DISTINCT in SELECT and aggregate SQL functions.
//
// 7.15.00 Mar 15. 2012
//        Added support for HAVING in GROUP BY statements
//        Fixed memory leaks.
//        Fixed DISTINCT bug causing problems in GROUP BY
//        Fixed very old bug which has only surfaced now in relation to Mac in
//          the Professional version of kbmList.pas.
//        Fixed missing conversion bugs in CASTTOSTRING and CASTTODATETIME SQL functions.
//        Fixed event issues in 64 bit compilation.
//        added Delete(ATable:TkbmSQLTable) and Delete(AName:string) to TkbmSQLTables.
//
// 7.16.00 May 23. 2012
//        Fixed sorton index memory leak.
//        Fixed double free bug of roworder index.
//        Fixed issues in CastToString (resulted in 0 length result fields)
//        Fixed issues with constant strings as result for a field. Eg. select 'abc' as field1 from ....
//        Fixed wrong precompiler conditional test for windows.
//        Fixed kbmSQL compilation in pre level 9 compilers (D2007 and earlier).
//        Added Aggregate function. Accepts aggregate information like GroupBy on all records in the current index.
//          Specify list of fields with fieldmodifiers for how to aggregate. Fields seperated by semicolon (;)
//          Eg. v:=Aggregate('field1:MIN;field2:AVG');
//          Returns variant array [0..1] with Min and Avg values for the given fields.
//          If only one field is specified the result is returned directly (not stored in an array).
//          Use fieldmodifiers MAX, MIN, AVG, SUM, COUNT, USR1, USR2 og USR3 to describe aggregation function.
//          If USR1,USR2 or USR3, the OnUserAggregate method of the source table will be called when needed.
//        Added support for STDDEV aggregate function (fieldmodifier STDDEV).
//        Fixed C++ header issue with EOF as var argument in eventhandlers. Renamed to AEOF.
//        Added SQL support for boolean constants TRUE and FALSE to SQL supporting syntaxes:
//          x = TRUE/FALSE, x IS TRUE/FALSE, x IS NOT TRUE/FALSE
//        Fixed SQL IS NULL and IS NOT NULL support.
//        Added SQL support for STDDEV function with same syntax as other aggregate functions (COUNT, AVG, MIN, MAX, SUM)
//        Added SQL functions MID(s,pos,count), LEFT(s,count), RIGHT(s,count). pos>=1.
//
// 7.17.00 Jun 19 2012
//        Fixed raising exception if SQL contains unknown field.
//        Added support for Count(distinct X) on string fields. Eg. Count(distinct left(fld1,4)).
//
// 7.18.00 Jul 11 2012
//        Fixed aggregate functions on IF. Eg SELECT SUM(IF(fld2=1,0,1)) AS SOMEFIELD ....
//          The fix actually fix all custom functions that returns ftVariant.
//          Its a requirement that those functions implements foDataType that returns
//          a true (non ftvariant) datatype based on arguments.
//        Fixed depending indexes bug as demonstrated by Steve Spralja in LoadFromDataset
//          on dataset with defined master/detail and sorton.
//
// 7.20.00 Sep 16 2012
//        SQL: Fixed bug with multiple AND/OR parts in statement.
//        SQL: Fixed bug related to * (all fields) handling with aggregate functions.
//        SQL: Fixed bug related to datatypes of source fields used by aggregate functions.
//        SQL: Fixed bug when DELETE or UPDATE statement given without WHERE clause.
//        SQL: Added Length(string) to standard functions.
//        SQL: Fixed bug: SELECT sum(fld4)+sum(fld5) FROM table GROUP BY fld3
//        SQL: Fixed bug: SELECT count(*)+3 FROM table
//        SQL: Fixed issues with HAVING
//        SQL: Added SQL support for LIMIT and OFFSET. Eg. SELECT * FROM table LIMIT 10 OFFSET 5
//        SQL: Improved internal handling of field aliases and unique field names.
//        SQL: Optimized performance on accessing source fields.
//        SQL: Added support for conditional aggregates like: SELECT SUM(IF(fld5>5,1,0)) FROM table GROUP BY fld5
//        Added support for Delphi XE3.
//        Dropped support for Delphi versions earlier than D2009.
//
// 7.21.00 Sep 22 2012
//        Fixed InternalAddRecord XE3 related bug.
//        Fixed empty XE3 dproj files.
//        SQL: Fixed following alias syntaxes:
//             SELECT fld2+1 as fld2a FROM Table1 ORDER BY fld2a DESC
//             SELECT fld2+1 as fld2 FROM Table1
//             SELECT fld1,fld2,fld3,fld3 AS SomeField1,fld4 AS SomeField2,fld5 FROM table1 WHERE fld5 IN (5) ORDER BY fld2,SomeField2
//             SELECT fld2 as Field2, fld3, sum(fld5) as fld5, Sum(fld2) as SomeField1, Sum(fld3) as SomeField2 FROM table1 GROUP BY Field2, fld3
//        Added Simplified Chinese resource file.
//
// 7.21.10 Oct 20 2012
//        Fixed index out of range bug introduced in 7.21.
//
// 7.22.00 Nov 1 2012
//        Fixed bookmark handling for XE3.
//        Fixed Win64 compilation.
//
// 7.22.10 Nov 5 2012
//        Added support for arguments AForce:boolean and AMarkAsInserted:boolean to CheckPoint.
//          AForce (default false) controls if to checkpoint records marked not for checkpointing.
//          AMarkAsInserted (default false) controls if to mark checkpointed record as inserted or unmodified.
//
// 7.23.00 Feb 20 2013
//        Fixed master/detail issues with depending indexes.
//        Improved AppendRecord to bias the index updates towards end of index.
//        Un-inlined pro version of kbmMemMove due to reported issues with XE3.
//        Changed use of dynamic to virtual for BCB.
//        Fixed issue with indexes added with autoaddindex that didnt base its
//          index on the current index (for example the detail index in a master/detail
//          relation)
//        Added support for append bias for insert/post.
//        Added optional argument to SQL function CastToString indicating minimum
//          size of the resulting field.
//        Changed so CopyRecords accepts copying data into datetime/date/time fields
//          directly from float/int/smallint/currency fields without going thru
//          string conversion.
//        Fix in CreateFieldDef. Name is blank in FPC so skip temporary renaming.
//        Fixed leaked detail index.
//        Renamed SQL SQR function to SQRT which is an "industry standard" naming.
//        Added SQL Exp,Min,Max,Avg,Sum,Abs to SQL standard function library.
//        Added Evaluate function to TkbmMemSQL to allow evaluation of expressions
//          like: v:=memsql.Evaluate('10+2/3') supporting the full expression
//          functionality of kbmSQL.
//        Added Calculate function to TkbmMemSQL that works like Evaluation except
//          boolean operators are not allowed.
//        Added optional argument to Evaluate and Calculate. When set to true
//          only syntax checking happens. Result is true if syntax check ok, else false.
//        Added support for variables as part of SQL/evaluation/calculation expressions.
//          Eg. '10+$somevar+20'. On expression evaluation time, OnGetVariableValue
//          event will be triggered with somevar as an argument.
//          On compilation/syntax checking time, OnGetVariableMetaData event will be
//          called allowing developer to do early validation of if a variable exists.
//        Added support for || operator (string concatenation).
//          It automatically converts data to string before concatenation.
//        Added support for OnGetVariableMetaData event for obtaining width and
//          datatype for a variable. Its only required to define the event if the
//          variable is to participate in a SQL result field.
//        Added grouping support for custom SQL functions.
//          Defined functions can be enabled/disabled using a partial search.
//          Eg. kbmSQLRegisteredFunctions.DisableGroup('MATH') will disable all
//          math functions, while DisableGroup('MATH.LOG') will only disable
//          functions part of the logarithmic math group.
//        Added Abs(v), LeftPad(v,char,count), RightPad(v,char,count) custom SQL
//          functions to kbmSQLStdFunc.
//        Added support for understanding "field references" as variable names
//          when using the Evaluate/Calculate method (which never operates on a table).
//          Hence its optional to put $ infront of variable names when used for
//          expression evaluation via the Evaluate/Calculate method.
//        Added static handling of decimal comma in SQL (its in our SQL always a dot).
//        Improved SQL error handling.
//        Added new OnError event to TkbmMemSQL for detailed info about syntax/semantic
//          errors.
//        Fixed crashes after expression parsing have failed.
//        Fixed leaks after failed expression parsing.
//        Fixed wrong multiple subtraction and division association in SQL
//          (was associated right instead of left resulting in wrong calculation)
//        Added argument SyntaxCheckOnly to Evaluate and Calculate. If set to
//          true, returns true or false depending if syntax is ok.
//        Added support for gracefully validating variable names and function
//          names during compilation without throwing exception.
//
// 7.24.00 Mar 19 2013
//        Added function DeleteRecords which deletes all records visible on the
//          current index/range. The function returns the number of records deleted.
//        Added overloaded method SetRange which accepts a string of fieldnames, and
//          a variant array for start values and another for end values.
//          It defines a range index where only records which have field values within
//          the start/end values are included. Use CancelRange to release the range restriction.
//        Added DeleteRange which accepts arguments like SetRange above, and deletes all records
//          within the range, and then automatically cancels the range again. Returns number of
//          records deleted.
//        Fixed FPC compilation error.
//        Added AOptions:TkbmSQLExpressionOptions with default value [seoOnlyNumericExpressions]
//          to Calculate and Evaluate methods of TkbmMemSQL. When seoOnlyNumericExpressions
//          is set, then expressions only support numeric sub expressions and result.
//        Fixed bugs in the SQL compiler/parser parts that Calculate and Evaluate use, to
//          allow for constantly compiling statements, when keyed in character by character
//          so an on the fly status can be shown indicating if the statement is valid or not.
//
// 7.25.00 Mar 28 2013
//        Modified derived index handling, so it supports respecting the full sort order of
//          the index its based on.
//
// 7.25.10 Mar 28 2013
//        Fixed unfortunate compilation bug introduced in 7.25.00
//
// 7.26.00 Apr 11 2013
//        Fixed index bug introduced in 7.25.00.
//        Modified to throw an exception if trying to add index on blob fields.
//        Added new Extract method. It simplifies extraction of selected fields to a TStrings.
//          It accepts a list of fields to extract and a format string and optional formatsettings.
//          A float/date/datetime/time field can be postfixed with one of the field options
//            DATE, TIME or DATETIME to have the value formatted as a date/time string.
//          The AFormat argument takes a string formatted as the Format function.
//          Eg. mt.Extract('fldInt;fldFloat;fldDateTime:DATETIME;fldString',
//                         astringlist,
//                         'I:%0.4d F:%g D:%s S:%s');
//          It populates the stringlist (astringlist) with strings formatted as:
//            I:0222 F:10293.37478584 D:05-02-2010 12:00 S:Some text from stringfield
//        Added SQL function Chr(byte) which returns a one character string.
//            Eg. Chr(65) returns ASCII 'A'. It accepts unicode numeric values.
//        Added SQL support for RECNO and ROWID built in pseudo fields.
//            RECNO returns the current record number (within the index that is current).
//            The value may change for a particular record if other records are inserted
//            in front of it in the current index.
//            ROWID returns the unique record ID. That value will not change for a record
//            while the source table is open.
//
// 7.30.00 June 6 2013
//        Supports XE4 Win32/Win64.
//        Removed KBM_MAX_FIELDS (256) limitation. Now any number of fields are automatically supported.
//        Fixed simple but serious STDDEV calculation bug.
//        Fixed * (wildcard) bug in SQL.
//        Improved Extract method to respect filters and ignore null field options.
//
// 7.40.00 Dec 18 2013
//        Supports XE5 Win32/Win64/MacOS/Android/IOS.
//        Removed all traces of pre LEVEL13 code.
//	      Improved handling of lookup fields in relation to master/detail and attached tables.
//        Added new kbmFMX control toolkit with dataaware grid, memo and image, and features for media scaling.
//        Added optional mapping argument to LoadFromDataSet, SaveToDataSet, UpdateToDataSet. Allows for field mapping when copying data.
//          Syntax: sourcefield=destfield;sourcefield=destfield....
//        Added support for FormatSettings property in Extract method.
//        Added Sqr(x), Root(x,y), Pow(x,y), Frac(x) standard SQL math functions.
//        Improved null handling for SQL custom functions.
//        Added optional 4.th argument to SQL IF function that is returned in case
//          the condition argument is NULL. IF(condition,iftrue,iffalse[,ifnull])
//        Added support for SQL XOR binary operator.
//        Improved SQL NOT, XOR,OR and AND binary operators to ensure operation is not
//          upscaled to float when providing float on one or both sides to enable
//          understanding for example 0 as false and 1 as true.
//        Fixed A/V in SQL when custom non preregistered function was attempted to be
//          used but not found.
//        Fixed SQL parse tree optimization that could result in illegal tree.
//        Fixed so SQL custom functions MIN, MAX, AVG and SUM are called when
//          2 or more arguments are provided. When only 1 argument is provided
//          those functions will be understood as standard SQL aggregates.
//        Fixed master/detail issue where m/d wasnt respected on initial open until current record has moved.
//        Fixed restoring correct dataset state after Lookup, LookupByIndex,
//        Added support for copying lookup field settings in CopyFieldProperties.
//        Fixed handling of ftLargeInt fields in Extract method for pre level 16 compilers.
//        Fixed bug in SQL when providing empty SQL statement.
//
// 7.40.10 Dec 27 2013
//        Optimized searching record ID when operating on roworder index.
//
// 7.50.00 Mar 30 2014
//        Added support for indexing memo fields.
//        Fixed LIKE filter problem related to Unicode and string/character/pointer conversion.
//        Fixed buffer overflow in PopulateField if populating a string field with larger than
//          field defined size.
//        Added SQL functions ISNULL(..), DATESTRING(..), TIMESTRING(..).
//        Improved handling of Null in SQL functions.
//        Fixed compilation for NextGen.
//        Fixed compilation for XE2.
//        Added kbmIndexableTypes which lists all field types that can be included in an index.
//        Added sfLoadAsANSI to CSV streamformat.
//
// 7.60.00 Apr 17 2014
//        Added support for XE6.
//        Added support for mobile apps using fast TkbmList (kbmMT Pro Edition).
//        Fixed A/V in Load in CSV streamformat.
//
// 7.61.00 Apr 27 2014
//        Fixed handling sort order on additional fields in M/D relation.
//        Fixed filter expression where const expression was of datatype ftShortInt.
//
// 7.62.00 May 12 2014
//        Fixed XE6 field lifecycle issues.
//
// 7.63.00 June 30 2014
//        Fixed ftByte in filter expression.
//        Fixed clearing out all fields when loaddef is set during data load from csv/binary.
//        Fixed clearing out all fields when createtableas.
//        Fixed leak when blob (varlength) field is edited, then cancelled.
//        Fixed bug when SetRange with fields not part of currently sorted index.
//        Added support for latest stable Lazarus (1.2.4) and FPC 2.6.4 (requirement)
//        Added support for using SQL syntax for filtering (default for FPC, optional for Delphi).
//          Undefine KBMMEMTABLE_SUPPORT_EXPRESSIONPARSER in kbmMemTable.inc for Delphi to use it.
//        Fixed AND/OR/XOR casting issue in SQL which showed in FPC.
//
// 7.64.00 Sep 9 2014
//        Added XE7 support.
//
// 7.64.10 Sep 16 2014
//        Accepting boolean values as a numeric value when using SQL Evaluate.
//        Changed seoOnlyNumericExpressions to be happening late instead of while parsing
//          to allow for custom functions returning numeric values based on non numeric parameters.
//
// 7.64.20 Sep 30 2014
//        Accepting upconverting values in SQL to string in binary expressions when needed. Eg. 5+"a" results in "5a".
//        Improved syntax check in evaluate/calculate methods when numericonly is defined and result is determined to be a string.
//        Added support for ftSingle datatype.
//
// 7.64.30 Oct 8 2014
//        Added SQL support for escaping field/table identifiers with [..] and also allowing for constructs like [[abc]]] which is understood
//          as [abc].
//        Added support for MIN/MAX aggregation on string fields.
//        Ensured that null values are not included in aggregation functions.
//
// 7.65.00 Dec 26 2014
//        Fixed bug in creating fielddefs for calculated/lookup field.
//        Fixed issue related to multilevel master/detail relations.
//        Fixed so field lifecycle is always set to automatic upon loading definitions
//           in CSV and binary streamformats.
{$ENDREGION}
{$REGION 'Newest release'}
// 7.66.00 Feb 27 2015
//        Fixed potential read buffer overrun in PopulateField of widestring/string fields.
//        Fixed read buffer overrun in SetFieldData and GetFieldData.
//
// 7.67.00 Mar 12 2015
//        Fixed yet another read buffer overrun bug.
//
// 7.68.00 Apr 12 2015
//        Added XE8 support.
//        Added support for mtifoNullFirst in index definition,
//           which indicates NULL values to be treated lower than any other
//           value (MSSQL style) instead of the default where NULL values are
//           higher than any other value (Oracle style).
//        Updated binary stream format to v. 400. ftFMTBCD fields are now
//           streamed as floating point values. Notice that it will support
//           loading older binary stream versions, but will _always_ save in v400
//           which is not compatible with older software.
//        Added exception when attempting to load a newer binary stream version
//           than what the current software supports.
//        Added new AddIndex2/AddFilteredIndex2 directly taking
//           TkbmMemTableCompareOptions instead of TIndexOptions.
//
// 7.69.00 May 30 2015
//        Fixed MacOS compilation.
//        Fixed IOS64 support.
//        Requirement for kbmMW v. 4.82.00+
//
// 7.70.00 Jun 5 2015
//        Added UseFieldDisplayName boolean to CSV streamformater to allow selection
//          if to use field name or field display name as header/field identifier.
//        Changes so default FormatSettings are initialized correctly in both CSV and
//          memtable component.
{$ENDREGION}
//=============================================================================

//=============================================================================
// Remove the remark on the next line if all records should be checked before use.
//{$define DO_CHECKRECORD}
//=============================================================================

//=============================================================================
// Uncomment the next line to use less optimized code.
{$define USE_SAFE_CODE}
//=============================================================================

{$IFDEF BCB}
{$ObjExportAll On}
{$ASSERTIONS ON}
{$ENDIF}

uses
  SysUtils,
  Classes,
  DB
{$IFNDEF FPC}
  ,DBCommon
{$ENDIF}
{$IFDEF MACOS}
  ,Posix.Unistd
  ,Posix.Stdio
{$ENDIF}
{$IFDEF LINUX}
  ,Types
  ,Libc
{$ELSE}
 {$IFDEF MSWINDOWS}
  ,Windows
 {$ENDIF}
{$ENDIF}
  ,SyncObjs
{$IFNDEF FPC}
  ,Masks
{$ENDIF}
  ,variants
  ,fmtbcd
 {$IFNDEF FPC}
  ,SqlTimSt
 {$ENDIF}
  ,kbmMemTypes
  ,kbmString
  ,kbmList
  ,kbmMove
{$IFDEF NEXTGEN}
 {$IFDEF ANDROID}
  ,Posix.Unistd
  ,Posix.Stdio
 {$ENDIF}
  ,System.Generics.Collections
{$ELSE}
 {$IFDEF LEVEL16}
  ,AnsiStrings
 {$ENDIF}
{$ENDIF}
  ;

{$B-}    // Enable short circuit evaluation.
{$T-}    // Disable typechecking on @

{$IFDEF KBMMEMTABLE_CODEGEAR_EDITION}
  const KBMMEMTABLE_EDITION = 'CodeGear';
{$ELSE}
  {$include kbmMemTableEdition.inc}
{$ENDIF}

const KBMMEMTABLE_VERSION = '7.70.00';

//=============================================================================

//***********************************************************************

const
     // Key buffer types.
     kbmkbMin=0;
     kbmkbKey=0;
     kbmkbRangeStart=1;
     kbmkbRangeEnd=2;
     kbmkbMasterDetail=3;
     kbmkbMax=3;

     // Field flags.
     kbmffIndirect = $01;
     kbmffCompress = $02;
     kbmffModified = $04;

     // Record identifier.
     kbmRecordIdent=$6A1B2C3E;

     // Consts for GetRows
     kbmBookmarkCurrent = $00000000;
     kbmBookmarkFirst = $00000001;
     kbmBookmarkLast = $00000002;
     kbmGetRowsRest = $FFFFFFFF;

     // Const for field flags.
     kbmffNull    = 0;
     kbmffUnknown = 1;
     kbmffData    = 2;

     // Internal index names.
     kbmRowOrderIndex = '__MT__ROWORDER_';
     kbmDetailIndex   = '__MT__DETAIL_';
     kbmGroupIndex    = '__MT__GROUP_';
     kbmDefSortIndex  = '__MT__DEFSORT_';
     kbmDefaultIndex  = '__MT__DEFAULT_';
     kbmRangeIndex    = '__MT__RANGE_';
     kbmAutoIndex     = '__MT__AUTO_';

     // Record flags.
     kbmrfInTable          = $01;  // 0000 0001    Is record a work record or actually from the table.
     kbmrfDontCheckPoint   = $02;  // 0000 0010    Is record marked for not to checkpoint.

{$IFDEF LINUX}
     INFINITE = LongWord($FFFFFFFF);
{$ENDIF}

type
  TkbmCustomMemTable = class;
  TkbmCustomMemTableClass = class of TkbmCustomMemTable;

{$IFDEF KBMMEMTABLE_SUPPORT_EXPRESSIONPARSER}
  // Filter expressions.
  TkbmExprParser = class(TExprParser)
  private
     FDataset:TkbmCustomMemTable;
  public
     constructor Create(DataSet: TkbmCustomMemTable; const Text: string;
                        Options: TFilterOptions);
     function Evaluate:boolean;
  end;
{$ELSE}
  TkbmExprParser = class
  private
    FDataset:TkbmCustomMemTable;
    FSQL:TObject;
  public
    constructor Create(DataSet: TkbmCustomMemTable; const Text: string);
    destructor Destroy; override;
    function Evaluate:boolean;
  end;
{$ENDIF}

  // Define error classes and error groups.
  EMemTableError = class(EDataBaseError);

  EMemTableFatalError = class(EMemTableError);
  EMemTableInvalidRecord = class(EMemTableFatalError);

  EMemTableIndexError = class(EMemTableError);
  EMemTableDupKey = class(EMemTableError);

  EMemTableFilterError = class(EMemTableError);

  EMemTableLocaleError = class(EMemTableError);
  EMemTableInvalidLocale = class(EMemTableLocaleError);

  TkbmCustomDeltaHandler = class;

{$IFDEF FPC}
  { TSQLTimeStamp }
  PSQLTimeStamp = ^TSQLTimeStamp;
  TSQLTimeStamp = packed record
    Year: SmallInt;
    Month: Word;
    Day: Word;
    Hour: Word;
    Minute: Word;
    Second: Word;
    Fractions: LongWord;
  end;
{$ENDIF}

  TkbmBookmark=record
      Bookmark:PkbmRecord;
      Flag:TBookmarkFlag;
      RecordID:TkbmNativeInt;
  end;

  PkbmBookmark = ^TkbmBookmark;

  TkbmUserBookmark=record
      Bookmark:PkbmRecord;
      DataID:TkbmNativeInt;
      RecordID:TkbmNativeInt;
  end;

  PkbmUserBookmark = ^TkbmUserBookmark;

  // IndexFieldOptions.
  TkbmifoOption = (mtifoDescending,mtifoCaseInsensitive,mtifoPartial,mtifoIgnoreNull,mtifoIgnoreLocale,mtifoAggregate,
                   mtifoAggSum,mtifoAggMin,mtifoAggMax,mtifoAggCount,mtifoAggAvg,mtifoAggStdDev,
                   mtifoAggUsr1,mtifoAggUsr2,mtifoAggUsr3,
                   mtifoIgnoreNonSpace,mtifoIgnoreKanatype,mtifoIgnoreSymbols,mtifoIgnoreWidth,
                   mtifoExtract,mtifoAsDate,mtifoAsTime,mtifoAsDateTime,mtifoNullFirst);
  TkbmifoOptions = set of TkbmifoOption;

  TkbmIndex=class;


  TkbmMemTableStorageType = (mtstDataSet,mtstStream,mtstBinaryStream,mtstFile,mtstBinaryFile);

  TkbmMemTableUpdateFlag = (mtufEdit,mtufAppend,mtufDontClear);
  TkbmMemTableUpdateFlags = set of TkbmMemTableUpdateFlag;

{$EXTERNALSYM TkbmFieldTypes}
 {$IFDEF LEVEL14}
  {$HPPEMIT 'typedef Set<Db::TFieldType, Db::ftUnknown, Db::ftSingle>  TkbmFieldTypes;'}
 {$ELSE}
  {$HPPEMIT 'typedef Set<Db::TFieldType, Db::ftUnknown, Db::ftOraInterval>  TkbmFieldTypes;'}
 {$ENDIF}
  TkbmFieldTypes = set of DB.TFieldType;

  TkbmMemTableCompareOption = (mtcoDescending,mtcoCaseInsensitive,mtcoPartialKey,mtcoIgnoreNullKey,mtcoIgnoreLocale,mtcoUnique,mtcoNonMaintained,mtcoNullFirst);
  TkbmMemTableCompareOptions = set of TkbmMemTableCompareOption;

  TkbmMemTableCopyTableOption = (mtcpoStructure,mtcpoOnlyActiveFields,mtcpoProperties,
                                 mtcpoLookup,mtcpoCalculated,mtcpoAppend,mtcpoFieldIndex,
                                 mtcpoDontDisableIndexes,
                                 mtcpoIgnoreErrors,
                                 mtcpoLookupAsData,mtcpoCalculatedAsData,mtcpoStringAsWideString,mtcpoWideStringUTF8);
  TkbmMemTableCopyTableOptions = set of TkbmMemTableCopyTableOption;

  TkbmOnFilterIndex = procedure(DataSet:TDataSet; Index:TkbmIndex; var Accept:boolean) of object;
  TkbmOnUserAggregate = procedure(DataSet:TDataSet; const UserFunction:integer; const Count:integer; const Value:double; var Accumulator:double) of object;

  PkbmVarLength=PByte;
  PPkbmVarLength=^PkbmVarLength;

  TkbmIndexType = (mtitNonSorted,mtitSorted);
  TkbmSearchType = (mtstFirst,mtstLast,mtstNearestBefore,mtstNearestAfter);

  TkbmFieldList = class
  private
     FCount:integer;
     FPrimaryCount:integer;
  public
     FieldCount:integer;

     FieldNo:array of integer;
     FieldOfs:array of integer;
     Options:array of TkbmifoOptions;
     LocateOptions:TLocateOptions;
     Fields:array of TField;

     constructor Create; virtual;
     destructor Destroy; override;
     function Add(const ADataSet:TkbmCustomMemTable; const AField:TField; const AValue:TkbmifoOptions):Integer;
     procedure Clear; virtual;
     function IndexOf(Item:TField): Integer;
     procedure AssignTo(AFieldList:TkbmFieldList);
     procedure MergeOptionsTo(AFieldList:TkbmFieldList); // Must be identical fieldlists.
     procedure DefineAdditionalOrderFields(ADataSet:TkbmCustomMemTable; AFieldList:TkbmFieldList);
     procedure ClearOptions;
     function GetAsString:string;

     procedure SetOptions(ADataSet:TkbmCustomMemTable; AOptions:TkbmifoOption; AFieldNames:string);
     function FindField(const AFieldName:string):TField;
     function StartsWith(AList:TkbmFieldList; const ASameCase:boolean; const AOnlyPrimary:boolean):boolean;
     function IsEqualTo(AList:TkbmFieldList; const ASameCase:boolean; const AOnlyPrimary:boolean):boolean;
     procedure Build(ADataset:TkbmCustomMemTable; const AFieldNames:string; const AAggregateFieldNaming:boolean = false);

     property Count: Integer read FCount;
     property PrimaryCount: Integer read FPrimaryCount;
  end;

  TkbmIndexes = class;

  TkbmIndex = class
  private
     FBeingFreed:       boolean;
     FDependantIndexes: TkbmIndexes;
     FReferences:       TkbmList;
     FName:             string;
     FDataSet:          TkbmCustomMemTable;
     FIndexFields:      string;
     FIndexFieldList:   TkbmFieldList;
     FIndexOptions:     TkbmMemTableCompareOptions;
     FRangeStartKey:    PkbmRecord;
     FRangeEndKey:      PkbmRecord;
     FSorted:           boolean;
     FDirty:            boolean;
     FType:             TkbmIndexType;
     FRowOrder:         boolean;
     FIsFiltered:       boolean;
     FIsRange:          boolean;
     FEnabled:          boolean;
     FUpdateStatus:     TUpdateStatusSet;
     FFilterParser:     TkbmExprParser;
     FFilterFunc:       TkbmOnFilterIndex;

{$IFDEF AUTOREFCOUNT}[weak]{$ENDIF}
     FBaseIndex:        TkbmIndex;

     procedure InternalSwap(const I,J:integer); {$IFDEF KBMMEMTABLE_SUPPORT_INLINE}inline;{$ENDIF}
     procedure InternalInsertionSort(const Lo,Hi:integer);
     procedure InternalFastQuickSort(const L,R:Integer);
     procedure SetEnabled(AValue:boolean);
  protected
     procedure SetRangeStartKey(AKeyRecord:PkbmRecord);
     procedure SetRangeEndKey(AKeyRecord:PkbmRecord);

     function GetIsOrdered:boolean;
     function GetIsFullScope:boolean;

     function CompareRecords(const AFieldList:TkbmFieldList;
         const AKeyRecord,ARecord:PkbmRecord;
         const APrimaryOnly:boolean;
         const ASortCompare,APartial,AUniqueConstraint:boolean): Integer;
     procedure FastQuickSort(const L,R:Integer);
     function BinarySearchRecordID(FirstNo,LastNo:TkbmNativeInt; const RecordID:TkbmNativeInt;
         const Desc:boolean; var Index:TkbmNativeInt):TkbmNativeInt;
     function SequentialSearchRecordID(const FirstNo,LastNo:TkbmNativeInt; const RecordID:TkbmNativeInt;
         var Index:TkbmNativeInt):TkbmNativeInt;
     function BinarySearch(FieldList:TkbmFieldList; FirstNo,LastNo:TkbmNativeInt;
         const KeyRecord:PkbmRecord; const SearchType:TkbmSearchType;
         const PrimaryOnly:boolean; const RespectFilter:boolean;
         var Index:TkbmNativeInt; var Found:boolean):TkbmNativeInt;
     function SequentialSearch(FieldList:TkbmFieldList; const FirstNo,LastNo:TkbmNativeInt;
         const KeyRecord:PkbmRecord; const SearchType:TkbmSearchType;
         const PrimaryOnly:boolean; const RespectFilter:boolean;
         const SearchOrdered:boolean;
         var Index:TkbmNativeInt; var Found:boolean):TkbmNativeInt;
     function FindRecordNumber(const ARecord:PkbmRecord):integer;
     function Filter(const ARecord:PkbmRecord; const ACheckRange:boolean):boolean;

  public
     constructor Create(ABase:TkbmIndex; Name:string; ADataSet:TkbmCustomMemtable; Fields:string; Options:TkbmMemTableCompareOptions; IndexType:TkbmIndexType); reintroduce; overload; virtual;
     constructor Create(ABase:TkbmIndex; IndexDef:TIndexDef; ADataSet:TkbmCustomMemtable); reintroduce; overload; virtual;

     destructor Destroy; override;

     function Search(FieldList:TkbmFieldList; KeyRecord:PkbmRecord; const SearchType:TkbmSearchType;
         const PrimaryOnly,RespectFilter:boolean;
         var Index:TkbmNativeInt;
         var Found:boolean):TkbmNativeInt;
     function SearchRecord(KeyRecord:PkbmRecord; var Index:TkbmNativeInt;
         const PrimaryOnly,RespectFilter:boolean):TkbmNativeInt;
     function SearchRecordID(RecordID:TkbmNativeInt; var Index:TkbmNativeInt):TkbmNativeInt;
     function IsDependingOn(AIndex:TkbmIndex):boolean;
     procedure Clear;
     procedure Load;
     procedure ReSort;
     procedure Rebuild;
     procedure RebuildDepending;

     property DependantIndexes:TkbmIndexes read FDependantIndexes;
     property Base:TkbmIndex read FBaseIndex;
     property Enabled:boolean read FEnabled write SetEnabled;
     property IsSorted:boolean read FSorted write FSorted;
     property IsDirty:boolean read FDirty write FDirty;
     property IsOrdered:boolean read GetIsOrdered;
     property IsFiltered:boolean read FIsFiltered write FIsFiltered;
     property IsRange:boolean read FIsRange write FIsRange;
     property IsFullScope:boolean read GetIsFullScope;
     property IndexType:TkbmIndexType read FType write FType;
     property IndexOptions:TkbmMemTableCompareOptions read FIndexOptions write FIndexOptions;
     property IndexFields:string read FIndexFields write FIndexFields;
     property IndexFieldList:TkbmFieldList read FIndexFieldList write FIndexFieldList;
     property Dataset:TkbmCustomMemTable read FDataSet write FDataSet;
     property Name:string read FName write FName;
     property References:TkbmList read FReferences write FReferences;
     property IsRowOrder:boolean read FRowOrder write FRowOrder;
     property UpdateStatus:TUpdateStatusSet read FUpdateStatus write FUpdateStatus;
     property RangeStartKey:PkbmRecord read FRangeStartKey write SetRangeStartKey;
     property RangeEndKey:PkbmRecord read FRangeEndKey write SetRangeEndKey;
  end;

  TkbmIndexUpdateHow = (mtiuhInsert,mtiuhEdit,mtiuhDelete,mtiuhAppend);

  TkbmIndexes = class
  private
     FInClear:        boolean;
     FIndexes:        TStringList;
     FRowOrderIndex:  TkbmIndex;   // This is the owner of all other indexes.
{$IFDEF AUTOREFCOUNT}[weak]{$ENDIF}
     FDataSet:        TkbmCustomMemTable;

  protected
     procedure IndexFreed(AIndex:TkbmIndex); virtual;

  public
     constructor Create(ADataSet:TkbmCustomMemTable); reintroduce; virtual;
     destructor Destroy; override;

     procedure Clear;
     procedure Add(const IndexDef:TIndexDef);

     procedure AddIndex(const Index:TkbmIndex);
     procedure DeleteIndex(const Index:TkbmIndex);

     procedure ReBuild(const IndexName:string);
     procedure Delete(const IndexName:string);
     function Get(const IndexName:string):TkbmIndex;
     function GetIndex(const Ordinal:integer):TkbmIndex;
     procedure Empty(const IndexName:string);

     function GetByFieldNames(FieldNames:string):TkbmIndex;

     procedure EmptyAll;
     procedure ReBuildAll;
     procedure MarkAllDirty;

     procedure CheckRecordUniqueness(const ARecord,ActualRecord:PkbmRecord);
     procedure ReflectToIndexes(const How:TkbmIndexUpdateHow; const OldRecord,NewRecord:PkbmRecord; const RecordPos:TkbmNativeInt; const DontVersion:boolean);
     function Search(const FieldList:TkbmFieldList; const KeyRecord:PkbmRecord; const SearchType:TkbmSearchType; const PrimaryOnly,RespectFilter,AutoAddIdx:boolean; var Index:TkbmNativeInt; var Found:boolean):TkbmNativeInt;
     function Count:integer;
  end;

  TkbmVersioningMode = (mtvm1SinceCheckPoint,mtvmAllSinceCheckPoint);

  TkbmProgressCode = (mtpcLoad,mtpcSave,mtpcEmpty,mtpcPack,mtpcCheckPoint,mtpcSearch,mtpcCopy,mtpcUpdate,mtpcSort);
  TkbmProgressCodes = set of TkbmProgressCode;

  TkbmState = (mtstBrowse,mtstLoad,mtstSave,mtstEmpty,mtstPack,mtstCheckPoint,mtstSearch,mtstUpdate,mtstSort);

  TkbmPerformance = (mtpfFast,mtpfBalanced,mtpfSmall);

  TkbmOnProgress = procedure(DataSet:TDataSet; Percentage:integer; Code:TkbmProgressCode) of object;
  TkbmOnLoadRecord = procedure(DataSet:TDataSet; var Accept:boolean) of object;
  TkbmOnLoadField = procedure(DataSet:TDataSet; FieldNo:integer; Field:TField) of object;
  TkbmOnSaveRecord = procedure(DataSet:TDataSet; var Accept:boolean) of object;
  TkbmOnSaveField = procedure(DataSet:TDataSet; FieldNo:integer; Field:TField) of object;
  TkbmOnCompressField = procedure(DataSet:TDataSet; Field:TField; const Buffer:PByte; var Size:TkbmLongInt; var ResultBuffer:PByte) of object;
  TkbmOnDecompressField = procedure(DataSet:TDataSet; Field:TField; const Buffer:PByte; var Size:TkbmLongInt; var ResultBuffer:PByte) of object;
  TkbmOnCompareFields = procedure(DataSet:TDataSet; AFld:TField; KeyField,AField:pointer; FieldType:TFieldType; Options:TkbmifoOptions; var FullCompare:boolean; var Result:integer) of object;

  TkbmOnSave = procedure(DataSet:TDataSet; StorageType:TkbmMemTableStorageType; Stream:TStream) of object;
  TkbmOnLoad = procedure(DataSet:TDataSet; StorageType:TkbmMemTableStorageType; Stream:TStream) of object;
  TkbmOnSetupField = procedure(DataSet:TDataSet; Field:TField; var FieldFlags:byte) of object;
  TkbmOnSetupFieldProperties = procedure(DataSet:TDataSet; Field:TField) of object;


  TkbmLocaleID = integer;

  TkbmStreamFlagData                = (sfSaveData,sfLoadData);
  TkbmStreamFlagCalculated          = (sfSaveCalculated,sfLoadCalculated);
  TkbmStreamFlagLookup              = (sfSaveLookup,sfLoadLookup);
  TkbmStreamFlagNonVisible          = (sfSaveNonVisible,sfLoadNonVisible);
  TkbmStreamFlagBlobs               = (sfSaveBlobs,sfLoadBlobs);
  TkbmStreamFlagDef                 = (sfSaveDef,sfLoadDef,sfAutoLayoutDef,sfLoadByFieldNo);
  TkbmStreamFlagIndexDef            = (sfSaveIndexDef,sfLoadIndexDef);
  TkbmStreamFlagFiltered            = (sfSaveFiltered);
  TkbmStreamFlagIgnoreRange         = (sfSaveIgnoreRange);
  TkbmStreamFlagIgnoreMasterDetail  = (sfSaveIgnoreMasterDetail);
  TkbmStreamFlagDeltas              = (sfSaveDeltas, sfLoadDeltas);
  TkbmStreamFlagDontFilterDeltas    = (sfSaveDontFilterDeltas);
  TkbmStreamFlagAppend              = (sfSaveAppend,sfSaveInsert);
  TkbmStreamFlagFieldKind           = (sfSaveFieldKind,sfLoadFieldKind);
  TkbmStreamFlagFromStart           = (sfLoadFromStart);
  TkbmStreamFlagDisplayWidth        = (sfLoadDetermineWidth);

  TkbmStreamFlagsData               = set of TkbmStreamFlagData;
  TkbmStreamFlagsCalculated         = set of TkbmStreamFlagCalculated;
  TkbmStreamFlagsLookup             = set of TkbmStreamFlagLookup;
  TkbmStreamFlagsNonVisible         = set of TkbmStreamFlagNonVisible;
  TkbmStreamFlagsBlobs              = set of TkbmStreamFlagBlobs;
  TkbmStreamFlagsDef                = set of TkbmStreamFlagDef;
  TkbmStreamFlagsIndexDef           = set of TkbmStreamFlagIndexDef;
  TkbmStreamFlagsFiltered           = set of TkbmStreamFlagFiltered;
  TkbmStreamFlagsIgnoreRange        = set of TkbmStreamFlagIgnoreRange;
  TkbmStreamFlagsIgnoreMasterDetail = set of TkbmStreamFlagIgnoreMasterDetail;
  TkbmStreamFlagsDeltas             = set of TkbmStreamFlagDeltas;
  TkbmStreamFlagsDontFilterDeltas   = set of TkbmStreamFlagDontFilterDeltas;
  TkbmStreamFlagsAppend             = set of TkbmStreamFlagAppend;
  TkbmStreamFlagsFieldKind          = set of TkbmStreamFlagFieldKind;
  TkbmStreamFlagsFromStart          = set of TkbmStreamFlagFromStart;
  TkbmStreamFlagsDisplayWidth       = set of TkbmStreamFlagDisplayWidth;

  TkbmOnCompress = procedure(Dataset:TkbmCustomMemTable; UnCompressedStream,CompressedStream:TStream) of object;
  TkbmOnDeCompress = procedure(Dataset:TkbmCustomMemTable; CompressedStream,DeCompressedStream:TStream) of object;

  TkbmDetermineLoadFieldsSituation = (dlfBeforeLoad,dlfAfterLoadDef);

{$IFDEF LEVEL16}
  [ComponentPlatformsAttribute({$IFDEF LEVEL23}pidiOSDevice64 or {$ENDIF}{$IFDEF LEVEL18}pidiOSSimulator or pidiOSDevice or {$ENDIF}{$IFDEF LEVEL19}pidAndroid or {$ENDIF}pidWin32 or pidWin64{$IFDEF LEVEL17} or pidOSX32{$ENDIF})]
{$ENDIF}
  TkbmCustomStreamFormat = class(TComponent)
  private
     FOrigStream:TStream;
     FWorkStream:TStream;
     FBookmark:TBookmark;

     FOnCompress:TkbmOnCompress;
     FOnDecompress:TkbmOnDeCompress;

     FWasFiltered:boolean;
     FWasRangeActive:boolean;
     FWasMasterLinkUsed:boolean;
     FWasEnableIndexes:boolean;
     FWasPersistent:boolean;

     FsfData:                TkbmStreamFlagsData;
     FsfCalculated:          TkbmStreamFlagsCalculated;
     FsfLookup:              TkbmStreamFlagsLookup;
     FsfNonVisible:          TkbmStreamFlagsNonVisible;
     FsfBlobs:               TkbmStreamFlagsBlobs;
     FsfDef:                 TkbmStreamFlagsDef;
     FsfIndexDef:            TkbmStreamFlagsIndexDef;
     FsfFiltered:            TkbmStreamFlagsFiltered;
     FsfIgnoreRange:         TkbmStreamFlagsIgnoreRange;
     FsfIgnoreMasterDetail:  TkbmStreamFlagsIgnoreMasterDetail;
     FsfDeltas:              TkbmStreamFlagsDeltas;
     FsfDontFilterDeltas:    TkbmStreamFlagsDontFilterDeltas;
     FsfAppend:              TkbmStreamFlagsAppend;
     FsfFieldKind:           TkbmStreamFlagsFieldKind;
     FsfFromStart:           TkbmStreamFlagsFromStart;
     FsfDisplayWidth:        TkbmStreamFlagsDisplayWidth;

     FOnBeforeSave:          TNotifyEvent;
     FOnAfterSave:           TNotifyEvent;
     FOnBeforeLoad:          TNotifyEvent;
     FOnAfterLoad:           TNotifyEvent;

     FFormatSettings:        TFormatSettings;

     procedure SetVersion(AVersion:string);
  protected
     SaveFields,
     LoadFields:array of integer;
     LoadFieldWidths:array of integer;

     procedure SetIgnoreAutoIncPopulation(ADataset:TkbmCustomMemTable; Value:boolean);

     function  GetVersion:string; virtual;

     procedure DetermineSaveFields(ADataset:TkbmCustomMemTable); virtual;
     procedure BeforeSave(ADataset:TkbmCustomMemTable); virtual;
     procedure SaveDef(ADataset:TkbmCustomMemTable); virtual;
     procedure SaveData(ADataset:TkbmCustomMemTable); virtual;
     procedure Save(ADataset:TkbmCustomMemTable); virtual;
     procedure AfterSave(ADataset:TkbmCustomMemTable); virtual;

     procedure DetermineLoadFieldIDs(ADataset:TkbmCustomMemTable; AList:TStringList; Situation:TkbmDetermineLoadFieldsSituation); virtual;
     procedure DetermineLoadFields(ADataset:TkbmCustomMemTable; Situation:TkbmDetermineLoadFieldsSituation); virtual;
     procedure DetermineLoadFieldIndex(ADataset:TkbmCustomMemTable; ID:string; FieldCount:integer; OrigIndex:integer; var NewIndex:integer; Situation:TkbmDetermineLoadFieldsSituation); virtual;
     procedure BeforeLoad(ADataset:TkbmCustomMemTable); virtual;
     procedure LoadDef(ADataset:TkbmCustomMemTable); virtual;
     procedure LoadData(ADataset:TkbmCustomMemTable); virtual;
     procedure Load(ADataset:TkbmCustomMemTable); virtual;
     procedure AfterLoad(ADataset:TkbmCustomMemTable); virtual;
     procedure Reposition(ADataset:TkbmCustomMemTable); virtual;
     procedure Refresh(ADataset:TkbmCustomMemTable); virtual;

     property WorkStream:TStream read FWorkStream write FWorkStream;
     property OrigStream:TStream read FOrigStream write FOrigStream;

     property sfData:TkbmStreamFlagsData read FsfData write FsfData;
     property sfCalculated:TkbmStreamFlagsCalculated read FsfCalculated write FsfCalculated;
     property sfLookup:TkbmStreamFlagsLookup read FsfLookup write FsfLookup;
     property sfNonVisible:TkbmStreamFlagsNonVisible read FsfNonVisible write FsfNonVisible;
     property sfBlobs:TkbmStreamFlagsBlobs read FsfBlobs write FsfBlobs;
     property sfDef:TkbmStreamFlagsDef read FsfDef write FsfDef;
     property sfIndexDef:TkbmStreamFlagsIndexDef read FsfIndexDef write FsfIndexDef;
     property sfFiltered:TkbmStreamFlagsFiltered read FsfFiltered write FsfFiltered;
     property sfIgnoreRange:TkbmStreamFlagsIgnoreRange read FsfIgnoreRange write FsfIgnoreRange;
     property sfIgnoreMasterDetail:TkbmStreamFlagsIgnoreMasterDetail read FsfIgnoreMasterDetail write FsfIgnoreMasterDetail;
     property sfDeltas:TkbmStreamFlagsDeltas read FsfDeltas write FsfDeltas;
     property sfDontFilterDeltas:TkbmStreamFlagsDontFilterDeltas read FsfDontFilterDeltas write FsfDontFilterDeltas;
     property sfAppend:TkbmStreamFlagsAppend read FsfAppend write FsfAppend;
     property sfFieldKind:TkbmStreamFlagsFieldKind read FsfFieldKind write FsfFieldKind;
     property sfFromStart:TkbmStreamFlagsFromStart read FsfFromStart write FsfFromStart;
     property sfDisplayWidth:TkbmStreamFlagsDisplayWidth read FsfDisplayWidth write FsfDisplayWidth;
     property Version:string read GetVersion write SetVersion;

     property OnBeforeSave:TNotifyEvent read FOnBeforeSave write FOnBeforeSave;
     property OnAfterSave:TNotifyEvent read FOnAfterSave write FOnAfterSave;
     property OnBeforeLoad:TNotifyEvent read FOnBeforeLoad write FOnBeforeLoad;
     property OnAfterLoad:TNotifyEvent read FOnAfterLoad write FOnAfterLoad;
     property OnCompress:TkbmOnCompress read FOnCompress write FOnCompress;
     property OnDeCompress:TkbmOnDecompress read FOnDecompress write FOnDecompress;

  public
     constructor Create(AOwner:TComponent); override;
     procedure Assign(Source:TPersistent); override;

     property FormatSettings:TFormatSettings read FFormatSettings write FFormatSettings;
  end;

{$IFDEF LEVEL16}
  [ComponentPlatformsAttribute({$IFDEF LEVEL23}pidiOSDevice64 or {$ENDIF}{$IFDEF LEVEL18}pidiOSSimulator or pidiOSDevice or {$ENDIF}{$IFDEF LEVEL19}pidAndroid or {$ENDIF}pidWin32 or pidWin64{$IFDEF LEVEL17} or pidOSX32{$ENDIF})]
{$ENDIF}
  TkbmStreamFormat = class(TkbmCustomStreamFormat)
  published
     property sfData;
     property sfCalculated;
     property sfLookup;
     property sfNonVisible;
     property sfBlobs;
     property sfDef;
     property sfIndexDef;
     property sfFiltered;
     property sfIgnoreRange;
     property sfIgnoreMasterDetail;
     property sfDeltas;
     property sfDontFilterDeltas;
     property sfAppend;
     property sfFieldKind;
     property sfFromStart;
     property Version;

     property OnBeforeLoad;
     property OnAfterLoad;
     property OnBeforeSave;
     property OnAfterSave;
     property OnCompress;
     property OnDeCompress;

 {$IFNDEF KBMMEMTABLE_DONT_SUPPORT_PUBLISHED_RECORDS}
     property FormatSettings;
 {$ENDIF}
  end;

  TkbmCompareHow = (chBreakNE,chBreakLT,chBreakGT,chBreakLTE,chBreakGTE);

  TkbmCommon = class;

{$IFDEF KBMMEMTABLE_DEBUG_LOCKS}
  TkbmCS = class(TCriticalSection)
  protected
     FCnt:integer;
     FCommon:TkbmCommon;
  public
     constructor Create(ACommon:TkbmCommon); reintroduce; virtual;
     destructor Destroy; override;
     procedure Acquire; override;
     procedure Release; override;
     procedure DumpAttachedTables(AText:string);
     procedure DumpLockCount(AText:string);
  end;
{$ELSE}
  TkbmCS = TCriticalSection;
{$ENDIF}

  TkbmCommon = class
  protected
      FStandalone:                            boolean;
      FRecords:                               TkbmList;

      FFieldCount:                            integer;
      FFieldOfs:                              array of TkbmLongInt;
      FFieldFlags:                            array of byte;

      FLanguageID,
      FSubLanguageID,
      FSortID:                                integer;

      // Data identifier.
      FDataID:                                TkbmNativeInt;

      // Setup from FLanguageID, FSubLanguageID and FSortID.
      FLocaleID:                              TkbmLocaleID;

      FBookmarkArraySize,
      FFixedRecordSize,
      FTotalRecordSize,
      FDataRecordSize,
      FCalcRecordSize,
      FVarLengthRecordSize,
      FStartCalculated,
      FStartBookmarks,
      FStartVarLength:                        TkbmLongInt;
      FVarLengthCount:                        integer;

      FIsDataModified:                        boolean;

      FAutoIncMin,
      FAutoIncMax:                            TkbmLongInt;

      // Holds the number of records marked as deleted but not yet removed (used during versioning of records).
      // Used for keeping track if any filtering should occur.
      FDeletedCount:                          TkbmNativeInt;

      FPerformance:                           TkbmPerformance;

      FAttachMaxCount:                        integer;

      // Holds a list of all actually deleted records for later reuse.
      FDeletedRecords:                        TkbmList;

      FVersioningMode:                        TkbmVersioningMode;
      FEnableVersioning:                      boolean;

      FTransactionLevel:                      integer;

      FThreadProtected:                       boolean;

{$IFDEF NEXTGEN}
      FAttachedTables:                        TList<TkbmCustomMemTable>;
{$ELSE}
      FAttachedTables:                        TList;
{$ENDIF}
{$IFDEF AUTOREFCOUNT}[weak]{$ENDIF}
      FOwner:                                 TkbmCustomMemTable;

{$IFDEF DO_CHECKRECORD}
      procedure _InternalCheckRecord(ARecord:PkbmRecord);
{$ENDIF}
      function _InternalCopyRecord(SourceRecord:PkbmRecord; CopyVarLengths:boolean):PkbmRecord;
      procedure _InternalCopyVarLength(SourceRecord,DestRecord:PkbmRecord; Field:TField);
      procedure _InternalCopyVarLengths(SourceRec,DestRec:PkbmRecord);
      procedure _InternalMoveRecord(SourceRecord,DestRecord:PkbmRecord);
      procedure _InternalTransferRecord(SourceRecord,DestRecord:PkbmRecord);
      procedure _InternalFreeRecordVarLengths(ARecord:PkbmRecord);
      procedure _InternalClearRecord(ARecord:PkbmRecord);  {$IFDEF KBMMEMTABLE_SUPPORT_INLINE}inline;{$ENDIF}
      procedure _InternalAppendRecord(ARecord:PkbmRecord); {$IFDEF KBMMEMTABLE_SUPPORT_INLINE}inline;{$ENDIF}
      procedure _InternalDeleteRecord(ARecord:PkbmRecord); {$IFDEF KBMMEMTABLE_SUPPORT_INLINE}inline;{$ENDIF}
      procedure _InternalPackRecords;
      procedure _InternalEmpty;
      function _InternalCompareRecords(const AFieldList:TkbmFieldList; const AMaxFields:integer; const AKeyRecord,ARecord:PkbmRecord; const AIgnoreNull,APartial,AUniqueConstraint:boolean; const AHow:TkbmCompareHow): Integer;

      procedure SetStandalone(Value:boolean);
      function GetStandalone:boolean;
      procedure SetAutoIncMin(Value:TkbmNativeInt);
      function GetAutoIncMin:TkbmNativeInt;
      procedure SetAutoIncMax(Value:TkbmNativeInt);
      function GetAutoIncMax:TkbmNativeInt;
      procedure SetPerformance(Value:TkbmPerformance);
      function GetPerformance:TkbmPerformance;
      procedure SetVersioningMode(Value:TkbmVersioningMode);
      function GetVersioningMode:TkbmVersioningMode;
      procedure SetEnableVersioning(Value:boolean);
      function GetEnableVersioning:boolean;
      procedure SetCapacity(Value:TkbmNativeInt);
      function GetCapacity:TkbmNativeInt;
      function GetTransactionLevel:integer;
      function GetIsDataModified:boolean;
      procedure SetIsDataModified(Value:boolean);
      procedure ClearModifiedFlags;
      function GetModifiedFlag(i:integer):boolean;
      procedure SetModifiedFlag(i:integer; Value:boolean);
      function GetAttachMaxCount:integer;
      procedure SetAttachMaxCount(Value:integer);
      function GetAttachCount:integer;

      procedure EnsureFields(const ACount:integer);

      procedure SetRecordID(ARecordID:TkbmNativeInt);
      procedure SetUniqueRecordID(ARecordID:TkbmNativeInt);
      procedure SetDeletedCount(ACount:TkbmNativeInt);

      function GetLanguageID:integer;
      procedure SetLanguageID(Value:integer);
      function GetSortID:integer;
      procedure SetSortID(Value:integer);
      function GetSubLanguageID:integer;
      procedure SetSubLanguageID(Value:integer);
      function GetLocaleID:TkbmLocaleID;
      procedure SetLocaleID(Value:TkbmLocaleID);

      procedure CalcLocaleID;
      function  GetUniqueDataID:TkbmNativeInt;
  public
      FLock:                                  TkbmCS;
      FUniqueRecordID:                        TkbmNativeInt;
      FRecordID:                              TkbmNativeInt;
      
      function GetDeletedRecordsCount:TkbmNativeInt;

      function GetFieldSize(FieldType:TFieldType; Size:TkbmLongInt):TkbmLongInt;  {$IFDEF KBMMEMTABLE_SUPPORT_INLINE}inline;{$ENDIF}
      function GetFieldDataOffset(Field:TField):TkbmLongInt;  {$IFDEF KBMMEMTABLE_SUPPORT_INLINE}inline;{$ENDIF}
      function GetFieldPointer(ARecord:PkbmRecord; Field:TField):PByte; {$IFDEF KBMMEMTABLE_SUPPORT_INLINE}inline;{$ENDIF}

      function _InternalAllocRecord:PkbmRecord;
      procedure _InternalFreeRecord(ARecord:PkbmRecord; FreeVarLengths, FreeVersions:boolean);

      constructor Create(AOwner:TkbmCustomMemTable);
      destructor Destroy; override;
      procedure Lock;   {$IFDEF KBMMEMTABLE_SUPPORT_INLINE}inline;{$ENDIF}
      procedure Unlock; {$IFDEF KBMMEMTABLE_SUPPORT_INLINE}inline;{$ENDIF}

      function GetFieldIsVarLength(FieldType:TFieldType; Size:TkbmLongInt):boolean; {$IFDEF KBMMEMTABLE_SUPPORT_INLINE}inline;{$ENDIF}
      function CompressFieldBuffer(Field:TField; const Buffer:pointer; var Size:TkbmLongInt):pointer;
      function DecompressFieldBuffer(Field:TField; const Buffer:pointer; var Size:TkbmLongInt):pointer;

      procedure DeAttachAllTables(AExceptTable:TkbmCustomMemTable);
      procedure AttachTable(ATable:TkbmCustomMemTable);
      procedure DeAttachTable(ATable:TkbmCustomMemTable);
      procedure LayoutRecord(const AFieldCount:integer);

      procedure AppendRecord(ARecord:PkbmRecord);
      procedure DeleteRecord(ARecord:PkbmRecord);
      procedure PackRecords;
      function RecordCount:TkbmNativeInt;
      function DeletedRecordCount:TkbmNativeInt;
      procedure Rollback;
      procedure Commit;
      procedure Undo(ARecord:PkbmRecord);

      function IsAnyTableActive:boolean;
      procedure CloseTables(Caller:TkbmCustomMemTable);
      procedure RefreshTables(Caller:TkbmCustomMemTable);
      procedure ResyncTables;
      procedure EmptyTables;
      procedure RebuildIndexes;
      procedure MarkIndexesDirty;
      procedure UpdateIndexes;
      procedure ClearIndexes;
      procedure ReflectToIndexes(const Caller:TkbmCustomMemTable; const How:TkbmIndexUpdateHow; const OldRecord,NewRecord:PkbmRecord; const RecordPos:integer; const DontVersion:boolean);

      procedure IncTransactionLevel;
      procedure DecTransactionLevel;
      property DataRecordSize:TkbmLongInt read FDataRecordSize;
      property Records :TkbmList read FRecords;

      property RecordID : TkbmNativeInt read FRecordID write SetRecordID;
      property UniqueRecordID: TkbmNativeInt read FUniqueRecordID write SetUniqueRecordID;
      property DeletedCount: TkbmNativeInt read FDeletedCount write SetDeletedCount;

      property AttachMaxCount:integer read GetAttachMaxCount write SetAttachMaxCount;
      property AttachCount:integer read GetAttachCount;
      property Standalone:boolean read GetStandalone write SetStandalone;
      property AutoIncMin:TkbmNativeInt read GetAutoIncMin write SetAutoIncMin;
      property AutoIncMax:TkbmNativeInt read GetAutoIncMax write SetAutoIncMax;
      property Performance:TkbmPerformance read GetPerformance write SetPerformance;
      property VersioningMode:TkbmVersioningMode read GetVersioningMode write SetVersioningMode;
      property EnableVersioning:boolean read GetEnableVersioning write SetEnableVersioning;
      property Capacity:TkbmNativeInt read GetCapacity write SetCapacity;
      property IsDataModified:boolean read GetIsDataModified write SetIsDataModified;
      property TransactionLevel:integer read GetTransactionLevel;
      property FieldModified[i:integer]:boolean read GetModifiedFlag write SetModifiedFlag;
      property LanguageID:integer read GetLanguageID write SetLanguageID;
      property SortID:integer read GetSortID write SetSortID;
      property SubLanguageID:integer read GetSubLanguageID write SetSubLanguageID;
      property LocaleID:TkbmLocaleID read GetLocaleID write SetLocaleID;
  end;

  TkbmMasterDataLink = class(TMasterDataLink)
  protected
     procedure RecordChanged(Field: TField); override;
  end;
  TkbmMasterDataLinkClass = class of TkbmMasterDataLink;

{$IFDEF LEVEL16}
  [ComponentPlatformsAttribute({$IFDEF LEVEL23}pidiOSDevice64 or {$ENDIF}{$IFDEF LEVEL18}pidiOSSimulator or pidiOSDevice or {$ENDIF}{$IFDEF LEVEL19}pidAndroid or {$ENDIF}pidWin32 or pidWin64{$IFDEF LEVEL17} or pidOSX32{$ENDIF})]
{$ENDIF}
  TkbmCustomMemTable = class(TDataSet)
  protected
        FTableID:                               integer;
        FIndexes:                               TkbmIndexes;

        FDefaultFormat:                         TkbmCustomStreamFormat;
        FCommaTextFormat:                       TkbmCustomStreamFormat;
        FPersistentFormat:                      TkbmCustomStreamFormat;
        FFormFormat:                            TkbmCustomStreamFormat;
        FAllDataFormat:                         TkbmCustomStreamFormat;

        FFilterRecord:                          PkbmRecord;
        FKeyRecord:                             PkbmRecord;
        FKeyBuffers:                            array [kbmkbMin..kbmkbMax] of PkbmRecord;
        FIgnoreReadOnly:                        boolean;
        FIgnoreAutoIncPopulation:               boolean;

        FIndexDefs:                             TIndexDefs;

        FCurIndex:                              TkbmIndex;
        FSortIndex:                             TkbmIndex;
        FDetailIndex:                           TkbmIndex;
        FRangeIndex:                            TkbmIndex;
        FGroupIndex:                            TkbmIndex;
        FGroupIndexOwned:                       boolean;
        FEnableIndexes:                         boolean;
        FAutoAddIndexes:                        boolean;

        FDesignActivation:                      boolean;
        FInterceptActive:                       boolean;

        FAutoUpdateFieldVariables:              boolean;

        FState:                                 TkbmState;

        FFilterParser:                          TkbmExprParser;
        FFilterOptions:                         TFilterOptions;

        FMasterDetailRelationChanged:           boolean;
        FMasterLink:                            TkbmMasterDataLink;
        FMasterLinkUsed:                        boolean;
        FIsOpen:                                Boolean;
        FRecNo:                                 TkbmNativeInt;
        FReposRecNo:                            TkbmNativeInt;
        FInsertRecNo:                           TkbmNativeInt;

        FBeforeCloseCalled:                     boolean;
        FDuringAfterOpen:                       boolean;

        FLoadLimit:                             TkbmNativeInt;
        FLoadCount:                             TkbmNativeInt;
        FLoadedCompletely:                      boolean;

        FSaveLimit:                             TkbmNativeInt;
        FSaveCount:                             TkbmNativeInt;
        FSavedCompletely:                       boolean;

        FDeltaHandler:                          TkbmCustomDeltaHandler;

        FOverrideActiveRecordBuffer:            PkbmRecord;
        FStatusFilter:                          TUpdateStatusSet;

        FAttachedTo:                            TkbmCustomMemTable;
        FAttachedAutoRefresh:                   boolean;

        FAutoIncField:                          TField;

        FRecalcOnFetch:                         boolean;

        FReadOnly:                              boolean;

        FPersistent:                            boolean;
        FPersistentFile:                        TFileName;
        FPersistentSaved:                       boolean;
        FPersistentBackup:                      boolean;
        FPersistentBackupExt:                   string;

        FStoreDataOnForm:                       boolean;
        FTempDataStorage:                       TMemoryStream;

        FDummyStr:                              string;

        FDetailIndexList:                       TkbmFieldList;
        FIndexList:                             TkbmFieldList;
        FRecalcOnIndex:                         boolean;
        FIndexFieldNames:                       string;
        FDetailFieldNames:                      string;
        FIndexName:                             string;
        FSortFieldNames:                        string;
        FAutoReposition:                        boolean;
        FRangeIgnoreNullKeyValues:              boolean;

        FSortedOn:                              string;

        FSortOptions:                           TkbmMemTableCompareOptions;

        FOnCompareFields:                       TkbmOnCompareFields;

        FOnSave:                                TkbmOnSave;
        FOnLoad:                                TkbmOnLoad;

        FProgressFlags:                         TkbmProgressCodes;
        FOnProgress:                            TkbmOnProgress;

        FOnLoadRecord:                          TkbmOnLoadRecord;
        FOnSaveRecord:                          TkbmOnSaveRecord;
        FOnLoadField:                           TkbmOnLoadField;
        FOnSaveField:                           TkbmOnSaveField;

        FOnCompressBlobStream:                  TkbmOnCompress;
        FOnDecompressBlobStream:                TkbmOnDecompress;

        FOnSetupField:                          TkbmOnSetupField;
        FOnSetupFieldProperties:                TkbmOnSetupFieldProperties;
        FOnCompressField:                       TkbmOnCompressField;
        FOnDecompressField:                     TkbmOnDecompressField;

        FBeforeInsert:                          TDatasetNotifyEvent;

        FOnFilterIndex:                         TkbmOnFilterIndex;
        FOnUserAggregate:                       TkbmOnUserAggregate;

        // Performance optimized.
        FIsFiltered:                            boolean;

        FFormatSettings:                        TFormatSettings;

        FCommon:                                TkbmCommon;

        class function GetMasterDataLinkClass:TkbmMasterDataLinkClass; virtual;

        procedure _InternalBeforeInsert(DataSet:TDataSet);

        function GetActiveRecord:PkbmRecord;

        procedure _InternalFirst; virtual;
        procedure _InternalLast; virtual;
        function  _InternalNext(ForceUseFilter:boolean):boolean; virtual;
        function  _InternalPrior(ForceUseFilter:boolean):boolean; virtual;

        procedure IndexFreed(AIndex:TkbmIndex); virtual;

        procedure FreeGroupIndex;
        procedure CreateGroupIndex(AGroupFields:string);
        procedure FreeDetailIndex;
        procedure CreateDetailIndex;
        procedure SetMasterFields(const Value: string);
        procedure SetDetailFields(const Value: string);
        function GetMasterFields: string;
        procedure SetDataSource(Value: TDataSource);
        procedure FreeRangeIndex;
        procedure CreateRangeIndex(AFieldNames:string = '');
        function GetRangeActive:boolean;

        procedure SetIsFiltered; virtual;
        property IsFiltered:boolean read FIsFiltered;
        procedure BuildFilter(var AFilterParser:TkbmExprParser; AFilter:string; AFilterOptions:TFilterOptions);
        procedure FreeFilter(var AFilterParser:TkbmExprParser);
        procedure DrawAutoInc(ARecord:PkbmRecord = nil);
        procedure PostAutoInc(ARecord:PkbmRecord = nil);

        function GetVersion:string;
        procedure SetIndexFieldNames(FieldNames:string);
        procedure SetIndexName(IndexName:string);
        procedure SetIndexDefs(Value:TIndexDefs);
        procedure SetCommaText(AString: String);
        function GetCommaText: String;
        function GetIndexByName(IndexName:string):TkbmIndex;
        function GetIndexField(Index:integer):TField;
        procedure SetIndexField(Index:integer; Value:TField);
        procedure SetAttachedTo(Value:TkbmCustomMemTable);
        procedure SetRecordTag(Value:TkbmNativeInt);
        function GetRecordTag:TkbmNativeInt;
        function GetIsVersioning:boolean;
        procedure SetStatusFilter(const Value:TUpdateStatusSet);
        procedure SetDeltaHandler(AHandler:TkbmCustomDeltaHandler);
        procedure SetAllData(AVariant:variant);
        function GetAllData:variant;
        function GetAutoIncValue:TkbmNativeInt;
        function GetAutoIncMin:TkbmNativeInt;
        procedure SetAutoIncMinValue(AValue:TkbmNativeInt);
        procedure SetAutoUpdateFieldVariables(AValue:boolean);
        function GetPerformance:TkbmPerformance;
        procedure SetPerformance(AValue:TkbmPerformance);
        function GetVersioningMode:TkbmVersioningMode;
        procedure SetVersioningMode(AValue:TkbmVersioningMode);
        function GetEnableVersioning:boolean;
        procedure SetEnableVersioning(AValue:boolean);
        function GetStandalone:boolean;
        procedure SetStandalone(AValue:boolean);
        function GetCapacity:TkbmNativeInt;
        procedure SetCapacity(AValue:TkbmNativeInt);
        function GetIsDataModified:boolean;
        procedure SetIsDataModified(AValue:boolean);
        function GetAttachMaxCount:integer;
        procedure SetAttachMaxCount(AValue:integer);
        function GetAttachCount:integer;

        function GetModifiedFlags(i:integer):boolean;
        function GetIndexes:TkbmIndexes;
        function GetTransactionLevel:integer;
        function GetDeletedRecordsCount:TkbmNativeInt;

        function GetLanguageID:integer;
        procedure SetLanguageID(Value:integer);
        function GetSortID:integer;
        procedure SetSortID(Value:integer);
        function GetSubLanguageID:integer;
        procedure SetSubLanguageID(Value:integer);
        function GetLocaleID:TkbmLocaleID;
        procedure SetLocaleID(Value:TkbmLocaleID);
        procedure SetActive(Value:boolean); override;

        procedure DoCheckInActive; virtual;
        procedure RebuildFieldLists; virtual;

        // Protected stuff which needs to be supported in the TDataset ancestor to make things work.
        procedure InternalOpen; override;
        procedure InternalClose; override;
        procedure InternalFirst;override;
        procedure InternalLast;override;
        procedure InternalAddRecord(Buffer:PkbmInternalAddRecord; Append: Boolean); override;
        procedure InternalDelete; override;
        procedure InternalInitRecord(Buffer:PkbmInternalInitRecord); override;
        procedure InternalPost; override;
        procedure InternalCancel; override;
        procedure InternalEdit; override;
        procedure InternalInsert; override;
        procedure InternalInitFieldDefs; override;
        procedure InternalInitFieldDefsOnOpen; virtual;
        procedure InternalCreateFieldsOnOpen; virtual;
        procedure InternalSetToRecord(Buffer:PkbmInternalSetToRecord); override;
        procedure CheckActive; override;
        procedure CheckInactive; override;
        procedure DoBeforeClose; override;
        procedure DoBeforeOpen; override;
        procedure DoAfterOpen; override;
        procedure DoAfterPost; override;
        procedure DoAfterDelete; override;
        procedure DoOnNewRecord; override;
        procedure DoBeforePost; override;
        procedure DoOnFilterRecord(ADataset:TDataset; var AFiltered:boolean); virtual;

        function IsCursorOpen: Boolean; override;
        function GetCanModify: Boolean; override;
        function GetRecordSize: Word;override;
        function GetRecordCount: integer;override;
{$IFDEF NEXTGEN}
        function AllocRecBuf: TRecBuf; override;
        procedure FreeRecBuf(var Buffer: TRecBuf); override;
{$ELSE}
        function AllocRecordBuffer: PkbmAllocRecordBuffer; override;
        procedure FreeRecordBuffer(var Buffer: PkbmFreeRecordBuffer); override;
 {$ENDIF}
        procedure CloseBlob(Field: TField); override;

{$IFNDEF KBMMEMTABLE_PUBLIC_FIELDDATA_SETTER}
        procedure SetFieldData(Field: TField; Buffer:PSetFieldDataBuffer); override;
{$ENDIF}
        function GetRecord(Buffer: PkbmGetRecord; GetMode: TGetMode; DoCheck: Boolean): TGetResult; override;
        function FindRecord(Restart, GoForward: Boolean): Boolean; override;
        function GetRecNo: integer;override;
        procedure SetRecNo(Value: integer);override;
        function GetIsIndexField(Field: TField): Boolean; override;
        function GetBookmarkFlag(Buffer: PkbmGetBookmarkFlag): TBookmarkFlag; override;
        procedure SetBookmarkFlag(Buffer: PkbmSetBookmarkFlag; Value: TBookmarkFlag); override;
{$IFDEF KBMMEMTABLE_USE_TBOOKMARK}
        procedure GetBookmarkData(Buffer: PkbmGetBookmarkData; Data:TBookmark); override;
        procedure SetBookmarkData(Buffer: PkbmSetBookmarkData; Data:TBookmark); override;
{$ELSE}
        procedure GetBookmarkData(Buffer: PkbmGetBookmarkData; Data:Pointer); override;
        procedure SetBookmarkData(Buffer: PkbmSetBookmarkData; Data:Pointer); override;
{$ENDIF}
{$IFDEF NEXTGEN}
        procedure InternalGotoBookmark(Bookmark:TBookmark); override;
{$ELSE}
        procedure InternalGotoBookmark(Bookmark:Pointer); override;
{$ENDIF}
        procedure InternalHandleException; override;
        function GetDataSource: TDataSource; override;
        procedure Notification(AComponent: TComponent; Operation: TOperation); override;
        procedure SetFiltered(Value:boolean); override;
        procedure SetFilterText(const Value:string); override;
        procedure SetLoadedCompletely(Value:boolean);
        procedure SetTableState(AValue:TkbmState);
        procedure CreateFieldDefs;
        procedure SetOnFilterRecord(const Value: TFilterRecordEvent); override;
        procedure ClearCalcFields(Buffer:PkbmClearCalcFields); override;

        procedure DataEvent(Event: TDataEvent; Info:TkbmDataEventInfo); override;
        procedure Loaded; override;

        // Internal lowlevel routines.
        procedure DefineProperties(Filer: TFiler); override;
        procedure ReadData(Stream:TStream);
        procedure WriteData(Stream:TStream);
        procedure InternalEmptyTable;

        procedure PopulateField(ARecord:PkbmRecord;Field:TField;AValue:Variant);
        procedure PopulateRecord(ARecord:PkbmRecord;Fields:string;Values:variant);
        procedure PopulateVarLength(ARecord:PkbmRecord;Field:TField;const Buffer; Size:Integer);
        function InternalBookmarkValid(Bookmark: PkbmInternalBookmarkValid):boolean;  {$IFDEF KBMMEMTABLE_SUPPORT_INLINE}{inline;}{$ENDIF}
        procedure PrepareKeyRecord(KeyRecordType:integer; Clear:boolean);
        function FilterExpression(ARecord:PkbmRecord; AFilterParser:TkbmExprParser):boolean;
        procedure MasterChanged(Sender: TObject); virtual;
        procedure MasterDisabled(Sender: TObject); virtual;

        // Internal medium level routines.
        procedure InternalSaveToStreamViaFormat(AStream:TStream; AFormat:TkbmCustomStreamFormat); virtual;
        procedure InternalLoadFromStreamViaFormat(AStream:TStream; AFormat:TkbmCustomStreamFormat); virtual;

        function UpdateRecords(Source,Destination:TDataSet; KeyFields:string; Count:TkbmNativeInt; Flags:TkbmMemTableUpdateFlags; Mapping:string):TkbmNativeInt;
        function LocateRecord(const KeyFields:string; const KeyValues:Variant; Options:TLocateOptions):integer;

        function CheckAutoInc:boolean;

{$IFDEF KBMMEMTABLE_SUPPORT_SETBLOCKREADSIZE}
        procedure SetBlockReadSize(Value: Integer); override;
{$ENDIF}

        property FormatSettings:TFormatSettings read FFormatSettings write FFormatSettings;
  public
        // Helper functions.
        function  __CalcFieldsSize:integer;                    {$IFDEF KBMMEMTABLE_SUPPORT_INLINE}inline;{$ENDIF}
        procedure __ClearBuffers;                              {$IFDEF KBMMEMTABLE_SUPPORT_INLINE}inline;{$ENDIF}
        procedure __ClearCalcFields(Buffer:PkbmRecord);        {$IFDEF KBMMEMTABLE_SUPPORT_INLINE}inline;{$ENDIF}
        procedure __GetCalcFields(Buffer:PkbmRecord);          {$IFDEF KBMMEMTABLE_SUPPORT_INLINE}inline;{$ENDIF}
{$IFDEF KBMMEMTABLE_SUPPORT_SETBLOCKREADSIZE}
        procedure __SetBlockReadSize(Value: Integer);
{$ENDIF}
        function  __SetTempState(const Value: TDataSetState):TDataSetState;
        procedure __RestoreState(const Value: TDataSetState);


        // Public stuff which needs to be supported in the TDataset ancestor to make things work.
        constructor Create(AOwner: TComponent); override;
        destructor Destroy; override;
        function BookmarkValid(Bookmark: TBookmark): boolean; override;
        function CompareBookmarks(Bookmark1, Bookmark2: TBookmark):Integer; override;
{$IFDEF KBMMEMTABLE_PUBLIC_FIELDDATA_SETTER}
        procedure SetFieldData(Field: TField; Buffer:PSetFieldDataBuffer); override;
{$ENDIF}
        function GetFieldData(Field: TField; {$IFDEF KBMMEMTABLE_USE_VAR_GETFIELDDATA}var {$ENDIF}Buffer: PGetFieldDataBuffer): Boolean; override;
        function UpdateStatus: TUpdateStatus; override;

        function CreateBlobStream(Field: TField; Mode: TBlobStreamMode): TStream; override;
        function IsSequenced:Boolean; override;

        procedure SavePersistent;
        procedure LoadPersistent;

        // Public low level routines.
        class function GetAggregateFieldName(const AFieldName:string; const AOptions:TkbmifoOptions):string; virtual;
        class function GetAggregateFieldOption(const AAggregateFunction:string):TkbmifoOptions; virtual;
        class function GetExtractFieldOption(const AExtractOption:string):TkbmifoOptions; virtual;

        procedure ClearModified;
        procedure DestroyIndexes;
        procedure CreateIndexes;
        function FilterRecord(ARecord:PkbmRecord; ForceUseFilter:boolean):Boolean;
        function IsMasterDetailActive:Boolean;
        procedure SwitchToIndex(Index:TkbmIndex);

        // Public medium level routines.
        function CreateFieldAs(Field:TField; const AForceAsData:boolean = false):TField;
        function MoveRecord(Source, Destination: TkbmNativeInt): Boolean;
        function CopyRecord(Source, Destination: TkbmNativeInt): Boolean;
        function MoveCurRecord(Destination:TkbmNativeInt):Boolean;
        function GetVersionFieldData(Field:TField; Version:integer):variant;
        function GetVersionStatus(Version:integer):TUpdateStatus;
        function GetVersionCount:integer;
        function SetVersionFieldData(Field:TField; AVersion:integer; AValue:variant):variant;
        function SetVersionStatus(AVersion:integer; AUpdateStatus:TUpdateStatus):TUpdateStatus;

        procedure ResetAutoInc;
        procedure Progress(Pct:integer; Code:TkbmProgressCode); virtual;
        function CopyRecords(Source,Destination:TDataSet; Count:TkbmNativeInt; IgnoreErrors:boolean; Mapping:string; WideStringAsUTF8:boolean):TkbmNativeInt; virtual;
        procedure AssignRecord(Source,Destination:TDataSet);
        procedure Lock; virtual;
        procedure Unlock; virtual;
        procedure UpdateFieldVariables;

        procedure CopyFieldProperties(Source,Destination:TField);
        procedure CopyFieldsProperties(Source,Destination:TDataSet);

        // Public high level routines.
        function Exists:boolean;
        procedure CreateTable;
        procedure EmptyTable;
        procedure CreateTableAs(Source:TDataSet; CopyOptions:TkbmMemTableCopyTableOptions);
        procedure DeleteTable;
        procedure PackTable;

        function AddIndex(const Name, Fields: string; Options: TIndexOptions):TkbmIndex; overload;
        function AddIndex(const Name, Fields: string; Options: TIndexOptions; AUpdateStatus:TUpdateStatusSet):TkbmIndex; overload;

        function AddIndex2(const Name, Fields: string; Options: TkbmMemTableCompareOptions):TkbmIndex; overload;
        function AddIndex2(const Name, Fields: string; Options: TkbmMemTableCompareOptions; AUpdateStatus:TUpdateStatusSet):TkbmIndex; overload;

        function AddFilteredIndex(const Name, Fields: string; Options: TIndexOptions; Filter:string; FilterOptions:TFilterOptions; FilterFunc:TkbmOnFilterIndex = nil):TkbmIndex; overload;
        function AddFilteredIndex(const Name, Fields: string; Options: TIndexOptions; AUpdateStatus:TUpdateStatusSet; Filter:string; FilterOptions:TFilterOptions; FilterFunc:TkbmOnFilterIndex=nil):TkbmIndex; overload;

        function AddFilteredIndex2(const Name, Fields: string; Options: TkbmMemTableCompareOptions; Filter:string; FilterOptions:TFilterOptions; FilterFunc:TkbmOnFilterIndex = nil):TkbmIndex; overload;
        function AddFilteredIndex2(const Name, Fields: string; Options: TkbmMemTableCompareOptions; AUpdateStatus:TUpdateStatusSet; Filter:string; FilterOptions:TFilterOptions; FilterFunc:TkbmOnFilterIndex = nil):TkbmIndex; overload;

        procedure DeleteIndex(const Name: string);
        procedure UpdateIndexes;
        function IndexFieldCount:Integer;
        procedure StartTransaction; virtual;
        procedure Commit; virtual;
        procedure Rollback; virtual;
{$IFDEF KBMMEMTABLE_SUPPORT_EXPRESSIONPARSER}
        function  TestFilter(const AFilter:string; AFilterOptions:TFilterOptions):boolean;
{$ENDIF}
        procedure Undo;

        procedure LoadFromFile(const FileName: string);
        procedure LoadFromStream(Stream:TStream);
        procedure LoadFromFileViaFormat(const FileName:string; AFormat:TkbmCustomStreamFormat);
        procedure LoadFromStreamViaFormat(Stream: TStream; AFormat:TkbmCustomStreamFormat);
        procedure SaveToFile(const FileName: string);
        procedure SaveToStream(Stream: TStream);
        procedure SaveToFileViaFormat(const FileName:string; AFormat:TkbmCustomStreamFormat);
        procedure SaveToStreamViaFormat(Stream: TStream; AFormat:TkbmCustomStreamFormat);

        procedure LoadFromDataSet(Source:TDataSet; CopyOptions:TkbmMemTableCopyTableOptions; Mapping:string=''); virtual;
        procedure SaveToDataSet(Destination:TDataSet; CopyOptions:TkbmMemTableCopyTableOptions = []; Mapping:string=''); virtual;
        procedure UpdateToDataSet(Destination:TDataSet; KeyFields:String; Flags:TkbmMemTableUpdateFlags; Mapping:string=''); overload; virtual;
        procedure UpdateToDataSet(Destination:TDataSet; KeyFields:String; Mapping:string=''); overload; virtual;

        procedure SortDefault;
        procedure Sort(Options:TkbmMemTableCompareOptions);
        procedure SortOn(const FieldNames:string; Options:TkbmMemTableCompareOptions);
        function Lookup(const KeyFields: string; const KeyValues: Variant; const ResultFields: string): Variant; override;
        function LookupByIndex(const IndexName:string; const KeyValues:Variant;
                               const ResultFields:string; RespFilter:boolean):Variant;
        function Locate(const KeyFields: string; const KeyValues: Variant; Options: TLocateOptions): Boolean; override;
        procedure SetKey;
        procedure EditKey;
        function GotoNearest:boolean;
        function GotoKey:boolean;
        function FindKey(const KeyValues:array of const): Boolean;
        function FindNearest(const KeyValues:array of const): Boolean;
        function DeleteRecords:integer; virtual;
        procedure ApplyRange;
        procedure CancelRange;
        procedure SetRange(const StartValues, EndValues:array of const); overload;
        procedure SetRange(const AFields:string; const StartValues, EndValues:array of variant); overload;
        function DeleteRange(const AFields:string; const StartValues, EndValues:array of variant):integer;
        procedure SetRangeStart;
        procedure SetRangeEnd;
        procedure EditRangeStart;
        procedure EditRangeEnd;
        procedure CheckPoint(const AForce:boolean = false; const AMarkAsInserted:boolean = false);
        function CheckPointRecord(RecordIndex:integer; const AForce:boolean = false; const AMarkAsInserted:boolean = false):TUpdateStatus;

        function GetRows(Rows:Integer; Start:Variant; Fields:Variant):Variant;
        procedure Reset;
        procedure GotoCurrent(DataSet:TkbmCustomMemTable); virtual;

        procedure GroupBy(ADestDataset:TkbmCustomMemTable; AGroupFields:string; AAggregateFields:string); overload; virtual;
        procedure GroupBy(ADestDataset:TkbmCustomMemTable; ASourceGroupFieldList:TkbmFieldList; ASourceAggregateFieldList,ADestAggregateFieldList:TkbmFieldList); overload; virtual;
        function Aggregate(AAggregateFields:string):variant; virtual;

        function Extract(AExtractFields:string; AStringList:TStrings; AFormat:string = ''):integer;

        property OverrideActiveRecordBuffer:PkbmRecord read FOverrideActiveRecordBuffer write FOverrideActiveRecordBuffer;

        property PersistentSaved:boolean read FPersistentSaved write FPersistentSaved stored false;
        property AttachedTo:TkbmCustomMemTable read FAttachedTo write SetAttachedTo;
        property AttachedAutoRefresh:boolean read FAttachedAutoRefresh write FAttachedAutoRefresh;

        property Performance:TkbmPerformance read GetPerformance write SetPerformance             default mtpfFast;

        property Filtered;
        property Filter;
        property CurIndex:TkbmIndex read FCurIndex;
        property IgnoreAutoIncPopulation:boolean read FIgnoreAutoIncPopulation write FIgnoreAutoIncPopulation;
        property AttachMaxCount:integer read GetAttachMaxCount write SetAttachMaxCount;
        property AttachCount:integer read GetAttachCount;
        property DesignActivation:boolean read FDesignActivation write FDesignActivation;
        property LanguageID:integer read GetLanguageID write SetLanguageID;
        property SortID:integer read GetSortID write SetSortID;
        property SubLanguageID:integer read GetSubLanguageID write SetSubLanguageID;
        property LocaleID:TkbmLocaleID read GetLocaleID write SetLocaleID;
        property Common:TkbmCommon read FCommon;
        property AutoIncValue:TkbmNativeInt read GetAutoIncValue;
        property AutoIncMinValue:TkbmNativeInt read GetAutoIncMin write SetAutoIncMinValue              default 0;
        property AutoUpdateFieldVariables:boolean read FAutoUpdateFieldVariables write SetAutoUpdateFieldVariables;
        property AllData:variant read GetAllData write SetAllData;
        property StoreDataOnForm:boolean read FStoreDataOnForm write FStoreDataOnForm             default false;
        property CommaText:string read GetCommaText write SetCommaText;
        property Capacity:TkbmNativeInt read GetCapacity write SetCapacity;
        property DeletedRecordsCount:TkbmNativeInt read GetDeletedRecordsCount;
        property IndexFieldNames:string read FIndexFieldNames write SetIndexFieldNames;
        property IndexName:string read FIndexName write SetIndexName;
        property EnableIndexes:boolean read FEnableIndexes write FEnableIndexes                   default true;
        property AutoAddIndexes:boolean read FAutoAddIndexes write FAutoAddIndexes                default false;
        property AutoReposition:boolean read FAutoReposition write FAutoReposition                default false;
        property SortFields:string read FSortFieldNames write FSortFieldNames;
        property SortOptions:TkbmMemTableCompareOptions read FSortOptions write FSortOptions;
        property ReadOnly:boolean read FReadOnly write FReadOnly                                  default false;
        property Standalone:boolean read GetStandalone write SetStandalone                        default false;
        property IgnoreReadOnly:boolean read FIgnoreReadOnly write FIgnoreReadOnly                default false;
        property RangeActive:boolean read GetRangeActive;
        property RangeIgnoreNullKeyValues:boolean read FRangeIgnoreNullKeyValues write FRangeIgnoreNullKeyValues default true;
        property PersistentFile:TFileName read FPersistentFile write FPersistentFile;
        property Persistent:boolean read FPersistent write FPersistent                            default false;
        property PersistentBackup:boolean read FPersistentBackup write FPersistentBackup;
        property PersistentBackupExt:string read FPersistentBackupExt write FPersistentBackupExt;
        property ProgressFlags:TkbmProgressCodes read FProgressFlags write FProgressFlags;
        property LoadLimit:TkbmNativeInt read FLoadLimit write FLoadLimit                               default -1;
        property LoadCount:TkbmNativeInt read FLoadCount write FLoadCount;
        property LoadedCompletely:boolean read FLoadedCompletely write FLoadedCompletely;
        property SaveLimit:TkbmNativeInt read FSaveLimit write FSaveLimit                               default -1;
        property SaveCount:TkbmNativeInt read FSaveCount write FSaveCount;
        property SavedCompletely:boolean read FSavedCompletely write FSavedCompletely;
        property RecalcOnFetch:boolean read FRecalcOnFetch write FRecalcOnFetch                   default true;
        property IsFieldModified[i:integer]:boolean read GetModifiedFlags;
        property EnableVersioning:boolean read GetEnableVersioning write SetEnableVersioning      default false;
        property VersioningMode:TkbmVersioningMode read GetVersioningMode write SetVersioningMode default mtvm1SinceCheckPoint;
        property IsVersioning:boolean read GetIsVersioning;
        property StatusFilter:TUpdateStatusSet read FStatusFilter write SetStatusFilter;
        property DeltaHandler:TkbmCustomDeltaHandler read FDeltaHandler write SetDeltaHandler;
        property Indexes:TkbmIndexes read GetIndexes;
        property IndexByName[IndexName:string]:TkbmIndex read GetIndexByName;
        property IndexDefs:TIndexDefs read FIndexDefs write SetIndexDefs;
        property IndexFields[Index:Integer]:TField read GetIndexField write SetIndexField;
        property RecalcOnIndex:boolean read FRecalcOnIndex write FRecalcOnIndex                   default false;
        property FilterOptions:TFilterOptions read FFilterOptions write FFilterOptions;
        property DetailFields: string read FDetailFieldNames write SetDetailFields;
        property MasterFields: string read GetMasterFields write SetMasterFields;
        property MasterSource: TDataSource read GetDataSource write SetDataSource;
        property RecordTag: TkbmNativeInt read GetRecordTag write SetRecordTag;
        property Version:string read GetVersion write FDummyStr;
        property IsDataModified:boolean read GetIsDataModified write SetIsDataModified;
        property TransactionLevel:integer read GetTransactionLevel;
        property TableState:TkbmState read FState write FState;
        property DefaultFormat:TkbmCustomStreamFormat read FDefaultFormat write FDefaultFormat;
        property CommaTextFormat:TkbmCustomStreamFormat read FCommaTextFormat write FCommaTextFormat;
        property PersistentFormat:TkbmCustomStreamFormat read FPersistentFormat write FPersistentFormat;
        property FormFormat:TkbmCustomStreamFormat read FFormFormat write FFormFormat;
        property AllDataFormat:TkbmCustomStreamFormat read FAllDataFormat write FAllDataFormat;
        property OnLoadRecord:TkbmOnLoadRecord read FOnLoadRecord write FOnLoadRecord;
        property OnLoadField:TkbmOnLoadField read FOnLoadField write FOnLoadField;
        property OnSaveRecord:TkbmOnSaveRecord read FOnSaveRecord write FOnSaveRecord;
        property OnSaveField:TkbmOnSaveField read FOnSaveField write FOnSaveField;
        property OnCompressBlobStream:TkbmOnCompress read FOnCompressBlobStream write FOnCompressBlobStream;
        property OnDecompressBlobStream:TkbmOnDecompress read FOnDecompressBlobStream write FOnDecompressBlobStream;
        property OnSetupField:TkbmOnSetupField read FOnSetupField write FOnSetupField;
        property OnSetupFieldProperties:TkbmOnSetupFieldProperties read FOnSetupFieldProperties write FOnSetupFieldProperties;
        property OnCompressField:TkbmOnCompressField read FOnCompressField write FOnCompressField;
        property OnDecompressField:TkbmOnDecompressField read FOnDecompressField write FOnDecompressField;
        property OnSave:TkbmOnSave read FOnSave write FOnSave;
        property OnLoad:TkbmOnLoad read FOnLoad write FOnLoad;
        property OnProgress:TkbmOnProgress read FOnProgress write FOnProgress;
        property OnCompareFields:TkbmOnCompareFields read FOnCompareFields write FOnCompareFields;
        property OnFilterIndex:TkbmOnFilterIndex read FOnFilterIndex write FOnFilterIndex;
        property OnUserAggregate:TkbmOnUserAggregate read FOnUserAggregate write FOnUserAggregate;
        property BeforeOpen;
        property AfterOpen;
        property BeforeClose;
        property AfterClose;
        property BeforeInsert:TDatasetNotifyEvent read FBeforeInsert write FBeforeInsert;
        property AfterInsert;
        property BeforeEdit;
        property AfterEdit;
        property BeforePost;
        property AfterPost;
        property BeforeCancel;
        property AfterCancel;
        property BeforeDelete;
        property AfterDelete;
        property BeforeScroll;
        property AfterScroll;
        property OnCalcFields;
        property OnDeleteError;
        property OnEditError;
        property OnFilterRecord;
        property OnNewRecord;
        property OnPostError;
        property Active;
  end;

{$IFDEF LEVEL16}
  [ComponentPlatformsAttribute({$IFDEF LEVEL23}pidiOSDevice64 or {$ENDIF}{$IFDEF LEVEL18}pidiOSSimulator or pidiOSDevice or {$ENDIF}{$IFDEF LEVEL19}pidAndroid or {$ENDIF}pidWin32 or pidWin64{$IFDEF LEVEL17} or pidOSX32{$ENDIF})]
{$ENDIF}
  TkbmMemTable = class(TkbmCustomMemTable)
  public
        property IgnoreReadOnly;
  published
        property Active;
        property DesignActivation;
        property AttachedTo;
        property AttachedAutoRefresh;
        property AttachMaxCount;
        property AutoIncMinValue;
        property AutoCalcFields;
        property FieldDefs;
        property Filtered;
        property DeltaHandler;
        property EnableIndexes;
        property AutoAddIndexes;
        property AutoReposition;
        property IndexFieldNames;
        property IndexName;
        property IndexDefs;
        property RecalcOnIndex;
        property RecalcOnFetch;
        property SortFields;
        property SortOptions;
        property ReadOnly;
        property Performance;
        property Standalone;
        property PersistentFile;
        property StoreDataOnForm;
        property Persistent;
        property PersistentBackup;
        property PersistentBackupExt;
        property ProgressFlags;
        property LoadLimit;
        property LoadedCompletely;
        property SaveLimit;
        property SavedCompletely;
        property EnableVersioning;
        property VersioningMode;
        property Filter;
        property FilterOptions;
        property MasterFields;
        property DetailFields;
        property MasterSource;
        property Version;
        property LanguageID;
        property SortID;
        property SubLanguageID;
        property LocaleID;
        property DefaultFormat;
        property CommaTextFormat;
        property PersistentFormat;
        property AllDataFormat;
        property FormFormat;
        property RangeIgnoreNullKeyValues;
  {$IFNDEF KBMMEMTABLE_DONT_SUPPORT_PUBLISHED_RECORDS}
        property FormatSettings:TFormatSettings read FFormatSettings write FFormatSettings;
  {$ENDIF}
        property OnProgress;
        property OnLoadRecord;
        property OnLoadField;
        property OnSaveRecord;
        property OnSaveField;
        property OnCompressBlobStream;
        property OnDecompressBlobStream;
        property OnSetupField;
        property OnSetupFieldProperties;
        property OnCompressField;
        property OnDecompressField;
        property OnSave;
        property OnLoad;
        property OnCompareFields;
        property OnFilterIndex;
        property OnUserAggregate;
        property BeforeOpen;
        property AfterOpen;
        property BeforeClose;
        property AfterClose;
        property BeforeInsert;
        property AfterInsert;
        property BeforeEdit;
        property AfterEdit;
        property BeforePost;
        property AfterPost;
        property BeforeCancel;
        property AfterCancel;
        property BeforeDelete;
        property AfterDelete;
        property BeforeScroll;
        property AfterScroll;
        property BeforeRefresh;
        property AfterRefresh;
        property OnCalcFields;
        property OnDeleteError;
        property OnEditError;
        property OnFilterRecord;
        property OnNewRecord;
        property OnPostError;
  end;

  TkbmBlobStream = class(TMemoryStream)
  private
    FWorkBuffer:PkbmRecord;
    FTableRecord:PkbmRecord;

    FField: TBlobField;
    FDataSet: TkbmCustomMemTable;
    FMode:TBlobStreamMode;
    FFieldNo: Integer;
    FModified: Boolean;

    // Internal work pointers.
    FpWorkBufferField:PByte;
    FpTableRecordField:PByte;

    FpWorkBufferBlob:PPkbmVarLength;
    FpTableRecordBlob:PPkbmVarLength;

    procedure ReadBlobData;
    procedure WriteBlobData;
  public
    constructor Create(Field: TBlobField; Mode: TBlobStreamMode);
    destructor Destroy; override;
    function Write(const Buffer; Count: Longint): Longint; override;
    procedure Truncate;
  end;

  // Handler which user must override to provide functionality when trying to update deltas on an external database.
  TkbmDeltaHandlerGetValue = procedure(ADeltaHandler:TkbmCustomDeltaHandler; AField:TField; var AValue:variant) of object;

{$IFDEF LEVEL16}
  [ComponentPlatformsAttribute({$IFDEF LEVEL23}pidiOSDevice64 or {$ENDIF}{$IFDEF LEVEL18}pidiOSSimulator or pidiOSDevice or {$ENDIF}{$IFDEF LEVEL19}pidAndroid or {$ENDIF}pidWin32 or pidWin64{$IFDEF LEVEL17} or pidOSX32{$ENDIF})]
{$ENDIF}
  TkbmCustomDeltaHandler = class(TComponent)
  private
     FOnGetValue:TkbmDeltaHandlerGetValue;
     FDataSet:TkbmCustomMemTable;
     procedure CheckDataSet;
     function GetValues(Index:integer):Variant;
     function GetOrigValues(Index:integer):Variant;
     function GetFieldCount:integer;
     function GetFieldNames(Index:integer):string;
     function GetFields(Index:integer):TField;
     function GetOrigValuesByName(Name:string):Variant;
     function GetValuesByName(Name:string):Variant;
     function GetRecordNo:TkbmNativeInt;
     function GetUniqueRecordID:TkbmNativeInt;
  protected
     FPRecord,FPOrigRecord:PkbmRecord;

     procedure BeforeRecord; virtual;
     procedure InsertRecord(var Retry:boolean; var State:TUpdateStatus); virtual;
     procedure DeleteRecord(var Retry:boolean; var State:TUpdateStatus); virtual;
     procedure ModifyRecord(var Retry:boolean; var State:TUpdateStatus); virtual;
     procedure UnmodifiedRecord(var Retry:boolean; var State:TUpdateStatus); virtual;
     procedure AfterRecord; virtual;

     procedure Notification(AComponent: TComponent; Operation: TOperation); override;
  public
     procedure Resolve; virtual;
     property DataSet:TkbmCustomMemTable read FDataSet write FDataSet;
     property FieldCount:integer read GetFieldCount;
     property OrigValues[i:integer]:Variant read GetOrigValues;
     property Values[i:integer]:Variant read GetValues;
     property OrigValuesByName[Name:string]:Variant read GetOrigValuesByName;
     property ValuesByName[Name:string]:Variant read GetValuesByName;
     property FieldNames[i:integer]:string read GetFieldNames;
     property Fields[i:integer]:TField read GetFields;
     property RecNo:TkbmNativeInt read GetRecordNo;
     property UniqueRecID:TkbmNativeInt read GetUniqueRecordID;
  published
     property OnGetValue:TkbmDeltaHandlerGetValue read FOnGetValue write FOnGetValue;
  end;

  function StreamToVariant(stream:TStream):variant;
  procedure VariantToStream(AVariant:variant; stream:TStream);

  function CompareFields(const KeyField,AField:pointer; const FieldType: TFieldType; const LocaleID:TkbmLocaleID; const IndexFieldOptions:TkbmifoOptions; var FullCompare:boolean):Integer;

  function IndexOptions2CompareOptions(AOptions:TIndexOptions):TkbmMemTableCompareOptions;
  function CompareOptions2IndexOptions(AOptions:TkbmMemTableCompareOptions):TIndexOptions;

  procedure CopyFieldDefs(Source:TFieldDefs; Dest:TFieldDefs);

const
  // All supported field types.
  kbmSupportedFieldTypes:TkbmFieldTypes=[DB.ftString
                                        ,DB.ftSmallint
                                        ,DB.ftInteger
                                        ,DB.ftWord
                                        ,DB.ftBoolean
                                        ,DB.ftFloat
                                        ,DB.ftCurrency
                                        ,DB.ftDate
                                        ,DB.ftTime
                                        ,DB.ftDateTime
                                        ,DB.ftAutoInc
                                        ,DB.ftBCD
                                        ,DB.ftFmtBCD
                                        ,DB.ftTimeStamp
                                        ,DB.ftBlob
                                        ,DB.ftMemo
                                        ,DB.ftGraphic
                                        ,DB.ftFmtMemo
                                        ,DB.ftParadoxOle
                                        ,DB.ftDBaseOle
                                        ,DB.ftTypedBinary
                                        ,DB.ftBytes
                                        ,DB.ftVarBytes
                                        ,DB.ftFixedChar
                                        ,DB.ftWideString
                                        ,DB.ftLargeInt
                                        ,DB.ftADT
                                        ,DB.ftArray
                                        ,DB.ftOraBlob
                                        ,DB.ftOraClob
                                        ,DB.ftGUID
{$IFDEF KBMMEMTABLE_SUPPORT_FT_FIXEDWIDECHAR}
                                        ,DB.ftFixedWideChar
{$endif}
{$IFDEF KBMMEMTABLE_SUPPORT_FT_WIDEMEMO}
                                        ,DB.ftWideMemo
{$ENDIF}
{$IFDEF KBMMEMTABLE_SUPPORT_FT_ORATIMESTAMP}
                                        ,DB.ftOraTimeStamp
{$endif}
{$IFDEF KBMMEMTABLE_SUPPORT_FT_ORAINTERVAL}
                                        ,DB.ftOraInterval
{$ENDIF}
{$IFDEF KBMMEMTABLE_SUPPORT_FT_LONGWORD}
                                        ,DB.ftLongWord
{$ENDIF}
{$IFDEF KBMMEMTABLE_SUPPORT_FT_SHORTINT}
                                        ,DB.ftShortint
{$ENDIF}
{$IFDEF KBMMEMTABLE_SUPPORT_FT_BYTE}
                                        ,DB.ftByte
{$ENDIF}
{$IFDEF KBMMEMTABLE_SUPPORT_FT_EXTENDED}
                                        ,DB.ftExtended
{$ENDIF}
{$IFDEF KBMMEMTABLE_SUPPORT_FT_SINGLE}
                                        ,DB.ftSingle
{$ENDIF}
                                        ];

  // All field types which should be treated as strings during save and load.
  kbmStringTypes:TkbmFieldTypes=[DB.ftString
                                ,DB.ftMemo
                                ,DB.ftFmtMemo
                                ,DB.ftWideString
                                ,DB.ftFixedChar
                                ,DB.ftOraClob
                                ,DB.ftGuid
{$IFDEF KBMMEMTABLE_SUPPORT_FT_FIXEDWIDECHAR}
                                ,DB.ftFixedWideChar
{$ENDIF}
{$IFDEF KBMMEMTABLE_SUPPORT_FT_WIDEMEMO}
                                ,DB.ftWideMemo
{$ENDIF}
{$IFDEF KBMMEMTABLE_SUPPORT_FT_ORAINTERVAL}
                                ,DB.ftOraInterval
{$endif}
                                ];
  // All field types which should be treated as binary types during save and load.
  kbmBinaryTypes:TkbmFieldTypes=[DB.ftBlob
                                ,DB.ftMemo
                                ,DB.ftGraphic
                                ,DB.ftFmtMemo
                                ,DB.ftParadoxOle
                                ,DB.ftDBaseOle
                                ,DB.ftTypedBinary
                                ,DB.ftVarBytes
                                ,DB.ftBytes
                                ,DB.ftOraBlob
                                ,DB.ftOraClob
                                 ];

  // All field types which should be treated as blobs.
  kbmBlobTypes:TkbmFieldTypes=[DB.ftBlob
                              ,DB.ftMemo
                              ,DB.ftGraphic
                              ,DB.ftFmtMemo
                              ,DB.ftParadoxOle
                              ,DB.ftDBaseOle
                              ,DB.ftTypedBinary
                              ,DB.ftOraBlob
                              ,DB.ftOraClob
{$IFDEF KBMMEMTABLE_SUPPORT_FT_WIDEMEMO}
                              ,DB.ftWideMemo
{$ENDIF}
                              ];

  // All non blob field types.
  kbmNonBlobTypes:TkbmFieldTypes=[DB.ftString
                                 ,DB.ftSmallint
                                 ,DB.ftInteger
                                 ,DB.ftWord
                                 ,DB.ftBoolean
                                 ,DB.ftFloat
                                 ,DB.ftCurrency
                                 ,DB.ftDate
                                 ,DB.ftTime
                                 ,DB.ftDateTime
                                 ,DB.ftAutoInc
                                 ,DB.ftBCD
                                 ,DB.ftFmtBCD
                                 ,DB.ftTimeStamp
                                 ,DB.ftBytes
                                 ,DB.ftVarBytes
                                 ,DB.ftGUID
                                 ,DB.ftWideString
                                 ,DB.ftFixedChar
                                 ,DB.ftLargeInt
                                 ,DB.ftADT
                                 ,DB.ftArray
{$IFDEF KBMMEMTABLE_SUPPORT_FT_FIXEDWIDECHAR}
                                 ,DB.ftFixedWideChar
{$endif}
{$IFDEF KBMMEMTABLE_SUPPORT_FT_ORATIMESTAMP}
                                 ,DB.ftOraTimeStamp
{$ENDIF}
{$IFDEF KBMMEMTABLE_SUPPORT_FT_ORAINTERVAL}
                                 ,DB.ftOraInterval
{$ENDIF}
{$IFDEF KBMMEMTABLE_SUPPORT_FT_LONGWORD}
                                 ,DB.ftLongWord
{$ENDIF}
{$IFDEF KBMMEMTABLE_SUPPORT_FT_SHORTINT}
                                 ,DB.ftShortint
{$ENDIF}
{$IFDEF KBMMEMTABLE_SUPPORT_FT_BYTE}
                                 ,DB.ftByte
{$ENDIF}
{$IFDEF KBMMEMTABLE_SUPPORT_FT_EXTENDED}
                                 ,DB.ftExtended
{$ENDIF}
{$IFDEF KBMMEMTABLE_SUPPORT_FT_SINGLE}
                                 ,DB.ftSingle
{$ENDIF}
                                 ];

  kbmIndexableTypes:TkbmFieldTypes=[DB.ftString
                                 ,DB.ftSmallint
                                 ,DB.ftInteger
                                 ,DB.ftWord
                                 ,DB.ftBoolean
                                 ,DB.ftFloat
                                 ,DB.ftCurrency
                                 ,DB.ftDate
                                 ,DB.ftTime
                                 ,DB.ftDateTime
                                 ,DB.ftAutoInc
                                 ,DB.ftBCD
                                 ,DB.ftFmtBCD
                                 ,DB.ftTimeStamp
                                 ,DB.ftBytes
                                 ,DB.ftVarBytes
                                 ,DB.ftGUID
                                 ,DB.ftWideString
                                 ,DB.ftFixedChar
                                 ,DB.ftLargeInt
                                 ,DB.ftADT
                                 ,DB.ftArray
                                 ,DB.ftMemo
                                 ,DB.ftFmtMemo
{$IFDEF KBMMEMTABLE_SUPPORT_FT_FIXEDWIDECHAR}
                                 ,DB.ftFixedWideChar
{$endif}
{$IFDEF KBMMEMTABLE_SUPPORT_FT_WIDEMEMO}
                                 ,DB.ftWideMemo
{$ENDIF}
{$IFDEF KBMMEMTABLE_SUPPORT_FT_ORATIMESTAMP}
                                 ,DB.ftOraTimeStamp
{$ENDIF}
{$IFDEF KBMMEMTABLE_SUPPORT_FT_ORAINTERVAL}
                                 ,DB.ftOraInterval
{$ENDIF}
{$IFDEF KBMMEMTABLE_SUPPORT_FT_LONGWORD}
                                 ,DB.ftLongWord
{$ENDIF}
{$IFDEF KBMMEMTABLE_SUPPORT_FT_SHORTINT}
                                 ,DB.ftShortint
{$ENDIF}
{$IFDEF KBMMEMTABLE_SUPPORT_FT_BYTE}
                                 ,DB.ftByte
{$ENDIF}
{$IFDEF KBMMEMTABLE_SUPPORT_FT_EXTENDED}
                                 ,DB.ftExtended
{$ENDIF}
{$IFDEF KBMMEMTABLE_SUPPORT_FT_SINGLE}
                                 ,DB.ftSingle
{$ENDIF}
                                 ];

  // Field types which should be stored as a variable chunk of memory.
  // Blobs are automatically treated as variable length datatypes.
  kbmVarLengthNonBlobTypes:TkbmFieldTypes=[DB.ftString
                                          ,DB.ftBytes
                                          ,DB.ftVarBytes
                                          ,DB.ftWideString
                                          ,DB.ftFixedChar
                                    ];

  NullVarLength=PkbmVarLength(0);

{KBMDEL
  mtifoDescending=$01;
  mtifoCaseInsensitive=$02;
  mtifoPartial=$04;
  mtifoIgnoreNull=$08;
  mtifoIgnoreLocale=$10;
}

  FieldKindNames: array[0..4] of string = (
    'Data', 'Calculated', 'Lookup', 'InternalCalc', 'Aggregate');

implementation

uses
{$include kbmMemRes.inc}
  TypInfo, IniFiles, Math

{$IFNDEF FPC}
 {$IFNDEF LEVEL16}
  ,Forms
 {$ENDIF}
{$ENDIF}

{$IFNDEF FPC}
 {$IFNDEF NEXTGEN}
  ,WideStrUtils
 {$ENDIF}
{$ENDIF}

{$IFDEF FPC}
  ,DBConst
{$ELSE}
  ,DBConsts
{$ENDIF}

{$IFNDEF KBMMEMTABLE_SUPPORT_EXPRESSIONPARSER}
  ,kbmSQLParser
  ,kbmSQLElements
{$ENDIF}
  ;

const
  // Field mappings needed for filtering. (What field type should be compared with what internal type).
  FldTypeMap: TFieldMap = (
     ord(DB.ftUnknown)     // ftUnknown
    ,ord(DB.ftString)      // ftString
    ,ord(DB.ftSmallInt)    // ftSmallInt
    ,ord(DB.ftInteger)     // ftInteger
    ,ord(DB.ftWord)        // ftWord
    ,ord(DB.ftBoolean)     // ftBoolean
    ,ord(DB.ftFloat)       // ftFloat
    ,ord(DB.ftFloat)       // ftCurrency
    ,ord(DB.ftBCD)         // ftBCD
    ,ord(DB.ftDate)        // ftDate
    ,ord(DB.ftTime)        // ftTime
    ,ord(DB.ftDateTime)    // ftDateTime
    ,ord(DB.ftBytes)       // ftBytes
    ,ord(DB.ftVarBytes)    // ftVarBytes
    ,ord(DB.ftInteger)     // ftAutoInc
    ,ord(DB.ftBlob)        // fBlob
    ,ord(DB.ftBlob)        // ftMemo
    ,ord(DB.ftBlob)        // ftGraphic
    ,ord(DB.ftBlob)        // ftFmtMemo
    ,ord(DB.ftBlob)        // ftParadoxOle
    ,ord(DB.ftBlob)        // ftDBaseOle
    ,ord(DB.ftBlob)        // ftTypedBinary
    ,ord(DB.ftUnknown)     // ftCursor
    ,ord(DB.ftString)      // ftFixedChar
    ,ord(DB.ftWideString)  // ftWideString
    ,ord(DB.ftLargeInt)    // ftLargeInt
    ,ord(DB.ftADT)         // ftADT
    ,ord(DB.ftArray)       // ftArray
    ,ord(DB.ftUnknown)     // ftReference
    ,ord(DB.ftUnknown)     // ftDataset
    ,ord(DB.ftBlob)        // ftOraBlob
    ,ord(DB.ftBlob)        // ftOraClob
    ,ord(DB.ftUnknown)     // ftVariant
    ,ord(DB.ftUnknown)     // ftInterface
    ,ord(DB.ftUnknown)     // ftIDispatch
    ,ord(DB.ftGUID)        // ftGUID
    ,ord(DB.ftTimeStamp)   // ftTimeStamp
    ,ord(DB.ftFmtBCD)      // ftFmtBCD
{$IFDEF KBMMEMTABLE_SUPPORT_FT_FIXEDWIDECHAR}
    ,ord(DB.ftWideString)  // ftFixedWideChar
{$ENDIF}
{$IFDEF KBMMEMTABLE_SUPPORT_FT_WIDEMEMO}
    ,ord(DB.ftWideString)  // ftWideMemo
{$ENDIF}
{$IFDEF KBMMEMTABLE_SUPPORT_FT_ORATIMESTAMP}
    ,ord(DB.ftTimeStamp)   // ftOraTimeStamp
{$ENDIF}
{$IFDEF KBMMEMTABLE_SUPPORT_FT_ORAINTERVAL}
    ,ord(DB.ftString)      // ftOraInterval)
{$ENDIF}
{$IFDEF KBMMEMTABLE_SUPPORT_FT_LONGWORD}
    ,ord(DB.ftLongWord)
{$ENDIF}
{$IFDEF KBMMEMTABLE_SUPPORT_FT_SHORTINT}
    ,ord(DB.ftShortint)
{$ENDIF}
{$IFDEF KBMMEMTABLE_SUPPORT_FT_BYTE}
    ,ord(DB.ftByte)
{$ENDIF}
{$IFDEF KBMMEMTABLE_SUPPORT_FT_EXTENDED}
    ,ord(DB.ftExtended)
{$ENDIF}
{$IFDEF KBMMEMTABLE_SUPPORT_FT_CONNECTION}
    ,ord(DB.ftUnknown)     // ftConnection
{$ENDIF}
{$IFDEF KBMMEMTABLE_SUPPORT_FT_STREAM}
    ,ord(DB.ftUnknown)     // ftParams
{$ENDIF}
{$IFDEF KBMMEMTABLE_SUPPORT_FT_PARAMS}
    ,ord(DB.ftUnknown)     // ftStream
{$ENDIF}
{$IFDEF KBMMEMTABLE_SUPPORT_FT_TIMESTAMPOFFSET}
    ,ord(DB.ftUnknown)     // ftTimeStampOffset
{$ENDIF}
{$IFDEF KBMMEMTABLE_SUPPORT_FT_OBJECT}
    ,ord(DB.ftUnknown)     // ftObject
{$ENDIF}
{$IFDEF KBMMEMTABLE_SUPPORT_FT_SINGLE}
    ,ord(DB.ftSingle)      // ftSingle
{$ENDIF}
    );

procedure kbmMemTableHandleException(ASender:TObject);
begin
{$IFDEF FPC}
      if Assigned(Classes.ApplicationHandleException) then
         Classes.ApplicationHandleException(ASender)
      else
          ShowException(ExceptObject,ExceptAddr);
{$ELSE}
 {$IFDEF LEVEL16}
        if Assigned(System.Classes.ApplicationHandleException) then
           System.Classes.ApplicationHandleException(ASender);
 {$ELSE}
        Application.HandleException(ASender);
 {$ENDIF}
{$ENDIF}
end;


{$IFDEF KBMMEMTABLE_DEBUG_LOCKS}
procedure TkbmCS.DumpAttachedTables(AText:string);
var
   i:TkbmNativeInt;
   lst:TList;
   s,a:string;
   t:TkbmCustomMemTable;
begin
     s:='';
     a:='';
     lst:=FCommon.FAttachedTables;
//     if lst=nil then
//     begin
//          OutputDebugString(PChar(' Attached tables = nil'));
//          exit;
//     end;
     for i:=0 to lst.Count-1 do
     begin
          t:=TkbmCustomMemTable(lst.Items[i]);
          s:=s+a+t.ClassName+'('+t.Name+')';
          a:=', ';
     end;
     OutputDebugString(PChar(AText+' Attached tables: '+s));
end;

procedure TkbmCS.DumpLockCount(AText:string);
begin
     OutputDebugString(PChar(AText+' CS Lockcount '+IntToStr(FCnt)+' '+FCommon.FOwner.ClassName + ' ('+FCommon.FOwner.Name+')'));
end;

constructor TkbmCS.Create(ACommon:TkbmCommon);
begin
     inherited Create;
     FCommon:=ACommon;
     FCnt:=0;
end;

destructor TkbmCS.Destroy;
begin
     inherited Destroy;
end;

procedure TkbmCS.Acquire;
begin
     inherited Acquire;
     inc(FCnt);
end;

procedure TkbmCS.Release;
begin
     dec(FCnt);
     inherited Release;
end;
{$ENDIF}

// -----------------------------------------------------------------------------------
// General procedures.
// -----------------------------------------------------------------------------------

function GetInt16(const Buffer:PByte; var Value:smallint):PByte; {$IFDEF KBMMEMTABLE_SUPPORT_INLINE}inline;{$ENDIF}
begin
     Result:=Buffer;
     Value:=byte(Result^); inc(Result);
     inc(Value,byte(Result^) shl 8); inc(Result);
end;

function PutInt16(const Buffer:PByte; const Value:smallint):PByte; {$IFDEF KBMMEMTABLE_SUPPORT_INLINE}inline;{$ENDIF}
begin
     Result:=Buffer;
     Result^:=byte(Value and $FF); inc(Result);
     Result^:=byte((Value shr 8) and $FF); inc(Result);
end;

function GetInt32(const Buffer:PByte; var Value:integer):PByte; {$IFDEF KBMMEMTABLE_SUPPORT_INLINE}inline;{$ENDIF}
begin
     Result:=Buffer;
     Value:=integer(byte(Result^)); inc(Result);
     inc(Value,integer(byte(Result^)) shl 8); inc(Result);
     inc(Value,integer(byte(Result^)) shl 16); inc(Result);
     inc(Value,integer(byte(Result^)) shl 24); inc(Result);
end;

function PutInt32(const Buffer:PByte; const Value:integer):PByte; {$IFDEF KBMMEMTABLE_SUPPORT_INLINE}inline;{$ENDIF}
begin
     Result:=Buffer;
     Result^:=byte(Value and $FF); inc(Result);
     Result^:=byte((Value shr 8) and $FF); inc(Result);
     Result^:=byte((Value shr 16) and $FF); inc(Result);
     Result^:=byte((Value shr 24) and $FF); inc(Result);
end;

function GetLongInt(const Buffer:PByte; var Value:TkbmLongInt):PByte; {$IFDEF KBMMEMTABLE_SUPPORT_INLINE}inline;{$ENDIF}
begin
     Result:=Buffer;
     Value:=longint(byte(Result^)); inc(Result);
     inc(Value,longint(byte(Result^)) shl 8); inc(Result);
     inc(Value,longint(byte(Result^)) shl 16); inc(Result);
     inc(Value,longint(byte(Result^)) shl 24); inc(Result);
end;

function PutLongInt(const Buffer:PByte; const Value:TkbmLongInt):PByte; {$IFDEF KBMMEMTABLE_SUPPORT_INLINE}inline;{$ENDIF}
begin
     Result:=Buffer;
     Result^:=byte(Value and $FF); inc(Result);
     Result^:=byte((Value shr 8) and $FF); inc(Result);
     Result^:=byte((Value shr 16) and $FF); inc(Result);
     Result^:=byte((Value shr 24) and $FF); inc(Result);
end;

function GetInt64(Buffer:PByte; var Value:int64):PByte; {$IFDEF KBMMEMTABLE_SUPPORT_INLINE}inline;{$ENDIF}
begin
     Result:=Buffer;
     Value:=byte(Result^); inc(Result);
     inc(Value,Int64(byte(Result^)) shl 8); inc(Result);
     inc(Value,Int64(byte(Result^)) shl 16); inc(Result);
     inc(Value,Int64(byte(Result^)) shl 24); inc(Result);
     inc(Value,Int64(byte(Result^)) shl 32); inc(Result);
     inc(Value,Int64(byte(Result^)) shl 40); inc(Result);
     inc(Value,Int64(byte(Result^)) shl 48); inc(Result);
     inc(Value,Int64(byte(Result^)) shl 56); inc(Result);
end;

function PutInt64(const Buffer:PByte; const Value:int64):PByte;
begin
     Result:=Buffer;
     Result^:=byte(Value and $FF); inc(Result);
     Result^:=byte((Value shr 8) and $FF); inc(Result);
     Result^:=byte((Value shr 16) and $FF); inc(Result);
     Result^:=byte((Value shr 24) and $FF); inc(Result);
     Result^:=byte((Value shr 32) and $FF); inc(Result);
     Result^:=byte((Value shr 40) and $FF); inc(Result);
     Result^:=byte((Value shr 48) and $FF); inc(Result);
     Result^:=byte((Value shr 56) and $FF); inc(Result);
end;

// Allocate a varlength.
function AllocVarLength(Size:TkbmLongInt):PkbmVarLength;
begin
     GetMem(Result,Size+4);
     FillChar(Result^,Size+4,0);
     PutLongInt(Result,Size);
end;

// Get pointer to varlength data.
function GetVarLengthData(AVarLength:PkbmVarLength):PByte; {$IFDEF KBMMEMTABLE_SUPPORT_INLINE}inline;{$ENDIF}
begin
     Result:=PByte(AVarLength)+4;
end;

// Get size of varlength data.
function GetVarLengthSize(AVarLength:PkbmVarLength):TkbmLongInt; {$IFDEF KBMMEMTABLE_SUPPORT_INLINE}inline;{$ENDIF}
begin
     GetLongInt(AVarLength,Result);
end;

// Allocate a varlength and populate it.
function AllocVarLengthAs(const Source:PByte; Size:TkbmLongInt):PkbmVarLength; {$IFDEF KBMMEMTABLE_SUPPORT_INLINE}inline;{$ENDIF}
begin
     Result:=AllocVarLength(Size);
     kbmMemMove(Source^,GetVarLengthData(Result)^,Size);
end;

// Duplicate varlength.
function CopyVarLength(AVarLength:PkbmVarLength):PkbmVarLength; {$IFDEF KBMMEMTABLE_SUPPORT_INLINE}inline;{$ENDIF}
var
   sz:TkbmLongInt;
begin
     sz:=GetVarLengthSize(AVarLength);
     Result:=AllocVarLength(sz);
     kbmMemMove(GetVarLengthData(AVarLength)^,GetVarLengthData(Result)^,sz);
end;

{$IFDEF KBM_DEBUG}
procedure DumpData(AString:string; APointer:PByte; ACount:integer);
var
   i:TkbmNativeInt;
   s:string;
begin
     s:='';
     for i:=0 to ACount-1 do
     begin
          s:=s+IntToHex(APointer^,2)+' ';
          inc(APointer);
     end;
     OutputDebugString(PChar(AString+': '+s));
end;

// Dump varlength.
procedure DumpVarLength(AVarLength:PkbmVarLength);
var
   s:string;
   i:TkbmLongInt;
   sz:TkbmLongInt;
   p:PByte;
begin
     s:='VarLength '+inttohex(TkbmLongInt(AVarLength),8);
     OutputDebugString(PChar(s));
     s:=inttostr(byte(AVarLength[0]))+inttostr(byte(AVarLength[1]))+
        inttostr(byte(AVarLength[2]))+inttostr(byte(AVarLength[3]));
     s:='Size data='+s;
     OutputDebugString(PChar(s));
     sz:=GetVarLengthSize(AVarLength);
     s:=' Size='+inttostr(sz);
     OutputDebugString(PChar(s));
     p:=GetVarLengthData(AVarLength);
     s:='';
     for i:=0 to sz-1 do
         s:=s+Chr(p[i]);
     s:=' Data='+s;
     OutputDebugString(PChar(s));
end;
{$ENDIF}

// Free a varlength.
procedure FreeVarLength(AVarLength:PkbmVarLength); {$IFDEF KBMMEMTABLE_SUPPORT_INLINE}inline;{$ENDIF}
begin
     if (AVarLength <> nil) then FreeMem(AVarLength);
end;

{$IFNDEF NEXTGEN}
// Extract WideString from a buffer.
function WideStringFromBuffer(ABuffer:pointer):WideString;
var
   sz:integer;
   p:PByte;
begin
     Result:='JENS';
 {$IFDEF KBMMEMTABLE_USE_WSTRLEN}
     sz:=WStrLen(PWideChar(ABuffer));
     p:=PByte(ABuffer);
     SetLength(Result,sz);
     sz:=sz*SizeOf(WideChar);
 {$ELSE}
     p:=PByte(ABuffer);
     sz:=PInteger(p)^;
     inc(p,sizeof(integer));
     SetLength(Result,sz div Sizeof(WideChar));
 {$ENDIF}
     kbmMemMove(p^,Pointer(Result)^,sz);
end;

// Put WideString into a buffer.
procedure WideStringToBuffer(AWideString:WideString; ABuffer:pointer);
var
   sz:integer;
   p:PByte;
   l:integer;
begin
     l:=length(AWideString);
     if l=0 then
        sz:=0
     else
         sz:=(l+1)*sizeof(WideChar); // Includes ending 00.
     p:=PByte(ABuffer);

     kbmMemMove(Pointer(AWideString)^,p^,sz);
end;
{$ENDIF}

// Put contents of a stream into a variant.
function StreamToVariant(stream:TStream):variant;
var
   p:PByte;
begin
     stream.Seek(0,soBeginning);
     Result:=VarArrayCreate([0,stream.Size - 1],VarByte);
     try
        p:=VarArrayLock(Result);
        try
           stream.ReadBuffer(p^,stream.Size);
        finally
           VarArrayUnlock(Result);
        end;
     except
        Result:=Unassigned;
     end;
end;

// Get contents of a variant and put it in a stream.
procedure VariantToStream(AVariant:variant; stream:TStream);
var
   p:PByte;
   sz:integer;
begin
     // Check if variant contains data and is an array.
     if VarIsEmpty(AVariant) or VarIsNull(AVariant) or (not VarIsArray(AVariant)) then exit;

     sz:=VarArrayHighBound(AVariant,1);
     p:=VarArrayLock(AVariant);
     try
        stream.WriteBuffer(p^,sz+1);
     finally
        VarArrayUnlock(AVariant);
     end;
end;

procedure CopyFieldDefs(Source:TFieldDefs; Dest:TFieldDefs);
var
   i:TkbmNativeInt;
begin
//OutputDebugString(PChar('CopyFieldDefs: Source='+Source.DataSet.Name+' Dest='+Dest.DataSet.Name));
     Dest.Clear;
     for i:=0 to Source.Count-1 do
         Dest.Add(Source[i].Name,Source[i].DataType,Source[i].Size,Source[i].Required);
     Dest.Updated:=true;
end;

{$IFDEF KBMMEMTABLE_SUPPORT_NESTEDFIELDS}
// Hack/circumvention to get the flat fielddef list updated correctly without incurring
// another refresh of the already filled out fielddefs.
type
   THackFlatList = class(TFlatList);

procedure UpdateFieldDefList(FieldDefs:TFieldDefs; FieldDefList:TFieldDefList);
  procedure AddFieldDefs(const AParentName:string; const AFieldDefs:TFieldDefs);
  var
     i,j:TkbmNativeInt;
     sFName,sCName:string;
     fd,cd:TFieldDef;
  begin
       for i:=0 to AFieldDefs.Count-1 do
       begin
            fd:=FieldDefs[i];
            sFName:=AParentName+fd.Name;
            FieldDefList.AddObject(sFName,fd);

            if fd.HasChildDefs then
            begin
                 if fd.DataType=ftArray then
                 begin
                      cd:=fd.ChildDefs[0];
                      for j:=0 to fd.Size-1 do
                      begin
                           sCName:=sFName+'['+inttostr(j)+']';
                           FieldDefList.AddObject(sCName,cd);
                           if cd.DataType=ftADT then
                              AddFieldDefs(sCName+'.',cd.ChildDefs);
                      end;
                 end
                 else if faUnNamed in fd.Attributes then
                      AddFieldDefs('',fd.ChildDefs)
                 else
                     AddFieldDefs(AParentName+fd.Name+'.',fd.ChildDefs);
            end;
       end;
  end;
var
   fdl:THackFlatList;
begin
     fdl:=THackFlatList(FieldDefList);
     fdl.Locked:=false;
     try
        fdl.BeginUpdate;
        fdl.Clear;
        AddFieldDefs('',FieldDefs);
        fdl.Updated:=true;
     finally
        fdl.EndUpdate;
        fdl.Locked:=true;
     end;
end;
{$ENDIF}


// Compare two fields.
 {$IFDEF FPC}
  const CSTR_EQUAL = 2;
 {$ENDIF}

function CompareFields(const KeyField,AField:pointer; const FieldType: TFieldType; const LocaleID:TkbmLocaleID; const IndexFieldOptions:TkbmifoOptions; var FullCompare:boolean):Integer;
var
{$IFDEF NEXTGEN}
   p:PChar;
   w1,w2:string;
{$ELSE}
   p:PAnsiChar;
   w1,w2:WideString;
{$ENDIF}
   d:Double;
   l,l1:integer;
   li1,li2:TkbmLongInt;
   cur1,cur2:Currency;
   tssql1,tssql2:TSQLTimeStamp;
 {$IFDEF KBMMEMTABLE_SUPPORT_FT_LONGWORD}
   lw1,lw2:LongWord;
 {$ENDIF}
 {$IFDEF MSWINDOWS}
   locale:TkbmLocaleID;
   c:integer;
 {$ENDIF}
begin
     case FieldType of
{$IFDEF KBMMEMTABLE_SUPPORT_FT_BYTE}
       DB.ftByte,
{$ENDIF}
{$IFDEF KBMMEMTABLE_SUPPORT_FT_SHORTINT}
       DB.ftShortint,
{$ENDIF}
       DB.ftInteger,
       DB.ftAutoInc:
          begin
             li1:=PkbmLongint(KeyField)^;
             li2:=PkbmLongInt(AField)^;
//OutputDebugString(PChar('Comparefields i1='+IntToStr(li1)+', i2='+IntToStr(li2)));
             if li1=li2 then Result:=0
             else if li1<li2 then Result:=-1
             else Result:=1;
             FullCompare:=true;
//OutputDebugString(PChar(format('Key=%d, Rec=%d, Res=%d',[li1,li2,Result])));
          end;

 {$IFDEF KBMMEMTABLE_SUPPORT_FT_ORAINTERVAL}
       DB.ftOraInterval,
 {$ENDIF}
       DB.ftGUID,
       DB.ftFixedChar,
       DB.ftMemo,
       DB.ftString:
          begin
               p:=nil;
               try
                  // If partial, cut to reference length. p1=reference field value, p2=tried field value.
                  p:=AField;
                  FullCompare:=not (mtifoPartial in IndexFieldOptions);
                  if not FullCompare then
                  begin
{$IFDEF NEXTGEN}
                       l:=SysUtils.StrLen(PChar(KeyField));
{$ELSE}
                       l:={$IFDEF LEVEL18}AnsiStrings.{$ENDIF}StrLen(PAnsiChar(KeyField));
{$ENDIF}
                       l1:={$IFDEF NEXTGEN}SysUtils.{$ELSE}{$IFDEF LEVEL18}AnsiStrings.{$ENDIF}{$ENDIF}StrLen(p);
                       FullCompare:=(l=l1);
                       if not FullCompare then
                       begin
                            if l>l1 then l:=l1;
{$IFDEF NEXTGEN}
                            p:=SysUtils.StrAlloc(l+1);
{$ELSE}
                            p:={$IFDEF LEVEL18}AnsiStrings.{$ENDIF}{$IFDEF FPC}StrAlloc{$ELSE}AnsiStrAlloc{$ENDIF}(l+1);
{$ENDIF}
                            {$IFDEF NEXTGEN}SysUtils.{$ELSE}{$IFDEF LEVEL18}AnsiStrings.{$ENDIF}{$ENDIF}StrLCopy(p,AField,l);
                       end;
                  end;

//OutputDebugString(PChar('Comparefields keyfield='+PAnsiChar(KeyField)+', p='+p));
{$IFNDEF NEXTGEN}
                  if (mtifoIgnoreLocale in IndexFieldOptions) then
                  begin

                       if (mtifoCaseInsensitive in IndexFieldOptions) then
                          Result:=kbmPCompStrIC(PAnsiChar(KeyField),p)
                       else
                            Result:=kbmPCompStr(PAnsiChar(KeyField),p);

                  end
                  else
                  begin
{$ENDIF}

{$IFDEF NEXTGEN}
                       if (mtifoCaseInsensitive in IndexFieldOptions) then
                          Result:=CompareText(MarshaledString(KeyField),string(MarshaledAString(p)))
                       else
                          Result:=CompareStr(MarshaledString(KeyField),string(MarshaledAString(p)));
{$ELSE}
 {$IFDEF MACOS}
                       //if (mtifoCaseInsensitive in IndexFieldOptions) then
                          Result:=CompareText(PAnsiChar(KeyField),string(p));
                       //else
                         // Result:=CompareStr(PAnsiChar(KeyField),string(p));
 {$ELSE}

  {$IFDEF POSIX}
                       if (mtifoCaseInsensitive in IndexFieldOptions) then
                          Result:=CompareText(string(KeyField),string(p))
                       else
                          Result:=CompareStr(string(KeyField),string(p));
  {$ELSE}
   {$IFDEF MSWINDOWS}
                       c:=0;
                       if (mtifoCaseInsensitive in IndexFieldOptions) then
                          c:=c or NORM_IGNORECASE;
                       if (mtifoIgnoreNonSpace in IndexFieldOptions) then
                          c:=c or NORM_IGNORENONSPACE;
                       if (mtifoIgnoreSymbols in IndexFieldOptions) then
                          c:=c or NORM_IGNORESYMBOLS;
                       if (mtifoIgnoreKanatype in IndexFieldOptions) then
                          c:=c or NORM_IGNOREKANATYPE;
                       if (mtifoIgnoreWidth in IndexFieldOptions) then
                          c:=c or NORM_IGNOREWIDTH;
                       Result:=CompareStringA(LocaleID,c,PAnsiChar(KeyField),{$IFDEF LEVEL18}AnsiStrings.{$ENDIF}StrLen(PAnsiChar(KeyField)),p,{$IFDEF LEVEL18}AnsiStrings.{$ENDIF}StrLen(PAnsiChar(p)));
                       if Result=0 then
                          raise EMemTableInvalidLocale.Create(kbmInvalidLocale);
                       Result:=Result-CSTR_EQUAL;
   {$ELSE}
                       //if (mtifoCaseInsensitive in IndexFieldOptions) then
                          Result:=AnsiCompareText(string(PAnsiChar(KeyField)),string(p));
                       //else
                       //   Result:=AnsiCompareStr(string(PAnsiChar(KeyField)),string(p));
   {$ENDIF}
  {$ENDIF}
 {$ENDIF}
{$ENDIF}
                       if Result<=-1 then Result:=-1
                       else if Result>=1 then Result:=1
                       else Result:=0;
{$IFNDEF NEXTGEN}
                  end;
{$ENDIF}
               finally
                  if p<>AField then {$IFDEF NEXTGEN}SysUtils.{$ELSE}{$IFDEF LEVEL18}AnsiStrings.{$ENDIF}{$ENDIF}StrDispose(p);
               end;
          end;

 {$IFDEF KBMMEMTABLE_SUPPORT_FT_WIDEMEMO}
       DB.ftWideMemo,
 {$ENDIF}
  {$IFDEF KBMMEMTABLE_SUPPORT_FT_FIXEDWIDECHAR}
       DB.ftFixedWideChar,
 {$ENDIF}
       DB.ftWideString:
          begin
               // If partial, cut to reference length. p1=reference field value, p2=tried field value.
{$IFDEF NEXTGEN}
               w1:=StrPas(KeyField);
               w2:=StrPas(AField);
{$ELSE}
               w1:=WideStringFromBuffer(KeyField);
               w2:=WideStringFromBuffer(AField);
{$ENDIF}
// OutputDebugString(PChar('w1=>'+w1+'< w2=>'+w2+'<'));
               FullCompare:=not (mtifoPartial in IndexFieldOptions);

               if not FullCompare then
               begin
                    l:=Length(w2);
                    l1:=Length(w1);
                    FullCompare:=(l=l1);
                    if not FullCompare then
                    begin
                         if l>l1 then l:=l1;
                         w2:=copy(w2,1,l);
                    end;
               end;

{$IFDEF MSWINDOWS}
               if (mtifoIgnoreLocale in IndexFieldOptions) then
                  locale:=LOCALE_USER_DEFAULT
               else
                   locale:=LocaleID;

               c:=0;
               if (mtifoCaseInsensitive in IndexFieldOptions) then
                  c:=c or NORM_IGNORECASE;
               if (mtifoIgnoreNonSpace in IndexFieldOptions) then
                  c:=c or NORM_IGNORENONSPACE;
               if (mtifoIgnoreSymbols in IndexFieldOptions) then
                  c:=c or NORM_IGNORESYMBOLS;
               if (mtifoIgnoreKanatype in IndexFieldOptions) then
                  c:=c or NORM_IGNOREKANATYPE;
               if (mtifoIgnoreWidth in IndexFieldOptions) then
                  c:=c or NORM_IGNOREWIDTH;
               Result:=CompareStringW(locale,c,PWideChar(w1),Length(w1),PWideChar(w2),Length(w2));
               if Result=0 then
                  raise EMemTableInvalidLocale.Create(kbmInvalidLocale);
               Result:=Result-CSTR_EQUAL;
{$ELSE}
               if (mtifoIgnoreWidth in IndexFieldOptions) then
                  Result:=CompareText(string(w1),string(w2))
               else
                  Result:=CompareStr(string(w1),string(w2));
{$ENDIF}
               if Result<=-1 then Result:=-1
               else if Result>=1 then Result:=1
               else Result:=0;
          end;

{$IFDEF KBMMWMTABLE_SUPPORT_FT_SINGLE}
       DB.ftSingle,
{$ENDIF}
       DB.ftFloat,
       DB.ftCurrency:
          begin
               if PDouble(KeyField)^=PDouble(AField)^ then
                  Result:=0
               else if PDouble(KeyField)^<PDouble(AField)^ then
                  Result:=-1
               else
                  Result := 1;
               FullCompare:=true;
          end;

{$IFDEF KBMMEMTABLE_SUPPORT_FT_LONGWORD}
       DB.ftLongWord:
          begin
             lw1:=PLongWord(KeyField)^;
             lw2:=PLongWord(AField)^;
             if lw1=lw2 then Result:=0
             else if lw1<lw2 then Result:=-1
             else Result:=1;
             FullCompare:=true;
          end;
{$ENDIF}
{$IFDEF KBMMEMTABLE_SUPPORT_FT_EXTENDED}
       DB.ftExtended:
          begin
               if PExtended(KeyField)^=PExtended(AField)^ then
                  Result:=0
               else if PExtended(KeyField)^<PExtended(AField)^ then
                  Result:=-1
               else
                  Result := 1;
               FullCompare:=true;
          end;
{$ENDIF}

       DB.ftSmallint:
          begin
               if PSmallInt(KeyField)^=PSmallInt(AField)^ then
                  Result:=0
               else if PSmallInt(KeyField)^<PSmallInt(AField)^ then
                  Result:=-1
               else
                  Result:=1;
               FullCompare:=true;
          end;

       DB.ftLargeInt:
          begin
               if PInt64(KeyField)^=PInt64(AField)^ then
                  Result:=0
               else if PInt64(KeyField)^<PInt64(AField)^ then
                  Result:=-1
               else
                  Result:=1;
               FullCompare:=true;
          end;

       DB.ftDate:
          begin
               if PLongint(KeyField)^=PLongint(AField)^ then
                  Result:=0
               else if PLongint(KeyField)^<PLongint(AField)^ then
                  Result:=-1
               else
                  Result:=1;
               FullCompare:=true;
          end;

       DB.ftTime:
          begin
               if PLongint(KeyField)^=PLongint(AField)^ then
                  Result:=0
               else if PLongint(KeyField)^<PLongint(AField)^ then
                  Result:=-1
               else
                  Result:=1;
               FullCompare:=true;
          end;

       DB.ftDateTime:
          begin
               d:=PDateTimeRec(KeyField)^.DateTime-PDateTimeRec(AField)^.DateTime;
               if d<0.0 then
                  Result:=-1
               else if d>0.0 then
                  Result:=1
               else
                  Result:=0;
               FullCompare:=true;
          end;

       DB.ftWord:
          begin
               if PWord(KeyField)^=PWord(AField)^ then
                  Result:=0
               else if PWord(KeyField)^<PWord(AField)^ then
                  Result:=-1
               else
                  Result:=1;
               FullCompare:=true;
          end;

       DB.ftBoolean:
          begin
               if PWordBool(KeyField)^=PWordBool(AField)^ then
                  Result:=0
               else if PWordBool(KeyField)^<PWordBool(AField)^ then
                  Result:=-1
               else
                  Result:=1;
               FullCompare:=true;
          end;

 {$IFDEF KBMMEMTABLE_SUPPORT_FT_ORATIMESTAMP}
       DB.ftOraTimeStamp,
 {$ENDIF}
       DB.ftTimeStamp:
          begin
               tssql1:=PSQLTimeStamp(KeyField)^;
               tssql2:=PSQLTimeStamp(AField)^;
               Result:=tssql1.Year-tssql2.Year;
               if Result=0 then
                  Result:=tssql1.Month-tssql2.Month;
               if Result=0 then
                  Result:=tssql1.Day-tssql2.Day;
               if Result=0 then
                  Result:=tssql1.Hour-tssql2.Hour;
               if Result=0 then
                  Result:=tssql1.Hour-tssql2.Hour;
               if Result=0 then
                  Result:=tssql1.Minute-tssql2.Minute;
               if Result=0 then
                  Result:=tssql1.Second-tssql2.Second;
               if Result=0 then
                  Result:=tssql1.Fractions-tssql2.Fractions;
               if Result<0 then Result:=-1
               else if Result>0 then Result:=1;
               FullCompare:=true;
          end;

       DB.ftBCD,
       DB.ftFmtBCD:
          begin
               Result:=0;
               if BcdToCurr(Pbcd(keyfield)^,cur1) and BcdToCurr(pbcd(afield)^,cur2) then
               begin
                    if cur1<cur2 then Result:=-1
                    else if cur1>cur2 then Result:=1;
               end;
               FullCompare:=true;
          end;
     else
         Result:=0;
     end;

     if (mtifoDescending in IndexFieldOptions) then Result:=-Result;
//OutputDebugString(PChar('   Result='+IntToStr(Result)));

end;

function IndexOptions2CompareOptions(AOptions:TIndexOptions):TkbmMemTableCompareOptions;
begin
     Result:=[];
     if ixUnique in AOptions then Result:=Result + [mtcoUnique];
     if ixDescending in AOptions then Result:=Result + [mtcoDescending];
     if ixCaseInsensitive in AOptions then Result:=Result + [mtcoCaseInsensitive];
     if ixNonMaintained in AOptions then Result:=Result + [mtcoNonMaintained];
end;

function CompareOptions2IndexOptions(AOptions:TkbmMemTableCompareOptions):TIndexOptions;
begin
     Result:=[];
     if mtcoUnique in AOptions then Result:=Result + [ixUnique];
     if mtcoDescending in AOptions then Result:=Result + [ixDescending];
     if mtcoCaseInsensitive in AOptions then Result:=Result + [ixCaseInsensitive];
     if mtcoNonMaintained in AOptions then Result:=Result + [ixNonMaintained];
end;

// -----------------------------------------------------------------------------------
// TkbmExprParser
// -----------------------------------------------------------------------------------

{$IFDEF KBMMEMTABLE_SUPPORT_EXPRESSIONPARSER}
constructor TkbmExprParser.Create(DataSet: TkbmCustomMemTable; const Text: string;
                   Options: TFilterOptions);
begin
     inherited Create(DataSet,Text,Options,[poExtSyntax],'',nil,FldTypeMap);
     FDataset:=DataSet;
end;

// Parse build filter.
function TkbmExprParser.Evaluate:boolean;

  function VIsNull(AVariant:Variant):Boolean;
  begin
       Result:=VarIsNull(AVariant) or VarIsEmpty(AVariant);
  end;
var
   //iVersion,iTotalSize,iNodes,iNodeStart:Word;
   iLiteralStart:Word;
   format:TFormatSettings;

{$IFDEF KBMMEMTABLE_SUPPORT_FT_UNICODE}
 {$DEFINE KBMMEMTABLE_SUPPORT_FT_1007}
{$ENDIF}
{$IFDEF NEXTGEN}
 {$DEFINE KBMMEMTABLE_SUPPORT_FT_1007}
{$ENDIF}

{$IFDEF KBMMEMTABLE_SUPPORT_FT_1007}
   function GetUnicodeString(pft:PByte):UnicodeString;
   var
      len:word;
      pWords:PWord;
      pR:Pointer;
   begin
        pWords:=PWord(pft);
        len:=pWords^ div 2;
        inc(pWords);
        SetLength(Result,len);
{$IFDEF NEXTGEN}
        pR:=pointer(@Result[0]);
{$ELSE}
        pR:=pointer(@Result[1]);
{$ENDIF}
        kbmMemMove(pWords^,pR^,len * 2);
   end;
{$ENDIF}

   function ParseNode(pfdStart,pfd:PByte):variant;
   var
      b:WordBool;
      i,z:TkbmNativeInt;
      year,mon,day,hour,min,sec,msec:word;

      iClass:NODEClass;
      iOperator:TCANOperator;
      pArg1,pArg2:PByte;
      sFunc,sArg1,sArg2:string;
      Arg1,Arg2:variant;

      //     FieldNo:integer;
      FieldName:String;
      DataType:word;
      DataOfs:integer;
//      DataSize:integer;

      ts:TTimeStamp;
      dt:TDateTime;
      cdt:Comp;
      bcd:TBCD;
      cur:Currency;

      PartLength:word;
      IgnoreCase:word;
      S1,S2:string;
   type
      PDouble=^Double;
      PTimeStamp=^TTimeStamp;
      PComp=^Comp;
      PWordBool=^WordBool;
      PBCD=^TBCD;
   begin
        // Get node class.
        iClass:=NODEClass(PInteger(@pfd[0])^);
        iOperator:=TCANOperator(PInteger(@pfd[4])^);
        inc(pfd,CANHDRSIZE);

        //ShowMessage(Format('Class=%d, Operator=%d',[ord(iClass),ord(iOperator)]));

        // Check class.
        case iClass of
            nodeFIELD:
               begin
                    case iOperator of
                         coFIELD2:
                           begin
//                                FieldNo:=PWord(@pfd[0])^ - 1;
                                DataOfs:=iLiteralStart+PWord(@pfd[2])^;
                                pArg1:=pfdStart;
                                inc(pArg1,DataOfs);
{$IFDEF NEXTGEN}
                                FieldName:=TMarshal.ReadStringAsUtf8(TPtrWrapper.Create(pArg1));
{$ELSE}
                                FieldName:=string(PAnsiChar(pArg1));
{$ENDIF}
                                Result:=FDataset.FieldByName(FieldName).Value;
                           end;
                         else
                             raise EMemTableFilterError.CreateFmt(kbmUnknownOperator,[ord(iOperator)]);
                    end;
               end;

            nodeCONST:
               begin
                    case iOperator of
                         coCONST2:
                           begin
                                DataType:=PWord(@pfd[0])^;

//                                DataSize:=PWord(@pfd[2])^;
                                DataOfs:=iLiteralStart+PWord(@pfd[4])^;
                                pArg1:=pfdStart;
                                inc(pArg1,DataOfs);

                                // Check type.
                                case DataType of
                                     ord(DB.ftSmallInt): Result:=PSmallInt(pArg1)^;
                                     ord(DB.ftWord): Result:=PWord(pArg1)^;
{$IFDEF KBMMEMTABLE_SUPPORT_FT_SHORTINT}
                                     ord(DB.ftShortint): Result:=PShortInt(pArg1)^;
{$ENDIF}
{$IFDEF KBMMEMTABLE_SUPPORT_FT_BYTE}
                                     ord(DB.ftByte): Result:=PByte(pArg1)^;
{$ENDIF}
                                     ord(DB.ftInteger),
                                     ord(DB.ftAutoInc):  Result:=PInteger(pArg1)^;

                                     ord(DB.ftLargeInt): Result:=PInt64(pArg1)^;
{$IFDEF KBMMWMTABLE_SUPPORT_FT_SINGLE}
                                     ord(DB.ftSingle),
{$ENDIF}
                                     ord(DB.ftFloat), ord(ftCurrency): Result:=PDouble(pArg1)^;
{$IFDEF KBMMEMTABLE_SUPPORT_FT_FIXEDWIDECHAR}
                                     ord(DB.ftFixedWideChar),
{$ENDIF}
                                     ord(DB.ftGUID),
                                     ord(DB.ftWideString):
{$IFDEF NEXTGEN}
                                        Result:=PString(pArg1)^;
{$ELSE}
                                        Result:=PWideString(pArg1)^;
{$ENDIF}
{$IFDEF KBMMEMTABLE_SUPPORT_FT_ORAINTERVAL}
                                     ord(DB.ftOraInterval),
{$ENDIF}
                                     ord(DB.ftString),
                                     ord(DB.ftFixedChar):
{$IFDEF NEXTGEN}
                                        Result:=PString(pArg1)^;
{$ELSE}
                                        Result:=String(AnsiString(PAnsiChar(pArg1)));
{$ENDIF}
                                     ord(DB.ftDate):
                                       begin
                                            ts.Date:=PInteger(pArg1)^;
                                            ts.Time:=0;
                                            dt:=TimeStampToDateTime(ts);
                                            Result:=dt;
                                       end;
                                     ord(DB.ftTime):
                                       begin
                                            ts.Date:=0;
                                            ts.Time:=PInteger(pArg1)^;;
                                            dt:=TimeStampToDateTime(ts);
                                            Result:=dt;
                                       end;
                                     ord(DB.ftDateTime):
                                       begin
                                            cdt:=PDouble(pArg1)^;
                                            ts:=MSecsToTimeStamp(cdt);
                                            dt:=TimeStampToDateTime(ts);
                                            Result:=dt;
                                       end;
                                     ord(DB.ftBoolean): Result:=PWordBool(pArg1)^;
    {$IFDEF KBMMEMTABLE_SUPPORT_FT_ORATIMESTAMP}
                                     ord(DB.ftOraTimeStamp),
  {$ENDIF}
                                     ord(DB.ftTimeStamp): Result:=VarSQLTimeStampCreate(PSQLTimeStamp(pArg1)^);
                                     ord(DB.ftBCD),
                                     ord(DB.ftFmtBCD):
                                       begin
                                            bcd:=PBCD(pArg1)^;
                                            BCDToCurr(bcd,Cur);
                                            Result:=Cur;
                                       end;

{$IFDEF KBMMEMTABLE_SUPPORT_FT_LONGWORD}
                                     ord(DB.ftLongWord): Result:=PLongWord(pArg1)^;
{$ENDIF}
{$IFDEF KBMMEMTABLE_SUPPORT_FT_EXTENDED}
                                     ord(DB.ftExtended): Result:=PExtended(pArg1)^;
{$ENDIF}
{$IFDEF KBMMEMTABLE_SUPPORT_FT_1007}
                                     $1007:                       // Midas Unicode.
                                          Result:=GetUnicodeString(PByte(pArg1));
{$ENDIF}
                                     else
                                         raise EMemTableFilterError.CreateFmt(kbmUnknownFieldType,[DataType]);
                                end;
                           end;
                    end;
               end;

            nodeUNARY:
               begin
                    pArg1:=pfdStart;
                    inc(pArg1,CANEXPRSIZE+PWord(@pfd[0])^);

                    case iOperator of
                         coISBLANK,coNOTBLANK:
                           begin
                                Arg1:=ParseNode(pfdStart,pArg1);
                                b:=VIsNull(Arg1);
                                if iOperator=coNOTBLANK then b:=not b;
                                Result:=Variant(b);
                           end;

                         coNOT:
                           begin
                                Arg1:=ParseNode(pfdStart,pArg1);
                                if VIsNull(Arg1) then
                                   Result:=Null
                                else
                                   Result:=Variant(not Arg1);
                           end;

                         coMINUS:
                           begin
                                Arg1:=ParseNode(pfdStart,pArg1);
                                if not VIsNull(Arg1) then
                                   Result:=-Arg1
                                else
                                    Result:=Null;
                           end;

                         coUPPER:
                           begin
                                Arg1:=ParseNode(pfdStart,pArg1);
                                if not VIsNull(Arg1) then
                                   Result:=UpperCase(Arg1)
                                else
                                    Result:=Null;
                           end;

                         coLOWER:
                           begin
                                Arg1:=ParseNode(pfdStart,pArg1);
                                if not VIsNull(Arg1) then
                                   Result:=LowerCase(Arg1)
                                else
                                    Result:=Null;
                           end;
                    end;
               end;

            nodeBINARY:
               begin
                    // Get Loper and Roper pointers to buffer.
                    pArg1:=pfdStart;
                    inc(pArg1,CANEXPRSIZE+PWord(@pfd[0])^);
                    pArg2:=pfdStart;
                    inc(pArg2,CANEXPRSIZE+PWord(@pfd[2])^);

                    // Check operator for what to do.
                    case iOperator of
                         coEQ:
                           begin
                                Arg1:=ParseNode(pfdStart,pArg1);
                                Arg2:=ParseNode(pfdStart,pArg2);
                                if VIsNull(Arg1) or VIsNull(Arg2) then b:=false
                                else b:=(Arg1 = Arg2);
                                Result:=Variant(b);
                                exit;
                           end;

                         coNE:
                           begin
                                Arg1:=ParseNode(pfdStart,pArg1);
                                Arg2:=ParseNode(pfdStart,pArg2);
                                if VIsNull(Arg1) or VIsNull(Arg2) then b:=false
                                else b:=(Arg1 <> Arg2);
                                Result:=Variant(b);
                                exit;
                           end;

                         coGT:
                           begin
                                Arg1:=ParseNode(pfdStart,pArg1);
                                Arg2:=ParseNode(pfdStart,pArg2);
                                if VIsNull(Arg1) or VIsNull(Arg2) then b:=false
                                else b:=(Arg1 > Arg2);
                                Result:=Variant(b);
                                exit;
                           end;

                         coGE:
                           begin
                                Arg1:=ParseNode(pfdStart,pArg1);
                                Arg2:=ParseNode(pfdStart,pArg2);
                                if VIsNull(Arg1) or VIsNull(Arg2) then b:=false
                                else b:=(Arg1 >= Arg2);
                                Result:=Variant(b);
                                exit;
                           end;

                         coLT:
                           begin
                                Arg1:=ParseNode(pfdStart,pArg1);
                                Arg2:=ParseNode(pfdStart,pArg2);
                                if VIsNull(Arg1) or VIsNull(Arg2) then b:=false
                                else b:=(Arg1 < Arg2);
                                Result:=Variant(b);
                                exit;
                           end;

                         coLE:
                           begin
                                Arg1:=ParseNode(pfdStart,pArg1);
                                Arg2:=ParseNode(pfdStart,pArg2);
                                if VIsNull(Arg1) or VIsNull(Arg2) then b:=false
                                else b:=(Arg1 <= Arg2);
                                Result:=Variant(b);
                                exit;
                           end;

                         coOR:
                           begin
                                Arg1:=ParseNode(pfdStart,pArg1);
                                Arg2:=ParseNode(pfdStart,pArg2);
                                if VIsNull(Arg1) or VIsNull(Arg2) then b:=false
                                else b:=(Arg1 or Arg2);
                                Result:=Variant(b);
                                exit;
                           end;

                         coAND:
                           begin
                                Arg1:=ParseNode(pfdStart,pArg1);
                                Arg2:=ParseNode(pfdStart,pArg2);
                                if VIsNull(Arg1) or VIsNull(Arg2) then b:=false
                                else b:=(Arg1 and Arg2);
                                Result:=Variant(b);
                                exit;
                           end;

                         coADD:
                           begin
                                Arg1:=ParseNode(pfdStart,pArg1);
                                Arg2:=ParseNode(pfdStart,pArg2);
                                if VIsNull(Arg1) or VIsNull(Arg2) then Result:=Null
                                else Result:=(Arg1 + Arg2);
                                exit;
                           end;

                         coSUB:
                           begin
                                Arg1:=ParseNode(pfdStart,pArg1);
                                Arg2:=ParseNode(pfdStart,pArg2);
                                if VIsNull(Arg1) or VIsNull(Arg2) then Result:=Null
                                else Result:=(Arg1 - Arg2);
                                exit;
                           end;

                         coMUL:
                           begin
                                Arg1:=ParseNode(pfdStart,pArg1);
                                Arg2:=ParseNode(pfdStart,pArg2);
                                if VIsNull(Arg1) or VIsNull(Arg2) then Result:=Null
                                else Result:=(Arg1 * Arg2);
                                exit;
                           end;

                         coDIV:
                           begin
                                Arg1:=ParseNode(pfdStart,pArg1);
                                Arg2:=ParseNode(pfdStart,pArg2);
                                if VIsNull(Arg1) or VIsNull(Arg2) then Result:=Null
                                else Result:=(Arg1 / Arg2);
                                exit;
                           end;

                         coMOD,coREM:
                           begin
                                Arg1:=ParseNode(pfdStart,pArg1);
                                Arg2:=ParseNode(pfdStart,pArg2);
                                if VIsNull(Arg1) or VIsNull(Arg2) then Result:=Null
                                else Result:=(Arg1 mod Arg2);
                                exit;
                           end;

                         coIN:
                           begin
                                Arg1:=ParseNode(PfdStart,pArg1);
                                Arg2:=ParseNode(PfdStart,pArg2);
                                if VIsNull(Arg1) or VIsNull(Arg2) then
                                begin
                                     Result:=false;
                                     exit;
                                end;

                                if VarIsArray(Arg2) then
                                begin
                                     b:=false;
                                     for i:=0 to VarArrayHighBound(Arg2,1) do
                                     begin
                                          if VarIsEmpty(Arg2[i]) then break;
                                          b:=(Arg1=Arg2[i]);
                                          if b then break;
                                     end;
                                end
                                else
                                    b:=(Arg1=Arg2);
                                Result:=Variant(b);
                                exit;
                           end;

                         coLike:
                           begin
                                Arg1:=ParseNode(PfdStart,pArg1);
                                Arg2:=ParseNode(PfdStart,pArg2);
                                if VIsNull(Arg1) or VIsNull(Arg2) then
                                begin
                                     Result:=false;
                                     exit;
                                end;
                                pArg1:=PByte(PChar(VarToStr(ParseNode(pfdStart,pArg1))));
                                pArg2:=PByte(PChar(VarToStr(ParseNode(pfdStart,pArg2))));
                                b:=MatchesMask(string(PChar(pArg1)),string(PChar(pArg2)));
                                Result:=Variant(b);
                                exit;
                           end;

                         else
                             raise EMemTableFilterError.CreateFmt(kbmOperatorNotSupported,[ord(iOperator)]);
                    end;
               end;

            nodeCOMPARE:
               begin
                    IgnoreCase:=PWord(@pfd[0])^;
                    PartLength:=PWord(@pfd[2])^;
                    pArg1:=pfdStart+CANEXPRSIZE+PWord(@pfd[4])^;
                    pArg2:=pfdStart+CANEXPRSIZE+PWord(@pfd[6])^;
                    Arg1:=ParseNode(pfdStart,pArg1);
                    Arg2:=ParseNode(pfdStart,pArg2);
                    if VIsNull(Arg1) or VIsNull(Arg2) then
                    begin
                         Result:=false;
                         exit;
                    end;

                    S1:=Arg1;
                    S2:=Arg2;
                    if IgnoreCase=1 then
                    begin
                         S1:=AnsiUpperCase(S1);
                         S2:=AnsiUpperCase(S2);
                    end;
                    if PartLength>0 then
                    begin
                         S1:=Copy(S1,1,PartLength);
                         S2:=Copy(S2,1,PartLength);
                    end;

                    case iOperator of
                         coEQ:
                            begin
                                 b:=(S1 = S2);
                                 Result:=Variant(b);
                                 exit;
                            end;

                         coNE:
                            begin
                                 b:=(S1 <> S2);
                                 Result:=Variant(b);
                                 exit;
                            end;

                         coLIKE:
                            begin
                                 pArg1:=PByte(PChar(VarToStr(ParseNode(pfdStart,pArg1))));
                                 pArg2:=PByte(PChar(VarToStr(ParseNode(pfdStart,pArg2))));
                                 b:=MatchesMask(string(PChar(pArg1)),string(PChar(pArg2)));
                                 Result:=Variant(b);
                                 exit;
                            end;

                         else
                             raise EMemTableFilterError.CreateFmt(kbmOperatorNotSupported,[ord(iOperator)]);
                    end;
               end;

            nodeFUNC:
               begin
                    case iOperator of
                         coFUNC2:
                            begin
                                 pArg1:=pfdStart;
                                 inc(pArg1,iLiteralStart+PWord(@pfd[0])^);
{$IFDEF NEXTGEN}
                                 sFunc:=UpperCase(string(pArg1));  // Function name
{$ELSE}
                                 sFunc:=AnsiUpperCase(string(PAnsiChar(pArg1)));  // Function name
{$ENDIF}
                                 pArg2:=pfdStart;
                                 inc(pArg2,CANEXPRSIZE+PWord(@pfd[2])^); // Pointer to Value or Const

                                 if sFunc='UPPER' then
                                 begin
                                      Arg2:=ParseNode(pfdStart,pArg2);
                                      if VIsNull(Arg2) then Result:=Null
                                      else Result:=UpperCase(VarToStr(Arg2));
                                 end

                                 else if sFunc='LOWER' then
                                 begin
                                      Arg2:=ParseNode(pfdStart,pArg2);
                                      if VIsNull(Arg2) then Result:=Null
                                      else Result:=LowerCase(VarToStr(Arg2));
                                 end

                                 else if sFunc='SUBSTRING' then
                                 begin
                                      Arg2:=ParseNode(pfdStart,pArg2);
                                      if VIsNull(Arg2) then
                                      begin
                                           Result:=Null;
                                           exit;
                                      end;

                                      Result:=Arg2;
                                      try
{$IFDEF NEXTGEN}
                                         pArg1:=PByte(VarToStr(Result[0]));
{$ELSE}
                                         pArg1:=PByte(AnsiString(VarToStr(Result[0])));
{$ENDIF}
                                      except
                                         on EVariantError do // no Params for "SubString"
                                            raise EMemTableFilterError.CreateFmt(kbmInvMissParam,[pArg1]);
                                      end;

                                      i:=Result[1];
                                      z:=Result[2];
                                      if (z=0) then
                                      begin
                                           if (Pos(',',Result[1])>0) then  // "From" and "To" entered without space!
                                              z:=StrToInt(Copy(Result[1],Pos(',',Result[1])+1,Length(Result[1])))
                                           else                            // No "To" entered so use all
                                              z:=Length(PChar(pArg1));
                                      end;
                                      Result:=Copy(PChar(pArg1),i,z);
                                 end

                                 else if sFunc='TRIM' then
                                 begin
                                      Arg2:=ParseNode(pfdStart,pArg2);
                                      if VIsNull(Arg2) then Result:=Null
                                      else Result:=Trim(VarToStr(Arg2));
                                 end

                                 else if sFunc='TRIMLEFT' then
                                 begin
                                      Arg2:=ParseNode(pfdStart,pArg2);
                                      if VIsNull(Arg2) then Result:=Null
                                      else Result:=TrimLeft(VarToStr(Arg2));
                                 end

                                 else if sFunc='TRIMRIGHT' then
                                 begin
                                      Arg2:=ParseNode(pfdStart,pArg2);
                                      if VIsNull(Arg2) then Result:=Null
                                      else Result:=TrimRight(VarToStr(Arg2));
                                 end

                                 else if sFunc='GETDATE' then
                                    Result:=Now

                                 else if sFunc='YEAR' then
                                 begin
                                      Arg2:=ParseNode(pfdStart,pArg2);
                                      if VIsNull(Arg2) then Result:=Null
                                      else
                                      begin
                                           DecodeDate(VarToDateTime(Arg2),year,mon,day);
                                           Result:=year;
                                      end;
                                 end


                                 else if sFunc='MONTH' then
                                 begin
                                      Arg2:=ParseNode(pfdStart,pArg2);
                                      if VIsNull(Arg2) then Result:=Null
                                      else
                                      begin
                                           DecodeDate(VarToDateTime(Arg2),year,mon,day);
                                           Result:=mon;
                                      end;
                                 end

                                 else if sFunc='DAY' then
                                 begin
                                      Arg2:=ParseNode(pfdStart,pArg2);
                                      if VIsNull(Arg2) then Result:=Null
                                      else
                                      begin
                                           DecodeDate(VarToDateTime(Arg2),year,mon,day);
                                           Result:=day;
                                      end;
                                 end

                                 else if sFunc='HOUR' then
                                 begin
                                      Arg2:=ParseNode(pfdStart,pArg2);
                                      if VIsNull(Arg2) then Result:=Null
                                      else
                                      begin
                                           DecodeTime(VarToDateTime(Arg2),hour,min,sec,msec);
                                           Result:=hour;
                                      end;
                                 end

                                 else if sFunc='MINUTE' then
                                 begin
                                      Arg2:=ParseNode(pfdStart,pArg2);
                                      if VIsNull(Arg2) then Result:=Null
                                      else
                                      begin
                                           DecodeTime(VarToDateTime(Arg2),hour,min,sec,msec);
                                           Result:=min;
                                      end;
                                 end

                                 else if sFunc='SECOND' then
                                 begin
                                      Arg2:=ParseNode(pfdStart,pArg2);
                                      if VIsNull(Arg2) then Result:=Null
                                      else
                                      begin
                                           DecodeTime(VarToDateTime(Arg2),hour,min,sec,msec);
                                           Result:=sec;
                                      end;
                                 end

                                 else if sFunc='DATE' then  // Format DATE('datestring','formatstring')
                                 begin                      // or     DATE(datevalue)
                                      Result:=ParseNode(pfdStart,pArg2);
                                      if VarIsArray(Result) then
                                      begin
                                           try
                                              sArg1:=VarToStr(Result[0]);
                                              sArg2:=VarToStr(Result[1]);
                                           except
                                              on EVariantError do // no Params for DATE
                                                 raise EMemTableFilterError.CreateFmt(kbmInvMissParam,[sArg1]);
                                           end;

                                           format.ShortDateFormat:=sArg2;
                                           Result:=StrToDate(sArg1,format);
                                      end
                                      else
                                          Result:=longint(trunc(VarToDateTime(Result)));
                                 end

                                 else if sFunc='TIME' then  // Format TIME('timestring','formatstring')
                                 begin                      // or     TIME(datetimevalue)
                                      Result:=ParseNode(pfdStart,pArg2);
                                      if VarIsArray(Result) then
                                      begin
                                           try
                                              sArg1:=VarToStr(Result[0]);
                                              sArg2:=VarToStr(Result[1]);
                                           except
                                              on EVariantError do // no Params for TIME
                                                 raise EMemTableFilterError.CreateFmt(kbmInvMissParam,[sArg1]);
                                           end;

                                           format.ShortTimeFormat:=sArg2;
                                           Result:=StrToTime(sArg1,format);
                                      end
                                      else
                                          Result:=Frac(VarToDateTime(Result));
                                 end

                                 else
                                    raise EMemTableFilterError.CreateFmt(kbmInvFunction,[pArg1]);
                            end;
                         else
                            raise EMemTableFilterError.CreateFmt(kbmOperatorNotSupported,[ord(iOperator)]);
                    end;
               end;

            nodeLISTELEM:
               begin
                    case iOperator of
                         coLISTELEM2:
                            begin
                                 Result:=VarArrayCreate([0,50],VarVariant); // Create VarArray for ListElements Values
                                 i:=0;
                                 pArg1:=pfdStart;
                                 inc(pArg1,CANEXPRSIZE+PWord(@pfd[i*2])^);

                                 repeat
                                       Arg1:=ParseNode(PfdStart,parg1);
                                       if VarIsArray(Arg1) then
                                       begin
                                            z:=0;
                                            while not VarIsEmpty(Arg1[z]) do
                                            begin
                                                 Result[i+z]:=Arg1[z];
                                                 inc(z);
                                            end;
                                       end
                                       else
                                          Result[i]:=Arg1;

                                       inc(i);
                                       pArg1:=pfdStart;
                                       inc(pArg1,CANEXPRSIZE+PWord(@pfd[i*2])^);
                                 until NODEClass(PInteger(@pArg1[0])^)<>NodeListElem;

                                 // Only one or no Value so don't return as VarArray
                                 if i<2 then
                                 begin
                                      if VIsNull(Result[0]) then
                                         Result:=Null
                                      else
                                          Result:=VarAsType(Result[0],varString);
                                 end;
                            end;
                         else
                            raise EMemTableFilterError.CreateFmt(kbmOperatorNotSupported,[ord(iOperator)]);
                    end;
               end;
        else
            raise EMemTableFilterError.CreateFmt('iClass '+kbmOutOfRange,[ord(iClass)]);
        end;
   end;
  {$WARNINGS ON}

var
   pfdStart,pfd:PByte;
// iVersion,iTotalSize,iNodes,iNodeStart:integer;
begin
     pfdStart:=@FilterData[0];
     pfd:=pfdStart;

     // Get header.
     //     iVersion:=PWord(@pfd[0])^;
     //     iTotalSize:=PWord(@pfd[2])^;
     //     iNodes:=PWord(@pfd[4])^;
     //     iNodeStart:=PWord(@pfd[6])^;
     iLiteralStart:=PWord(@pfd[8])^;
     inc(pfd,10);

     // Show header.
     format:=FDataset.FFormatSettings;
{
     ShowMessage(Format('Version=%d, TotalSize=%d, Nodes=%d, NodeStart=%d, LiteralStart=%d',
        [iVersion,iTotalSize,iNodes,iNodeStart,iLiteralStart]));

     s:='';
     for i:=0 to FFilterParser.DataSize-1 do
     begin
          b:=FFilterParser.FilterData[i];
          if (b>=32) and (b<=127) then s1:=chr(b)
          else s1:=' ';
          s:=s+Format('%d=%0.2x%s ',[i,FFilterParser.FilterData[i],s1]);
     end;
     ShowMessage(s);
}
     Result:=WordBool(ParseNode(pfdStart,pfd));
end;

{$ELSE}
constructor TkbmExprParser.Create(DataSet: TkbmCustomMemTable; const Text: string);
var
   s:string;
   i:integer;
begin
     inherited Create;
     FDataset:=DataSet;
     FSQL:=TkbmSQLParser.Create(nil);
     with TkbmSQLParser(FSQL) do
     begin
          Init;
          s:=trim(Text);
          if Text='' then
             s:='true';
          SetSource('FILTER '+s);
          if not Execute then
             raise Exception.Create('Expression invalid.');
          if not (Operation is TkbmSQLEvaluationOperation) then
             raise Exception.Create('Expression invalid.');
          with TkbmSQLEvaluationOperation(Operation) do
          begin
               for i:=0 to RefSourceFields.Count-1 do
                   RefSourceFields.FieldNodes[i].OriginatingField:=FDataset.FieldByName(RefSourceFields.FieldNodes[i].FieldName);
               Optimize;
          end;
     end;
end;

destructor TkbmExprParser.Destroy;
begin
     TkbmSQLParser(FSQL).Free;
     inherited Destroy;
end;

function TkbmExprParser.Evaluate:boolean;
begin
     with TkbmSQLParser(FSQL) do
          Result:=TkbmSQLEvaluationOperation(Operation).Evaluate;
end;
{$ENDIF}

// -----------------------------------------------------------------------------------
// TkbmFieldList
// -----------------------------------------------------------------------------------
constructor TkbmFieldList.Create;
begin
     FCount:=0;
end;

destructor TkbmFieldList.Destroy;
begin
     inherited;
end;

// Build field list from list of fieldnames.
// fld1;fld2;fld3...
// Each field can contain options:
// fldname:options
// Options can be either C for Caseinsensitive or D for descending or a combination.
procedure TkbmFieldList.Build(ADataset:TkbmCustomMemTable; const AFieldNames:string; const AAggregateFieldNaming:boolean = false);
var
   p,p1:integer;
   fld:TField;
   s,sname,sopt:string;
   opt:TkbmifoOptions;
begin
     Clear;
     p:=1;
     while p<=length(AFieldNames) do
     begin
          // Extract fieldname and options from list of fields.
          s:=ExtractFieldName(AFieldNames,p);
          p1:=pos(':',s);
          Opt:=[];
          if p1<=0 then
             sname:=s
          else
          begin
               sname:=copy(s,1,p1-1);
               sopt:=uppercase(copy(s,p1+1,length(s)));
               opt:=ADataSet.GetAggregateFieldOption(sopt);
               if opt=[] then
                  opt:=ADataSet.GetExtractFieldOption(sopt);
               if opt=[] then
               begin
                    if pos('C',sopt)>0 then Include(opt,mtifoCaseInsensitive);
                    if pos('D',sopt)>0 then Include(opt,mtifoDescending);
                    if pos('N',sopt)>0 then Include(opt,mtifoIgnoreNull);
                    if pos('P',sopt)>0 then Include(opt,mtifoPartial);
                    if pos('L',sopt)>0 then Include(opt,mtifoIgnoreLocale);
                    if pos('S',sopt)>0 then Include(opt,mtifoIgnoreNonSpace);
                    if pos('K',sopt)>0 then Include(opt,mtifoIgnoreKanatype);
                    if pos('I',sopt)>0 then Include(opt,mtifoIgnoreSymbols);
                    if pos('W',sopt)>0 then Include(opt,mtifoIgnoreWidth);
               end
               else
                   if AAggregateFieldNaming then
                      sname:=ADataSet.GetAggregateFieldName(sname,opt);
          end;
          fld:=ADataset.FieldByName(sname);
          if (fld.FieldKind in [fkData,fkInternalCalc,fkCalculated,fkLookup]) and (fld.DataType in (kbmSupportedFieldTypes-kbmBinaryTypes)) then
             Add(ADataSet,fld,opt)
          else
              DatabaseErrorFmt(kbmIndexErr,[fld.DisplayName]);
          if fld.FieldKind=fkCalculated then
             ADataSet.FRecalcOnIndex:=true;
     end;

     FPrimaryCount:=Count;
end;

// Compare two field lists.
// Returns true if they are exactly equal, otherwise false.
function TkbmFieldList.IsEqualTo(AList:TkbmFieldList; const ASameCase:boolean; const AOnlyPrimary:boolean):boolean;
var
   i,j:TkbmNativeInt;
begin
     Result:=false;

     if AOnlyPrimary then
        j:=AList.FPrimaryCount
     else
         j:=AList.Count;
     if Count<j then exit;

     for i:=0 to j-1 do
         if (Fields[i]<>AList.Fields[i]) or
            (ASameCase and ((mtifoCaseInsensitive in Options[i])<>(mtifoCaseInsensitive in AList.Options[i]))) then exit;
     Result:=true;
end;

// Compare two field lists.
// Returns true if list2 is contained in list1, otherwise false.
function TkbmFieldList.StartsWith(AList:TkbmFieldList; const ASameCase:boolean; const AOnlyPrimary:boolean):boolean;
var
   i,j:TkbmNativeInt;
begin
     Result:=false;

     if AOnlyPrimary then
        j:=AList.FPrimaryCount
     else
         j:=AList.Count;
     if Count<j then exit;

     for i:=j downto 0 do
         if (Fields[i]<>AList.Fields[i]) or
            (ASameCase and ((mtifoCaseInsensitive in Options[i])<>(mtifoCaseInsensitive in AList.Options[i]))) then exit;
     Result:=true;
end;

// Find field from list.
function TkbmFieldList.FindField(const AFieldName:string):TField;
var
   fld:TField;
   i:TkbmNativeInt;
begin
     Result:=nil;
     for i:=0 to Count-1 do
     begin
          fld:=Fields[i];
          if fld.FieldName = AFieldName then
          begin
               Result:=fld;
               break;
          end;
     end;
end;

// Setup options for specific fields in the fieldlist.
procedure TkbmFieldList.SetOptions(ADataSet:TkbmCustomMemTable; AOptions:TkbmifoOption; AFieldNames:string);
var
   i,j:TkbmNativeInt;
   lst:TkbmFieldList;
   b:boolean;
begin
     // Set flags.
     lst:=TkbmFieldList.Create;
     try
        lst.Build(ADataSet,AFieldNames);
        for i:=0 to Count-1 do
        begin
             b:=false;
             for j:=0 to lst.count-1 do
                if lst.Fields[j]=Fields[i] then
                begin
                     b:=true;
                     break;
                end;

             if b then
                Include(Options[i],AOptions)
             else
                Exclude(Options[i],AOptions);
        end;
     finally
        lst.Free;
     end;
end;

function TkbmFieldList.Add(const ADataSet:TkbmCustomMemTable; const AField:TField; const AValue:TkbmifoOptions):Integer;
begin
     Result:=FCount;
     if length(Fields)<=FCount then
     begin
          SetLength(Fields,FCount+20);
          SetLength(Options,FCount+20);
          SetLength(FieldOfs,FCount+20);
          SetLength(FieldNo,FCount+20);
     end;

     Fields[FCount]:=AField;
     Options[FCount]:=AValue;
     FieldOfs[FCount]:=ADataSet.FCommon.GetFieldDataOffset(AField);
     FieldNo[FCount]:=AField.FieldNo;
     inc(FCount);
end;

procedure TkbmFieldList.Clear;
begin
     FCount:=0;
     SetLength(Fields,20);
     SetLength(Options,20);
     SetLength(FieldOfs,20);
     SetLength(FieldNo,20);
end;

function TkbmFieldList.IndexOf(Item:TField):integer;
var
   i:TkbmNativeInt;
begin
     for i:=0 to FCount-1 do
     begin
          if Fields[i]=Item then
          begin
               Result:=i;
               exit;
          end;
     end;
     Result:=-1;
end;

procedure TkbmFieldList.AssignTo(AFieldList:TkbmFieldList);
var
   i:TkbmNativeInt;
begin
     AFieldList.Clear;
     for i:=0 to Count-1 do
     begin
          AFieldList.Fields[i]:=Fields[i];
          AFieldList.Options[i]:=Options[i];
          AFieldList.FieldOfs[i]:=FieldOfs[i];
          AFieldList.FieldNo[i]:=FieldNo[i];
     end;
     AFieldList.FCount:=FCount;
     AFieldList.FPrimaryCount:=FPrimaryCount;
end;

procedure TkbmFieldList.MergeOptionsTo(AFieldList:TkbmFieldList);
var
   i:TkbmNativeInt;
   n:TkbmNativeInt;
begin
     n:=FCount;
     if n>AFieldList.FCount then n:=AFieldList.FCount;
     for i:=0 to n-1 do
         AFieldList.Options[i]:=AFieldList.Options[i] + Options[i];
end;

procedure TkbmFieldList.DefineAdditionalOrderFields(ADataSet:TkbmCustomMemTable; AFieldList:TkbmFieldList);
var
   i:integer;
   fld:TField;
   ifo:TkbmifoOptions;
begin
     for i:=0 to AFieldList.Count-1 do
     begin
          fld:=AFieldList.Fields[i];
          if IndexOf(fld)<0 then
          begin
               ifo:=AFieldList.Options[i];
               Add(ADataSet,fld,ifo);
          end;
     end;

end;

procedure TkbmFieldList.ClearOptions;
var
   i:TkbmNativeInt;
   n:TkbmNativeInt;
begin
     n:=Count;
     for i:=0 to n-1 do
         Options[i]:=[];
end;

function TkbmFieldList.GetAsString:string;
var
   i:TkbmNativeInt;
   s,a:string;
begin
     s:='';
     a:='';
     for i:=0 to FCount-1 do
     begin
          s:=s+a+Fields[i].FieldName;
          a:=';';
     end;
     Result:=s;
end;

// -----------------------------------------------------------------------------------
// TkbmCommon
// -----------------------------------------------------------------------------------
// Lowlevel record handling routines.
// Allocate space for a record structure.
function TkbmCommon._InternalAllocRecord:PkbmRecord;
begin
     GetMem(Result,FTotalRecordSize);
     Result^.Data:=PByte(Result)+Sizeof(TkbmRecord);
{$IFDEF DO_CHECKRECORD}
     Result^.StartIdent:=kbmRecordIdent;
     Result^.EndIdent:=kbmRecordIdent;
{$ENDIF}
     _InternalClearRecord(Result);
{$IFDEF DO_CHECKRECORD}
     _InternalCheckRecord(Result);
{$ENDIF}
end;

{$IFDEF DO_CHECKRECORD}
// Check record validity.
procedure TkbmCommon._InternalCheckRecord(ARecord:PkbmRecord);
begin
     // Check record identifier.
     if (ARecord^.StartIdent<>kbmRecordIdent) or (ARecord^.EndIdent<>kbmRecordIdent) then
        raise EMemTableInvalidRecord.Create(kbmInvalidRecord+inttostr(integer(ARecord)));
end;
{$ENDIF}

// Free var lengths in record.
procedure TkbmCommon._InternalFreeRecordVarLengths(ARecord:PkbmRecord);
var
   i:TkbmNativeInt;
   pVarLength:PPkbmVarLength;
   pField:PByte;
   fld:TField;
begin
     // Delete varlengths if any defined.
     if FVarLengthCount>0 then
     begin
          // Browse fields to delete varlengths.
          for i:=0 to FFieldCount-1 do
          begin
               fld:=FOwner.Fields[i];
               if (fld.FieldNo>0) and ((FFieldFlags[fld.FieldNo-1] and kbmffIndirect)<>0) then
               begin
                    pField:=GetFieldPointer(ARecord,fld);
                    pVarLength:=PPkbmVarLength(pField+1);
                    if (pVarLength^<>nil) then
                    begin
                         FreeVarLength(pVarLength^);
                         pVarLength^:=nil;
                         pField^:=kbmffNull;
                    end;
               end;
          end;
     end;
end;

// Transfer temporary buffer record to storage record.
procedure TkbmCommon._InternalTransferRecord(SourceRecord,DestRecord:PkbmRecord);
var
   i:TkbmNativeInt;
   pFieldSrc,pFieldDest:PByte;
   pVarLengthSrc,pVarLengthDest:PPkbmVarLength;
   fld:TField;
begin
     // Transfer varlengths.
     // Source varlengths will be freed, but null flag retained from storage.
     if FVarLengthCount>0 then
     begin
          // Browse fields to merge varlengths.
          for i:=0 to FFieldCount-1 do
          begin
               fld:=FOwner.Fields[i];
               if (fld.FieldNo>0) and ((FFieldFlags[fld.FieldNo-1] and kbmffIndirect)<>0) then
               begin
                    pFieldSrc:=GetFieldPointer(SourceRecord,fld);
                    pFieldDest:=GetFieldPointer(DestRecord,fld);
                    pVarLengthSrc:=PPkbmVarLength(pFieldSrc+1);
                    pVarLengthDest:=PPkbmVarLength(pFieldDest+1);

                    // If source varlength, move it.
                    if (pVarLengthSrc^<>nil) then
                    begin
                         // Check if destination allocated, free it.
                         if pVarLengthDest^<>nil then
                            FreeVarLength(pVarLengthDest^);
                         pVarLengthDest^:=pVarLengthSrc^;
                         pVarLengthSrc^:=nil;
                    end

                    // Else if no source, check if indirect null.
                    else if (pFieldSrc^=kbmffNull) and (pVarLengthDest^<>nil) then
                    begin
                         FreeVarLength(pVarLengthDest^);
                         pVarLengthDest^:=nil;
                         pFieldDest^:=kbmffNull;
                    end;
               end;
          end;
     end;

     // Move fixed part of record to storage.
     _InternalMoveRecord(SourceRecord,DestRecord);
end;

// Deallocate space for a record.
procedure TkbmCommon._InternalFreeRecord(ARecord:PkbmRecord; FreeVarLengths,FreeVersions:boolean);
begin
     if ARecord=nil then exit;

{$IFDEF DO_CHECKRECORD}
     _InternalCheckRecord(ARecord);
{$ENDIF}
     if FreeVarLengths then _InternalFreeRecordVarLengths(ARecord);

     // Free record data, incl. previous versioning records if any, but only if actual record in table.
     with ARecord^ do
     begin
           if ((Flag and kbmrfInTable)<>0) and FreeVersions and (PrevRecordVersion<>nil) then
           begin
                _InternalFreeRecord(PrevRecordVersion,FreeVarLengths,true);
                PrevRecordVersion:=nil;
           end;

           // Clear out data to be able to handle invalid bookmarks.
           FillChar(ARecord^,FTotalRecordSize,0);
     end;
     // Free record.
     FreeMem(ARecord);
end;

// Clear record buffer.
procedure TkbmCommon._InternalClearRecord(ARecord:PkbmRecord);
begin
{$IFDEF DO_CHECKRECORD}
     _InternalCheckRecord(ARecord);
{$ENDIF}
     ARecord^.RecordNo:=-1;
     ARecord^.RecordID:=-1;
     ARecord^.UniqueRecordID:=-1;
     ARecord^.Tag:=0;
     ARecord^.PrevRecordVersion:=nil;
     ARecord^.TransactionLevel:=-1;
     ARecord^.UpdateStatus:=usUnmodified;
     ARecord^.Flag:=0;
     FillChar(ARecord^.Data^,FDataRecordSize,0);
end;

// Allocate space for a duplicate record, and copy the info to it.
function TkbmCommon._InternalCopyRecord(SourceRecord:PkbmRecord;CopyVarLengths:boolean):PkbmRecord;
begin
{$IFDEF DO_CHECKRECORD}
     _InternalCheckRecord(SourceRecord);
{$ENDIF}

     Result:=_InternalAllocRecord;
     with Result^ do
     begin
          _InternalMoveRecord(SourceRecord,Result);
          if CopyVarLengths then _InternalCopyVarLengths(SourceRecord,Result);
     end;
end;

// Copy a var length from one record to another.
// If destination has a var length allready, it will be deleted.
procedure TkbmCommon._InternalCopyVarLength(SourceRecord,DestRecord:PkbmRecord; Field:TField);
var
   pFldSrc,pFldDest:PByte;
   pVarLenSrc,pVarLenDest:PPkbmVarLength;
   pVarLenClone:PkbmVarLength;
begin
     pFldSrc:=GetFieldPointer (SourceRecord,Field);
     pFldDest:=GetFieldPointer(DestRecord,Field);

     pVarLenSrc:=PPkbmVarLength(pFldSrc+1);
     pVarLenDest:=PPkbmVarLength(pFldDest+1);

     // Check if varlength in destination, then delete.
     if (pVarLenDest^ <> nil) then
     begin
          FreeVarLength(pVarLenDest^);
          pVarLenDest^:=nil;
          pFldDest^:=kbmffNull;  // Set field value to NULL.
     end;

     // Copy varlength from source to destination.
     if (pVarLenSrc^ <> nil) then
     begin
          pVarLenClone:=CopyVarLength(pVarLenSrc^);
          pVarLenDest^:=pVarLenClone;
          pFldDest^:=kbmffData;  // Set field value to NOT NULL.
     end;
end;

// Copy var lengths from one record to another.
procedure TkbmCommon._InternalCopyVarLengths(SourceRec,DestRec:PkbmRecord);
var
   i:TkbmNativeInt;
   fld:TField;
begin
     // Copy varlengths if any defined.
     if FVarLengthCount>0 then
     begin
          // Browse fields to copy varlengths.
          for i:=0 to FFieldCount-1 do
          begin
               fld:=FOwner.Fields[i];
               if (fld.FieldNo>0) and ((FFieldFlags[fld.FieldNo-1] and kbmffIndirect)<>0) then
                 _InternalCopyVarLength(SourceRec,DestRec,FOwner.Fields[i]);
          end;
     end;
end;

// Compression of a field buffer.
function TkbmCommon.CompressFieldBuffer(Field:TField; const Buffer:pointer; var Size:TkbmLongInt):pointer;
 {$IFNDEF KBMMEMTABLE_USE_WSTRLEN}
var
   p:PByte;
   sz:TkbmLongInt;
 {$ENDIF}
begin
     case Field.DataType of
        ftFixedChar,
        ftString:
          begin
               // Store the 0 even if its taking up one extra byte in all cases.
               // Simplifies decompression.
{$IFDEF NEXTGEN}
               Size:=SysUtils.StrLen(PChar(Buffer))+1;
{$ELSE}
               Size:={$IFDEF LEVEL18}AnsiStrings.{$ENDIF}StrLen(PAnsiChar(Buffer))+1;
{$ENDIF}
               Result:=Buffer;
          end;

 {$IFDEF KBMMEMTABLE_SUPPORT_FT_FIXEDWIDECHAR}
        ftFixedWideChar,
 {$ENDIF}
        ftWideString:
          begin
 {$IFDEF KBMMEMTABLE_USE_WSTRLEN}
               // Return actual size of the data rather than max size.
               // Includes ending 00.
  {$IFDEF NEXTGEN}
               Size:=strlen(PChar(Buffer))+1;
  {$ELSE}
               Size:=(WStrLen(PWideChar(Buffer))+1) * sizeof(WideChar);
  {$ENDIF}
               Result:=Buffer;
 {$ELSE}
  {$IFDEF FPC}
               sz:=(length(WideString(PWideChar(Buffer)))+1)*sizeof(WideChar);
               p:=PByte(Buffer);
               Size:=sz;
  {$ELSE}
               p:=PByte(Buffer);
               sz:=PInteger(p)^;
               Size:=sz+sizeof(Integer);
  {$ENDIF}

               // Return actual size of the data rather than max size.
               Result:=Buffer;
 {$ENDIF}
          end;
        else
          begin
               Result:=Buffer;
          end;
     end;
end;

// Decompression of a field buffer.
// Since we at the time only handles strings truncated at the 0 char,
// simply return the buffer and allready known size.

function TkbmCommon.DecompressFieldBuffer(Field:TField; const Buffer:pointer; var Size:TkbmLongInt):pointer;
begin
     Result:=Buffer;
end;

function TkbmCommon.GetDeletedRecordsCount:TkbmNativeInt;
begin
     Result:=DeletedRecordCount;
end;

// Move contents of one record to another.
// If not to move varlength fields, copies field contents by fieldcontents.

procedure TkbmCommon._InternalMoveRecord(SourceRecord,DestRecord:PkbmRecord);
var
   i:TkbmNativeInt;
   fld:TField;
begin
{$IFDEF DO_CHECKRECORD}
     _InternalCheckRecord(SourceRecord);
     DestRecord^.StartIdent:=kbmRecordIdent;
     DestRecord^.EndIdent:=kbmRecordIdent;
{$ENDIF}
     DestRecord^.RecordNo:=SourceRecord^.RecordNo;
     DestRecord^.RecordID:=SourceRecord^.RecordID;
     DestRecord^.UniqueRecordID:=SourceRecord^.UniqueRecordID;
     DestRecord^.UpdateStatus:=SourceRecord^.UpdateStatus;
     DestRecord^.PrevRecordVersion:=SourceRecord^.PrevRecordVersion;
     DestRecord^.TransactionLevel:=SourceRecord^.TransactionLevel;
     DestRecord^.Tag:=SourceRecord^.Tag;

     // Move fixed part of record, excluding varlengths.
     kbmMemMove(SourceRecord^.Data^,DestRecord^.Data^,FFixedRecordSize);

     // Copy varlengths null flags.
     if (FVarLengthCount>0) then
     begin
          // Browse fields to copy varlengths nullflags.
          for i:=0 to FFieldCount-1 do
          begin
               fld:=FOwner.Fields[i];
               if (fld.FieldNo>0) and ((FFieldFlags[fld.FieldNo-1] and kbmffIndirect)<>0) then
                  GetFieldPointer(DestRecord,fld)^ := GetFieldPointer(SourceRecord,fld)^;
          end;
     end;
end;

// Compare two records.

function TkbmCommon._InternalCompareRecords(const AFieldList:TkbmFieldList; const AMaxFields:integer; const AKeyRecord,ARecord:PkbmRecord; const AIgnoreNull,APartial,AUniqueConstraint:boolean; const AHow:TkbmCompareHow): Integer;
var
   i,o:integer;
   p1,p2:PByte;
   sz1,sz2:TkbmLongInt;
   pv1,pv2:PkbmVarLength;
   fld:TField;
   n:integer;
   flags:byte;
   RecID:TkbmNativeInt;
   ARec:PkbmRecord;
   ifo:TkbmifoOptions;
   FullCompare:boolean;
   fno:integer;
const
   // First index is if mtifoNullFirst, 2nd is if mtifoDescending.
   nvl:array[boolean,boolean] of integer = (
      (-1,1),
      (1,-1)
   );
   nvlneg:array[boolean,boolean] of integer = (
      (1,-1),
      (-1,1)
   );

begin
{$IFDEF USE_SAFE_CODE}
     if (AKeyRecord=nil) or (ARecord=nil) then
     begin
          Result:=0;
          exit;
     end;
{$ENDIF}

{$IFDEF DO_CHECKRECORD}
     _InternalCheckRecord(KeyRecord);
     _InternalCheckRecord(ARecord);
{$ENDIF}

     n:=AFieldList.Count;
     if (AMaxFields>0) and (AMaxFields<n) then n:=AMaxFields;

     // Loop through all indexfields, left to right.
     FullCompare:=true;
     i:=0;
     Result:=0;
     while i<n do
     begin
          fld:=AFieldList.Fields[i];
          ifo:=AFieldList.Options[i];

          if AHow<>chBreakNE then Exclude(ifo,mtifoDescending); // $FF -
          if APartial then Include(ifo,mtifoPartial);

          // Get data for specified field for the two records.
          o:=AFieldList.FieldOfs[i];
          p1:=AKeyRecord^.Data;
          p2:=ARecord^.Data;
          inc(p1,o);
          inc(p2,o);

          // Check if to ignore null field in key record.
          if (p1[0]=kbmffNull) and (AIgnoreNull or (mtifoIgnoreNull in ifo)) then
          begin
//OutputDebugString(PChar('Ignore null in key. Partial result='+IntToStr(Result)));
               inc(i);
               continue;
          end;

          // Check if both not null.
          if (p1[0]<>kbmffNull) and (p2[0]<>kbmffNull) then
          begin
               // Skip null flag.
               inc(p1);
               inc(p2);

               // Check if indirect fields.
               fno:=AFieldList.FieldNo[i];
               if (fno>0) then
               begin
                    flags:=FFieldFlags[fno-1];
                    if (flags and kbmffIndirect)<>0 then
                    begin
                         pv1:=PPkbmVarLength(p1)^;
                         if pv1=nil then
                         begin
                              // Find the record in the recordlist using the unique record id.
                              RecID:=AKeyRecord^.RecordID;
                              if (RecID>=0) then
                              begin
                                   ARec:=PkbmRecord(FRecords.Items[RecID]);
                                   p1:=GetFieldPointer(ARec,fld);
                                   inc(p1);
                                   pv1:=PPkbmVarLength(p1)^;
                              end
                              // If by any chance no valid recordis is found, something is really rotten.
                              else raise EMemTableInvalidRecord.Create(kbmInvalidRecord);
                         end;

                         p1:=GetVarLengthData(pv1);
                         pv2:=PPkbmVarLength(p2)^;
                         if pv2=nil then
                         begin
                              // Find the record in the recordlist using the unique record id.
                              RecID:=ARecord^.RecordID;
                              if (RecID>=0) then
                              begin
                                   ARec:=PkbmRecord(FRecords.Items[RecID]);
                                   p2:=GetFieldPointer(ARec,fld);
                                   inc(p2);
                                   pv2:=PPkbmVarLength(p2)^;
                              end
                              // If by any chance no valid recordis is found, something is really rotten.
                              else raise EMemTableInvalidRecord.Create(kbmInvalidRecord);
                         end;
                         p2:=GetVarLengthData(pv2);

                         if (flags and kbmffCompress)<>0 then
                         begin
                              sz1:=GetVarLengthSize(pv1);
                              sz2:=GetVarLengthSize(pv2);
                              if (Assigned(FOwner.FOnDecompressField)) then
                              begin
                                   FOwner.FOnDecompressField(FOwner,fld,p1,sz1,p1);
                                   FOwner.FOnDecompressField(FOwner,fld,p2,sz2,p2);
                              end
                              else
                              begin
                                   p1:=DecompressFieldBuffer(fld,p1,sz1);
                                   p2:=DecompressFieldBuffer(fld,p2,sz2);
                              end;
                         end;
                    end;
               end;

               // Compare the fields.
               if (Assigned(FOwner.FOnCompareFields)) then
               begin
                    Result:=0;
                    FOwner.FOnCompareFields(FOwner,fld,p1,p2,fld.DataType,ifo,FullCompare,Result);
               end
               else
                   Result:=CompareFields(p1,p2,fld.DataType,FLocaleID,ifo,FullCompare);
          end
          else if (p1[0]<>kbmffNull) then
          begin
               // Set default result when comparing with null. (Oracle (default) NULL is highest, MSSQL (mtifoNullFirst) NULL is lowest).
               Result:=nvl[mtifoNullFirst in ifo,mtifoDescending in ifo];
//OutputDebugString(PChar('Key<>null, comparer=null, Result='+IntToStr(Result)));
          end
          else if (p2[0]<>kbmffNull) then
          begin
               Result:=nvlneg[mtifoNullFirst in ifo,mtifoDescending in ifo];
//OutputDebugString(PChar('Key=null, comparer<>null, Result='+IntToStr(Result)));
          end
          else
          begin
               if AUniqueConstraint then
                  Result:=nvl[mtifoNullFirst in ifo,mtifoDescending in ifo]
               else
                   Result:=0;
//OutputDebugString(PChar('Key=null, comparer=null, Result='+IntToStr(Result)));
          end;

          // Check type of comparison.
          case AHow of
               chBreakNE:
                  begin
                       if (Result<>0) or (not FullCompare) then break;
                  end;

               chBreakGTE:
                  begin
                       if Result>=0 then break;
                  end;

               chBreakLTE:
                  begin
                       if Result<=0 then break;
                  end;

               chBreakGT:
                  begin
                       if Result>0 then break;
                  end;

               chBreakLT:
                  begin
                       if Result<0 then break;
                  end;
          end;
          inc(i);
     end;

//OutputDebugString(PChar(' CompareRecords Result='+IntToStr(Result)));
end;

// Append record to chain of records.
procedure TkbmCommon._InternalAppendRecord(ARecord:PkbmRecord);
begin
{$IFDEF DO_CHECKRECORD}
     _InternalCheckRecord(ARecord);
{$ENDIF}

     AppendRecord(ARecord);
end;

// Delete record from chain.
procedure TkbmCommon._InternalDeleteRecord(ARecord:PkbmRecord);
begin
     if ARecord=nil then exit;

{$IFDEF DO_CHECKRECORD}
     _InternalCheckRecord(ARecord);
{$ENDIF}

     DeleteRecord(ARecord);
     _InternalFreeRecord(ARecord,true,true);
end;

// Pack records.
procedure TkbmCommon._InternalPackRecords;
begin
     PackRecords;
end;

// Purge all records.
procedure TkbmCommon._InternalEmpty;
var
   i:TkbmNativeInt;
begin
     // Remove the records.
     for i:=0 to FRecords.Count-1 do
         _InternalFreeRecord (FRecords.Items[i],true,true);

     FDeletedRecords.Clear;
     FRecordID:=0;
     FUniqueRecordID:=0;
     FRecords.Clear;
     FDataID:=GetUniqueDataID;
end;

function TkbmCommon.GetFieldSize(FieldType:TFieldType; Size:TkbmLongInt):TkbmLongInt;
begin
     case FieldType of
 {$IFDEF KBMMEMTABLE_SUPPORT_FT_FIXEDWIDECHAR}
          DB.ftFixedWideChar,
 {$endif}
          DB.ftWideString:         Result:=Size*sizeof(WideChar)+sizeof(integer); // 4 bytes length + 2 bytes/character

{$IFDEF KBMMEMTABLE_SUPPORT_FT_ORAINTERVAL}
          DB.ftOraInterval:        Result:=20+1; // 20 chars + zero end character.
{$ENDIF}
          DB.ftFixedChar,
          DB.ftString:             Result:=Size+1; // Size + zero end character.
          DB.ftGUID:               Result:=38+1; // 38 + zero end character.
          DB.ftBytes:              Result:=Size;
          DB.ftVarBytes:           Result:=Size+SizeOf(Word);
          DB.ftSmallInt:           Result:=SizeOf(SmallInt);
          DB.ftInteger:            Result:=SizeOf(Integer);
{$IFDEF KBMMEMTABLE_SUPPORT_FT_SHORTINT}
          DB.ftShortint:           Result:=SizeOf(Integer);
{$ENDIF}
{$IFDEF KBMMEMTABLE_SUPPORT_FT_BYTE}
          DB.ftByte:               Result:=SizeOf(Integer);
{$ENDIF}
          DB.ftLargeInt:           Result:=SizeOf(Int64);
          DB.ftADT,ftArray:        Result:=0;
          DB.ftWord:               Result:=SizeOf(Word);
          DB.ftBoolean:            Result:=SizeOf(WordBool);
{$IFDEF KBMMWMTABLE_SUPPORT_FT_SINGLE}
          DB.ftSingle:             Result:=SizeOf(Double);
{$ENDIF}
          DB.ftFloat:              Result:=SizeOf(Double);
          DB.ftCurrency:           Result:=SizeOf(Double);
          DB.ftDate:               Result:=SizeOf(longint);
          DB.ftTime:               Result:=SizeOf(longint);
          DB.ftDateTime:           Result:=SizeOf(TDateTimeRec);
 {$IFDEF KBMMEMTABLE_SUPPORT_FT_ORATIMESTAMP}
          DB.ftOraTimeStamp,
{$ENDIF}
          DB.ftTimeStamp:          Result:=SizeOf(TSQLTimeStamp);
          DB.ftAutoInc:            Result:=SizeOf(Integer);
          DB.ftBlob:               Result:=0;
{$IFDEF KBMMEMTABLE_SUPPORT_FT_WIDEMEMO}
          DB.ftWideMemo:           Result:=0;
{$ENDIF}
          DB.ftMemo:               Result:=0;
          DB.ftGraphic:            Result:=0;
          DB.ftFmtMemo:            Result:=0;
          DB.ftParadoxOle:         Result:=0;
          DB.ftDBaseOle:           Result:=0;
          DB.ftTypedBinary:        Result:=0;
          DB.ftOraBlob,
          DB.ftOraClob:            Result:=0;
          DB.ftBCD,
          DB.ftFmtBCD:             Result:=34; // SizeOf(TBCD);
{$IFDEF KBMMEMTABLE_SUPPORT_FT_LONGWORD}
          DB.ftLongWord:           Result:=SizeOf(LongWord);
{$ENDIF}
{$IFDEF KBMMEMTABLE_SUPPORT_FT_EXTENDED}
          DB.ftExtended:        Result:=SizeOf(Extended);
{$ENDIF}
     else
          Result:=0;
     end;
end;


function TkbmCommon.GetFieldPointer(ARecord:PkbmRecord; Field:TField):PByte;
var
   n:integer;
begin
{$IFDEF USE_SAFE_CODE}
     Result:=nil;
     if ARecord=nil then exit;
{$ENDIF}

     Result:=ARecord^.Data;

{$IFDEF USE_SAFE_CODE}
     if Result=nil then exit;
{$ENDIF}
     n:=Field.FieldNo;
     if n>0 then
        inc(Result,FFieldOfs[n-1])
     else
        inc(Result,FStartCalculated+Field.Offset);
end;

function TkbmCommon.GetFieldDataOffset(Field:TField):TkbmLongInt;
var
   n:integer;
begin
     n:=Field.FieldNo;
     if n>0 then
        Result:=FFieldOfs[n-1]
     else
        Result:=FStartCalculated+Field.Offset;
end;

constructor TkbmCommon.Create(AOwner:TkbmCustomMemTable);
begin
     inherited Create;

     FLock:=TkbmCS.Create{$IFDEF KBMMEMTABLE_DEBUG_LOCKS}(self){$ENDIF};

     // Setup owner of table.
     FOwner:=AOwner;

     // Create physical list of records.
     FRecords:=TkbmList.Create;

     // Create list of deleted but not purged records.
     FDeletedRecords:=TkbmList.Create;

     // Set default data id to a random number. (max 2 bytes).
     FDataID:=GetUniqueDataID;

     // Set defaults.
     FAutoIncMin:=0;
     FAutoIncMax:=0;
     FDeletedCount:=0;
     FUniqueRecordID:=0;
     FRecordID:=0;
     FPerformance:=mtpfFast;
     FVersioningMode:=mtvm1SinceCheckPoint;
     FEnableVersioning:=false;
     FTransactionLevel:=0;

{$IFDEF MSWINDOWS}
     LocaleID:=LOCALE_USER_DEFAULT;
{$ELSE}
     LocaleID:=0;
{$ENDIF}

     // Attached tables.
     FAttachMaxCount:=1;
{$IFDEF NEXTGEN}
     FAttachedTables:=TList<TkbmCustomMemTable>.Create;
{$ELSE}
     FAttachedTables:=TList.Create;
{$ENDIF}
end;

destructor TkbmCommon.Destroy;
var
   i:TkbmNativeInt;
   mt:TkbmCustomMemTable;
begin
     // Check if any tables attached to this, deattach them.
     for i:=0 to FAttachedTables.Count-1 do
     begin
          mt:=TkbmCustomMemTable(FAttachedTables.Items[i]);
          if mt<>nil then
          begin
               mt.Close;
               mt.AttachedTo:=nil;
          end;
     end;

     FAttachedTables.free;
     FDeletedRecords.free;
     FRecords.free;

     FLock.Free;
     inherited;
end;

function TkbmCommon.GetUniqueDataID:TkbmNativeInt;
begin
     repeat
           Result:=random(1 shl 30 + 1);
     until Result<>FDataID;
end;

procedure TkbmCommon.ClearModifiedFlags;
var
   i:TkbmNativeInt;
begin
     Lock;
     try
        // Clear all modification flags.
        for i:=0 to length(FFieldFlags)-1 do
            FFieldFlags[i]:=FFieldFlags[i] and (high(byte)-kbmffModified);
     finally
        UnLock;
     end;
end;

function TkbmCommon.GetModifiedFlag(i:integer):boolean;
begin
     Lock;
     Result:=false;
     try
        if (i<0) or (i>=FFieldCount) then raise ERangeError.CreateFmt(kbmOutOfRange,[i]);
        Result:=(FFieldFlags[i] and kbmffModified)<>0;
     finally
        Unlock;
     end;
end;

procedure TkbmCommon.SetModifiedFlag(i:integer; Value:boolean);
begin
     Lock;
     try
        if (i<0) or (i>=FFieldCount) then raise ERangeError.CreateFmt(kbmOutOfRange,[i]);
        if Value then
           FFieldFlags[i]:=FFieldFlags[i] and kbmffModified
        else
           FFieldFlags[i]:=FFieldFlags[i] and (high(byte)-kbmffModified);
     finally
        Unlock;
     end;
end;

function TkbmCommon.GetAttachMaxCount:integer;
begin
     Lock;
     try
        Result:=FAttachMaxCount;
     finally
        Unlock;
     end;
end;

function TkbmCommon.GetAttachCount:integer;
var
   i:TkbmNativeInt;
begin
     Lock;
     Result:=0;
     try
        for i:=0 to FAttachedTables.Count-1 do
            if FAttachedTables.Items[i]<>nil then inc(Result);
     finally
        Unlock;
     end;
end;

procedure TkbmCommon.EnsureFields(const ACount:integer);
begin
     if Length(FFieldOfs)<=ACount then
     begin
          SetLength(FFieldOfs,ACount+20);
          SetLength(FFieldFlags,ACount+20);
     end;
end;

procedure TkbmCommon.SetAttachMaxCount(Value:integer);
begin
     if Value=FAttachMaxCount then exit;

     if Value<1 then
        raise ERangeError.Create(kbmInvArgument);

     if IsAnyTableActive then
        raise EMemTableError.Create(kbmTableMustBeClosed);

     Lock;
     try
        FAttachedTables.count:=Value;
        FAttachMaxCount:=Value;
     finally
        Unlock;
     end;
end;

procedure TkbmCommon.SetRecordID(ARecordID:TkbmNativeInt);
begin
     Lock;
     try
        FRecordID:=ARecordID;
     finally
        Unlock;
     end;
end;

procedure TkbmCommon.SetUniqueRecordID(ARecordID:TkbmNativeInt);
begin
     Lock;
     try
        FUniqueRecordID:=ARecordID;
     finally
        Unlock;
     end;
end;

procedure TkbmCommon.SetDeletedCount(ACount:TkbmNativeInt);
begin
     Lock;
     try
        FDeletedCount:=ACount;
     finally
        Unlock;
     end;
end;

procedure TkbmCommon.CalcLocaleID;
var
   ALID:word;
begin
     Lock;
     try
        ALID:=(word(FSubLanguageID) shl 10) or word(FLanguageID);
        FLocaleID:=(FSortID shl 16) or ALID;
     finally
        Unlock;
     end;
end;

function TkbmCommon.GetLocaleID:integer;
begin
     Lock;
     try
        Result:=FLocaleID;
     finally
        Unlock;
     end;
end;

procedure TkbmCommon.SetLocaleID(Value:integer);
var
   ALID:word;
begin
     Lock;
     try
        FLocaleID:=Value;
        FSortID:=(FLocaleID shr 16) and $F;
        ALID:=FLocaleID and $FFFF;
        FLanguageID:=ALID and $FF;
        FSubLanguageID:=(ALID shr 10) and $FF;
     finally
        Unlock;
     end;
end;

function TkbmCommon.GetLanguageID:integer;
begin
     Lock;
     try
        Result:=FLanguageID;
     finally
        Unlock;
     end;
end;

procedure TkbmCommon.SetLanguageID(Value:integer);
begin
     Lock;
     try
        FLanguageID:=Value;
        CalcLocaleID;
     finally
        Unlock;
     end;
end;

function TkbmCommon.GetSortID:integer;
begin
     Lock;
     try
        Result:=FSortID;
     finally
        Unlock;
     end;
end;

procedure TkbmCommon.SetSortID(Value:integer);
begin
     Lock;
     try
        FSortID:=Value;
        CalcLocaleID;
     finally
        Unlock;
     end;
end;

function TkbmCommon.GetSubLanguageID:integer;
begin
     Lock;
     try
        Result:=FSubLanguageID;
     finally
        Unlock;
     end;
end;

procedure TkbmCommon.SetSubLanguageID(Value:integer);
begin
     Lock;
     try
        FSubLanguageID:=Value;
        CalcLocaleID;
     finally
        Unlock;
     end;
end;

procedure TkbmCommon.Lock;
begin
     if not FStandalone then
        FLock.Enter;
end;

procedure TkbmCommon.Unlock;
begin
     if not FStandalone then
        FLock.Leave;
end;

// Rollback transaction.
procedure TkbmCommon.RollBack;
var
   i:TkbmNativeInt;
   pRec:PkbmRecord;
begin
     // Loop through all records and discard newest current transactions.
     FIsDataModified:=false;
     for i:=0 to FRecords.count-1 do
     begin
          pRec:=PkbmRecord(FRecords.Items[i]);
          if pRec=nil then continue;

          // While same transaction level.
          while pRec^.TransactionLevel=FTransactionLevel do
          begin
               // Check what happened with this version.
               case pRec^.UpdateStatus of
                    // Inserted, delete it again.
                    usInserted:
                      begin
                           _InternalFreeRecord(pRec,true,true);
                           FDeletedRecords.Add(pointer(i));
                           FRecords.Items[i]:=nil;
                           pRec:=nil;
                           break;
                      end;

                    // Marked for deletion or modified, change to older version.
                    usDeleted,
                    usModified:
                      begin
                           FRecords.Items[i]:=pRec^.PrevRecordVersion;
                           _InternalFreeRecord(pRec,true,false);
                      end;

                    // Done nothing. Skip.
                    usUnmodified: break;
               end;
               pRec:=FRecords.Items[i];
          end;

          // Check if still modified, set modified flag.
          if (pRec<>nil) and (pRec^.UpdateStatus<>usUnmodified) then FIsDataModified:=true;
     end;
end;

// Commit transaction.
procedure TkbmCommon.Commit;
var
   i:TkbmNativeInt;
   pRec:PkbmRecord;
begin
     // Loop through all records and discard older transactions.
     for i:=0 to FRecords.count-1 do
     begin
          pRec:=PkbmRecord(FRecords.Items[i]);
          if pRec=nil then continue;

          // While same transaction level, use newest, discard rest.
          if pRec^.TransactionLevel<>FTransactionLevel then continue;

          // Simply decrement transaction level.
          dec(pRec^.TransactionLevel);
     end;
end;

procedure TkbmCommon.Undo(ARecord:PkbmRecord);
var
   recid:TkbmNativeInt;
   oRecord:PkbmRecord;
begin
     if ARecord=nil then exit;

     Lock;
     try
        // Get the record pointer from the storage.
        recid:=ARecord^.RecordID;
        if recid<0 then exit;
        ARecord:=FRecords.Items[recid];

        // Check what happened with this version.
        case ARecord^.UpdateStatus of

             // Inserted, delete it again.
             usInserted:
               begin
                    ReflectToIndexes(nil,mtiuhDelete,ARecord,nil,ARecord^.RecordNo,true);
                    exit;
               end;

             // Marked for deletion or modified, change to older version.
             usDeleted,usModified:
               begin
                    // Figure out what status the record had before.
                    oRecord:=ARecord^.PrevRecordVersion;

                    // Free references to deleted record version.
                    ReflectToIndexes(nil,mtiuhDelete,ARecord,nil,-1,true);

                    // Reinsert previous record version.
                    ReflectToIndexes(nil,mtiuhInsert,nil,oRecord,oRecord^.RecordNo,true);

                    // Update physical record buffer.
                    FRecords.Items[recid]:=ARecord^.PrevRecordVersion;
               end;

             // Done nothing. Skip.
             usUnmodified: exit;
        end;
     finally
        Unlock;
     end;
end;

// Return false if the field should be included as fixed size in the record.
function TkbmCommon.GetFieldIsVarLength(FieldType:TFieldType; Size:TkbmLongInt):boolean;
begin
     Result:=false;

     // No need to store small amounts of data or fixed length data indirectly.
     if (FieldType in kbmVarLengthNonBlobTypes) and (Size>12) then
     begin
          // If should be as fast as possible, dont go indirectly, else ok.
          if (FPerformance <> mtpfFast) then Result:=true;
     end
     else if (FieldType in kbmBlobTypes) then Result:=true;
end;

procedure TkbmCommon.SetStandalone(Value:boolean);
begin
     Lock;
     try
        if FAttachedTables.Count>1 then
           raise EMemTableError.Create(kbmChildrenAttached);
        FStandAlone:=Value;
     finally
        Unlock;
     end;
end;

function TkbmCommon.GetStandalone:boolean;
begin
     Result:=FStandalone;
end;

function TkbmCommon.RecordCount:TkbmNativeInt;
begin
     Lock;
     try
        Result:=FRecords.Count;
     finally
        Unlock;
     end;
end;

procedure TkbmCommon.AppendRecord(ARecord:PkbmRecord);
var
   d,r:TkbmNativeInt;
begin
     Lock;
     try
        // Check if to reuse a deleted spot.
        if FDeletedRecords.Count>0 then
        begin
             d:=FDeletedRecords.Count-1;
             r:=Integer(FDeletedRecords.Items[d]);

             // Put 'physical' record number into record.
             ARecord^.RecordID:=r;
             FDeletedRecords.Delete(d);

             // Put unique record number into record.
             ARecord^.UniqueRecordID:=FUniqueRecordID;
             inc(FUniqueRecordID);

             FRecords.Items[r]:=ARecord;
             ARecord^.Flag:=(ARecord^.Flag or kbmrfIntable);
        end
        else
        begin
             // Put 'physical' record number into record.
             ARecord^.RecordID:=FRecordID;
             inc(FRecordID);

             // Put unique record number into record.
             ARecord^.UniqueRecordID:=FUniqueRecordID;
             inc(FUniqueRecordID);

             ARecord^.Flag:=(ARecord^.Flag or kbmrfIntable);
             FRecords.Add(ARecord);

             // Check if running out of valid bookmark ID's.
             // Very unlikely (needs inserting 2 billion records), but possible.
             if FUniqueRecordID>=high(TkbmNativeInt) then
                raise EMemTableFatalError.Create(kbmOutOfBookmarks);
        end;

        if FEnableVersioning then
           ARecord^.UpdateStatus:=usInserted;
     finally
        Unlock;
     end;
end;

procedure TkbmCommon.DeleteRecord(ARecord:PkbmRecord);
begin
     Lock;
     try
        FDeletedRecords.Add (pointer(ARecord.RecordID));
        FRecords.Items[ARecord.RecordID]:=nil;
     finally
        Unlock;
     end;
end;

procedure TkbmCommon.PackRecords;
var
   i:TkbmNativeInt;
begin
     Lock;
     try
        FRecords.Pack;
        for i:=0 to FRecords.Count-1 do
            if FRecords.Items[i]<>nil then PkbmRecord(FRecords.Items[i])^.RecordID:=i;
        FDeletedRecords.Clear;
     finally
        Unlock;
     end;
end;

function TkbmCommon.DeletedRecordCount:TkbmNativeInt;
begin
     Lock;
     try
        Result:=FDeletedRecords.Count;
     finally
        Unlock;
     end;
end;

procedure TkbmCommon.SetAutoIncMin(Value:TkbmNativeInt);
begin
     Lock;
     try
        FAutoIncMin:=Value;
        if FAutoIncMax<FAutoIncMin then FAutoIncMax:=FAutoIncMin-1;
     finally
        Unlock;
     end;
end;

procedure TkbmCommon.SetAutoIncMax(Value:TkbmNativeInt);
begin
     Lock;
     try
        FAutoIncMax:=Value;
     finally
        Unlock;
     end;
end;

function TkbmCommon.GetAutoIncMin:TkbmNativeInt;
begin
     Result:=FAutoIncMin;
end;

function TkbmCommon.GetAutoIncMax:TkbmNativeInt;
begin
     Result:=FAutoIncMax;
end;

procedure TkbmCommon.SetPerformance(Value:TkbmPerformance);
begin
     Lock;
     try
        FPerformance:=Value;
     finally
        Unlock;
     end;
end;

function TkbmCommon.GetPerformance:TkbmPerformance;
begin
     Result:=FPerformance;
end;

procedure TkbmCommon.SetVersioningMode(Value:TkbmVersioningMode);
begin
     Lock;
     try
        FVersioningMode:=Value;
     finally
        Unlock;
     end;
end;

function TkbmCommon.GetVersioningMode:TkbmVersioningMode;
begin
     Result:=FVersioningMode;
end;

procedure TkbmCommon.SetEnableVersioning(Value:boolean);
begin
     Lock;
     try
        FEnableVersioning:=Value;
     finally
        Unlock;
     end;
end;

function TkbmCommon.GetEnableVersioning:boolean;
begin
     Result:=FEnableVersioning;
end;

procedure TkbmCommon.SetCapacity(Value:TkbmNativeInt);
begin
     Lock;
     try
//TODO KBM        FRecords.Capacity:=Value;
     finally
        Unlock;
     end;
end;

function TkbmCommon.GetCapacity:TkbmNativeInt;
begin
     Lock;
     try
        Result:=FRecords.Capacity;
     finally
        Unlock;
     end;
end;

function TkbmCommon.GetIsDataModified:boolean;
begin
     Lock;
     try
        Result:=FIsDataModified;
     finally
        Unlock;
     end;
end;

procedure TkbmCommon.SetIsDataModified(Value:boolean);
begin
     Lock;
     try
        FIsDataModified:=Value;
     finally
        Unlock;
     end;
end;

function TkbmCommon.GetTransactionLevel:integer;
begin
     Lock;
     try
        Result:=FTransactionLevel;
     finally
        Unlock;
     end;
end;

procedure TkbmCommon.IncTransactionLevel;
begin
     Lock;
     try
        inc(FTransactionLevel);
     finally
        Unlock;
     end;
end;

procedure TkbmCommon.DecTransactionLevel;
begin
     Lock;
     try
        if FTransactionLevel>0 then
           dec(FTransactionLevel);
     finally
        Unlock;
     end;
end;

procedure TkbmCommon.DeAttachAllTables(AExceptTable:TkbmCustomMemTable);
var
   i:TkbmNativeInt;
   t:TkbmCustomMemTable;
begin
     Lock;
     try
        for i:=0 to FAttachedTables.Count-1 do
        begin
             t:=FAttachedTables.Items[i];
             if t<>nil then
             begin
                  if t<>AExceptTable then
                     t.AttachedTo:=nil;
             end;
        end;
     finally
        Unlock;
     end;
end;

procedure TkbmCommon.DeAttachTable(ATable:TkbmCustomMemTable);
var
   i:integer;
begin
     Lock;
     try
        i:=FAttachedTables.IndexOf(ATable);
        if i>=0 then FAttachedTables.Items[i]:=nil; // Only mark as unused.
     finally
        Unlock;
     end;
end;

procedure TkbmCommon.AttachTable(ATable:TkbmCustomMemTable);
var
   i:integer;
begin
     Lock;
     try
        // Look for unused spot.
        i:=FAttachedTables.IndexOf(nil);
        if i<0 then
        begin
             if IsAnyTableActive then
                raise EMemTableError.Create(kbmTableMustBeClosed);
             FAttachedTables.Add(ATable);
             ATable.FTableID:=FAttachedTables.Count-1;
             FAttachMaxCount:=FAttachedTables.Count;
        end
        else
        begin
             // Reuse spot.
             FAttachedTables.Items[i]:=ATable;
             ATable.FTableID:=i;
//             ClearBookmarkInfo(ATable.FTableID);
        end;
     finally
        Unlock;
     end;
end;

// Define recordlayout based on a table.
procedure TkbmCommon.LayoutRecord(const AFieldCount:integer);
  procedure EnumerateFieldDefs(SomeFieldDefs:TFieldDefs; var NbrFields:integer);
  var
     i:TkbmNativeInt;
{$IFDEF KBMMEMTABLE_SUPPORT_NESTEDFIELDS}
     j:TkbmNativeInt;
{$ENDIF}
     sz:integer;
  begin
       with FOwner do
       begin
            for i:=0 to SomeFieldDefs.Count - 1 do
                with SomeFieldDefs[i] do
                begin
                     EnsureFields(NbrFields);

                     // Check if field type supported.
                     if not (DataType in kbmSupportedFieldTypes) then
                        raise EMemTableError.Create(kbminternalOpen1Err+
                              Name
                              +' ('+DisplayName+')'
                              +Format(kbminternalOpen2Err,[integer(DataType)]));

                     // Determine if field is subject to being an indirect field.
                     if { (Fields[FieldNo-1].FieldKind=fkData) and  - Should not be needed since all fielddefs are datafields. }
                        GetFieldIsVarLength(DataType,Size) then
                     begin
                          FFieldFlags[NbrFields]:=FFieldFlags[NbrFields] or kbmffIndirect;

                          // Call user app. to allow override of default unless a blobtype.
                          if (Assigned(FOnSetupField)) and (not (Fields[FieldNo-1].DataType in kbmBlobTypes)) then
                             FOnSetupField(FOwner,Fields[FieldNo-1],FFieldFlags[NbrFields]);
                     end;

                     // If an indirect field (a varlength), dont set fieldofs at this time.
                     if (FFieldFlags[NbrFields] and kbmffIndirect)<>0 then
                     begin
                          FFieldOfs[NbrFields]:=-1;
                          inc(NbrFields);
                     end
                     else
                     begin
                          // Else normal fixed size field embedded in the record.
                          FFieldOfs[NbrFields]:=FFixedRecordSize;

{$IFDEF KBMMEMTABLE_SUPPORT_NESTEDFIELDS}
                          // Check if arraytype field.
                          if ChildDefs.Count > 0 then
                          begin
                               inc(NbrFields);
                               sz:=GetFieldSize(DataType,Size)+1;
                               inc(FFixedRecordSize,sz);
                               if DataType = ftArray then
                                  for j:=1 to Size do EnumerateFieldDefs(ChildDefs,NbrFields)
                               else
                                   EnumerateFieldDefs(ChildDefs,NbrFields);
                          end
                          else
                          begin
                               // Look for fieldsize.
                               sz:=GetFieldSize(DataType,Size)+1;
                               inc(FFixedRecordSize,sz);
                               inc(NbrFields);
                          end;
{$ELSE}
                          // Look for fieldsize.
                          sz:=GetFieldSize(DataType,Size)+1;
                          inc(FFixedRecordSize,sz);
                          inc(NbrFields);
{$ENDIF}
                     end;
                end;
           end;
  end;

  procedure EnumerateVarLengthFieldDefs(SomeFieldDefs:TFieldDefs; var NbrFields:integer);
  var
     i:TkbmNativeInt;
{$IFDEF KBMMEMTABLE_SUPPORT_NESTEDFIELDS}
     j:TkbmNativeInt;
{$ENDIF}
  begin
       with FOwner do
       begin
            for i:=0 to SomeFieldDefs.Count - 1 do
                with SomeFieldDefs[i] do
                begin
                     EnsureFields(NbrFields);

                     // Check if a varlength field (blobs and long strings f.ex.).
                     if (FFieldFlags[NbrFields] and kbmffIndirect)<>0 then
                     begin
                          // Check if to compress it.
                          if FPerformance=mtpfSmall then
                             FFieldFlags[NbrFields]:=FFieldFlags[NbrFields] or kbmffCompress;
                          FFieldOfs[NbrFields]:=FStartVarLength+FVarLengthCount*(SizeOf(PkbmVarLength)+1);
                          inc(FVarLengthCount);
                     end;
                     inc(NbrFields);

{$IFDEF KBMMEMTABLE_SUPPORT_NESTEDFIELDS}
                     // Check if arraytype field. Adjust field counter.
                     if ChildDefs.Count > 0 then
                     begin
                          if DataType = ftArray then
                             for j:=1 to Size do EnumerateVarLengthFieldDefs(ChildDefs,NbrFields)
                          else
                              EnumerateVarLengthFieldDefs(ChildDefs,NbrFields);
                     end;
{$ENDIF}
                end;
       end;
  end;

var
   Temp:integer;
begin
     if FOwner.FieldDefs.Count<=0 then
        raise EMemTableError.Create(kbmVarReason2Err);

     // Calculate size of bookmark array in record.
     FBookmarkArraySize:=sizeof(TkbmBookmark)*FAttachMaxCount;

     // Calculate non blob field offsets into the record.
     FFixedRecordSize:=0;
     FFieldCount:=AFieldCount;
     Temp:=0;
     EnumerateFieldDefs(FOwner.FieldDefs,Temp);

     // Calculate some size variables.
     FStartCalculated:=FFixedRecordSize;
     FCalcRecordSize:=FOwner.__CalcFieldsSize;
     FStartBookmarks:=FStartCalculated+FCalcRecordSize;
     FStartVarLength:=FStartBookmarks+FBookmarkArraySize;

     // Calculate number of var length fields and their place in the record.
     FVarLengthCount:=0;                  // Know of no var length fields in the definition yet.
     Temp:=0;
     EnumerateVarLengthFieldDefs(FOwner.FieldDefs,Temp);
     FVarLengthRecordSize:=FVarLengthCount * (SizeOf(PkbmVarLength)+1);

     // Calculate total sizes in different variations.
     FFixedRecordSize:=FStartVarLength;
     FDataRecordSize:=FFixedRecordSize+FVarLengthRecordSize;
     FTotalRecordSize:=sizeof(TkbmRecord)+FDataRecordSize;

     FIsDataModified:=False;
     ClearModifiedFlags;
end;

function TkbmCommon.IsAnyTableActive:boolean;
var
   i:TkbmNativeInt;
begin
     Result:=false;
     Lock;
     try
        for i:=0 to FAttachedTables.Count-1 do
        begin
             if (FAttachedTables.Items[i]<>nil) and (TkbmCustomMemTable(FAttachedTables.Items[i]).Active) then
             begin
                  Result:=true;
                  exit;
             end;
        end;
     finally
        Unlock;
     end;
end;

procedure TkbmCommon.CloseTables(Caller:TkbmCustomMemTable);
var
   i:TkbmNativeInt;
begin
     Lock;
     try
        for i:=FAttachedTables.count-1 downto 0 do
            if (FAttachedTables.Items[i]<>nil) and (Caller<>TkbmCustomMemTable(FAttachedTables.Items[i])) then
               with TkbmCustomMemTable(FAttachedTables.Items[i]) do Close;
     finally
        Unlock;
     end;
end;

procedure TkbmCommon.RefreshTables(Caller:TkbmCustomMemTable);
var
   i:TkbmNativeInt;
begin
     Lock;
     try
        for i:=0 to FAttachedTables.count-1 do
            if (FAttachedTables.Items[i]<>nil) and (Caller<>TkbmCustomMemTable(FAttachedTables.Items[i])) then
               with TkbmCustomMemTable(FAttachedTables.Items[i]) do
                    if Active and (State in [dsBrowse]) then Refresh;
     finally
            Unlock;
     end;
end;

procedure TkbmCommon.ResyncTables;
var
   i:TkbmNativeInt;
begin
     Lock;
     try
        for i:=0 to FAttachedTables.count-1 do
            if (FAttachedTables.Items[i]<>nil) then
                with TkbmCustomMemTable(FAttachedTables.Items[i]) do
                     if Active then Resync([]);
     finally
        UnLock;
     end;
end;

procedure TkbmCommon.EmptyTables;
var
   i:TkbmNativeInt;
begin
     Lock;
     try
        for i:=0 to FAttachedTables.count-1 do
            if (FAttachedTables.Items[i]<>nil) then
                with TkbmCustomMemTable(FAttachedTables.Items[i]) do
                     if Active then InternalEmptyTable;
        _InternalEmpty;
     finally
        UnLock;
     end;
end;

procedure TkbmCommon.RebuildIndexes;
var
   i:TkbmNativeInt;
begin
     Lock;
     try
        for i:=0 to FAttachedTables.Count-1 do
            if (FAttachedTables.Items[i]<>nil) then
                with TkbmCustomMemTable(FAttachedTables.Items[i]) do
                     if Active then Indexes.ReBuildAll;
     finally
        Unlock;
     end;
end;

procedure TkbmCommon.MarkIndexesDirty;
var
   i:TkbmNativeInt;
begin
     Lock;
     try
        for i:=0 to FAttachedTables.Count-1 do
            if (FAttachedTables.Items[i]<>nil) then
                with TkbmCustomMemTable(FAttachedTables.Items[i]) do
                     if Active then Indexes.MarkAllDirty;
     finally
        Unlock;
     end;
end;

procedure TkbmCommon.ClearIndexes;
var
   i:TkbmNativeInt;
begin
     Lock;
     try
        for i:=0 to FAttachedTables.Count-1 do
            if (FAttachedTables.Items[i]<>nil) then
                with TkbmCustomMemTable(FAttachedTables.Items[i]) do
                     if Active then
                        Indexes.EmptyAll;
     finally
        Unlock;
     end;
end;

procedure TkbmCommon.UpdateIndexes;
var
   i:TkbmNativeInt;
begin
     Lock;
     try
        for i:=0 to FAttachedTables.Count-1 do
            if (FAttachedTables.Items[i]<>nil) then
                with TkbmCustomMemTable(FAttachedTables.Items[i]) do
                     if Active then UpdateIndexes;
     finally
        Unlock;
     end;
end;

procedure TkbmCommon.ReflectToIndexes(const Caller:TkbmCustomMemTable; const How:TkbmIndexUpdateHow; const OldRecord,NewRecord:PkbmRecord; const RecordPos:integer; const DontVersion:boolean);
var
   i:TkbmNativeInt;
   mt:TkbmCustomMemTable;
   rp:integer;
begin
     Lock;
     try
        for i:=0 to FAttachedTables.Count-1 do
        begin
             mt:=TkbmCustomMemTable(FAttachedTables.Items[i]);
             if mt=nil then continue;
             if mt<>Caller then rp:=-1
             else rp:=RecordPos;
             if mt.Active then
                mt.Indexes.ReflectToIndexes(How,OldRecord,NewRecord,rp,DontVersion);
        end;
     finally
        Unlock;
     end;
end;

// -----------------------------------------------------------------------------------
// TkbmIndex
// -----------------------------------------------------------------------------------
constructor TkbmIndex.Create(ABase:TkbmIndex; Name:string; ADataSet:TkbmCustomMemtable; Fields:string; Options:TkbmMemTableCompareOptions; IndexType:TkbmIndexType);
var
   i:integer;
begin
     inherited Create;

     FDependantIndexes:=TkbmIndexes.Create(DataSet);
     FName:=Name;
     FBaseIndex:=ABase;
     FIndexFields:=Fields;
     FDataSet:=ADataSet;
     FType:=IndexType;
     FRowOrder:=false;
     FEnabled:=true;
     FFilterParser:=nil;
     FFilterFunc:=nil;
     FSorted:=(DataSet.FCommon.RecordCount<=0) and (IndexType=mtitSorted);
     FDirty:=(DataSet.FCommon.RecordCount>0);
     FUpdateStatus:=[usInserted,usModified,usUnmodified];

     FReferences:=TkbmList.Create;

     // Build list of fields in index, and check them for validity.
     FIndexFieldList:=TkbmFieldList.create;
     FIndexFieldList.Build(FDataSet,FIndexFields);

     // Check if valid fields to index on.
     for i:=0 to FIndexFieldList.Count-1 do
     begin
          if not (FIndexFieldList.Fields[i].DataType in kbmIndexableTypes) then
             raise EMemTableError.Create('TkbmIndex.Create: '+Format(kbminternalOpen2Err,[integer(FIndexFieldList.Fields[i].DataType)]));
     end;

     FIndexOptions:=Options;
     if (mtcoDescending in Options) then FIndexFieldList.SetOptions(FDataSet,mtifoDescending,FIndexFields);
     if (mtcoCaseInsensitive in Options) then FIndexFieldList.SetOptions(FDataSet,mtifoCaseInsensitive,FIndexFields);
     if (mtcoPartialKey in Options) then FIndexFieldList.SetOptions(FDataSet,mtifoPartial,FIndexFields);
     if (mtcoIgnoreLocale in Options) then FIndexFieldList.SetOptions(FDataSet,mtifoIgnoreLocale,FIndexFields);
     if (mtcoIgnoreNullKey in Options) then FIndexFieldList.SetOptions(FDataSet,mtifoIgnoreNull,FIndexFields);
     if (mtcoNullFirst in Options) then FIndexFieldList.SetOptions(FDataSet,mtifoNullFirst,FIndexFields);

     if ABase<>nil then
     begin
          ABase.DependantIndexes.AddIndex(self);
          FIndexFieldList.DefineAdditionalOrderFields(FDataSet,ABase.FIndexFieldList);
     end;
end;

constructor TkbmIndex.Create(ABase:TkbmIndex; IndexDef:TIndexDef; ADataSet:TkbmCustomMemtable);
var
   i:integer;
begin
     inherited Create;

     FDependantIndexes:=TkbmIndexes.Create(DataSet);
     FName:=IndexDef.Name;
     FBaseIndex:=ABase;
     FIndexFields:=IndexDef.Fields;
     FDataSet:=ADataSet;
     FType:=mtitSorted;
     FRowOrder:=false;
     FEnabled:=true;
     FFilterParser:=nil;
     FFilterFunc:=nil;
     FSorted:=DataSet.FCommon.RecordCount<=0;
     FDirty:=(DataSet.FCommon.RecordCount>0);
     FUpdateStatus:=[usInserted,usModified,usUnmodified];

     FReferences:=TkbmList.Create;

     // Build list of fields in index, and check them for validity.
     FIndexFieldList:=TkbmFieldList.create;
     FIndexFieldList.Build(FDataSet,IndexDef.Fields);

     // Check if valid fields to index on.
     for i:=0 to FIndexFieldList.Count-1 do
     begin
          if not (FIndexFieldList.Fields[i].DataType in kbmIndexableTypes) then
             raise EMemTableError.Create('TkbmIndex.Create: '+Format(kbminternalOpen2Err,[integer(FIndexFieldList.Fields[i].DataType)]));
     end;

     FIndexOptions:=IndexOptions2CompareOptions(IndexDef.Options);

     if (mtcoDescending in FIndexOptions) then FIndexFieldList.SetOptions(FDataSet,mtifoDescending,FIndexFields);
     if (mtcoCaseInsensitive in FIndexOptions) then FIndexFieldList.SetOptions(FDataSet,mtifoCaseInsensitive,FIndexFields);
     if (mtcoPartialKey in FIndexOptions) then FIndexFieldList.SetOptions(FDataSet,mtifoPartial,FIndexFields);
     if (mtcoIgnoreLocale in FIndexOptions) then FIndexFieldList.SetOptions(FDataSet,mtifoIgnoreLocale,FIndexFields);
     if (mtcoIgnoreNullKey in FIndexOptions) then FIndexFieldList.SetOptions(FDataSet,mtifoIgnoreNull,FIndexFields);
     if (mtcoNullFirst in FIndexOptions) then FIndexFieldList.SetOptions(FDataSet,mtifoNullFirst,FIndexFields);

     if ABase<>nil then
        ABase.DependantIndexes.AddIndex(self);
end;

destructor TkbmIndex.Destroy;
begin
     FBeingFreed:=true;

//OutputDebugString(PChar('Destroy index: '+Name));
     if FDataset<>nil then
        FDataset.IndexFreed(self);

     if FBaseIndex<>nil then
        FBaseIndex.DependantIndexes.DeleteIndex(self);
     FDependantIndexes.Clear;
     FDependantIndexes.Free;

     Clear;
     if FFilterParser<>nil then
     begin
          FFilterParser.Free;
          FFilterParser:=nil;
     end;
     FReferences.free;
     FIndexFieldList.free;
     SetRangeStartKey(nil);
     SetRangeEndKey(nil);
     inherited;
end;

procedure TkbmIndex.SetRangeStartKey(AKeyRecord:PkbmRecord);
begin
     if FRangeStartKey<>nil then
     begin
          FDataSet.FCommon._InternalFreeRecord(FRangeStartKey,true,true);
          FRangeStartKey:=nil;
     end;
     if AKeyRecord<>nil then
        FRangeStartKey:=FDataSet.FCommon._InternalCopyRecord(AKeyRecord,true);
     FIsRange:=(FRangeStartkey<>nil) or (FRangeEndKey<>nil);
end;

procedure TkbmIndex.SetRangeEndKey(AKeyRecord:PkbmRecord);
begin
     if FRangeEndKey<>nil then
     begin
          FDataSet.FCommon._InternalFreeRecord(FRangeEndKey,true,true);
          FRangeEndKey:=nil;
     end;
     if AKeyRecord<>nil then
        FRangeEndKey:=FDataSet.FCommon._InternalCopyRecord(AKeyRecord,true);
     FIsRange:=(FRangeStartkey<>nil) or (FRangeEndKey<>nil);
end;

function TkbmIndex.GetIsOrdered:boolean;
begin
     Result:=FSorted and (not FDirty);
end;

function TkbmIndex.GetIsFullScope:boolean;
begin
     Result:=not (FDirty or IsFiltered or IsRange);
end;

function TkbmIndex.Filter(const ARecord:PkbmRecord; const ACheckRange:boolean):boolean;
var
   OldOverride:PkbmRecord;
begin
     if ACheckRange and FIsRange then
     begin
          if (FRangeStartKey<>nil) and (CompareRecords(FIndexFieldList,FRangeStartKey,ARecord,true,false,false,false)>0) then
          begin
               Result:=false;
               exit;
          end;

          if (FRangeEndKey<>nil) and (CompareRecords(FIndexFieldList,FRangeEndKey,ARecord,true,false,false,false)<0) then
          begin
               Result:=false;
               exit;
          end;
     end;

     if not (IsFiltered or Assigned(FFilterFunc) or Assigned(FDataSet.OnFilterIndex)
            or Assigned(FFilterParser)
     ) then
     begin
          Result:=true;
          exit;
     end;

     OldOverride:=FDataSet.FOverrideActiveRecordBuffer;
     try
        FDataSet.FOverrideActiveRecordBuffer:=ARecord;

        // Call filtering function if defined.
        if Assigned(FFilterFunc) then
        begin
             FFilterFunc(FDataSet,self,Result);
             if not Result then exit;
        end;

        // Call users own filtering if specified.
        if Assigned(FDataSet.OnFilterIndex) then
        begin
             FDataSet.OnFilterIndex(FDataSet,self,Result);
             if not Result then exit;
        end;

        // Check if filterstring active.
        if Assigned(FFilterParser) then
        begin
             Result:=FDataSet.FilterExpression(ARecord,FFilterParser);
             if not Result then exit;
        end;
     finally
        FDataSet.FOverrideActiveRecordBuffer:=OldOverride;
     end;
end;

procedure TkbmIndex.SetEnabled(AValue:boolean);
begin
     FEnabled:=AValue;
     if FEnabled and FDirty then
        Rebuild;
end;

// Compare two arbitrary records for sort.
function TkbmIndex.CompareRecords(const AFieldList:TkbmFieldList;
    const AKeyRecord,ARecord:PkbmRecord;
    const APrimaryOnly:boolean;
    const ASortCompare,APartial,AUniqueConstraint:boolean): Integer;
const
     RetCodes: array[Boolean, Boolean] of ShortInt = ((2,-1),(1,0));
var
   n:integer;
begin
     with FDataSet do
     begin
          // Compare record contents.
          if APrimaryOnly then
             n:=AFieldList.PrimaryCount
          else
              n:=AFieldList.Count;
          Result:=FCommon._InternalCompareRecords(AFieldList,n,AKeyRecord,ARecord,false,APartial,AUniqueConstraint,chBreakNE);

          // Couldnt compare them according to fieldcontents, will now compare according to recnum.
          if (Result=0) and ASortCompare then
          begin
               Result:=RetCodes[AKeyRecord^.RecordNo>=0,ARecord^.RecordNo>=0];
               if Result=2 then
                  Result:=AKeyRecord.RecordID - ARecord.RecordID;

               // If descending sort on first field, invert Result.
               if (mtifoDescending in AFieldList.Options[0]) then Result:=-Result;

          end;
     end;
end;

// Binary search routine on Record ID index.
// Non-recursive function.
function TkbmIndex.BinarySearchRecordID(FirstNo,LastNo:TkbmNativeInt; const RecordID:TkbmNativeInt; const Desc:boolean; var Index:TkbmNativeInt):TkbmNativeInt;
var
   Mid:TkbmNativeInt;
   pRec:PkbmRecord;
begin
     Index:=-1;

     while FirstNo<=LastNo do
     begin
          // Look in the center of the interval.
          Mid:=(LastNo+FirstNo+1) div 2;
          pRec:=PkbmRecord(FReferences.Items[Mid]);

          // Compare records.
          Result:=RecordID - pRec^.RecordID;

          // If found exactly.
          if Result=0 then
          begin
               Index:=Mid;
               exit;
          end;

          if Desc then Result:=-Result;

          // Not matching, dig deeper.

          // If the key is smaller than the middle record, look in the lower half segment.
          if Result<0 then
             LastNo:=Mid-1
          else
              FirstNo:=Mid+1;
     end;
     Result:=0;
end;

// Enhanced non recursive binary search.
function TkbmIndex.BinarySearch(FieldList:TkbmFieldList; FirstNo,LastNo:TkbmNativeInt;
         const KeyRecord:PkbmRecord; const SearchType:TkbmSearchType;
         const PrimaryOnly:boolean; const RespectFilter:boolean;
         var Index:TkbmNativeInt; var Found:boolean):TkbmNativeInt;

   // Backtrack to first matching key from found key.
   // Does not respect filters.
   function LocateFirstMatchingKey(AIndex:TkbmNativeInt):integer;
   var
      i:TkbmNativeInt;
      r:integer;
      PRec:PkbmRecord;
   begin
        Result:=FirstNo;
        for i:=AIndex downto FirstNo do
        begin
             pRec:=PkbmRecord(FReferences.Items[i]);
             r:=CompareRecords(FieldList,KeyRecord,pRec,PrimaryOnly,false,false,false);
             if r>0 then
             begin
                  Result:=i+1;
                  break;
             end;
        end;
   end;

   // Forwardtrack to last matching key from found key.
   // Does not respect filters.
   function LocateLastMatchingKey(AIndex:TkbmNativeInt):integer;
   var
      i:TkbmNativeInt;
      r:integer;
      PRec:PkbmRecord;
   begin
        Result:=LastNo;
        for i:=AIndex to LastNo do
        begin
             pRec:=PkbmRecord(FReferences.Items[i]);
             r:=CompareRecords(FieldList,KeyRecord,pRec,PrimaryOnly,false,false,false);
             if r<0 then
             begin
                  Result:=i-1;
                  break;
             end;
        end;
   end;
var
   Mid:TkbmNativeInt;
   PRec:PkbmRecord;
begin
     Result:=0;
     Found:=false;

     if FieldList=nil then FieldList:=FIndexFieldList;
     while FirstNo<=LastNo do
     begin
          Mid:=(FirstNo+LastNo) div 2;
//OutputDebugString(PChar('BIN: FirstNo='+inttostr(FirstNo)+' Mid='+inttostr(Mid)+' LastNo='+inttostr(LastNo)));
          pRec:=PkbmRecord(FReferences.Items[Mid]);
          Result:=CompareRecords(FieldList,KeyRecord,pRec,PrimaryOnly,false,false,false);
//OutputDebugString(PChar('   : Result='+inttostr(Result)));
          if Result<0 then
             LastNo:=Mid-1
          else if Result>0 then
             FirstNo:=Mid+1
          else
          begin
               // Found a matching record.
               case SearchType of
                    mtstFirst,
                    mtstNearestBefore:
                      begin
                           // Scan for first match.
                           FirstNo:=LocateFirstMatchingKey(Mid);
                           break;
                      end;

                    mtstLast,
                    mtstNearestAfter:
                      begin
                           // Scan for last match.
                           LastNo:=LocateLastMatchingKey(Mid);
                           break;
                      end;
               end;
          end;
     end;
//OutputDebugString(PChar('BIN: Found='+inttostr(ord(Found))+' Result='+inttostr(Result)+' Index='+inttostr(Index)));

     // Respect filters.
     if FirstNo>LastNo then
     begin
          Index:=FirstNo;
          Found:=false;
     end
     else
         Result:=SequentialSearch(FieldList,FirstNo,LastNo,KeyRecord,SearchType,PrimaryOnly,RespectFilter,true,Index,Found);
end;

// Sequential search.
function TkbmIndex.SequentialSearch(FieldList:TkbmFieldList; const FirstNo,LastNo:TkbmNativeInt;
         const KeyRecord:PkbmRecord; const SearchType:TkbmSearchType;
         const PrimaryOnly:boolean; const RespectFilter:boolean;
         const SearchOrdered:boolean;
         var Index:TkbmNativeInt; var Found:boolean):TkbmNativeInt;
var
   i:TkbmNativeInt;
   pRec:PkbmRecord;
   partial:boolean;
begin
     if FieldList=nil then FieldList:=FIndexFieldList;
     Result:=0;
     Index:=-1;
     Found:=false;

     partial:=SearchType in [mtstNearestBefore,mtstNearestAfter];
     if SearchType in [mtstLast,mtstNearestAfter] then
     begin
          // Scan thru records in range in descending order.
          for i:=LastNo downto FirstNo do
          begin
               // Check if to recalc before compare.
               pRec:=PkbmRecord(FReferences.Items[i]);
               with FDataSet do
               begin
                    // Call progress function.
                    if (i mod 100) = 0 then
                      FDataSet.Progress(trunc((i-FirstNo)/(LastNo-FirstNo+1)*100),mtpcSearch);

                    if FRecalcOnIndex then
                    begin
                         // Fill calc fields part of buffer
                         __ClearCalcFields(pRec);
                         __GetCalcFields(pRec);
                    end;
               end;

//OutputDebugString(PChar('SEQREV: FirstNo='+inttostr(FirstNo)+' i='+inttostr(i)+' LastNo='+inttostr(LastNo)));
               // Check key record equal to record.
               Result:=CompareRecords(FieldList,KeyRecord,pRec,PrimaryOnly,false,partial,false);
//OutputDebugString(PChar('   REV: Result='+inttostr(Result)));

               // Check if found match but filtered.
               if (Result=0) and FDataset.IsFiltered and RespectFilter then
               begin
                    if not FDataset.FilterRecord(pRec,false) then continue;
               end;

               // Check if nearest or match.
               if (Result=0) or (partial and (Result>0)) then
               begin
                    Index:=i;
                    Found:=true;
                    exit;
               end;

               // Check if searching ordered index and no reason to continue search.
               if (Result>0) and IsOrdered then
               begin
                    Index:=i;
                    Found:=false;
                    exit;
               end;

          end;
          Index:=FirstNo;
     end
     else
     begin
          // Scan thru records in range in ascending order.
          for i:=FirstNo to LastNo do
          begin
               // Check if to recalc before compare.
               pRec:=PkbmRecord(FReferences.Items[i]);
               with FDataSet do
               begin
                    // Call progress function.
                    if (i mod 100) = 0 then
                      FDataSet.Progress(trunc((i-FirstNo)/(LastNo-FirstNo+1)*100),mtpcSearch);

                    if FRecalcOnIndex then
                    begin
                         // Fill calc fields part of buffer
                         __ClearCalcFields(pRec);
                         __GetCalcFields(pRec);
                    end;
               end;

//OutputDebugString(PChar('SEQFWD: FirstNo='+inttostr(FirstNo)+' i='+inttostr(i)+' LastNo='+inttostr(LastNo)));
               // Check key record equal to record.
               Result:=CompareRecords(FieldList,KeyRecord,pRec,PrimaryOnly,false,partial,false);
//OutputDebugString(PChar('   FWD: Result='+inttostr(Result)));

               // Check if found match but filtered.
               if (Result=0) and FDataset.IsFiltered and RespectFilter then
               begin
                    if not FDataset.FilterRecord(pRec,false) then continue;
               end;

               // Check if nearest or match.
               if (Result=0) or (partial and (Result<0)) then
               begin
                    Index:=i;
                    Found:=true;
                    exit;
               end;

               // Check if searching ordered index and no reason to continue search.
               if (Result<0) and SearchOrdered then
               begin
                    Index:=i;
                    Found:=false;
                    exit;
               end;

          end;
          Index:=LastNo;
     end;
//OutputDebugString(PChar('SEQ: Found='+inttostr(ord(Found))+' Result='+inttostr(Result)+' Index='+inttostr(Index)));
end;

// Sequential search for record ID.
function TkbmIndex.SequentialSearchRecordID(const FirstNo,LastNo:TkbmNativeInt; const RecordID:TkbmNativeInt; var Index:TkbmNativeInt):TkbmNativeInt;
var
   i:TkbmNativeInt;
   pRec:PkbmRecord;
begin
     // Loop for all records.
     Result:=0;
     Index:=-1;
     for i:=FirstNo to LastNo do
     begin
          // Call progress function.
          if (i mod 100) = 0 then
            FDataSet.Progress(trunc((i-FirstNo)/(LastNo-FirstNo+1)*100),mtpcSearch);

          pRec:=PkbmRecord(FReferences.Items[i]);
          Result:=RecordID - pRec^.RecordID;

          if (Result=0) then
          begin
               Index:=i;
               exit;
          end;
     end;
end;

// Search.
// Aut. choose between indexed seq. search and indexed binary search.
function TkbmIndex.Search(FieldList:TkbmFieldList; KeyRecord:PkbmRecord; const SearchType:TkbmSearchType; const PrimaryOnly,RespectFilter:boolean; var Index:TkbmNativeInt; var Found:boolean):TkbmNativeInt;
var
   n:TkbmNativeInt;
begin
     Index:=-1;

     // Lock the record list for our use, to make sure nobody alters it.
     FDataSet.Progress(0,mtpcSearch);
     FDataSet.FState:=mtstSearch;
     FDataSet.FCommon.Lock;
     try
        // Utilize best search method.
        n:=FReferences.Count;
        if n<=0 then
           Result:=0
        else if FSorted and (not FRowOrder) then
            Result:=BinarySearch(FieldList,0,n-1,KeyRecord,SearchType,PrimaryOnly,RespectFilter,Index,Found)
        else
           Result:=SequentialSearch(FieldList,0,n-1,KeyRecord,SearchType,PrimaryOnly,RespectFilter,false,Index,Found);
     finally
        FDataSet.FCommon.Unlock;
        FDataSet.Progress(100,mtpcSearch);
        FDataSet.FState:=mtstBrowse;
     end;
end;

// Search for specific record in index.
function TkbmIndex.SearchRecord(KeyRecord:PkbmRecord; var Index:TkbmNativeInt; const PrimaryOnly,RespectFilter:boolean):TkbmNativeInt;
var
   First,Last:TkbmNativeInt;
   i:TkbmNativeInt;
   Found:boolean;
begin
     Index:=-1;
     Result:=0;

     // Lock the record list for our use, to make sure nobody alters it.
     FDataSet.FCommon.Lock;
     FDataSet.Progress(0,mtpcSearch);
     try
        // Check if anything to search.
        if (FReferences.count>0) then
        begin
             // Assume whole range.
             First:=0;
             Last:=FReferences.count-1;

             // Try to minimize the sequential scan for record.
             if FRowOrder or (FSorted and (FReferences.Count>5)) then
             begin
                  i:=-1;
                  if FRowOrder then
                  begin
                       SearchRecordID(KeyRecord^.RecordID,Index);
                       if Index>=0 then exit;
                  end
                  else
                     BinarySearch(nil,0,FReferences.Count-1,KeyRecord,mtstFirst,PrimaryOnly,RespectFilter,i,Found);
                  if Found and (i>=0) then First:=i;
             end;

             // Sequential scan for correct record id from that point.
             SequentialSearchRecordID(First,Last,KeyRecord^.RecordID,Index);
        end;
     finally
        FDataSet.FCommon.Unlock;
     end;
end;

// Search for specific record ID in row order index only.
function TkbmIndex.SearchRecordID(RecordID:TkbmNativeInt; var Index:TkbmNativeInt):TkbmNativeInt;
begin
     // Try to look for it by binary search.
     // If records are inserted here and there in the index, they will not be sorted
     // as the roworderindex indicates the order the user has put the records in
     // using append and insert. But as a good guess, there should be a good chance
     // of finding a record by a binary search. If it wasnt found, we will try again
     // using a sequential search to be on the safe side.
     Index:=-1;
     Result:=0; // To fix bogus warning from compiler.

     // Lock the record list for our use, to make sure nobody alters it.
     FDataSet.FCommon.Lock;
     try
        if FRowOrder then
        begin
             Result:=BinarySearchRecordID(0,FReferences.Count-1,RecordID,false,Index);
             if Index<0 then
                Result:=BinarySearchRecordID(0,FReferences.Count-1,RecordID,true,Index);
        end;

        if Index<0 then
            Result:=SequentialSearchRecordID(0,FReferences.Count-1,RecordID,Index);
     finally
        FDataSet.FCommon.Unlock;
     end;
end;

function TkbmIndex.IsDependingOn(AIndex:TkbmIndex):boolean;
var
   i:TkbmIndex;
begin
     if AIndex=self then
     begin
          Result:=true;
          exit;
     end;

     i:=self;
     while (i<>nil) and (i.Base<>nil) do
     begin
          if i=AIndex then
          begin
               Result:=true;
               exit;
          end;
          i:=i.Base;
     end;

     Result:=false;
end;

// Routines used by FastQuicksort.
procedure TkbmIndex.InternalSwap(const I,J:integer);
var
   t:PkbmRecord;
begin
     t:=FReferences[I];
     FReferences[I]:=FReferences[J];
     FReferences[J]:=t;
end;

procedure TkbmIndex.InternalFastQuickSort(const L,R:Integer);
var
   I,J:integer;
   P:PkbmRecord;
begin
     if ((R-L)>4) then
//     if ((R-L)>0) then
     begin
          I:=(R+L) div 2;
          if CompareRecords(FIndexFieldList,PkbmRecord(FReferences[L]),PkbmRecord(FReferences[I]),false,true,false,false)>0 then
           InternalSwap(L,I);
          if CompareRecords(FIndexFieldList,PkbmRecord(FReferences[L]),PkbmRecord(FReferences[R]),false,true,false,false)>0 then
           InternalSwap(L,R);
          if CompareRecords(FIndexFieldList,PkbmRecord(FReferences[I]),PkbmRecord(FReferences[R]),false,true,false,false)>0 then
           InternalSwap(I,R);

          J:=R-1;
          InternalSwap(I,J);
          I:=L;
          P:=PkbmRecord(FReferences[J]);
          while true do
          begin
               Inc(I);
               Dec(J);
               while (i<=j) and (CompareRecords(FIndexFieldList,PkbmRecord(FReferences[I]),P,false,true,false,false) < 0) do Inc(I);
               while (j>=i) and (CompareRecords(FIndexFieldList,PkbmRecord(FReferences[J]),P,false,true,false,false) > 0) do Dec(J);
               if (J<=I) then break;
               InternalSwap(I,J);
          end;
          InternalSwap(I,R-1);
          InternalFastQuickSort(L,J);
          InternalFastQuickSort(I+1,R);
     end;
end;

procedure TkbmIndex.InternalInsertionSort(const Lo,Hi:integer);
var
   I,J:TkbmNativeInt;
   P:PkbmRecord;
begin
     for I:=Lo+1 to Hi do
     begin
          P:=PkbmRecord(FReferences.Items[I]);
          J:=I;
          while ((J>Lo) and (CompareRecords(FIndexFieldList,PkbmRecord(FReferences[J-1]),P,false,true,false,false)>0)) do
          begin
               FReferences[J]:=FReferences[J-1];
               dec(J);
          end;
          FReferences[J]:=P;
     end;
end;

// Sort the record refences using the Fast Quicksort algorithm.
procedure TkbmIndex.FastQuickSort(const L,R:Integer);
begin
     InternalFastQuickSort(L,R);
     InternalInsertionSort(L,R);
     FSorted:=true;
end;

procedure TkbmIndex.Clear;
begin
     FReferences.Clear;
     FSorted:=IsRowOrder;
     FDirty:=true;
end;

function TkbmIndex.FindRecordNumber(const ARecord:PkbmRecord):integer;
var
   i:TkbmNativeInt;
begin
     for i:=0 to FReferences.Count-1 do
         if PkbmRecord(FReferences[i])=ARecord then
         begin
              Result:=i;
              exit;
         end;
     Result:=-1;
end;

procedure TkbmIndex.Load;
var
   i:TkbmNativeInt;
   found:boolean;
   iFirst,iLast:TkbmNativeInt;
   bRangeFilter:boolean;
   bRangeStartFound:boolean;
   bRangeEndFound:boolean;
   p:PkbmRecord;
   lst:TkbmList;
begin
     Clear;

     FSorted:=false;
     FDirty:=true;

     FDataSet.FCommon.Lock;
     try
        // Check if based on original data or another index.
        // If another index, then check if sorted and range enabled, locate first/last for range.
        bRangeStartFound:=false;
        bRangeEndFound:=false;
        if FBaseIndex<>nil then
        begin
             lst:=FBaseIndex.FReferences;
             if FIsRange and FBaseIndex.IsOrdered and (FBaseIndex.IndexFieldList.StartsWith(IndexFieldList,true,true)) then
             begin
                  if FRangeStartKey<>nil then
                  begin
                       FBaseIndex.BinarySearch(FIndexFieldList,0,lst.Count-1,
                                               FRangeStartKey,mtstFirst,
                                               true,false,iFirst,found);
                       bRangeStartFound:=found;
                       if not found then
                          iFirst:=0;
                  end
                  else
                      iFirst:=0;

                  if FRangeEndKey<>nil then
                  begin
                       FBaseIndex.BinarySearch(FIndexFieldList,0,lst.Count-1,
                                               FRangeEndKey,mtstLast,
                                               true,false,iLast,found);
                       bRangeEndFound:=found;
                       if not found then
                          iLast:=lst.Count-1;
                  end
                  else
                      iLast:=lst.Count-1;
             end
             else
             begin
                  lst:=FBaseIndex.FReferences;
                  iFirst:=0;
                  iLast:=lst.Count-1;
             end;
        end
        else
        begin
             lst:=FDataSet.FCommon.FRecords;
             iFirst:=0;
             iLast:=lst.Count-1;
        end;
        bRangeFilter:=not (bRangeStartFound and bRangeEndFound);

        // Set capacity for references and recnolist.
        if not IsFiltered then
           FReferences.Capacity:=lst.Count
        else
            FReferences.Capacity:=100;

        // Add the references.
        for i:=iFirst to iLast do
        begin
             p:=PkbmRecord(lst.Items[i]);
             if p<>nil then
             begin
                  if not (p^.UpdateStatus in FUpdateStatus) or
                     ((p^.UpdateStatus=usDeleted) and (not FDataSet.EnableVersioning)) then
                     continue;
                  if not Filter(p,bRangeFilter) then
                     continue;
                  FReferences.Add(p);
             end;
        end;
     finally
        FDataSet.FCommon.Unlock;
     end;
end;

procedure TkbmIndex.ReSort;
var
   i:TkbmNativeInt;
begin
     // If not sorted, dont bother to sort the index.
     if FType=mtitNonSorted then
     begin
          if IsRowOrder then
             FSorted:=true;
          exit;
     end;

     // Lock the record list for our use, to make sure nobody alters it.
     FDataSet.Progress(0,mtpcSort);
     FDataSet.FState:=mtstSort;
     FDataSet.FCommon.Lock;
     try
        // Sort the index.
        FastQuickSort(0,FReferences.Count-1);

        // If unique index, look for duplicates.
        if mtcoUnique in FIndexOptions then
           for i:=1 to FReferences.Count-1 do
           begin
                if (CompareRecords(FIndexFieldList,FReferences[i-1],
                                                   FReferences[i],
                                                   true,false,false,true)=0) then
                   raise EMemTableDupKey.Create(Name+' : '+kbmDupIndex);
           end;
     finally
        FDataSet.FCommon.Unlock;
        FDataSet.Progress(100,mtpcSort);
        FDataSet.FState:=mtstBrowse;
     end;
end;

procedure TkbmIndex.Rebuild;
begin
     if FDataset.Active then
     begin
          Load;
          if FReferences.Count>0 then
             ReSort;
          FSorted:=FType=mtitSorted;
     end;
     FSorted:=(FType=mtitSorted);
     FDirty:=false;

     if (FDataSet.FCurIndex=self) and (FDataSet.FRecNo>=FReferences.Count) then
        FDataSet.FRecNo:=FReferences.Count-1;
end;

procedure TkbmIndex.RebuildDepending;
begin
     FDependantIndexes.ReBuildAll;
end;

// -----------------------------------------------------------------------------------
// TkbmIndexes
// -----------------------------------------------------------------------------------
constructor TkbmIndexes.Create(ADataSet:TkbmCustomMemTable);
begin
     inherited Create;
     FIndexes:=TStringList.Create;
     FDataSet:=ADataSet;
     FRowOrderIndex:=nil;
end;

destructor TkbmIndexes.Destroy;
begin
     if FRowOrderIndex<>nil then
        FRowOrderIndex.Free; // It will destroy all other indexes.

     FIndexes.free;
     inherited;
end;

// Remove indexes.
procedure TkbmIndexes.Clear;
var
   i:TkbmNativeInt;
   lIndex:TkbmIndex;
begin
     FInClear:=true;
     try
        for i:=FIndexes.Count-1 downto 0 do
        begin
             lIndex:=TkbmIndex(FIndexes.Objects[i]);
             if lIndex = FRowOrderIndex then
                lIndex.Clear
             else
             begin
                  FIndexes.Delete(i);
                  if not lIndex.FBeingFreed then
                     lIndex.Free;
             end;
        end;
     finally
        FInClear:=false;
     end;
end;

function TkbmIndexes.Count:integer;
begin
     Result:=FIndexes.Count;
end;

procedure TkbmIndexes.IndexFreed(AIndex:TkbmIndex);
begin
     if FInClear then
        exit;

     DeleteIndex(AIndex);
end;

function TkbmIndexes.Get(const IndexName:string):TkbmIndex;
var
   i:TkbmNativeInt;
   lIndex:TkbmIndex;
begin
     for i:=0 to FIndexes.Count-1 do
     begin
          lIndex:=TkbmIndex(FIndexes.Objects[i]);
          if (UpperCase(lIndex.FName) = UpperCase(IndexName)) then
          begin
               Result:=lIndex;
               exit;
          end;
     end;
     Result:=nil;
end;

function TkbmIndexes.GetIndex(const Ordinal:integer):TkbmIndex;
begin
     Result:=nil;
     if (Ordinal<0) or (Ordinal>=FIndexes.Count) then exit;
     Result:=TkbmIndex(FIndexes.Objects[Ordinal]);
end;

// Lookup first index on specified fieldnames.
function TkbmIndexes.GetByFieldNames(FieldNames:string):TkbmIndex;
var
   i:TkbmNativeInt;
   lIndex:TkbmIndex;
begin
     Result:=nil;
     FieldNames:=UpperCase(FieldNames);
     for i:=0 to FIndexes.count-1 do
     begin
         lIndex:=TkbmIndex(FIndexes.Objects[i]);
         if (UpperCase(lIndex.FIndexFields) = FieldNames) then
         begin
              Result:=lIndex;
              break;
         end;
     end;
end;

procedure TkbmIndexes.AddIndex(const Index:TkbmIndex);
begin
     FIndexes.AddObject(Index.FName,Index);
end;

procedure TkbmIndexes.Add(const IndexDef:TIndexDef);
var
   lIndex:TkbmIndex;
begin
     if (IndexDef.Fields='') then
        raise EMemTableError.Create(kbmMissingNames);

     lIndex:=TkbmIndex.Create(FRowOrderIndex,IndexDef,FDataSet);
     AddIndex(lIndex);
end;

procedure TkbmIndexes.DeleteIndex(const Index:TkbmIndex);
var
   iIndex:integer;
   lIndex:TkbmIndex;
begin
     // Dont allow deletion of roworder index.
     if Index=FRowOrderIndex then exit;

     for iIndex:=0 to FIndexes.count-1 do
     begin
          lIndex:=TkbmIndex(FIndexes.Objects[iIndex]);
          if lIndex=Index then
          begin
               FIndexes.delete(iIndex);
               if not lIndex.FBeingFreed then
                  FreeAndNil(lIndex);
               break;
          end;
     end;
end;

procedure TkbmIndexes.Delete(const IndexName:string);
var
   lIndex:TkbmIndex;
begin
     lIndex:=Get(IndexName);
     DeleteIndex(lIndex);
end;

procedure TkbmIndexes.Empty(const IndexName:string);
var
   lIndex:TkbmIndex;
begin
     // Get reference to index reference list.
     lIndex:=Get(IndexName);
     lIndex.Clear;
end;

procedure TkbmIndexes.EmptyAll;
var
   i:TkbmNativeInt;
   lIndex:TkbmIndex;
begin
     for i:=0 to FIndexes.Count-1 do
     begin
          lIndex:=TkbmIndex(FIndexes.Objects[i]);
          lIndex.Clear;
     end;
end;

procedure TkbmIndexes.ReBuild(const IndexName:string);
var
   lIndex:TkbmIndex;
begin
     // Get reference to index reference list.
     if not FDataSet.Active then exit;
     lIndex:=Get(IndexName);
     lIndex.Rebuild;


     if not FDataSet.Active then exit;
     lIndex:=Get(IndexName);
     if lIndex = nil then raise EMemTableInvalidLocale.Create(kbmInvalidLocale);
     lIndex.Rebuild;
end;

procedure TkbmIndexes.ReBuildAll;
var
   i:TkbmNativeInt;
   lIndex:TkbmIndex;
begin
     for i:=0 to FIndexes.Count-1 do
     begin
          lIndex:=TkbmIndex(FIndexes.Objects[i]);
          lIndex.Rebuild;
     end;
end;

procedure TkbmIndexes.MarkAllDirty;
var
   iIndex:integer;
   lIndex:TkbmIndex;
begin
     for iIndex:=0 to FIndexes.Count-1 do
     begin
          lIndex:=TkbmIndex(FIndexes.Objects[iIndex]);
          lIndex.FSorted:=false;
          lIndex.FDirty:=true;
     end;
end;

// Search for keyrecord on specified fields.
// Aut. selects the optimal search method depending if an index is available.
// CurIndex is a reference to the current index, that is the one the Index value will refer
// to as a Result.
function TkbmIndexes.Search(const FieldList:TkbmFieldList; const KeyRecord:PkbmRecord;
    const SearchType:TkbmSearchType;
    const PrimaryOnly,RespectFilter,AutoAddIdx:boolean;
    var Index:TkbmNativeInt;
    var Found:boolean):TkbmNativeInt;
var
   i:TkbmNativeInt;
   s:string;
   idxFound:boolean;
   ix:TkbmIndex;
   fl:TkbmFieldList;
   co:TkbmMemTableCompareOptions;
begin
     // Check for index to search on.
     Index:=-1;
     Found:=false;
     Result:=0;

     // Create merger of the options of the given and the index field list and use it.
     fl:=TkbmFieldList.Create;
     try
        if FDataSet.FCurIndex.IsOrdered then
        begin
             // Look for an index which can be used.
             idxFound:=false;
             for i:=FIndexes.Count-1 downto 0 do
                 if TkbmIndex(FIndexes.Objects[i]).IsOrdered and
                    TkbmIndex(FIndexes.Objects[i]).FIndexFieldList.StartsWith(FieldList,true,true) then
                 begin
                      idxFound:=true;
                      break;
                 end;

             // If no index found, optionally add one.
             if not idxFound and AutoAddIdx then
             begin
                  // Build new string of fieldnames for new index.
                  s:=FieldList.Fields[0].FieldName;
                  for i:=1 to FieldList.Count-1 do
                      s:=s+';'+FieldList.Fields[i].FieldName;

                  // Add new index.
                  co:=[];
                  if loCaseInsensitive in FieldList.LocateOptions then
                     co:=co+[mtcoCaseInsensitive];

                  ix:=TkbmIndex.Create(FDataSet.CurIndex,kbmAutoIndex+s,FDataSet,s,co,mtitSorted);
                  AddIndex(ix);
                  ix.Rebuild;
                  idxFound:=true;
                  i:=FIndexes.Count-1;
             end;

             // If found an index to search on.
             if idxFound then
             begin
                  ix:=TkbmIndex(FIndexes.Objects[i]);

                  // Merge two fieldlists options.
                  FieldList.AssignTo(fl);
                  ix.FIndexFieldList.MergeOptionsTo(fl);

                  //ix.FIndexFieldList.AssignTo(fl);
                  //FieldList.MergeOptionsTo(fl);

                  // Search.
                  Result:=ix.Search(fl,KeyRecord,SearchType,PrimaryOnly,RespectFilter,Index,Found);

                  // Check if found record.
                  if Found then
                  begin
                       // Check if it wasnt current index that was searched. Then have to research on current.
                       if FDataSet.FCurIndex<>ix then
                       begin
                            // Do 2nd search.
                            Result:=FDataSet.FCurIndex.SearchRecord(ix.References.Items[Index],Index,PrimaryOnly,RespectFilter);
                            if Index>=ix.FReferences.Count then
                               Found:=false;
                       end;
                  end
                  else
                      Found:=false;
                  exit;
             end;
        end;

        // No compatible indexes found, do a sequential search on current index.
        with FDataSet.FCurIndex do
        begin
             // Use given fieldlist as base instead of index field list. Then merge indexfieldlist options in.
             FieldList.AssignTo(fl);
             FDataSet.FCurIndex.SequentialSearch(fl,0,FReferences.Count-1,KeyRecord,SearchType,PrimaryOnly,RespectFilter,false,Index,Found);
        end;
     finally
        fl.Free;
     end;
end;

// Check a record for acceptance regarding indexdefinitions.
procedure TkbmIndexes.CheckRecordUniqueness(const ARecord,ActualRecord:PkbmRecord);
var
   i:TkbmNativeInt;
   iResult:TkbmNativeInt;
   lIndex:TkbmIndex;
   Found:boolean;
begin
     // If indexes not enabled, dont make uniqueness test.
     if not FDataSet.FEnableIndexes then exit;

     // Check all indexes for uniqueness.
     for i:=0 to FIndexes.Count-1 do
     begin
          lIndex:=TkbmIndex(FIndexes.Objects[i]);
          with lIndex do
          begin
               if not FEnabled then continue;
               if (mtcoNonMaintained in FIndexOptions) then continue;

               // Check if unique index and duplicate key, complain.
               if (mtcoUnique in FIndexOptions)
                  and ((Search(nil,ARecord,mtstFirst,true,false,iResult,Found)=0) and Found and (iResult>=0))
                  and (lIndex.FReferences[iResult] <> ActualRecord) then
                  raise EMemTableDupKey.Create(lIndex.Name+' : '+kbmDupIndex);
          end;
     end;
end;

// Call this during insert, append, edit or delete of record to update the index lists.
// OldRecord contains a reference to the actual 'physical' record.
// NewRecord points to a buffer which contains the new value soon to be copied into OldRecord. (Edit)
// NewRecord is nil on append or insert since OldRecord allready will contain values.
// RecordPos specifies current FRecNo.
// Returns pos in current index for operation.
procedure TkbmIndexes.ReflectToIndexes(const How:TkbmIndexUpdateHow;
     const OldRecord,NewRecord:PkbmRecord;
     const RecordPos:TkbmNativeInt;
     const DontVersion:boolean);
var
   i,ni:TkbmNativeInt;
   iInsert,iDelete:TkbmNativeInt;
   lIndex:TkbmIndex;
   IsRowOrderIndex,IsCurIndex,IsIndexesEnabled:boolean;
   DoAppend:boolean;
   Found:boolean;
   n,m:integer;
   st:TkbmSearchType;
begin
     // Loop through all indexes.
     FDataset.FReposRecNo:=-1;
     ni:=FIndexes.Count-1;
     for i:=0 to ni do
     begin
          lIndex:=TkbmIndex(FIndexes.Objects[i]);

          IsRowOrderIndex:=(lIndex = FRowOrderIndex);
          IsCurIndex:=(lIndex = FDataSet.FCurIndex);
          IsIndexesEnabled:=FDataset.FEnableIndexes;

          with lIndex do
          begin
               // Check if to skip updating this index. Never skip if deleting on the current index.
               if (mtcoNonMaintained in FIndexOptions) or
                  ((not (IsIndexesEnabled and lIndex.Enabled)) and ((How<>mtiuhDelete) or (not IsCurIndex))) then
               begin
                    if (How<>mtiuhEdit) or (not IsRowOrder) then
                    begin
                         FSorted:=false;
                         FDirty:=true;
                    end;
                    continue;
               end;

               // If its a sorted index, it will be ordered after this unless it was unordered from start.
               FSorted:=(FType=mtitSorted);

               // Check how to update index.
               case How of
                    mtiuhEdit:
                       begin
                            // Is it the roworder index? Dont do anything since it wont change anything.
                            if IsRowOrderIndex then continue;

                            // Do not update indexes if key has not changed unless updatestatus filtering is enabled.
                            if (usModified in FUpdateStatus) and (lIndex.CompareRecords(lIndex.FIndexFieldList,OldRecord,NewRecord,false,false,false,false)=0) then
                            begin
                                 if IsCurIndex then FDataset.FReposRecNo:=NewRecord.RecordNo;
                                 continue;
                            end;

                            // Search the original position.
                            // Is it the current index, then use FRecNo, otherwise look for it.
                            if IsCurIndex and (RecordPos>=0) then
                               iDelete:=RecordPos
                            else
                            begin
                                 iDelete:=-1;
                                 SearchRecord(OldRecord,iDelete,false,false);
                            end;
                            // Check if didnt find original record in index, dont try to delete it.
                            // Situation can occur if several tables are attached together and
                            // one of the tables have filtered indexes, but the other not.
                            // Then changing a value in the non filtered index on one table will
                            // Result in a request to update the filtered index on the other table.
                            // And in that table, the record wont be found because it wasnt there in
                            // the first place because of the filter.
                            if iDelete>=0 then
                               FReferences.Delete(iDelete);

                            // Check if filtering.
                            if not Filter(NewRecord,true) then continue;

                            // Check if modified not allowed by updatestatus filter.
                            if not (usModified in FUpdateStatus) then continue;

                            // If any references left, look for nearest insertion place.
                            iInsert:=-1;
                            Search(nil,NewRecord,mtstFirst,false,false,iInsert,Found);

                            // If found insertion place, insert.
                            if iInsert>=0 then
                               // Insert the reference at new place.
                               FReferences.Insert(iInsert,OldRecord)
                            else
                               // Add reference to list.
                               iInsert:=FReferences.Add(OldRecord);

                            if IsCurIndex then
                            begin
                                 FDataset.FReposRecNo:=iInsert;
                                 OldRecord^.RecordNo:=iInsert;
                            end;
                       end;

                    mtiuhInsert,
                    mtiuhAppend:
                       begin
                            // Check if filtering.
                            if not Filter(NewRecord,true) then continue;

                            // Check if inserted not allowed by updatestatus filter.
                            if not (usInserted in FUpdateStatus) then continue;

                            // Is it the roworder index? Is it the same as the one we are looking at?
                            if IsRowOrderIndex then
                            begin
                                 if IsCurIndex then
                                 begin
                                      if RecordPos<0 then
                                         DoAppend:=true
                                      else
                                      begin
                                           iInsert:=RecordPos-1;
                                           DoAppend:=false;
                                      end;
                                 end
                                 else
                                 begin
                                      iInsert:=-1;
                                      DoAppend:=true;
                                 end;
                            end
                            else
                            begin
                                 n:=FReferences.Count;
                                 if n>0 then
                                 begin
                                      iInsert:=-1;
                                      if How=mtiuhAppend then
                                         st:=mtstLast
                                      else
                                          st:=mtstFirst;
                                      m:=Search(nil,NewRecord,st,false,false,iInsert,Found);
                                      DoAppend:=((m>0) or ((m=0) and (How=mtiuhAppend))) and ((iInsert<0) or (iInsert>=n-1))
                                 end
                                 else
                                     DoAppend:=true;
                            end;

                            // Figure out if to append or to insert to index.
                            if DoAppend then
                            begin
                                 // Append reference to index.
                                 iInsert:=FReferences.Add(NewRecord);
                            end
                            else
                            begin
                                 // Insert reference.
                                 if iInsert<0 then iInsert:=0;
                                 FReferences.Insert(iInsert,NewRecord);
                            end;

                            if IsCurIndex then
                            begin
                                 FDataset.FReposRecNo:=iInsert;
                                 NewRecord^.RecordNo:=iInsert;
                            end;
                       end;

                    mtiuhDelete:
                       begin
                            // Check if to leave deleted record in index (only a point if versioning.
                            if (usDeleted in FUpdateStatus) and (FDataSet.EnableVersioning) and (not DontVersion) then
                            begin
                                 // Check if this index contained original record or not.
                                 // If it didnt, we have to insert it now.

                                 if not (OldRecord^.UpdateStatus in FUpdateStatus) then
                                 begin

                                      // Check record ok due to other filters.
                                      if Filter(OldRecord,true) then
                                      begin
                                           // Figure out where to place record in index.
                                           n:=FReferences.Count;
                                           if n>0 then
                                           begin
                                                iInsert:=-1;
                                                m:=Search(nil,OldRecord,mtstFirst,false,false,iInsert,Found);
                                                DoAppend:=(m>0) and (iInsert>=n-1)
                                           end
                                           else
                                               DoAppend:=true;

                                           // Figure out if to append or to insert to index.
                                           if DoAppend then
                                           begin
                                                // Append reference to index.
                                                iInsert:=FReferences.Add(OldRecord);
                                           end
                                           else
                                           begin
                                                // Insert reference.
                                                if iInsert<0 then iInsert:=0;
                                                FReferences.Insert(iInsert,OldRecord);
                                           end;

                                           if IsCurIndex then
                                           begin
                                                FDataset.FReposRecNo:=iInsert;
                                                OldRecord^.RecordNo:=iInsert;
                                           end;
                                      end;
                                 end;
                                 // Dont remove it from this index.
                                 continue;
                            end;

                            if FReferences.Count>0 then
                            begin
                                 iDelete:=-1;

                                 // Is it the roworder index?
                                 if IsRowOrderIndex then
                                    SearchRecordID(OldRecord^.RecordID,iDelete)
                                 else
                                     SearchRecord(OldRecord,iDelete,false,false);
                                 if iDelete>=0 then
                                    FReferences.Delete(iDelete);
                                 if IsCurIndex then FDataset.FReposRecNo:=iDelete;
                            end;
                       end;
               end;
          end;
     end;
end;

// -----------------------------------------------------------------------------------
// TkbmMasterDataLink
// -----------------------------------------------------------------------------------

procedure TkbmMasterDataLink.RecordChanged(Field:TField);
begin
     // Dont refresh if editing fields not related to the M/D join.
     if Assigned(OnMasterChange) then
     begin
          case DataSource.State of
              dsEdit: if (Fields.IndexOf(Field)>=0) then
                         OnMasterChange(Self);
              dsSetKey: ; // Do nothing.
          else
              inherited RecordChanged(Field);
          end;
     end;
end;


// -----------------------------------------------------------------------------------
// TkbmCustomMemTable
// -----------------------------------------------------------------------------------

class function TkbmCustomMemTable.GetMasterDataLinkClass:TkbmMasterDataLinkClass;
begin
     Result:=TkbmMasterDataLink;
end;

constructor TkbmCustomMemTable.Create(AOwner: TComponent);
begin
     inherited Create(AOwner);

{$IFDEF MSWINDOWS}
 {$IFDEF LEVEL16}
     FFormatSettings:=TFormatSettings.Create(LOCALE_SYSTEM_DEFAULT);
 {$ELSE}
     GetLocaleFormatSettings(LOCALE_SYSTEM_DEFAULT, FFormatSettings);
 {$ENDIF}
{$ELSE}
 {$IFDEF LEVEL16}
     FFormatSettings:=TFormatSettings.Create;
 {$ENDIF}
{$ENDIF}

     FTableID:=0;
     FCommon:=TkbmCommon.Create(self);

     // Now attach to the common tablestructure.
     FCommon.AttachTable(self);

     // Create indexlist.
     FIndexes:=TkbmIndexes.Create(self);

     // Add row order index to indexlist.
     with FIndexes do
     begin
          FRowOrderIndex:=TkbmIndex.Create(nil,kbmRowOrderIndex,self,'',[],mtitNonSorted);
          FRowOrderIndex.FRowOrder:=true;
          AddIndex(FRowOrderIndex);
     end;

     // Default all load/save operations should load/save all records.
     FLoadLimit:=-1;
     FLoadCount:=-1;
     FLoadedCompletely:=false;
     FSaveLimit:=-1;
     FSaveCount:=-1;
     FSavedCompletely:=false;

     // Suppose standalone table.
     // If FAttachedTo points to another memtable, FRecords and FDeletedRecords will point
     // to the other tables FRecords and FDeletedRecords.
//     FAttachedChildren:=TThreadList.Create;
     FAttachedTo:=nil;
     FAttachedAutoRefresh:=true;

     FAutoReposition:=false;
     FDesignActivation:=true;

     FRecNo:=-1;

     FPersistent:=false;
     FRecalcOnIndex:=false;
     FRecalcOnFetch:=true;
     FRangeIgnoreNullKeyValues:=true;

     FGroupIndexOwned:=false;

     FProgressFlags:=[mtpcSave,mtpcLoad,mtpcCopy];
     FState:=mtstBrowse;

     Inherited BeforeInsert:=_InternalBeforeInsert;

     FFilterParser:=nil;

     FIndexList:=TkbmFieldList.Create;
     FDetailIndexList:=TkbmFieldList.Create;
     FIndexDefs:=TIndexDefs.Create(Self);
     FSortIndex:=nil;
     FDetailIndex:=nil;
     FRangeIndex:=nil;
     FGroupIndex:=nil;
     FEnableIndexes:=true;
     FAutoAddIndexes:=false;

     FStoreDataOnForm:=false;
     FTempDataStorage:=nil;

     FAutoUpdateFieldVariables:=false;

     FMasterLink:=GetMasterDataLinkClass.Create(Self);
     FMasterLink.OnMasterDisable:=MasterDisabled;
     FMasterLink.OnMasterChange:=MasterChanged;
     FMasterLinkUsed:=true;
end;

destructor TkbmCustomMemTable.Destroy;
begin
     Close;

     // Check if temporary data storage left over.
     if FTempDataStorage<>nil then FTempDataStorage.free;
     FTempDataStorage:=nil;

     // Delete filterbuffers if assigned.
     FreeFilter(FFilterParser);

     // Free indexreferences.
     FIndexes.free;

     // Free index definitions.
     FIndexDefs.free;
     FIndexDefs:=nil;

     // Delete allocated memory.
     FreeAndNil(FMasterLink);
     FreeAndNil(FIndexList);
     FreeAndNil(FDetailIndexList);
     FreeAndNil(FSortIndex);

     // Must be before deletion of records, otherwise it fails.
     inherited Destroy;

     // Dont delete shared data if attached to it.
     FCommon.DeAttachTable(Self);
     if FAttachedTo=nil then
        FCommon.free;
end;

procedure TkbmCustomMemTable.__ClearCalcFields(Buffer:PkbmRecord);
begin
     ClearCalcFields(PkbmClearCalcFields(buffer));
end;

procedure TkbmCustomMemTable.__GetCalcFields(Buffer:PkbmRecord);
begin
     GetCalcFields(PkbmGetCalcFields(buffer));
end;

function TkbmCustomMemTable.__CalcFieldsSize:integer;
begin
     Result:=CalcFieldsSize;
end;

procedure TkbmCustomMemTable.__ClearBuffers;
begin
     ClearBuffers;
end;

function TkbmCustomMemTable.__SetTempState(const Value: TDataSetState): TDataSetState;
begin
     Result:=SetTempState(Value);
end;


procedure TkbmCustomMemTable.__RestoreState(const Value: TDataSetState);
begin
     RestoreState(Value);
end;

{$IFDEF KBMMEMTABLE_SUPPORT_SETBLOCKREADSIZE}
procedure TkbmCustomMemTable.__SetBlockReadSize(Value:integer);
begin
     SetblockReadSize(Value);
end;
{$ENDIF}

procedure TkbmCustomMemTable.Lock;
begin
     FCommon.Lock;
     FCommon.FThreadProtected:=true;
end;

procedure TkbmCustomMemTable.Unlock;
begin
     FCommon.FThreadProtected:=false;
     FCommon.Unlock;
end;

procedure TkbmCustomMemTable.Loaded;
begin
     if not FDesignActivation then
        FInterceptActive:=true;
     try
        inherited Loaded;
     finally
        FInterceptActive:=false;
     end;
end;

procedure TkbmCustomMemTable.DataEvent(Event: TDataEvent; Info:TkbmDataEventInfo);
begin
     if FCommon.FThreadProtected then
     begin
          if Event = deFieldChange then
          begin
               DisableControls;
               try
                  inherited DataEvent(Event,Info);
               finally
                  EnableControls;
               end;
          end
          else
              exit;
     end
     else
     inherited DataEvent(Event,Info);
end;

procedure TkbmCustomMemTable.SetActive(Value:boolean);
begin
     if FInterceptActive and Value then exit;
     if (not Value) and (Persistent) then SavePersistent; 
     inherited SetActive(Value);
end;

procedure TkbmCustomMemTable._InternalBeforeInsert(DataSet:TDataSet);
begin
     FInsertRecNo:=GetRecNo;
     if Assigned(FBeforeInsert) then FBeforeInsert(DataSet);
end;

procedure TkbmCustomMemTable.Progress(Pct:integer; Code:TkbmProgressCode);
begin
     if Assigned(FOnProgress) and (Code in FProgressFlags) then FOnProgress(self,Pct,Code);
end;

// Get current component version.
function TkbmCustomMemTable.GetVersion:string;
begin
     Result:=KBMMEMTABLE_VERSION + ' ' + KBMMEMTABLE_EDITION+' Edition';
end;

// Handle saving and loading static data from the form.
procedure TkbmCustomMemTable.DefineProperties(Filer:TFiler);
begin
     inherited DefineProperties(Filer);
     Filer.DefineBinaryProperty('Data', ReadData, WriteData, FStoreDataOnForm);
end;

procedure TkbmCustomMemTable.ReadData(Stream:TStream);
begin
     if FTempDataStorage<>nil then
     begin
          FTempDataStorage.free;
          FTempDataStorage:=nil;
     end;
     FTempDataStorage:=TMemoryStream.Create;
     FTempDataStorage.LoadFromStream(Stream);
end;

procedure TkbmCustomMemTable.WriteData(Stream:TStream);
begin
     if Active then
        InternalSaveToStreamViaFormat(Stream,FFormFormat);
end;

// Update the properties if some component we are dependent on is removed.
procedure TkbmCustomMemTable.Notification(AComponent: TComponent; Operation: TOperation);
begin
     if Operation=opRemove then
     begin
          // Check if this table.
          if AComponent=self then
          begin
               // Close table.
//               if Active then SavePersistent;
               Close;
          end;
          if (FMasterLink<>nil) and (AComponent=FMasterLink.DataSource) then
             FMasterLink.DataSource:=nil;
          if AComponent=FDeltaHandler then
          begin
               (AComponent as TkbmCustomDeltaHandler).FDataSet:=nil;
               FDeltaHandler:=nil;
          end;
          if AComponent=FAttachedTo then FAttachedTo:=nil;
          if AComponent=FDefaultFormat then FDefaultFormat:=nil;
          if AComponent=FCommaTextFormat then FCommaTextFormat:=nil;
          if AComponent=FPersistentFormat then FPersistentFormat:=nil;
          if AComponent=FFormFormat then FFormFormat:=nil;
          if AComponent=FAllDataFormat then FAllDataFormat:=nil;
     end;
     inherited Notification(AComponent, Operation);
end;

procedure TkbmCustomMemTable.IndexFreed(AIndex:TkbmIndex);
begin
     if FIndexes=nil then
        exit;                     // NEXTGEN quirk. When getting here during destruction, we will get a nil FIndexes even though FIndexes is not entirely destroyed yet.
     if AIndex=FIndexes.FRowOrderIndex then
        FCurIndex:=nil
     else if FCurIndex.IsDependingOn(AIndex) then
     begin
          if AIndex.Base<>nil then
             FCurIndex:=AIndex.Base
          else
             FCurIndex:=FIndexes.FRowOrderIndex;
          if Active then
             Resync([]);
     end;
     if AIndex=FSortIndex then FSortIndex:=nil;
     if AIndex=FGroupIndex then FGroupIndex:=nil;
     if AIndex=FDetailIndex then FDetailIndex:=nil;
     if AIndex=FRangeIndex then FRangeIndex:=nil;

     Indexes.IndexFreed(AIndex);
end;

// Set minimum autoinc value.
procedure TkbmCustomMemTable.SetAutoIncMinValue(AValue:TkbmNativeInt);
begin
     FCommon.AutoIncMin:=AValue;
end;

function TkbmCustomMemTable.GetAutoIncValue:TkbmNativeInt;
begin
     Result:=FCommon.AutoIncMax;
end;

function TkbmCustomMemTable.GetAutoIncMin:TkbmNativeInt;
begin
     Result:=FCommon.AutoIncMin;
end;

procedure TkbmCustomMemTable.SetLoadedCompletely(Value:boolean);
begin
     FLoadedCompletely:=Value;
end;

procedure TkbmCustomMemTable.SetTableState(AValue:TkbmState);
begin
     FState:=AValue;
end;

// Set performance.
procedure TkbmCustomMemTable.SetPerformance(AValue:TkbmPerformance);
begin
     FCommon.Performance:=AValue;
end;

function TkbmCustomMemTable.GetPerformance:TkbmPerformance;
begin
     Result:=FCommon.Performance;
end;

// Set versioning mode.
procedure TkbmCustomMemTable.SetVersioningMode(AValue:TkbmVersioningMode);
begin
     FCommon.VersioningMode:=AValue;
end;

function TkbmCustomMemTable.GetVersioningMode:TkbmVersioningMode;
begin
     Result:=FCommon.VersioningMode;
end;

// Set versioning enabled.
procedure TkbmCustomMemTable.SetEnableVersioning(AValue:boolean);
begin
     FCommon.EnableVersioning:=AValue;
end;

function TkbmCustomMemTable.GetEnableVersioning:boolean;
begin
     Result:=FCommon.EnableVersioning;
end;

procedure TkbmCustomMemTable.SetStandalone(AValue:boolean);
begin
     FCommon.Lock;
     try
        if AValue=FCommon.Standalone then exit;

        // Check if open.
        if Active then
           raise EMemTableError.Create(kbmTableMustBeClosed);

        // Check if myself attached.
        if FAttachedTo<>nil then
           raise EMemTableError.Create(kbmIsAttached);

        FCommon.Standalone:=AValue;
     finally
        FCommon.Unlock;
     end;
end;

function TkbmCustomMemTable.GetStandalone:boolean;
begin
     Result:=FCommon.GetStandalone;
end;

procedure TkbmCustomMemTable.SetCapacity(AValue:TkbmNativeInt);
begin
     FCommon.Capacity:=AValue;
end;

function TkbmCustomMemTable.GetCapacity:TkbmNativeInt;
begin
     Result:=FCommon.Capacity;
end;

function TkbmCustomMemTable.GetAttachMaxCount:integer;
begin
     Result:=FCommon.AttachMaxCount;
end;

function TkbmCustomMemTable.GetAttachCount:integer;
begin
     Result:=FCommon.AttachCount;
end;

procedure TkbmCustomMemTable.SetAttachMaxCount(AValue:integer);
begin
     FCommon.AttachMaxCount:=AValue;
end;

function TkbmCustomMemTable.GetIsDataModified:boolean;
begin
     Result:=FCommon.IsDataModified;
end;

procedure TkbmCustomMemTable.SetIsDataModified(AValue:boolean);
begin
     FCommon.IsDataModified:=AValue;
end;

function TkbmCustomMemTable.GetTransactionLevel:integer;
begin
     Result:=FCommon.TransactionLevel;
end;

function TkbmCustomMemTable.GetIndexes:TkbmIndexes;
begin
     if assigned(FIndexes) then Result:=FIndexes
       else Result := nil;
end;

function TkbmCustomMemTable.GetDeletedRecordsCount:TkbmNativeInt;
begin
     Result:=FCommon.GetDeletedRecordsCount;
end;

// Set transaction level.
procedure TkbmCustomMemTable.StartTransaction;
begin
     if not active then exit;
     if (not IsVersioning) or (VersioningMode <> mtvmAllSinceCheckPoint) then
        raise EMemTableError.Create(kbmTransactionVersioning);

     FCommon.IncTransactionLevel;
end;

// Rollback transaction.
procedure TkbmCustomMemTable.Rollback;
begin
     FCommon.Lock;
     try
        // Check if transaction started.
        if not active or (FCommon.FTransactionLevel<=0) then exit;
        Cancel;
        UpdateCursorPos;
        FCommon.RollBack;
        FCommon.DecTransactionLevel;
        FCommon.RebuildIndexes;
        CursorPosChanged;
        ClearBuffers;
        Refresh;
     finally
        FCommon.Unlock;
     end;
end;

// Commit transaction.
procedure TkbmCustomMemTable.Commit;
begin
     FCommon.Lock;
     try
        // Check if transaction started.
        if not active or (FCommon.FTransactionLevel<=0) then exit;
        FCommon.Commit;
        FCommon.DecTransactionLevel;
        FCommon.RebuildIndexes;
        Refresh;
     finally
        FCommon.Unlock;
     end;
end;

procedure TkbmCustomMemTable.Undo;
begin
     if not EnableVersioning then exit;
     FCommon.Undo(GetActiveRecord);
     Refresh;
end;

// Get number of versions of the current record.
function TkbmCustomMemTable.GetVersionCount:integer;
var
   pRec:PkbmRecord;
begin
     Result:=1;

     FCommon.Lock;
     try
        if not Active then raise EMemTableError.Create(kbmNoCurrentRecord);
        pRec:=GetActiveRecord;
        if pRec=nil then raise EMemTableError.Create(kbmNoCurrentRecord);

        while pRec^.PrevRecordVersion<>nil do
        begin
             inc(Result);
             pRec:=pRec^.PrevRecordVersion;
        end;
     finally
        FCommon.Unlock;
     end;
end;

// Get data of a specific version of a record.
function TkbmCustomMemTable.GetVersionFieldData(Field:TField; Version:integer):variant;
var
   pRec:PkbmRecord;
begin
     Result:=Null;
     FCommon.Lock;
     try
        if not Active then raise EMemTableError.Create(kbmNoCurrentRecord);
        pRec:=GetActiveRecord;
        if pRec=nil then raise EMemTableError.Create(kbmNoCurrentRecord);

        while (Version>0) and (pRec^.PrevRecordVersion<>nil) do
        begin
             dec(Version);
             pRec:=pRec^.PrevRecordVersion;
        end;

        FOverrideActiveRecordBuffer:=pRec;
        try
           Result:=Field.AsVariant;
        finally
           FOverrideActiveRecordBuffer:=nil;
        end;
     finally
        FCommon.Unlock;
     end;
end;

// Get TUpdateStatus of a specific version of a record.
function TkbmCustomMemTable.GetVersionStatus(Version:integer):TUpdateStatus;
var
   pRec:PkbmRecord;
begin
     Result:=usUnmodified;
     FCommon.Lock;
     try
        if not Active then raise EMemTableError.Create(kbmNoCurrentRecord);
        pRec:=GetActiveRecord;
        if pRec=nil then raise EMemTableError.Create(kbmNoCurrentRecord);

        while (Version>0) and (pRec^.PrevRecordVersion<>nil) do
        begin
             dec(Version);
             pRec:=pRec^.PrevRecordVersion;
        end;
        Result:=pRec^.UpdateStatus;
     finally
        FCommon.Unlock;
     end;
end;

function TkbmCustomMemTable.SetVersionFieldData(Field:TField; AVersion:integer; AValue:variant):variant;
var
   pRec:PkbmRecord;
begin
     Result:=Null;
     FCommon.Lock;
     try
        if not Active then raise EMemTableError.Create(kbmNoCurrentRecord);
        pRec:=GetActiveRecord;
        if pRec=nil then raise EMemTableError.Create(kbmNoCurrentRecord);

        while (AVersion>0) and (pRec^.PrevRecordVersion<>nil) do
        begin
             dec(AVersion);
             pRec:=pRec^.PrevRecordVersion;
        end;

        FOverrideActiveRecordBuffer:=pRec;
        try
           Result:=Field.Value;
           Field.Value:=AValue;
        finally
           FOverrideActiveRecordBuffer:=nil;
        end;
     finally
        FCommon.Unlock;
     end;
end;

function TkbmCustomMemTable.SetVersionStatus(AVersion:integer; AUpdateStatus:TUpdateStatus):TUpdateStatus;
var
   pRec:PkbmRecord;
begin
     Result:=usUnmodified;
     FCommon.Lock;
     try
        if not Active then raise EMemTableError.Create(kbmNoCurrentRecord);
        pRec:=GetActiveRecord;
        if pRec=nil then raise EMemTableError.Create(kbmNoCurrentRecord);

        while (AVersion>0) and (pRec^.PrevRecordVersion<>nil) do
        begin
             dec(AVersion);
             pRec:=pRec^.PrevRecordVersion;
        end;

        Result:=pRec^.UpdateStatus;
        pRec^.UpdateStatus:=AUpdateStatus;

     finally
        FCommon.Unlock;
     end;
end;

function TkbmCustomMemTable.AddIndex(const Name, Fields: string; Options: TIndexOptions; AUpdateStatus:TUpdateStatusSet):TkbmIndex;
var
   Index:TkbmIndex;
begin
     FIndexDefs.Add(Name,Fields,Options);
     FIndexDefs.Updated:=true;
     try
        Index:=TkbmIndex.Create(Indexes.FRowOrderIndex,Name,self,Fields,IndexOptions2CompareOptions(Options),mtitSorted);
        Index.FUpdateStatus:=AUpdateStatus;
        Indexes.AddIndex(Index);
        UpdateIndexes;
        Result:=Index;
     except
        DeleteIndex(Name);
        UpdateIndexes;
        raise;
     end;
end;

function TkbmCustomMemTable.AddIndex2(const Name, Fields: string; Options: TkbmMemTableCompareOptions; AUpdateStatus:TUpdateStatusSet):TkbmIndex;
var
   Index:TkbmIndex;
begin
     FIndexDefs.Add(Name,Fields,CompareOptions2IndexOptions(Options));
     FIndexDefs.Updated:=true;
     try
        Index:=TkbmIndex.Create(Indexes.FRowOrderIndex,Name,self,Fields,Options,mtitSorted);
        Index.FUpdateStatus:=AUpdateStatus;
        Indexes.AddIndex(Index);
        UpdateIndexes;
        Result:=Index;
     except
        DeleteIndex(Name);
        UpdateIndexes;
        raise;
     end;
end;

function TkbmCustomMemTable.AddFilteredIndex(const Name, Fields: string; Options: TIndexOptions; AUpdateStatus:TUpdateStatusSet; Filter:string; FilterOptions:TFilterOptions; FilterFunc:TkbmOnFilterIndex = nil):TkbmIndex;
var
   Index:TkbmIndex;
begin
     FIndexDefs.Add(Name,Fields,Options);
     FIndexDefs.Updated:=true;
     try
        Index:=TkbmIndex.Create(Indexes.FRowOrderIndex,Name,self,Fields,IndexOptions2CompareOptions(Options),mtitSorted);
        Index.FIsFiltered:=true;
        Index.FUpdateStatus:=AUpdateStatus;
        if Filter<>'' then
           BuildFilter(Index.FFilterParser,Filter,FilterOptions)
        else
            Index.FFilterParser:=nil;
        Index.FFilterFunc:=FilterFunc;
        Indexes.AddIndex(Index);
        UpdateIndexes;
        Result:=Index;
     except
        DeleteIndex(Name);
        UpdateIndexes;
        raise;
     end;
end;

function TkbmCustomMemTable.AddFilteredIndex2(const Name, Fields: string; Options: TkbmMemTableCompareOptions; AUpdateStatus:TUpdateStatusSet; Filter:string; FilterOptions:TFilterOptions; FilterFunc:TkbmOnFilterIndex = nil):TkbmIndex;
var
   Index:TkbmIndex;
begin
     FIndexDefs.Add(Name,Fields,CompareOptions2IndexOptions(Options));
     FIndexDefs.Updated:=true;
     try
        Index:=TkbmIndex.Create(Indexes.FRowOrderIndex,Name,self,Fields,Options,mtitSorted);
        Index.FIsFiltered:=true;
        Index.FUpdateStatus:=AUpdateStatus;
        if Filter<>'' then
           BuildFilter(Index.FFilterParser,Filter,FilterOptions)
        else
            Index.FFilterParser:=nil;
        Index.FFilterFunc:=FilterFunc;
        Indexes.AddIndex(Index);
        UpdateIndexes;
        Result:=Index;
     except
        DeleteIndex(Name);
        UpdateIndexes;
        raise;
     end;
end;

function TkbmCustomMemTable.AddIndex(const Name, Fields: string; Options: TIndexOptions):TkbmIndex;
begin
     Result:=AddIndex(Name,Fields,Options,[usInserted,usModified,usUnmodified]);
end;

function TkbmCustomMemTable.AddIndex2(const Name, Fields: string; Options: TkbmMemTableCompareOptions):TkbmIndex;
begin
     Result:=AddIndex2(Name,Fields,Options,[usInserted,usModified,usUnmodified]);
end;

function TkbmCustomMemTable.AddFilteredIndex(const Name, Fields: string; Options: TIndexOptions; Filter:string; FilterOptions:TFilterOptions; FilterFunc:TkbmOnFilterIndex = nil):TkbmIndex;
begin
     Result:=AddFilteredIndex(Name,Fields,Options,[usInserted,usModified,usUnmodified],Filter,FilterOptions,FilterFunc);
end;

function TkbmCustomMemTable.AddFilteredIndex2(const Name, Fields: string; Options: TkbmMemTableCompareOptions; Filter:string; FilterOptions:TFilterOptions; FilterFunc:TkbmOnFilterIndex = nil):TkbmIndex;
begin
     Result:=AddFilteredIndex2(Name,Fields,Options,[usInserted,usModified,usUnmodified],Filter,FilterOptions,FilterFunc);
end;

procedure TkbmCustomMemTable.DeleteIndex(const Name: string);
var
   i:integer;
begin
     i:=FIndexDefs.IndexOf(Name);
     if i>=0 then
     begin
          FIndexDefs.Delete(i);
          UpdateIndexes;
     end;
     FIndexDefs.Updated:=true;
end;

procedure TkbmCustomMemTable.SwitchToIndex(Index:TkbmIndex);
var
   id:TkbmNativeInt;
begin
     if Index=FCurIndex then exit;

     id:=-1;
     if Active then
     begin
          CheckBrowseMode;
          id:=PkbmRecord(ActiveBuffer)^.RecordID;
     end;

//     CancelRange;
     if FCurIndex<>nil then UpdateCursorPos;

     if Index=nil then Index:=Indexes.FRowOrderIndex;

     // Check if index is updated. If not, update it.
     if not Index.IsOrdered then
        Index.Rebuild;

     if Index.IsRowOrder then
     begin
          FIndexFieldNames:='';
          FIndexName:='';
     end
     else
     begin
          FIndexFieldNames:=Index.FIndexFields;
          FIndexName:=Index.FName;
     end;
     FCurIndex:=Index;

     FCurIndex.FIndexFieldList.AssignTo(FIndexList);

     try
        // Repos recordno.
        if FRecNo>=FCurIndex.FReferences.Count then
           FRecNo:=FCurIndex.FReferences.Count-1;
        if Active and (FRecNo>=0) then
        begin
             FCurIndex.SearchRecordID(id,FRecNo);

             // Check if record accepted according to current filter. If not, seek first.
             if (FRecNo<0) or (FRecNo>=FCurIndex.FReferences.Count) or (not FilterRecord(FCurIndex.FReferences.Items[FRecNo],false)) then
                First;
        end;
//        Refresh;
        Resync([]);
     except
        SetState(dsInactive);
        CloseCursor;
        raise;
     end;
end;

procedure TkbmCustomMemTable.SetIndexFieldNames(FieldNames:string);
var
   lIndex:TkbmIndex;
begin
     if Active then
     begin
          FMasterDetailRelationChanged:=true;
          if FieldNames='' then
             SwitchToIndex(nil)
          else
          begin
               lIndex:=Indexes.GetByFieldNames(FieldNames);
               if lIndex=nil then
               begin
                    lIndex:=TkbmIndex.Create(FCurIndex,kbmDefaultIndex,self,FieldNames,[],mtitSorted);
                    Indexes.AddIndex(lIndex);
                    lIndex.Rebuild;
               end;
               if lIndex<>nil then
                  SwitchToIndex(lIndex);
          end;
     end
     else
         FIndexFieldNames:=FieldNames;
end;

procedure TkbmCustomMemTable.SetIndexName(IndexName:string);
var
   lIndex:TkbmIndex;
begin
     if Active then
     begin
          if IndexName='' then
             SwitchToIndex(nil)
          else
          begin
               lIndex:=Indexes.Get(IndexName);
               if lIndex<>nil then SwitchToIndex(lIndex);
          end;
     end
     else
         FIndexName:=IndexName;
end;

procedure TkbmCustomMemTable.SetIndexDefs(Value:TIndexDefs);
begin
     FIndexDefs.assign(Value);
end;

procedure TkbmCustomMemTable.SetAutoUpdateFieldVariables(AValue:boolean);
begin
     if FAutoUpdateFieldVariables<>AValue then
     begin
          FAutoUpdateFieldVariables:=AValue;
          if Active then UpdateFieldVariables;
     end;
end;

procedure TkbmCustomMemTable.UpdateFieldVariables;
var
   i:TkbmNativeInt;
begin
     if Assigned(owner)
        and (ComponentCount>0)
        and (Owner.ComponentCount>0)
        and not (csDesigning in ComponentState) then
     begin
          for i:=Pred(FieldCount) downto 0 do
              if not Assigned(Owner.FindComponent(Name+Fields[i].FieldName)) then
              begin
                   Fields[i].Name:=Name+Fields[i].FieldName;
                   RemoveComponent(Fields[i]);
                   Owner.InsertComponent(Fields[i]);
              end;
     end;
end;

procedure TkbmCustomMemTable.SetRecordTag(Value:TkbmNativeInt);
var
   p:PkbmRecord;
   r:TkbmNativeInt;
begin
     FCommon.Lock;
     try
        p:=GetActiveRecord;
        if p=nil then raise EMemTableError.Create(kbmNoCurrentRecord);
           r:=p^.RecordID;
        if (r<0) or (r>=FCommon.FRecords.Count) then
           raise EMemTableError.Create(kbmNoCurrentRecord);

        PkbmRecord(FCommon.FRecords.Items[r]).Tag:=Value;
        PkbmRecord(p).Tag:=Value;
     finally
        FCommon.Unlock;
     end;
end;

function TkbmCustomMemTable.GetRecordTag:TkbmNativeInt;
var
   p:PkbmRecord;
begin
     Result:=0;
     FCommon.Lock;
     try
        p:=GetActiveRecord;
        if p=nil then raise EMemTableError.Create(kbmNoCurrentRecord);
        Result:=p^.Tag;
     finally
        FCommon.Unlock;
     end;
end;

function TkbmCustomMemTable.GetIsVersioning:boolean;
begin
     Result:=FCommon.EnableVersioning;
end;

procedure TkbmCustomMemTable.SetStatusFilter(const Value:TUpdateStatusSet);
begin
     CheckBrowseMode;
     UpdateCursorPos;
     if FStatusFilter<>value then
     begin
          FStatusFilter:=Value;
          SetIsFiltered;
          Refresh;
//          Resync([]);
     end;
end;

function TkbmCustomMemTable.UpdateStatus:TUpdateStatus;
var
   p:PkbmRecord;
begin
     p:=GetActiveRecord;
     if assigned(p) then
        Result:=p^.UpdateStatus
     else
         Result:=inherited UpdateStatus;
end;

procedure TkbmCustomMemTable.SetAttachedTo(Value:TkbmCustomMemTable);
var
   i:TkbmNativeInt;
   fld:TField;
begin
     if Value=FAttachedTo then exit;
     if Value=self then
        raise EMemTableError.Create(kbmCantAttachToSelf);

     Close;

     // If sort index used before, free it.
     if (FSortIndex<>nil) then
     begin
          Indexes.DeleteIndex(FSortIndex);
          FSortIndex:=nil;
     end;

     // If detail index used before, free it.
     if (FDetailIndex<>nil) then
     begin
          Indexes.DeleteIndex(FDetailIndex);
          FDetailIndex:=nil;
     end;

     // If range index used before, free it.
     if (FRangeIndex<>nil) then
     begin
          Indexes.DeleteIndex(FRangeIndex);
          FRangeIndex:=nil;
     end;

     // If group index used before, free it.
     if (FGroupIndex<>nil) then
     begin
          Indexes.DeleteIndex(FGroupIndex);
          FGroupIndex:=nil;
     end;

     // Check if attached to something, break the attachment.
     if FAttachedTo<>nil then
     begin
          FCommon.DeAttachTable(self);
          FAttachedTo:=nil;

          // Prepare local memorytable.
          FCommon:=TkbmCommon.Create(self);

          // Add row order index to indexlist.
          with Indexes do
          begin
               FRowOrderIndex:=TkbmIndex.Create(nil,kbmRowOrderIndex,self,'',[],mtitNonSorted);
               FRowOrderIndex.FRowOrder:=true;
               AddIndex(FRowOrderIndex);
          end;

          FCommon.Standalone:=false;
     end;

     // Make the new attachment.
     if Value<>nil then
     begin
          // Check if trying to make 3 level attachment. Disallow.
          if Value.FAttachedTo<>nil then
             raise EMemTableError.Create(kbmCantAttachToSelf2);

          // Prepare attached to memorytable.
          FCommon.Free;
          FCommon:=Value.FCommon;
          try
             FCommon.AttachTable(self);
          except
             // Prepare local memorytable.
             FCommon:=TkbmCommon.Create(self);
             raise;
          end;
          FAttachedTo:=Value;

          FCurIndex:=Indexes.FRowOrderIndex;

          try
             if (not Value.Active) and (csDesigning in ComponentState) then Value.InternalInitFieldDefs;
          except
          end;

          CopyFieldDefs(Value.FieldDefs,FieldDefs);

          // Make sure fields match attached fields.
          if not (csDesigning in ComponentState) then
          begin
               for i:=0 to FAttachedTo.FieldCount-1 do
               begin
{$IFDEF KBMMEMTABLE_SUPPORT_FIELDFULLNAME}
                    fld:=FindField(FAttachedTo.Fields[i].FullName);
{$ELSE}
                    fld:=FindField(FAttachedTo.Fields[i].FieldName);
{$ENDIF}
                    if fld=nil then
                    begin
                         fld:=CreateFieldAs(FAttachedTo.Fields[i]);
                         CopyFieldProperties(FAttachedTo.Fields[i], fld);
//                         fld.visible:=false;
                    end;
               end;
          end;
     end;
end;

// Set filtered property.
procedure TkbmCustomMemTable.SetFiltered(Value:boolean);
begin
     if Value=Filtered then exit;
     inherited SetFiltered(Value);
     if Active then
     begin
          if Value and (FFilterParser=nil) and (Filter<>'') then
          begin
               SetFilterText(Filter);
               SetIsFiltered;
               exit;
          end;
          SetIsFiltered;
          ClearBuffers;
          Refresh;
          First;
     end;
end;

// Parse a filterstring and build filter structure.
procedure TkbmCustomMemTable.SetFilterText(const Value:string);
begin
     inherited SetFilterText(Value);

     // Remove old filter.
     FreeFilter(FFilterParser);

     // If active, build filter.
     if Active then
     begin
          BuildFilter(FFilterParser,Value,FFilterOptions);
          SetIsFiltered;
          if Filtered then
          begin
               ClearBuffers;
               First;
               Refresh;
          end;
     end;
end;

procedure TkbmCustomMemTable.SetOnFilterRecord(const Value: TFilterRecordEvent);
begin
     inherited SetOnFilterRecord(Value);
     SetIsFiltered;
end;

procedure TkbmCustomMemTable.ClearCalcFields(Buffer:PkbmClearCalcFields);
begin
     FCommon.Lock;
     try
        FillChar(PkbmRecord(Buffer).Data[FCommon.FStartCalculated],__CalcFieldsSize,0);
     finally
        FCommon.Unlock;
     end;
     inherited;
end;

// Set delta handler.
procedure TkbmCustomMemTable.SetDeltaHandler(AHandler:TkbmCustomDeltaHandler);
begin
     if FDeltaHandler<>nil then FDeltaHandler.FDataSet:=nil;
     if AHandler<>nil then AHandler.FDataSet:=self;
     FDeltaHandler:=AHandler;
end;

// Set the contents of a memtable from a variant.
procedure TkbmCustomMemTable.SetAllData(AVariant:variant);
var
   ms:TMemoryStream;
begin
     // Check if variant contains data.
     if VarIsEmpty(AVariant) or VarIsNull(AVariant) or (not VarIsArray(AVariant)) then exit;

     ms:=TMemoryStream.Create;
     try
        VariantToStream(AVariant,ms);

        EmptyTable;
        ms.Seek(0,TSeekOrigin.soBeginning);
        LoadFromStreamViaFormat(ms,FAllDataFormat);
     finally
        ms.Free;
     end;
end;

function TkbmCustomMemTable.GetAllData:variant;
var
   ms:TMemoryStream;
begin
     Result:=Unassigned;
     if not Active then exit;

     ms:=TMemoryStream.Create;
     try
        SaveToStreamViaFormat(ms,FAllDataFormat);
        Result:=StreamToVariant(ms);
     finally
        ms.Free;
     end;
end;

function TkbmCustomMemTable.GetMasterFields: string;
begin
     Result:=FMasterLink.FieldNames;
end;

procedure TkbmCustomMemTable.SetMasterFields(const Value: string);
begin
     FMasterLink.FieldNames:=Value;

     // Build master field list.
     if Active then
        RebuildFieldLists;
end;

procedure TkbmCustomMemTable.SetDetailFields(const Value: string);
begin
     FDetailFieldNames:=Value;
     FMasterDetailRelationChanged:=true;

     // Build detail field list.
     if Active then
     begin
          RebuildFieldLists;
          MasterChanged(self);
     end;
end;

function TkbmCustomMemTable.GetDataSource: TDataSource;
begin
     Result:=FMasterLink.DataSource;
end;

procedure TkbmCustomMemTable.SetDataSource(Value: TDataSource);
begin
     if IsLinkedTo(Value) then DatabaseError(kbmSelfRef,Self);
     FMasterLink.DataSource:=Value;
     if Active then
     begin
          RebuildFieldLists;
          MasterChanged(self);
     end;
end;

procedure TkbmCustomMemTable.CreateDetailIndex;
var
   bRebuild:boolean;
begin
     if FMasterDetailRelationChanged then
        FreeDetailIndex;

     if FDetailIndex=nil then
     begin
          if (FDetailIndexList.Count<=0) then
             FDetailIndex:=TkbmIndex.Create(FCurIndex,kbmDetailIndex,self,FIndexFieldNames,[mtcoIgnoreNullKey],mtitSorted)
          else
             FDetailIndex:=TkbmIndex.Create(FCurIndex,kbmDetailIndex,self,FDetailFieldNames,[mtcoIgnoreNullKey],mtitSorted);

          Indexes.AddIndex(FDetailIndex);
          FMasterDetailRelationChanged:=false;
          bRebuild:=true;
     end
     else
         bRebuild:=false;

     FDetailIndex.RangeStartKey:=FKeyBuffers[kbmkbMasterDetail];
     FDetailIndex.RangeEndKey:=FKeyBuffers[kbmkbMasterDetail];
     FDetailIndex.Rebuild;
     FDetailIndex.RebuildDepending;

     if bRebuild then
        SwitchToIndex(FDetailIndex)
     else
        Resync([]);
end;

procedure TkbmCustomMemTable.FreeDetailIndex;
begin
     if FDetailIndex<>nil then
     begin
          Indexes.DeleteIndex(FDetailIndex);
          First;
     end;
end;

function TkbmCustomMemTable.GetRangeActive:boolean;
begin
     Result:=FCurIndex.Name=kbmRangeIndex;
end;

procedure TkbmCustomMemTable.CreateRangeIndex;
begin
     if FRangeIndex<>nil then
        FreeRangeIndex;
     FRangeIndex:=TkbmIndex.Create(FCurIndex,kbmRangeIndex,self,FIndexFieldNames,[mtcoIgnoreNullKey],mtitSorted);
     FRangeIndex.RangeStartKey:=FKeyBuffers[kbmkbRangeStart];
     FRangeIndex.RangeEndKey:=FKeyBuffers[kbmkbRangeEnd];
     FRangeIndex.Rebuild;
     Indexes.AddIndex(FRangeIndex);
     SwitchToIndex(FRangeIndex);
end;

procedure TkbmCustomMemTable.FreeRangeIndex;
begin
     if FRangeIndex<>nil then
     begin
          FreeAndNil(FRangeIndex);
          First;
     end;
end;

procedure TkbmCustomMemTable.CreateGroupIndex(AGroupFields:string);
begin
     if FGroupIndex<>nil then
        FreeGroupIndex;

     FGroupIndex:=TkbmIndex.Create(FCurIndex,kbmGroupIndex,self,AGroupFields,[],mtitSorted);
     FGroupIndexOwned:=true;
     Indexes.AddIndex(FGroupIndex);
     FGroupIndex.Rebuild;
end;

procedure TkbmCustomMemTable.FreeGroupIndex;
begin
     if FGroupIndex<>nil then
     begin
          if FGroupIndexOwned then
             FreeAndNil(FGroupIndex);
          FGroupIndexOwned:=false;
          First;
     end;
end;

procedure TkbmCustomMemTable.MasterChanged(Sender: TObject);
var
   i,n:TkbmNativeInt;
   aList:TkbmFieldList;
begin
//OutputDebugString(PChar('MasterChanged:'+Name));
     // Check if no fields defined for master/detail. Do nothing.
     if (FMasterLink.Fields.Count<=0) then
     begin
          FreeDetailIndex;
          exit;
     end;

     // check if to defined index for master detail (performance) or use detailindexfields (scan).
     if (FDetailIndexList.Count<=0) then
        aList:=FIndexList
     else
         aList:=FDetailIndexList;
     n:=aList.Count;
     if n<=0 then
     begin
          FreeDetailIndex;
          exit;
     end;
     if FMasterLink.Fields.Count<n then
        n:=FMasterLink.Fields.Count;

     // Check if not allocated master keybuffer.
     if FKeyBuffers[kbmkbMasterDetail]=nil then
        FKeyBuffers[kbmkbMasterDetail]:=FCommon._InternalAllocRecord;

     // Fill masterrecord with masterfield values.
     for i:=0 to n-1 do
         PopulateField(FKeyBuffers[kbmkbMasterDetail],aList.Fields[i],TField(FMasterLink.Fields.Items[i]).Value);
     for i:=n to aList.Count-1 do
         PopulateField(FKeyBuffers[kbmkbMasterDetail],aList.Fields[i],Null);

     CreateDetailIndex;

     // Reposition.
     Refresh;
     First;
end;

procedure TkbmCustomMemTable.MasterDisabled(Sender: TObject);
begin
     SetIsFiltered;
     First;
end;

// SetKey, EditKey, FindKey, FindNearest, GotoKey, Ranges.

procedure TkbmCustomMemTable.PrepareKeyRecord(KeyRecordType:integer; Clear:boolean);
begin
     // If keybuffer not assigned, allocate for it.
     if not assigned(FKeyBuffers[KeyRecordType]) then FKeyBuffers[KeyRecordType]:=FCommon._InternalAllocRecord;

     // Switch keybuffer.
     FKeyRecord:=FKeyBuffers[KeyRecordType];
     if Clear then
     begin
          FCommon._InternalFreeRecordVarLengths(FKeyRecord);
          FCommon._InternalClearRecord(FKeyRecord);
     end;
end;

procedure TkbmCustomMemTable.SetKey;
begin
     PrepareKeyRecord(kbmkbKey,true);
     SetState(dsSetKey);
     DataEvent(deDataSetChange,0);
end;

procedure TkbmCustomMemTable.EditKey;
begin
     PrepareKeyRecord(kbmkbKey,false);
     SetState(dsSetKey);
     DataEvent(deDataSetChange,0);
end;

function TkbmCustomMemTable.GotoKey:boolean;
var
   Index:TkbmNativeInt;
   found:boolean;
begin
     Result:=false;

     CheckBrowseMode;

     if not Assigned(FKeyBuffers[kbmkbKey]) then exit;

     SetState(dsBrowse);
     CursorPosChanged;

     PrepareKeyRecord(kbmkbKey,false);

     DisableControls;
     try
        // Locate record.
        Index:=-1;
        found:=false;

        // Locate record.
        Index:=-1;
        FCurIndex.Search(FCurIndex.FIndexFieldList,FKeyRecord,mtstFirst,true,true,Index,Found);
        if Found then
        begin
             FRecNo:=Index;
             Result:=true;
             Resync([]);
             DoAfterScroll;
        end;
     finally
        EnableControls;
        SetFound(Result);
     end;
end;

function TkbmCustomMemTable.FindKey(const KeyValues:array of const):boolean;
var
   i,j,k:TkbmNativeInt;
   fld:TField;
   SaveState:TDataSetState;
begin
     CheckBrowseMode;

     if FIndexFieldNames='' then raise EMemTableError.Create(kbmVarReason2Err);

     PrepareKeyRecord(kbmkbKey,true);

     SaveState:=SetTempState(dsSetKey);
     try
        // Fill values into keyrecord.
        FCurIndex.FIndexFieldList.AssignTo(FIndexList);
        j:=FIndexList.Count-1;
        k:=High(KeyValues);
        if k>=j then k:=j;
        for i:=0 to k do
        begin
             fld:=FIndexList.Fields[i];
             fld.AssignValue(KeyValues[i]);
        end;
     finally
        RestoreState(SaveState);
     end;

     // Goto key.
     Result:=GotoKey;
end;

function TkbmCustomMemTable.GotoNearest:boolean;
var
   Index:TkbmNativeInt;
   Found:boolean;
begin
     Result:=false;
     CheckBrowseMode;

     if not Assigned(FKeyBuffers[kbmkbKey]) then exit;

     SetState(dsBrowse);
     CursorPosChanged;

     PrepareKeyRecord(kbmkbKey,false);

     DisableControls;
     try
        // Look for record.
        Index:=-1;
        FCurIndex.Search(FIndexList,FKeyRecord,mtstNearestAfter,true,true,Index,Found);
        if (Index>=0) then
        begin
             FRecNo:=Index;
             Result:=true;
             Resync([]);
             DoAfterScroll;
        end;

     finally
        EnableControls;
        SetFound(Result);
     end;
end;

function TkbmCustomMemTable.FindNearest(const KeyValues:array of const):boolean;
var
   i,j,k:TkbmNativeInt;
   fld:TField;
   SaveState:TDataSetState;
begin
     CheckBrowseMode;

     // Fill values into keyrecord.
     PrepareKeyRecord(kbmkbKey,true);

     SaveState:=SetTempState(dsSetKey);
     try
        FCurIndex.FIndexFieldList.AssignTo(FIndexList);
        j:=FIndexList.Count-1;
        k:=High(KeyValues);
        if k>=j then k:=j;
        for i:=0 to k do
        begin
             fld:=FIndexList.Fields[i];
             fld.AssignValue(KeyValues[i]);
        end;

     finally
        RestoreState(SaveState);
     end;

     Result:=GotoNearest;
end;

procedure TkbmCustomMemTable.SetRangeStart;
begin
     // Prepare setting key values in key records.
     FCurIndex.FIndexFieldList.AssignTo(FIndexList);

     SetState(dsSetKey);
     PrepareKeyRecord(kbmkbRangeStart,true);
     DataEvent(deDataSetChange,0);
end;

procedure TkbmCustomMemTable.SetRangeEnd;
begin
     // Prepare setting key values in key records.
     FCurIndex.FIndexFieldList.AssignTo(FIndexList);

     SetState(dsSetKey);
     PrepareKeyRecord(kbmkbRangeEnd,true);
     DataEvent(deDataSetChange,0);
end;

procedure TkbmCustomMemTable.EditRangeStart;
begin
     // Prepare setting key values in key records.
     FCurIndex.FIndexFieldList.AssignTo(FIndexList);

     SetState(dsSetKey);
     PrepareKeyRecord(kbmkbRangeStart,false);
     DataEvent(deDataSetChange,0);
end;

procedure TkbmCustomMemTable.EditRangeEnd;
begin
     // Prepare setting key values in key records.
     FCurIndex.FIndexFieldList.AssignTo(FIndexList);

     SetState(dsSetKey);
     PrepareKeyRecord(kbmkbRangeEnd,false);
     DataEvent(deDataSetChange,0);
end;

procedure TkbmCustomMemTable.ApplyRange;
begin
     SetState(dsBrowse);
     CreateRangeIndex;
end;

procedure TkbmCustomMemTable.CancelRange;
var
   n:integer;
begin
     if FRangeIndex=nil then
        exit;

     if pointer(ActiveBuffer)<>nil then
        n:=PkbmRecord(ActiveBuffer)^.RecordID
     else
         n:=-1;

     FreeRangeIndex;

     if Active then
     begin
          if n<0 then First
          else FCurIndex.SearchRecordID(n,FRecNo);
          Resync([]);
     end;
end;

procedure TkbmCustomMemTable.SetRange(const StartValues, EndValues:array of const);
var
   i,j,k:TkbmNativeInt;
   fld:TField;
begin
     CheckBrowseMode;
     if FRangeIndex<>nil then
        FreeRangeIndex;

     // Prepare setting key values in key records.
     FCurIndex.FIndexFieldList.AssignTo(FIndexList);
     j:=FIndexList.Count;

     // Setup start key values.
     SetRangeStart;
     k:=Length(StartValues);
     for i:=0 to j-1 do
     begin
          fld:=FIndexList.Fields[i];
          fld.Clear;
          if i<k then
             fld.AssignValue(StartValues[i]);
     end;

     // Setup end key values.
     SetRangeEnd;
     k:=Length(EndValues);
     for i:=0 to j-1 do
     begin
          fld:=FIndexList.Fields[i];
          fld.Clear;
          if i<k then
             fld.AssignValue(EndValues[i]);
     end;

     ApplyRange;
end;

procedure TkbmCustomMemTable.SetRange(const AFields:string; const StartValues, EndValues:array of variant);
var
   i,j,k,l:TkbmNativeInt;
   fld:TField;
   v:variant;
begin
     if FRangeIndex<>nil then
        FreeRangeIndex;

     FRangeIndex:=TkbmIndex.Create(FCurIndex,kbmRangeIndex,self,AFields,[mtcoIgnoreNullKey],mtitSorted);

     PrepareKeyRecord(kbmkbRangeStart,true);
     PrepareKeyRecord(kbmkbRangeEnd,true);

     j:=FRangeIndex.FIndexFieldList.PrimaryCount;
     k:=Length(StartValues);
     l:=Length(EndValues);
     for i:=0 to j-1 do
     begin
          fld:=FRangeIndex.FIndexFieldList.Fields[i];
          if i<k then
             v:=StartValues[i]
          else
             v:=null;
          PopulateField(FKeyBuffers[kbmkbRangeStart],fld,v);

          if i<l then
             v:=EndValues[i]
          else
             v:=null;
          PopulateField(FKeyBuffers[kbmkbRangeEnd],fld,v);
     end;

     FRangeIndex.RangeStartKey:=FKeyBuffers[kbmkbRangeStart];
     FRangeIndex.RangeEndKey:=FKeyBuffers[kbmkbRangeEnd];
     FRangeIndex.Rebuild;
     Indexes.AddIndex(FRangeIndex);
     SwitchToIndex(FRangeIndex);
end;

function TkbmCustomMemTable.DeleteRecords:integer;
begin
     Result:=0;
     First;
     while not Eof do
     begin
          Delete;
          inc(Result);
     end;
end;

function TkbmCustomMemTable.DeleteRange(const AFields:string; const StartValues, EndValues:array of variant):integer;
begin
     SetRange(AFields,StartValues,EndValues);
     try
        Result:=DeleteRecords;
     finally
        CancelRange;
     end;
end;

procedure TkbmCustomMemTable.DrawAutoInc(ARecord:PkbmRecord = nil);
begin
     // Update autoinc if such a field is defined.
     if Assigned(FAutoIncField) and (not FIgnoreAutoIncPopulation) then
     begin
          if ARecord=nil then
             ARecord:=GetActiveRecord;
          PopulateField(ARecord,FAutoIncField,FCommon.AutoIncMax+1);
     end;
end;

procedure TkbmCustomMemTable.PostAutoInc(ARecord:PkbmRecord = nil);
var
   pai:PByte;
   n:integer;
begin
     // If an autoinc field is specified, allways keep track of highest used number.
     if Assigned(FAutoIncField) then
     begin
          if ARecord=nil then
             ARecord:=GetActiveRecord;
          pai:=FCommon.GetFieldPointer(ARecord,FAutoIncField);
          FCommon.Lock;
          try
             n:=PInteger(pai+1)^;
             if (pai[0]<>kbmffNull) and (FCommon.FAutoIncMax<n) then FCommon.FAutoIncMax:=n;
          finally
             FCommon.Unlock;
          end;
     end;
end;

// Copy masterfields to detail table if a master/detail relation.
procedure TkbmCustomMemTable.DoOnNewRecord;
var
   i,n:TkbmNativeInt;
   aList:TkbmFieldList;
begin
     // Copy link values from master to detail.
     if FMasterLink.Active and (FMasterLink.Fields.Count > 0) and ((FDetailIndexList.Count>0) or (FIndexList.Count>0)) then
     begin
          // check if to use detailfieldlist or indexfieldlist (backwards compability).
          if (FDetailIndexList.Count<=0) then
             aList:=FIndexList
          else
              aList:=FDetailIndexList;
          n:=FMasterLink.Fields.Count;
          if aList.Count<n then n:=aList.Count;

          for i:=0 to n-1 do
              Alist.Fields[i].Value := TField(FMasterLink.Fields[i]).Value;
     end;

     // If a DefaultExpression exists, fill data with default
     for i:=0 to Fields.Count-1 do
         if (Fields[i].DataType<>ftLargeInt)  // Due to Borland not implementing full largeint support in variants.
            and (Fields[i].DefaultExpression<>'') then
                TField(Fields[i]).Value:=TField(Fields[i]).DefaultExpression;

     inherited DoOnNewRecord;

     DrawAutoInc;
end;

// Update max. autoinc. value.
procedure TkbmCustomMemTable.DoBeforePost;
begin
     inherited DoBeforePost;
     PostAutoInc;
end;

procedure TkbmCustomMemTable.DoOnFilterRecord(ADataset:TDataset; var AFiltered:boolean);
begin
     if Assigned(OnFilterRecord) then OnFilterRecord(ADataset,AFiltered);
end;

procedure TkbmCustomMemTable.DestroyIndexes;
begin
     Indexes.Clear;
end;

procedure TkbmCustomMemTable.CreateIndexes;
var
   i:TkbmNativeInt;
begin
     Indexes.Clear;

     for i:=0 to FIndexDefs.Count-1 do
         Indexes.Add(FIndexDefs.Items[i]);
end;

function TkbmCustomMemTable.GetIndexByName(IndexName:string):TkbmIndex;
begin
     Result:=Indexes.Get(IndexName);
end;

function TkbmCustomMemTable.IndexFieldCount:Integer;
begin
     Result:=FCurIndex.FIndexFieldList.Count;
end;

function TkbmCustomMemTable.GetIndexField(Index: Integer): TField;
begin
     if (Index<0) or (Index>=IndexFieldCount) then
        Result:=nil
     else
         Result:=FCurIndex.FIndexFieldList.Fields[Index];
end;

procedure TkbmCustomMemTable.SetIndexField(Index:Integer; Value:TField);
var
   s,a:string;
   i:TkbmNativeInt;
   lIndex:TkbmIndex;
begin
     // Try to find a predefined index matching this and other specified fields.
     s:='';
     a:='';
     for i:=0 to FCurIndex.FIndexFieldList.count-1 do
         s:=s+a+FCurIndex.FIndexFieldList.Fields[i].FieldName;

     lIndex:=Indexes.GetByFieldNames(s);
     if lIndex<>nil then SwitchToIndex(lIndex);
end;

procedure TkbmCustomMemTable.UpdateIndexes;
var
   i,j:TkbmNativeInt;
   lIndex:TkbmIndex;
   DoRefresh:boolean;
begin
     DoRefresh:=false;

     // Check if to delete any indexes.
     for i:=Indexes.Count-1 downto 0 do
     begin
          j:=FIndexDefs.IndexOf(Indexes.FIndexes.Strings[i]);
          if j<0 then
          begin
               lIndex:=TkbmIndex(Indexes.FIndexes.Objects[i]);
               if (lIndex = Indexes.FRowOrderIndex) or (lIndex = FSortIndex) then continue;
               Indexes.FIndexes.Delete(i);
               if lIndex = FCurIndex then
               begin
                    FCurIndex:=Indexes.FRowOrderIndex;
                    DoRefresh:=true;
//                    Resync([]);
                    FIndexFieldNames:='';
               end;
               lIndex.free;
          end;
     end;

     // Check if to add any indexes.
     for i:=0 to FIndexDefs.Count-1 do
     begin
          with FIndexDefs.Items[i] do
          begin
               j:=Indexes.FIndexes.IndexOf(FIndexDefs.Items[i].Name);
               if j<0 then
               begin
                    lIndex:=TkbmIndex.Create(Indexes.FRowOrderIndex,Name,self,Fields,IndexOptions2CompareOptions(Options),mtitSorted);
                    Indexes.AddIndex(lIndex);
               end;
          end;
     end;

     // Check if to rebuild any indexes.
     for i:=0 to Indexes.Count-1 do
     begin
          lIndex:=TkbmIndex(Indexes.FIndexes.Objects[i]);
          if lIndex.FDirty then
          begin
               lIndex.Rebuild;
               if (lIndex = FCurIndex) and (Active) then DoRefresh:=true; // Resync([]);
          end;
     end;

     if DoRefresh then Refresh;
end;

function TkbmCustomMemTable.CreateBlobStream(Field: TField; Mode: TBlobStreamMode): TStream;
begin
     Result:=TkbmBlobStream.Create(Field as TBlobField, Mode);
end;

procedure TkbmCustomMemTable.CreateTable;
var
   i:TkbmNativeInt;
begin
     DoCheckInactive;

     // If no fielddefs existing, use the previously defined fields.
     if FieldDefs.Count = 0 then
        for i:=0 to FieldCount-1 do
            with Fields[i] do
                 if FieldKind = fkData then
                    FieldDefs.Add(FieldName, DataType, Size, Required);

     // Remove previously defined fields and indexes.
     DestroyIndexes;
     DestroyFields;

     // Create fields and indexes.
     CreateFields;
     if FCommon.FOwner=self then
        FCommon.LayoutRecord(FieldCount);
{$IFDEF LEVEL21}
     Fields.LifeCycles:=[lcAutomatic];
{$ENDIF}
     CreateIndexes;

     ResetAutoInc;
end;

// Create field as another field.
function TkbmCustomMemTable.CreateFieldAs(Field:TField; const AForceAsData:boolean = false):TField;
var
   cl:TFieldClass;
begin
     Result:=nil;

     if not (Field.DataType in kbmSupportedFieldTypes) then exit;
     cl:=TFieldClass(Field.ClassType);
     Result:=cl.Create(owner);
     Result.Size:=Field.Size;
     Result.FieldName:=Field.FieldName;
     if not AForceAsData then
     begin
          Result.FieldKind:=Field.FieldKind;
          Result.KeyFields:=Field.KeyFields;
          Result.LookupDataSet:=Field.LookupDataSet;
          Result.LookupResultField:=Field.LookupResultField;
          Result.LookupKeyFields:=Field.LookupKeyFields;
     end
     else
         Result.FieldKind:=fkData;
     if Field is TBCDField then
        TBCDField(Result).Precision:=TBCDField(Field).Precision;
     Result.DataSet:=self;
end;

// Create memory table as another dataset.
procedure TkbmCustomMemTable.CreateTableAs(Source:TDataSet; CopyOptions:TkbmMemTableCopyTableOptions);
{$IFDEF KBMMEMTABLE_SUPPORT_NESTEDFIELDS}
  procedure AssignFieldDef(Src,Dest:TFieldDef);
  var
     i:integer;
  begin
       with Dest do
       begin
            if Collection <> nil then Collection.BeginUpdate;
            try
               // FieldNo is defaulted.
               Name := Src.Name;
               DataType := Src.DataType;
               Size := Src.Size;
               Precision := Src.Precision;
               Attributes := Src.Attributes;
               InternalCalcField := Src.InternalCalcField;
               if HasChildDefs then ChildDefs.Clear;
               if Src.HasChildDefs then
                 for i := 0 to Src.ChildDefs.Count - 1 do
                   AssignFieldDef(Src.ChildDefs[i],AddChild);
            finally
               if Collection <> nil then Collection.EndUpdate;
            end;
       end;
  end;
{$ENDIF}
var
   i:TkbmNativeInt;
   fld:TField;
   fd:TFieldDef;
begin
     DoCheckInactive;

     if Source=nil then exit;

     // Add fields as they are defined in the other dataset.
     if not Source.Active then
        Source.FieldDefs.Update;

     FieldDefs.Clear;
     Fields.Clear;
{$IFDEF KBMMEMTABLE_SUPPORT_NESTEDFIELDS}
//OutputDebugString(PChar('AssignFieldDefs: Source='+Source.FieldDefs.DataSet.Name+' Dest='+Name));
     for i:=0 to Source.FieldDefs.Count-1 do
         AssignFieldDef(Source.FieldDefs.Items[i],FieldDefs.AddFieldDef);
{$ELSE}
     CopyFieldDefs(Source.FieldDefs,FieldDefs);
{$ENDIF}

     // Check which fielddefs we wont keep and potentially convert datatypes.
     for i:=FieldDefs.Count-1 downto 0 do
     begin
          fd:=FieldDefs.Items[i];

          // Remove non supported fieldsdefs.
          if not (fd.DataType in kbmSupportedFieldTypes) then
             FieldDefs.Delete(i)

          // Remove nonactive fields.
          else if (mtcpoOnlyActiveFields in CopyOptions) and
                  (Source.FindField(fd.Name)=nil) then
                      FieldDefs.Delete(i)

          // Remove calculated/lookup fields. They will be added later.
          else if (fd.InternalCalcField) then
               FieldDefs.Delete(i)

          // Check if to convert string fields to widestring fields.
          else if (mtcpoStringAsWideString in CopyOptions) and
               (fd.DataType in [ftString,ftFixedChar]) then
               fd.DataType:=ftWideString;
     end;

     // Destroy existing fields.
     DestroyFields;
{$IFDEF NOP_LEVEL21}
     if not (TFieldLifeCycle.lcAutomatic in Source.Fields.LifeCycles) then
{$ELSE}
     if not Source.DefaultFields then
{$ENDIF}
     begin
          CreateFields;
{$IFDEF LEVEL21}
          Fields.LifeCycles:=[TFieldLifeCycle.lcAutomatic];
{$ENDIF}
     end;

     // Copy lookup and calculated fields if specified.
     for i:=0 to Source.FieldCount-1 do
     begin
          if ((Source.Fields[i].FieldKind=fkLookup) and (mtcpoLookup in CopyOptions)) then
          begin
               fld:=CreateFieldAs(Source.Fields[i],(mtcpoLookupAsData in CopyOptions));
               if mtcpoFieldIndex in CopyOptions then
                  fld.Index:=Source.Fields[i].Index;
          end

          else if ((Source.Fields[i].FieldKind=fkCalculated) and (mtcpoCalculated in CopyOptions)) then
          begin
               fld:=CreateFieldAs(Source.Fields[i],(mtcpoCalculatedAsData in CopyOptions));
               if mtcpoFieldIndex in CopyOptions then
                  fld.Index:=Source.Fields[i].Index;
          end;
     end;

     // Copy fieldproperties from source.
     if mtcpoProperties in CopyOptions then
        CopyFieldsProperties(Source,self);

     ResetAutoInc;
end;

// Delete table.
procedure TkbmCustomMemTable.DeleteTable;
begin
     DoCheckInactive;
     DestroyFields;
end;

procedure TkbmCustomMemTable.CheckActive;
begin
     inherited CheckActive;
//     if not FCommon.IsAnyTableActive then
//        DatabaseError(SDataSetClosed,Self);
end;

procedure TkbmCustomMemTable.CheckInActive;
begin
     inherited CheckInActive;
end;

procedure TkbmCustomMemTable.DoCheckInActive;
begin
     if FCommon.IsAnyTableActive then
        FCommon.CloseTables(nil)
end;

function TkbmCustomMemTable.GetModifiedFlags(i:integer):boolean;
begin
     Result:=false;
     FCommon.Lock;
     try
        if (i<0) or (i>=FieldCount) then raise ERangeError.CreateFmt(kbmOutOfRange,[i]);
        Result:=(FCommon.FFieldFlags[i] and kbmffModified)<>0;
     finally
        FCommon.Unlock;
     end;
end;

function TkbmCustomMemTable.GetLocaleID:integer;
begin
     Result:=FCommon.LocaleID;
end;

procedure TkbmCustomMemTable.SetLocaleID(Value:integer);
begin
     FCommon.LocaleID:=Value;
end;

function TkbmCustomMemTable.GetLanguageID:integer;
begin
     Result:=FCommon.LanguageID;
end;

procedure TkbmCustomMemTable.SetLanguageID(Value:integer);
begin
     FCommon.LanguageID:=Value;
end;

function TkbmCustomMemTable.GetSortID:integer;
begin
     Result:=FCommon.SortID;
end;

procedure TkbmCustomMemTable.SetSortID(Value:integer);
begin
     FCommon.SortID:=Value;
end;

function TkbmCustomMemTable.GetSubLanguageID:integer;
begin
     Result:=FCommon.SubLanguageID;
end;

procedure TkbmCustomMemTable.SetSubLanguageID(Value:integer);
begin
     FCommon.SubLanguageID:=Value;
end;

procedure TkbmCustomMemTable.CreateFieldDefs;
var
   i:TkbmNativeInt;
   fd:TFieldDef;
begin
     FieldDefs.Clear;

     // Add dummy fielddefs to circumvent automatic naming by Delphi.
     for i:=0 to Fieldcount-1 do
     begin
          fd:=FieldDefs.AddFieldDef;
          //In FPC/IOS. fd.Name is blank so skip naming
          if fd.Name <> '' then
             fd.Name:='____'+fd.Name;
     end;

     // Now name and setup fielddefs correctly.
     for i:=0 to Fieldcount-1 do
     begin
          fd:=FieldDefs[i];
          fd.Name:=Fields[i].FieldName;
          fd.DataType:=Fields[i].DataType;
          fd.Size:=Fields[i].Size;
          fd.Required:=Fields[i].Required;
          fd.InternalCalcField:=(Fields[i].FieldKind<>fkData);
          if Fields[i].ReadOnly then
             fd.Attributes:=fd.Attributes+[faReadonly];
     end;
end;

procedure TkbmCustomMemTable.RebuildFieldLists;
begin
     // Build detail field list.
     if (FDetailFieldNames<>'') then
        FDetailIndexList.Build(Self,FDetailFieldNames)
     else
         FDetailIndexList.Clear;
end;

procedure TkbmCustomMemTable.InternalOpen;
//var
//   i:TkbmNativeInt;
begin
     // Check if owner table is open.
     if (Self<>FCommon.FOwner) and (not FCommon.FOwner.Active) then FCommon.FOwner.Open;

     // Attach to common.
     FCommon.Lock;
     try
        InternalInitFieldDefsOnOpen;
        if Self=FCommon.FOwner then
        begin
             InternalCreateFieldsOnOpen;
             ResetAutoInc;
        end
        else
        begin
//             for i:=0 to FieldDefs.Count-1 do
//                 OutputDebugString(PChar(inttostr(i)+') fieldname='+FieldDefs.Items[i].Name));

{$IFDEF NOP_LEVEL21}
            if (TFieldLifeCycle.lcAutomatic in Fields.LifeCycles) then
{$ELSE}
            if DefaultFields then
{$ENDIF}
            begin
                 CreateFields;
{$IFDEF LEVEL21}
                 Fields.LifeCycles:=[TFieldLifeCycle.lcAutomatic];
{$ENDIF}
            end;
        end;

        // Setup size of bookmark as exposed to applications.
        // Bookmark contains record pointer + a 2 byte table identifier + a 4 byte unique record identifier.
        BookmarkSize := sizeof(TkbmUserBookmark);

{$IFDEF KBMMEMTABLE_SUPPORT_NESTEDFIELDS}
        UpdateFieldDefList(FieldDefs,FieldDefList);
{$ENDIF}
        BindFields(True);

        // If Im the owner then layout the records.
        if FCommon.FOwner=self then FCommon.LayoutRecord(FieldCount);

        FIsOpen:=True;
        FRecNo:=-1;
        FReposRecNo:=-1;

        // Prepare index.
        CreateIndexes;

        // Select roworder index. Designtime selected alternative index will be selected in AfterOpen.
        FCurIndex:=Indexes.FRowOrderIndex;

        RebuildFieldLists;
        ClearBuffers;

        FMasterDetailRelationChanged:=true;

        // Set flag that before close has not yet been called (used by destructor).
        FBeforeCloseCalled:=false;
     finally
        FCommon.Unlock;
     end;
end;

procedure TkbmCustomMemTable.InternalClose;
var
   i:integer;  // To avoid H2135 hint (for loop removed due to zero executions) dont use TkbmNativeInt
begin
     // Check if to call before close.
     if not FBeforeCloseCalled then DoBeforeClose;

     FCommon.Lock;
     try
        // Check if owner, close others and empty table.
        if FCommon.FOwner=self then
        begin
             EmptyTable;
             FCommon.CloseTables(self);
        end;
     finally
        FCommon.Unlock;
     end;

     FRecNo:=-1;

     // Remove all indexes (except roworderindex).
     DestroyIndexes;
     FCurIndex:=Indexes.FRowOrderIndex;

     FIsOpen:=False;
     BindFields(False);

     // Delete keybuffers if assigned.
     FKeyRecord:=nil;
     for i:=kbmkbMin to kbmkbMax do
         if Assigned(FKeyBuffers[i]) then
         begin
              FCommon._InternalFreeRecord(FKeyBuffers[i],true,false);
              FKeyBuffers[i]:=nil;
         end;

{$IFDEF NOP_LEVEL21}
     if (TFieldLifeCycle.lcAutomatic in Fields.LifeCycles) then
{$ELSE}
     if DefaultFields then
{$ENDIF}
        DestroyFields;
end;

procedure TkbmCustomMemTable.ResetAutoInc;
begin
     FAutoIncField:=nil;
     FCommon.AutoIncMax:=FCommon.AutoIncMin-1;
     CheckAutoInc;
end;

function TkbmCustomMemTable.CheckAutoInc:boolean;
var
   i:TkbmNativeInt;
begin
     Result:=False;
     for i:=0 to FieldCount-1 do
         if Fields[i].DataType=ftAutoInc then
         begin
              FAutoIncField:=Fields[i];
              Result:=True;
              break;
         end;
end;

procedure TkbmCustomMemTable.InternalInitFieldDefsOnOpen;
begin
     InternalInitFieldDefs;
end;

procedure TkbmCustomMemTable.InternalCreateFieldsOnOpen;
begin
{$IFDEF NOP_LEVEL21}
     if (TFieldLifeCycle.lcAutomatic in Fields.LifeCycles) then
{$ELSE}
     if DefaultFields then
{$ENDIF}
     begin
          if (Self=FCommon.FOwner) and (FieldDefs.Count<=0) then
             raise EMemTableError.Create(kbmVarReason2Err);
          CreateFields;
{$IFDEF LEVEL21}
          Fields.LifeCycles:=[TFieldLifeCycle.lcAutomatic];
{$ENDIF}
     end
     else
         CreateFieldDefs;
end;

procedure TkbmCustomMemTable.InternalInitFieldDefs;
begin
     // Check if attached to another table, use that tables definitions.
     if FAttachedTo<>nil then
     begin
          FAutoIncField:=FAttachedTo.FAutoIncField;
          CopyFieldDefs(FAttachedTo.FieldDefs,FieldDefs);
          exit;
     end;

     // If using predefined fields, generate fielddefs according to fields.
{$IFDEF NOP_LEVEL21}
     if not (TFieldLifeCycle.lcAutomatic in Fields.LifeCycles) then
{$ELSE}
     if not DefaultFields then
{$ENDIF}
        CreateFieldDefs;

     // Look for autoinc field if any.
     ResetAutoInc;
end;

function TkbmCustomMemTable.GetActiveRecord:PkbmRecord;
var
   RecID:integer;
begin
     FCommon.Lock;

     try
        // Check if to return a pointer to a specific buffer.
        if FOverrideActiveRecordBuffer<>nil then
        begin
             Result:=FOverrideActiveRecordBuffer;
             exit;
        end;

        // Else return depending on dataset state.
        case State of
             dsBrowse:              if not IsEmpty then
                                       Result:=PkbmRecord(ActiveBuffer)
                                    else
                                        Result:=nil;

             dsCalcFields:          Result:=PkbmRecord(CalcBuffer);
             dsFilter:              Result:=FFilterRecord;

             dsEdit:                Result:=PkbmRecord(ActiveBuffer);

             dsInsert:              Result:=PkbmRecord(ActiveBuffer);
             dsNewValue,dsCurValue: Result:=PkbmRecord(ActiveBuffer);
             dsOldValue:            begin
                                         // Return database record as Result.
                                         // According to the description of TField.OldValue in the help files,
                                         // OldValue should return the original value of the field before the
                                         // field is posted to. After the post, the oldvalue=curvalue.
                                         // Since the data in the table has not been updated before the post,
                                         // and currently edited data is in the active workrecord only, accessing
                                         // the tahle record will return the original record.
                                         // CHANGED 25. FEB. 2002 KBM
                                         // To make it more compatible with the workings of TClientDataset,
                                         // it will instead return the original unchanged version of the record
                                         // if one exists.
                                         RecID:=PkbmRecord(ActiveBuffer)^.RecordID;

                                         if (RecID>=0) then
                                         begin
                                              Result:=PkbmRecord(FCommon.FRecords.Items[RecID]);
                                              while Result^.PrevRecordVersion<>nil do
                                                    Result:=Result^.PrevRecordVersion;
                                         end
                                         else
                                             Result:=PkbmRecord(ActiveBuffer);
                                    end;

             dsSetKey:              Result:=FKeyRecord;
             dsBlockRead:           Result:=PkbmRecord(ActiveBuffer);
        else
             Result:=nil;
        end;
     finally
        FCommon.Unlock;
     end;
end;

// Result is data in the buffer and a boolean return (true=not null, false=is null).
function TkbmCustomMemTable.GetFieldData(Field: TField; {$IFDEF KBMMEMTABLE_USE_VAR_GETFIELDDATA}var {$ENDIF}Buffer:PGetFieldDataBuffer): Boolean;
var
   SourceBuffer:PByte;
   cBuffer:PByte;
   ActRec,CurRec:PkbmRecord;
   IsVarLength,IsCompressed:boolean;
   pVarLength:PkbmVarLength;
   RecID:TkbmNativeInt;
   cSz:TkbmLongInt;
{$IFDEF KBMMEMTABLE_USE_TVALUEBUFFER}
   n:TkbmLongInt;
{$ENDIF}
begin
     FCommon.Lock;
     try
        Result:=False;
        if not FIsOpen then exit;
        ActRec:=GetActiveRecord;
        if ActRec=nil then exit;
        SourceBuffer:=FCommon.GetFieldPointer(ActRec,Field);
        if SourceBuffer=nil then Exit;

        // Check if calculated field. At the same time check for if varlength field.
        if Field.FieldKind<>fkData then
        begin
             IsVarLength:=false;
             IsCompressed:=false;
        end
        else
        begin
             IsVarLength:=(FCommon.FFieldFlags[Field.FieldNo-1] and kbmffIndirect)<>0;
             IsCompressed:=(FCommon.FFieldFlags[Field.FieldNo-1] and kbmffCompress)<>0;
        end;

        // Return null status.
        Result:=SourceBuffer[0]<>kbmffNull;
        if not Result then exit;

        // Check if varlength field, get the data indirectly. If no data avail. get the data from the db.
        if IsVarLength then
        begin
             pVarLength:=PPkbmVarLength(SourceBuffer+1)^;

             // If varlength field not populated, check if database original populated.
             if (pVarLength = nil) then
             begin
                  // Find the record in the recordlist using the unique record id.
                  RecID:=ActRec^.RecordID;
                  if (RecID>=0) then
                  begin
                       CurRec:=PkbmRecord(FCommon.FRecords.Items[RecID]);
                       cBuffer:=FCommon.GetFieldPointer(CurRec,Field);
                       pVarLength:=PPkbmVarLength(cBuffer+1)^;
                  end

                  // If by any chance no valid recordis is found, something is really rotten.
                  else if Assigned(Buffer) then
                      raise EMemTableInvalidRecord.Create(kbmInvalidRecord);
             end;

             // Check if to get data or not. Blobfields dont return data.
             if (not (Field.DataType in kbmBlobTypes))
                and Assigned(Buffer) and (pVarLength<>nil) then
             begin
                  cBuffer:=GetVarLengthData(pVarLength);
                  cSz:=GetVarLengthSize(pVarLength);

                  // Check if compressed field, decompress buffer.
                  if IsCompressed then
                  begin
                       if Assigned(FCommon.FOwner.FOnDeCompressField) then
                          FCommon.FOwner.FOnDecompressField(self,Field,CBuffer,cSz,CBuffer)
                       else
                           CBuffer:=FCommon.DecompressFieldBuffer(Field,CBuffer,CSz);
                  end;

{$IFDEF KBMMEMTABLE_USE_TVALUEBUFFER}
                  n:=length(Buffer);
                  if cSz>n then
                     cSz:=n;
{$ENDIF}
                  kbmMemMove(cBuffer^,Buffer{$IFDEF KBMMEMTABLE_USE_TVALUEBUFFER}[0]{$ELSE}^{$ENDIF},cSz);
             end;
        end
        else
        begin
             if Assigned(Buffer) then
             begin
                  cSz:=FCommon.GetFieldSize(Field.DataType,Field.Size);
{$IFDEF KBMMEMTABLE_USE_TVALUEBUFFER}
                  n:=length(Buffer);
                  if cSz>n then
                     cSz:=n;
{$ENDIF}
                  kbmMemMove(SourceBuffer[1], Buffer{$IFDEF KBMMEMTABLE_USE_TVALUEBUFFER}[0]{$ELSE}^{$ENDIF}, cSz);
             end;
        end;
     finally
        FCommon.Unlock;
     end;
end;

procedure TkbmCustomMemTable.SetFieldData(Field: TField; Buffer:PSetFieldDataBuffer);
var
   DestinationBuffer:PByte;
   cBuffer:PByte;
   ppVarLength:PPkbmVarLength;
   IsVarLength,IsCompressed:boolean;
   sz:TkbmLongInt;
   n:integer;
   cSz:TkbmLongInt;
begin
     if not FIsOpen then exit;

     FCommon.Lock;
     try
        if not (State in (dsWriteModes+[dsCalcFields])) then
           DatabaseError(kbmEditModeErr,Self);
        DestinationBuffer:=FCommon.GetFieldPointer(GetActiveRecord,Field);
        if DestinationBuffer=nil then Exit;

        if (not FIgnoreReadOnly) and ((FReadOnly or Field.ReadOnly) and (not (State in [dsSetKey,dsCalcFields]))) then
           DatabaseErrorFmt(kbmReadOnlyErr,[Field.DisplayName]);

        sz:=FCommon.GetFieldSize(Field.DataType,Field.Size);

        // Set the null value from the buffer.
        if Buffer<>nil then
           DestinationBuffer^:=kbmffData
        else
            DestinationBuffer^:=kbmffNull;
        inc(DestinationBuffer);

        // Check if calculated field. At the same time check for if varlength field.
        if Field.FieldKind in [fkData,fkInternalCalc,fkCalculated] then Field.Validate(Buffer);
        if Field.FieldKind<>fkData then
        begin
             IsVarLength:=false;
             IsCompressed:=false;
        end
        else
        begin
             IsVarLength:=(FCommon.FFieldFlags[Field.FieldNo-1] and kbmffIndirect)<>0;
             IsCompressed:=(FCommon.FFieldFlags[Field.FieldNo-1] and kbmffCompress)<>0;
        end;

        // Check if varlength field, set the data indirectly.
        if IsVarLength then
        begin
             ppVarLength:=PPkbmVarLength(DestinationBuffer);
             // If varlength field populated, clear it out.
             if (ppVarLength^ <> nil) then
             begin
                  FreeVarLength(ppVarLength^);
                  ppVarLength^:=nil;
             end;

             // Check if to populate the varlength field.
             if Assigned(Buffer) then
             begin
                  cBuffer:={$IFDEF KBMMEMTABLE_USE_TVALUEBUFFER}@Buffer[0]{$ELSE}Buffer{$ENDIF};
                  cSz:=sz;
{$IFDEF KBMMEMTABLE_USE_TVALUEBUFFER}
                  n:=length(Buffer);
                  if cSz>n then
                     cSz:=n;
{$ENDIF}
                  // Check if to compress the data.
                  if IsCompressed then
                  begin
                       if Assigned(FCommon.FOwner.FOnCompressField) then
                          FCommon.FOwner.FOnCompressField(self,Field,cBuffer,cSz,cBuffer)
                       else
                           cBuffer:=FCommon.CompressFieldBuffer(Field,cBuffer,cSz);
                  end;

                  ppVarLength^:=AllocVarLengthAs(cBuffer,cSz);
             end;
        end
        else
        begin
             if Assigned(Buffer) then
             begin
{$IFDEF KBMMEMTABLE_USE_TVALUEBUFFER}
                  n:=length(Buffer);
                  if sz>n then
                     sz:=n;
{$ENDIF}
                  kbmMemMove(Buffer{$IFDEF KBMMEMTABLE_USE_TVALUEBUFFER}[0]{$ELSE}^{$ENDIF},DestinationBuffer^,sz);
             end;
        end;

        // Set modified flag.
        n:=Field.FieldNo-1;
        if (n>=0) then FCommon.FFieldFlags[n]:=FCommon.FFieldFlags[n] or kbmffModified;

        if not (State in [dsCalcFields, dsFilter, dsNewValue]) then
           DataEvent(deFieldChange, TkbmNativeInt(Field));
     finally
        FCommon.Unlock;
     end;
end;

function TkbmCustomMemTable.IsCursorOpen: Boolean;
begin
     Result:=FIsOpen;
end;

function TkbmCustomMemTable.GetCanModify: Boolean;
begin
     if FIgnoreReadOnly then
        Result:=true
     else
         Result:=not FReadOnly;
end;

function TkbmCustomMemTable.GetRecordSize: Word;
begin
     Result:=FCommon.FTotalRecordSize;
end;

{$IFDEF NEXTGEN}
function TkbmCustomMemTable.AllocRecBuf: TRecBuf;
begin
     Result:=PkbmAllocRecordBuffer(FCommon._InternalAllocRecord);
end;

procedure TkbmCustomMemTable.FreeRecBuf(var Buffer: TRecBuf);
begin
     FCommon._InternalFreeRecord(PkbmRecord(Buffer),false,false);
     Buffer:=0;
end;
{$ELSE}
function TkbmCustomMemTable.AllocRecordBuffer:PkbmAllocRecordBuffer;
begin
     Result:=PkbmAllocRecordBuffer(FCommon._InternalAllocRecord);
end;

procedure TkbmCustomMemTable.FreeRecordBuffer(var Buffer:PkbmFreeRecordBuffer);
begin
     FCommon._InternalFreeRecord(PkbmRecord(Buffer),false,false);
     Buffer:=nil;
end;
{$ENDIF}

{$IFDEF KBMMEMTABLE_SUPPORT_SETBLOCKREADSIZE}
procedure TkbmCustomMemTable.SetBlockReadSize(Value:integer);
begin
     if Value <> BlockReadSize then
     begin
          if (Value > 0) or (Value < -1) then
          begin
               inherited SetBlockReadSize(Value);
               BlockReadNext;
          end
          else
          begin
               Value:=0;
               inherited SetBlockReadSize(Value);

               CursorPosChanged;
               Resync([]);
          end;
     end;
end;
{$ENDIF}

// Fill one field with contents of a variant.
procedure TkbmCustomMemTable.PopulateField(ARecord:PkbmRecord; Field:TField; AValue:Variant);
var
   p:PByte;
   pValue:PByte;
   CBuffer:PByte;
   fn:integer;
   si:smallint;
   bcd:TBcd;
   c:Currency;
   li:Int64;
{$IFDEF NEXTGEN}
   ws:string;
{$ELSE}
   ws:WideString;
{$ENDIF}
   i:integer;
   w:word;
   wb:WordBool;
   d:double;
   dt:TDateTime;
   ts:TTimeStamp;
   dtr:TDateTimeRec;
   flags:byte;
   sz,sz1:TkbmLongInt;
   CSz:TkbmLongInt;
   ppVarLength:PPkbmVarLength;
{$IFNDEF NEXTGEN}
   s:TBytes;
{$ENDIF}
{$IFNDEF KBMMEMTABLE_DONT_SUPPORT_TIMESTAMPPOPULATION}
   tssql:TSQLTimeStamp;
{$ENDIF}
{$IFDEF KBMMEMTABLE_SUPPORT_FT_EXTENDED}
   ex:Extended;
{$ENDIF}
{$IFDEF KBMMEMTABLE_SUPPORT_FT_LONGWORD}
   lw:LongWord;
{$ENDIF}
begin
//OutputDebugString(PChar(Field.Name+':'+inttostr(FCommon.GetFieldDataOffset(Field))));
     p:=FCommon.GetFieldPointer(ARecord,Field);
     sz:=FCommon.GetFieldSize(Field.DataType,Field.Size);

     FCommon.Lock;
     try
        // Populate with null?
        if VarIsNull(AValue) or VarIsEmpty(AValue) then
        begin
             p[0]:=kbmffNull;
             fn:=Field.FieldNo-1;
             if fn>=0 then
             begin
                  flags:=FCommon.FFieldFlags[fn];
                  if (flags and kbmffIndirect)<>0 then
                  begin
                       inc(p);
                       ppVarLength:=PPkbmVarLength(p);

                       // If varlength field populated, clear it out.
                       if (ppVarLength^ <> nil) then
                       begin
                            FreeVarLength(ppVarLength^);
                            ppVarLength^:=nil;
                       end;
                       exit;
                  end;
             end;
             FillChar(p[1],sz, 0);
             exit;
        end;

        // Populate with value.
        p[0]:=kbmffData;
        with Field do
        begin
             case DataType of
{$IFDEF KBMMEMTABLE_SUPPORT_FT_FIXEDWIDECHAR}
                  DB.ftFixedWideChar,
{$ENDIF}
                  DB.ftWideString:
                      begin
                           ws:=AValue;
                           i:=length(ws);
                           sz1:=(i+1)*sizeof(WideChar);
{$IFDEF NEXTGEN}
                           pValue:=PByte(@ws[0]);
{$ELSE}
                           SetLength(s,sz1);
                           WideStringToBuffer(ws,@s[0]);
                           pValue:=PByte(@s[0]);
{$ENDIF}
                           if sz1<sz then
                              sz:=sz1;

                      end;

{$IFDEF KBMMEMTABLE_SUPPORT_FT_ORAINTERVAL}
                  DB.ftOraInterval,
{$ENDIF}
                  DB.ftFixedChar,
                  DB.ftGUID,
                  DB.ftString:
                      begin
{$IFDEF NEXTGEN}
                           ws:=AValue;
                           pValue:=PByte(@ws[0]);
                           i:=length(ws);
{$ELSE}
                           SetLength(s,dsMaxStringSize);
                           {$IFDEF LEVEL18}AnsiStrings.{$ENDIF}StrLCopy(@s[0],PAnsiChar(AnsiString(VarToStr(AValue))),DataSize);
                           if TStringField(Field).Transliterate then
                              DataSet.Translate(@s[0],@s[0],True);
                           pValue:=@s[0];
                           i:=length(s);
{$ENDIF}
                           sz1:=(i+1)*sizeof(Char);
                           if sz1<sz then
                              sz:=sz1;
                      end;

                  DB.ftSmallint:
                      begin
                           si:=AValue;
                           pValue:=PByte(@si);
                      end;
                  DB.ftLargeInt:
                      begin
                           li:=AValue;
                           pValue:=PByte(@li);
                      end;
{$IFDEF KBMMEMTABLE_SUPPORT_FT_SHORTINT}
                  DB.ftShortint,
{$ENDIF}
{$IFDEF KBMMEMTABLE_SUPPORT_FT_BYTE}
                  DB.ftByte,
{$ENDIF}
                  DB.ftInteger,
                  DB.ftAutoInc:
                      begin
                           i:=AValue;
                           pValue:=PByte(@i);
                      end;

                  DB.ftBCD:
                      begin
                           c:=AValue;
                           i:=TBCDField(Field).Precision;
                           if i<=TBCDField(Field).Size then i:=TBCDField(Field).Size+1;
                           CurrToBCD(c,bcd,i,TBCDField(Field).Size);
                           pValue:=PByte(@bcd);
                      end;

                  DB.ftDate:
                      begin
                           if VarType(AValue) in [varDate,varDouble,varSingle,varInteger] then
                              dt:=AValue
                           else
                              dt:=StrToDateTime(VarToStr(AValue));
                           ts:=DateTimeToTimeStamp(dt);
                           dtr.Date:=ts.Date;
                           pValue:=PByte(@dtr);
                      end;

                  DB.ftTime:
                      begin
                           if VarType(AValue) in [varDate,varDouble,varSingle,varInteger] then
                              dt:=AValue
                           else
                              dt:=StrToDateTime(VarToStr(AValue));
                           ts:=DateTimeToTimeStamp(dt);
                           dtr.Time:=ts.Time;
                           pValue:=PByte(@dtr);
                      end;

                  DB.ftDateTime:
                      begin
                           if VarType(AValue) in [varDate,varDouble,varSingle,varInteger] then
                              dt:=AValue
                           else
                              dt:=StrToDateTime(VarToStr(AValue));
                           ts:=DateTimeToTimeStamp(dt);
                           dtr.DateTime:=TimeStampToMSecs(ts);
                           pValue:=PByte(@dtr);
                      end;
{$IFDEF KBMMEMTABLE_SUPPORT_FT_ORATIMESTAMP}
                  DB.ftOraTimeStamp,
{$ENDIF}
                  DB.ftTimeStamp:
                      begin
{$IFDEF KBMMEMTABLE_DONT_SUPPORT_TIMESTAMPPOPULATION}
  raise Exception.Create('This SKU do not support population of a timestamp field.');
{$ELSE}
                           tssql:=VarToSQLTimeStamp(AValue);
                           pValue:=PByte(@tssql);
{$ENDIF}
                      end;

                  DB.ftWord:
                      begin
                           w:=AValue;
                           pValue:=PByte(@w);
                      end;

                  DB.ftBoolean:
                      begin
                           wb:=AValue;
                           pValue:=PByte(@wb);
                      end;

{$IFDEF KBMMWMTABLE_SUPPORT_FT_SINGLE}
                  DB.ftSingle,
{$ENDIF}
                  DB.ftFloat,
                  DB.ftCurrency:
                      begin
                           d:=AValue;
                           pValue:=PByte(@d);
                      end;
{$IFDEF KBMMEMTABLE_SUPPORT_FT_LONGWORD}
                  DB.ftLongWord:
                      begin
                           lw:=AValue;
                           pValue:=PByte(@lw);
                      end;
{$ENDIF}
{$IFDEF KBMMEMTABLE_SUPPORT_FT_EXTENDED}
                  DB.ftExtended:
                      begin
                           ex:=AValue;
                           pValue:=PByte(@ex);
                      end;
{$ENDIF}
             else
                 raise EMemTableError.Create('PopulateField: '+Format(kbminternalOpen2Err,[integer(DataType)]));
             end;

             // If anything to store.
             if (pValue<>nil) then
             begin
                  // Check if varlength field, set the data indirectly.
                  inc(p);
                  fn:=Field.FieldNo-1;
                  if fn>=0 then      // Calculated fields are never varlengths.
                  begin
                       flags:=FCommon.FFieldFlags[fn];
                       if (flags and kbmffIndirect)<>0 then
                       begin
                            ppVarLength:=PPkbmVarLength(p);

                            // If varlength field populated, clear it out.
                            if (ppVarLength^ <> nil) then
                            begin
                                 FreeVarLength(ppVarLength^);
                                 ppVarLength^:=nil;
                            end;

                            // Check if to populate the varlength field.
                            cBuffer:=pValue;
                            cSz:=sz;

                            // Check if to compress the data.
                            if (flags and kbmffCompress)<>0 then
                            begin
                                 if Assigned(FOnCompressField) then
                                    FCommon.FOwner.FOnCompressField(self,Field,pValue,cSz,CBuffer)
                                 else
                                     CBuffer:=FCommon.CompressFieldBuffer(Field,pValue,CSz);
                            end;

                            ppVarLength^:=AllocVarLengthAs(CBuffer,CSz);
                       end
                       else
                           kbmMemMove(pValue^,p^,sz);

                       FCommon.FFieldFlags[fn]:=FCommon.FFieldFlags[fn] or kbmffModified;
                  end
                  else
                      kbmMemMove(pValue^,p^,sz);
             end;
        end;
     finally
        FCommon.Unlock;
     end;
end;

// Populate a varlength field with a value.
procedure TkbmCustomMemTable.PopulateVarLength(ARecord:PkbmRecord;Field:TField;const Buffer;Size:integer);
var
   pField:PByte;
   pVarLength:PPkbmVarLength;
begin
     pField:=FCommon.GetFieldPointer(ARecord,Field);
     if pField=nil then exit;

     pVarLength:=PPkbmVarLength(pField+1);
     if pVarLength^<>nil then
     begin
          FreeVarLength(pVarLength^);
          pVarLength^:=nil;
     end;

     pVarLength^:=AllocVarLength(Size);

     if Size<>0 then
     begin
          pField[0]:=kbmffData;
          kbmMemMove(Buffer, GetVarLengthData(pVarLength^)^,Size);
     end
     else
         pField[0]:=kbmffNull;
end;

// Fill record with values for specified fields.
procedure TkbmCustomMemTable.PopulateRecord(ARecord:PkbmRecord; Fields:string; Values:variant);
var
   FieldList:TkbmFieldList;
   i:TkbmNativeInt;
   n:TkbmNativeInt;
begin
     FieldList := TkbmFieldList.Create;
     try
        FieldList.Build(self,Fields);

        n:=VarArrayDimCount(Values);
        if n>1 then raise EMemTableError.Create(kbmVarArrayErr);
        if (n=0) and (FieldList.count>1) then raise EMemTableError.Create(kbmVarReason1Err);
        if FieldList.Count<1 then raise EMemTableError.Create(kbmVarReason2Err);

        // Single value.
        if n=0 then
        begin
             PopulateField(ARecord,FieldList.Fields[0],Values);
             exit;
        end;

        // Several values.
        for i:=0 to FieldList.Count-1 do
        begin
             PopulateField(ARecord,FieldList.Fields[i],Values[i]);
        end;
     finally
        FieldList.free;
     end;
end;

procedure TkbmCustomMemTable.InternalFirst;
begin
     _InternalFirst;
end;

procedure TkbmCustomMemTable.InternalLast;
begin
     _InternalLast;
end;

procedure TkbmCustomMemTable._InternalFirst;
begin
     FRecNo:=-1;
end;

procedure TkbmCustomMemTable._InternalLast;
begin
     FRecNo:=FCurIndex.FReferences.Count;
end;

function TkbmCustomMemTable._InternalNext(ForceUseFilter:boolean):boolean;
var
   pRec:PkbmRecord;
   r:TkbmNativeInt;
begin
     // If not filtered.
     if not (ForceUseFilter or IsFiltered) then
     begin
          Result:=(FRecNo<FCurIndex.FReferences.Count-1);
          if Result then Inc(FRecNo);
          exit;
     end;

     // Handle filtering.
     r:=FRecNo+1;
     Result:=false;
     while r<FCurIndex.FReferences.Count do
     begin
          pRec:=PkbmRecord(FCurIndex.FReferences.Items[r]);
          if FilterRecord(pRec,ForceUseFilter) then
          begin
               Result:=true;
               FRecNo:=r;
               break;
          end;
          inc(r);
     end;
end;

function TkbmCustomMemTable._InternalPrior(ForceUseFilter:boolean):boolean;
var
   pRec:PkbmRecord;
begin
     // If not filtered.
     if not (ForceUseFilter or IsFiltered) then
     begin
          Result:=(FRecNo>0);
          if Result then Dec(FRecNo);
          exit;
     end;

     // Handle filtering.
     Result:=false;
     while FRecNo>0 do
     begin
          Dec(FRecNo);
          pRec:=PkbmRecord(FCurIndex.FReferences.Items[FRecNo]);
          if FilterRecord(pRec,ForceUseFilter) then
          begin
               Result:=true;
               break;
          end;
     end;
end;

// Getrecord fetches valid nonfiltered record.
// Only fixed record contents are copied to the buffer.
// All varchars are only referenced to record in recordlist.
// All versions are only referenced to recordversions in recordlist.
function TkbmCustomMemTable.GetRecord(Buffer:PkbmGetRecord; GetMode: TGetMode; DoCheck: Boolean): TGetResult;
var
   pRec:PkbmRecord;
   pbmData:PkbmBookmark;
begin
     case GetMode of
          gmCurrent: begin
                          if FRecNo>=FCurIndex.FReferences.Count then Result:=grEOF
                          else if FRecNo<0 then Result:=grBOF
                          else
                          begin
                               Result:=grOK;
                               if IsFiltered then
                               begin
                                    pRec:=PkbmRecord(FCurIndex.FReferences.Items[FRecNo]);
                                    if not FilterRecord(pRec,false) then Result:=grEOF;
                               end;
                          end;
                     end;
          gmNext:    begin
                          if _InternalNext(false) then Result:=grOK
                          else Result:=grEOF;
                     end;
          gmPrior:   begin
                          if _InternalPrior(false) then Result:=grOK
                          else Result:=grBOF;
                     end;
          else
              Result:=grOK;
     end;
     if Result=grOk then
     begin
          pRec:=PkbmRecord(FCurIndex.FReferences.Items[FRecNo]);

          // Fill record part of buffer
          FCommon._InternalFreeRecordVarLengths(PkbmRecord(Buffer));
          FCommon._InternalClearRecord(PkbmRecord(Buffer));

          // Move record contents to avoid copying all versions and varlengths just for scrolling through records.
          FCommon._InternalMoveRecord(pRec,PkbmRecord(Buffer));

          //fill information part of buffer
          with PkbmRecord(Buffer)^ do
          begin
               RecordNo:=FRecNo;
               RecordID:=pRec^.RecordID;
               UniqueRecordID:=pRec^.UniqueRecordID;

               // Setup bookmark data.
               pbmData:=PkbmBookmark(Data+FCommon.FStartBookmarks);
               inc(pbmData,FTableID);
               pbmData^.Bookmark:=pRec;
               pbmData^.RecordID:=UniqueRecordID;
               pbmData^.Flag:=bfCurrent;

               Flag:=Flag and (not (kbmrfIntable));
          end;
          if FRecalcOnFetch then
             GetCalcFields(Buffer);

     end
     else
         if (GetMode=gmCurrent) then Result:=grError;
end;

function TkbmCustomMemTable.FindRecord(Restart, GoForward: Boolean): Boolean;
var
   Status:boolean;
begin
     CheckBrowseMode;
     DoBeforeScroll;
     SetFound(False);
     UpdateCursorPos;
     CursorPosChanged;

     if GoForward then
     begin
          if Restart then _InternalFirst;
          Status := _InternalNext(true);
     end else
     begin
          if Restart then _InternalLast;
          Status := _InternalPrior(true);
     end;

     if Status then
     begin
          Resync([rmExact, rmCenter]);
          SetFound(True);
     end;
     Result := Found;
     if Result then DoAfterScroll;
end;

// Free filter buffers.
procedure TkbmCustomMemTable.FreeFilter(var AFilterParser:TkbmExprParser);
begin
     if Assigned(AFilterParser) then
     begin
          AFilterParser.free;
          AFilterParser:=nil;
     end;
end;

// Parse filterstring and build new filter.
// Filter operators supported:
// = < > <> <= >= AND OR NULL
// Field Operator Constant Eg: Field1>32 and Field2='ABC'
procedure TkbmCustomMemTable.BuildFilter(var AFilterParser:TkbmExprParser; AFilter:string; AFilterOptions:TFilterOptions);
begin
     if AFilterParser<>nil then
     begin
          AFilterParser.free;
          AFilterParser:=nil;
     end;

     AFilter:=Trim(AFilter);
     if AFilter='' then exit;

{$IFDEF KBMMEMTABLE_SUPPORT_EXPRESSIONPARSER}
     AFilterParser:=TkbmExprParser.Create(self,AFilter,AFilterOptions);
{$ELSE}
     AFilterParser:=TkbmExprParser.Create(self,AFilter);
{$ENDIF}
end;

// Filter record according to filterexpression.
function TkbmCustomMemTable.FilterExpression(ARecord:PkbmRecord;AFilterParser:TkbmExprParser):boolean;
var
   oldrec:PkbmRecord;
begin
     oldrec:=FOverrideActiveRecordBuffer;
     try
        FOverrideActiveRecordBuffer:=ARecord;
        Result:=AFilterParser.Evaluate;
     finally
        FOverrideActiveRecordBuffer:=oldrec;
     end;
//     ShowMessage(Format('noderes=%d Result=%d',[integer(noderes),ord(Result)]));
end;

// Is any record filtering applied.
procedure TkbmCustomMemTable.SetIsFiltered;
begin
     FCommon.Lock;
     try
        FIsFiltered:=(FStatusFilter<>[]) or
             ((FCommon.FDeletedCount>0) and not (usDeleted in FStatusFilter)) or
             (Filtered and (
              Assigned(FFilterParser) or
              Assigned(OnFilterRecord)));
     finally
        FCommon.Unlock;
     end;
end;

// Return true if this table is detail table in a master/detail relation, else false.
function TkbmCustomMemTable.IsMasterDetailActive:Boolean;
begin
     Result:=FMasterLinkUsed and Assigned(FMasterLink.DataSource) and (FMasterLink.FieldNames<>'') and ((FDetailIndexList.Count>0) or (FIndexList.Count>0));
end;

// Filter records in general for
// master/detail, range and userdefined filter.
function TkbmCustomMemTable.FilterRecord(ARecord:PkbmRecord; ForceUseFilter:boolean): Boolean;
var
   SaveState: TDatasetState;
label
   L_Exit;
begin
     Result:=True;
     if not (ForceUseFilter or IsFiltered) then Exit;

     // Check if record is deleted, but versioning.
     if ((FStatusFilter<>[]) and not (ARecord^.UpdateStatus in FStatusFilter)) then
     begin
          Result:=False;
          exit;
     end;

     // Now we will apply the filters on the record.
     SaveState:=SetTempState(dsFilter);
     FFilterRecord:=ARecord;

     // Check if to recalc before compare.
     if FRecalcOnIndex then
     begin
          __ClearCalcFields(ARecord);
          __GetCalcFields(ARecord);
     end;

     // Check filters.
     if ForceUseFilter or Filtered then
     begin
          // Call users own filtering if specified.
          DoOnFilterRecord(self,Result);
          if not Result then goto L_Exit;

          // Check if filterstring active.
          if Assigned(FFilterParser) then
          begin
               Result:=FilterExpression(ARecord,FFilterParser);
               if not Result then goto L_exit;
          end;
     end;

L_Exit:
     // Finished filtering.
     RestoreState(SaveState);
end;

{$IFDEF KBMMEMTABLE_SUPPORT_EXPRESSIONPARSER}
// Test an user filter against to the current record
function TkbmCustomMemTable.TestFilter(const AFilter:string; AFilterOptions:TFilterOptions):boolean;
var
   parser: TkbmExprParser;
begin
     Result:=Active;
     if (AFilter='') or not Active then exit;
     parser:=nil;
     try
        BuildFilter(parser,AFilter,AFilterOptions);
        Result:=FilterExpression(GetActiveRecord,parser);
     finally
        if parser<>nil then FreeFilter(parser);
     end;
end;
{$ENDIF}

procedure TkbmCustomMemTable.InternalSetToRecord(Buffer:PkbmInternalSetToRecord);
var
   i:TkbmNativeInt;
   Rec:PkbmRecord;
begin
     Rec:=PkbmRecord(Buffer);
     if Rec=nil then exit;

     if Rec.RecordNo<>-1 then
     begin
          FRecNo:=Rec.RecordNo;
          exit;
     end;

     // If record number not readily available search for it.
     FCommon.Lock;
     try
        FCurIndex.SearchRecord(Rec,i,true,true);
        if (i>=0) then FRecNo:=i;
     finally
        FCommon.Unlock;
     end;
end;

function TkbmCustomMemTable.GetRecordCount: integer;
var
   SaveState: TDataSetState;
   SavePosition: integer;
   SaveModified:boolean;
   TempBuffer: PkbmGetRecord;
begin
     if not Active then
{$IFDEF FPC}
        DatabaseError('Dataset is not active',Self);
{$ELSE}
        DatabaseError(SDatasetClosed,Self);
{$ENDIF}

     if not IsFiltered then Result:=FCurIndex.FReferences.Count
     else
     begin
          Result:=0;
          SaveModified:=Modified;
          SaveState:=SetTempState(dsBrowse);
          SavePosition:=FRecNo;
          TempBuffer:=PkbmGetRecord(FCommon._InternalAllocRecord);
          try
             InternalFirst;
             while GetRecord(TempBuffer,gmNext,True)=grOk do Inc(Result);
          finally
             RestoreState(SaveState);
             FRecNo:=SavePosition;
             SetModified(SaveModified);
             FCommon._InternalFreeRecord(PkbmRecord(TempBuffer),false,false);
          end;
     end;
end;

function TkbmCustomMemTable.GetRecNo: integer;
begin
     if (State=dsInactive) or (pointer(ActiveBuffer)=nil) or IsEmpty or (PkbMRecord(ActiveBuffer)^.RecordNo<0) then Result:=-1
     else Result:=PkbmRecord(ActiveBuffer)^.RecordNo+1;
end;

procedure TkbmCustomMemTable.SetRecNo(Value: Integer);
var
   r:integer;
begin
     CheckActive;
     r:=GetRecNo;
     if Value=r then exit;
     if (Value<1) or (Value>FCurIndex.FReferences.Count) then exit;

// Doesnt work in XE5 with databinding
//     if not IsFiltered then
//     begin
//          DoBeforeScroll;
//          FRecNo:=Value-1;
//          DoAfterScroll;
//          Resync([]);
//     end
//     else
//     begin
          MoveBy(Value-r);
//     end;
//     CursorPosChanged;
end;

procedure TkbmCustomMemTable.InternalAddRecord(Buffer:PkbmInternalAddRecord; Append: Boolean);
var
   pRec,pCopyRec:PkbmRecord;
   where:integer;
   how:TkbmIndexUpdateHow;
begin
     pRec:=PkbmRecord(Buffer);

     // Check record acceptance.
     Indexes.CheckRecordUniqueness(pRec,nil);

     // Copy the reference record.
     pCopyRec:=FCommon._InternalCopyRecord(pRec,true);

     // Update indexes and add physical record.
     if Append then
     begin
          where:=-1;
          how:=mtiuhAppend;
     end
     else
     begin
          where:=FRecNo;
          how:=mtiuhInsert;
     end;

     FCommon.ReflectToIndexes(self,how,nil,pCopyRec,where,false);

     // Append the reference record.
     pCopyRec^.TransactionLevel:=TransactionLevel;
     pCopyRec^.Flag:=pCopyRec^.Flag or kbmrfInTable;
     IsDataModified:=true;

     FCommon._InternalAppendRecord(pCopyRec);
end;

procedure TkbmCustomMemTable.InternalDelete;
var
   pRec,pDelRec:PkbmRecord;
begin
     FCommon.Lock;
     try
        pRec:=PkbmRecord(FCurIndex.FReferences.Items[FRecNo]);

        // Update indexes.
        FCommon.ReflectToIndexes(self,mtiuhDelete,pRec,nil,FRecNo,false);

        // Check if versioning. Dont delete the record. Only mark it as so.
        if IsVersioning then
        begin
             pDelRec:=FCommon._InternalCopyRecord(pRec,true);
             pRec^.PrevRecordVersion:=pDelRec;
             pRec^.UpdateStatus:=usDeleted;
             pRec^.TransactionLevel:=TransactionLevel;
             inc(FCommon.FDeletedCount);
        end
        else
        begin
             FCommon._InternalDeleteRecord(pRec);

             // After deleted last record, reset state of the table to empty.
             if (FCommon.FRecords.Count=0) then FCommon._InternalEmpty;
        end;

//Removed Aug. 4. to solve grid reposition problem.        ClearBuffers;
        IsDataModified:=true;
     finally
        FCommon.Unlock;
     end;
end;

procedure TkbmCustomMemTable.InternalInitRecord(Buffer:PkbmInternalInitRecord);
begin
     // Clearout record contents.
     FCommon._InternalClearRecord(PkbmRecord(Buffer));
end;

procedure TkbmCustomMemTable.InternalPost;
var
   pActRec,pNewRec,pRec:PkbmRecord;
begin
     inherited InternalPost;    // Otherwise the requiredfieldscheck will not happen.

     FCommon.Lock;
     try
        pActRec:=PkbmRecord(ActiveBuffer);

        if State = dsEdit then
        begin
             // Get reference to record to modify.
             pRec:=FCurIndex.FReferences[FRecNo];

             // Check that record does not violate index.
             Indexes.CheckRecordUniqueness(pActRec,pRec);

             // Check if to update version.
             if Modified then
             begin
                  pActRec^.UpdateStatus:=UsModified;
                  if IsVersioning then
                  begin
                       // Check if only to keep original record since checkpoint.
                       if (FCommon.FVersioningMode=mtvmAllSinceCheckPoint) or
                          ((FCommon.FVersioningMode=mtvm1SinceCheckPoint) and (pActRec^.PrevRecordVersion=nil)) then
                       begin
                            pActRec^.PrevRecordVersion:=FCommon._InternalCopyRecord(pRec,True);
                            pActRec^.PrevRecordVersion^.Flag:=pActRec^.PrevRecordVersion^.Flag or kbmrfInTable;
                       end;
                  end;
                  IsDataModified:=true;
             end;

             // Update index.
             FCommon.ReflectToIndexes(self,mtiuhEdit,pRec,pActRec,FRecNo,false);

             // Alter the physical record.
             FCommon._InternalTransferRecord(pActRec,pRec);
             pRec^.Flag:=pRec^.Flag or kbmrfInTable;
             pRec^.TransactionLevel:=TransactionLevel;
        end
        else  // dsInsert.
        begin
             // Check record acceptance.
             Indexes.CheckRecordUniqueness(pActRec,nil);

             // New record. Allocate room for it and copy the reference record.
             pNewRec:=FCommon._InternalCopyRecord(pActRec,true);
             FCommon._InternalFreeRecordVarLengths(pActRec);

             // Add the physical record.
             FCommon._InternalAppendRecord(pNewRec);
             pNewRec^.TransactionLevel:=TransactionLevel;
             pNewRec^.Flag:=pNewRec^.Flag or kbmrfInTable;

             // Add to index.
             // If BOF bookmark flag set, then append, dont insert.
             if GetBookmarkFlag(PkbmGetBookmarkFlag(pNewRec))=bfEOF then
                FCommon.ReflectToIndexes(self,mtiuhAppend,nil,pNewRec,-1,false)
             else
                FCommon.ReflectToIndexes(self,mtiuhInsert,nil,pNewRec,FInsertRecNo,false);

             IsDataModified:=true;
        end;
        FCommon.ClearModifiedFlags;
     finally
        FCommon.Unlock;
     end;
end;

procedure TkbmCustomMemTable.InternalEdit;
begin
     inherited InternalEdit;
end;

procedure TkbmCustomMemTable.InternalInsert;
begin
     inherited InternalInsert;
end;

procedure TkbmCustomMemTable.InternalCancel;
begin
     inherited InternalCancel;
     FCommon.ClearModifiedFlags;
     FCommon._InternalFreeRecordVarLengths(PkbmRecord(ActiveBuffer));
end;

// Bookmark handling.

procedure TkbmCustomMemTable.SetBookmarkFlag(Buffer:PkbmSetBookmarkFlag; Value: TBookmarkFlag);
var
   pbmData:PkbmBookmark;
begin
     pbmData:=PkbmBookmark(PkbmRecord(Buffer).Data+FCommon.FStartBookmarks);
     inc(pbmData,FTableID);
     pbmData^.Flag:=Value;
end;

function TkbmCustomMemTable.GetBookmarkFlag(Buffer:PkbmGetBookmarkFlag): TBookmarkFlag;
var
   pbmData:PkbmBookmark;
begin
     pbmData:=PkbmBookmark(PkbmRecord(Buffer).Data+FCommon.FStartBookmarks);
     inc(pbmData,FTableID);
     Result:=pbmData^.Flag;
end;

{$IFDEF KBMMEMTABLE_USE_TBOOKMARK}
procedure TkbmCustomMemTable.GetBookmarkData(Buffer: PkbmGetBookmarkData; Data:TBookmark);
{$ELSE}
procedure TkbmCustomMemTable.GetBookmarkData(Buffer: PkbmGetBookmarkData; Data:Pointer);
{$ENDIF}
var
   pbmData:PkbmBookmark;
   pbmUser:PkbmUserBookmark;
begin
     pbmData:=PkbmBookmark(PkbmRecord(Buffer)^.Data+FCommon.FStartBookmarks);
     inc(pbmData,FTableID);

     pbmUser:=PkbmUserBookmark(Data);
     pbmUser^.Bookmark:=pbmData^.Bookmark;
     pbmUser^.DataID:=FCommon.FDataID;
     pbmUser^.RecordID:=pbmData^.RecordID;
end;

{$IFDEF KBMMEMTABLE_USE_TBOOKMARK}
procedure TkbmCustomMemTable.SetBookmarkData(Buffer: PkbmSetBookmarkData; Data:TBookmark);
{$ELSE}
procedure TkbmCustomMemTable.SetBookmarkData(Buffer: PkbmSetBookmarkData; Data:Pointer);
{$ENDIF}
var
   pbmData:PkbmBookmark;
   pbmUser:PkbmUserBookmark;
begin
     pbmData:=PkbmBookmark(PkbmRecord(Buffer)^.Data+FCommon.FStartBookmarks);
     inc(pbmData,FTableID);
     pbmUser:=PkbmUserBookmark(Data);
     pbmData^.Bookmark:=pbmUser^.Bookmark;
end;

// Check if a bookmarkpointer is actually valid.
function TkbmCustomMemTable.InternalBookmarkValid(Bookmark:PkbmInternalBookmarkValid):boolean;
var
   p:PkbmRecord;
   pbmUser:PkbmUserBookmark;
begin
     Result:=Bookmark<>nil;
     if Result then
     begin
          pbmUser:=PkbmUserBookmark(Bookmark);
          Result:=(pbmUser<>nil) and (pbmUser^.DataID=FCommon.FDataID);
          if Result then
          begin
               p:=PkbmRecord(pbmUser^.Bookmark);
// OutputDebugString(PChar('pbmUser:'+inttostr(integer(pbmUser))+' p:'+inttostr(integer(p))+' p.Data:'+inttostr(integer(p^.Data))+' us:'+inttostr(ord(p.UpdateStatus))+' usDeleted:'+inttostr(ord(usDeleted))));
               Result:=(p<>nil) and (p^.Data<>nil) and (p^.UniqueRecordID=pbmUser.RecordID) and (p^.UpdateStatus<>usDeleted);
          end;
     end;
end;

function TkbmCustomMemTable.BookmarkValid(Bookmark: TBookmark): boolean;
begin
     Result:=InternalBookmarkValid(Bookmark);
end;

function TkbmCustomMemTable.CompareBookmarks(Bookmark1, Bookmark2: TBookmark):Integer;
var
   pUser1,pUser2:PkbmUserBookmark;
   p1,p2:PkbmRecord;
begin
     // Check for invalid/uninitialized bookmarks
     if not (BookMarkValid(Bookmark1) and BookMarkValid(Bookmark2)) then
     begin
          Result:=2; //0;
          exit;
     end;

     // Check contents of bookmark.
     pUser1:=PkbmUserBookmark(Bookmark1);
     pUser2:=PkbmUserBookmark(Bookmark2);
     p1:=pUser1^.Bookmark;
     p2:=pUser2^.Bookmark;

     // Compare record contents.
     FCommon.Lock;
     try
        if FCurIndex=FIndexes.FRowOrderIndex then
           Result:=p1^.RecordID - p2^.RecordID
        else
        begin
             Result:=FCurIndex.CompareRecords(FCurIndex.FIndexFieldList,p1,p2,true,true,false,false);
             if Result=0 then Result:=p1^.RecordNo-p2^.RecordNo;
             if Result=0 then Result:=p1^.RecordID-p2^.RecordID;
        end;
     finally
        FCommon.Unlock;
     end;

     // Convert to -1,0,1 range.
     if Result<0 then Result:=-1
     else if Result>0 then Result:=1;
end;

{$IFDEF NEXTGEN}
procedure TkbmCustomMemTable.InternalGotoBookmark(Bookmark:TBookmark);
{$ELSE}
procedure TkbmCustomMemTable.InternalGotoBookmark(Bookmark:PkbmInternalGotoBookmark);
{$ENDIF}
var
   i:TkbmNativeInt;
   pb:PkbmRecord;
   pbUser:PkbmUserBookmark;
begin
     if Bookmark=nil then
        raise EMemTableError.CreateFmt(kbmBookmErr,[-200]);
     if not InternalBookmarkValid(Bookmark) then
        exit;

     pbUser:=PkbmUserBookmark(Bookmark);
     pb:=pbUser^.Bookmark;
     FCommon.Lock;
     try
        FCurIndex.SearchRecord(pb,i,true,true);
        if (i>=0) then FRecNo:=i;
     finally
        FCommon.Unlock;
     end;
end;

procedure TkbmCustomMemTable.InternalHandleException;
begin
     inherited;
end;

procedure TkbmCustomMemTable.SaveToFileViaFormat(const FileName:string; AFormat:TkbmCustomStreamFormat);
var
   Stream: TStream;
begin
     CheckActive;
     if not Assigned(AFormat) then
        raise EMemTableError.Create(kbmNoFormat);

     if (sfSaveAppend in AFormat.sfAppend) and FileExists(FileName) then
     begin
          Stream := TFileStream.Create(FileName,fmOpenReadWrite + fmShareDenyWrite);
          Stream.Seek(0,TSeekOrigin.soEnd);
     end
     else
         Stream := TFileStream.Create(FileName,fmCreate);
     try
        if (assigned(FOnSave)) then FOnSave(self,mtstFile,Stream);
        InternalSaveToStreamViaFormat(Stream,AFormat);
     finally
        Stream.Free;
     end;
end;

procedure TkbmCustomMemTable.SaveToFile(const FileName:string);
begin
     SaveToFileViaFormat(FileName,FDefaultFormat);
end;

procedure TkbmCustomMemTable.SaveToStreamViaFormat(Stream:TStream; AFormat:TkbmCustomStreamFormat);
begin
     CheckActive;
     if (assigned(FOnSave)) then FOnSave(self,mtstStream,Stream);
     InternalSaveToStreamViaFormat(Stream,AFormat);
end;

procedure TkbmCustomMemTable.SaveToStream(Stream:TStream);
begin
     SaveToStreamViaFormat(Stream,FDefaultFormat);
end;

procedure TkbmCustomMemTable.CloseBlob(Field:TField);
var
   pRec:PkbmRecord;
   pField:PByte;
   pBlob:PPkbmVarLength;
begin
     if FCurIndex=nil then // During shutdown.
        exit;
     if (FRecNo<0) or
        ((Assigned(FCurIndex) and (FRecNo>=FCurIndex.FReferences.Count))) or
        (not (State in [dsEdit,dsInactive])) then
     begin
          if Field.DataType in kbmBlobTypes then
          begin
               pRec:=PkbmRecord(ActiveBuffer);
               pField:=FCommon.GetFieldPointer(pRec,Field);

               pBlob:=PPkbmVarLength(pField+1);
               pField[0]:=kbmffNull;
               if pBlob^<>nil then
                  FreeVarLength(pBlob^);
               pBlob^:=nil;
          end;
     end;
end;


procedure TkbmCustomMemTable.InternalSaveToStreamViaFormat(AStream:TStream; AFormat:TkbmCustomStreamFormat);
begin
     if not Assigned(AFormat) then raise EMemTableError.Create(kbmNoFormat);

     with AFormat do
     begin
          FOrigStream:=AStream;
          FWorkStream:=nil;
          try
             BeforeSave(self);
             try
                Save(self);
             finally
                AfterSave(self);
             end;
          finally
             FWorkStream:=nil;
             FOrigStream:=nil;
          end;
     end;
end;

procedure TkbmCustomMemTable.InternalLoadFromStreamViaFormat(AStream:TStream; AFormat:TkbmCustomStreamFormat);
begin
     if not Assigned(AFormat) then raise EMemTableError.Create(kbmNoFormat);

     with AFormat do
     begin
          FOrigStream:=AStream;
          FWorkStream:=nil;
          try
             BeforeLoad(self);
             Load(self);
          finally
             AfterLoad(self);
             FOrigStream:=nil;
             FWorkStream:=nil;
          end;
     end;
end;

procedure TkbmCustomMemTable.LoadFromFileViaFormat(const FileName: string; AFormat:TkbmCustomStreamFormat);
var
   Stream: TStream;
begin
     Stream := TFileStream.Create(FileName, fmOpenRead+fmShareDenyWrite);
     try
        if assigned(FOnLoad) then FOnLoad(self,mtstFile,Stream);
        InternalLoadFromStreamViaFormat(Stream,AFormat);
     finally
        Stream.Free;
     end;
end;

procedure TkbmCustomMemTable.LoadFromFile(const FileName:string);
begin
     LoadFromFileViaFormat(FileName,FDefaultFormat);
end;

procedure TkbmCustomMemTable.LoadFromStreamViaFormat(Stream:TStream; AFormat:TkbmCustomStreamFormat);
begin
     if assigned(FOnLoad) then FOnLoad(self,mtstStream,Stream);
     InternalLoadFromStreamViaFormat(Stream,AFormat);
end;

procedure TkbmCustomMemTable.LoadFromStream(Stream:TStream);
begin
     if assigned(FOnLoad) then FOnLoad(self,mtstStream,Stream);
     InternalLoadFromStreamViaFormat(Stream,FDefaultFormat);
end;

procedure TkbmCustomMemTable.InternalEmptyTable;
var
   OldState:TkbmState;
begin
     OldState:=FState;
     FState:=mtstEmpty;
     try
        CheckBrowseMode;
        ClearBuffers;
        DataEvent(deDataSetChange, 0);
        FIndexes.EmptyAll;
        FRecNo:=-1;
        FReposRecNo:=-1;
     finally
        FState:=OldState;
     end;
end;

procedure TkbmCustomMemTable.EmptyTable;
begin
     Progress(0,mtpcEmpty);
     DisableControls;
     try
        FCommon.EmptyTables;
     finally
        EnableControls;
        Progress(100,mtpcEmpty);
     end;
end;

procedure TkbmCustomMemTable.PackTable;
begin
     Cancel;
     Commit;
     CheckPoint;
     Progress(0,mtpcPack);
     FState:=mtstPack;
     DisableControls;
     try
        ClearBuffers;
        FCommon._InternalPackRecords;
        First;
     finally
        EnableControls;
        Progress(100,mtpcPack);
        FState:=mtstBrowse;
     end;
end;

// Checkpoint a single record.
// Throws away old version records, and actually removes delete marked records.
function TkbmCustomMemTable.CheckPointRecord(RecordIndex:integer; const AForce:boolean = false; const AMarkAsInserted:boolean = false):TUpdateStatus;
var
   ARecord:PkbmRecord;
begin
     ARecord:=FCommon.FRecords.Items[RecordIndex];
     if ARecord=nil then
     begin
          Result:=usUnmodified;
          exit;
     end;

     // Check if allow to checkpoint.
     if (not AForce) and ((ARecord^.Flag and kbmrfDontCheckPoint)=kbmrfDontCheckPoint) then
     begin
          Result:=ARecord^.UpdateStatus;
          exit;
     end;

     // Check if versioning data, remove them.
     if ARecord^.PrevRecordVersion<>nil then
     begin
          FCommon._InternalFreeRecord(ARecord^.PrevRecordVersion,true,true);
          ARecord^.PrevRecordVersion:=nil
     end;

     // Check if deleted record, delete it real this time.
     if ARecord^.UpdateStatus=usDeleted then
     begin
          FCommon.ReflectToIndexes(self,mtiuhDelete,ARecord,nil,RecordIndex,false);
          FCommon._InternalDeleteRecord(ARecord);
          Result:=usDeleted;
     end
     else
     begin
          // Reset status flags.
          if AMarkAsInserted then
             ARecord^.UpdateStatus:=usInserted
          else
              ARecord^.UpdateStatus:=usUnModified;
          ARecord^.Flag:=ARecord^.Flag and (not kbmrfDontCheckPoint);
          Result:=ARecord^.UpdateStatus;
     end;
end;


// Define checkpoint for versioning.
// Throws away old version records, and actually removes delete marked records.
procedure TkbmCustomMemTable.CheckPoint(const AForce:boolean = false; const AMarkAsInserted:boolean = false);
var
   i:TkbmNativeInt;
   oEnableVersioning:boolean;
   ProgressCnt:integer;
begin
     if FAttachedTo<>nil then raise EMemTableError.Create(kbmCantCheckpointAttached);
     UpdateCursorPos;

     // Make sure operations are really happening and not just versioned.
     FCommon.FIsDataModified:=false;
     Progress(0,mtpcCheckPoint);
     ProgressCnt:=0;
     FCommon.Lock;
     try
        oEnableVersioning:=FCommon.FEnableVersioning;
        FCommon.FEnableVersioning:=false;
        FState:=mtstCheckPoint;
        ClearBuffers;
        for i:=FCommon.FRecords.Count-1 downto 0 do
        begin
             inc(ProgressCnt);
             ProgressCnt:=ProgressCnt mod 100;
             if ProgressCnt=0 then Progress(trunc(i/FCommon.FRecords.Count * 100),mtpcCheckPoint);
             if CheckpointRecord(i,AForce,AMarkAsInserted)<>usUnModified then
                FCommon.FIsDataModified:=true;
        end;

        FCommon.FDeletedCount:=0;
        FCommon.FEnableVersioning:=oEnableVersioning;
     finally
        FCommon.Unlock;
        First;
        Progress(100,mtpcCheckPoint);
        FState:=mtstBrowse;
     end;
end;

procedure TkbmCustomMemTable.SetCommaText(AString: String);
var
   stream:TMemoryStream;
begin
     EmptyTable;
     stream:=TMemoryStream.Create;
     try
        stream.Write(Pointer(AString)^,length(AString));
        stream.Seek(0,TSeekOrigin.soBeginning);
        LoadFromStreamViaFormat(stream,FCommaTextFormat);
     finally
        stream.free;
     end;
end;

function TkbmCustomMemTable.GetCommaText: String;
var
   stream:TStringStream;
begin
     Result:='';
     stream:=TStringStream.Create{$IFDEF FPC}(''){$ENDIF};
     try
        SaveToStreamViaFormat(stream,FCommaTextFormat);
        Result:=stream.DataString;
     finally
        stream.free;
     end;
end;

// Save persistent table.
procedure TkbmCustomMemTable.SavePersistent;
var
   TempFile:string;
   BackupFile : String;
begin
     if not Active then exit;

     // If persistent, save info to file.
     if (not FPersistentSaved) and (not (csDesigning in ComponentState))
        and FPersistent and (FPersistentFile<>'') and (FPersistentFormat<>nil) then
     begin
          TempFile:=ChangeFileExt(FPersistentFile,'.$$$');
          SaveToFileViaFormat(TempFile,FPersistentFormat);
          if FPersistentBackup then
          begin
               BackupFile:=ChangeFileExt(FPersistentFile,FPersistentBackupExt);
               try
                  SysUtils.DeleteFile(BackupFile);
               except
               end;
               SysUtils.RenameFile(FPersistentFile,BackupFile);
          end
          else
          begin
               try
                  SysUtils.DeleteFile(FPersistentFile);
               except
               end;
          end;
          SysUtils.RenameFile(TempFile,FPersistentFile);
          FPersistentSaved:=true;
     end;
end;

// Check if persistent file exists... ala does the table exist in storage.
function TkbmCustomMemTable.Exists:boolean;
begin
     Result:=FileExists(FPersistentFile);
end;

// Load persistent table.
procedure TkbmCustomMemTable.LoadPersistent;
begin
     if FPersistent and (FPersistentFile<>'') and FileExists(FPersistentFile) then
     begin
          FPersistent:=false;
          try
             LoadFromFileViaFormat(FPersistentFile,FPersistentFormat);
             first;
          finally
             FPersistent:=true;
          end;
     end;
     FPersistentSaved:=false;
end;

// Sneak in before the table is closed.
procedure TkbmCustomMemTable.DoBeforeClose;
begin
     // Check if not in browse mode.
     if (State in [dsEdit,dsInsert]) then Cancel;

     if not FBeforeCloseCalled then
        inherited DoBeforeClose;
//     SavePersistent;
     FBeforeCloseCalled:=true;
end;

// Sneak in before the table is opened.
procedure TkbmCustomMemTable.DoBeforeOpen;
begin
     inherited DoBeforeOpen;
end;

// Sneak in after the table is opened.
procedure TkbmCustomMemTable.DoAfterOpen;
begin
     // DoAfterOpen is not reentrant. Thus prevent that situation.
     if FDuringAfterOpen then exit;

     FDuringAfterOpen:=true;
     try
        Indexes.MarkAllDirty;
        UpdateIndexes;

        // Switch index.
        if FIndexFieldNames<>'' then
           SetIndexFieldNames(FIndexFieldNames)
        else if FIndexName<>'' then
           SetIndexName(FIndexName);

        // If to load data from form, do it.
        if FTempDataStorage<>nil then
        begin
             if FStoreDataOnForm then LoadFromStreamViaFormat(FTempDataStorage,FFormFormat);
             FTempDataStorage.free;
             FTempDataStorage:=nil;
        end;

        // If persistent, read info from file.
        LoadPersistent;

        // If filtering, reapply it.
        if Filtered then
        begin
             Filtered:=false;
             Filtered:=true;
        end;

        inherited DoAfterOpen;

        MasterChanged(self);
     finally
        FDuringAfterOpen:=false;
        if FAutoUpdateFieldVariables then UpdateFieldVariables;
     end;
end;

// Sneak in after a post to update attached tables.
procedure TkbmCustomMemTable.DoAfterPost;
begin
     if FAttachedAutoRefresh then
        FCommon.RefreshTables(Self);

     // Check if to reposition.
     if FAutoReposition and (FReposRecNo>=0) then
     begin
          FRecNo:=FReposRecNo;
          FReposRecNo:=-1;
          Resync([]);
     end;

     inherited DoAfterPost;
end;

// Sneak in after a delete to update attached tables.
procedure TkbmCustomMemTable.DoAfterDelete;
begin
     if FAttachedAutoRefresh then
        FCommon.RefreshTables(self);

     FReposRecNo:=-1; // Nothing to reposition to.
     inherited DoAfterDelete;
end;

// Locate record.
// If the keyfields are the same as sorted fields and the table is currently sorted,
// it will make a fast binary search. Otherwise it will make a sequential search.
// Binary searches dont take partial record in account.
function TkbmCustomMemTable.LocateRecord(const KeyFields: string; const KeyValues: Variant; Options: TLocateOptions):Integer;
var
   KeyFieldsList:TkbmFieldList;
   KeyRecord:PkbmRecord;
   i:integer;
   Index:TkbmNativeInt;
   Found:boolean;
begin
     Result := -1;
     I := VarArrayDimCount(KeyValues);
     if I > 1 then
        raise EMemTableError.Create(kbmVarArrayErr);

     CheckBrowseMode;
     CursorPosChanged;

     // Prepare list of fields representing the keys to search for.
     KeyFieldsList := TkbmFieldList.Create;
     try
        KeyFieldsList.Build(self,KeyFields);
        KeyFieldsList.LocateOptions:=Options;

        // Setup key options.
        if loCaseInsensitive in Options then KeyFieldsList.SetOptions(self,mtifoCaseInsensitive,KeyFields);
        if loPartialKey in Options then KeyFieldsList.SetOptions(self,mtifoPartial,KeyFields);

        // Populate a keyrecord.
        KeyRecord:=FCommon._InternalAllocRecord;
        try
           // Fill it with values.
           PopulateRecord(KeyRecord,KeyFields,KeyValues);

           // Locate record.
           Index:=-1;
           Indexes.Search(KeyFieldsList,KeyRecord,mtstFirst,true,true,FAutoAddIndexes,Index,Found);
           if Found then
              Result:=Index;

        finally
           // Free reference record.
           FCommon._InternalFreeRecord(KeyRecord,true,false);
        end;

     finally
        KeyFieldsList.Free;
     end;
end;

function TkbmCustomMemTable.Lookup(const KeyFields: string; const KeyValues: Variant; const ResultFields: string): Variant;
var
   n:integer;
   oState:TDataSetState;
begin
     Result := Null;
     n:=LocateRecord(KeyFields, KeyValues, []);
     SetFound(n>=0);
     if n>=0 then
     begin
          oState:=SetTempState(dsCalcFields);
          try
             CalculateFields(PkbmCalculateFields(FCurIndex.FReferences[n]));
             Result:=FieldValues[ResultFields];
          finally
             RestoreState(oState);
          end;
     end;
end;

function TkbmCustomMemTable.LookupByIndex(const IndexName:string; const KeyValues:Variant;
                                          const ResultFields:string; RespFilter:boolean):Variant;
var
   i:TkbmNativeInt;
   idx:TkbmIndex;
   KeyFieldsList:TkbmFieldList;
   KeyRecord:PkbmRecord;
   f:boolean;
   oState:TDataSetState;
begin
     Result:=null;
     idx:=GetIndexByName(IndexName);
     if idx=nil then exit;

     if VarArrayDimCount(KeyValues)>1 then
        raise EMemTableError.Create(kbmVarArrayErr);

     CheckBrowseMode;
     CursorPosChanged;

     // Prepare list of fields representing the keys to search for.
     KeyFieldsList := TkbmFieldList.Create;
     try
        KeyFieldsList.Build(self,idx.IndexFields);

        // Populate a keyrecord.
        KeyRecord := FCommon._InternalAllocRecord;
        try
           // Fill it with values.
           PopulateRecord(KeyRecord,idx.IndexFields,KeyValues);

           // Locate record.
           i:=-1;
           if not((idx.Search(KeyFieldsList,KeyRecord,mtstFirst,true,RespFilter,i,f)=0) and (i>=0)) then
              i:=-1;
        finally
           // Free reference record.
           FCommon._InternalFreeRecord(KeyRecord,true,false);
     end;
     finally
        KeyFieldsList.Free;
     end;

     SetFound(f);
     if f then
     begin
          oState:=SetTempState(dsCalcFields);
          try
             CalculateFields(PkbmCalculateFields(idx.FReferences[i]));
             Result:=FieldValues[ResultFields];
          finally
             RestoreState(oState);
          end;
     end;
end;

function TkbmCustomMemTable.Locate(const KeyFields: string; const KeyValues: Variant; Options: TLocateOptions): Boolean;
var
   n:integer;
begin
     DoBeforeScroll;
     n:=LocateRecord(KeyFields, KeyValues, Options);
     Result:=(n>=0);
     SetFound(Result);
     if n>=0 then
     begin
          FRecNo:=n;
          Resync([rmExact, rmCenter]);
          DoAfterScroll;
     end;
end;

// Copy properties from one field to another.
procedure TkbmCustomMemTable.CopyFieldProperties(Source,Destination:TField);
begin
     // Did we get valid parameters.
     if (Source=nil) or (Destination=nil) or (Source=Destination) then exit;

     // Copy general properties.
     with Source do
     begin
{$IFNDEF FPC}
          Destination.EditMask:=EditMask;
{$ENDIF}
          Destination.DisplayWidth:=DisplayWidth;
          Destination.DisplayLabel:=DisplayLabel;
          Destination.Required:=Required;
          Destination.ReadOnly:=ReadOnly;
          Destination.Visible:=Visible;
          Destination.DefaultExpression:=DefaultExpression;
          Destination.Alignment:=Alignment;
          Destination.ProviderFlags:=ProviderFlags;
          if not Destination.DataSet.Active then
          begin
               Destination.Lookup:=Lookup;
               Destination.LookupCache:=LookupCache;
               Destination.LookupDataSet:=LookupDataSet;
               Destination.LookupKeyFields:=LookupKeyFields;
               Destination.LookupResultField:=LookupResultField;
               Destination.KeyFields:=KeyFields;
          end;
     end;

     // Copy field type specific properties.
     if Source is TNumericField then
        with TNumericField(Source) do
        begin
             TNumericField(Destination).DisplayFormat:=DisplayFormat;
             TNumericField(Destination).EditFormat:=EditFormat;
        end;

     if Source is TIntegerField then
        with TIntegerField(Source) do
        begin
             TIntegerField(Destination).MaxValue:=MaxValue;
             TIntegerField(Destination).MinValue:=MinValue;
        end;

     if Source is TDateTimeField then
        with TDateTimeField(Source) do
             TDateTimeField(Destination).DisplayFormat:=DisplayFormat;

     if Source is TBooleanField then
        with TBooleanField(Source) do
             TBooleanField(Destination).DisplayValues:=DisplayValues;

     if Source is TStringField then
        with TStringField(Source) do
             TStringField(Destination).Transliterate:=Transliterate;

     if Source is TFloatField then
        with TFloatField(Source) do
        begin
             TFloatField(Destination).MaxValue:=MaxValue;
             TFloatField(Destination).MinValue:=MinValue;
             TFloatField(Destination).Precision:=Precision;
             TFloatField(Destination).currency:=currency;
        end;

     if Source is TBCDField then
        with TBCDField(Source) do
        begin
             TBCDField(Destination).MaxValue:=MaxValue;
             TBCDField(Destination).MinValue:=MinValue;
             TBCDField(Destination).currency:=currency;
        end;

     if Source is TBlobField then
        with TBlobField(Source) do
        begin
             TBlobField(Destination).BlobType:=BlobType;
             TBlobField(Destination).Transliterate:=Transliterate;
        end;

     // Call eventhandler if needed.
     if Assigned(FOnSetupFieldProperties) then FOnSetupFieldProperties(self,Destination);
end;

// Copy properties from source to destination.
// Handles different fieldorder between the two datasets.
procedure TkbmCustomMemTable.CopyFieldsProperties(Source,Destination:TDataSet);
var
   i:TkbmNativeInt;
   fc:TkbmNativeInt;
   f:TField;
begin
     // Did we get valid parameters.
     if (Source=nil) or (Destination=nil) or (Source=Destination) then exit;

     // Copy constraints from source to destination.
     fc:=Destination.FieldCount-1;
     for i:=0 to fc do
     begin
          // Find matching fieldnames on both sides. If fieldname not found, dont copy it.
          f:=Source.FindField(Destination.Fields[i].FieldName);
          if f=nil then continue;
          CopyFieldProperties(f, Fields[i]);
     end;
end;

// Copy records from source to destination.
// Handles different fieldorder between the two datasets.
// Supports field mapping.
// Returns the number of records copied.
function TkbmCustomMemTable.CopyRecords(Source,Destination:TDataSet;Count:TkbmNativeInt; IgnoreErrors:boolean; Mapping:string; WideStringAsUTF8:boolean):TkbmNativeInt;
var
   i:TkbmNativeInt;
   fc:TkbmNativeInt;
   f:TField;
   fsrc,fdst:TField;
   fi:array of integer;
   Accept:boolean;
   RecCnt:integer;
   ProgressCnt:integer;
   cpAutoInc:boolean;
   slMap:TStringList;
   s,
   sExpectedSourceFieldName:string;
begin
     Result:=0;

     // Did we get valid parameters.
     if (Source=nil) or (Destination=nil) or (Source=Destination) then exit;

     // Build name index relations between destination and source dataset.
     slMap:=TStringList.Create;
     try
        slMap.Delimiter:=';';
        slMap.StrictDelimiter:=true;
        slMap.DelimitedText:=Mapping;

        fc:=Destination.FieldCount-1;
        SetLength(fi,fc+1);
        Progress(0,mtpcCopy);
        for i:=0 to fc do
        begin
             // Check if not a datafield or not a supported field, dont copy it.
             case Destination.Fields[i].FieldKind of
                  fkLookup: fi[i]:=-2; // Dont copy, dont clearout.
                  fkData,fkInternalCalc,fkCalculated:
                    begin
                         // If unknown datatype, dont copy, just leave untouched.
                         if not (Destination.Fields[i].DataType in (kbmSupportedFieldTypes)) then
                         begin
                              fi[i]:=-1;
                              continue;
                         end;

                         // Check if to copy autoinc from source.
                         if Destination.Fields[i].DataType=ftAutoInc then
                         begin
                              cpAutoInc:=Destination.isEmpty;
                              if not cpAutoInc then
                              begin
                                   fi[i]:=-1;
                                   continue;
                              end;
                         end;

                         // Find matching fieldnames on both sides. If fieldname not found, dont copy it, just clearout.
                         sExpectedSourceFieldName:=Destination.Fields[i].FieldName;
                         s:=slMap.Values[sExpectedSourceFieldName];
                         if s<>'' then
                            sExpectedSourceFieldName:=s;

                         f:=Source.FindField(sExpectedSourceFieldName);
                         if f=nil then
                         begin
                              fi[i]:=-1;
                              continue;
                         end;

   { Commented out to allow copying non datafields.
                         // If not a datafield just clearout.
                         if f.FieldKind<>fkData then
                         begin
                              fi[i]:=-1;
                              continue;
                         end;
   }

                         // Else copy the field.
                         fi[i]:=f.Index;
                    end;
             else
                 // Other fieldkind, dont copy, just clearout.
                 fi[i]:=-1;
             end;
        end;
     finally
        slMap.Free;
     end;

     // Check number of records in source.
     if Assigned(FOnProgress) then
     begin
          RecCnt:=Source.RecordCount;
          if (RecCnt<=0) then Progress(50,mtpcCopy);
     end
     else RecCnt:=-1;

     // Copy data.
     FLoadedCompletely:=true;
     if (RecCnt<=0) then Progress(50,mtpcCopy);
     ProgressCnt:=0;
     while not Source.EOF do
     begin
          // Update progress.
          if (RecCnt>0) then
          begin
               inc(ProgressCnt);
               if (ProgressCnt mod 100)=0 then Progress(trunc(ProgressCnt/RecCnt*100),mtpcCopy);
          end;

          // Check acceptance of record.
          Accept:=true;
          if Assigned(FOnSaveRecord) and (Source=self) then FOnSaveRecord(Self,Accept);
          if not Accept then
          begin
               Source.Next;
               continue;
          end;

          Destination.Append;
          for i:=0 to fc do
          begin
               try
                  if fi[i]>=0 then
                  begin
                       fsrc:=Source.Fields[fi[i]];
                       fdst:=Destination.Fields[i];

                       if Assigned(FOnSaveField) and (Source=self) then FOnSaveField(Self,i,fsrc);

                       if fsrc.IsNull then
                          fdst.Clear
                       else
                       if fsrc.DataType=ftLargeint then
                          fdst.AsString:=fsrc.AsString

                       // Check if to do automatic UTF8 conversion.
                       else if WideStringAsUTF8 and ((fsrc.DataType in [ftWideString
{$IFDEF KBMMEMTABLE_SUPPORT_FT_WIDEMEMO}
                              ,ftWideMemo
{$ENDIF}
{$IFDEF KBMMEMTABLE_SUPPORT_FT_FIXEDWIDECHAR}
                              ,ftFixedWideChar
{$ENDIF}
                              ]) or (fdst.DataType in [ftWideString
{$IFDEF KBMMEMTABLE_SUPPORT_FT_WIDEMEMO}
                              ,ftWideMemo
{$ENDIF}
{$IFDEF KBMMEMTABLE_SUPPORT_FT_FIXEDWIDECHAR}
                              ,ftFixedWideChar
{$ENDIF}
                              ])) then
                       begin
                            if fsrc.DataType=fdst.DataType then
                               fdst.Value:=fsrc.Value
                            else if fsrc.DataType in [ftString,ftFixedChar] then
{$IFDEF NEXTGEN}
                               fdst.Value:=TEncoding.UTF8.GetString(fsrc.AsBytes)
{$ELSE}
                               fdst.Value:= {$IFDEF FPC}
                                             UTF8Decode(UTF8String(fsrc.AsString))
                                            {$ELSE}
                                             UTF8ToWideString(RawByteString(fsrc.AsString))
                                            {$ENDIF}
{$ENDIF}
                            else if fdst.DataType in [ftString,ftFixedChar] then
{$IFDEF NEXTGEN}
                               fdst.AsBytes:=TEncoding.UTF8.GetBytes(fsrc.Value)
{$ELSE}
                               fdst.AsString:=string(UTF8Encode(fsrc.Value))
{$ENDIF}
                            else
                               fdst.Value:=fsrc.Value;
                       end

                       // Special error handling for ftOraClob and ftOraBlob fields
                       else if ((fsrc is TBlobField) and (TBlobField(fsrc).BlobType in [ftOraClob,ftOraBlob])) then
                       begin
                            try
{$IFDEF KBMMW_SUPPORTASBYTES}
                               fdst.AsBytes:=fsrc.AsBytes;
{$ELSE}
                               fdst.AsString:=fsrc.AsString;
{$ENDIF}
                            except
                               on E: Exception do
                               begin
                                 // swallow the BDE error, check classname not to import BDE classes.
                                 if E.ClassName='EDBEngineError' then
                                    // ***IMPACT ALERT***
                                    // this leaves the field defined but empty this breaks previous
                                    // functionality where this and subsequent fields just weren't
                                    // defined at all
                                    fdst.Clear
                                 else
                                    raise E;
                               end;
                            end
                       end
                       else if (fdst.DataType in [ftDate,ftTime,ftDateTime]) and (fsrc.DataType in [ftFloat,ftInteger,ftWord,ftSmallInt,ftCurrency]) then
                           fdst.AsDateTime:=fsrc.AsFloat
                       else if fsrc.ClassType<>fdst.ClassType then
                          fdst.AsString:=fsrc.AsString
                       else
                          fdst.Value:=fsrc.Value;
                       if Assigned(FOnLoadField) and (Destination=self) then FOnLoadField(Self,i,fdst);
                  end;
               except
                  if not IgnoreErrors then raise;
               end;
          end;

          Accept:=true;
          if Assigned(FOnLoadRecord) and (Destination=self) then FOnLoadRecord(Self,Accept);
          if Accept then
          begin
               try
                  Destination.Post;
               except
                  if not IgnoreErrors then
                     raise
                  else
                     Destination.Cancel;
               end;
               inc(Result);
               if (Count>0) and (Result>=Count) then
               begin
                    FLoadedCompletely:=Source.EOF;
                    break;
               end;
          end
          else Destination.Cancel;

          Source.next;
     end;
     Progress(100,mtpcCopy);
end;

// Assign the contents of active record in source to active record in destination.
// Handles different fieldorder between the two datasets.
procedure TkbmCustomMemTable.AssignRecord(Source,Destination:TDataSet);
var
   i:TkbmNativeInt;
   fc:TkbmNativeInt;
   f,sf:TField;
   fi:array of integer;
   Accept:boolean;
begin
     // Did we get valid parameters.
     if (Source=nil) or (Destination=nil) or (Source=Destination) then exit;

     // Build name index relations between destination and source dataset.
     fc:=Destination.FieldCount-1;
     SetLength(fi,fc+1);
     Progress(0,mtpcCopy);
     for i:=0 to fc do
     begin
          // Check if not a datafield or not a supported field, dont copy it.
          case Destination.Fields[i].FieldKind of
               fkLookup: fi[i]:=-2; // Dont copy, dont clearout.
               fkData,fkInternalCalc,fkCalculated:
                 begin
                      // If unknown datatype or autoinc field, dont copy, just leave untouched.
                      if not (Destination.Fields[i].DataType in (kbmSupportedFieldTypes)) then
                      begin
                           fi[i]:=-1;
                           continue;
                      end;

                      // Find matching fieldnames on both sides. If fieldname not found, dont copy it, just clearout.
                      f:=Source.FindField(Destination.Fields[i].FieldName);
                      if f=nil then
                      begin
                           fi[i]:=-1;
                           continue;
                      end;

{ Commented out to allow copying non datafields.
                      // If not a datafield just clearout.
                      if f.FieldKind<>fkData then
                      begin
                           fi[i]:=-1;
                           continue;
                      end;
}

                      // Else copy the field.
                      fi[i]:=f.Index;
                 end;
          else
              // Other fieldkind, dont copy, just clearout.
              fi[i]:=-1;
          end;
     end;

     // Determine if to copy.
     Accept:=true;
     if Assigned(FOnSaveRecord) and (Source=self) then FOnSaveRecord(Self,Accept);
     if not Accept then exit;

     // Copy data.
     Destination.Edit;
     for i:=0 to fc do
     begin
          if Assigned(FOnSaveField) and (Source=self) then FOnSaveField(Self,i,Source.Fields[i]);
          if fi[i]>=0 then
          begin
               sf:=Source.Fields[fi[i]];
               f:=Destination.Fields[i];

               if sf.IsNull then
                  f.Clear
               else
               begin
                    if sf.DataType=ftLargeInt then
                       f.AsString:=sf.AsString
                    else
                        f.Value:=sf.Value;
               end;

          end;
          if Assigned(FOnLoadField) and (Destination=self) then FOnLoadField(Self,i,Destination.Fields[i]);
     end;

     // Determine if to post.
     if Assigned(FOnLoadRecord) and (Destination=self) then FOnLoadRecord(Self,Accept);
     if Accept then Destination.post
     else Destination.Cancel;

     Progress(100,mtpcCopy);
end;

// Update destination with records not matching or existing in source.
function TkbmCustomMemTable.UpdateRecords(Source,Destination:TDataSet; KeyFields:string; Count:TkbmNativeInt; Flags:TkbmMemTableUpdateFlags; Mapping:string):TkbmNativeInt;
var
   i:TkbmNativeInt;
   fc:TkbmNativeInt;
   f:TField;
   fi:array of integer;
   Accept,DoUpdate:boolean;
   KeyValues:Variant;
   KeyFieldsList:TkbmFieldList;
   RecCnt:integer;
   ProgressCnt:integer;
   slMap:TStringList;
   s,
   sExpectedSourceFieldName:string;
begin
     Progress(0,mtpcUpdate);
     FState:=mtstUpdate;
     KeyFieldsList := TkbmFieldList.Create;
     try
        KeyFieldsList.Build(self,KeyFields);
        if KeyFieldsList.Count > 1 then
           KeyValues:=VarArrayCreate([0, KeyFieldsList.Count-1 ], varVariant)
        else
            KeyValues:=null;
        Result:=0;

        // Did we get valid parameters.
        if (Source=nil) or (Destination=nil) or (Source=Destination) then exit;

        // Build name index relations between destination and source dataset.
        slMap:=TStringList.Create;
        try
           slMap.Delimiter:=';';
           slMap.StrictDelimiter:=true;
           slMap.DelimitedText:=Mapping;

           fc:=Destination.FieldCount-1;
           SetLength(fi,fc+1);
           for i:=0 to fc do
           begin
                // Check if not a datafield or not a supported field, dont copy it.
                case Destination.Fields[i].FieldKind of
                     fkLookup: fi[i]:=-2; // Dont copy, dont clearout.
                     fkData,fkInternalCalc,fkCalculated:
                       begin
                            // If unknown datatype, dont copy, just clearout.
                            if not (Destination.Fields[i].DataType in (kbmSupportedFieldTypes)) then
                            begin
                                 fi[i]:=-1;
                                 continue;
                            end;

                            // Find matching fieldnames on both sides. If fieldname not found, dont copy it, just clearout.
                            sExpectedSourceFieldName:=Destination.Fields[i].FieldName;
                            s:=slMap.Values[sExpectedSourceFieldName];
                            if s<>'' then
                               sExpectedSourceFieldName:=s;

                            f:=Source.FindField(sExpectedSourceFieldName);
                            if f=nil then
                            begin
                                 fi[i]:=-1;
                                 continue;
                            end;

                            // Else copy the field.
                            fi[i]:=f.Index;
                       end;
                else
                    // Other fieldkind, dont copy, just clearout.
                    fi[i]:=-1;
                end;

                // Check if not to clear out afterall.
                if (mtufDontClear in Flags) and (fi[i]=-1) then fi[i]:=-2;
           end;
        finally
           slMap.Free;
        end;

        // Copy data.
        Source.First;
        RecCnt:=Source.RecordCount;
        if (RecCnt<=0) then Progress(50,mtpcCopy);
        ProgressCnt:=0;
        while not Source.EOF do
        begin
             // Update progress.
             if (RecCnt>0) then
             begin
                  inc(ProgressCnt);
                  if (ProgressCnt mod 100)=0 then Progress(trunc(ProgressCnt/RecCnt*100),mtpcCopy);
             end;

             Accept:=true;
             if Assigned(FOnSaveRecord) and (Source=self) then FOnSaveRecord(Self,Accept);
             if not Accept then
             begin
                  Source.Next;
                  continue;
             end;

             // Convert variant array of values to a list of values.
             if KeyFieldsList.Count > 1 then
             begin
                  for i:=0 to KeyFieldsList.count-1 do
                      KeyValues[i]:=KeyFieldsList.Fields[i].AsVariant;
             end
             else
                 KeyValues:=KeyFieldsList.Fields[0].AsVariant;

             // Look for record in dest. dataset to determine if to append or update record.
             DoUpdate:=true;
             if not Destination.Locate(KeyFields,KeyValues,[]) then
             begin
                  if (mtufAppend in Flags) then
                     Destination.Append
                  else
                      DoUpdate:=false;
             end
             else
             begin
                  if (mtufEdit in Flags) then
                     Destination.Edit
                  else
                      DoUpdate:=false;
             end;

             if DoUpdate then
             begin
                  // Update record fields.
                  for i:=0 to fc do
                  begin
                       if Assigned(FOnSaveField) and (Source=self) then FOnSaveField(Self,i,Source.Fields[i]);
                       if fi[i]>=0 then
                       begin
                            if Source.Fields[fi[i]].IsNull then
                               Destination.Fields[i].Clear
                            else
                                Destination.Fields[i].AsString:=Source.Fields[fi[i]].AsString;
                       end;
                       if Assigned(FOnLoadField) and (Destination=self) then FOnLoadField(Self,i,Destination.Fields[i]);
                  end;

                  Accept:=true;
                  if Assigned(FOnLoadRecord) and (Destination=self) then FOnLoadRecord(Self,Accept);
                  if Accept then
                  begin
                       Destination.Post;
                       inc(Result);
                       if (Count>0) and (Result>=Count) then break;
                  end
                  else Destination.Cancel;
             end;

             Source.next;
        end;

     finally
        KeyFieldsList.Free;
        Progress(100,mtpcUpdate);
        FState:=mtstBrowse;
     end;
end;

procedure TkbmCustomMemTable.UpdateToDataSet(Destination:TDataSet; KeyFields:string; Flags:TkbmMemTableUpdateFlags; Mapping:string='');
var
   DestActive:boolean;
   DestDisabled:boolean;
begin
     CheckBrowseMode;

     if Destination=self then exit;

     if (assigned(FOnSave)) then FOnSave(self,mtstDataSet,nil);

     // Remember state of destination.
     DestActive:=Destination.Active;
     DestDisabled:=Destination.ControlsDisabled;

     // Dont update controls while appending to destination
     if not DestDisabled then Destination.DisableControls;
     DisableControls;
     try
        try
           // Open destination
           if not DestActive then Destination.Open;
           Destination.CheckBrowseMode;
           Destination.UpdateCursorPos;

           // Open this if not opened.
           Open;
           CheckBrowseMode;

           // Move to first record in this.
           First;
           UpdateRecords(self,Destination,KeyFields,-1,Flags,Mapping);
        finally
           Destination.First;
        end;
     finally
        EnableControls;
        if not DestActive then Destination.Close;
        if not DestDisabled then Destination.EnableControls;
     end;
end;

procedure TkbmCustomMemTable.UpdateToDataSet(Destination:TDataSet; KeyFields:string; Mapping:string='');
begin
     UpdateToDataSet(Destination,KeyFields,[mtufEdit,mtufAppend],Mapping);
end;

// Fill the memorytable with data from another dataset.
procedure TkbmCustomMemTable.LoadFromDataSet(Source:TDataSet; CopyOptions:TkbmMemTableCopyTableOptions; Mapping:string='');
var
   SourceActive:boolean;
   SourceDisabled:boolean;
   OldMasterSource:TDataSource;
   OldFiltered:boolean;
   OldEnableIndexes:boolean;
   BM:TBookmark;
   IgnoreErrors:boolean;
   widestringasutf8:boolean;
begin
     if Source=self then exit;

     // Check if specified append together with structure. Not allowed.
     if (mtcpoAppend in CopyOptions) and ((mtcpoStructure in CopyOptions) or (mtcpoProperties in CopyOptions)) then
        raise EMemTableError.Create(kbmCannotMixAppendStructure);

     FState:=mtstLoad;

     if (assigned(FOnLoad)) then FOnLoad(self,mtstDataSet,nil);

     // If not to append, close this table.
     if (mtcpoStructure in CopyOptions) then
        Close
     else
         if not (mtcpoAppend in CopyOptions) then EmptyTable;

     // Remember state of source.
     SourceActive:=Source.Active;
     SourceDisabled:=Source.ControlsDisabled;

     // Remember state of this.
     OldFiltered:=Filtered;
     OldMasterSource:=MasterSource;
     OldEnableIndexes:=EnableIndexes;
     if not (mtcpoDontDisableIndexes in CopyOptions) then EnableIndexes:=false;

     widestringasutf8:=(mtcpoWideStringUTF8 in CopyOptions);

     IgnoreErrors:=mtcpoIgnoreErrors in CopyOptions;
     FIgnoreReadOnly:=true;

     // Dont update controls while scrolling through source.
     if not SourceDisabled then Source.DisableControls;
     DisableControls;
     try
        if not SourceActive then Source.Open;
        try                                  // Not all datasets support this.
           BM:=Source.GetBookmark;
        except
           BM:=nil;
        end;
        try

           // Dont want to check filtering while copying.
           Filtered := False;
           MasterSource:=nil;

           // Open source.
           Source.CheckBrowseMode;
           try
              Source.UpdateCursorPos;      // Not all datasets supports this.
           except
           end;

           // Create this memorytable as a copy of the other one.
           if mtcpoStructure in CopyOptions then CreateTableAs(Source,CopyOptions);
           if not Active then Open;

           // Copy fieldproperties from source after open to also copy properties of default fields.
           if (not (mtcpoAppend in CopyOptions)) and (mtcpoProperties in CopyOptions) then
              CopyFieldsProperties(Source,self);

           CheckBrowseMode;

           // Move to first record in source.
           if not Source.BOF then Source.First;
           FLoadCount:=CopyRecords(Source,self,FLoadLimit,IgnoreErrors,Mapping,widestringasutf8);
           First;
        finally
           EnableIndexes:=OldEnableIndexes;
           FCommon.MarkIndexesDirty;
           FCommon.UpdateIndexes;
           try
              Source.GotoBookmark(BM);                     // Not all datasets supports this.
           except
           end;
           try                                             // Not all datasets supports this.
              if BM<>nil then Source.FreeBookmark(BM);
           except
           end;
        end;
     finally
        FIgnoreReadOnly:=false;
        if not SourceActive then Source.Close;
        if not SourceDisabled then Source.EnableControls;
        Filtered:=OldFiltered;
        MasterSource:=OldMasterSource;
        EnableControls;
        UpdateCursorPos;
        CursorPosChanged;
        FState:=mtstBrowse;
     end;
end;

// Append the data in this memory table to another dataset.
procedure TkbmCustomMemTable.SaveToDataSet(Destination:TDataSet; CopyOptions:TkbmMemTableCopyTableOptions = []; Mapping:string='');
var
   DestActive:boolean;
   DestDisabled:boolean;
   IgnoreErrors:boolean;
   widestringasutf8:boolean;
begin
     if Destination=self then exit;

     IgnoreErrors:=mtcpoIgnoreErrors in CopyOptions;
     widestringasutf8:=(mtcpoWideStringUTF8 in CopyOptions);

     FState:=mtstSave;
     if (assigned(FOnSave)) then FOnSave(self,mtstDataSet,nil);

     // Remember state of destination.
     DestActive:=Destination.Active;
     DestDisabled:=Destination.ControlsDisabled;

     // Dont update controls while appending to destination
     if not DestDisabled then Destination.DisableControls;
     DisableControls;

     try
        // Open destination
        if not DestActive then Destination.Open;
        try
           Destination.CheckBrowseMode;
           Destination.UpdateCursorPos;

           // Open this if not opened.
           Open;
           CheckBrowseMode;

           // Move to first record in this.
           First;
           FSaveCount:=CopyRecords(self,Destination,FSaveLimit,IgnoreErrors,Mapping,widestringasutf8);
        finally
           Destination.First;
        end;

     finally
        EnableControls;
        if not DestActive then Destination.Close;
        if not DestDisabled then Destination.EnableControls;
        FState:=mtstBrowse;
     end;
end;

function TkbmCustomMemTable.IsSequenced: Boolean;
begin
     Result:=not Filtered;
end;

// Record rearranging.

// Move record from one place in table to another.
// Only rearranges the roworder index.
function TkbmCustomMemTable.MoveRecord(Source, Destination: TkbmNativeInt): Boolean;
var
   p: Pointer;
begin
     Result := False;
     if FCurIndex<>Indexes.FRowOrderIndex then exit;

     {Because property RecNo has values 1..FRecords.Count
      and FRecNo has values 0..FRecords.Count - 1}
     Dec(Source);
     Dec(Destination);

     if (Source <> Destination) and (Source > -1) and (Source < FCurIndex.FReferences.Count)
        and (Destination > -1) and (Destination < FCurIndex.FReferences.Count) then
     begin
          p:=FCurIndex.FReferences[Source];
          FCurIndex.FReferences.Delete(Source);
          FCurIndex.FReferences.Insert(Destination,p);
          Result:=true;
     end;
end;

// Copies record to another place in the table (roworder index).
// Destination=0 means append to end of roworder index.
// Only rearranges the roworder index.
function TkbmCustomMemTable.CopyRecord(Source, Destination: TkbmNativeInt): Boolean;
var
   p,pCopyRec: PkbmRecord;
begin
     Result := False;
     if FCurIndex<>Indexes.FRowOrderIndex then exit;

     if (Source <> Destination) and (Source > 0) and (Source <= FCurIndex.FReferences.Count)
        and (Destination >= -1) and (Destination <= FCurIndex.FReferences.Count) then
     begin
          p:=FCurIndex.FReferences[Source-1];

          // Copy the reference record.
          pCopyRec:=FCommon._InternalCopyRecord(p,true);

          // Update indexes and add physical record.
          FCommon.ReflectToIndexes(self,mtiuhInsert,nil,pCopyRec,Destination,false);

          // Append the reference record.
          pCopyRec^.TransactionLevel:=TransactionLevel;
          pCopyRec^.Flag:=pCopyRec^.Flag or kbmrfInTable;
          IsDataModified:=true;

          DrawAutoInc(pCopyRec);
          PostAutoInc(pCopyRec);
          FCommon._InternalAppendRecord(pCopyRec);
          Result:=true;
     end;
end;

// Move record to the specified destination.
function TkbmCustomMemTable.MoveCurRecord(Destination: TkbmNativeInt): Boolean;
begin
     Result := MoveRecord(RecNo,Destination);
end;

// Sorting.

// Callback function for TDataset to know if specified field is an index.
function TkbmCustomMemTable.GetIsIndexField(Field:TField):Boolean;
begin
     Result:=FIndexList.IndexOf(Field)>=0;
end;

class function TkbmCustomMemTable.GetAggregateFieldName(const AFieldName:string; const AOptions:TkbmifoOptions):string;
begin
     if mtifoAggSum in AOptions then
        Result:=AFieldName+'_SUM'
     else if mtifoAggMin in AOptions then
        Result:=AFieldName+'_MIN'
     else if mtifoAggMax in AOptions then
        Result:=AFieldName+'_MAX'
     else if mtifoAggCount in AOptions then
        Result:=AFieldName+'_COUNT'
     else if mtifoAggAvg in AOptions then
        Result:=AFieldName+'_AVG'
     else if mtifoAggStdDev in AOptions then
        Result:=AFieldName+'_STDDEV'
     else if mtifoAggUsr1 in AOptions then
        Result:=AFieldName+'_USR1'
     else if mtifoAggUsr2 in AOptions then
        Result:=AFieldName+'_USR2'
     else if mtifoAggUsr3 in AOptions then
        Result:=AFieldName+'_USR3'
     else
         Result:=AFieldName;
end;

class function TkbmCustomMemTable.GetExtractFieldOption(const AExtractOption:string):TkbmifoOptions;
var
   p:integer;
   sMainOption,sSubOption:string;
begin
     p:=pos(':',AExtractOption);
     if (p>0) then
     begin
          sMainOption:=Copy(AExtractOption,1,p-1);
          sSubOption:=Copy(AExtractOption,p+1,length(AExtractOption));
     end
     else
     begin
          sMainOption:=AExtractOption;
          sSubOption:='';
     end;

     if sMainOption='DATE' then
        Result:=[mtifoExtract,mtifoAsDate]
     else if sMainOption='TIME' then
        Result:=[mtifoExtract,mtifoAsTime]
     else if sMainOption='DATETIME' then
        Result:=[mtifoExtract,mtifoAsDateTime]
     else if sMainOption='N' then
        Result:=[mtifoExtract,mtifoIgnoreNull]
     else
         Result:=[];

     if sSubOption='N' then
        Result:=Result+[mtifoIgnoreNull];
end;

class function TkbmCustomMemTable.GetAggregateFieldOption(const AAggregateFunction:string):TkbmifoOptions;
begin
     if AAggregateFunction='MAX' then
        Result:=[mtifoAggregate,mtifoAggMax]
     else if AAggregateFunction='MIN' then
        Result:=[mtifoAggregate,mtifoAggMin]
     else if AAggregateFunction='COUNT' then
        Result:=[mtifoAggregate,mtifoAggCount]
     else if AAggregateFunction='SUM' then
        Result:=[mtifoAggregate,mtifoAggSum]
     else if AAggregateFunction='STDDEV' then
        Result:=[mtifoAggregate,mtifoAggStdDev]
     else if AAggregateFunction='AVG' then
        Result:=[mtifoAggregate,mtifoAggAvg]
     else if AAggregateFunction='USR1' then
        Result:=[mtifoAggregate,mtifoAggUsr1]
     else if AAggregateFunction='USR2' then
        Result:=[mtifoAggregate,mtifoAggUsr2]
     else if AAggregateFunction='USR3' then
        Result:=[mtifoAggregate,mtifoAggUsr3]
     else
         Result:=[];
end;

// Sort using specified sortfields and options.
procedure TkbmCustomMemTable.SortDefault;
begin
     Sort(FSortOptions);
end;

// Do sort on specified sortfields.
procedure TkbmCustomMemTable.Sort(Options:TkbmMemTableCompareOptions);
var
   idx:TkbmIndex;
begin
     if not Active then exit;
     CheckBrowseMode;

     try
        // Check if old sort index defined, remove it.
        if FSortIndex<>nil then
        begin
             Indexes.DeleteIndex(FSortIndex);
             FSortIndex:=nil;
        end;

        // Is any sort fields setup.
        if (Trim(FSortFieldNames)<>'') then
        begin
             // Now add a new index.
             if IsMasterDetailActive then
                idx:=FDetailIndex
             else
                 idx:=FIndexes.FRowOrderIndex;
             FSortIndex:=TkbmIndex.Create(idx,kbmDefSortIndex,self,FSortFieldNames,Options,mtitSorted);
             Indexes.AddIndex(FSortIndex);
             FSortIndex.Rebuild;
        end
        else
            FSortIndex:=nil;
        SwitchToIndex(FSortIndex);
     finally
        if FCurIndex=nil then
           SwitchToIndex(FIndexes.FRowOrderIndex);
     end;
end;

// Do sort on specifed fieldnames.
procedure TkbmCustomMemTable.SortOn(const FieldNames:string; Options:TkbmMemTableCompareOptions);
var
   idx:TkbmIndex;
begin
     if not Active then exit;
     CheckBrowseMode;
     FSortedOn:=FieldNames;

     // Reset curindex to make sure to set it to something afterwards.
     FCurIndex:=nil;
     try

        // Check if old sort index defined, remove it.
        if FSortIndex<>nil then
        begin
             Indexes.DeleteIndex(FSortIndex);
             FSortIndex:=nil;
        end;

        // If specifying new fields to sort on, create index on those fields, otherwise select roworderindex.
        if (Trim(FieldNames)<>'') then
        begin
             // Now add a new index.
             if IsMasterDetailActive then
                idx:=FDetailIndex
             else
                 idx:=FIndexes.FRowOrderIndex;
             FSortIndex:=TkbmIndex.Create(idx,kbmDefSortIndex,self,FieldNames,Options,mtitSorted);
             Indexes.AddIndex(FSortIndex);
             FSortIndex.Rebuild;
        end
        else
            FSortIndex:=nil;
        SwitchToIndex(FSortIndex);
     finally
        if FCurIndex=nil then
           SwitchToIndex(FIndexes.FRowOrderIndex);
     end;
end;

// Get specified rows as a variant.
function TkbmCustomMemTable.GetRows(Rows:Integer; Start:Variant; Fields:Variant):Variant;
var
   FldList:TkbmFieldList;
   FldCnt,RowCnt,RealCnt:integer;
   i,j:TkbmNativeInt;
   FRows:array of array of variant;
begin
     Result:=Unassigned;

     // If Start parameter is Unassigned or kbmBookMarkCurrent
     // retrieving starts at current position.
     // If it is assigned anything other than the bookmark consts
     // a valid TBookmark is assumed (casted to TkbmNativeInt when called)
     if not VarIsEmpty(Start) then
     begin
          if Start=kbmBookMarkLast then Last // doesnt make too much sense...
          else if Start=kbmBookMarkFirst then First
          else
            try
               //GoToBookMark(Pointer(TkbmNativeInt(Start)))
               GoToBookMark(TBookmark(TkbmNativeInt(Start)));
            except
            end; // raise?
     end;

     // If Rows parameter matches kbmGetRowsRest the table is scanned to the end.
     if Rows=Integer(kbmGetRowsRest) then
        RowCnt:=GetRecordCount-GetRecNo+1
     else
        RowCnt:=Rows;

     FldList:=TkbmFieldList.Create;
     try
        // Fields parameter can be
        // - single fieldname
        // - single fieldpos
        // - array of fieldnames
        // - array of fieldpos
        // - Unassigned (=all fields)
        if VarIsEmpty(Fields) then
        begin
             for i:=0 to pred(self.Fields.Count) do
               FldList.Add(self,self.Fields[i],[]);
        end
        else if VarIsArray(Fields) then
        begin
             if VarType(Fields[0])=varInteger then
                for i:=0 to VarArrayHighBound(Fields,1) do
                    FldList.Add(self,FieldByNumber(Fields[i]),[])
             else
                for i:=0 to VarArrayHighBound(Fields,1) do
                    FldList.Add(self,FieldByName(VarToStr(Fields[i])),[]);
        end
        else
        begin
             if VarType(Fields)=varInteger then
                FldList.Add(self,FieldByNumber(Fields),[])
             else
                FldList.Add(self,FieldByName(VarToStr(Fields)),[]);
        end;

        RealCnt:=0;
        FldCnt:=FldList.Count;
        SetLength(FRows,FldCnt,RowCnt);

        for j:=0 to pred(RowCnt) do
        begin
             for i:=0 to pred(FldCnt) do
             begin
                  // TBlobField.AsVariant doesnt return NULL for empty blobs
                  if FldList.Fields[i].IsNull then
                     FRows[i,j]:=Null
                  else
                     FRows[i,j]:=FldList.Fields[i].AsVariant;
             end;
             inc(RealCnt);
             Next;
             if EOF then Break;
        end;
     finally
        FldList.Free;
     end;

     if RealCnt<>RowCnt then
       SetLength(FRows,FldCnt,RealCnt);
     Result:=FRows;
end;

procedure TkbmCustomMemTable.ClearModified;
begin
     FCommon.ClearModifiedFlags;
     SetIsDataModified(false);
end;

procedure TkbmCustomMemTable.Reset;
begin
     Close;
     IndexName:='';
     MasterFields:='';
     IndexFieldNames:='';
     SetDataSource(nil);
     Indexes.Clear;
     Fields.Clear;
     FIndexDefs.Clear;
     FieldDefs.Clear;
     Filtered:=false;
     ClearModified;
end;

procedure TkbmCustomMemTable.GotoCurrent(DataSet:TkbmCustomMemTable);
var
   id:TkbmNativeInt;
   iRecNo:TkbmNativeInt;
   pRec:PkbmRecord;
begin
     pRec:=PkbmRecord(DataSet.ActiveBuffer);
     if pRec=nil then exit;

     id:=pRec^.RecordID;

     CurIndex.SearchRecordID(id,iRecNo);
     if (iRecNo<0) or (iRecNo>=CurIndex.References.Count)
        or (not FilterRecord(CurIndex.References.Items[iRecNo],false))
        then
           raise Exception.Create('Record not found');
     RecNo:=iRecNo;
end;

procedure TkbmCustomMemtable.GroupBy(ADestDataset:TkbmCustomMemTable; AGroupFields:string; AAggregateFields:string);
var
   i,nAggs:TkbmNativeInt;
   fld:TField;
   fo:TkbmifoOptions;
   gbsrcflist,
   aggsrcflist,aggdstflist:TkbmFieldList;
   ft:TFieldType;
   s:string;
begin
     gbsrcflist:=TkbmFieldList.Create;
     aggsrcflist:=TkbmFieldList.Create;
     aggdstflist:=TkbmFieldList.Create;
     try
        // Build list of fields to group on.
        gbsrcflist.Build(self,AGroupFields);

        // Build list of fields to aggregate on.
        aggsrcflist.Build(self,AAggregateFields);

        // Build destination dataset. It will contain aggregate fields.
        nAggs:=aggsrcflist.Count;
        ADestDataset.Reset;
        for i:=0 to nAggs-1 do
        begin
             fld:=aggsrcflist.Fields[i];
             fo:=aggsrcflist.Options[i];
             s:=GetAggregateFieldName(fld.FieldName,fo);
             ft:=fld.DataType;
             if [mtifoAggAvg,mtifoAggStdDev,mtifoAggUsr1,mtifoAggUsr2,mtifoAggUsr3]*fo<>[] then
                ft:=ftFloat;
             ADestDataset.FieldDefs.Add(s,
                                        ft,
                                        fld.Size);
        end;

        // Create destination table.
        ADestDataset.CreateTable;
        ADestDataset.Open;

        // Build list of destination fields to aggregate on.
        aggdstflist.Build(ADestDataset,AAggregateFields,true);

        GroupBy(ADestDataset,gbsrcflist,aggsrcflist,aggdstflist);
     finally
        gbsrcflist.Free;
        aggsrcflist.Free;
        aggdstflist.Free;
     end;
end;

procedure TkbmCustomMemtable.GroupBy(ADestDataset:TkbmCustomMemTable; ASourceGroupFieldList:TkbmFieldList; ASourceAggregateFieldList,ADestAggregateFieldList:TkbmFieldList);
var
   j,i,nAggs:TkbmNativeInt;
   s:string;
   fld:TField;
   fo:TkbmifoOptions;
   idx:TkbmIndex;
   aggcount:array of integer;
   aggfloatdata:array of double;
   aggstringdata:array of string;
   aggdataStdDevMean:array of double;
   aggstringtype:array of boolean;
   d,d1:double;
   beginrec,currec:PkbmRecord;
   bGrouped:boolean;

   procedure ClearAggregateData;
   var
      i:TkbmNativeInt;
   begin
        for i:=0 to nAggs-1 do
        begin
             aggcount[i]:=0;
             aggfloatdata[i]:=NaN;
             aggstringdata[i]:='';
             aggdataStdDevMean[i]:=0;
             aggstringtype[i]:=ASourceAggregateFieldList.Fields[i].DataType in kbmStringTypes;
        end;
   end;

   procedure PreSetAggregateFieldData;
   var
      i:TkbmNativeInt;
      fo:TkbmifoOptions;
   begin
        OverrideActiveRecordBuffer:=beginrec;
        try
           for i:=0 to nAggs-1 do
           begin
                fo:=ADestAggregateFieldList.Options[i];
                if not (mtifoAggregate in fo) then
                   ADestAggregateFieldList.Fields[i].Value:=ASourceAggregateFieldList.Fields[i].Value;
           end;
        finally
           OverrideActiveRecordBuffer:=nil;
        end;
   end;

   procedure PostSetAggregateFieldData;
   var
      i:TkbmNativeInt;
      fo:TkbmifoOptions;
   begin
        for i:=0 to nAggs-1 do
        begin
             fo:=ADestAggregateFieldList.Options[i];
             if (mtifoAggregate in fo) then
             begin
                  if mtifoAggCount in fo then
                     ADestAggregateFieldList.Fields[i].AsInteger:=aggcount[i]
                  else
                  begin
                       if aggstringtype[i] then
                           ADestAggregateFieldList.Fields[i].AsString:=aggstringdata[i]
                       else if IsNan(aggfloatdata[i]) then
                           ADestAggregateFieldList.Fields[i].Clear
                       else
                       begin
                            if mtifoAggAvg in fo then
                            begin
                                 if aggcount[i]>0 then
                                    ADestAggregateFieldList.Fields[i].AsFloat:=aggfloatdata[i] / aggcount[i];
                            end
                            else if mtifoAggStdDev in fo then
                            begin
                                 if aggcount[i]<2 then
                                    ADestAggregateFieldList.Fields[i].AsFloat:=0.0
                                 else
                                    ADestAggregateFieldList.Fields[i].AsFloat:=sqrt(aggfloatdata[i] / (aggcount[i]-1));
                            end
                            else
                               ADestAggregateFieldList.Fields[i].AsFloat:=aggfloatdata[i];
                       end;
                  end;
             end;
        end;
   end;

begin
     // Verify that non aggregated fields are part of the group fields, else complain.
     nAggs:=ASourceAggregateFieldList.Count;
     for i:=0 to nAggs-1 do
     begin
          fld:=ASourceAggregateFieldList.Fields[i];
          fo:=ASourceAggregateFieldList.Options[i];
          if not (mtifoAggregate in fo) then
          begin
               if ASourceGroupFieldList.IndexOf(fld)<0 then
                  raise Exception.Create('Field must be listed as group field or be covered by aggregate function.');
          end;
     end;

     bGrouped:=nAggs>0;

     SetLength(aggfloatdata,nAggs);
     SetLength(aggstringdata,nAggs);
     SetLength(aggstringtype,nAggs);
     SetLength(aggdataStdDevMean,nAggs);
     SetLength(aggcount,nAggs);

     // Check if current index is not containing full scope of data.
     // Then we need to create a derived index from it.
     FreeGroupIndex;
     if bGrouped then
     begin
          s:=ASourceGroupFieldList.GetAsString;
          if not CurIndex.IsFullScope then
             CreateGroupIndex(s)
          else
          begin
               idx:=Indexes.GetByFieldNames(s);
               if (idx<>nil) and idx.IsFullScope and idx.IsOrdered then
               begin
                    FGroupIndex:=idx;
                    FGroupIndexOwned:=false;
               end
               else
                   CreateGroupIndex(s);
          end;
     end;

     // Loop thru all records in source table based on group index and add up in destination table.
     beginrec:=nil;
     for j:=0 to FGroupIndex.References.Count-1 do
     begin
          currec:=FGroupIndex.References[j];

          // If to remember previous record but not set yet.
          if beginrec=nil then
          begin
               beginrec:=currec;

               // Prepare new record.
               ADestDataset.Append;
               ClearAggregateData;
               PreSetAggregateFieldData;
          end

          // Check if different keyfields compared to last record,
          // then we have collected a group.
          else if bGrouped then
          begin
//               OverrideActiveRecordBuffer:=beginrec;
//               s:='beginrec ('+inttostr(integer(beginrec))+'='+ASourceGroupFieldList.Fields[0].AsString;
//               OverrideActiveRecordBuffer:=currec;
//               s:=s+'currec ('+inttostr(integer(currec))+'='+ASourceGroupFieldList.Fields[0].AsString;
//               OverrideActiveRecordBuffer:=nil;
//               OutputDebugString(PChar(s));
               if (FCommon._InternalCompareRecords(ASourceGroupFieldList,ASourceGroupFieldList.Count,beginrec,currec,false,false,false,chBreakNE)<>0) then
               begin
                    beginrec:=currec;

                    // Post
                    PostSetAggregateFieldData;
                    ADestDataset.Post;

                    // Prepare new record.
                    ClearAggregateData;
                    ADestDataset.Append;
                    PreSetAggregateFieldData;
               end;
          end;

          // Populate aggregates.
          OverrideActiveRecordBuffer:=currec;
          for i:=0 to ASourceAggregateFieldList.Count-1 do
          begin
               fo:=ASourceAggregateFieldList.Options[i];
               fld:=ASourceAggregateFieldList.Fields[i];
               if not fld.IsNull then
               begin
                    if (mtifoAggSum in fo) or (mtifoAggAvg in fo) then
                    begin
                         d:=ASourceAggregateFieldList.Fields[i].AsFloat;
                         if isNan(aggfloatdata[i]) then
                            aggfloatdata[i]:=d
                         else
                             aggfloatdata[i]:=aggfloatdata[i]+d;
                    end
                    else if mtifoAggMin in fo then
                    begin
                         if aggstringtype[i] then
                         begin
                              s:=fld.AsString;
                              if aggstringdata[i]='' then
                                 aggstringdata[i]:=s
                              else if s<aggstringdata[i] then
                                 aggstringdata[i]:=s;
                         end
                         else
                         begin
                              d:=fld.AsFloat;
                              if IsNan(aggfloatdata[i]) then
                                 aggfloatdata[i]:=d
                              else if (not IsNan(d)) and (d<aggfloatdata[i]) then
                                   aggfloatdata[i]:=d
                         end;
                    end
                    else if mtifoAggMax in fo then
                    begin
                         if aggstringtype[i] then
                         begin
                              s:=fld.AsString;
                              if s>aggstringdata[i] then
                                 aggstringdata[i]:=s;
                         end
                         else
                         begin
                              d:=fld.AsFloat;
                              if IsNan(aggfloatdata[i]) then
                                 aggfloatdata[i]:=d
                              else if (not IsNan(d)) and (d>aggfloatdata[i]) then
                                   aggfloatdata[i]:=d;
                         end;
                    end
                    else if mtifoAggStdDev in fo then
                    begin
                         d:=ASourceAggregateFieldList.Fields[i].AsFloat;
                         if isNan(aggfloatdata[i]) then
                         begin
                              aggfloatdata[i]:=0;
                              aggdataStdDevMean[i]:=d;
                         end
                         else
                         begin
                              d1:=aggdataStdDevMean[i];
                              aggdataStdDevMean[i]:=aggdataStdDevMean[i] + ((d - d1) / aggcount[i]);
                              aggfloatdata[i]:=aggfloatdata[i] + ((d - d1) * (d - aggdataStdDevMean[i]));
                         end;
                    end
                    else if mtifoAggUsr1 in fo then
                    begin
                         if Assigned(ADestDataset.OnUserAggregate) then
                         begin
                              d:=ASourceAggregateFieldList.Fields[i].AsFloat;
                              ADestDataset.OnUserAggregate(self,1,aggcount[i],d,aggfloatdata[i]);
                         end;
                    end
                    else if mtifoAggUsr2 in fo then
                    begin
                         if Assigned(ADestDataset.OnUserAggregate) then
                         begin
                              d:=ASourceAggregateFieldList.Fields[i].AsFloat;
                              ADestDataset.OnUserAggregate(self,2,aggcount[i],d,aggfloatdata[i]);
                         end;
                    end
                    else if mtifoAggUsr3 in fo then
                    begin
                         if Assigned(ADestDataset.OnUserAggregate) then
                         begin
                              d:=ASourceAggregateFieldList.Fields[i].AsFloat;
                              ADestDataset.OnUserAggregate(self,3,aggcount[i],d,aggfloatdata[i]);
                         end;
                    end;
                    inc(aggcount[i]);
               end;
          end;
          OverrideActiveRecordBuffer:=nil;
     end;
     if ADestDataset.State=dsInsert then
     begin
          PostSetAggregateFieldData;
          ADestDataset.Post;
     end;
end;

function TkbmCustomMemtable.Aggregate(AAggregateFields:string):variant;
var
   j,i,nAggs:TkbmNativeInt;
   fo:TkbmifoOptions;
   aggcount:array of integer;
   aggstringdata:array of string;
   aggfloatdata:array of double;
   aggdataStdDevMean:array of double;
   aggstringtype:array of boolean;
   d,d1:double;
   currec:PkbmRecord;
   aggsrcflist:TkbmFieldList;
   fld:TField;
   s:string;

   procedure ClearAggregateData;
   var
      i:TkbmNativeInt;
   begin
        for i:=0 to nAggs-1 do
        begin
             aggcount[i]:=0;
             aggfloatdata[i]:=NaN;
             aggstringdata[i]:='';
             aggdataStdDevMean[i]:=0;
             aggstringtype[i]:=aggsrcflist.Fields[i].DataType in kbmStringTypes;
        end;
   end;

   function GetAggregateValue(AIndex:integer):variant;
   var
      fo:TkbmifoOptions;
   begin
        fo:=aggsrcflist.Options[AIndex];
        if (mtifoAggregate in fo) then
        begin
             if mtifoAggCount in fo then
                Result:=aggcount[AIndex]
             else
             begin
                  if aggstringtype[AIndex] then
                      Result:=aggstringdata[AIndex]
                  else if IsNan(aggfloatdata[AIndex]) then
                     Result:=Variants.Null
                  else
                  begin
                       if mtifoAggAvg in fo then
                       begin
                            if aggcount[AIndex]<1 then
                               Result:=Variants.Null
                            else
                                Result:=aggfloatdata[AIndex] / aggcount[AIndex];
                       end
                       else if mtifoAggStdDev in fo then
                       begin
                            if aggcount[AIndex]<2 then
                               Result:=0.0
                            else
                                Result:=sqrt(aggfloatdata[AIndex] / (aggcount[AIndex]-1))
                       end
                       else
                          Result:=aggfloatdata[AIndex];
                  end;
             end;
        end
        else
            Result:=Variants.Null;
   end;

begin
     aggsrcflist:=TkbmFieldList.Create;
     try
        // Build list of fields to aggregate on.
        aggsrcflist.Build(self,AAggregateFields);

        // Build destination resultset. It will contain aggregate fields.
        nAggs:=aggsrcflist.Count;
        if nAggs<1 then
        begin
             Result:=Variants.Null;
             exit;
        end;

        SetLength(aggfloatdata,nAggs);
        SetLength(aggstringdata,nAggs);
        SetLength(aggstringtype,nAggs);
        SetLength(aggdataStdDevMean,nAggs);
        SetLength(aggcount,nAggs);

        // Loop thru all records in source table based on group index and add up in destination table.
        ClearAggregateData;
        for j:=0 to FCurIndex.References.Count-1 do
        begin
             currec:=FCurIndex.References[j];

             // Populate aggregates.
             OverrideActiveRecordBuffer:=currec;
             for i:=0 to aggsrcflist.Count-1 do
             begin
                  fo:=aggsrcflist.Options[i];
                  fld:=aggsrcflist.Fields[i];
                  if not fld.IsNull then
                  begin
                       if (mtifoAggSum in fo) or (mtifoAggAvg in fo) then
                       begin
                            d:=aggsrcflist.Fields[i].AsFloat;
                            if isNan(aggfloatdata[i]) then
                               aggfloatdata[i]:=d
                            else
                                aggfloatdata[i]:=aggfloatdata[i]+d;
                       end
                       else if mtifoAggMin in fo then
                       begin
                            if aggstringtype[i] then
                            begin
                                 s:=fld.AsString;
                                 if aggstringdata[i]='' then
                                    aggstringdata[i]:=s
                                 else if s<aggstringdata[i] then
                                    aggstringdata[i]:=s;
                            end
                            else
                            begin
                                 d:=fld.AsFloat;
                                 if IsNan(aggfloatdata[i]) then
                                    aggfloatdata[i]:=d
                                 else if (not IsNan(d)) and (d<aggfloatdata[i]) then
                                      aggfloatdata[i]:=d
                            end;
                       end
                       else if mtifoAggMax in fo then
                       begin
                            if aggstringtype[i] then
                            begin
                                 s:=fld.AsString;
                                 if s>aggstringdata[i] then
                                    aggstringdata[i]:=s;
                            end
                            else
                            begin
                                 d:=fld.AsFloat;
                                 if IsNan(aggfloatdata[i]) then
                                    aggfloatdata[i]:=d
                                 else if (not IsNan(d)) and (d>aggfloatdata[i]) then
                                      aggfloatdata[i]:=d;
                            end;
                       end
                       else if mtifoAggStdDev in fo then
                       begin
                            d:=aggsrcflist.Fields[i].AsFloat;
                            if isNan(aggfloatdata[i]) then
                            begin
                                 aggfloatdata[i]:=0;
                                 aggdataStdDevMean[i]:=d;
                            end
                            else
                            begin
                                 d1:=aggdataStdDevMean[i];
                                 aggdataStdDevMean[i]:=aggdataStdDevMean[i] + ((d - d1) / aggcount[i]);
                                 aggfloatdata[i]:=aggfloatdata[i] + ((d - d1) * (d - aggdataStdDevMean[i]));
                            end;
                       end
                       else if mtifoAggUsr1 in fo then
                       begin
                            if Assigned(FOnUserAggregate) then
                            begin
                                 d:=aggsrcflist.Fields[i].AsFloat;
                                 FOnUserAggregate(self,1,aggcount[i],d,aggfloatdata[i]);
                            end;
                       end
                       else if mtifoAggUsr2 in fo then
                       begin
                            if Assigned(FOnUserAggregate) then
                            begin
                                 d:=aggsrcflist.Fields[i].AsFloat;
                                 FOnUserAggregate(self,2,aggcount[i],d,aggfloatdata[i]);
                            end;
                       end
                       else if mtifoAggUsr3 in fo then
                       begin
                            if Assigned(FOnUserAggregate) then
                            begin
                                 d:=aggsrcflist.Fields[i].AsFloat;
                                 FOnUserAggregate(self,3,aggcount[i],d,aggfloatdata[i]);
                            end;
                       end;
                       inc(aggcount[i]);
                  end;
             end;
             OverrideActiveRecordBuffer:=nil;
        end;

        if nAggs=1 then
           Result:=GetAggregateValue(0)
        else
        begin
             Result:=VarArrayCreate([0,nAggs-1],varVariant);
             for i:=0 to nAggs-1 do
                 Result[i]:=GetAggregateValue(i);
        end;
     finally
        aggsrcflist.Free;
     end;
end;

function TkbmCustomMemtable.Extract(AExtractFields:string; AStringList:TStrings; AFormat:string = ''):integer;
var
   aExtended:array of Extended;
   aCurrency:array of Currency;
   aUnicodeString:array of UnicodeString;
   aInt64:array of Int64;
type
    TConstArray=array of TVarRec;

   procedure InitializeConstArray(const AFields:TkbmFieldList);
   var
      n:integer;
   begin
        n:=AFields.Count;
        SetLength(aExtended,n);
        SetLength(aCurrency,n);
        SetLength(aUnicodeString,n);
        SetLength(aInt64,n);
   end;

   procedure FinalizeConstArray;
   begin
        SetLength(aExtended,0);
        SetLength(aCurrency,0);
        SetLength(aUnicodeString,0);
        SetLength(aInt64,0);
   end;

   function FieldValues2ConstArray(const AFields:TkbmFieldList):TConstArray;
   var
      i,n:integer;
      fld:TField;
   begin
        n:=AFields.Count;
        SetLength(Result,n);
        for i:=0 to n-1 do
        begin
             fld:=AFields.Fields[i];
             if fld.IsNull and (mtifoIgnoreNull in AFields.Options[i]) then
             begin
                  SetLength(Result,0);
                  exit;
             end;

             case fld.DataType of
                DB.ftAutoInc,
                DB.ftInteger,
                DB.ftSmallint,
{$IFDEF KBMMEMTABLE_SUPPORT_FT_LONGWORD}
                DB.ftLongWord,
{$ENDIF}
{$IFDEF KBMMEMTABLE_SUPPORT_FT_SHORTINT}
                DB.ftShortint,
{$ENDIF}
{$IFDEF KBMMEMTABLE_SUPPORT_FT_BYTE}
                DB.ftByte,
{$ENDIF}
                DB.ftWord:
                   begin
                        Result[i].VType:=vtInteger;
                        if fld.IsNull then
                           Result[i].VInteger:=0
                        else
                            Result[i].VInteger:=fld.AsInteger;
                   end;

                DB.ftLargeint:
                   begin
                        Result[i].VType:=vtInt64;
                        if fld.IsNull then
                           aInt64[i]:=0
                        else
{$IFNDEF LEVEL16}
                            aInt64[i]:=(fld as TLargeIntField).AsLargeInt;
{$ELSE}
                            aInt64[i]:=fld.AsLargeInt;
{$ENDIF}
                        Result[i].VInt64:=@aInt64[i];
                   end;

{$IFDEF KBMMEMTABLE_SUPPORT_FT_EXTENDED}
                DB.ftExtended,
{$ENDIF}
{$IFDEF KBMMEMTABLE_SUPPORT_FT_SINGLE}
                DB.ftSingle,
{$ENDIF}
                DB.ftFloat,
                DB.ftDateTime,
                DB.ftTime,
                DB.ftDate:
                   begin
                        if mtifoAsDate in AFields.Options[i] then
                        begin
                             Result[i].VType:=vtUnicodeString;
                             if fld.IsNull then
                                aUnicodeString[i]:=''
                             else
                                 aUnicodeString[i]:=DateToStr(fld.AsFloat,FFormatSettings);
                             Result[i].VUnicodeString:=pointer(aUnicodeString[i]);
                        end
                        else if mtifoAsTime in AFields.Options[i] then
                        begin
                             Result[i].VType:=vtUnicodeString;
                             if fld.IsNull then
                                aUnicodeString[i]:=''
                             else
                                 aUnicodeString[i]:=TimeToStr(fld.AsFloat,FFormatSettings);
                             Result[i].VUnicodeString:=pointer(aUnicodeString[i]);
                        end
                        else if mtifoAsDateTime in AFields.Options[i] then
                        begin
                             Result[i].VType:=vtUnicodeString;
                             if fld.IsNull then
                                aUnicodeString[i]:=''
                             else
                                 aUnicodeString[i]:=DateTimeToStr(fld.AsFloat,FFormatSettings);
                             Result[i].VUnicodeString:=pointer(aUnicodeString[i]);
                        end
                        else
                        begin
                             Result[i].VType:=vtExtended;
                             if fld.IsNull then
                                aExtended[i]:=Math.NaN
                             else
                                 aExtended[i]:={$IFDEF FPC}
                                                fld.AsFloat
                                               {$ELSE}
                                                fld.AsExtended
                                               {$ENDIF};
                             Result[i].VExtended:=@aExtended[i];
                        end;
                   end;

                DB.ftCurrency:
                   begin
                        Result[i].VType := vtCurrency;
                        if fld.IsNull then
                           aCurrency[i]:=0.0
                        else
                            aCurrency[i]:=fld.AsCurrency;
                        Result[i].VCurrency:=@aCurrency[i];
                   end;

                DB.ftBoolean:
                   begin
                        Result[i].VType:=vtBoolean;
                        Result[i].VBoolean:=fld.AsBoolean;
                   end;

                DB.ftString,
                DB.ftWideString,
                DB.ftFixedChar,
                DB.ftFixedWideChar,
                DB.ftWideMemo,
                DB.ftMemo:
                   begin
                        Result[i].VType:=vtUnicodeString;
                        if fld.IsNull then
                           aUnicodeString[i]:=''
                        else
{$IFDEF NEXTGEN}
                            aUnicodeString[i]:=fld.AsString;
{$ELSE}
                            aUnicodeString[i]:=fld.AsWideString;
{$ENDIF}
                        Result[i].VUnicodeString:=pointer(aUnicodeString[i]);
                   end;

                else
                    raise EMemTableError.Create('Unsupported field type in Extract: '+inttostr(ord(fld.DataType)));
             end;
        end;
   end;

var
   flist:TkbmFieldList;
   i,j:integer;
   ac:TConstArray;
   currec,oldrec:PkbmRecord;
   a,s:string;
   b:boolean;
begin
     AStringList.BeginUpdate;
     try
        AStringList.Clear;
        Result:=0;
        if not Active then
           exit;

        flist:=TkbmFieldList.Create;
        try
           // Build list of fields to extract
           flist.Build(self,AExtractFields);
           if flist.Count<=0 then
              exit;
           InitializeConstArray(flist);
           try
              FCommon.Lock;
              try
                 oldrec:=OverrideActiveRecordBuffer;
                 try
                    b:=Length(AFormat)>0;
                    for i:=0 to FCurIndex.References.Count-1 do
                    begin
                         currec:=FCurIndex.References[i];
                         if currec=nil then
                            continue;
                         OverrideActiveRecordBuffer:=currec;

                         // Calculate fields.
                         __ClearCalcFields(OverrideActiveRecordBuffer);
                         __GetCalcFields(OverrideActiveRecordBuffer);

                         // Check filter of record.
                         if not FilterRecord(OverrideActiveRecordBuffer,false) then
                            continue;

                         if b then
                         begin
                              ac:=FieldValues2ConstArray(flist);
                              if length(ac)=0 then
                                 continue;

                              s:=Format(AFormat,ac,FFormatSettings);
                         end
                         else
                         begin
                              a:='';
                              s:='';
                              for j:=0 to flist.Count-1 do
                              begin
                                   s:=s+a+flist.Fields[i].AsString;
                                   a:=' ';
                              end;
                         end;

                         AStringList.Add(s);
                    end;
                 finally
                    OverrideActiveRecordBuffer:=oldrec;
                 end;
              finally
                 FCommon.Unlock;
              end;
           finally
              FinalizeConstArray;
           end;

        finally
           flist.Free;
        end;
     finally
        AStringList.EndUpdate;
        Result:=AStringList.Count;
     end;
end;

// -----------------------------------------------------------------------------------
// TkbmBlobStream
// -----------------------------------------------------------------------------------

// On create, make a stream access to the specified blobfield in the current record.
constructor TkbmBlobStream.Create(Field:TBlobField;Mode:TBlobStreamMode);
var
   RecID:TkbmNativeInt;
begin
     // Remember proposed field and mode.
     FMode:=Mode;
     FField:=Field;
     FFieldNo:=FField.FieldNo;
     FDataSet:=TkbmCustomMemTable(FField.DataSet);

     // Dont want other to mess with out blob while we are using it.
     FDataSet.FCommon.Lock;
     try
          // If a write mode, check if allowed to write.
          if Mode<>bmRead then
          begin
               if (not FDataSet.FIgnoreReadOnly) and (FField.ReadOnly) then DatabaseErrorFmt(kbmReadOnlyErr,[FField.DisplayName]);
               if not (FDataSet.State in [dsEdit, dsInsert]) then DatabaseError(kbmEditModeErr);
          end;

          // Get pointers to work buffer.
          FWorkBuffer:=PkbmRecord(FDataSet.GetActiveRecord);
          if FWorkBuffer=nil then exit;
          FpWorkBufferField:=FDataset.FCommon.GetFieldPointer(FWorkBuffer,FField);
          FpWorkBufferBlob:=PPkbmVarLength(FpWorkBufferField+1);
          RecID:=FWorkBuffer^.RecordID;

          // Get pointers to table record buffer.
          if (RecID>=0) then
          begin
               FTableRecord:=PkbmRecord(FDataSet.FCommon.FRecords.Items[RecID]);
               FpTableRecordField:=FDataSet.FCommon.GetFieldPointer(FTableRecord,FField);
               FpTableRecordBlob:=PPkbmVarLength(FpTableRecordField+1);
          end
          else
          begin
               // In case of a totally new non posted record.
               FTableRecord:=nil;
               FpTableRecordField:=nil;
               FpTableRecordBlob:=nil;
          end;

          // Write mode, truncate blob.
          if Mode=bmWrite then
               Truncate
          else
              // Read the blob data into the memorystream.
              ReadBlobData;
     finally
        FDataSet.FCommon.Unlock;
     end;
end;


// On destroy, update the blobfield in the current record if the blob has changed.
destructor TkbmBlobStream.Destroy;
begin
     try
        if FModified then
        begin
             WriteBlobData;
             FField.Modified:=true;
             FDataSet.DataEvent(deFieldChange,TkbmNativeInt(FField));
        end;
     except
        kbmMemTableHandleException(self);
     end;

     inherited Destroy;
end;

// Move the contents of the memorystream into the blob.
procedure TkbmBlobStream.WriteBlobData;
var
   Blob:PkbmVarLength;
   Stream:TMemoryStream;
   sz:TkbmNativeInt;
begin
     // Check if old blob in workbuffer, free it.
     if (FpWorkBufferBlob<>nil) and (FpWorkBufferBlob^<>nil) then
     begin
          FreeVarLength(FpWorkBufferBlob^);
          FpWorkBufferBlob^:=nil;
     end;

     // If to compress the blob data, do it.
     if Assigned(FDataSet.FOnCompressBlobStream) then
     begin
          Stream:=TMemoryStream.Create;
          try
             FDataSet.FOnCompressBlobStream(FDataSet,self,Stream);
             sz:=Stream.Size;
             if sz>0 then
                Blob:=AllocVarLengthAs(Stream.Memory,sz)
             else
                Blob:=nil;
          finally
             Stream.free;
          end;
     end
     else
     begin
          sz:=self.Size;

          // Otherwise just save raw data to the inmemory blob.
          if sz>0 then
             Blob:=AllocVarLengthAs(self.Memory,self.Size)
          else
             Blob:=nil;
     end;

     // Update with new allocation.
     FpWorkBufferBlob^:=Blob;

     // Set Null flag in work record.
     if Blob<>nil then
        FpWorkBufferField[0]:=kbmffData
     else
         FpWorkBufferField[0]:=kbmffNull;
end;


// Move the contents of the blob into the memory stream.
procedure TkbmBlobStream.ReadBlobData;
var
   Blob:PkbmVarLength;
   Stream:TMemoryStream;
   sz:TkbmLongInt;
begin
     // Get blob.
     Blob:=FpWorkBufferBlob^;
     if Blob=nil then
     begin
          // Check if to read from table (not null).
          if (FpWorkBufferField[0]<>kbmffNull) then
             Blob:=FpTableRecordBlob^
          else
              // Nothing to read. Null blob.
              exit;
     end;

     // Get size of blob.
     sz:=GetVarLengthSize(Blob);

     // If to decompress stream, save the blob in a memory stream and decompress it.
     if Assigned(FDataSet.FOnDeCompressBlobStream) then
     begin
          Stream:=TMemoryStream.Create;
          try
             Stream.SetSize(longint(sz));
             kbmMemMove(GetVarLengthData(Blob)^,Stream.Memory^,sz);
             FDataSet.FOnDecompressBlobStream(FDataSet,Stream,self);
          finally
             Stream.free;
          end;
     end
     else
     begin
          // Copy the data to the stream.
{$IFDEF FPC}
          self.SetSize64(sz);
{$ELSE}
          self.SetSize(longint(sz));
{$ENDIF}
          kbmMemMove(GetVarLengthData(Blob)^,self.Memory^,sz);
     end;
     self.Position:=0;
end;


function TkbmBlobStream.Write(const Buffer;Count:longint): longint;
begin
     Result:=inherited Write(Buffer,Count);
     if (FMode=bmWrite) or (FMode=bmReadWrite) then FModified:=true;
end;

// Null a blob.
procedure TkbmBlobStream.Truncate;
begin
     Clear;
     // If blob allocated in workbuffer, remove allocation.
     if FpWorkBufferBlob^<>nil then
     begin
          FreeVarLength(FpWorkBufferBlob^);
          FpWorkBufferBlob^:=nil;
     end;
     FpWorkBufferField[0]:=kbmffNull;

     FModified:=true;
end;

// -----------------------------------------------------------------------------------
// Handler for resolving delta's. Must be overridden to be usable.
// -----------------------------------------------------------------------------------

procedure TkbmCustomDeltaHandler.Notification(AComponent: TComponent; Operation: TOperation);
begin
     inherited Notification(AComponent,Operation);
     if (Operation=opRemove) and (AComponent=FDataset) then FDataset:=nil;
end;

procedure TkbmCustomDeltaHandler.CheckDataSet;
begin
     if FDataSet=nil then raise EMemTableError.Create(kbmDeltaHandlerAssign);
end;

procedure TkbmCustomDeltaHandler.Resolve;
var
   i:TkbmNativeInt;
   pRec,pOrigRec:PkbmRecord;
   st:TUpdateStatus;
   oAttachedAutoRefresh:boolean;
   Retry:boolean;
begin
     CheckDataSet;
     oAttachedAutoRefresh:=FDataSet.FAttachedAutoRefresh;
     FDataSet.FAttachedAutoRefresh:=false;
     FDataSet.FCommon.Lock;
     try
        // Do not refresh views _while_ resolving. Wait until afterwards.

        for i:=0 to FDataSet.FCommon.FRecords.Count-1 do
        begin
             // Check status of record.
             pRec:=PkbmRecord(FDataSet.FCommon.FRecords.Items[i]);
             if pRec=nil then continue;

             // Find oldest version.
             pOrigRec:=pRec;
             while pOrigRec^.PrevRecordVersion<>nil do
                   pOrigRec:=pOrigRec^.PrevRecordVersion;


             // Check what status to react on.
             if pRec^.UpdateStatus=usDeleted then
             begin
                  // Dont resolve inserts that were deleted again.
                  if pOrigRec^.UpdateStatus=usInserted then st:=usUnmodified
                  else st:=usDeleted;
             end
             else if pOrigRec^.UpdateStatus=usInserted then st:=usInserted
             else st:=pRec^.UpdateStatus;
             FPRecord:=pRec;
             FPOrigRecord:=pOrigRec;

             BeforeRecord;
             try
                repeat
                  Retry:=false;
                  case st of
                       usDeleted:    DeleteRecord(Retry,st);
                       usInserted:   InsertRecord(Retry,st);
                       usModified:   ModifyRecord(Retry,st);
                       usUnModified: UnmodifiedRecord(Retry,st);
                  end;
                until not Retry;
             finally
                AfterRecord;
             end;
        end;
     finally
        FDataSet.FCommon.Unlock;

        // Check if to refresh other tables.
        FDataSet.FAttachedAutoRefresh:=oAttachedAutoRefresh;
        if FDataset.FAttachedAutoRefresh then
           FDataSet.FCommon.RefreshTables(nil);
     end;
end;

procedure TkbmCustomDeltaHandler.BeforeRecord;
begin
end;

procedure TkbmCustomDeltaHandler.InsertRecord(var Retry:boolean; var State:TUpdateStatus);
begin
end;

procedure TkbmCustomDeltaHandler.DeleteRecord(var Retry:boolean; var State:TUpdateStatus);
begin
end;

procedure TkbmCustomDeltaHandler.ModifyRecord(var Retry:boolean; var State:TUpdateStatus);
begin
end;

procedure TkbmCustomDeltaHandler.UnmodifiedRecord(var Retry:boolean; var State:TUpdateStatus);
begin
end;

procedure TkbmCustomDeltaHandler.AfterRecord;
begin
end;

function TkbmCustomDeltaHandler.GetFieldCount:integer;
begin
     CheckDataSet;
     Result:=FDataSet.FieldCount;
end;

function TkbmCustomDeltaHandler.GetOrigValues(Index:integer):Variant;
begin
     CheckDataSet;
     FDataSet.FOverrideActiveRecordBuffer:=FPOrigRecord;
     try
        if FDataSet.Fields[Index].IsNull then
           Result:=Null
        else
            Result:=FDataSet.Fields[Index].AsVariant;
     finally
        FDataSet.FOverrideActiveRecordBuffer:=nil;
     end;
end;

function TkbmCustomDeltaHandler.GetValues(Index:integer):Variant;
begin
     CheckDataSet;
     FDataSet.FOverrideActiveRecordBuffer:=FPRecord;
     try
        if FDataSet.Fields[Index].IsNull then
           Result:=Null
        else
            Result:=FDataSet.Fields[Index].AsVariant;

        if Assigned(FOnGetValue) then FOnGetValue(self,FDataSet.Fields[Index],Result);
     finally
        FDataSet.FOverrideActiveRecordBuffer:=nil;
     end;
end;

function TkbmCustomDeltaHandler.GetOrigValuesByName(Name:string):Variant;
var
   fld:TField;
begin
     CheckDataSet;
     FDataSet.FOverrideActiveRecordBuffer:=FPOrigRecord;
     try
        fld:=FDataSet.FieldByName(Name);
        if fld.IsNull then
           Result:=Null
        else
            Result:=fld.AsVariant;
     finally
        FDataSet.FOverrideActiveRecordBuffer:=nil;
     end;
end;

function TkbmCustomDeltaHandler.GetValuesByName(Name:string):Variant;
var
   fld:TField;
begin
     CheckDataSet;
     FDataSet.FOverrideActiveRecordBuffer:=FPRecord;
     try
        fld:=FDataSet.FieldByName(Name);
        if fld.IsNull then
           Result:=Null
        else
            Result:=fld.AsVariant;
        if Assigned(FOnGetValue) then FOnGetValue(self,fld,Result);
     finally
        FDataSet.FOverrideActiveRecordBuffer:=nil;
     end;
end;

function TkbmCustomDeltaHandler.GetFieldNames(Index:integer):string;
begin
     CheckDataSet;
     Result:=FDataSet.Fields[Index].FieldName;
end;

function TkbmCustomDeltaHandler.GetFields(Index:integer):TField;
begin
     CheckDataSet;
     Result:=FDataSet.Fields[Index];
end;

function TkbmCustomDeltaHandler.GetRecordNo:TkbmNativeInt;
begin
     Result:=FPRecord^.RecordNo+1;
end;

function TkbmCustomDeltaHandler.GetUniqueRecordID:TkbmNativeInt;
begin
     Result:=FPRecord^.UniqueRecordID;
end;

// TkbmCustomStreamFormat
//*******************************************************************

constructor TkbmCustomStreamFormat.Create(AOwner:TComponent);
begin
     inherited Create(AOwner);
{$IFDEF MSWINDOWS}
 {$IFDEF LEVEL16}
     FFormatSettings:=TFormatSettings.Create(LOCALE_SYSTEM_DEFAULT);
 {$ELSE}
     GetLocaleFormatSettings(LOCALE_SYSTEM_DEFAULT, FFormatSettings);
 {$ENDIF}
{$ELSE}
 {$IFDEF LEVEL16}
     FFormatSettings:=TFormatSettings.Create;
 {$ENDIF}
{$ENDIF}

     sfData:=[sfSaveData,sfLoadData];
     sfCalculated:=[];
     sfLookup:=[];
     sfNonVisible:=[sfSaveNonVisible,sfLoadNonVisible];
     sfBlobs:=[sfSaveBlobs,sfLoadBlobs];
     sfDef:=[sfSaveDef,sfLoadDef];
     sfIndexDef:=[sfSaveIndexDef,sfLoadIndexDef];
     sfFiltered:=[sfSaveFiltered];
     sfIgnoreRange:=[sfSaveIgnoreRange];
     sfIgnoreMasterDetail:=[sfSaveIgnoreMasterDetail];
     sfDeltas:=[];
     sfDontFilterDeltas:=[];
     sfAppend:=[];
     sfFieldKind:=[sfSaveFieldKind];
     sfFromStart:=[sfLoadFromStart];
//     FAutoReposition:=true;
end;

procedure TkbmCustomStreamFormat.Assign(Source:TPersistent);
begin
     if Source is TkbmCustomStreamFormat then
     begin
          sfData:=TkbmCustomStreamFormat(Source).sfData;
          sfCalculated:=TkbmCustomStreamFormat(Source).sfCalculated;
          sfLookup:=TkbmCustomStreamFormat(Source).sfLookup;
          sfNonVisible:=TkbmCustomStreamFormat(Source).sfNonVisible;
          sfBlobs:=TkbmCustomStreamFormat(Source).sfBlobs;
          sfDef:=TkbmCustomStreamFormat(Source).sfDef;
          sfIndexDef:=TkbmCustomStreamFormat(Source).sfIndexDef;
          sfFiltered:=TkbmCustomStreamFormat(Source).sfFiltered;
          sfIgnoreRange:=TkbmCustomStreamFormat(Source).sfIgnoreRange;
          sfIgnoreMasterDetail:=TkbmCustomStreamFormat(Source).sfIgnoreMasterDetail;
          sfDeltas:=TkbmCustomStreamFormat(Source).sfDeltas;
          sfDontFilterDeltas:=TkbmCustomStreamFormat(Source).sfDontFilterDeltas;
          sfAppend:=TkbmCustomStreamFormat(Source).sfAppend;
          sfFieldKind:=TkbmCustomStreamFormat(Source).sfFieldKind;
          sfFromStart:=TkbmCustomStreamFormat(Source).sfFromStart;
          exit;
     end;
     inherited Assign(Source);
end;

procedure TkbmCustomStreamFormat.SetIgnoreAutoIncPopulation(ADataset:TkbmCustomMemTable; Value:boolean);
begin
     ADataset.FIgnoreAutoIncPopulation:=value;
end;

procedure TkbmCustomStreamFormat.SetVersion(AVersion:string);
begin
end;

function  TkbmCustomStreamFormat.GetVersion:string;
begin
     Result:='1.00';
end;

procedure TkbmCustomStreamFormat.DetermineSaveFields(ADataset:TkbmCustomMemTable);
var
   i:TkbmNativeInt;
   nf:TkbmNativeInt;
begin
     // Setup flags for fields to save.
     with ADataset do
     begin
          nf:=Fieldcount;
          SetLength(SaveFields,nf);

          for i:=0 to nf-1 do
          begin
               // Default dont save this field.
               SaveFields[i]:=-1;

               // If a blob field, only save if specified.
               if (Fields[i].DataType in kbmBlobTypes) then
               begin
                    if not (sfSaveBlobs in sfBlobs) then continue;
                    SaveFields[i]:=i;
               end;

               // Only save fields of specific types.
               case Fields[i].FieldKind of
                    fkData,fkInternalCalc: if sfSaveData in sfData then SaveFields[i]:=i;
                    fkCalculated: if sfSaveCalculated in sfCalculated then SaveFields[i]:=i;
                    fkLookup: if sfSaveLookup in sfLookup then SaveFields[i]:=i;
                    else SaveFields[i]:=-1;
               end;

               // If not to save invisible fields, dont.
               if not (Fields[i].Visible or (sfSaveNonVisible in sfNonVisible)) then SaveFields[i]:=-1;
          end;
     end;
end;

procedure TkbmCustomStreamFormat.DetermineLoadFields(ADataset:TkbmCustomMemTable; Situation:TkbmDetermineLoadFieldsSituation);
var
   i,j,nf:TkbmNativeInt;
   lst:TStringList;
begin
     // Setup flags for fields to save.
     lst:=TStringList.Create;
     try
        with ADataset do
        begin
             lst.Clear;
             DetermineLoadFieldIDs(ADataset,lst,Situation);
             nf:=lst.Count;
             SetLength(LoadFields,nf);
             SetLength(LoadFieldWidths,nf);

             for i:=0 to nf-1 do
             begin
                  // Default dont load this field.
                  LoadFields[i]:=-1;
                  LoadFieldWidths[i]:=-1;

                  // Let child component make initial desision of what to load and order.
                  DetermineLoadFieldIndex(ADataset,lst.Strings[i],nf,i,LoadFields[i],Situation);

                  // Only load fields of specific types.
                  j:=LoadFields[i];
                  if j>=0 then
                  begin
                       case Fields[j].FieldKind of
                            fkData,fkInternalCalc: if not (sfLoadData in sfData) then LoadFields[i]:=-1;
                            fkCalculated: if not (sfLoadCalculated in sfCalculated) then LoadFields[i]:=-1;
                            fkLookup: if not (sfLoadLookup in sfLookup) then LoadFields[i]:=-1;
                            else LoadFields[i]:=-1;
                       end;

                       // If a blob field, only load if specified.
                       if (Fields[j].DataType in kbmBlobTypes) and (not (sfLoadBlobs in sfBlobs)) then LoadFields[i]:=-1;

                       // If not to load invisible fields, dont.
                       if not (Fields[j].Visible or (sfLoadNonVisible in sfNonVisible)) then LoadFields[i]:=-1;
                  end;
             end;
        end;
     finally
        lst.Free;
     end;
end;

procedure TkbmCustomStreamFormat.BeforeSave(ADataset:TkbmCustomMemTable);
begin
     with ADataset do
     begin
          DisableControls;

          Common.Lock;
          TableState:=mtstSave;
          Progress(0,mtpcSave);

          FWasFiltered:=Filtered;
          FWasRangeActive:=RangeActive;
          FWasMasterLinkUsed:=FCurIndex=FDetailIndex;

          if Filtered and (sfSaveFiltered in sfFiltered) then Filtered:=false;
          if FWasMasterLinkUsed and (sfSaveIgnoreMasterDetail in sfIgnoreMasterDetail) then
          begin
             SwitchToIndex(FDetailIndex.Base);
             FMasterLinkUsed:=false;
          end;
          if RangeActive and (sfSaveIgnoreRange in sfIgnoreRange) then
             SwitchToIndex(FRangeIndex.Base);

          // If to compress stream, create memory stream to save to instead.
          if Assigned(FOnCompress) then
             FWorkStream:=TMemoryStream.Create
          else
             FWorkStream:=FOrigStream;

          // Check if to append. If not truncate stream.
          if (sfSaveAppend in sfAppend) then
             FWorkStream.Seek(0,TSeekOrigin.soEnd)
          else if not (sfSaveInsert in sfAppend) then
          begin
               FWorkStream.Size:=0;
               FWorkStream.Position:=0;
          end;

          // Determine fields to save.
          DetermineSaveFields(ADataset);

          SetIsFiltered;
     end;
end;

procedure TkbmCustomStreamFormat.AfterSave(ADataset:TkbmCustomMemTable);
begin
     try
        // If to compress stream do the compression to the dest stream.
        if Assigned(FOnCompress) then
           FOnCompress(ADataset,FWorkStream,FOrigStream);

        with ADataset do
        begin
             if FWasMasterLinkUsed then
             begin
                  SwitchToIndex(FDetailIndex);
                  FMasterLinkUsed:=true;
             end;
             if FWasRangeActive then
                SwitchToIndex(FRangeIndex);
             Filtered:=FWasFiltered;

             TableState:=mtstBrowse;
             Progress(100,mtpcSave);
             Common.Unlock;

             SetIsFiltered;
             EnableControls;
        end;
     finally
        if FWorkStream<>FOrigStream then
        begin
             FWorkStream.Free;
             FWorkStream:=nil;
        end;
     end;
end;

procedure TkbmCustomStreamFormat.SaveDef(ADataset:TkbmCustomMemTable);
begin
end;

procedure TkbmCustomStreamFormat.SaveData(ADataset:TkbmCustomMemTable);
begin
end;

procedure TkbmCustomStreamFormat.Save(ADataset:TkbmCustomMemTable);
begin
     if Assigned(FOnBeforeSave) then FOnBeforeSave(self);
     SaveDef(ADataset);
     SaveData(ADataset);
     if Assigned(FOnAfterSave) then FOnAfterSave(self);
end;

procedure TkbmCustomStreamFormat.BeforeLoad(ADataset:TkbmCustomMemTable);
begin
     with ADataset do
     begin
          // Dont let persistence react on internal open/close statements.
          DisableControls;

          Common.Lock;

          TableState:=mtstLoad;

          FWasPersistent:=FPersistent;
          FWasEnableIndexes:=FEnableIndexes;

          FPersistent:=false;
          FEnableIndexes:=false;

          FIgnoreReadOnly:=true;
          FIgnoreAutoIncPopulation:=true;

          if Active and (RecordCount>0) and FAutoReposition then
             FBookmark:=GetBookmark
          else
             FBookmark:=nil;
          Progress(0,mtpcLoad);

          // If to decompress stream, create memory stream to load from instead.
          if Assigned(OnDecompress) then
          begin
               FWorkStream:=TMemoryStream.Create;
               if sfLoadFromStart in sfFromStart then
                  FOrigStream.Position:=0;
               OnDecompress(ADataset,FOrigStream,FWorkStream);
               FWorkStream.Position:=0;
          end
          else
          begin
               FWorkStream:=FOrigStream;
               if sfLoadFromStart in sfFromStart then
                  FWorkStream.Position:=0;
          end;

          // Determine fields to load.
          DetermineLoadFields(ADataset,dlfBeforeLoad);
     end;
end;

procedure TkbmCustomStreamFormat.AfterLoad(ADataset:TkbmCustomMemTable);
var
   i,j:TkbmNativeInt;
   nf:TkbmNativeInt;
begin
     try
        with ADataset do
        begin
             // Check if to update displaywidths.
             nf:=length(LoadFields);
             for i:=0 to nf-1 do
             begin
                  if LoadFieldWidths[i]>0 then
                  begin
                       j:=LoadFields[i];
                       if j>=0 then
                          ADataSet.Fields[j].DisplayWidth:=LoadFieldWidths[i];
                  end;
             end;
             // Dont let persistence react on internal open/close statements.
             FPersistent:=FWasPersistent;
             FEnableIndexes:=FWasEnableIndexes;

             FIgnoreReadOnly:=false;
             FIgnoreAutoIncPopulation:=false;

             Common.MarkIndexesDirty;
             Common.RebuildIndexes;
             Common.Unlock;

             Reposition(ADataset);

             EnableControls;

             Progress(100,mtpcLoad);
             TableState:=mtstBrowse;
//             if FAutoReposition then Refresh;
             if FAutoUpdateFieldVariables then UpdateFieldVariables;
        end;
        Refresh(ADataset);
     finally
        if FWorkStream<>FOrigStream then
        begin
             FWorkStream.Free;
             FWorkStream:=nil;
        end;
     end;
end;

procedure TkbmCustomStreamFormat.Reposition(ADataset:TkbmCustomMemTable);
begin
     with ADataset do
     begin
          __ClearBuffers;
          if FAutoReposition then
          begin

               if Assigned(FBookmark) then
               begin
                    if BookmarkValid(FBookmark) then
                       GotoBookmark(FBookmark)
                    else
                        First;
                    FreeBookmark(FBookmark);
                    FBookmark:=nil;
               end
               else
                   First;
          end
          else
              First;
     end;
end;

procedure TkbmCustomStreamFormat.Refresh(ADataset:TkbmCustomMemTable);
begin
     ADataset.Refresh;
end;

procedure TkbmCustomStreamFormat.LoadDef(ADataset:TkbmCustomMemTable);
begin
end;

procedure TkbmCustomStreamFormat.LoadData(ADataset:TkbmCustomMemTable);
begin
end;

procedure TkbmCustomStreamFormat.Load(ADataset:TkbmCustomMemTable);
begin
     if Assigned(FOnBeforeLoad) then FOnBeforeLoad(self);
     LoadDef(ADataset);
     DetermineLoadFields(ADataset,dlfAfterLoadDef); // Give another chance. LoadDef might have changed something,
     if Length(LoadFields)<=0 then
        raise EMemTableError.Create('Couldnt determine field count for load.');
     LoadData(ADataset);
     if Assigned(FOnAfterLoad) then FOnAfterLoad(self);
end;

procedure TkbmCustomStreamFormat.DetermineLoadFieldIDs(ADataset:TkbmCustomMemTable; AList:TStringList; Situation:TkbmDetermineLoadFieldsSituation);
var
   i:TkbmNativeInt;
begin
     AList.Clear;
     for i:=0 to ADataset.FieldCount-1 do
         AList.Add(ADataset.Fields[i].DisplayName);
end;

procedure TkbmCustomStreamFormat.DetermineLoadFieldIndex(ADataset:TkbmCustomMemTable; ID:string; FieldCount:integer; OrigIndex:integer; var NewIndex:integer; Situation:TkbmDetermineLoadFieldsSituation);
begin
     // Default dont load anything.
end;

end.
