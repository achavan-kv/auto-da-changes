IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[HyperionEODSP]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[HyperionEODSP]
GO

SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[HyperionEODSP]
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : HyperionEOD.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Hyperion End of Day
-- Author       : John Croft
-- Date         : 12 October 2012
--
-- This procedure will create execute the Hyperion Extract procedures
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 12/04/13 jec #12859 - UAT12602
-- 16/10/13 ip #15464 - Raise an error if BCP Path is incorrect.
-- ================================================
		@return INT output
As	
	set @return = 0
	
	declare  @process VARCHAR(50), @datefrom DATETIME, @dateto DATETIME, @runno INT,
			 @BCPPathCheck varchar(200),		--#15464
			 @BCPFileName varchar(200),			--#15464
			 @BCPCheck varchar(500),			--#15464
			 @CheckExists int					--#15464
	
	select @runno= MAX(runno) from InterfaceControl where interface='Hyperion'
	
	select @dateFrom= DATEADD(MONTH, -1, DATEADD(month, DATEDIFF(MONTH, 0, datestart), 0)) from InterfaceControl where runno=@runno and interface='Hyperion'		-- #12859
	SET @dateTo = DATEADD(MONTH, 1, @dateFrom)				-- #12859
	
	--#15464
	select @BCPPathCheck = value from countrymaintenance WHERE Codename = 'BCPpath' 
	set @BCPFileName = '\bcp.exe'
	set @BCPCheck = @BCPPathCheck + @BCPFileName

	EXEC master..xp_fileexist @BCPCheck, @CheckExists out --Check if the BCP Path is setup correctly.

	if(@CheckExists = 1)		--#15464
	BEGIN

		set @process='t_cat_orden_service'	
		exec t_cat_orden_service

		if @@ERROR=0
		BEGIN
			set @process='t_cat_prod_departamento'	
			exec t_cat_prod_departamento
		END
		if @@ERROR=0
		BEGIN
			set @process='t_cat_prod_division'	
			exec t_cat_prod_division
		END
		if @@ERROR=0
		BEGIN
			set @process='t_cat_productos'	
			exec t_cat_productos
		END
		if @@ERROR=0
		BEGIN
			set @process='t_src_aging_rr'	
			exec t_src_aging_rr @dateFrom, @dateTo
		END
		if @@ERROR=0
		BEGIN
			set @process='t_src_cyc_indicadores'	
			exec t_src_cyc_indicadores @dateTo
		END
		if @@ERROR=0
		BEGIN
			set @process='t_src_cyc_plazos_cartera'	
			exec t_src_cyc_plazos_cartera @dateTo
		END
		if @@ERROR=0
		BEGIN
			set @process='t_src_tipo_producto'	
			exec t_src_tipo_producto @dateTo
		END
		if @@ERROR=0
		BEGIN
			set @process='t_src_plan_pagos'	
			exec t_src_plan_pagos @dateTo
		END
		if @@ERROR=0
		BEGIN
			set @process='t_src_service_servitotal'	
			exec t_src_service_servitotal @dateFrom, @dateTo
		END
		if @@ERROR=0
		BEGIN
			set @process='t_src_sysde_claseprod'	
			exec t_src_sysde_claseprod @dateFrom, @dateTo
		END
		
		if @@ERROR !=0
			BEGIN 
				set @return=@@ERROR	
				declare @error VARCHAR(100)=convert(VARCHAR(5),@return) + ' Hyperion Extract End of day failed - process = ' + @process
				RAISERROR (@error,16,1)	
				RETURN 
			END 
	END
	ELSE	--#15464	
	BEGIN		
			raiserror ('The BCP Path setup beneath Country Maintenance is incorrect',16,1)
	END
GO
-- end end end end end end end end end end end end end end end end end end end end end end end
