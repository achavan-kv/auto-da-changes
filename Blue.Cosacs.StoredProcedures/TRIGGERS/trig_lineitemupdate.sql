/*05/10/04 KEF Incorrect where clause, joining inserted.itemno with delelted.acctno, changed to deleted.itemno */

if exists (select * from sysobjects where name = 'trig_lineitemupdate')
drop trigger trig_lineitemupdate
GO

CREATE TRIGGER trig_lineitemupdate
ON lineitem For UPDATE

As 
DECLARE 
	@acctno char(12) ,@itemno varchar (10),@stocklocn smallint,@oldcontractno varchar (10),
	@error varchar(256), @category integer, @newcontractno varchar (10), @itemId INT		-- RI jec 06/06/11

SELECT
	@acctno = inserted.acctno,
	@itemno = s.IUPC,			-- RI jec 06/06/11  inserted.itemno,
	@itemid=inserted.itemId,			-- RI jec 06/06/11
	@stocklocn = inserted.stocklocn,
	@newcontractno = ISNULL(inserted.contractno,''),
	@oldcontractno = ISNULL(deleted.contractno,'')
	
FROM inserted INNER JOIN stockinfo s on inserted.itemId	= s.ID, deleted
WHERE inserted.acctno = deleted.acctno and inserted.itemId = deleted.itemId	and		-- RI jec 06/06/11  inserted.itemno = deleted.itemno and 
		inserted.stocklocn = deleted.stocklocn and deleted.contractno != inserted.contractno and
		--inserted.parentitemno = deleted.parentitemno -- NM 09/04/2009 to avoid an issue in Oracle Export (CR996)
		inserted.parentitemid = deleted.parentitemId		-- RI jec 06/06/11
		
IF @oldcontractno != '' and  @newcontractno = ''
BEGIN
	Select @category = category From stockitem 
		Where itemid = @itemId and stocklocn = @stocklocn		-- RI jec 06/06/11
	
	--IF @category in (12, 82) --warranty categories
	IF @category in (select distinct code from code where category ='WAR') --warranty categories  --IP - 30/10/09 - CoSACS Improvement - Remove hardcoded discounts & warranties
	BEGIN
		SET @error =' error blank contract saving ' + @acctno + ' ' +  @itemno
  		RAISERROR(@error, 16, 1) 
	END
END

GO
