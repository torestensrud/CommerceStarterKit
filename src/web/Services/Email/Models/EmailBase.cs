/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System;
using System.IO;
using System.Linq;
using System.Net.Mail;
using EPiServer;
using EPiServer.Core;
using EPiServer.Logging;
using EPiServer.ServiceLocation;
using OxxCommerceStarterKit.Web.Models.ViewModels.Email;

namespace OxxCommerceStarterKit.Web.Services.Email.Models
{
	/// <summary>
	/// Uses Postal http://aboutcode.net/postal/
	///  and PreMailer.Net https://github.com/milkshakesoftware/PreMailer.Net
	///  from NuGet packages
	/// </summary>
	public abstract class EmailBase : Postal.Email, INotificationSettings
	{
		public string To { get; set; }
		public string From { get; set; }
		public string Subject { get; set; }
		public string Header { get; set; }
		public string Footer { get; set; }
	}
}
