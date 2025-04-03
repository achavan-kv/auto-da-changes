delete FROM dbo.Control
WHERE screen IN ('DeliveryScheduleDetails','DeliveryScheduling','ImmediateDelivery','OrdersForDelivery','ScheduleOverride','StaffMaintenance')
OR control IN ('menuImmediateDelivery','menuRePrintDeliveryNote','menuOrdersForDelivery','menuDelSchedule','menuPrintDelSched','menuTransportMaint','menuDeliveryNotification','menuTransportSchedule','menuAmendReprintPicklist',
'menuCancelCollectionNotes','menuScheduleChanges')


