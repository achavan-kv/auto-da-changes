SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CountryGetOptionsSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CountryGetOptionsSP]
GO

CREATE PROCEDURE 	dbo.DN_CountryGetOptionsSP
			@country		char(2),
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	origbr,
			countrycode,
			countryname,
			hobranchno,
			cheqdays,
			letterdays,
			letterporw,
			taxtype,
			taxrate,
			taxname,
			servpcent,
			daterun,
			dateweek1,
			weekno,
			batchctrlno,
			bailfee,
			bailpcent,
			rcptsperbook,
			weeklst400,
			taxinvtype,
			agrmttaxtype,
			datelastscor,
			minhpage,
			maxhpage,
			datelastcalc,
			dailyexport,
			globdelpcent,
			onemonthdel,
			deldays,
			arrearsonpay,
			systemopen,
			timing,
			addtomin,
			addtoterm,
			adminfee,
			autodownsc5,
			cashintrate,
			datechargesstart,
			lastchargesweekno,
			lastyrlastweekno,
			lettersgap,
			percentaddto1,
			percentaddto2,
			percentaddto3,
			smallbalance,
			dutyfree,
			loyaltycard,
			locktimeout,
			rebpcent,
			deliveryslots,
			defdelnotebranch,
			minperiod,
			ddleadtime,
			ddfee,
			ddenabled,
			ddbankacctno,
			highstatus,
			paymentmethod,
			agrgprint,
			agrtimeprint,
			print90,
			serviceprint,
			nocents,
			ddgenno,
			ddservicetype,
			ddidcodefrom,
			ddidcodeto,
			ddbankbranch,
			ddbankacctname,
			ddbankaccttype,
			ddtest,
			varstamp,
			notaxcopies,
			noagrcopies,
			agrprinttype,
			ddmaxrejections,
			ddfeeoverride,
			fixeddatefirst,
			transactenabled,
			transacturl,
			AllowZeroStock,
			CODDefault,
			decimalPlaces,
			sanctionminyears,
			mandwarrantyno=case mandwarrantyno
			when 0 then convert(tinyint,1)
			else convert(tinyint,0)
                                        end,
			paywholeunits,
			bcommissionequalsfee,
			serviceprintdp,
			manualrefer,
			LaserPrintTax,
			printwarrantycontract,
			codpercentage,
			noninterestitem,
			PrintCreditNote,
			WarrantyCustCopy,
			WarrantyCreditCopy,
			WarrantyHOCopy,
			DefaultDelDays,
			percenttopay,
			Securitisation,
			NoRFSummary,
			NoRFDetails,
			CreditScanInterval,
			SecureRefunds,
			warrantystylesheet,
		             minreferences,
			creditwarrantydays,
			printscheduleofpayments,
			CancellationCode as 'CancellationRejectionCode',
			PrintToolBar,
			giftvoucheraccount,
			defaultvoucherexpiry,
			RIFileTransfer
	FROM country
	WHERE countrycode = @country

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

