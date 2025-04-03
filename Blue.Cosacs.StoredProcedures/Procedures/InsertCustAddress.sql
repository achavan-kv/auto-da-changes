SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

---------------------------------------------------------------------------------------------------

IF exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[InsertCustAddress]') and objectproperty(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[InsertCustAddress]
GO

CREATE PROCEDURE [dbo].[InsertCustAddress] 	
	@origbr smallint ,
	@custid varchar(20)  ,
	@addtype char(2)  ,
	@datein datetime  ,
	@cusaddr1 varchar(50) ,
	@cusaddr2 varchar(50) ,
	@cusaddr3 varchar(50) ,
	@cuspocode varchar(10) ,
	@custlocn varchar(76) ,
	@resstatus char(1) ,
	@mthlyrent float ,
	@datemoved datetime ,
	@hasstring smallint  ,
	@Email char(60)  ,
	@PropType char(4)  ,
	@empeenochange int  ,
	@datechange smalldatetime ,
	@Notes nvarchar(1000)  ,
	@deliveryarea nvarchar(8) ,
	@zone varchar(4)
AS

INSERT INTO custaddress
        ( origbr ,
          custid ,
          addtype ,
          datein ,
          cusaddr1 ,
          cusaddr2 ,
          cusaddr3 ,
          cuspocode ,
          custlocn ,
          resstatus ,
          mthlyrent ,
          datemoved ,
          hasstring ,
          Email ,
          PropType ,
          empeenochange ,
          datechange ,
          Notes ,
          deliveryarea ,
          zone
        )
VALUES  ( @origbr, -- origbr - smallint
          @custid , -- custid - varchar(20)
          @addtype , -- addtype - char(2)
          @datein , -- datein - datetime
          @cusaddr1 , -- cusaddr1 - varchar(50)
          @cusaddr2 , -- cusaddr2 - varchar(50)
          @cusaddr3 , -- cusaddr3 - varchar(50)
          @cuspocode , -- cuspocode - varchar(10)
          @custlocn , -- custlocn - varchar(76)
          @resstatus , -- resstatus - char(1)
          @mthlyrent , -- mthlyrent - float
          @datemoved , -- datemoved - datetime
          @hasstring , -- hasstring - smallint
          @Email , -- Email - char(60)
          @PropType , -- PropType - char(4)
          @empeenochange , -- empeenochange - int
          @datechange , -- datechange - smalldatetime
          @Notes , -- Notes - nvarchar(1000)
          @deliveryarea , -- deliveryarea - nvarchar(8)
          @zone  -- zone - varchar(4)
        )