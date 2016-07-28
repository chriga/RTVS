﻿CREATE PROCEDURE _PROCEDURENAME_
AS
BEGIN
DECLARE @RCodeQuery NVARCHAR(max);
DECLARE @RCode NVARCHAR(max);
DECLARE @ParmDefinition NVARCHAR(max);

SET @RCodeQuery = 'SELECT @RCodeOUT = RCode FROM RCodeTable WHERE SProcName = ''_PROCEDURENAME_''';
SET @ParmDefinition = N'@RCodeOUT NVARCHAR(max) OUTPUT';

EXEC sp_executesql @RCodeQuery, @ParmDefinition, @RCodeOUT=@RCode OUTPUT;
SELECT @RCode;

EXEC sp_execute_external_script @language = N'R'
    , @script = @RCode
    , @input_data_1 = N'SELECT * FROM ABC'
    WITH RESULT SETS (([MY] NVARCHAR(max)));
END;
