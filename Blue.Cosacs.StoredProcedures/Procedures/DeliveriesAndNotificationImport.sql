IF EXISTS
(SELECT name
FROM    sysobjects
WHERE   name = N'DeliveriesAndNotificationImport'
    AND type = 'P'
)
DROP PROCEDURE DeliveriesAndNotificationImport
GO
CREATE PROCEDURE [dbo].[DeliveriesAndNotificationImport]
-- =============================================
-- Author:		??
-- Create date: ??
-- Description:	Deliveries and Notification Import
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 02/03/10  jec CR1072 Malaysia merge
-- =============================================
AS
        --*****************************************************************************
        -- begin get list of files for import
        --*****************************************************************************
        DECLARE @vchrFile      VARCHAR(1000),
                @fileListRow   VARCHAR(300),
                @fileWithPath  VARCHAR(1000),
                @command       VARCHAR(2000),
                @runno         INT,
                @nextsequence INT,
                @lastsequence INT,
                @dosCommand    VARCHAR(8000),
                @first BIT,
                @err           INT
            
        
        SET NOCOUNT ON
        
        SET XACT_ABORT OFF
        
        DECLARE @transValue MONEY
        
        -- get all despatchnotes from directory
        SELECT @vchrFile = [Value]
        FROM   CountryMaintenance
        WHERE  codename = '3PLFileDir'
        -- UAT 4 check for final '\' in directory in @vchrFile, append if neccesary
        IF RIGHT(@vchrFile,1) <> '\'
        BEGIN
                SET @vchrFile = @vchrFile + '\'
        END
        -- Build the dos command to get a list of files
        SELECT @dosCommand = 'insert into #tempFileList(fileListRow) ' + 'exec master.dbo.xp_cmdshell ''dir /b /ON ' + @vchrFile + 'POD*.DAT '''
        -- Create a temporary table to store the file list
        CREATE TABLE #tempFileList
                     (
                                  fileListRow VARCHAR(1000) NULL
                     )
        EXEC(@dosCommand)
        --*****************************************************************************
        -- begin get list of files for import
        --*****************************************************************************
        -- CR 953 Check for existence of files in the directory
        IF
        (SELECT  TOP 1 fileListRow
                 FROM     [#tempFileList]
                 WHERE    fileListRow IS NOT NULL
                 ORDER BY fileListRow
        )
        = 'File Not Found'
        BEGIN
                SELECT @RunNo = RunNo
                FROM   interfacecontrol
                WHERE  interface = 'LOGIMPORT'
                IF @RunNo IS NULL
                OR @RunNo  = ''
                SET @RunNo = 1
                ELSE
                SET @RunNo = @RunNo + 1
                INSERT
                INTO   interfacecontrol
                       (
                              interface,
                              runno,
                              datestart,
                              result,
                              filename
                       )
                       VALUES
                       (
                              'LOGIMPORT',
                              @runno,
                              GETDATE(),
                              'F',
                              NULL
                       )
                INSERT
                INTO   interfaceerror
                       (
                              interface,
                              runno,
                              errordate,
                              severity,
                              errortext
                       )
                       VALUES
                       (
                              'LOGIMPORT',
                              @runno,
                              GETDATE(),
                              0,
                              'No POD files exist in the 3PL File Directory'
                       )
                       RAISERROR
                       (
                              'No POD files exist',
                              16,1
                       )
                RETURN
        END
        -- CR 953 Check that first file (i.e one with the lowest sequence number) is the next in sequence. If not then terminate and log an error.
        
        SET @nextSequence =
        (SELECT  TOP 1 SUBSTRING(fileListRow,LEN(fileListRow) - 8,5)
                 FROM     [#tempFileList]
                 WHERE    fileListRow IS NOT NULL
                 ORDER BY fileListRow
        )
       
        SET @lastSequence =
        (SELECT  TOP 1 SUBSTRING([fileName],LEN([fileName]) - 8,5)
                 FROM     interfacecontrol
                 WHERE    interface           = 'LOGIMPORT'
                      AND result              = 'P'
                      AND RIGHT([fileName],3) = 'DAT'
                 ORDER BY SUBSTRING([fileName],LEN([fileName]) - 8,5) DESC
        )
        IF @nextSequence <> @lastSequence + 1
        BEGIN
                SELECT @RunNo = RunNo
                FROM   interfacecontrol
                WHERE  interface = 'LOGIMPORT'
                IF @RunNo IS NULL
                OR @RunNo  = ''
                SET @RunNo = 1
                ELSE
                SET @RunNo       = @RunNo + 1
                SET @fileListRow =
                (SELECT  TOP 1 fileListRow
                FROM     [#tempFileList]
                WHERE    fileListRow IS NOT NULL
                ORDER BY fileListRow
                )
                SET @fileWithPath = @vchrFile + @fileListRow
                INSERT
                INTO   interfacecontrol
                       (
                              interface,
                              runno,
                              datestart,
                              result,
                              filename
                       )
                       VALUES
                       (
                              'LOGIMPORT',
                              @runno,
                              GETDATE(),
                              'F',
                              @fileWithPath
                       )
                INSERT
                INTO   interfaceerror
                       (
                              interface,
                              runno,
                              errordate,
                              severity,
                              errortext
                       )
                       VALUES
                       (
                              'LOGIMPORT',
                              @runno,
                              GETDATE(),
                              0,
                              'File name does not contain the next sequence number. file: ' + @fileWithPath
                       )
                       RAISERROR
                       (
                              'File name does not contain the next sequence number',
                              16,1
                       )
                RETURN
        END
        --------------------------------------------------------------------------------------------------------------------
        -- File list now loaded into #tempfilelist.
        -- Ready to loop through each file.
        --------------------------------------------------------------------------------------------------------------------
        
        SET @first = 1
        -- now loop through each file
        -- SELECT * FROM #tempfilelist
        DECLARE c1 CURSOR READ_ONLY FOR
        SELECT   fileListRow
        FROM     #tempfilelist
        ORDER BY fileListRow OPEN c1
        FETCH NEXT
        FROM  c1
        INTO  @fileListRow
        WHILE @@FETCH_STATUS = 0
        BEGIN
			EXEC DeliveriesAndNotificationImport_File   @fileWithPath =  @fileWithPath,
								                        @vchrFile = @vchrFile,    
														@fileListRow = @fileListRow,  
														@first = @first,
														@nextsequence = @nextsequence, 
														@lastsequence = @lastsequence 
        
                FETCH NEXT
                FROM  c1
                INTO  @fileListRow
        END
        CLOSE c1
        DEALLOCATE c1
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End