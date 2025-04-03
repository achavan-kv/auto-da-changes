IF NOT EXISTS (SELECT * FROM   sys.columns WHERE  object_id = OBJECT_ID(N'[CLAmortizationSchedule]') AND name = 'adminfee')
BEGIN
/*Update CLAmortizationSchedule Table with  adminfee column */
ALTER TABLE CLAmortizationSchedule
ADD adminfee decimal(15,2);
END
