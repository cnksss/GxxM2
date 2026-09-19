program demo;

uses kbmMemTable, kbmMemCSVStreamFormat, DB, SysUtils;

var
  mt:TkbmMemTable;
  csv:TkbmCSVStreamFormat;
  i:integer;
begin
     mt:=TkbmMemTable.Create(nil);
     try
        // Define fields for table.
        mt.FieldDefs.Clear;
        mt.FieldDefs.Add('ID',ftAutoInc,0,true);
        mt.FieldDefs.Add('Name',ftString,30,false);
        mt.FieldDefs.Add('Value',ftFloat,0,false);
        mt.CreateTable;
        mt.Open;

        // Populate table with data.
        for i:=0 to 100 do
        begin
             mt.Append;
             mt.FieldByName('Name').AsString:='AName '+inttostr(i);
             mt.FieldByName('Value').AsFloat:=i * 100 / 3;
             mt.Post;
        end;

        // Store table to disk.
        csv:=TkbmCSVStreamFormat.Create(nil);
        try
           mt.DefaultFormat:=csv;
           mt.SaveToFile('demo.csv');
        finally
           csv.Free;
        end;
     finally
        mt.Free;
     end;
end.
