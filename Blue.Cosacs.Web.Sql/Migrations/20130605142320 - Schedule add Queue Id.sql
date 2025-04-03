alter table Scheduling.Schedule
drop column MessageType

alter table Scheduling.Schedule
add QueueId int
