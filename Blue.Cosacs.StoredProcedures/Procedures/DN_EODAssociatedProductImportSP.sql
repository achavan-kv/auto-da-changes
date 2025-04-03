SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
SET ANSI_WARNINGS ON
go

if exists (SELECT * FROM dbo.sysobjects WHERE id = object_id('[dbo].[DN_EODAssociatedProductImportSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_EODAssociatedProductImportSP]
GO

CREATE PROCEDURE DN_EODAssociatedProductImportSP
        @source varchar(20),
        @return int OUTPUT

AS

    SET 	@return = 0			--initialise return code
     
    
    DELETE 
    FROM 
        StockInfoAssociated
    WHERE
        [Source] = @source

	SET @return = @@error

    select @return


	IF(@source = 'NonStocks')
    BEGIN
	    IF(@return = 0)
	    BEGIN


            UPDATE 
                dbo.temp_nonstockassociatedload
             SET 
                ProductGroup = LTRIM(RTRIM(ProductGroup)),
                Category = LTRIM(RTRIM(Category)),
                Class = LTRIM(RTRIM(Class)),
                SubClass = LTRIM(RTRIM(SubClass)),
                AssocItemId=LTRIM(RTRIM(REPLACE(REPLACE(REPLACE(AssocItemId, CHAR(10), ''), CHAR(13), ''), CHAR(9), '')))

             SELECT 
                 CASE
                    WHEN isnull(t.ProductGroup,'') = ''
                        THEN 'Any'
					WHEN h.Name = 'Furniture'
					THEN 'PCF'
					ELSE 'PCE'
                END as ProductGroup,
                CASE 
                    WHEN isnull(t.category,'') = ''
                        THEN 0
                    WHEN isnull(dmcategory.LegacyCode, '') = ''
                        THEN 0
                    ELSE
                        CAST(dmcategory.LegacyCode as int)
                    END as Category,
                CASE
                    WHEN isnull(t.Class,'') = ''
                        THEN 'Any'
                    WHEN isnull(hclass.Code, '') = ''
                        THEN 'Any'
                    ELSE
                        hclass.Code
                END as Class,
                CASE
                    WHEN isnull(t.SubClass,'') = ''
                        THEN 'Any'
                    WHEN isnull(hsubclass.Code, '') = ''
                        THEN 'Any'
                    ELSE    
                        hsubclass.Code
                END as SubClass,
                si.Id as AssocItemId,
                @source as [Source]
            INTO
                #tempNonStocks
            FROM
                temp_nonstockassociatedload t
            INNER JOIN 
                stockinfo si on si.itemno = t.AssocItemId
            LEFT JOIN 
                Merchandising.HierarchyTag h on h.Id = t.productgroup
            LEFT JOIN 
                Merchandising.HierarchyTag hcategory on hcategory.Id = t.category
            LEFT JOIN
                Merchandising.ClassMapping dmcategory on hcategory.Code = dmcategory.ClassCode
            LEFT JOIN 
                Merchandising.HierarchyTag hclass on hclass.Id = t.class
            LEFT JOIN 
                Merchandising.HierarchyTag hsubclass on hsubclass.Id = t.subclass
               


	        INSERT INTO StockInfoAssociated
	        (
	     	    ProductGroup,
			    Category,
                Class,
                SubClass,
                AssocItemId,
                [Source]    
	        )
	        SELECT distinct
                t.ProductGroup,
                t.Category,
                t.Class,
                t.SubClass,
                t.AssocItemId,
                t.[Source]
	        FROM   
                #tempNonStocks t

            drop table #tempNonStocks

		    SET @return = @@error
	    END
    END 
	IF(@source = 'Merchandising')
    BEGIN
	    IF(@return = 0)
	    BEGIN

             UPDATE 
                dbo.temp_MerchandisingAssociatedLoad
             SET 
                Division = LTRIM(RTRIM(Division)),
                Department = LTRIM(RTRIM(Department)),
                Class = LTRIM(RTRIM(Class)),
                SubClass = LTRIM(RTRIM(SubClass)),
                SKU=LTRIM(RTRIM(REPLACE(REPLACE(REPLACE(SKU, CHAR(10), ''), CHAR(13), ''), CHAR(9), '')))

	        INSERT INTO StockInfoAssociated
	        (
	     	    ProductGroup,
			    Category,
                Class,
                SubClass,
                AssocItemId,
                [Source]    
	        )
	        SELECT 
                CASE
                    WHEN isnull(t.Division,'') = ''
                        THEN 'Any'
					WHEN t.division = 'Furniture'
					THEN 'PCF'
					ELSE 'PCE'
                END,
				CASE
					WHEN isnull(t.Department,'') = ''
                        THEN 0
                    ELSE
                        t.Department
					END,
                CASE
                    WHEN isnull(t.Class,'') = ''
                        THEN 'Any'
                    ELSE
                        t.Class
                END,
                CASE
                    WHEN isnull(t.SubClass,'') = ''
                        THEN 'Any'
                    ELSE    
                        t.SubClass
                END,
                si.Id,
                @source
	        FROM   
                temp_MerchandisingAssociatedLoad t
            INNER JOIN 
                stockinfo si on si.itemno = t.SKU

		    SET @return = @@error
	    END
    END  
    


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
SET ANSI_WARNINGS Off
