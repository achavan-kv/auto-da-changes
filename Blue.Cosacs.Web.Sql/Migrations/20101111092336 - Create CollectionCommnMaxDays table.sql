-- Table to hold MaxDays Parameter at EOD

Create TABLE CollectionCommnMaxDays 
(
	RunNo INT,
	MaxDays int
)

insert into CollectionCommnMaxDays (RunNo,MaxDays)
select MAX(RunNo),MAX(Value) 
from CountryMaintenance ,InterfaceControl
where codename='MaxDaysCallerComm'
and interface='COLLCOMMNS'	

