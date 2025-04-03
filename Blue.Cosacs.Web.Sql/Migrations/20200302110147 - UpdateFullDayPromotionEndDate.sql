-- ===================================================================================
-- Author:			Ashwini Akula
-- Create Date:		02/03/2020
-- Description:		To update all the existing promotion's end date's timestamp from
--					'00:00:00' to '23:59:00' in case of fullday promotion.
-- ===================================================================================

UPDATE	Merchandising.Promotion 
SET		EndDATE = CAST(CONVERT(VARCHAR(10), CONVERT(DATE,enddate), 23) + ' ' + '23:59:00' AS DATETIME) 
WHERE	CONVERT(VARCHAR(8),enddate, 108) = '00:00:00' 

