/*

** Created by : Alex Ayscough
** Created on : 01 Apr 2005
*/

IF EXISTS(SELECT name FROM sysobjects
	  WHERE name = 'trig_branch_update' 
  	  AND type = 'TR')

DROP TRIGGER trig_branch_update
go

CREATE TRIGGER trig_branch_update
ON branch
FOR update
AS 
  declare @warehouseno varchar (3),@error varchar (500)
	select @warehouseno = warehouseno from inserted

  if Not(datalength(@warehouseno) =2 or datalength(@warehouseno) =3)
   begin
      set @error =' warehouseno must be two or three digits for FACT 2000 interface: raised by trigger trig_branch_update'
          RAISERROR(@error, 16, 1)
  end
GO
