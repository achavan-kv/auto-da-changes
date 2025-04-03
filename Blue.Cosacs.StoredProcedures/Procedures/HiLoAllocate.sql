IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[HiLoAllocate]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[HiLoAllocate]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create procedure [dbo].[HiLoAllocate]
  @Sequence varchar(128),
  @CurrentHi int output,
  @MaxLo int output
as
begin
	-- allocates the next Hi number from a specific named @Sequence
	-- HiLo numbers work by allocating intervals of numbers (with count MaxLo) for safe offline allocation of IDs
	-- in the range [@CurrentHi..@CurrentHi + @MaxLo[
	begin tran
		update HiLo with (SERIALIZABLE, ROWLOCK)
		set @CurrentHi = NextHi,
			@MaxLo = MaxLo,
			NextHi += MaxLo
		where Sequence = @Sequence
		
		-- if the sequence does not exist yet, we initialize it:
		if @@rowcount = 0
		begin
			set @CurrentHi = 1
			set @MaxLo = 100
			
			insert HiLo ([Sequence], [NextHi], [MaxLo])
			values (@Sequence, @CurrentHi + @MaxLo, @MaxLo)
		end
	commit tran
	
	select @CurrentHi, @MaxLo
	
end
GO
