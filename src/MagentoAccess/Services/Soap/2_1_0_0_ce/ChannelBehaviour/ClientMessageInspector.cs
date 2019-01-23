using System;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Text.RegularExpressions;
using System.Xml;
using MagentoAccess.Misc;
using Netco.Logging;

namespace MagentoAccess.Services.Soap._2_1_0_0_ce.ChannelBehaviour
{
	internal class ClientMessageInspector : IClientMessageInspector
	{
		private const string StandartNamespaceStub = "hereshouldbeyourmagentostoreurl.com";
		private const string StandartNamespaceStubWithProtocol = "http://" + StandartNamespaceStub;
		public string AccessToken;
		public bool LogRawMessages { get; set; } = false;
		private readonly object lockObject = new object();
		private string replacedUrl;
		private const string RegexSplitterString = "/soap/[a-zA-Z]*?services=";
		private Regex urlSplitterRegex = new Regex( RegexSplitterString, RegexOptions.Compiled );

		public object BeforeSendRequest( ref Message request, IClientChannel channel )
		{
			//trace
			var logSucceed = true;// this.TryToLogMessage( ref request );

			//legacy behaviour
			HttpRequestMessageProperty httpRequestMessage;
			object httpRequestMessageObject;
			if( request.Properties.TryGetValue( HttpRequestMessageProperty.Name, out httpRequestMessageObject ) )
			{
				httpRequestMessage = httpRequestMessageObject as HttpRequestMessageProperty;
				if( string.IsNullOrEmpty( httpRequestMessage.Headers[ "Accept-Encoding" ] ) )
					httpRequestMessage.Headers.Remove( "Accept-Encoding" );
			}
			else
			{
				httpRequestMessage = new HttpRequestMessageProperty();
				httpRequestMessage.Headers.Add( "Accept-Encoding", "" );
				request.Properties.Add( HttpRequestMessageProperty.Name, httpRequestMessage );
			}

			//Auth magento 2.0
			if( httpRequestMessage.Headers.AllKeys.Contains( "Authorization" ) )
				httpRequestMessage.Headers[ "Authorization" ] = "Bearer " + this.AccessToken;
			else
				httpRequestMessage.Headers.Add( "Authorization", "Bearer " + this.AccessToken );

			//Crutch for magento 2.0
			var newValue = channel.RemoteAddress.Uri.ToString();
			//var protocolUrlPartIndex = newValue.IndexOf( "//", StringComparison.Ordinal ) + 2;
			//var soapUrlPartIndex = newValue.IndexOf( "/soap/default?services=", StringComparison.Ordinal );
			var urlPartIndex = this.urlSplitterRegex.Match( newValue );
			var soapUrlPartIndex = urlPartIndex.Index;
			var storeUrlWithoutProtocol = newValue.Substring( 0, soapUrlPartIndex ).Trim();

			lock( this.lockObject )
			{
				if( string.IsNullOrWhiteSpace( this.replacedUrl ) )
					this.replacedUrl = storeUrlWithoutProtocol;
			}
			this.ReplaceInMessageBody( ref request, StandartNamespaceStubWithProtocol, storeUrlWithoutProtocol, !logSucceed);

			return null;
		}

		private void ReplaceInMessageBody( ref Message request, string magentoCe, string newValue, bool logOriginalMessage )
		{
			using( var reader = request.GetReaderAtBodyContents() )
			{
				var content = reader.ReadOuterXml();

				var contentWithChanges = content.Replace( magentoCe, newValue );
				var xmlReader = XmlReader.Create( this.GenerateStreamFromString( contentWithChanges ) );
				var newMessage = Message.CreateMessage( request.Version, null, xmlReader );
				newMessage.Properties.CopyProperties( request.Properties );
				if( request.Headers != null && request.Headers.Count > 0 )
					newMessage.Headers.CopyHeaderFrom( request, 0 );
				request = newMessage;

				try
				{
					//if( logOriginalMessage && this.LogRawMessages )
					//{
					//	var logStr = content;
					//	logStr += request.Properties.Select( x => $"{{{x.Key};{x.Value.ToString()}}}" ).Aggregate( "", ( acc, x ) => acc + x );
					//	logStr += request.Headers.Select( x => $"{{'{x.Actor};{x.Name.ToString()}}}" ).Aggregate( "", ( acc, x ) => acc + x );
					//	MagentoLogger.LogTraceRequestMessage( logStr );
					//}
				}
				catch
				{
				}
			}
		}

		public Stream GenerateStreamFromString( string s )
		{
			MemoryStream stream = new MemoryStream();
			StreamWriter writer = new StreamWriter( stream );
			writer.Write( s );
			writer.Flush();
			stream.Position = 0;
			return stream;
		}

		public void AfterReceiveReply( ref Message reply, object correlationState )
		{
			//trace
			var logSucceed = true;// this.TryToLogMessage( ref reply );

			var prop =
				reply.Properties[ HttpResponseMessageProperty.Name.ToString() ] as HttpResponseMessageProperty;

			if( prop != null )
			{
				// get the content type headers
				var contentType = prop.Headers[ "Content-Type" ];
			}

			this.ReplaceInMessageBody( ref reply, this.replacedUrl, StandartNamespaceStubWithProtocol, !logSucceed );
		}

		private bool TryToLogMessage( ref Message reply )
		{
			try
			{
				if( this.LogRawMessages )
				{
					var buffer = reply.CreateBufferedCopy( int.MaxValue );
					reply = buffer.CreateMessage();
					var originalMessage = buffer.CreateMessage();
					var messageSerialized = originalMessage.ToString();

					var httpPropertyName = HttpResponseMessageProperty.Name.ToString();
					if( originalMessage.Properties.ContainsKey( httpPropertyName ) )
					{
						var property = originalMessage.Properties[ httpPropertyName ] as HttpResponseMessageProperty;
						if( property != null )
							messageSerialized = "HttpStatusCode: " + property.StatusCode.ToString() + ", message:" + messageSerialized;
					}
					MagentoLogger.LogTraceResponseMessage( messageSerialized );
					return true;
				}
			}
			catch
			{
			}
			return false;
		}
	}
}