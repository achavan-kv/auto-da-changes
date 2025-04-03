-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

if not exists( select * from Code where Code='Hyperion' and CATEGORY='EDC')
Begin
insert into dbo.code (
	origbr,
	category,
	code,
	codedescript,
	statusflag,
	sortorder,
	reference,
	additional
) VALUES ( 
	/* origbr - smallint */ 0,
	/* category - varchar(12) */ 'EDC',
	/* code - varchar(12) */ 'Hyperion',
	/* codedescript - nvarchar(64) */ N'Hyperion Data Extract',
	/* statusflag - char(1) */ 'L',
	/* sortorder - smallint */ 8,
	/* reference - varchar(12) */ '0',
	/* additional - varchar(15) */ '0' )	-- CanReRun

End

if exists (select * from sys.tables where name = 'productclass')
BEGIN
	DROP TABLE dbo.productclass
END

if NOT exists (select * from sys.tables where name = 'productclass')
BEGIN
		select '00' as class, convert(varchar(100),'FREE GIFTS') as [description]
		into productclass 

		insert into productclass
		select '10', 'TELEVISIONS' union 
		select '11', 'VIDEO' union 
		select '12', 'VIDEO CAMERA/PHOTOGRAPHY' union 
		select '13', 'COMPUTER GAMES' union 
		select '19', 'WARRANTIES' union 
		select '20', 'MAJOR AUDIO SYSTEMS' union 
		select '21', 'MAJOR AUDIO SEPARATES' union 
		select '22', 'PORTABLE AUDIO' union 
		select '23', 'MUSICAL INSTRUMENTS' union 
		select '30', 'WASHER/DRYERS' union 
		select '31', 'REFRIGERATION' union 
		select '32', 'COOKING' union 
		select '33', 'FLOORCARE' union 
		select '40', 'BUSINESS EQUIPMENT' union 
		select '41', 'TELECOMMUNICATIONS' union 
		select '50', 'KITCHEN APPLIANCES' union 
		select '51', 'LIGHTING & COOLING APPLIANCES' union 
		select '52', 'DIY & LARGE MISCELLANEOUS' union 
		select '53', 'PERSONAL CARE & SMALL MISC.' union 
		select '54', 'VEHICLES' union 
		select '59', 'COOKER SPARES' union 
		select '60', 'ACCESSORIES' union 
		select '71', 'SPARE PARTS' union 
		select '74', 'SPARE PARTS' union 
		select '80', 'KITCHENWARE' union 
		select '81', 'CROCKERY' union 
		select '82', 'GLASSWARE' union 
		select '83', 'CUTLERY' union 
		select '84', 'COOKWARE' union 
		select '85', 'BATHROOM ACCESSORIES' union 
		select '86', 'COOLERS' union 
		select '87', 'PROPANE PRODUCTS' union 
		select '89', 'KITCHEN ACCESSORIES' union 
		select '7L', 'LABOUR' union 
		select 'BA', 'BED - METAL' union 
		select 'BB', 'BED - WOODEN' union 
		select 'BC', 'PULL OUT BED - WOODEN' union 
		select 'BD', 'PULL OUT BED - METAL' union 
		select 'BE', 'DOUBLE DECKER - METAL' union 
		select 'BF', 'DOUBLE DECKER - WOODEN' union 
		select 'BG', 'TRIPLE DECKER - METAL' union 
		select 'BH', 'TRIPLE DECKER - WOODEN' union 
		select 'BJ', 'SINGLE/DOUBLE DECKER - METAL' union 
		select 'BK', 'BABY COTS' union 
		select 'BL', 'MATTRESS - FOAM' union 
		select 'BM', 'MATTRESS - SPRING' union 
		select 'BN', 'BASE (DIVAN)' union 
		select 'BP', 'HEADBOARD' union 
		select 'BQ', 'DIVAN SET SINGLE' union 
		select 'BR', 'DIVAN SET DOUBLE OR LARGER' union 
		select 'BS', 'PILLOWS' union 
		select 'BT', 'BOLSTERS' union 
		select 'BV', 'COMFORTERS, QUILTS, SHEETS, ETC.' union 
		select 'BW', 'BED/STUDY DESK' union 
		select 'CA', '3 DOOR BEDROOM SUITE - POLYESTER' union 
		select 'CB', '3 DOOR BEDROOM SUITE - WOODEN' union 
		select 'CC', '2 DOOR ROBE' union 
		select 'CD', '3 DOOR ROBE' union 
		select 'CE', '4 DOOR BEDROOM SUITE - POLYESTER' union 
		select 'CF', '4 DOOR BEDROOM SUITE - WOODEN' union 
		select 'CG', '4 DOOR ROBE' union 
		select 'CH', '5 DOOR BEDROOM SUITE - MIXED' union 
		select 'CJ', 'WARDROBE - COMBI' union 
		select 'CK', 'WARDROBE WITH TOP BOXES' union 
		select 'CL', 'CHESTS' union 
		select 'CM', 'BEDSIDE CABINETS' union 
		select 'CN', 'DRESSING TABLES' union 
		select 'CP', 'DRESSING TABLE STOOLS' union 
		select 'CQ', 'MIRROR STANDS' union 
		select 'CR', 'CHILDRENS BEDROOM' union 
		select 'CS', 'OVERBED UNIT' union 
		select 'DA', '7 PIECE DINING SET - WOODEN' union 
		select 'DB', '9 PCE DINING SET' union 
		select 'DC', '11 PCE DINING SET' union 
		select 'DE', '7 PIECE DINING SET - METAL' union 
		select 'DF', 'TABLE + BENCH SET' union 
		select 'DG', '5 PIECE DINING SET - WOODEN' union 
		select 'DH', '5 PIECE DINING SET - METAL' union 
		select 'DK', 'DINING CHAIR ONLY' union 
		select 'DL', 'DINING TABLE ONLY' union 
		select 'DM', 'WALL UNIT - WOODEN & BUFFET HUTCH' union 
		select 'DN', 'WALL UNIT - METAL' union 
		select 'DP', 'SIDEBOARD' union 
		select 'DQ', 'CORNER WALL UNIT - WOODEN' union 
		select 'DR', 'KITCHEN CABINETS' union 
		select 'DS', 'ALTAR CABINETS' union 
		select 'DT', 'MODULAR CABINETS' union 
		select 'DV', 'CHAIR PADS' union 
		select 'FA', 'RUGS - LARGE' union 
		select 'FB', 'RUGS - MEDIUM' union 
		select 'FC', 'RUGS - SMALL' union 
		select 'FD', 'RUGS - SMALL IRREGULAR' union 
		select 'FE', 'ROLL STOCK CARPET - HARDBACK' union 
		select 'FG', 'ROLL STOCK CARPET - FOAMBACK' union 
		select 'FH', 'VINYL FLOORING' union 
		select 'FJ', 'TILES' union 
		select 'GA', 'ARMCHAIRS GARDEN' union 
		select 'GB', 'RECLINING BEDS GARDEN' union 
		select 'GC', 'CHAIRS GARDEN' union 
		select 'GD', 'TABLES GARDEN' union 
		select 'GE', 'GARDEN SWING' union 
		select 'GF', 'PATIO SET' union 
		select 'GT', 'PATIO CART' union 
		select 'LA', 'SOFA SET - WOODEN FRAME' union 
		select 'LB', 'SOFA SET - PVC' union 
		select 'LC', 'SOFA SET - PVC/FABRIC' union 
		select 'LD', 'SOFA SET - CANE' union 
		select 'LE', 'SOFA SET - METAL' union 
		select 'LF', 'SOFA SET - CARVED - PLAIN' union 
		select 'LG', 'ARMCHAIR' union 
		select 'LH', 'SOFA SET - FULL FABRIC' union 
		select 'LJ', 'FOOTSTOOL' union 
		select 'LK', 'SOFA SET - HALF LEATHER' union 
		select 'LL', 'SOFA SET - FULL LEATHER' union 
		select 'LM', '3 SEATER SOFA ONLY' union 
		select 'LN', '2 SEATER SOFA' union 
		select 'LP', 'RECLINER CHAIR' union 
		select 'LQ', 'SOFA FUTON BED' union 
		select 'LR', 'CORNER GROUP' union 
		select 'LS', 'TV/ROCKER CHAIRS' union 
		select 'LT', 'CUSHION COVERS - STANDARD' union 
		select 'LV', 'CUSHION INTERIORS - STANDARD' union 
		select 'LW', 'COMPLETE CUSHIONS' union 
		select 'MA', 'STUDY DESK' union 
		select 'MB', 'BOOKSHELF - SINGLE' union 
		select 'MC', 'BOOKSHELF - DOUBLE' union 
		select 'MD', 'FILING RACKS' union 
		select 'ME', 'WRITING DESKS' union 
		select 'MF', 'OFFICE CHAIRS' union 
		select 'MG', 'COMPUTER TABLES' union 
		select 'MH', 'OIL PAINTINGS' union 
		select 'MJ', 'PICTURE PRINTS' union 
		select 'MK', 'MIRRORS' union 
		select 'ML', 'CLOCKS' union 
		select 'MM', 'LAMPS' union 
		select 'MN', 'PLANTS' union 
		select 'MP', 'FLOWER POTS' union 
		select 'MQ', 'POLYESTER' union 
		select 'MR', 'VINYL' union 
		select 'MS', 'FABRIC' union 
		select 'MT', 'BEAN BAGS' union 
		select 'MV', 'FOOTSTOOLS' union 
		select 'SA', 'SHOE RACKS' union 
		select 'SB', 'TV/VIDEO UNITS' union 
		select 'SC', 'PARTS – WOOD' union 
		select 'SD', 'PARTS – HARDWARE' union 
		select 'SE', 'OCCASIONAL TABLES' union 
		select 'SF', 'PARTS – MISC' union 
		select 'SH', 'SHELVING UNITS' union 
		select 'SJ', 'KIDS TABLES & CHAIRS' union 
		select 'SK', 'KIDS ACCESSORIES' union 
		select 'SL', 'RACKS & TROLLEYS' union 
		select 'SM', 'FOLDING CHAIRS' union 
		select 'SN', 'STOOLS' union 
		select 'SP', 'FOLDING TABLE' union 
		select 'XW', 'WARRANTIES'  


		update stockinfo set class = LEFT(LTRIM(itemno), 2), SubClass = LEFT(LTRIM(itemno), 3) where category in (select code from code where category in ('PCW', 'PCE'))

		update stockinfo set class = LEFT(LTRIM(itemno), 2), SubClass = LEFT(LTRIM(itemno), 2) where category in (select code from code where category in ('PCO', 'PCF'))
END


