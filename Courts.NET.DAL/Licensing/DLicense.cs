using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Sql;
using System.Data.SqlClient;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data.Common;
using System.Data;
using Blue.Licensing.Common;

namespace STL.DAL.Licensing
{
	public class DLicense
	{
        const int intervalMultiplier = 3;

		#region Messages

		const string RegisterSuccess = "Success. The machine has been registered.";
		static string RegisterFailed = "The license limit has been reached." + Environment.NewLine + "Please Contact your system administrator.";
		const string UnRegisterSuccess = "Success. The machine has been unregistered.";
		const string UnRegisterFailed = "The machine has not been unregistered.";
		
		#endregion
		
		/// <summary>
		/// Manually set the machine limit for testing
		/// </summary>
		public int? MachineLimit;
		/// <summary>
		/// Manually set the register interval for testing
		/// </summary>
		public Interval RegisterInterval;
 
		/*
		 * Sql was used here instead of a stored procedure in order to prevent
		 * users from simply replacing the stored procedure, which would allow them to use 
		 * more machines than their license permits
		 */

		public DLicense()
		{
			this.db = DatabaseFactory.CreateDatabase();
		}

		Database db;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="machine"></param>
        /// <param name="license"></param>
        /// <returns></returns>
		public RegistrationResult Register(Machine machine, License license)
		{
            try
            {
                new LicenseValidationService().Validate(license.ToBase64());

                if (machine.CustomerApplicationId.ToUpper() != license.Payload.CustomerApplicationId.ToUpper())
                    throw new LicenseValidationException("Application Id does not match License Application Id ( " + license.Payload.CustomerApplicationId.ToUpper() + " )");
            }
            catch (LicenseValidationException ex)
            {
                return new RegistrationResult
                (
                    machine,
                    RegisterFailed,
                    0,
                    RegisterInterval = GetRegisterInterval(license)
                ) { StatusMsg = ex.Message };
            }

			machine.Registered = false;

			DbCommand cmd = db.GetSqlStringCommand(
			@"
				DISABLE TRIGGER ALL ON [LicenseAudit]

				--DELETE FROM [LicenseAudit]
				--WHERE Timestamp < DATEADD(ms, @Interval, GETDATE())

				DECLARE @CurrentMachineCount int
				SET @CurrentMachineCount = (SELECT COUNT(DISTINCT MacAddress) FROM [LicenseAudit] WHERE MacAddress <> @MacAddress)

				IF @CurrentMachineCount < @MachineLimit
				BEGIN
					INSERT INTO [LicenseAudit]
					([MacAddress] ,[Timestamp], [UserId], [MachineName])
					VALUES(@MacAddress, GETDATE(), @UserId, @MachineName)
				END

				SELECT @CurrentMachineCount");
				  
			db.AddInParameter(cmd, "@MacAddress", DbType.String, machine.MachineId);
            db.AddInParameter(cmd, "@Interval", DbType.Int32, (GetRegisterInterval(license).MilliSeconds * intervalMultiplier) * -1);
			db.AddInParameter(cmd, "@MachineLimit", DbType.Int32, GetMachineLimit(license));
			db.AddInParameter(cmd, "@UserId", DbType.Int32, machine.UserId);
            db.AddInParameter(cmd, "@MachineName", DbType.String, machine.MachineName);

			object currentMachineCount = db.ExecuteScalar(cmd);
			if (currentMachineCount == DBNull.Value)
				currentMachineCount = 0;

			machine.Registered = (int)currentMachineCount < GetMachineLimit(license);

			return new RegistrationResult
			(
				machine,
				machine.Registered ? RegisterSuccess
									: RegisterFailed,
				(int)currentMachineCount,
				RegisterInterval = GetRegisterInterval(license)
			);
		}

		public RegistrationResult UnRegister(Machine machine)
		{
			DbCommand cmd = db.GetSqlStringCommand(
			@"DELETE FROM [LicenseAudit] 
				WHERE [MacAddress] = @MacAddress");

			db.AddInParameter(cmd, "@MacAddress", DbType.String, machine.MachineId);

			machine.Registered = !(db.ExecuteNonQuery(cmd) > 0);

			return new RegistrationResult
			(
				machine,
				!machine.Registered ? UnRegisterSuccess
									: UnRegisterFailed
			);
		}

		public void ClearLicenseAudit()
		{
			DbCommand cmd = db.GetSqlStringCommand(
			@"DELETE FROM [LicenseAudit]");

			db.ExecuteNonQuery(cmd);
		}

        public int GetMachineLimit(License license)
		{
			if (MachineLimit.HasValue)
				return MachineLimit.Value;

            return license.Payload.MaximumActiveCount;
		}

		public Interval GetRegisterInterval(License license)
		{
			if (RegisterInterval != null)
				return RegisterInterval;

			return new Interval(license.Payload.RegistrationIntervalTimeSpan);
		}

		public License GetLicense()
		{
            DbCommand cmd = db.GetSqlStringCommand(
            @"SELECT [Value] FROM [CountryMaintenance] 
				WHERE [CodeName] = @CodeName");

            db.AddInParameter(cmd, "@CodeName", DbType.String, STL.Common.CountryParameterNames.EposLicense);

            object licenseAsBase64 = db.ExecuteScalar(cmd);
            if (licenseAsBase64 == null)
                throw new LicenseValidationException("License Not Found.");

            return new LicenseValidationService().Validate((string)licenseAsBase64);
		}
}
}
