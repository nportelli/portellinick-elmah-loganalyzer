﻿using ElmahLogAnalyzer.Core.Domain;
using ElmahLogAnalyzer.Core.Domain.Export;
using ElmahLogAnalyzer.Core.Infrastructure.Cache;
using ElmahLogAnalyzer.Core.Infrastructure.Configuration;
using ElmahLogAnalyzer.Core.Infrastructure.FileSystem;
using ElmahLogAnalyzer.Core.Infrastructure.Logging;
using ElmahLogAnalyzer.Core.Infrastructure.Settings;
using ElmahLogAnalyzer.Core.Infrastructure.Web;
using ElmahLogAnalyzer.Core.Integrations.HttpUserAgentSearch;
using ElmahLogAnalyzer.Core.Presentation;
using Ninject.Activation;
using Ninject.Modules;
using NLog;

namespace ElmahLogAnalyzer.Core.Infrastructure.Dependencies
{
	public class NinjectCoreModule : NinjectModule
	{
		public override void Load()
		{
			Bind<IErrorLogRepository>().To<ErrorLogRepository>()
				.InScope(context => DataSourceScopeController.KeepAlive);

			Bind<IErrorLogSource>().To<SqlServerErrorLogSource>()
				.When(context => DataSourceScopeController.Source == ErrorLogSourcesEnum.SqlServer)
				.InScope(context => DataSourceScopeController.KeepAlive)
				.WithConstructorArgument("connection", (context => DataSourceScopeController.Connection));

			Bind<IErrorLogSource>().To<SqlServerCompactErrorLogSource>()
				.When(context => DataSourceScopeController.Source == ErrorLogSourcesEnum.SqlServerCompact)
				.InScope(context => DataSourceScopeController.KeepAlive)
				.WithConstructorArgument("connection", (context => DataSourceScopeController.Connection));

			Bind<IErrorLogSource>().To<FileErrorLogSource>()
				.When(context => DataSourceScopeController.Source == ErrorLogSourcesEnum.Files)
				.InScope(context => DataSourceScopeController.KeepAlive)
				.WithConstructorArgument("connection", (context => DataSourceScopeController.Connection));

			Bind<IErrorLogFileParser>().To<ErrorLogFileParser>();
			Bind<IReportGenerator>().To<ReportGenerator>();
			Bind<IErrorLogExporter>().To<ErrorLogExporter>();
			Bind<ISettingsManager>().To<SettingsManager>();
			Bind<ICacheHelper>().To<CacheHelper>();
			Bind<IWebRequestHelper>().To<WebRequestHelper>();
			Bind<IClientInformationResolver>().To<ClientInformationResolver>();
			Bind<IUrlNavigationLauncher>().To<UrlNavigationLauncher>();
			Bind<IErrorLogDownloader>().To<ErrorLogDownloader>();
			Bind<IUrlPing>().To<UrlPing>();
			Bind<ICsvParser>().To<CsvParser>();
			Bind<IHttpUserAgentSearchLauncherFactory>().To<HttpUserAgentSearchLauncherFactory>();
			Bind<IFileSystemHelper>().To<FileSystemHelper>();
			Bind<IWebServerConnectionsHelper>().To<WebServerConnectionsHelper>();
			Bind<IDatabaseCreator>().To<SqlCeDatabaseCreator>();
			Bind<SearchPresenter>().ToSelf();
			Bind<ReportPresenter>().ToSelf();
			Bind<SettingsPresenter>().ToSelf();
			Bind<ConnectToWebServerPresenter>().ToSelf();
			Bind<ConnectToDatabasePresenter>().ToSelf();
			Bind<ExportPresenter>().ToSelf();
			Bind<ILog>().ToMethod(GetLogger);
		}

		private static ILog GetLogger(IContext context)
		{
			var loggerName = "Default";

			if (context.Request.Target != null)
			{
				loggerName = context.Request.Target.Member.DeclaringType.FullName;
			}

			var logger = LogManager.GetLogger(loggerName);

			return new Log(logger);
		}
	}
}
