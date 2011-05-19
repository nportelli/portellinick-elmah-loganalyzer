﻿using System;
using Crepido.ElmahOfflineViewer.Core.Common;
using Crepido.ElmahOfflineViewer.Core.Domain;
using Crepido.ElmahOfflineViewer.Core.Infrastructure.Web;

namespace Crepido.ElmahOfflineViewer.Core.Presentation
{
	public class SelectServerPresenter
	{
		private readonly IUrlPing _urlPing;

		public SelectServerPresenter(ISelectServerView view, IUrlPing urlPing)
		{
			View = view;
			_urlPing = urlPing;

			RegisterEvents();
		}

		public ISelectServerView View { get; private set; }

		private void RegisterEvents()
		{
			View.OnConnectToServer += View_OnConnectToServer;
		}

		private void View_OnConnectToServer(object sender, EventArgs e)
		{
			View.ClearErrorMessage();
			
			if (!View.Url.HasValue())
			{
				View.DisplayErrorMessage("Invalid url");
				return;
			}

			try
			{
				var serverUrl = new Uri(View.Url);
				serverUrl = new ElmahUrlHelper().ResolveElmahRootUrl(serverUrl);
				View.Url = serverUrl.AbsoluteUri;

				var serverResponded = _urlPing.Ping(serverUrl);

				if (!serverResponded)
				{
					View.DisplayErrorMessage("Failed to connect to server");
					return;
				}

				View.CloseView();
			}
			catch (ArgumentException)
			{
				View.DisplayErrorMessage("Invalid url");
			}
			catch (Exception ex)
			{
				View.DisplayErrorMessage(ex.Message);
			}
		}
	}
}
