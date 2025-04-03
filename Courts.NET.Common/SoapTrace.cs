using System;
using System.IO;
using System.Web.Services.Protocols;
using System.Configuration;

namespace STL.Common
{
	/// <summary>
	/// This class provides a switchable mechanism for tracing the 
	/// SOAP exchange involved in web service calls. Useful to 
	/// debugging purposes.
	/// </summary>
	/// 
	public class TraceExtension : SoapExtension
	{
		Stream oldStream;
		Stream newStream;
		string filename;

		public override object GetInitializer(LogicalMethodInfo methodInfo,
			SoapExtensionAttribute attribute)
		{
			return ((TraceExtensionAttribute) attribute).Filename;
		}

		public override object GetInitializer(Type serviceType)
		{
			return typeof(TraceExtension);
		}

		public override void Initialize(object initializer)
		{
			filename = (string) initializer;
		}

		public override void ProcessMessage(SoapMessage message)
		{
			switch (message.Stage) 
			{

				case SoapMessageStage.BeforeSerialize:
					break;

				case SoapMessageStage.AfterSerialize:
					WriteOutput( message );
					break;

				case SoapMessageStage.BeforeDeserialize:
					WriteInput( message );
					break;

				case SoapMessageStage.AfterDeserialize:
					break;

				default:
					throw new Exception("invalid stage");
			}
		}

		public override Stream ChainStream( Stream stream )
		{
			oldStream = stream;
			newStream = new MemoryStream();
			return newStream;
		}

		public void WriteOutput( SoapMessage message )
		{
			newStream.Position = 0;
			FileStream fs = new FileStream(filename, FileMode.Append, FileAccess.Write);
			StreamWriter w = new StreamWriter(fs);
			w.WriteLine("---------------------------------- Response at " + DateTime.Now);
			w.Flush();
			Copy(newStream, fs);
			fs.Close();
			newStream.Position = 0;
			Copy(newStream, oldStream);
		}

		public void WriteInput( SoapMessage message )
		{
			Copy(oldStream, newStream);
			FileStream fs = new FileStream(filename, FileMode.Append, FileAccess.Write);
			StreamWriter w = new StreamWriter(fs);
			w.WriteLine("================================== Request at " + DateTime.Now);
			w.Flush();
			newStream.Position = 0;
			Copy(newStream, fs);
			fs.Close();
			newStream.Position = 0;
		}

		void Copy(Stream from, Stream to) 
		{

			TextReader reader = new StreamReader(from);
			TextWriter writer = new StreamWriter(to);
			writer.WriteLine(reader.ReadToEnd());
			writer.Flush();
		}
	}

	[AttributeUsage(AttributeTargets.Method)]
	public class TraceExtensionAttribute : SoapExtensionAttribute
	{
        private string filename = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + ConfigurationManager.AppSettings["soapTrace"];
		private int priority;

		public override Type ExtensionType
		{
			get
			{
				return typeof(TraceExtension);
			}
		}

		public override int Priority
		{
			get
			{
				return priority;
			}
			set
			{
				priority = value;
			}
		}

		public string Filename
		{
			get
			{
				return filename;
			}
			set
			{
				filename = value;
			}
		}
	}
}
