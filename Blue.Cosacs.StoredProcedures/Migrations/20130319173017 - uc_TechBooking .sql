DELETE FROM Service.TechnicianBooking
WHERE RequestId IN (
SELECT RequestId FROM Service.TechnicianBooking
GROUP BY RequestId
HAVING COUNT(*) > 1)

ALTER TABLE Service.TechnicianBooking
ADD CONSTRAINT u_techBookingRequestId UNIQUE (RequestId)